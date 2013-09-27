using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Fabric.Clients.Cs;
using Fabric.Clients.Cs.Api;
using NHibernate;
using PhotoGallery.Domain;
using PhotoGallery.Infrastructure;
using PhotoGallery.Services.Account.Tools;

namespace PhotoGallery.Daemon.Fabric {
	
	/*================================================================================================*/
	public class Exporter {

		public static bool StopThreads;

		private readonly ExporterData vData;
		private readonly IFabricClient vFab;
		private readonly FabricUser vUser;
		private readonly Stopwatch vTimer;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public Exporter(ExporterData pData, IFabricClient pFab, FabricUser pUser) {
			vData = pData;
			vFab = pFab;
			vUser = pUser;
			vTimer = new Stopwatch();
		}
		
		/*--------------------------------------------------------------------------------------------*/
		public void SendAll(int pLoopI) {
			vTimer.Start();
			LogDebug("SendAll ("+pLoopI+")");

			if ( pLoopI > 4 ) {
				LogDebug("SendAll kill (too many loops)");
				return;
			}

			bool restart;
			while ( LoadAndSendArtifacts() ) { }
			while ( LoadAndSendFactors(out restart) ) { }

			if ( restart ) {
				Thread.Sleep(2000);
				LogDebug("SendAll restart");
				SendAll(pLoopI+1);
			}

			LogDebug("SendAll done");
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private bool LoadAndSendArtifacts() {
			IList<FabricArtifact> artList;

			using ( ISession s = vData.SessProv.OpenSession() ) {
				artList = vData.Query.GetFabricArtifacts(s, vUser);
			}

			LogDebug("SendAllArtifacts: "+artList.Count);

			if ( artList.Count == 0 ) {
				return false;
			}

			using ( ISession s = vData.SessProv.OpenSession() ) {
				using ( ITransaction tx = s.BeginTransaction() ) {
					SendArtifacts(s, artList);
					tx.Commit();
				}
			}

			return true;
		}
		
		/*--------------------------------------------------------------------------------------------*/
		private bool LoadAndSendFactors(out bool pRestartAll) {
			IList<FabricFactor> facList;
			var saveFacList = new List<FabricFactor>();

			using ( ISession s = vData.SessProv.OpenSession() ) {
				facList = vData.Query.GetFabricFactors(s, vUser);
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
			LogDebug("SendAllFactors: "+saveFacList.Count+" (+ skip "+skip+")");

			if ( saveFacList.Count == 0 ) {
				return false;
			}

			using ( ISession s = vData.SessProv.OpenSession() ) {
				using ( ITransaction tx = s.BeginTransaction() ) {
					SendFactors(s, saveFacList);
					tx.Commit();
				}
			}

			return true;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private void SendArtifacts(ISession pSess, IEnumerable<FabricArtifact> pArtList) {
			foreach ( FabricArtifact art in pArtList ) {
				if ( StopThreads ) {
					LogDebug("SendArtifacts Stop!");
					return;
				}

				try {
					FabInstance fi = vFab.Services.Modify.AddInstance
						.Post(art.Name, art.Disamb, art.Note).FirstDataItem();
					LogDebug("SendArtifacts Art: "+art.Id+" => "+fi.ArtifactId+" ("+art.Name+")");
					art.ArtifactId = fi.ArtifactId;
					pSess.Update(art);
				}
				catch ( Exception e ) {
					LogError("SendArtifacts Err: "+art.Id+", "+e.Message, e);
				}
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		private void SendFactors(ISession pSess, IEnumerable<FabricFactor> pFacList) {
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
				LogDebug("SendFactorsStop!");
				return;
			}

			try {
				IList<FabBatchResult> batchRes = vFab.Services.Modify.AddFactors
					.Post(batch.ToArray()).Data;

				foreach ( FabBatchResult fbr in batchRes ) {
					LogDebug("SendFactors Fac: "+fbr.BatchId+" => "+fbr.ResultId);

					FabricFactor ff = batchMap[fbr.BatchId];
					ff.FactorId = fbr.ResultId;
					pSess.Update(ff);
				}
				
			}
			catch ( Exception e ) {
				LogError("SendFactors Err: "+e.Message, e);
			}
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private void LogDebug(string pText) {
			Log.Debug(GetLogPrefix()+pText+GetLogTime());
		}

		/*--------------------------------------------------------------------------------------------*/
		private void LogError(string pText, Exception pEx) {
			Log.Error(GetLogPrefix()+pText+GetLogTime(), pEx);
		}
		
		/*--------------------------------------------------------------------------------------------*/
		private string GetLogPrefix() {
			string s = (vFab.UseDataProviderPerson ? "DataProv" : "User-"+vFab.PersonSession.SessionId);
			return "Export["+s+"] ";
		}

		/*--------------------------------------------------------------------------------------------*/
		private string GetLogTime() {
			return " ("+vTimer.Elapsed.TotalSeconds.ToString("0.000")+" sec)";
		}

	}

}