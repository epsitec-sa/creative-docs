using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer;

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


		public MailContactEntity GetMailContactByExample(MailContactEntity example)
		{
			return this.GetGenericContactByExample<MailContactEntity> (example);
		}


		public IEnumerable<MailContactEntity> GetAllMailContacts()
		{
			return this.GetAllGenericContacts<MailContactEntity> ();
		}


		public IEnumerable<MailContactEntity> GetMailContactByNaturalPerson(NaturalPersonEntity naturalPerson)
		{
			return this.GetGenericContactByNaturalPerson<MailContactEntity> (naturalPerson);
		}


		public IEnumerable<MailContactEntity> GetMailContactByLegalPerson(LegalPersonEntity legalPerson)
		{
			return this.GetGenericContactByLegalPerson<MailContactEntity> (legalPerson);
		}


		public IEnumerable<MailContactEntity> GetMailContactByRoles(params ContactRoleEntity[] roles)
		{
			return this.GetGenericContactByRoles<MailContactEntity> (roles);
		}


		public IEnumerable<MailContactEntity> GetMailContactByComments(params CommentEntity[] comments)
		{
			return this.GetGenericContactByComments<MailContactEntity> (comments);
		}


		public IEnumerable<MailContactEntity> GetMailContactsByComplement(string complement)
		{
			MailContactEntity example = this.CreateMailContactExampleByComplement (complement);

			return this.GetMailContactsByExample (example);
		}


		public MailContactEntity GetMailContactByComplement(string complement)
		{
			MailContactEntity example = this.CreateMailContactExampleByComplement (complement);

			return this.GetMailContactByExample (example);
		}


		public IEnumerable<MailContactEntity> GetMailContactsByAddress(AddressEntity address)
		{
			MailContactEntity example = this.CreateMailContactExampleByAddress (address);

			return this.GetMailContactsByExample (example);
		}


		public MailContactEntity GetMailContactByAddress(AddressEntity address)
		{
			MailContactEntity example = this.CreateMailContactExampleByAddress (address);

			return this.GetMailContactByExample (example);
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
