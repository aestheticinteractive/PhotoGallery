using System;
using System.Runtime.Remoting.Contexts;
using System.Web;
using Fabric.Clients.Cs.Api;
using Fabric.Clients.Cs.Session;
using PhotoGallery.Domain;

namespace PhotoGallery.Daemon.Fabric {

	/*================================================================================================*/
	public class SavedSession : IFabricPersonSession, IContextProperty {

		public const string PropName = "SavedSess";

		public FabricPersonSession Session { get; private set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public SavedSession(FabricPersonSession pSession) {
			Session = pSession;
		}

		/*--------------------------------------------------------------------------------------------*/
		public string SessionId {
			get { return Session.SessionId; }
		}

		/*--------------------------------------------------------------------------------------------*/
		public string GrantCode {
			get { return Session.GrantCode; }
		}

		/*--------------------------------------------------------------------------------------------*/
		public string BearerToken {
			get { return Session.BearerToken; }
		}

		/*--------------------------------------------------------------------------------------------*/
		public string RefreshToken {
			get { return Session.RefreshToken; }
		}

		/*--------------------------------------------------------------------------------------------*/
		public bool IsAuthenticated {
			get { return true; }
		}

		/*--------------------------------------------------------------------------------------------*/
		public DateTime Expiration {
			get { return new DateTime(Session.Expiration); }
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public FabOauthLogout Logout() {
			throw new NotImplementedException();
		}

		/*--------------------------------------------------------------------------------------------*/
		public bool RefreshTokenIfNecessary() {
			throw new NotImplementedException();
		}

		/*--------------------------------------------------------------------------------------------*/
		public void SaveToCookies(HttpCookieCollection pCookies) {
			throw new NotImplementedException();
		}

		/*--------------------------------------------------------------------------------------------*/
		public string GetGrantCodeUrl(bool pSwitchUser = false) {
			throw new NotImplementedException();
		}

		/*--------------------------------------------------------------------------------------------*/
		public string GetGrantWindowOpenScript(string pGrantCodeUrl = null) {
			throw new NotImplementedException();
		}

		/*--------------------------------------------------------------------------------------------*/
		public FabOauthAccess HandleGrantCodeRedirect(HttpRequestBase pRedirectRequest) {
			throw new NotImplementedException();
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public bool IsNewContextOK(Context pNewContext) {
			return true;
		}

		/*--------------------------------------------------------------------------------------------*/
		public void Freeze(Context pNewContext) {}

		/*--------------------------------------------------------------------------------------------*/
		public string Name {
			get { return PropName; }
		}

	}

}