using System.Collections.Generic;

namespace PhotoGallery.Web.Models {

	/*================================================================================================*/
	public abstract class BaseModel : IBaseModel {
		
		public abstract string AreaTitle { get; }
		public abstract IMvcLink AreaLink { get; }
		public abstract List<IMvcLink> AreaLinks { get; }

		protected virtual string ControllerTitle { get; set; }
		protected virtual string ActionTitle { get; set; }
		protected virtual string PageTitle { get; set; }


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

	}

}