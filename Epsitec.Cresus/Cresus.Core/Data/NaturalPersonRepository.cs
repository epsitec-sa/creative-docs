using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer;

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


		public IEnumerable<NaturalPersonEntity> GetNaturalPersonsByExample(NaturalPersonEntity example, int index, int count)
		{
			return this.GetGenericPersonsByExample<NaturalPersonEntity> (example, index, count);
		}


		public IEnumerable<NaturalPersonEntity> GetAllNaturalPersons()
		{
			return this.GetAllGenericPersons<NaturalPersonEntity> ();
		}


		public IEnumerable<NaturalPersonEntity> GetNaturalPersonsByPreferredLanguage(LanguageEntity preferredLanguage)
		{
			return this.GetGenericPersonsByPreferredLanguage<NaturalPersonEntity> (preferredLanguage);
		}


		public IEnumerable<NaturalPersonEntity> GetNaturalPersonsByContacts(params AbstractContactEntity[] contacts)
		{
			return this.GetGenericPersonsByContacts<NaturalPersonEntity> (contacts);
		}



		public IEnumerable<NaturalPersonEntity> GetNaturalPersonsByTitle(PersonTitleEntity title)
		{
			NaturalPersonEntity example = this.CreateNaturalPersonExampleByTitle (title);

			return this.GetNaturalPersonsByExample (example);
		}



		public IEnumerable<NaturalPersonEntity> GetNaturalPersonsByFirstname(string firstname)
		{
			NaturalPersonEntity example = this.CreateNaturalPersonExampleByFirstname (firstname);

			return this.GetNaturalPersonsByExample (example);
		}



		public IEnumerable<NaturalPersonEntity> GetNaturalPersonsByLastname(string lastname)
		{
			NaturalPersonEntity example = this.CreateNaturalPersonExampleByLastname (lastname);

			return this.GetNaturalPersonsByExample (example);
		}



		public IEnumerable<NaturalPersonEntity> GetNaturalPersonsByGender(PersonGenderEntity gender)
		{
			NaturalPersonEntity example = this.CreateNaturalPersonExampleByGender (gender);

			return this.GetNaturalPersonsByExample (example);
		}



		public IEnumerable<NaturalPersonEntity> GetNaturalPersonsByBirthDate(Date date)
		{
			NaturalPersonEntity example = this.CreateNaturalPersonExampleByBirthDate (date);

			return this.GetNaturalPersonsByExample (example);
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
