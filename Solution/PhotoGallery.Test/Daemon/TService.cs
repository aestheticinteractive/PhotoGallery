using System.Collections.Generic;
using System.Threading;
using Fabric.Clients.Cs;
using Moq;
using NHibernate;
using NUnit.Framework;
using PhotoGallery.Daemon.Fabric;
using PhotoGallery.Database;
using PhotoGallery.Domain;

namespace PhotoGallery.Test.Daemon {

	/*================================================================================================*/
	[TestFixture]
	public class TService {

		private Mock<ISessionProvider> vMockSessProv;
		private Mock<Queries> vMockQuery;
		private Mock<IExporter> vMockExport;
		private ServiceContext vSvcCtx;

		private IList<Mock<IFabricClient>> vMockFabClients;
		private IList<string> vFabClientKeys;
		private IList<Thread> vExportThreads;
		private IList<ExporterContext> vExportCtxs;
		private IList<IFabricClient> vExportFabClients;
		private IList<FabricUser> vExportUsers;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		[SetUp]
		public void SetUp() {
			FabricClient.ResetInitialization();

			vMockSessProv = new Mock<ISessionProvider>();
			vMockQuery = new Mock<Queries>();
			vMockExport = new Mock<IExporter>();

			vSvcCtx = new ServiceContext();
			vSvcCtx.SessProv = vMockSessProv.Object;
			vSvcCtx.Query = vMockQuery.Object;

			vMockFabClients = new List<Mock<IFabricClient>>();
			vFabClientKeys = new List<string>();
			vExportThreads = new List<Thread>();
			vExportCtxs = new List<ExporterContext>();
			vExportFabClients = new List<IFabricClient>();
			vExportUsers = new List<FabricUser>();

			vSvcCtx.FabClientProv = (ck => {
				vFabClientKeys.Add(ck);

				var mockFab = new Mock<IFabricClient>();
				vMockFabClients.Add(mockFab);

				//Log.Debug("TEST-FabClientProv: "+ck+" / "+vFabClientKeys.Count);
				return mockFab.Object;
			});

			vSvcCtx.ExportProv = (ctx, fc, user) => {
				vExportThreads.Add(Thread.CurrentThread);
				vExportCtxs.Add(ctx);
				vExportFabClients.Add(fc);
				vExportUsers.Add(user);
				//Log.Debug("TEST-ExportProv: "+Thread.CurrentThread.ManagedThreadId+" / "+
				//	vExportThreads.Count);
				return vMockExport.Object;
			};
		}

