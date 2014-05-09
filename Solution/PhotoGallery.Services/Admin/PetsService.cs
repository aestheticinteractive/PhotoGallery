using System.Collections.Generic;
using Fabric.Clients.Cs;
using Fabric.Clients.Cs.Api;
using NHibernate;
using PhotoGallery.Domain;
using PhotoGallery.Services.Account.Tools;

namespace PhotoGallery.Services.Admin {
	
	/*================================================================================================*/
	public class PetsService : BaseService {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public PetsService(IFabricClient pFab) : base(pFab) {}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public void AddDog(string pName, string pDisamb, string pNote, long pAnimalTypeArtifactId,
																	IList<long> pBelongsToArtifactIds) {
			using ( ISession s = NewSession() ) {
				using ( ITransaction tx = s.BeginTransaction() ) {
					var fa = new FabricArtifact();
					fa.Type = (byte)FabricArtifact.ArtifactType.Tag;
					fa.Name = pName;
					fa.Disamb = pDisamb;
					fa.Note = pNote;
					s.Save(fa);

					var t = new Tag();
					t.Type = (byte)Tag.TagType.Pet;
					t.Name = fa.Name;
					t.FabricArtifact = fa;
					s.Save(t);

					var fb = new FabricFactorBuilder(null, "'"+pName+"' is an instance of <animal "+
						pAnimalTypeArtifactId+">");
					fb.Init(
						fa,
						DescriptorTypeId.IsAnInstanceOf,
						pAnimalTypeArtifactId,
						FactorAssertionId.Fact,
						true
					);
					s.Save(fb.ToFactor());

					foreach ( long artId in pBelongsToArtifactIds ) {
						fb = new FabricFactorBuilder(null, "'"+pName+
							"' belongs to (<pet>) <person "+artId+">");
						fb.Init(
							fa,
							DescriptorTypeId.BelongsTo,
							artId,
							FactorAssertionId.Fact,
							true
						);
						fb.DesTypeRefineId = LiveArtifactId.Pet;
						s.Save(fb.ToFactor());
					}

					tx.Commit();
				}
			}
		}

	}

}