using System.Collections.Generic;
using System.Web.Mvc;
using PhotoGallery.Services.Account.Tools;
using PhotoGallery.Services.Admin;
using PhotoGallery.Web.Application;
using PhotoGallery.Web.Controllers;

namespace PhotoGallery.Web.Areas.Admin.Controllers {

	/*================================================================================================*/
	public partial class PetsController : BaseController {

		private readonly PetsService vPets;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public PetsController(PetsService pPets) {
			vPets = pPets;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		[AdminAuthorize]
		public virtual ActionResult Index() {
			/*IList<long> gulliverOwners = new List<long>();
			gulliverOwners.Add(58693012139540480); //zach
			gulliverOwners.Add(58693016040243200); //mel
			gulliverOwners.Add(58693019070627840); //ellie
			gulliverOwners.Add(58693020376104960); //penny

			vPets.AddDog("Gulliver Kinstner", "Lhasa Apso, pet", null,
				(long)LiveArtifactId.LhasaApso, gulliverOwners);*/

			/*IList<long> chewyOwners = new List<long>();
			chewyOwners.Add(58694628326506496); //dave
			chewyOwners.Add(58694638811217920); //pat

			vPets.AddDog("Chewbacca McDonald", "Dog, pet", null,
				(long)LiveArtifactId.Dog, chewyOwners);*/

			return View();
		}

	}

}