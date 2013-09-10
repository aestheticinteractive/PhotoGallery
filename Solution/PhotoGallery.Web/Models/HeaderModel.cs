using System;
using Fabric.Clients.Cs.Api;

namespace PhotoGallery.Web.Models {

	/*================================================================================================*/
	public class HeaderModel {

		public bool IsPersonAuthenticated { get; set; }
		public Func<bool, string> GetPersonLoginOpenScript { get; set; }
		public FabUser User { get; set; }

	}

}