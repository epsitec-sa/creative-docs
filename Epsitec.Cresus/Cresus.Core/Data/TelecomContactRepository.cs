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


		public IEnumerable<TelecomContactEntity> GetTelecomContactsByExample(TelecomContactEntity example, int index, int count)
		{
			return this.GetGenericContactsByExample<TelecomContactEntity> (example, index, count);
		}


		public IEnumerable<TelecomContactEntity> GetAllTelecomContacts()
		{
			return this.GetAllGenericContacts<TelecomContactEntity> ();
		}


		public IEnumerable<TelecomContactEntity> GetTelecomContactsByNaturalPerson(NaturalPersonEntity naturalPerson)
		{
			return this.GetGenericContactsByNaturalPerson<TelecomContactEntity> (naturalPerson);
		}


		public IEnumerable<TelecomContactEntity> GetTelecomContactsByLegalPerson(LegalPersonEntity legalPerson)
		{
			return this.GetGenericContactsByLegalPerson<TelecomContactEntity> (legalPerson);
		}


		public IEnumerable<TelecomContactEntity> GetTelecomContactsByRoles(params ContactRoleEntity[] roles)
		{
			return this.GetGenericContactsByRoles<TelecomContactEntity> (roles);
		}


		public IEnumerable<TelecomContactEntity> GetTelecomContactsByComments(params CommentEntity[] comments)
		{
			return this.GetGenericContactsByComments<TelecomContactEntity> (comments);
		}


		public IEnumerable<TelecomContactEntity> GetTelecomContactsByNumber(string number)
		{
			TelecomContactEntity example = this.CreateTelecomContactExampleByNumber (number);

			return this.GetTelecomContactsByExample (example);
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
