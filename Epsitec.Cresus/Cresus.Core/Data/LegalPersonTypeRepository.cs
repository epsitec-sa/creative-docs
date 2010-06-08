using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer;

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


		public LegalPersonTypeEntity GetLegalPersonTypeByExample(LegalPersonTypeEntity example)
		{
			return this.GetEntityByExample<LegalPersonTypeEntity> (example);
		}


		public IEnumerable<LegalPersonTypeEntity> GetAllLegalPersonTypes()
		{
			LegalPersonTypeEntity example = this.CreateLegalPersonTypeExample ();

			return this.GetLegalPersonTypesByExample (example);
		}


		public LegalPersonTypeEntity GetLegalPersonTypeByShortName(string shortName)
		{
			LegalPersonTypeEntity example = this.CreateLegalPersonTypeExampleByShortName (shortName);

			return this.GetLegalPersonTypeByExample (example);
		}


		public LegalPersonTypeEntity GetLegalPersonTypeByName(string name)
		{
			LegalPersonTypeEntity example = this.CreateLegalPersonTypeExampleByName (name);

			return this.GetLegalPersonTypeByExample (example);
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
