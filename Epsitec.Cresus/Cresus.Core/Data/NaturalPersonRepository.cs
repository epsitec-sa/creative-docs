using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Data
{


	public class NaturalPersonRepository : AbstractPersonRepository
	{


		public NaturalPersonRepository(DataContext dataContext) : base (dataContext)
		{
		}


		public IEnumerable<NaturalPersonEntity> GetNaturalPersonsByExample(NaturalPersonEntity example)
		{
			return this.GetGenericPersonsByExample<NaturalPersonEntity> (example);
		}


		public IEnumerable<NaturalPersonEntity> GetNaturalPersonsByRequest(Request request)
		{
			return this.GetGenericPersonsByRequest<NaturalPersonEntity> (request);
		}


		public IEnumerable<NaturalPersonEntity> GetNaturalPersonsByExample(NaturalPersonEntity example, int index, int count)
		{
			return this.GetGenericPersonsByExample<NaturalPersonEntity> (example, index, count);
		}


		public IEnumerable<NaturalPersonEntity> GetNaturalPersonsByRequest(Request request, int index, int count)
		{
			return this.GetGenericPersonsByRequest<NaturalPersonEntity> (request, index, count);
		}


		public IEnumerable<NaturalPersonEntity> GetAllNaturalPersons()
		{
			return this.GetAllGenericPersons<NaturalPersonEntity> ();
		}


		public IEnumerable<NaturalPersonEntity> GetAllNaturalPersons(int index, int count)
		{
			return this.GetAllGenericPersons<NaturalPersonEntity> (index, count);
		}


		public IEnumerable<NaturalPersonEntity> GetNaturalPersonsByPreferredLanguage(LanguageEntity preferredLanguage)
		{
			return this.GetGenericPersonsByPreferredLanguage<NaturalPersonEntity> (preferredLanguage);
		}


		public IEnumerable<NaturalPersonEntity> GetNaturalPersonsByPreferredLanguage(LanguageEntity preferredLanguage, int index, int count)
		{
			return this.GetGenericPersonsByPreferredLanguage<NaturalPersonEntity> (preferredLanguage, index, count);
		}


		public IEnumerable<NaturalPersonEntity> GetNaturalPersonsByContacts(params AbstractContactEntity[] contacts)
		{
			return this.GetGenericPersonsByContacts<NaturalPersonEntity> (contacts);
		}


		public IEnumerable<NaturalPersonEntity> GetNaturalPersonsByContacts(int index, int count, params AbstractContactEntity[] contacts)
		{
			return this.GetGenericPersonsByContacts<NaturalPersonEntity> (index, count, contacts);
		}


		public IEnumerable<NaturalPersonEntity> GetNaturalPersonsByTitle(PersonTitleEntity title)
		{
			NaturalPersonEntity example = this.CreateNaturalPersonExampleByTitle (title);

			return this.GetNaturalPersonsByExample (example);
		}


		public IEnumerable<NaturalPersonEntity> GetNaturalPersonsByTitle(PersonTitleEntity title, int index, int count)
		{
			NaturalPersonEntity example = this.CreateNaturalPersonExampleByTitle (title);

			return this.GetNaturalPersonsByExample (example, index, count);
		}


		public IEnumerable<NaturalPersonEntity> GetNaturalPersonsByFirstname(string firstname)
		{
			NaturalPersonEntity example = this.CreateNaturalPersonExampleByFirstname (firstname);

			return this.GetNaturalPersonsByExample (example);
		}


		public IEnumerable<NaturalPersonEntity> GetNaturalPersonsByFirstname(string firstname, int index, int count)
		{
			NaturalPersonEntity example = this.CreateNaturalPersonExampleByFirstname (firstname);

			return this.GetNaturalPersonsByExample (example, index, count);
		}



		public IEnumerable<NaturalPersonEntity> GetNaturalPersonsByLastname(string lastname)
		{
			NaturalPersonEntity example = this.CreateNaturalPersonExampleByLastname (lastname);

			return this.GetNaturalPersonsByExample (example);
		}



		public IEnumerable<NaturalPersonEntity> GetNaturalPersonsByLastname(string lastname, int index, int count)
		{
			NaturalPersonEntity example = this.CreateNaturalPersonExampleByLastname (lastname);

			return this.GetNaturalPersonsByExample (example, index, count);
		}


		public IEnumerable<NaturalPersonEntity> GetNaturalPersonsByGender(PersonGenderEntity gender)
		{
			NaturalPersonEntity example = this.CreateNaturalPersonExampleByGender (gender);

			return this.GetNaturalPersonsByExample (example);
		}


		public IEnumerable<NaturalPersonEntity> GetNaturalPersonsByGender(PersonGenderEntity gender, int index, int count)
		{
			NaturalPersonEntity example = this.CreateNaturalPersonExampleByGender (gender);

			return this.GetNaturalPersonsByExample (example, index, count);
		}


		public IEnumerable<NaturalPersonEntity> GetNaturalPersonsByBirthDate(Date date)
		{
			NaturalPersonEntity example = this.CreateNaturalPersonExampleByBirthDate (date);

			return this.GetNaturalPersonsByExample (example);
		}


		public IEnumerable<NaturalPersonEntity> GetNaturalPersonsByBirthDate(Date date, int index, int count)
		{
			NaturalPersonEntity example = this.CreateNaturalPersonExampleByBirthDate (date);

			return this.GetNaturalPersonsByExample (example, index, count);
		}


		public NaturalPersonEntity CreateNaturalPersonExample()
		{
			return this.CreateGenericPersonExample<NaturalPersonEntity> ();
		}


		private NaturalPersonEntity CreateNaturalPersonExampleByTitle(PersonTitleEntity title)
		{
			NaturalPersonEntity example = this.CreateNaturalPersonExample ();
			example.Title = title;

			return example;
		}


		private NaturalPersonEntity CreateNaturalPersonExampleByFirstname(string firstname)
		{
			NaturalPersonEntity example = this.CreateNaturalPersonExample ();
			example.Firstname = firstname;

			return example;
		}


		private NaturalPersonEntity CreateNaturalPersonExampleByLastname(string lastname)
		{
			NaturalPersonEntity example = this.CreateNaturalPersonExample ();
			example.Lastname = lastname;

			return example;
		}


		private NaturalPersonEntity CreateNaturalPersonExampleByGender(PersonGenderEntity gender)
		{
			NaturalPersonEntity example = this.CreateNaturalPersonExample ();
			example.Gender = gender;

			return example;
		}


		private NaturalPersonEntity CreateNaturalPersonExampleByBirthDate(Date birthDate)
		{
			NaturalPersonEntity example = this.CreateNaturalPersonExample ();
			example.BirthDate = birthDate;

			return example;
		}


	}


}
