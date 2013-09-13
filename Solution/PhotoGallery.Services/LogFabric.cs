using System;
using System.Diagnostics.CodeAnalysis;
using Fabric.Clients.Cs;
using Fabric.Clients.Cs.Logging;
using PhotoGallery.Infrastructure;

namespace PhotoGallery.Services {

	/*================================================================================================*/
	[ExcludeFromCodeCoverage]
	public class LogFabric : IFabricLog {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public void Info(IFabricClientConfig pConfig, string pText) {
			Output(pConfig, pText, Log.Info);
		}

		/*--------------------------------------------------------------------------------------------*/
		public void Debug(IFabricClientConfig pConfig, string pText) {
			Output(pConfig, pText, Log.Debug);
		}

		/*--------------------------------------------------------------------------------------------*/
		public void Error(IFabricClientConfig pConfig, string pText) {
			Output(pConfig, pText, Log.Error);
		}

		/*--------------------------------------------------------------------------------------------*/
		public void Fatal(IFabricClientConfig pConfig, string pText) {
			Output(pConfig, pText, Log.Fatal);
		}

		/*--------------------------------------------------------------------------------------------*/
		public void Warn(IFabricClientConfig pConfig, string pText) {
			Output(pConfig, pText, Log.Warn);
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private void Output(IFabricClientConfig pConfig, string pText, Action<String> pAction) {
			pAction("FAB["+pConfig.ConfigKey+"]: "+pText);
		}

	}

}