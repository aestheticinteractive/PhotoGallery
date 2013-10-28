using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Fabric.Clients.Cs;
using PhotoGallery.Services;

namespace PhotoGallery.Web.Application.Windsor {

	/*================================================================================================*/
	public class WebLogicInstaller : IWindsorInstaller {
		

		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public void Install(IWindsorContainer pCont, IConfigurationStore pStore) {
			IRegistration svc = Classes
				.FromAssemblyContaining<BaseService>()
				.BasedOn<BaseService>()
				.LifestyleTransient();

			IRegistration fab = Component
				.For<IFabricClient>()
				.ImplementedBy<FabricClient>()
				.LifestyleTransient();

			pCont.Register(svc, fab);
		}

	}

}