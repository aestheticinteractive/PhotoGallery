using System.Web.Mvc;
using PhotoGallery.Services.Admin;
using PhotoGallery.Web.Application;
using PhotoGallery.Web.Areas.Admin.Models;
using PhotoGallery.Web.Controllers;

namespace PhotoGallery.Web.Areas.Admin.Controllers {

	/*================================================================================================*/
	public partial class PeopleController : BaseController {

		private readonly PeopleService vPeople;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public PeopleController(PeopleService pPeople) {
			vPeople = pPeople;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		[AdminAuthorize]
		public virtual ActionResult Index() {
			/*vPeople.AddPersonTag("Zach Kinstner", PeopleService.Gender.Male);
			vPeople.AddPersonTag("Melissa Kinstner", PeopleService.Gender.Female);
			vPeople.AddPersonTag("Elliot Kinstner", PeopleService.Gender.Female);
			vPeople.AddPersonTag("Penelope Kinstner", PeopleService.Gender.Female);*/

			/*vPeople.AddPersonTag("Rick Kinstner", PeopleService.Gender.Male);
			vPeople.AddPersonTag("Sue Kinstner", PeopleService.Gender.Female);*/

			var m = new PeopleModel();
			m.PersonTags = vPeople.GetPersonTags();
			return View(m);
		}

	}

}