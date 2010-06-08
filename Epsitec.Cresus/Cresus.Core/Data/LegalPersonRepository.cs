using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Data
{


	public class LegalPersonRepository : AbstractPersonRepository
	{


		public LegalPersonRepository(DataContext dataContext) : base (dataContext)
		{
		}


		public IEnumerable<LegalPersonEntity> GetLegalPersonsByExample(LegalPersonEntity example)
		{
			return this.GetGenericPersonsByExample<LegalPersonEntity> (example);
		}


		public IEnumerable<LegalPersonEntity> GetLegalPersonsByExample(LegalPersonEntity example, int index, int count)
		{
			return this.GetGenericPersonsByExample<LegalPersonEntity> (example, index, count);
		}


		public IEnumerable<LegalPersonEntity> GetAllLegalPersons()
		{
			return this.GetAllGenericPersons<LegalPersonEntity> ();
		}


		public IEnumerable<LegalPersonEntity> GetLegalPersonsByPreferredLanguage(LanguageEntity preferredLanguage)
		{
			return this.GetGenericPersonsByPreferredLanguage<LegalPersonEntity> (preferredLanguage);
		}


		public IEnumerable<LegalPersonEntity> GetLegalPersonsByContacts(params AbstractContactEntity[] contacts)
		{
			return this.GetGenericPersonsByContacts<LegalPersonEntity> (contacts);
		}


		public IEnumerable<LegalPersonEntity> GetLegalPersonsByParent(LegalPersonEntity parent)
		{
			LegalPersonEntity example = this.CreateLegalPersonExampleByParent (parent);

			return this.GetLegalPersonsByExample (example);
		}


		public IEnumerable<LegalPersonEntity> GetLegalPersonsByLegalPersonType(LegalPersonTypeEntity legalPersonType)
		{
			LegalPersonEntity example = this.CreateLegalPersonExampleByLegalPersonType (legalPersonType);

			return this.GetLegalPersonsByExample (example);
		}


		public IEnumerable<LegalPersonEntity> GetLegalPersonsByName(string name)
		{
			LegalPersonEntity example = this.CreateLegalPersonExampleByName (name);

			return this.GetLegalPersonsByExample (example);
		}


		public IEnumerable<LegalPersonEntity> GetLegalPersonsByShortName(string shortName)
		{
			LegalPersonEntity example = this.CreateLegalPersonExampleByShortName (shortName);

			return this.GetLegalPersonsByExample (example);
		}


		public IEnumerable<LegalPersonEntity> GetLegalPersonsByComplement(string complement)
		{
			LegalPersonEntity example = this.CreateLegalPersonExampleByComplement (complement);

			return this.GetLegalPersonsByExample (example);
		}


		public LegalPersonEntity CreateLegalPersonExample()
		{
			return this.CreateGenericPersonExample<LegalPersonEntity> ();
		}


		private LegalPersonEntity CreateLegalPersonExampleByParent(LegalPersonEntity parent)
		{
			LegalPersonEntity example = this.CreateLegalPersonExample ();
			example.Parent = parent;

			return example;
		}


		private LegalPersonEntity CreateLegalPersonExampleByLegalPersonType(LegalPersonTypeEntity legalPersonType)
		{
			LegalPersonEntity example = this.CreateLegalPersonExample ();
			example.LegalPersonType = legalPersonType;

			return example;
		}


		private LegalPersonEntity CreateLegalPersonExampleByName(string name)
		{
			LegalPersonEntity example = this.CreateLegalPersonExample ();
			example.Name = name;

			return example;
		}


		private LegalPersonEntity CreateLegalPersonExampleByShortName(string shortName)
		{
			LegalPersonEntity example = this.CreateLegalPersonExample ();
			example.ShortName = shortName;

			return example;
		}


		private LegalPersonEntity CreateLegalPersonExampleByComplement(string complement)
		{
			LegalPersonEntity example = this.CreateLegalPersonExample ();
			example.Complement = complement;

			return example;
		}


	}


}
