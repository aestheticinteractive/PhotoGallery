using System;
using System.Collections.Generic;
using System.Threading;
using Fabric.Clients.Cs;
using Fabric.Clients.Cs.Api;
using NHibernate;
using PhotoGallery.Domain;
using PhotoGallery.Infrastructure;
using PhotoGallery.Services.Account.Tools;

namespace PhotoGallery.Services.Fabric {
	
	/*================================================================================================*/
	public static class FabricService {

		private static IFabricClient DataProvClient;
		private static bool StopThreads;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public static void SetDataProvClient(IFabricClient pFab) {
			DataProvClient = pFab;
			DataProvClient.UseDataProviderPerson = true;
			SetupClientLogger(pFab);

			CheckForNewTasks();
		}

		/*--------------------------------------------------------------------------------------------*/
		internal static void SetupClientLogger(IFabricClient pFab) {
			pFab.Config.Logger = new LogFabric();
		}

		/*--------------------------------------------------------------------------------------------*/
		public static void CheckForNewTasks(IFabricClient pUserFabClient=null) {
			LogDebug(DataProvClient, "CheckForNewTasks");
			var t = new Thread(StartDataProvThread);
			t.Start(DataProvClient);

			if ( pUserFabClient != null ) {
				LogDebug(pUserFabClient, "CheckForNewTasks");
				//t = new Thread(StartUserThread);
				//t.Start(pFab);
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		public static void StopAllThreads() {
			StopThreads = true;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private static void StartDataProvThread(object pFabClient) {
			IFabricClient fab = (IFabricClient)pFabClient;

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

			SendAll(fab, getArtList, getFacList);
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private static void SendAll(IFabricClient pFab, Func<ISession, IList<FabricArtifact>> pGetArts,
													Func<ISession, IList<FabricFactor>> pGetFacs) {
			bool restart;

			while ( LoadAndSendArtifacts(pFab, pGetArts) ) {
				//continue...
			}
			
			while ( LoadAndSendFactors(pFab, pGetFacs, out restart) ) {
				if ( restart ) {
					SendAll(pFab, pGetArts, pGetFacs);
					return;
				}
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		private static bool LoadAndSendArtifacts(IFabricClient pFab, 
													Func<ISession, IList<FabricArtifact>> pGetArts) {
			IList<FabricArtifact> artList;

			using ( ISession s = BaseService.NewSession() ) {
				artList = pGetArts(s);
			}

			LogDebug(pFab, "SendAll Artifacts: "+artList.Count);

			if ( artList.Count == 0 ) {
				return false;
			}

			using ( ISession s = BaseService.NewSession() ) {
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
			int skip = 0;

			using ( ISession s = BaseService.NewSession() ) {
				facList = pGetFacs(s);
			}

			foreach ( FabricFactor ff in facList ) {
				if ( ff.Primary != null && ff.Primary.ArtifactId == null ) {
					skip++;
					facList.Remove(ff);
				}
				else if ( ff.Related != null && ff.Related.ArtifactId == null ) {
					skip++;
					facList.Remove(ff);
				}
			}

			LogDebug(pFab, "SendAll Factors: "+facList.Count+" (skip "+skip+")");
			pRestartAll = (skip > 0);

			if ( facList.Count == 0 ) {
				return false;
			}

			using ( ISession s = BaseService.NewSession() ) {
				using ( ITransaction tx = s.BeginTransaction() ) {
					SendFactors(pFab, s, facList);
					tx.Commit();
				}
			}

			return true;
		}

		/*--------------------------------------------------------------------------------------------*/
		private static void SendArtifacts(IFabricClient pFab, ISession pSess, 
																IEnumerable<FabricArtifact> pArtList) {
			foreach ( FabricArtifact art in pArtList ) {
				if ( StopThreads ) {
					LogDebug(pFab, "SendArtifacts Stop!");
					return;
				}

				try {
					var fi = new FabInstance { ArtifactId = 10000+art.Id };
					//FabInstance fi = pFab.Services.Modify.AddInstance
					//	.Post(art.Name, art.Disamb, art.Note).FirstDataItem();

					LogDebug(pFab, "SendArtifacts Art: "+art.Id+" => "+fi.ArtifactId+" ("+art.Name+")");
					art.ArtifactId = fi.ArtifactId;
					pSess.Update(art);
				}
				catch ( Exception e ) {
					LogDebug(pFab, "SendArtifacts Err: "+art.Id+", "+e.Message);
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
				IList<FabBatchResult> batchRes = fakeBatchRes;
				//IList<FabBatchResult> batchRes = pFab.Services.Modify.AddFactors
				//	.Post(batch.ToArray()).Data;

				foreach ( FabBatchResult fbr in batchRes ) {
					LogDebug(pFab, "SendFactors Fac: "+fbr.BatchId+" => "+fbr.ResultId);

					FabricFactor ff = batchMap[fbr.BatchId];
					ff.FactorId = fbr.ResultId;
					pSess.Update(ff);
				}
				
			}
			catch ( Exception e ) {
				LogDebug(pFab, "SendFactors Err: "+e.Message);
			}
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private static void LogDebug(IFabricClient pFab, string pText) {
			Log.Debug("[Fabric Bg "+(pFab.UseDataProviderPerson ? "DP" : "User")+"] "+pText);
		}

	}

}