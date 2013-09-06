using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.MicroKernel;

namespace PhotoGallery.Web.Application.Windsor {

	/*================================================================================================*/
	public class WindsorControllerFactory : DefaultControllerFactory {

		private readonly IKernel vKernel;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public WindsorControllerFactory(IKernel pKernel) {
			vKernel = pKernel;
		}
		
		/*--------------------------------------------------------------------------------------------*/
		public override void ReleaseController(IController pController) {
			vKernel.ReleaseComponent(pController);
		}
		
		/*--------------------------------------------------------------------------------------------*/
		protected override IController GetControllerInstance(RequestContext pReq, Type pType) {
			if ( pType == null ) {
				throw new HttpException(404,
					string.Format("The controller for path '{0}' could not be found.",
					pReq.HttpContext.Request.Path));
			}

			return (IController)vKernel.Resolve(pType);
		}

	}

}