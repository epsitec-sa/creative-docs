using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Data
{


	public class UriSchemeRepository : Repository
	{


		public UriSchemeRepository(DataContext dataContext) : base (dataContext)
		{
		}


		public IEnumerable<UriSchemeEntity> GetUriSchemesByExample(UriSchemeEntity example)
		{
			return this.GetEntitiesByExample<UriSchemeEntity> (example);
		}


		public UriSchemeEntity GetUriSchemeByExample(UriSchemeEntity example)
		{
			return this.GetEntityByExample<UriSchemeEntity> (example);
		}


		public IEnumerable<UriSchemeEntity> GetAllUriSchemes()
		{
			UriSchemeEntity example = this.CreateUriSchemeExample ();

			return this.GetUriSchemesByExample (example);
		}


		public UriSchemeEntity GetUriSchemeByCode(string code)
		{
			UriSchemeEntity example = this.CreateUriSchemeExampleByCode (code);

			return this.GetUriSchemeByExample (example);
		}


		public UriSchemeEntity GetUriSchemeByName(string name)
		{
			UriSchemeEntity example = this.CreateUriSchemeExampleByName (name);

			return this.GetUriSchemeByExample (example);
		}


		public UriSchemeEntity CreateUriSchemeExample()
		{
			return this.CreateExample<UriSchemeEntity> ();
		}


		private UriSchemeEntity CreateUriSchemeExampleByCode(string code)
		{
			UriSchemeEntity example = this.CreateUriSchemeExample ();
			example.Code = code;

			return example;
		}


		private UriSchemeEntity CreateUriSchemeExampleByName(string name)
		{
			UriSchemeEntity example = this.CreateUriSchemeExample ();
			example.Name = name;

			return example;
		}


	}


}
