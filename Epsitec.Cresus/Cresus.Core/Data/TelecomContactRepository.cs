using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Browser;

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


		public IEnumerable<TelecomContactEntity> GetTelecomContactsByRequest(Request request)
		{
			return this.GetGenericContactsByRequest<TelecomContactEntity> (request);
		}


		public IEnumerable<TelecomContactEntity> GetTelecomContactsByExample(TelecomContactEntity example, int index, int count)
		{
			return this.GetGenericContactsByExample<TelecomContactEntity> (example, index, count);
		}


		public IEnumerable<TelecomContactEntity> GetTelecomContactsByRequest(Request request, int index, int count)
		{
			return this.GetGenericContactsByRequest<TelecomContactEntity> (request, index, count);
		}


		public IEnumerable<TelecomContactEntity> GetAllTelecomContacts()
		{
			return this.GetAllGenericContacts<TelecomContactEntity> ();
		}


		public IEnumerable<TelecomContactEntity> GetAllTelecomContacts(int index, int count)
		{
			return this.GetAllGenericContacts<TelecomContactEntity> (index, count);
		}


		public IEnumerable<TelecomContactEntity> GetTelecomContactsByNaturalPerson(NaturalPersonEntity naturalPerson)
		{
			return this.GetGenericContactsByNaturalPerson<TelecomContactEntity> (naturalPerson);
		}


		public IEnumerable<TelecomContactEntity> GetTelecomContactsByNaturalPerson(NaturalPersonEntity naturalPerson, int index, int count)
		{
			return this.GetGenericContactsByNaturalPerson<TelecomContactEntity> (naturalPerson, index, count);
		}


		public IEnumerable<TelecomContactEntity> GetTelecomContactsByLegalPerson(LegalPersonEntity legalPerson)
		{
			return this.GetGenericContactsByLegalPerson<TelecomContactEntity> (legalPerson);
		}


		public IEnumerable<TelecomContactEntity> GetTelecomContactsByLegalPerson(LegalPersonEntity legalPerson, int index, int count)
		{
			return this.GetGenericContactsByLegalPerson<TelecomContactEntity> (legalPerson, index, count);
		}


		public IEnumerable<TelecomContactEntity> GetTelecomContactsByRoles(params ContactRoleEntity[] roles)
		{
			return this.GetGenericContactsByRoles<TelecomContactEntity> (roles);
		}


		public IEnumerable<TelecomContactEntity> GetTelecomContactsByRoles(int index, int count, params ContactRoleEntity[] roles)
		{
			return this.GetGenericContactsByRoles<TelecomContactEntity> (index, count, roles);
		}


		public IEnumerable<TelecomContactEntity> GetTelecomContactsByComments(params CommentEntity[] comments)
		{
			return this.GetGenericContactsByComments<TelecomContactEntity> (comments);
		}


		public IEnumerable<TelecomContactEntity> GetTelecomContactsByComments(int index, int count, params CommentEntity[] comments)
		{
			return this.GetGenericContactsByComments<TelecomContactEntity> (index, count, comments);
		}


		public IEnumerable<TelecomContactEntity> GetTelecomContactsByNumber(string number)
		{
			TelecomContactEntity example = this.CreateTelecomContactExampleByNumber (number);

			return this.GetTelecomContactsByExample (example);
		}


		public IEnumerable<TelecomContactEntity> GetTelecomContactsByNumber(string number, int index, int count)
		{
			TelecomContactEntity example = this.CreateTelecomContactExampleByNumber (number);

			return this.GetTelecomContactsByExample (example, index, count);
		}


		public IEnumerable<TelecomContactEntity> GetTelecomContactsByExtension(string extension)
		{
			TelecomContactEntity example = this.CreateTelecomContactExampleByExtension (extension);

			return this.GetTelecomContactsByExample (example);
		}


		public IEnumerable<TelecomContactEntity> GetTelecomContactsByExtension(string extension, int index, int count)
		{
			TelecomContactEntity example = this.CreateTelecomContactExampleByExtension (extension);

			return this.GetTelecomContactsByExample (example, index, count);
		}


		public IEnumerable<TelecomContactEntity> GetTelecomContactsByTelecomScheme(TelecomTypeEntity telecomType)
		{
			TelecomContactEntity example = this.CreateTelecomContactExampleByTelecomType (telecomType);

			return this.GetTelecomContactsByExample (example);
		}


		public IEnumerable<TelecomContactEntity> GetTelecomContactsByTelecomScheme(TelecomTypeEntity telecomType, int index, int count)
		{
			TelecomContactEntity example = this.CreateTelecomContactExampleByTelecomType (telecomType);

			return this.GetTelecomContactsByExample (example, index, count);
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
