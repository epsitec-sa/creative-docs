using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Data
{


	public class UriContactRepository : AbstractContactRepository
	{


		public UriContactRepository(DataContext dataContext) : base (dataContext)
		{
		}


		public IEnumerable<UriContactEntity> GetUriContactsByExample(UriContactEntity example)
		{
			return this.GetGenericContactsByExample<UriContactEntity> (example);
		}


		public IEnumerable<UriContactEntity> GetUriContactsByExample(UriContactEntity example, int index, int count)
		{
			return this.GetGenericContactsByExample<UriContactEntity> (example, index, count);
		}


		public IEnumerable<UriContactEntity> GetAllUriContacts()
		{
			return this.GetAllGenericContacts<UriContactEntity> ();
		}


		public IEnumerable<UriContactEntity> GetUriContactsByNaturalPerson(NaturalPersonEntity naturalPerson)
		{
			return this.GetGenericContactsByNaturalPerson<UriContactEntity> (naturalPerson);
		}


		public IEnumerable<UriContactEntity> GetUriContactsByLegalPerson(LegalPersonEntity legalPerson)
		{
			return this.GetGenericContactsByLegalPerson<UriContactEntity> (legalPerson);
		}


		public IEnumerable<UriContactEntity> GetUriContactsByRoles(params ContactRoleEntity[] roles)
		{
			return this.GetGenericContactsByRoles<UriContactEntity> (roles);
		}


		public IEnumerable<UriContactEntity> GetUriContactsByComments(params CommentEntity[] comments)
		{
			return this.GetGenericContactsByComments<UriContactEntity> (comments);
		}


		public IEnumerable<UriContactEntity> GetUriContactsByUri(string uri)
		{
			UriContactEntity example = this.CreateUriContactExampleByUri (uri);

			return this.GetUriContactsByExample (example);
		}


		public IEnumerable<UriContactEntity> GetUriContactsByUriScheme(UriSchemeEntity uriScheme)
		{
			UriContactEntity example = this.CreateUriContactExampleByUriScheme (uriScheme);

			return this.GetUriContactsByExample (example);
		}


		public UriContactEntity CreateUriContactExample()
		{
			return this.CreateGenericContactExample<UriContactEntity> ();
		}


		private UriContactEntity CreateUriContactExampleByUri(string uri)
		{
			UriContactEntity example = this.CreateUriContactExample ();
			example.Uri = uri;

			return example;
		}


		private UriContactEntity CreateUriContactExampleByUriScheme(UriSchemeEntity uriScheme)
		{
			UriContactEntity example = this.CreateUriContactExample ();
			example.UriScheme = uriScheme;

			return example;
		}


	}


}