		/*--------------------------------------------------------------------------------------------*/
		private Service NewService() {
			return new Service(vSvcCtx, 123, "secret", 456);
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		[Test]
		public void New() {
			FabricClient.ResetInitialization();
			NewService();
			
			Assert.True(FabricClient.IsInitialized, "FabricClient should be initialized.");

			Assert.AreEqual(1, vFabClientKeys.Count, "Incorrect FabricClient count.");
			Assert.AreEqual(1, vExportThreads.Count, "Incorrect Export count.");
			Assert.AreEqual("dataProv", vFabClientKeys[0], "Incorrect FabricClient ConfigKey used.");

			Assert.NotNull(vExportCtxs[0], "ExporterContext should be filled.");
			Assert.AreEqual(vSvcCtx.SessProv, vExportCtxs[0].SessProv,
				"Incorrect ExporterContext.SessProv.");
			Assert.AreEqual(vSvcCtx.Query, vExportCtxs[0].Query, "Incorrect ExporterContext.Query.");
			Assert.Null(vExportCtxs[0].SavedSession, "ExporterContext.SavedSession should be null.");

			Assert.AreEqual(vMockFabClients[0].Object, vExportFabClients[0],
				"Incorrect Exporter FabClient.");
			Assert.Null(vExportUsers[0], "Exporter User should be null.");

			vMockExport.Verify(x => x.SendAll(0), Times.Once());

			Assert.AreNotEqual(Thread.CurrentThread, vExportThreads[0],
				"Exporter should occur in its own thread.");
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(8)]
		public void FindSessions(int pCount) {
			var mockSess = new Mock<ISession>();
			vMockSessProv.Setup(x => x.OpenSession()).Returns(mockSess.Object);

			var list = new List<FabricPersonSession>();

			for ( int i = 0 ; i < pCount ; ++i ) {
				var fps = new FabricPersonSession();
				fps.SessionId = "sessionId"+i;
				list.Add(fps);
			}

			vMockQuery.Setup(x => x.FindUpdatableSessions(mockSess.Object)).Returns(list);

			////

			var svc = NewService();
			svc.FindSessions();
			Thread.Sleep(200); //wait for Threads to finish

			////

			Assert.AreEqual(pCount+1, vFabClientKeys.Count, "Incorrect FabricClient count.");
			Assert.AreEqual(pCount+1, vExportThreads.Count, "Incorrect Export count.");
			Assert.AreEqual("dataProv", vFabClientKeys[0], "Incorrect FabricClient ConfigKey used.");

			for ( int i = 1 ; i < pCount+1 ; ++i ) {
				Assert.Null(vFabClientKeys[i], "FabClientKeys should be null @ "+i+".");
				
				Assert.NotNull(vExportCtxs[i], "ExporterContext should be filled @ "+i+".");
				Assert.AreEqual(vSvcCtx.SessProv, vExportCtxs[i].SessProv,
					"Incorrect ExporterContext.SessProv @ "+i+".");
				Assert.AreEqual(vSvcCtx.Query, vExportCtxs[i].Query,
					"Incorrect ExporterContext.Query @ "+i+".");
				Assert.NotNull(vExportCtxs[i].SavedSession,
					"ExporterContext.SavedSession should be filled @ "+i+".");

				IFabricClient fab = vExportFabClients[i];
				Assert.AreEqual(vMockFabClients[i].Object, fab,
					"Incorrect Exporter FabClient @ "+i+".");
				Assert.Null(vExportUsers[i], "Exporter User should be null @ "+i+".");

				Assert.AreNotEqual(Thread.CurrentThread, vExportThreads[i],
					"Exporter should occur in its own thread @ "+i+".");

				FabricPersonSession fps = list[i-1];
				vMockQuery.Verify(x => x.TurnOffSessionUpdate(mockSess.Object, fps), Times.Once());
				vMockQuery.Verify(x => x.GetCurrentUser(mockSess.Object, fab), Times.Once());
			}

			vMockExport.Verify(x => x.SendAll(0), Times.Exactly(pCount+1));
			vMockQuery.Verify(x => x.TurnOffSessionUpdate(
				mockSess.Object, It.IsAny<FabricPersonSession>()), Times.Exactly(pCount));
			vMockQuery.Verify(x => x.GetCurrentUser(
				mockSess.Object, It.IsAny<IFabricClient>()), Times.Exactly(pCount));
		}

		/*--------------------------------------------------------------------------------------------*/
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(8)]
		public void DeleteOldSessions(int pCount) {
			var mockSess = new Mock<ISession>();
			vMockSessProv.Setup(x => x.OpenSession()).Returns(mockSess.Object);

			var list = new List<FabricPersonSession>();

			for ( int i = 0 ; i < pCount ; ++i ) {
				var fps = new FabricPersonSession();
				fps.SessionId = "sessionId"+i;
				list.Add(fps);
			}

			vMockQuery.Setup(x => x.FindExpiredSessions(mockSess.Object)).Returns(list);

			////

			var svc = NewService();
			svc.DeleteOldSessions();

			////
			
			for ( int i = 1 ; i < pCount+1 ; ++i ) {
				FabricPersonSession fps = list[i-1];
				vMockQuery.Verify(x => x.DeleteSession(mockSess.Object, fps), Times.Once());
			}
			
			vMockQuery.Verify(x => x.DeleteSession(
				mockSess.Object, It.IsAny<FabricPersonSession>()), Times.Exactly(pCount));
		}

		//TEST: Service scenarios where IsSessionActive() returns true


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		[Test]
		public void StopAllThreads() {
			var svc = NewService();
			svc.StopAllThreads();
			Assert.True(Exporter.StopThreads, "Incorrect Exporter.StopThreads.");
		}

	}

}