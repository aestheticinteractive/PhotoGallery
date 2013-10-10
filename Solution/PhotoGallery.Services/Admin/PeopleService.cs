using System.Collections.Generic;
using Fabric.Clients.Cs;
using Fabric.Clients.Cs.Api;
using NHibernate;
using PhotoGallery.Domain;
using PhotoGallery.Services.Account.Tools;
using PhotoGallery.Services.Admin.Dto;

namespace PhotoGallery.Services.Admin {
	
	/*================================================================================================*/
	public class PeopleService : BaseService {

		public enum Gender {
			Unknown,
			Male,
			Female
		};


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public PeopleService(IFabricClient pFab) : base(pFab) {}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public IList<WebPersonTag> GetPersonTags() {
			using ( ISession s = NewSession() ) {
				IList<Tag> tags = s.QueryOver<Tag>()
					.Where(x => x.Type == (byte)Tag.TagType.Person)
					.Fetch(x => x.FabricArtifact).Eager
					.List();

				var perTags = new List<WebPersonTag>();

				foreach ( var t in tags ) {
					perTags.Add(new WebPersonTag(t));
				}

				return perTags;
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		public void AddPersonTag(string pName, Gender pGender) {
			LiveArtifactId perArtId = LiveArtifactId.Person;
			string perStr = "person";

			switch ( pGender ) {
				case Gender.Male:
					perArtId = LiveArtifactId.MalePerson;
					perStr = "male person";
					break;

				case Gender.Female:
					perArtId = LiveArtifactId.FemalePerson;
					perStr = "female person";
					break;
			}

			using ( ISession s = NewSession() ) {
				using ( ITransaction tx = s.BeginTransaction() ) {
					var fa = new FabricArtifact();
					fa.Type = (byte)FabricArtifact.ArtifactType.Tag;
					fa.Name = pName;
					fa.Disamb = perStr;
					s.Save(fa);

					var t = new Tag();
					t.Type = (byte)Tag.TagType.Person;
					t.Name = fa.Name;
					t.FabricArtifact = fa;
					s.Save(t);

					var fb = new FabricFactorBuilder(null, "<person> is an instance of '"+perStr+"'");
					fb.Init(
						fa,
						FabEnumsData.DescriptorTypeId.IsAnInstanceOf,
						perArtId,
						FabEnumsData.FactorAssertionId.Fact,
						true
					);
					s.Save(fb.ToFactor());

					tx.Commit();
				}
			}
		}

	}

}