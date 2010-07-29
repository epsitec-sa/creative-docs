using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Context;

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


		public IEnumerable<UriSchemeEntity> GetUriSchemesByRequest(Request request)
		{
			return this.GetEntitiesByRequest<UriSchemeEntity> (request);
		}


		public IEnumerable<UriSchemeEntity> GetUriSchemesByExample(UriSchemeEntity example, int index, int count)
		{
			return this.GetEntitiesByExample<UriSchemeEntity> (example, index, count);
		}


		public IEnumerable<UriSchemeEntity> GetUriSchemesByRequest(Request request, int index, int count)
		{
			return this.GetEntitiesByRequest<UriSchemeEntity> (request, index, count);
		}


		public IEnumerable<UriSchemeEntity> GetAllUriSchemes()
		{
			UriSchemeEntity example = this.CreateUriSchemeExample ();

			return this.GetUriSchemesByExample (example);
		}


		public IEnumerable<UriSchemeEntity> GetAllUriSchemes(int index, int count)
		{
			UriSchemeEntity example = this.CreateUriSchemeExample ();

			return this.GetUriSchemesByExample (example, index, count);
		}


		public IEnumerable<UriSchemeEntity> GetUriSchemesByCode(string code)
		{
			UriSchemeEntity example = this.CreateUriSchemeExampleByCode (code);

			return this.GetUriSchemesByExample (example);
		}


		public IEnumerable<UriSchemeEntity> GetUriSchemesByCode(string code, int index, int count)
		{
			UriSchemeEntity example = this.CreateUriSchemeExampleByCode (code);

			return this.GetUriSchemesByExample (example, index, count);
		}


		public IEnumerable<UriSchemeEntity> GetUriSchemesByName(string name)
		{
			UriSchemeEntity example = this.CreateUriSchemeExampleByName (name);

			return this.GetUriSchemesByExample (example);
		}


		public IEnumerable<UriSchemeEntity> GetUriSchemesByName(string name, int index, int count)
		{
			UriSchemeEntity example = this.CreateUriSchemeExampleByName (name);

			return this.GetUriSchemesByExample (example, index, count);
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
