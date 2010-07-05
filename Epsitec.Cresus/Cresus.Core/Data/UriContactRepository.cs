using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Browser;

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


		public IEnumerable<UriContactEntity> GetUriContactsByExample(UriContactEntity example, Request constrainer)
		{
			return this.GetGenericContactsByExample<UriContactEntity> (example, constrainer);
		}


		public IEnumerable<UriContactEntity> GetUriContactsByExample(UriContactEntity example, int index, int count)
		{
			return this.GetGenericContactsByExample<UriContactEntity> (example, index, count);
		}


		public IEnumerable<UriContactEntity> GetUriContactsByExample(UriContactEntity example, Request constrainer, int index, int count)
		{
			return this.GetGenericContactsByExample<UriContactEntity> (example, constrainer, index, count);
		}


		public IEnumerable<UriContactEntity> GetAllUriContacts()
		{
			return this.GetAllGenericContacts<UriContactEntity> ();
		}


		public IEnumerable<UriContactEntity> GetAllUriContacts(int index, int count)
		{
			return this.GetAllGenericContacts<UriContactEntity> (index, count);
		}


		public IEnumerable<UriContactEntity> GetUriContactsByNaturalPerson(NaturalPersonEntity naturalPerson)
		{
			return this.GetGenericContactsByNaturalPerson<UriContactEntity> (naturalPerson);
		}


		public IEnumerable<UriContactEntity> GetUriContactsByNaturalPerson(NaturalPersonEntity naturalPerson, int index, int count)
		{
			return this.GetGenericContactsByNaturalPerson<UriContactEntity> (naturalPerson, index, count);
		}


		public IEnumerable<UriContactEntity> GetUriContactsByLegalPerson(LegalPersonEntity legalPerson)
		{
			return this.GetGenericContactsByLegalPerson<UriContactEntity> (legalPerson);
		}


		public IEnumerable<UriContactEntity> GetUriContactsByLegalPerson(LegalPersonEntity legalPerson, int index, int count)
		{
			return this.GetGenericContactsByLegalPerson<UriContactEntity> (legalPerson, index, count);
		}


		public IEnumerable<UriContactEntity> GetUriContactsByRoles(params ContactRoleEntity[] roles)
		{
			return this.GetGenericContactsByRoles<UriContactEntity> (roles);
		}


		public IEnumerable<UriContactEntity> GetUriContactsByRoles(int index, int count, params ContactRoleEntity[] roles)
		{
			return this.GetGenericContactsByRoles<UriContactEntity> (index, count, roles);
		}


		public IEnumerable<UriContactEntity> GetUriContactsByComments(params CommentEntity[] comments)
		{
			return this.GetGenericContactsByComments<UriContactEntity> (comments);
		}


		public IEnumerable<UriContactEntity> GetUriContactsByComments(int index, int count, params CommentEntity[] comments)
		{
			return this.GetGenericContactsByComments<UriContactEntity> (index, count, comments);
		}


		public IEnumerable<UriContactEntity> GetUriContactsByUri(string uri)
		{
			UriContactEntity example = this.CreateUriContactExampleByUri (uri);

			return this.GetUriContactsByExample (example);
		}


		public IEnumerable<UriContactEntity> GetUriContactsByUri(string uri, int index, int count)
		{
			UriContactEntity example = this.CreateUriContactExampleByUri (uri);

			return this.GetUriContactsByExample (example, index, count);
		}


		public IEnumerable<UriContactEntity> GetUriContactsByUriScheme(UriSchemeEntity uriScheme)
		{
			UriContactEntity example = this.CreateUriContactExampleByUriScheme (uriScheme);

			return this.GetUriContactsByExample (example);
		}


		public IEnumerable<UriContactEntity> GetUriContactsByUriScheme(UriSchemeEntity uriScheme, int index, int count)
		{
			UriContactEntity example = this.CreateUriContactExampleByUriScheme (uriScheme);

			return this.GetUriContactsByExample (example, index, count);
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
