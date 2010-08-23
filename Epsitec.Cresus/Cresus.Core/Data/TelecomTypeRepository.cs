using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Data
{


	public class TelecomTypeRepository : Repository
	{


		public TelecomTypeRepository(DataContext dataContext) : base (dataContext)
		{	
		}


		public IEnumerable<TelecomTypeEntity> GetTelecomTypesByExample(TelecomTypeEntity example)
		{
			return this.GetEntitiesByExample<TelecomTypeEntity> (example);
		}


		public IEnumerable<TelecomTypeEntity> GetTelecomTypesByRequest(Request request)
		{
			return this.GetEntitiesByRequest<TelecomTypeEntity> (request);
		}


		public IEnumerable<TelecomTypeEntity> GetTelecomTypesByExample(TelecomTypeEntity example, int index, int count)
		{
			return this.GetEntitiesByExample<TelecomTypeEntity> (example, index, count);
		}


		public IEnumerable<TelecomTypeEntity> GetTelecomTypesByRequest(Request request, int index, int count)
		{
			return this.GetEntitiesByRequest<TelecomTypeEntity> (request, index, count);
		}


		public IEnumerable<TelecomTypeEntity> GetAllTelecomTypes()
		{
			TelecomTypeEntity example = this.CreateTelecomTypeExample ();

			return this.GetTelecomTypesByExample (example);
		}


		public IEnumerable<TelecomTypeEntity> GetAllTelecomTypes(int index, int count)
		{
			TelecomTypeEntity example = this.CreateTelecomTypeExample ();

			return this.GetTelecomTypesByExample (example, index, count);
		}


		public IEnumerable<TelecomTypeEntity> GetTelecomTypesByCode(string code)
		{
			TelecomTypeEntity example = this.CreateTelecomTypeExampleByCode (code);

			return this.GetTelecomTypesByExample (example);
		}


		public IEnumerable<TelecomTypeEntity> GetTelecomTypesByCode(string code, int index, int count)
		{
			TelecomTypeEntity example = this.CreateTelecomTypeExampleByCode (code);

			return this.GetTelecomTypesByExample (example, index, count);
		}


		public IEnumerable<TelecomTypeEntity> GetTelecomTypesByName(FormattedText name)
		{
			TelecomTypeEntity example = this.CreateTelecomTypeExampleByName (name);

			return this.GetTelecomTypesByExample (example);
		}


		public IEnumerable<TelecomTypeEntity> GetTelecomTypesByName(FormattedText name, int index, int count)
		{
			TelecomTypeEntity example = this.CreateTelecomTypeExampleByName (name);

			return this.GetTelecomTypesByExample (example, index, count);
		}


		public TelecomTypeEntity CreateTelecomTypeExample()
		{
			return this.CreateExample<TelecomTypeEntity> ();
		}


		private TelecomTypeEntity CreateTelecomTypeExampleByCode(string code)
		{
			TelecomTypeEntity example = this.CreateTelecomTypeExample ();
			example.Code = code;

			return example;
		}


		private TelecomTypeEntity CreateTelecomTypeExampleByName(FormattedText name)
		{
			TelecomTypeEntity example = this.CreateTelecomTypeExample ();
			example.Name = name;

			return example;
		}


	}


}
