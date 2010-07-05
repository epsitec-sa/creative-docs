using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Browser;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Data
{


	public class LegalPersonTypeRepository : Repository
	{


		public LegalPersonTypeRepository(DataContext dataContext) : base (dataContext)
		{
		}


		public IEnumerable<LegalPersonTypeEntity> GetLegalPersonTypesByExample(LegalPersonTypeEntity example)
		{
			return this.GetEntitiesByExample<LegalPersonTypeEntity> (example);
		}


		public IEnumerable<LegalPersonTypeEntity> GetLegalPersonTypesByExample(LegalPersonTypeEntity example, Request constrainer)
		{
			return this.GetEntitiesByExample<LegalPersonTypeEntity> (example, constrainer);
		}


		public IEnumerable<LegalPersonTypeEntity> GetLegalPersonTypesByExample(LegalPersonTypeEntity example, int index, int count)
		{
			return this.GetEntitiesByExample<LegalPersonTypeEntity> (example, index, count);
		}


		public IEnumerable<LegalPersonTypeEntity> GetLegalPersonTypesByExample(LegalPersonTypeEntity example, Request constrainer, int index, int count)
		{
			return this.GetEntitiesByExample<LegalPersonTypeEntity> (example, constrainer, index, count);
		}


		public IEnumerable<LegalPersonTypeEntity> GetAllLegalPersonTypes()
		{
			LegalPersonTypeEntity example = this.CreateLegalPersonTypeExample ();

			return this.GetLegalPersonTypesByExample (example);
		}


		public IEnumerable<LegalPersonTypeEntity> GetAllLegalPersonTypes(int index, int count)
		{
			LegalPersonTypeEntity example = this.CreateLegalPersonTypeExample ();

			return this.GetLegalPersonTypesByExample (example, index, count);
		}


		public IEnumerable<LegalPersonTypeEntity> GetLegalPersonTypesByShortName(string shortName)
		{
			LegalPersonTypeEntity example = this.CreateLegalPersonTypeExampleByShortName (shortName);

			return this.GetLegalPersonTypesByExample (example);
		}


		public IEnumerable<LegalPersonTypeEntity> GetLegalPersonTypesByShortName(string shortName, int index, int count)
		{
			LegalPersonTypeEntity example = this.CreateLegalPersonTypeExampleByShortName (shortName);

			return this.GetLegalPersonTypesByExample (example, index, count);
		}


		public IEnumerable<LegalPersonTypeEntity> GetLegalPersonTypesByName(string name)
		{
			LegalPersonTypeEntity example = this.CreateLegalPersonTypeExampleByName (name);

			return this.GetLegalPersonTypesByExample (example);
		}


		public IEnumerable<LegalPersonTypeEntity> GetLegalPersonTypesByName(string name, int index, int count)
		{
			LegalPersonTypeEntity example = this.CreateLegalPersonTypeExampleByName (name);

			return this.GetLegalPersonTypesByExample (example, index, count);
		}


		public LegalPersonTypeEntity CreateLegalPersonTypeExample()
		{
			return this.CreateExample<LegalPersonTypeEntity> ();
		}


		private LegalPersonTypeEntity CreateLegalPersonTypeExampleByShortName(string shortName)
		{
			LegalPersonTypeEntity example = this.CreateLegalPersonTypeExample ();
			example.ShortName = shortName;

			return example;
		}


		private LegalPersonTypeEntity CreateLegalPersonTypeExampleByName(string name)
		{
			LegalPersonTypeEntity example = this.CreateLegalPersonTypeExample ();
			example.Name = name;

			return example;
		}


	}


}
