using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Browser;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Data
{


	public class LanguageRepository : Repository
	{


		public LanguageRepository(DataContext dataContext)
			: base (dataContext)
		{
		}


		public IEnumerable<LanguageEntity> GetLanguagesByExample(LanguageEntity example)
		{
			return this.GetEntitiesByExample<LanguageEntity> (example);
		}


		public IEnumerable<LanguageEntity> GetLanguagesByExample(LanguageEntity example, Request constrainer)
		{
			return this.GetEntitiesByExample<LanguageEntity> (example, constrainer);
		}


		public IEnumerable<LanguageEntity> GetLanguagesByExample(LanguageEntity example, int index, int count)
		{
			return this.GetEntitiesByExample<LanguageEntity> (example, index, count);
		}


		public IEnumerable<LanguageEntity> GetLanguagesByExample(LanguageEntity example, Request constrainer, int index, int count)
		{
			return this.GetEntitiesByExample<LanguageEntity> (example, constrainer, index, count);
		}


		public IEnumerable<LanguageEntity> GetAllLanguages()
		{
			LanguageEntity example = this.CreateLanguageExample ();

			return this.GetLanguagesByExample (example);
		}


		public IEnumerable<LanguageEntity> GetAllLanguages(int index, int count)
		{
			LanguageEntity example = this.CreateLanguageExample ();

			return this.GetLanguagesByExample (example, index, count);
		}


		public IEnumerable<LanguageEntity> GetLanguagesByShortCode(string code)
		{
			LanguageEntity example = this.CreateLanguageExampleByShortCode (code);

			return this.GetLanguagesByExample (example);
		}


		public IEnumerable<LanguageEntity> GetLanguagesByShortCode(string code, int index, int count)
		{
			LanguageEntity example = this.CreateLanguageExampleByShortCode (code);

			return this.GetLanguagesByExample (example, index, count);
		}


		public IEnumerable<LanguageEntity> GetLanguagesByName(string name)
		{
			LanguageEntity example = this.CreateLanguageExampleByName (name);

			return this.GetLanguagesByExample (example);
		}


		public IEnumerable<LanguageEntity> GetLanguagesByName(string name, int index, int count)
		{
			LanguageEntity example = this.CreateLanguageExampleByName (name);

			return this.GetLanguagesByExample (example, index, count);
		}


		public LanguageEntity CreateLanguageExample()
		{
			return this.CreateExample<LanguageEntity> ();
		}


		private LanguageEntity CreateLanguageExampleByShortCode(string code)
		{
			LanguageEntity example = this.CreateLanguageExample ();
			example.Code = code;

			return example;
		}


		private LanguageEntity CreateLanguageExampleByName(string name)
		{
			LanguageEntity example = this.CreateLanguageExample ();
			example.Name = name;

			return example;
		}


	}


}
