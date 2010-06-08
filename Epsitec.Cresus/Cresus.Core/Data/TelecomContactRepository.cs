using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Data
{
	
	
	public class TelecomContactRepository : AbstractContactRepository
	{


		public TelecomContactRepository(DataContext dataContext) : base (dataContext)
		{
		}


		public IEnumerable<TelecomContactEntity> GetTelecomContactsByExample(TelecomContactEntity example)
		{
			return this.GetGenericContactsByExample<TelecomContactEntity> (example);
		}


		public TelecomContactEntity GetTelecomContactByExample(TelecomContactEntity example)
		{
			return this.GetGenericContactByExample<TelecomContactEntity> (example);
		}


		public IEnumerable<TelecomContactEntity> GetAllTelecomContacts()
		{
			return this.GetAllGenericContacts<TelecomContactEntity> ();
		}


		public IEnumerable<TelecomContactEntity> GetTelecomContactByNaturalPerson(NaturalPersonEntity naturalPerson)
		{
			return this.GetGenericContactByNaturalPerson<TelecomContactEntity> (naturalPerson);
		}


		public IEnumerable<TelecomContactEntity> GetTelecomContactByLegalPerson(LegalPersonEntity legalPerson)
		{
			return this.GetGenericContactByLegalPerson<TelecomContactEntity> (legalPerson);
		}


		public IEnumerable<TelecomContactEntity> GetTelecomContactByRoles(params ContactRoleEntity[] roles)
		{
			return this.GetGenericContactByRoles<TelecomContactEntity> (roles);
		}


		public IEnumerable<TelecomContactEntity> GetTelecomContactByComments(params CommentEntity[] comments)
		{
			return this.GetGenericContactByComments<TelecomContactEntity> (comments);
		}


		public IEnumerable<TelecomContactEntity> GetTelecomContactsByNumber(string number)
		{
			TelecomContactEntity example = this.CreateTelecomContactExampleByNumber (number);

			return this.GetTelecomContactsByExample (example);
		}


		public TelecomContactEntity GetTelecomContactByNumber(string number)
		{
			TelecomContactEntity example = this.CreateTelecomContactExampleByNumber (number);

			return this.GetTelecomContactByExample (example);
		}


		public IEnumerable<TelecomContactEntity> GetTelecomContactsByExtension(string extension)
		{
			TelecomContactEntity example = this.CreateTelecomContactExampleByExtension (extension);

			return this.GetTelecomContactsByExample (example);
		}


		public IEnumerable<TelecomContactEntity> GetTelecomContactsByTelecomScheme(TelecomTypeEntity telecomType)
		{
			TelecomContactEntity example = this.CreateTelecomContactExampleByTelecomType (telecomType);

			return this.GetTelecomContactsByExample (example);
		}



		public TelecomContactEntity CreateTelecomContactExample()
		{
			return this.CreateGenericContactExample<TelecomContactEntity> ();
		}


		private TelecomContactEntity CreateTelecomContactExampleByNumber(string number)
		{
			TelecomContactEntity example = this.CreateTelecomContactExample ();
			example.Number = number;

			return example;
		}


		private TelecomContactEntity CreateTelecomContactExampleByExtension(string extension)
		{
			TelecomContactEntity example = this.CreateTelecomContactExample ();
			example.Extension = extension;

			return example;
		}


		private TelecomContactEntity CreateTelecomContactExampleByTelecomType(TelecomTypeEntity telecomType)
		{
			TelecomContactEntity example = this.CreateTelecomContactExample ();
			example.TelecomType = telecomType;

			return example;
		}


	}


}
