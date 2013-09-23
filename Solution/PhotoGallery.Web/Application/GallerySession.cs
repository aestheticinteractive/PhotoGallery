using System.Web;
using PhotoGallery.Services.Main.Dto;

namespace PhotoGallery.Web.Application {

	/*================================================================================================*/
	public class GallerySession {

		private const string PhotoSetKey = "PhotoSet";

		private readonly HttpSessionStateBase vSess;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public GallerySession(HttpSessionStateBase pSession) {
			vSess = pSession;
		}

		/*--------------------------------------------------------------------------------------------*/
		private T GetSessObj<T>(string pKey) {
			return (vSess[pKey] == null ? default(T) : (T)vSess[pKey]);
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public WebPhotoSet PhotoSet {
			get {
				return GetSessObj<WebPhotoSet>(PhotoSetKey);
			}
			set {
				vSess[PhotoSetKey] = value;
			}
		}

	}

}