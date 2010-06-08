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


		public UriContactEntity GetUriContactByExample(UriContactEntity example)
		{
			return this.GetGenericContactByExample<UriContactEntity> (example);
		}


		public IEnumerable<UriContactEntity> GetAllUriContacts()
		{
			return this.GetAllGenericContacts<UriContactEntity> ();
		}


		public IEnumerable<UriContactEntity> GetUriContactByNaturalPerson(NaturalPersonEntity naturalPerson)
		{
			return this.GetGenericContactByNaturalPerson<UriContactEntity> (naturalPerson);
		}


		public IEnumerable<UriContactEntity> GetUriContactByLegalPerson(LegalPersonEntity legalPerson)
		{
			return this.GetGenericContactByLegalPerson<UriContactEntity> (legalPerson);
		}


		public IEnumerable<UriContactEntity> GetUriContactByRoles(params ContactRoleEntity[] roles)
		{
			return this.GetGenericContactByRoles<UriContactEntity> (roles);
		}


		public IEnumerable<UriContactEntity> GetUriContactByComments(params CommentEntity[] comments)
		{
			return this.GetGenericContactByComments<UriContactEntity> (comments);
		}


		public IEnumerable<UriContactEntity> GetUriContactsByUri(string uri)
		{
			UriContactEntity example = this.CreateUriContactExampleByUri (uri);

			return this.GetUriContactsByExample (example);
		}


		public UriContactEntity GetUriContactByUri(string uri)
		{
			UriContactEntity example = this.CreateUriContactExampleByUri (uri);

			return this.GetUriContactByExample (example);
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
