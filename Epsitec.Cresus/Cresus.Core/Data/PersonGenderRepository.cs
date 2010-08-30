using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Data
{
	public class PersonGenderRepository : Repository<PersonGenderEntity>
	{
		public PersonGenderRepository(DataContext dataContext) : base (dataContext)
		{
		}


		public IEnumerable<PersonGenderEntity> GetPersonGendersByExample(PersonGenderEntity example)
		{
			return this.GetEntitiesByExample<PersonGenderEntity> (example);
		}


		public IEnumerable<PersonGenderEntity> GetPersonGendersByRequest(Request request)
		{
			return this.GetEntitiesByRequest<PersonGenderEntity> (request);
		}


		public IEnumerable<PersonGenderEntity> GetPersonGendersByExample(PersonGenderEntity example, int index, int count)
		{
			return this.GetEntitiesByExample<PersonGenderEntity> (example, index, count);
		}


		public IEnumerable<PersonGenderEntity> GetPersonGendersByRequest(Request request, int index, int count)
		{
			return this.GetEntitiesByRequest<PersonGenderEntity> (request, index, count);
		}


		public IEnumerable<PersonGenderEntity> GetAllPersonGenders()
		{
			PersonGenderEntity example = this.CreatePersonGenderExample ();

			return this.GetPersonGendersByExample (example);
		}


		public IEnumerable<PersonGenderEntity> GetAllPersonGenders(int index, int count)
		{
			PersonGenderEntity example = this.CreatePersonGenderExample ();

			return this.GetPersonGendersByExample (example, index, count);
		}


		public IEnumerable<PersonGenderEntity> GetPersonGendersByShortCode(string code)
		{
			PersonGenderEntity example = this.CreatePersonGenderExampleByShortCode (code);

			return this.GetPersonGendersByExample (example);
		}


		public IEnumerable<PersonGenderEntity> GetPersonGendersByShortCode(string code, int index, int count)
		{
			PersonGenderEntity example = this.CreatePersonGenderExampleByShortCode (code);

			return this.GetPersonGendersByExample (example, index, count);
		}


		public IEnumerable<PersonGenderEntity> GetPersonGendersByName(FormattedText name)
		{
			PersonGenderEntity example = this.CreatePersonGenderExampleByName (name);

			return this.GetPersonGendersByExample (example);
		}


		public IEnumerable<PersonGenderEntity> GetPersonGendersByName(FormattedText name, int index, int count)
		{
			PersonGenderEntity example = this.CreatePersonGenderExampleByName (name);

			return this.GetPersonGendersByExample (example, index, count);
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


		private PersonGenderEntity CreatePersonGenderExampleByName(FormattedText name)
		{
			PersonGenderEntity example = this.CreatePersonGenderExample ();
			example.Name = name;

			return example;
		}


	}


}
