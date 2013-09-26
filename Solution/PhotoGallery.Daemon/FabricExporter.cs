using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Fabric.Clients.Cs;
using Fabric.Clients.Cs.Api;
using NHibernate;
using PhotoGallery.Domain;
using PhotoGallery.Infrastructure;
using PhotoGallery.Services.Account;
using PhotoGallery.Services.Account.Tools;

namespace PhotoGallery.Daemon {
	
	/*================================================================================================*/
	internal static class FabricExporter {

		public static bool StopThreads;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public static void StartDataProvThread(object pFabClient) {
			var sw = Stopwatch.StartNew();
			IFabricClient fab = (IFabricClient)pFabClient;
			LogDebug(fab, "StartDataProvThread");

			Func<ISession, IList<FabricArtifact>> getArtList = (s => s
				.QueryOver<FabricArtifact>()
				.Where(x => x.ArtifactId == null && x.Creator == null)
				.Take(10)
				.List()
			);

			Func<ISession, IList<FabricFactor>> getFacList = (s => s
				.QueryOver<FabricFactor>()
				.Where(x => x.FactorId == null && x.Creator == null)
				.Fetch(x => x.Primary).Eager
				.Fetch(x => x.Related).Eager
				.Take(10)
				.List()
			);

			SendAll(fab, getArtList, getFacList, 0);
			LogDebug(fab, "StartDataProvThread done: "+sw.Elapsed.TotalMilliseconds+"ms");
		}

