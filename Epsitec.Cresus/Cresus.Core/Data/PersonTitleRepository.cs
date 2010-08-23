using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Context;

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


		public IEnumerable<PersonTitleEntity> GetPersonTitlesBRequest(Request request)
		{
			return this.GetEntitiesByRequest<PersonTitleEntity> (request);
		}


		public IEnumerable<PersonTitleEntity> GetPersonTitlesByExample(PersonTitleEntity example, int index, int count)
		{
			return this.GetEntitiesByExample<PersonTitleEntity> (example, index, count);
		}


		public IEnumerable<PersonTitleEntity> GetPersonTitlesByRequest(Request request, int index, int count)
		{
			return this.GetEntitiesByRequest<PersonTitleEntity> (request, index, count);
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


		public IEnumerable<PersonTitleEntity> GetPersonTitlesByShortName(FormattedText shortName)
		{
			PersonTitleEntity example = this.CreatePersonTitleExampleByShortName (shortName);

			return this.GetPersonTitlesByExample (example);
		}


		public IEnumerable<PersonTitleEntity> GetPersonTitlesByShortName(FormattedText shortName, int index, int count)
		{
			PersonTitleEntity example = this.CreatePersonTitleExampleByShortName (shortName);

			return this.GetPersonTitlesByExample (example, index, count);
		}


		public IEnumerable<PersonTitleEntity> GetPersonTitlesByName(FormattedText name)
		{
			PersonTitleEntity example = this.CreatePersonTitleExampleByName (name);

			return this.GetPersonTitlesByExample (example);
		}


		public IEnumerable<PersonTitleEntity> GetPersonTitlesByName(FormattedText name, int index, int count)
		{
			PersonTitleEntity example = this.CreatePersonTitleExampleByName (name);

			return this.GetPersonTitlesByExample (example, index, count);
		}


		public PersonTitleEntity CreatePersonTitleExample()
		{
			return this.CreateExample<PersonTitleEntity> ();
		}


		private PersonTitleEntity CreatePersonTitleExampleByShortName(FormattedText shortName)
		{
			PersonTitleEntity example = this.CreatePersonTitleExample ();
			example.ShortName = shortName;

			return example;
		}


		private PersonTitleEntity CreatePersonTitleExampleByName(FormattedText name)
		{
			PersonTitleEntity example = this.CreatePersonTitleExample ();
			example.Name = name;

			return example;
		}


	}


}
