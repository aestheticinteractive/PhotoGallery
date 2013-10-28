using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace PhotoGallery.Web.Application.Windsor {

	/*================================================================================================*/
	public class ControllersInstaller : IWindsorInstaller {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public void Install(IWindsorContainer pCont, IConfigurationStore pStore) {
			IRegistration ctrl = Classes.FromThisAssembly()
				.BasedOn<IController>()
				.LifestyleTransient();

			pCont.Register(ctrl);
		}

	}

}