		/*--------------------------------------------------------------------------------------------*/
		public static void StartUserThread(object pFabClient) {
			var sw = Stopwatch.StartNew();
			IFabricClient fab = (IFabricClient)pFabClient;
			FabricUser u;
			LogDebug(fab, "StartUserThread");

			using ( ISession s = FabricService.NewSession() ) {
				u = HomeService.GetCurrentUser(fab, s);
			}

			Func<ISession, IList<FabricArtifact>> getArtList = (s => s
				.QueryOver<FabricArtifact>()
				.Where(x => x.ArtifactId == null && x.Creator.Id == u.Id)
				.Take(10)
				.List()
			);

			Func<ISession, IList<FabricFactor>> getFacList = (s => s
				.QueryOver<FabricFactor>()
				.Where(x => x.FactorId == null && x.Creator.Id == u.Id)
				.Fetch(x => x.Primary).Eager
				.Fetch(x => x.Related).Eager
				.Take(20)
				.List()
			);

			SendAll(fab, getArtList, getFacList, 0);
			LogDebug(fab, "StartUserThread done: "+sw.Elapsed.TotalMilliseconds+"ms");
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private static void SendAll(IFabricClient pFab, Func<ISession, IList<FabricArtifact>> pGetArts,
											Func<ISession, IList<FabricFactor>> pGetFacs, int pLoopI) {
			if ( pLoopI > 4 ) {
				LogDebug(pFab, "SendAll Kill. Too many loops: "+pLoopI);
				return;
			}
			
			
			bool restart;
			while ( LoadAndSendArtifacts(pFab, pGetArts) ) {}
			while ( LoadAndSendFactors(pFab, pGetFacs, out restart) ) {}

			if ( restart ) {
				Thread.Sleep(2000);
				LogDebug(pFab, "SendAll Restart!");
				SendAll(pFab, pGetArts, pGetFacs, pLoopI+1);
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		private static bool LoadAndSendArtifacts(IFabricClient pFab, 
													Func<ISession, IList<FabricArtifact>> pGetArts) {
			IList<FabricArtifact> artList;

			using ( ISession s = FabricService.NewSession() ) {
				artList = pGetArts(s);
			}

			LogDebug(pFab, "SendAll Artifacts: "+artList.Count);

			if ( artList.Count == 0 ) {
				return false;
			}

			using ( ISession s = FabricService.NewSession() ) {
				using ( ITransaction tx = s.BeginTransaction() ) {
					SendArtifacts(pFab, s, artList);
					tx.Commit();
				}
			}

			return true;
		}
		
		/*--------------------------------------------------------------------------------------------*/
		private static bool LoadAndSendFactors(IFabricClient pFab, 
								Func<ISession, IList<FabricFactor>> pGetFacs, out bool pRestartAll) {
			IList<FabricFactor> facList;
			var saveFacList = new List<FabricFactor>();

			using ( ISession s = FabricService.NewSession() ) {
				facList = pGetFacs(s);
			}

			foreach ( FabricFactor ff in facList ) {
				if ( ff.Primary != null && ff.Primary.ArtifactId == null ) {
					continue;
				}
				
				if ( ff.Related != null && ff.Related.ArtifactId == null ) {
					continue;
				}

				saveFacList.Add(ff);
			}

			int skip = facList.Count-saveFacList.Count;
			facList.Clear();
			pRestartAll = (skip > 0);
			LogDebug(pFab, "SendAll Factors: "+saveFacList.Count+" (+ skip "+skip+")");

			if ( saveFacList.Count == 0 ) {
				return false;
			}

			using ( ISession s = FabricService.NewSession() ) {
				using ( ITransaction tx = s.BeginTransaction() ) {
					SendFactors(pFab, s, saveFacList);
					tx.Commit();
				}
			}

			return true;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private static void SendArtifacts(IFabricClient pFab, ISession pSess, 
																IEnumerable<FabricArtifact> pArtList) {
			foreach ( FabricArtifact art in pArtList ) {
				if ( StopThreads ) {
					LogDebug(pFab, "SendArtifacts Stop!");
					return;
				}

				try {
					FabInstance fi = pFab.Services.Modify.AddInstance
						.Post(art.Name, art.Disamb, art.Note).FirstDataItem();
					LogDebug(pFab, "SendArtifacts Art: "+art.Id+" => "+fi.ArtifactId+" ("+art.Name+")");
					art.ArtifactId = fi.ArtifactId;
					pSess.Update(art);
				}
				catch ( Exception e ) {
					LogError(pFab, "SendArtifacts Err: "+art.Id+", "+e.Message, e);
				}
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		private static void SendFactors(IFabricClient pFab, ISession pSess,
																IEnumerable<FabricFactor> pFacList) {
			var batch = new List<FabBatchNewFactor>();
			var batchMap = new Dictionary<long, FabricFactor>();
			var fakeBatchRes = new List<FabBatchResult>();

			foreach ( FabricFactor fac in pFacList ) {
				FabBatchNewFactor fb = FabricFactorBuilder.DbFactorToBatchFactor(fac);
				batch.Add(fb);
				batchMap.Add(fb.BatchId, fac);

				var fbr = new FabBatchResult { BatchId = fb.BatchId, ResultId = 10000+fb.BatchId };
				fakeBatchRes.Add(fbr);
			}

			if ( StopThreads ) {
				LogDebug(pFab, "SendFactorsStop!");
				return;
			}

			try {
				IList<FabBatchResult> batchRes = pFab.Services.Modify.AddFactors
					.Post(batch.ToArray()).Data;

				foreach ( FabBatchResult fbr in batchRes ) {
					LogDebug(pFab, "SendFactors Fac: "+fbr.BatchId+" => "+fbr.ResultId);

					FabricFactor ff = batchMap[fbr.BatchId];
					ff.FactorId = fbr.ResultId;
					pSess.Update(ff);
				}
				
			}
			catch ( Exception e ) {
				LogError(pFab, "SendFactors Err: "+e.Message, e);
			}
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private static string GetLogPrefix(IFabricClient pFab) {
			return "FabBg["+(pFab.UseDataProviderPerson ? "DP" : "User")+"] ";
		}
		
		/*--------------------------------------------------------------------------------------------*/
		private static void LogDebug(IFabricClient pFab, string pText) {
			Log.Debug(GetLogPrefix(pFab)+pText);
		}

		/*--------------------------------------------------------------------------------------------*/
		private static void LogError(IFabricClient pFab, string pText, Exception pEx) {
			Log.Error(GetLogPrefix(pFab)+pText, pEx);
		}

	}

}