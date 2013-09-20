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
			pCont.Register(Classes.FromAssemblyContaining<BaseService>()
				.BasedOn<BaseService>()
				.LifestyleTransient());

			pCont.Register(Component.For<IFabricClient>().ImplementedBy<FabricClient>());
		}

	}

}