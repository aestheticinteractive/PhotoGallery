using System;
using System.Diagnostics.CodeAnalysis;
using Fabric.Clients.Cs.Logging;
using PhotoGallery.Infrastructure;

namespace PhotoGallery.Services {

	/*================================================================================================*/
	[ExcludeFromCodeCoverage]
	public class LogFabric : IFabricLog {

		private const string Empty32 = "                                ";

		public bool WriteToConsole { get; set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public void Info(string pSessionId, string pText) {
			Output(pSessionId, pText, Log.Info);
		}

		/*--------------------------------------------------------------------------------------------*/
		public void Debug(string pSessionId, string pText) {
			Output(pSessionId, pText, Log.Debug);
		}

		/*--------------------------------------------------------------------------------------------*/
		public void Error(string pSessionId, string pText) {
			Output(pSessionId, pText, Log.Error);
		}

		/*--------------------------------------------------------------------------------------------*/
		public void Fatal(string pSessionId, string pText) {
			Output(pSessionId, pText, Log.Fatal);
		}

		/*--------------------------------------------------------------------------------------------*/
		public void Warn(string pSessionId, string pText) {
			Output(pSessionId, pText, Log.Warn);
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private void Output(string pSessionId, string pText, Action<String> pAction) {
			string str = "FAB["+(pSessionId ?? Empty32)+"]: "+pText;
			pAction(str);

			if ( WriteToConsole ) {
				Console.WriteLine(str);
			}
		}

	}

}