using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Data
{


	public class PersonGenderRepository : Repository
	{


		public PersonGenderRepository(DataContext dataContext) : base (dataContext)
		{
		}


		public IEnumerable<PersonGenderEntity> GetPersonGendersByExample(PersonGenderEntity example)
		{
			return this.GetEntitiesByExample<PersonGenderEntity> (example);
		}


		public PersonGenderEntity GetPersonGenderByExample(PersonGenderEntity example)
		{
			return this.GetEntityByExample<PersonGenderEntity> (example);
		}


		public IEnumerable<PersonGenderEntity> GetAllPersonGenders()
		{
			PersonGenderEntity example = this.CreatePersonGenderExample ();

			return this.GetPersonGendersByExample (example);
		}


		public PersonGenderEntity GetPersonGenderByShortCode(string code)
		{
			PersonGenderEntity example = this.CreatePersonGenderExampleByShortCode (code);

			return this.GetPersonGenderByExample (example);
		}


		public PersonGenderEntity GetPersonGenderByName(string name)
		{
			PersonGenderEntity example = this.CreatePersonGenderExampleByName (name);

			return this.GetPersonGenderByExample (example);
		}


		public PersonGenderEntity CreatePersonGenderExample()
		{
			return this.CreateExample<PersonGenderEntity> ();
		}


		private PersonGenderEntity CreatePersonGenderExampleByShortCode(string code)
		{
			PersonGenderEntity example = this.CreatePersonGenderExample ();
			example.Code = code;

			return example;
		}


		private PersonGenderEntity CreatePersonGenderExampleByName(string name)
		{
			PersonGenderEntity example = this.CreatePersonGenderExample ();
			example.Name = name;

			return example;
		}


	}


}
