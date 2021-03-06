﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Fabric.Clients.Cs;
using Fabric.Clients.Cs.Daemon;
using Fabric.Clients.Cs.Session;
using PhotoGallery.Domain;
using PhotoGallery.Infrastructure;

namespace PhotoGallery.Daemon.Export {

	/*================================================================================================*/
	public class GalleryExport : IExportServiceDelegate {

		private readonly Queries vQuery;
		private readonly Func<IFabricPersonSession, IFabricClient> vClientProv;
		private readonly IFabricClient vDbClient;
		private readonly ExportService vExpSvc;

		private bool vStop;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public GalleryExport(Queries pQuery, Func<IFabricPersonSession, IFabricClient> pClientProv,
																	long pAppId, string pAppSecret) {
			vQuery = pQuery;
			vClientProv = pClientProv;

			if ( !FabricClient.IsInitialized ) {
				FabricClient.InitOnce(new FabricClientConfig("gallery", "http://api.inthefabric.com",
					pAppId, pAppSecret, (ck => "http://www.zachkinstner.com"),
					(ck => new FabricSessionContainer())));
			}

			vDbClient = vClientProv(null);
			vDbClient.UseAppDataProvider = true;

			vExpSvc = new ExportService(this);
		}

		/*--------------------------------------------------------------------------------------------*/
		public void Start() {
			while ( true ) {
				if ( !vStop ) { //don't exit the loop; the user decides when to finally exit the app
					vExpSvc.StartNewExports();
				}

				Thread.Sleep(10000);
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		public void Stop() {
			vStop = true;
			GalleryExportForClient.Stopped = true;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public IFabricClient GetDataProvClient() {
			return vDbClient;
		}

		/*--------------------------------------------------------------------------------------------*/
		public IList<IFabricClient> GetUserClients() {
			vQuery.DeleteExpiredSessions();

			IList<FabricPersonSession> list = vQuery.FindUpdatableSessions();

			if ( list.Count > 0 ) {
				Log.Debug("GalleryExport.GetUserClients: "+list.Count+" / "+DateTime.Now);
			}

			return list.Select(fps => vClientProv(new SavedSession(fps))).ToList();
		}

		/*--------------------------------------------------------------------------------------------*/
		public void HandleExpiredUserClient(IFabricClient pClient) {
			//do nothing
		}

		/*--------------------------------------------------------------------------------------------*/
		public IExportForClientDelegate GetExportForClientDelegate(IFabricClient pClient) {
			return new GalleryExportForClient(vQuery, pClient, vQuery.GetCurrentUser(pClient));
		}

		/*--------------------------------------------------------------------------------------------*/
		public IExportForClient GetExportForClient(IExportForClientDelegate pDelegate) {
			return new ExportForClient(pDelegate);
		}

	}

}