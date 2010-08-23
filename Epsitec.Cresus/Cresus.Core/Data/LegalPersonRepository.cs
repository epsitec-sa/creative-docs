using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Context;

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


		public IEnumerable<LegalPersonEntity> GetLegalPersonsByRequest(Request request)
		{
			return this.GetGenericPersonsByRequest<LegalPersonEntity> (request);
		}


		public IEnumerable<LegalPersonEntity> GetLegalPersonsByExample(LegalPersonEntity example, int index, int count)
		{
			return this.GetGenericPersonsByExample<LegalPersonEntity> (example, index, count);
		}


		public IEnumerable<LegalPersonEntity> GetLegalPersonsByRequest(Request request, int index, int count)
		{
			return this.GetGenericPersonsByRequest<LegalPersonEntity> (request, index, count);
		}


		public IEnumerable<LegalPersonEntity> GetAllLegalPersons()
		{
			return this.GetAllGenericPersons<LegalPersonEntity> ();
		}


		public IEnumerable<LegalPersonEntity> GetAllLegalPersons(int index, int count)
		{
			return this.GetAllGenericPersons<LegalPersonEntity> (index, count);
		}


		public IEnumerable<LegalPersonEntity> GetLegalPersonsByPreferredLanguage(LanguageEntity preferredLanguage)
		{
			return this.GetGenericPersonsByPreferredLanguage<LegalPersonEntity> (preferredLanguage);
		}


		public IEnumerable<LegalPersonEntity> GetLegalPersonsByPreferredLanguage(LanguageEntity preferredLanguage, int index, int count)
		{
			return this.GetGenericPersonsByPreferredLanguage<LegalPersonEntity> (preferredLanguage, index, count);
		}


		public IEnumerable<LegalPersonEntity> GetLegalPersonsByContacts(params AbstractContactEntity[] contacts)
		{
			return this.GetGenericPersonsByContacts<LegalPersonEntity> (contacts);
		}


		public IEnumerable<LegalPersonEntity> GetLegalPersonsByContacts(int index, int count, params AbstractContactEntity[] contacts)
		{
			return this.GetGenericPersonsByContacts<LegalPersonEntity> (index, count, contacts);
		}


		public IEnumerable<LegalPersonEntity> GetLegalPersonsByLegalPersonType(LegalPersonTypeEntity legalPersonType)
		{
			LegalPersonEntity example = this.CreateLegalPersonExampleByLegalPersonType (legalPersonType);

			return this.GetLegalPersonsByExample (example);
		}


		public IEnumerable<LegalPersonEntity> GetLegalPersonsByLegalPersonType(LegalPersonTypeEntity legalPersonType, int index, int count)
		{
			LegalPersonEntity example = this.CreateLegalPersonExampleByLegalPersonType (legalPersonType);

			return this.GetLegalPersonsByExample (example, index, count);
		}


		public IEnumerable<LegalPersonEntity> GetLegalPersonsByName(FormattedText name)
		{
			LegalPersonEntity example = this.CreateLegalPersonExampleByName (name);

			return this.GetLegalPersonsByExample (example);
		}


		public IEnumerable<LegalPersonEntity> GetLegalPersonsByName(FormattedText name, int index, int count)
		{
			LegalPersonEntity example = this.CreateLegalPersonExampleByName (name);

			return this.GetLegalPersonsByExample (example, index, count);
		}


		public IEnumerable<LegalPersonEntity> GetLegalPersonsByShortName(FormattedText shortName)
		{
			LegalPersonEntity example = this.CreateLegalPersonExampleByShortName (shortName);

			return this.GetLegalPersonsByExample (example);
		}


		public IEnumerable<LegalPersonEntity> GetLegalPersonsByShortName(FormattedText shortName, int index, int count)
		{
			LegalPersonEntity example = this.CreateLegalPersonExampleByShortName (shortName);

			return this.GetLegalPersonsByExample (example, index, count);
		}


		public IEnumerable<LegalPersonEntity> GetLegalPersonsByComplement(FormattedText complement)
		{
			LegalPersonEntity example = this.CreateLegalPersonExampleByComplement (complement);

			return this.GetLegalPersonsByExample (example);
		}


		public IEnumerable<LegalPersonEntity> GetLegalPersonsByComplement(FormattedText complement, int index, int count)
		{
			LegalPersonEntity example = this.CreateLegalPersonExampleByComplement (complement);

			return this.GetLegalPersonsByExample (example, index, count);
		}


		public LegalPersonEntity CreateLegalPersonExample()
		{
			return this.CreateGenericPersonExample<LegalPersonEntity> ();
		}



		private LegalPersonEntity CreateLegalPersonExampleByLegalPersonType(LegalPersonTypeEntity legalPersonType)
		{
			LegalPersonEntity example = this.CreateLegalPersonExample ();
			example.LegalPersonType = legalPersonType;

			return example;
		}


		private LegalPersonEntity CreateLegalPersonExampleByName(FormattedText name)
		{
			LegalPersonEntity example = this.CreateLegalPersonExample ();
			example.Name = name;

			return example;
		}


		private LegalPersonEntity CreateLegalPersonExampleByShortName(FormattedText shortName)
		{
			LegalPersonEntity example = this.CreateLegalPersonExample ();
			example.ShortName = shortName;

			return example;
		}


		private LegalPersonEntity CreateLegalPersonExampleByComplement(FormattedText complement)
		{
			LegalPersonEntity example = this.CreateLegalPersonExample ();
			example.Complement = complement;

			return example;
		}


	}


}
