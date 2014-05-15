using System;
using System.Collections.Generic;
using System.Diagnostics;
using Fabric.Clients.Cs;
using Fabric.Clients.Cs.Api;
using Fabric.Clients.Cs.Daemon;
using PhotoGallery.Domain;
using PhotoGallery.Infrastructure;
using PhotoGallery.Services.Account.Tools;

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
		private readonly IDictionary<CreateFabInstance, FabricArtifact> vInstanceMap;
		private readonly IDictionary<CreateFabFactor, FabricFactor> vFactorMap;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public GalleryExportForClient(Queries pQuery, IFabricClient pClient, FabricUser pUser) {
			vTimer = Stopwatch.StartNew();
			vQuery = pQuery;
			vClient = pClient;
			vUser = pUser;
			vStop = (vClient == null || (!vClient.UseAppDataProvider && vUser == null));

			vUpdateList = new List<object>();
			vInstanceMap = new Dictionary<CreateFabInstance, FabricArtifact>();
			vFactorMap = new Dictionary<CreateFabFactor, FabricFactor>();
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
		public IList<CreateFabClass> GetNewClasses() {
			return new List<CreateFabClass>();
		}

		/*--------------------------------------------------------------------------------------------*/
		public IList<CreateFabInstance> GetNewInstances() {
			SaveUpdates();

			IList<FabricArtifact> artList = vQuery.GetFabricArtifacts(10, vUser);
			var list = new List<CreateFabInstance>();

			foreach ( FabricArtifact art in artList ) {
				var cfi = new CreateFabInstance();
				cfi.Name = art.Name;
				cfi.Disamb = art.Disamb;
				cfi.Note = art.Note;
				list.Add(cfi);

				vInstanceMap.Add(cfi, art);
			}

			if ( list.Count > 0 ) {
				LogDebug("GetNewInstances: "+list.Count);
			}

			return list;
		}

		/*--------------------------------------------------------------------------------------------*/
		public IList<CreateFabUrl> GetNewUrls() {
			return new List<CreateFabUrl>();
		}

		/*--------------------------------------------------------------------------------------------*/
		public IList<CreateFabFactor> GetNewFactors() {
			SaveUpdates();

			IList<FabricFactor> facList = vQuery.GetFabricFactors(20, vUser);
			var list = new List<CreateFabFactor>();
			var fixes = new List<object>();

			foreach ( FabricFactor ff in facList ) {
				//skip factors that rely on not-yet-exported artifacts

				if ( ff.Primary != null && ff.Primary.ArtifactId == null ) {
					continue;
				}

				if ( ff.Related != null && ff.Related.ArtifactId == null ) {
					continue;
				}

				if ( FactorTasks.FixFactorRefs(ff, vQuery, vClient) ) {
					fixes.Add(ff);
				}

				CreateFabFactor cff = FabricFactorBuilder.DbFactorToCreateFabFactor(ff);
				list.Add(cff);
				vFactorMap.Add(cff, ff);
			}

			if ( list.Count > 0 ) {
				LogDebug("GetNewFactors: "+list.Count+
					" (skips="+(facList.Count-list.Count)+", fixes="+fixes.Count+")");
			}

			vQuery.UpdateObjects(fixes); //for FixFactorRefs
			return list;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public void OnClassExport(CreateFabClass pCreate, FabClass pClass) {
			LogInfo("OnClassExport: "+pClass.Id);
		}

		/*--------------------------------------------------------------------------------------------*/
		public void OnInstanceExport(CreateFabInstance pCreate, FabInstance pInstance) {
			FabricArtifact art = vInstanceMap[pCreate];
			LogInfo("OnInstanceExport: "+art.Id+" => "+pInstance.Id);
			art.ArtifactId = pInstance.Id;
			vUpdateList.Add(art);
		}

		/*--------------------------------------------------------------------------------------------*/
		public void OnUrlExport(CreateFabUrl pCreate, FabUrl pUrl) {
			LogInfo("OnUrlExport: "+pUrl.Id);
		}

		/*--------------------------------------------------------------------------------------------*/
		public void OnFactorExport(CreateFabFactor pCreate, FabFactor pFactor) {
			FabricFactor fac = vFactorMap[pCreate];
			LogInfo("OnFactorExport: "+fac.Id+" => "+pFactor.Id);
			fac.FactorId = pFactor.Id;
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
			string lbl = (vClient.UseAppDataProvider ? "DataProv" : vClient.PersonSession.SessionId);
			pLogFunc("GEFC["+lbl+"] "+vTimer.Elapsed.TotalSeconds.ToString("0.000")+"s | "+pText);
		}

	}

}