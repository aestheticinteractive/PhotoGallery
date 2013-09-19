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

		private static bool StopThreads;

		private static Thread vDataProvThread;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public static void StopAllThreads() {
			StopThreads = true;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public static void BeginDataProv(IFabricClient pFab) {
			vDataProvThread = new Thread(StartDataProvThread);
			vDataProvThread.Start(pFab);
		}

		/*--------------------------------------------------------------------------------------------*/
		private static void StartDataProvThread(object pFabClient) {
			IFabricClient fab = (IFabricClient)pFabClient;
			fab.UseDataProviderPerson = true;

			Func<ISession, IList<FabricArtifact>> getArtList = (s => s
				.QueryOver<FabricArtifact>()
				.Where(x => x.ArtifactId == null && x.Creator == null)
				.List()
			);

			Func<ISession, IList<FabricFactor>> getFacList = (s => s
				.QueryOver<FabricFactor>()
				.Where(x => x.FactorId == null && x.Creator == null)
				.List()
			);

			while ( true ) {
				SendAll(fab, getArtList, getFacList);

				if ( StopThreads ) {
					return;
				}

				Thread.Sleep(5000);
			}
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private static void SendAll(IFabricClient pFab, 
													Func<ISession, IList<FabricArtifact>> pGetArtList,
													Func<ISession, IList<FabricFactor>> pGetFabList) {
			IList<FabricArtifact> artList;

			using ( ISession s = BaseLogic.NewSession() ) {
				artList = pGetArtList(s);
			};

			using ( ISession s = BaseLogic.NewSession() ) {
				using ( ITransaction tx = s.BeginTransaction() ) {
					SendArtifacts(pFab, s, artList);
					tx.Commit();
				}
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		private static void SendArtifacts(IFabricClient pFab, ISession pSess, 
																IEnumerable<FabricArtifact> pArtList) {
			foreach ( FabricArtifact art in pArtList ) {
				Log.Debug("ArtToFab: "+art.Id+" / "+art.Name);

				FabInstance fi = new FabInstance() { ArtifactId = 10000+art.Id };
				//FabInstance fi = pFab.Services.Modify.AddInstance
				//	.Post(art.Name, art.Disamb, art.Note).FirstDataItem();

				Log.Debug(" ... "+fi.ArtifactId);
				art.ArtifactId = fi.ArtifactId;
				pSess.Update(art);
			}
		}

	}

}