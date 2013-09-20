using System;
using System.Collections.Generic;
using System.Threading;
using Fabric.Clients.Cs;
using Fabric.Clients.Cs.Api;
using NHibernate;
using PhotoGallery.Domain;
using PhotoGallery.Infrastructure;

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
			var t = new Thread(StartDataProvThread);
			t.Start(DataProvClient);

			if ( pUserFabClient != null ) {
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
				.Take(10)
				.List()
			);

			SendAll(fab, getArtList, getFacList);
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private static void SendAll(IFabricClient pFab, 
													Func<ISession, IList<FabricArtifact>> pGetArtList,
													Func<ISession, IList<FabricFactor>> pGetFacList) {
			while ( true ) {
				IList<FabricArtifact> artList;

				using ( ISession s = BaseService.NewSession() ) {
					artList = pGetArtList(s);
				}

				Log.Debug("SendAll Artifacts: "+artList.Count);

				if ( artList.Count == 0 ) {
					break;
				}

				using ( ISession s = BaseService.NewSession() ) {
					using ( ITransaction tx = s.BeginTransaction() ) {
						SendArtifacts(pFab, s, artList);
						tx.Commit();
					}
				}
			}

			while ( true ) {
				IList<FabricFactor> facList;

				using ( ISession s = BaseService.NewSession() ) {
					facList = pGetFacList(s);
				}

				Log.Debug("SendAll Factors: "+facList.Count);

				if ( facList.Count == 0 ) {
					break;
				}

				using ( ISession s = BaseService.NewSession() ) {
					using ( ITransaction tx = s.BeginTransaction() ) {
						SendFactors(pFab, s, facList);
						tx.Commit();
					}
				}
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		private static void SendArtifacts(IFabricClient pFab, ISession pSess, 
																IEnumerable<FabricArtifact> pArtList) {
			foreach ( FabricArtifact art in pArtList ) {
				if ( StopThreads ) {
					Log.Debug("SendArtifacts Stop!");
					return;
				}

				try {
					var fi = new FabInstance { ArtifactId = 10000+art.Id };
					//FabInstance fi = pFab.Services.Modify.AddInstance
					//	.Post(art.Name, art.Disamb, art.Note).FirstDataItem();

					Log.Debug("SendArtifacts Art: "+art.Id+" => "+fi.ArtifactId+" ("+art.Name+")");
					art.ArtifactId = fi.ArtifactId;
					pSess.Update(art);
				}
				catch ( Exception e ) {
					Log.Debug("SendArtifacts Err: "+art.Id+", "+e.Message);
				}
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		private static void SendFactors(IFabricClient pFab, ISession pSess,
																IEnumerable<FabricFactor> pFacList) {
			var batch = new List<FabBatchNewFactor>();
			
			foreach ( FabricFactor fac in pFacList ) {
				var fb = new FabBatchNewFactor();
				batch.Add(fb);
			}
		}

	}

}