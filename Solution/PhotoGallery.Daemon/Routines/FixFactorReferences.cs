using System;
using System.Collections.Generic;
using Fabric.Clients.Cs;
using Fabric.Clients.Cs.Api;
using NHibernate;
using PhotoGallery.Database;
using PhotoGallery.Domain;
using PhotoGallery.Infrastructure;

namespace PhotoGallery.Daemon.Routines {

	/*================================================================================================*/
	public class FixFactorReferences {

		private readonly ISessionProvider vSessProv;
		private readonly IFabricClient vDpClient;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public FixFactorReferences(ISessionProvider pSessProv, IFabricClient pDpClient) {
			vSessProv = pSessProv;
			vDpClient = pDpClient;
		}

		/*--------------------------------------------------------------------------------------------*/
		public void Start() {
			//LoadAndSaveArtifacts();
			//UpdateFactors();
		}

		/*--------------------------------------------------------------------------------------------* /
		private void LoadAndSaveArtifacts() {
			using ( ISession s = vSessProv.OpenSession() ) {
				IList<long> artIds = s.QueryOver<FabricFactor>()
					.Where(x => x.RelatedArtifactId > 0)
					.SelectList(sl => sl
						.SelectGroup(x => x.RelatedArtifactId)
					)
					.List<long>();

				foreach ( long artId in artIds ) {
					Log.Debug("Finding "+artId);
					long id = artId;
					FabricArtifact art = s.QueryOver<FabricArtifact>()
						.Where(x => x.ArtifactId == id)
						.SingleOrDefault();

					if ( art != null ) {
						Log.Debug(" * ALREADY STORED!");
						continue;
					}
					
					////

					art = new FabricArtifact();

					FabClass fc = vDpClient.Services.Traversal.GetRootStep.ClassId(artId)
						.Get().FirstDataItem();

					if ( fc != null ) {
						Log.Debug(" * Class: "+fc.Name);
						art.ArtifactId = fc.ArtifactId;
						art.Name = fc.Name;
						art.Disamb = fc.Disamb;
						art.Note = fc.Note;
						art.Type = (byte)FabricArtifact.ArtifactType.FabClass;
						
						s.Save(art);
						continue;
					}

					////

					FabInstance fi = vDpClient.Services.Traversal.GetRootStep.InstanceId(artId)
						.Get().FirstDataItem();
						
					if ( fi == null ) {
						throw new Exception("Unknown artifact type for: "+artId);
					}

					Log.Debug(" * Instance: "+fi.Name);
					art.ArtifactId = fi.ArtifactId;
					art.Name = fi.Name;
					art.Disamb = fi.Disamb;
					art.Note = fi.Note;
					art.Type = (byte)FabricArtifact.ArtifactType.FabInstance;
					s.Save(art);
				}
			}
		}

		/*--------------------------------------------------------------------------------------------* /
		private void UpdateFactors() {
			using ( ISession s = vSessProv.OpenSession() ) {
				IList<long> artIds = s.QueryOver<FabricFactor>()
					.Where(x => x.RelatedArtifactId > 0)
					.SelectList(sl => sl
						.SelectGroup(x => x.RelatedArtifactId)
					)
					.List<long>();

				var map = new Dictionary<long, FabricArtifact>();

				foreach ( long artId in artIds ) {
					long id = artId;
					FabricArtifact art = s.QueryOver<FabricArtifact>()
						.Where(x => x.ArtifactId == id)
						.SingleOrDefault();

					if ( art == null ) {
						throw new Exception("Artifact not saved: "+artId);
					}

					if ( art.ArtifactId != artId ) {
						throw new Exception("Incorrect Artifact mapping: "+artId+" => "+art.Id);
					}

					map.Add(artId, art);
					Log.Debug("Map: "+artId+" => "+art.Id);
				}

				////

				for ( int i = 0 ; i < artIds.Count ; ++i ) {
					long artId = artIds[i];

					IList<FabricFactor> facList = s.QueryOver<FabricFactor>()
						.Where(x => x.RelatedArtifactId == artId)
						.List();

					Log.Debug(i+"/"+artIds.Count+" | Updating "+facList.Count+" Factors: "+artId);

					if ( facList.Count > 100 ) {
						Log.Debug("-- SKIP!");
						continue;
					}

					using ( ITransaction tx = s.BeginTransaction() ) {
						foreach ( FabricFactor ff in facList ) {
							if ( ff.RelatedArtifactId != artId ) {
								throw new Exception("Incorrect RelatedArtifactId: "+
									ff.RelatedArtifactId+" != "+artId);
							}

							ff.Related = map[artId];
							ff.RelatedArtifactId = 0;
							s.Update(ff);
						}

						tx.Commit();
					}
				}
			}
		}*/

	}

}