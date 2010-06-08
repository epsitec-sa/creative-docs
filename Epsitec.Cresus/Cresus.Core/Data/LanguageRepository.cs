using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer;

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


		public LanguageEntity GetLanguageByExample(LanguageEntity example)
		{
			return this.GetEntityByExample<LanguageEntity> (example);
		}


		public IEnumerable<LanguageEntity> GetAllLanguages()
		{
			LanguageEntity example = this.CreateLanguageExample ();

			return this.GetLanguagesByExample (example);
		}


		public LanguageEntity GetLanguageByShortCode(string code)
		{
			LanguageEntity example = this.CreateLanguageExampleByShortCode (code);

			return this.GetLanguageByExample (example);
		}


		public LanguageEntity GetLanguageByName(string name)
		{
			LanguageEntity example = this.CreateLanguageExampleByName (name);

			return this.GetLanguageByExample (example);
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
