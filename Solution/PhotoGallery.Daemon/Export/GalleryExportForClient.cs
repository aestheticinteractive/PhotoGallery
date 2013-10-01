using System.Collections.Generic;
using System.Diagnostics;
using Fabric.Clients.Cs;
using Fabric.Clients.Cs.Api;
using Fabric.Clients.Cs.Daemon;
using Fabric.Clients.Cs.Daemon.Data;
using PhotoGallery.Domain;
using PhotoGallery.Infrastructure;
using PhotoGallery.Services.Account.Tools;
using InstanceData = Fabric.Clients.Cs.Daemon.Data.InstanceData;

namespace PhotoGallery.Daemon.Export {

	/*================================================================================================*/
	public class GalleryExportForClient : IExportForClientDelegate {

		public static bool Stopped;

		private readonly Stopwatch vTimer;
		private readonly Queries vQuery;
		private readonly IFabricClient vClient;
		private readonly FabricUser vUser;
		private readonly IList<object> vUpdateList;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public GalleryExportForClient(Queries pQuery, IFabricClient pClient, FabricUser pUser) {
			vTimer = Stopwatch.StartNew();
			vQuery = pQuery;
			vClient = pClient;
			vUser = pUser;
			vUpdateList = new List<object>();
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public IFabricClient GetClient() {
			return vClient;
		}

		/*--------------------------------------------------------------------------------------------*/
		public bool StopExporting() {
			return Stopped;
		}

		/*--------------------------------------------------------------------------------------------*/
		public bool FakeFabricRequestMode() {
			return true;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public IList<ClassData> GetNewClasses() {
			return new List<ClassData>();
		}

		/*--------------------------------------------------------------------------------------------*/
		public IList<InstanceData> GetNewInstances() {
			IList<FabricArtifact> artList = vQuery.GetFabricArtifacts(vUser);
			var dataList = new List<InstanceData>();

			foreach ( FabricArtifact art in artList ) {
				var d = new InstanceData();
				d.ExporterId = art.Id;
				d.Name = art.Name;
				d.Disamb = art.Disamb;
				d.Note = art.Note;
				dataList.Add(d);
			}

			LogDebug("GetNewInstances: "+dataList.Count);
			return dataList;
		}

		/*--------------------------------------------------------------------------------------------*/
		public IList<UrlData> GetNewUrls() {
			return new List<UrlData>();
		}

		/*--------------------------------------------------------------------------------------------*/
		public IList<FabBatchNewFactor> GetNewFactors() {
			IList<FabricFactor> facList = vQuery.GetFabricFactors(vUser);
			var dataList = new List<FabBatchNewFactor>();

			foreach ( FabricFactor ff in facList ) {
				if ( ff.Primary != null && ff.Primary.ArtifactId == null ) {
					continue;
				}

				if ( ff.Related != null && ff.Related.ArtifactId == null ) {
					continue;
				}

				dataList.Add(FabricFactorBuilder.DbFactorToBatchFactor(ff));
			}

			LogDebug("GetNewFactors: "+dataList.Count+" (skip "+(facList.Count-dataList.Count)+")");
			return dataList;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public void OnClassExport(ClassData pClassData, FabClass pClass) {
			//do nothing
		}

		/*--------------------------------------------------------------------------------------------*/
		public void OnInstanceExport(InstanceData pInstanceData, FabInstance pInstance) {
			FabricArtifact art = vQuery.LoadArtifact((int)pInstanceData.ExporterId);
			art.ArtifactId = pInstance.ArtifactId;
			vUpdateList.Add(art);
			TrySendUpdates();
		}

		/*--------------------------------------------------------------------------------------------*/
		public void OnUrlExport(UrlData pUrlData, FabUrl pUrl) {
			//do nothing
		}

		/*--------------------------------------------------------------------------------------------*/
		public void OnFactorExport(FabBatchResult pFactor) {
			FabricFactor fac = vQuery.LoadFactor((int)pFactor.BatchId);
			fac.FactorId = pFactor.ResultId;
			vUpdateList.Add(fac);
			TrySendUpdates();
		}

		/*--------------------------------------------------------------------------------------------*/
		public void OnExportComplete() {
			TrySendUpdates(true);
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private void TrySendUpdates(bool pForce=false) {
			if ( !pForce && vUpdateList.Count < 10 ) {
				return;
			}

			LogDebug("TrySendUpdates: "+vUpdateList.Count+" / "+pForce);
			vQuery.UpdateObjects(vUpdateList);
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private void LogDebug(string pText) {
			Log.Debug(GetLogPrefix()+pText+GetLogTime());
		}

		/*--------------------------------------------------------------------------------------------*/
		private string GetLogPrefix() {
			return "GalleryExportForClient["+(vClient.UseDataProviderPerson ?
				"DataProv" :"User-"+vClient.PersonSession.SessionId)+"] ";
		}

		/*--------------------------------------------------------------------------------------------*/
		private string GetLogTime() {
			return " ("+vTimer.Elapsed.TotalSeconds.ToString("0.000")+" sec)";
		}

	}

}