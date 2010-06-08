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


		public IEnumerable<PersonGenderEntity> GetPersonGendersByExample(PersonGenderEntity example, int index, int count)
		{
			return this.GetEntitiesByExample<PersonGenderEntity> (example, index, count);
		}


		public IEnumerable<PersonGenderEntity> GetAllPersonGenders()
		{
			PersonGenderEntity example = this.CreatePersonGenderExample ();

			return this.GetPersonGendersByExample (example);
		}


		public IEnumerable<PersonGenderEntity> GetPersonGendersByShortCode(string code)
		{
			PersonGenderEntity example = this.CreatePersonGenderExampleByShortCode (code);

			return this.GetPersonGendersByExample (example);
		}


		public IEnumerable<PersonGenderEntity> GetPersonGendersByName(string name)
		{
			PersonGenderEntity example = this.CreatePersonGenderExampleByName (name);

			return this.GetPersonGendersByExample (example);
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
