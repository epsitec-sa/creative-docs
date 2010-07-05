using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Browser;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Data
{


	public class PersonTitleRepository : Repository
	{


		public PersonTitleRepository(DataContext dataContext)
			: base (dataContext)
		{
		}


		public IEnumerable<PersonTitleEntity> GetPersonTitlesByExample(PersonTitleEntity example)
		{
			return this.GetEntitiesByExample<PersonTitleEntity> (example);
		}


		public IEnumerable<PersonTitleEntity> GetPersonTitlesByExample(PersonTitleEntity example, Request constrainer)
		{
			return this.GetEntitiesByExample<PersonTitleEntity> (example, constrainer);
		}


		public IEnumerable<PersonTitleEntity> GetPersonTitlesByExample(PersonTitleEntity example, int index, int count)
		{
			return this.GetEntitiesByExample<PersonTitleEntity> (example, index, count);
		}


		public IEnumerable<PersonTitleEntity> GetPersonTitlesByExample(PersonTitleEntity example, Request constrainer, int index, int count)
		{
			return this.GetEntitiesByExample<PersonTitleEntity> (example, constrainer, index, count);
		}


		public IEnumerable<PersonTitleEntity> GetAllPersonTitles()
		{
			PersonTitleEntity example = this.CreatePersonTitleExample ();

			return this.GetPersonTitlesByExample (example);
		}


		public IEnumerable<PersonTitleEntity> GetAllPersonTitles(int index, int count)
		{
			PersonTitleEntity example = this.CreatePersonTitleExample ();

			return this.GetPersonTitlesByExample (example, index, count);
		}


		public IEnumerable<PersonTitleEntity> GetPersonTitlesByShortName(string shortName)
		{
			PersonTitleEntity example = this.CreatePersonTitleExampleByShortName (shortName);

			return this.GetPersonTitlesByExample (example);
		}


		public IEnumerable<PersonTitleEntity> GetPersonTitlesByShortName(string shortName, int index, int count)
		{
			PersonTitleEntity example = this.CreatePersonTitleExampleByShortName (shortName);

			return this.GetPersonTitlesByExample (example, index, count);
		}


		public IEnumerable<PersonTitleEntity> GetPersonTitlesByName(string name)
		{
			PersonTitleEntity example = this.CreatePersonTitleExampleByName (name);

			return this.GetPersonTitlesByExample (example);
		}


		public IEnumerable<PersonTitleEntity> GetPersonTitlesByName(string name, int index, int count)
		{
			PersonTitleEntity example = this.CreatePersonTitleExampleByName (name);

			return this.GetPersonTitlesByExample (example, index, count);
		}


		public PersonTitleEntity CreatePersonTitleExample()
		{
			return this.CreateExample<PersonTitleEntity> ();
		}


		private PersonTitleEntity CreatePersonTitleExampleByShortName(string shortName)
		{
			PersonTitleEntity example = this.CreatePersonTitleExample ();
			example.ShortName = shortName;

			return example;
		}


		private PersonTitleEntity CreatePersonTitleExampleByName(string name)
		{
			PersonTitleEntity example = this.CreatePersonTitleExample ();
			example.Name = name;

			return example;
		}


	}


}
