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
		}

		/*--------------------------------------------------------------------------------------------*/
		public static void CheckForNewTasks(IFabricClient pFab) {
			var t = new Thread(StartDataProvThread);
			t.Start(DataProvClient);

			//t = new Thread(StartUserThread);
			//t.Start(pFab);
		}

		/*--------------------------------------------------------------------------------------------*/
		public static void StopAllThreads() {
			StopThreads = true;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private static void StartDataProvThread(object pFabClient) {
			IFabricClient fab = (IFabricClient)pFabClient;
			fab.UseDataProviderPerson = true;

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
													Func<ISession, IList<FabricFactor>> pGetFabList) {
			IList<FabricArtifact> artList;

			using ( ISession s = BaseLogic.NewSession() ) {
				artList = pGetArtList(s);
			}

			using ( ISession s = BaseLogic.NewSession() ) {
				using ( ITransaction tx = s.BeginTransaction() ) {
					SendArtifacts(pFab, s, artList);
					tx.Commit();
				}
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		private static void SendArtifacts(IFabricClient pFab, ISession pSess, 
																	IList<FabricArtifact> pArtList) {
			Log.Debug("SendArtifacts: "+pArtList.Count);

			foreach ( FabricArtifact art in pArtList ) {
				if ( StopThreads ) {
					Log.Debug("SendArtifacts Stop!");
					return;
				}

				try {
					Log.Debug("SendArtifacts Art: "+art.Id+" / "+art.Name);

					FabInstance fi = new FabInstance { ArtifactId = 10000+art.Id };
					//FabInstance fi = pFab.Services.Modify.AddInstance
					//	.Post(art.Name, art.Disamb, art.Note).FirstDataItem();

					Log.Debug("SendArtifacts Fac: "+art.Id+" => "+fi.ArtifactId);
					art.ArtifactId = fi.ArtifactId;
					pSess.Update(art);
				}
				catch ( Exception e ) {
					Log.Debug("SendArtifacts Err: "+art.Id+", "+e.Message);
				}
			}
		}

	}

}