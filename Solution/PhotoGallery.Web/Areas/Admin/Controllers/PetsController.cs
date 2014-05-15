using System.Collections.Generic;
using System.Web.Mvc;
using PhotoGallery.Services.Account.Tools;
using PhotoGallery.Services.Admin;
using PhotoGallery.Web.Application;
using PhotoGallery.Web.Areas.Admin.Models;
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



			/*IList<long> boomerOwners = new List<long>();
			boomerOwners.Add(58694649307463680); //david
			boomerOwners.Add(58694670308343808); //jen
			boomerOwners.Add(58694680783618048); //caleb

			vPets.AddDog("Boomer McDonald", "Dog, pet", null,
				(long)LiveArtifactId.Dog, boomerOwners);*/



			/*IList<long> pixieOwners = new List<long>();
			pixieOwners.Add(58693020653977600); //rick
			pixieOwners.Add(58693020958064640); //sue

			vPets.AddDog("Pixie Kinstner", "Dog, pet", null,
				(long)LiveArtifactId.Dog, pixieOwners);*/



			/*IList<long> kateOwners = new List<long>();
			kateOwners.Add(58694360862031872); //lance
			kateOwners.Add(58694523385020416); //julia

			vPets.AddDog("Kate Kinstner", "Great Dane, pet", null,
				(long)LiveArtifactId.GreatDane, kateOwners);*/

			
			
			/*IList<long> anyaOwners = new List<long>();
			anyaOwners.Add(60256745309601792); //joanne

			vPets.AddDog("Anya Khanani", "Dog, pet", null,
				(long)LiveArtifactId.Dog, anyaOwners);*/



			return View(new HomeModel());
		}

	}

}