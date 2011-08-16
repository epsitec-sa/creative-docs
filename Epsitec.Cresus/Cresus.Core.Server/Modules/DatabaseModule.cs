using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support.Extensions;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Server.AdditionalResponses;
using Nancy;
using Epsitec.Cresus.Core.Business;
using Epsitec.Common.Support.EntityEngine;

namespace Epsitec.Cresus.Core.Server.Modules
{
	public class DatabasesModule : CoreModule
	{

		static DatabasesModule()
		{
			DatabasesModule.databases = new Dictionary<string, Database> ();

			DatabasesModule.databases["customers"] = new Database<CustomerEntity>
			{
				Title = "Clients",
				DatabaseName = "customers",
				CSSClass = "customer"
			};

			DatabasesModule.databases["articles"] = new Database<ArticleDefinitionEntity>
			{
				Title = "Articles",
				DatabaseName = "articles",
				CSSClass = "article"
			};

			DatabasesModule.databases["genders"] = new Database<PersonGenderEntity>
			{
				Title = "Genres",
				DatabaseName = "genders",
				CSSClass = "gender"
			};
		}

		public DatabasesModule()
			: base ("/database")
		{

			Get["/list"] = parameters =>
			{
				var list = new List<object> ();

				DatabasesModule.databases.ForEach (o => list.Add (o.Value));

				return Response.AsCoreSuccess (list);
			};

			Get["/{name}"] = parameters =>
			{
				var coreSession = GetCoreSession ();
				var context = coreSession.GetBusinessContext ();

				string name = parameters.name;

				var type = DatabasesModule.databases[name].GetDatabaseType ();
				var method = typeof (BusinessContext).GetMethod ("GetAllEntities");
				var m = method.MakeGenericMethod (type);
				var o = m.Invoke (context, new object[0]);

				var enumerable = o as IEnumerable<AbstractEntity>;

				var articles = from x in enumerable
							   select x;

				var list = new List<object> ();

				articles.ForEach (c => list.Add (new
				{
					name = c.GetCompactSummary ().ToSimpleText (),
					uniqueId = coreSession.GetBusinessContext ().DataContext.GetNormalizedEntityKey (c).Value.ToString ()
				}));

				var res = Response.AsJson (list);

				return res;

			};

		}

		private readonly static Dictionary<string, Database> databases;
	}

	abstract class Database
	{
		public abstract System.Type GetDatabaseType();
	}

	sealed class Database<T> : Database
		where T : AbstractEntity
	{
		public string Title;
		public string DatabaseName;
		public string CSSClass;

		public override System.Type GetDatabaseType()
		{
			return typeof (T);
		}
	}
}
