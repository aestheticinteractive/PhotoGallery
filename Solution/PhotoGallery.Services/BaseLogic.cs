﻿using Fabric.Clients.Cs;
using NHibernate;
using PhotoGallery.Database;

namespace PhotoGallery.Services {

	/*================================================================================================*/
	public class BaseLogic {

		protected IFabricClient Fab { get; private set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public BaseLogic(IFabricClient pFab) {
			Fab = pFab;
			Connect.InitOnce();
		}

		/*--------------------------------------------------------------------------------------------*/
		protected ISession NewSession() {
			return new SessionProvider().OpenSession();
		}

	}

}