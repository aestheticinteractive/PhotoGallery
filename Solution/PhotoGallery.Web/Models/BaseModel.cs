using System.Collections.Generic;

namespace PhotoGallery.Web.Models {

	/*================================================================================================*/
	public abstract class BaseModel : IBaseModel {
		
		/*public static ActionLinkModel<HomeController> HomeLink = 
			new ActionLinkModel<HomeController>(x => x.Index(), Resources.Root_Pages_Home);
		public static ActionLinkModel<Areas.Me.Controllers.HomeController> MeLink =
			new ActionLinkModel<Areas.Me.Controllers.HomeController>(x => x.Index(), " ");
		public static ActionLinkModel<AccountController> LoginLink =
			new ActionLinkModel<AccountController>(x => x.Login(), Resources.Root_Pages_Login);
		public static ActionLinkModel<AccountController> LogoutLink =
			new ActionLinkModel<AccountController>(x => x.Logout(), Resources.Root_Pages_Logout);*/
		
		public abstract string AreaTitle { get; }
		public abstract IMvcLink AreaLink { get; }
		public abstract List<IMvcLink> AreaLinks { get; }

		protected virtual string ControllerTitle { get; set; }
		protected virtual string ActionTitle { get; set; }
		protected virtual string PageTitle { get; set; }

		public IMenuItem MenuRoot { get; private set; }
		public virtual string MenuTitle { get { return null; } }

		public string MenuFooterNotes { get; set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected BaseModel() {
			MenuRoot = new MenuItem(null);
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public virtual string HtmlTitle {
			get {
				string t = "Kinstner Photo Gallery :: "+AreaTitle;

				if ( ControllerTitle != null ) {
					t += " :: "+ControllerTitle;

					if ( ActionTitle != null ) {
						t += " > "+ActionTitle;

						if ( PageTitle != null ) {
							t += " > "+PageTitle;
						}
					}
				}

				return t;
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		public void AddSubItem(MenuItem pItem) {
			MenuRoot.SubItems.Add(pItem);
		}

	}

}