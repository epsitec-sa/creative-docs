using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Data
{


	public class MailContactRepository : AbstractContactRepository
	{


		public MailContactRepository(DataContext dataContext) : base (dataContext)
		{
		}


		public IEnumerable<MailContactEntity> GetMailContactsByExample(MailContactEntity example)
		{
			return this.GetGenericContactsByExample<MailContactEntity> (example);
		}


		public IEnumerable<MailContactEntity> GetMailContactsByRequest(Request request)
		{
			return this.GetGenericContactsByRequest<MailContactEntity> (request);
		}


		public IEnumerable<MailContactEntity> GetMailContactsByExample(MailContactEntity example, int index, int count)
		{
			return this.GetGenericContactsByExample<MailContactEntity> (example, index, count);
		}


		public IEnumerable<MailContactEntity> GetMailContactsByRequest(Request request, int index, int count)
		{
			return this.GetGenericContactsByRequest<MailContactEntity> (request, index, count);
		}


		public IEnumerable<MailContactEntity> GetAllMailContacts()
		{
			return this.GetAllGenericContacts<MailContactEntity> ();
		}


		public IEnumerable<MailContactEntity> GetAllMailContacts(int index, int count)
		{
			return this.GetAllGenericContacts<MailContactEntity> (index, count);
		}


		public IEnumerable<MailContactEntity> GetMailContactsByNaturalPerson(NaturalPersonEntity naturalPerson)
		{
			return this.GetGenericContactsByNaturalPerson<MailContactEntity> (naturalPerson);
		}


		public IEnumerable<MailContactEntity> GetMailContactsByNaturalPerson(NaturalPersonEntity naturalPerson, int index, int count)
		{
			return this.GetGenericContactsByNaturalPerson<MailContactEntity> (naturalPerson, index, count);
		}


		public IEnumerable<MailContactEntity> GetMailContactsByLegalPerson(LegalPersonEntity legalPerson)
		{
			return this.GetGenericContactsByLegalPerson<MailContactEntity> (legalPerson);
		}


		public IEnumerable<MailContactEntity> GetMailContactsByLegalPerson(LegalPersonEntity legalPerson, int index, int count)
		{
			return this.GetGenericContactsByLegalPerson<MailContactEntity> (legalPerson, index, count);
		}


		public IEnumerable<MailContactEntity> GetMailContactsByRoles(params ContactRoleEntity[] roles)
		{
			return this.GetGenericContactsByRoles<MailContactEntity> (roles);
		}


		public IEnumerable<MailContactEntity> GetMailContactsByRoles(int index, int count, params ContactRoleEntity[] roles)
		{
			return this.GetGenericContactsByRoles<MailContactEntity> (index, count, roles);
		}


		public IEnumerable<MailContactEntity> GetMailContactsByComments(params CommentEntity[] comments)
		{
			return this.GetGenericContactsByComments<MailContactEntity> (comments);
		}


		public IEnumerable<MailContactEntity> GetMailContactsByComments(int index, int count, params CommentEntity[] comments)
		{
			return this.GetGenericContactsByComments<MailContactEntity> (index, count, comments);
		}


		public IEnumerable<MailContactEntity> GetMailContactsByComplement(string complement)
		{
			MailContactEntity example = this.CreateMailContactExampleByComplement (complement);

			return this.GetMailContactsByExample (example);
		}


		public IEnumerable<MailContactEntity> GetMailContactsByComplement(string complement, int index, int count)
		{
			MailContactEntity example = this.CreateMailContactExampleByComplement (complement);

			return this.GetMailContactsByExample (example, index, count);
		}


		public IEnumerable<MailContactEntity> GetMailContactsByAddress(AddressEntity address)
		{
			MailContactEntity example = this.CreateMailContactExampleByAddress (address);

			return this.GetMailContactsByExample (example);
		}


		public IEnumerable<MailContactEntity> GetMailContactsByAddress(AddressEntity address, int index, int count)
		{
			MailContactEntity example = this.CreateMailContactExampleByAddress (address);

			return this.GetMailContactsByExample (example, index, count);
		}


		public MailContactEntity CreateMailContactExample()
		{
			return this.CreateGenericContactExample<MailContactEntity> ();
		}


		private MailContactEntity CreateMailContactExampleByComplement(string complement)
		{
			MailContactEntity example = this.CreateMailContactExample ();
			example.Complement = complement;

			return example;
		}


		private MailContactEntity CreateMailContactExampleByAddress(AddressEntity address)
		{
			MailContactEntity example = this.CreateMailContactExample ();
			example.Address = address;

			return example;
		}


	}


}
