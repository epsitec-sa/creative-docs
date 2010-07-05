using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Browser;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Data
{


	public class CaseEventTypeRepository : Repository
	{


		public CaseEventTypeRepository(DataContext dataContext)
			: base (dataContext)
		{
		}


		public IEnumerable<CaseEventTypeEntity> GetCaseEventTypesByExample(CaseEventTypeEntity example)
		{
			return this.GetEntitiesByExample<CaseEventTypeEntity> (example);
		}


		public IEnumerable<CaseEventTypeEntity> GetCaseEventTypesByRequest(Request request)
		{
			return this.GetEntitiesByRequest<CaseEventTypeEntity> (request);
		}


		public IEnumerable<CaseEventTypeEntity> GetCaseEventTypesByExample(CaseEventTypeEntity example, int index, int count)
		{
			return this.GetEntitiesByExample<CaseEventTypeEntity> (example, index, count);
		}


		public IEnumerable<CaseEventTypeEntity> GetCaseEventTypesByRequest(Request request, int index, int count)
		{
			return this.GetEntitiesByRequest<CaseEventTypeEntity> (request, index, count);
		}


		public IEnumerable<CaseEventTypeEntity> GetAllCaseEventTypes()
		{
			CaseEventTypeEntity example = this.CreateCaseEventTypeExample ();

			return this.GetCaseEventTypesByExample (example);
		}


		public IEnumerable<CaseEventTypeEntity> GetAllCaseEventTypes(int index, int count)
		{
			CaseEventTypeEntity example = this.CreateCaseEventTypeExample ();

			return this.GetCaseEventTypesByExample (example, index, count);
		}


		public IEnumerable<CaseEventTypeEntity> GetCaseEventTypesByName(string name)
		{
			CaseEventTypeEntity example = this.CreateCaseEventTypeExampleByName (name);

			return this.GetCaseEventTypesByExample (example);
		}


		public IEnumerable<CaseEventTypeEntity> GetCaseEventTypesByName(string name, int index, int count)
		{
			CaseEventTypeEntity example = this.CreateCaseEventTypeExampleByName (name);

			return this.GetCaseEventTypesByExample (example, index, count);
		}


		public CaseEventTypeEntity CreateCaseEventTypeExample()
		{
			return this.CreateExample<CaseEventTypeEntity> ();
		}


		private CaseEventTypeEntity CreateCaseEventTypeExampleByName(string name)
		{
			CaseEventTypeEntity example = this.CreateCaseEventTypeExample ();
			example.Code = name;

			return example;
		}


	}


}
