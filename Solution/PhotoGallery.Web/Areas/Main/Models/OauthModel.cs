namespace PhotoGallery.Web.Areas.Main.Models {

	/*================================================================================================*/
	public class OauthModel : MainBaseModel {

		public bool LoginSuccess { get; set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected override string PageTitle { get { return "Home"; } }

	}

}