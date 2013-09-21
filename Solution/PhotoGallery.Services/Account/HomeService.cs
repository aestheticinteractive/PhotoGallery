using Fabric.Clients.Cs;
using Fabric.Clients.Cs.Api;
using NHibernate;
using PhotoGallery.Domain;
using PhotoGallery.Services.Account.Dto;
using PhotoGallery.Services.Fabric;

namespace PhotoGallery.Services.Account {
	
	/*================================================================================================*/
	public class HomeService : BaseService {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public HomeService(IFabricClient pFab) : base(pFab) {}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public bool IsPersonAuthenticated() {
			return Fab.PersonSession.IsAuthenticated;
		}

		/*--------------------------------------------------------------------------------------------*/
		public string GetPersonLoginOpenScript(bool pSwitch) {
			string url = Fab.PersonSession.GetGrantCodeUrl(pSwitch);
			return Fab.PersonSession.GetGrantWindowOpenScript(url);
		}

		/*--------------------------------------------------------------------------------------------*/
		public FabUser GetActiveFabUser() {
			return FabricService.GetActiveUser(Fab);
		}

		/*--------------------------------------------------------------------------------------------*/
		public FabOauthLogout Logout() {
			if ( !IsPersonAuthenticated() ) {
				return null;
			}

			return Fab.PersonSession.Logout();
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public WebUser GetWebUser() {
			if ( !IsPersonAuthenticated() ) {
				return null;
			}

			FabUser fabUser = GetActiveFabUser();

			using ( ISession s = NewSession() ) {
				FabricUser u = s.QueryOver<FabricUser>()
					.Where(x => x.Name == fabUser.Name)
					.Take(1)
					.SingleOrDefault();

				return new WebUser(fabUser, u);
			}
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public static FabricUser GetCurrentUser(IFabricClient pFab, ISession pSess) {
			FabUser fabUser = FabricService.GetActiveUser(pFab);

			if ( fabUser == null ) {
				return null;
			}

			return pSess.QueryOver<FabricUser>()
				.Where(x => x.Name == fabUser.Name)
				.Take(1)
				.SingleOrDefault();
		}

	}

}