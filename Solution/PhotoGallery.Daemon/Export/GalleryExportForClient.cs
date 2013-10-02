using System;
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
		private readonly bool vStop;

		private readonly IList<object> vUpdateList;
		private readonly IDictionary<int, FabricArtifact> vInstanceMap;
		private readonly IDictionary<int, FabricFactor> vFactorMap;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public GalleryExportForClient(Queries pQuery, IFabricClient pClient, FabricUser pUser) {
			vTimer = Stopwatch.StartNew();
			vQuery = pQuery;
			vClient = pClient;
			vUser = pUser;
			vStop = (vClient == null || (!vClient.UseDataProviderPerson && vUser == null));

			vUpdateList = new List<object>();
			vInstanceMap = new Dictionary<int, FabricArtifact>();
			vFactorMap = new Dictionary<int, FabricFactor>();
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public IFabricClient GetClient() {
			return vClient;
		}

		/*--------------------------------------------------------------------------------------------*/
		public bool StopExporting() {
			return (Stopped || vStop);
		}

		/*--------------------------------------------------------------------------------------------*/
		public bool FakeFabricRequestMode() {
			return false;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public IList<ClassData> GetNewClasses() {
			return new List<ClassData>();
		}

		/*--------------------------------------------------------------------------------------------*/
		public IList<InstanceData> GetNewInstances() {
			SaveUpdates();

			IList<FabricArtifact> artList = vQuery.GetFabricArtifacts(10, vUser);
			var dataList = new List<InstanceData>();

			foreach ( FabricArtifact art in artList ) {
				var d = new InstanceData();
				d.ExporterId = art.Id;
				d.Name = art.Name;
				d.Disamb = art.Disamb;
				d.Note = art.Note;
				dataList.Add(d);

				vInstanceMap.Add(art.Id, art);
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
			SaveUpdates();

			IList<FabricFactor> facList = vQuery.GetFabricFactors(20, vUser);
			var dataList = new List<FabBatchNewFactor>();

			foreach ( FabricFactor ff in facList ) {
				if ( ff.Primary != null && ff.Primary.ArtifactId == null ) {
					continue;
				}

				if ( ff.Related != null && ff.Related.ArtifactId == null ) {
					continue;
				}

				dataList.Add(FabricFactorBuilder.DbFactorToBatchFactor(ff));
				vFactorMap.Add(ff.Id, ff);
			}

			LogDebug("GetNewFactors: "+dataList.Count+" (skip "+(facList.Count-dataList.Count)+")");
			return dataList;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public void OnClassExport(ClassData pClassData, FabClass pClass) {
			LogInfo("OnClassExport: "+pClassData.ExporterId+" => "+pClass.ArtifactId);
		}

		/*--------------------------------------------------------------------------------------------*/
		public void OnInstanceExport(InstanceData pInstanceData, FabInstance pInstance) {
			LogInfo("OnInstanceExport: "+pInstanceData.ExporterId+" => "+pInstance.ArtifactId);

			FabricArtifact art = vInstanceMap[(int)pInstanceData.ExporterId];
			art.ArtifactId = pInstance.ArtifactId;
			vUpdateList.Add(art);
		}

		/*--------------------------------------------------------------------------------------------*/
		public void OnUrlExport(UrlData pUrlData, FabUrl pUrl) {
			LogInfo("OnUrlExport: "+pUrlData.ExporterId+" => "+pUrl.ArtifactId);
		}

		/*--------------------------------------------------------------------------------------------*/
		public void OnFactorExport(FabBatchResult pFactor) {
			LogInfo("OnFactorExport: "+pFactor.BatchId+" => "+pFactor.ResultId);

			FabricFactor fac = vFactorMap[(int)pFactor.BatchId];
			fac.FactorId = pFactor.ResultId;
			vUpdateList.Add(fac);
		}

		/*--------------------------------------------------------------------------------------------*/
		public void OnExportComplete() {
			SaveUpdates();
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private void SaveUpdates() {
			if ( vUpdateList.Count > 0 ) {
				LogDebug("SaveUpdates: "+vUpdateList.Count);
				vQuery.UpdateObjects(vUpdateList);
			}

			vUpdateList.Clear();
			vInstanceMap.Clear();
			vFactorMap.Clear();
		}

		/*--------------------------------------------------------------------------------------------*/
		private void LogInfo(string pText) {
			LogWith(Log.Info, pText);
		}

		/*--------------------------------------------------------------------------------------------*/
		private void LogDebug(string pText) {
			LogWith(Log.Debug, pText);
		}

		/*--------------------------------------------------------------------------------------------*/
		private void LogWith(Action<string> pLogFunc, string pText) {
			string lbl = (vClient.UseDataProviderPerson ? "DataProv" : vClient.PersonSession.SessionId);
			pLogFunc("GEFC["+lbl+"] "+vTimer.Elapsed.TotalSeconds.ToString("0.000")+"s | "+pText);
		}

	}

}