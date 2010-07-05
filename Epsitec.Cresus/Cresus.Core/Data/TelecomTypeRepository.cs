using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Browser;

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


		public IEnumerable<TelecomTypeEntity> GetTelecomTypesByExample(TelecomTypeEntity example, Request constrainer)
		{
			return this.GetEntitiesByExample<TelecomTypeEntity> (example, constrainer);
		}


		public IEnumerable<TelecomTypeEntity> GetTelecomTypesByExample(TelecomTypeEntity example, int index, int count)
		{
			return this.GetEntitiesByExample<TelecomTypeEntity> (example, index, count);
		}


		public IEnumerable<TelecomTypeEntity> GetTelecomTypesByExample(TelecomTypeEntity example, Request constrainer, int index, int count)
		{
			return this.GetEntitiesByExample<TelecomTypeEntity> (example, constrainer, index, count);
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


		public IEnumerable<TelecomTypeEntity> GetTelecomTypesByName(string name)
		{
			TelecomTypeEntity example = this.CreateTelecomTypeExampleByName (name);

			return this.GetTelecomTypesByExample (example);
		}


		public IEnumerable<TelecomTypeEntity> GetTelecomTypesByName(string name, int index, int count)
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


		private TelecomTypeEntity CreateTelecomTypeExampleByName(string name)
		{
			TelecomTypeEntity example = this.CreateTelecomTypeExample ();
			example.Name = name;

			return example;
		}


	}


}
