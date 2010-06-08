using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer;

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


		public PersonTitleEntity GetPersonTitleByExample(PersonTitleEntity example)
		{
			return this.GetEntityByExample<PersonTitleEntity> (example);
		}


		public IEnumerable<PersonTitleEntity> GetAllPersonTitles()
		{
			PersonTitleEntity example = this.CreatePersonTitleExample ();

			return this.GetPersonTitlesByExample (example);
		}


		public PersonTitleEntity GetPersonTitleByShortName(string shortName)
		{
			PersonTitleEntity example = this.CreatePersonTitleExampleByShortName (shortName);

			return this.GetPersonTitleByExample (example);
		}


		public PersonTitleEntity GetPersonTitleByName(string name)
		{
			PersonTitleEntity example = this.CreatePersonTitleExampleByName (name);

			return this.GetPersonTitleByExample (example);
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
