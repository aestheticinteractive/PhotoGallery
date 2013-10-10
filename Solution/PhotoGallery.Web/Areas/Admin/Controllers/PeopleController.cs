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
			var m = new PeopleModel();
			m.PersonTags = vPeople.GetPersonTags();
			return View(m);
		}

		/*--------------------------------------------------------------------------------------------*/
		[AdminAuthorize]
		[HttpPost]
		public virtual ActionResult Index(PeopleModel pModel) {
			if ( ModelState.IsValid ) {
				var g = (pModel.AddIsMale ? PeopleService.Gender.Male : PeopleService.Gender.Female);
				vPeople.AddPersonTag(pModel.AddName, g);
				return RedirectToAction(MVC.Admin.People.Index());
			}

			return View(pModel);
		}

	}

}