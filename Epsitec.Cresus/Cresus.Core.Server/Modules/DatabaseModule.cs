using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support.Extensions;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Server.AdditionalResponses;
using Nancy;
using Epsitec.Cresus.Core.Business;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.DataLayer.Context;

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
				CSSClass = IconsBuilder.GetCSSClassName ("Base.Customer", IconSize.ThirtyTwo)
			};

			DatabasesModule.databases["articles"] = new Database<ArticleDefinitionEntity>
			{
				Title = "Articles",
				DatabaseName = "articles",
				CSSClass = IconsBuilder.GetCSSClassName ("Base.ArticleDefinition", IconSize.ThirtyTwo)
			};

			DatabasesModule.databases["genders"] = new Database<PersonGenderEntity>
			{
				Title = "Genres",
				DatabaseName = "genders",
				CSSClass = IconsBuilder.GetCSSClassName ("Base.PersonGender", IconSize.ThirtyTwo)
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

				var start = (int) Request.Query.start;
				var limit = (int) Request.Query.limit;

				var list = from c in enumerable
						   let summary = c.GetCompactSummary ().ToSimpleText ()
						   // orderby summary // TODO Awefully slow !
						   select new
						   {
							   name = summary,
							   uniqueId = coreSession.GetBusinessContext ().DataContext.GetNormalizedEntityKey (c).Value.ToString ()
						   };
				var subset = list.Skip (start).Take (limit);

				var dic = new Dictionary<string, object> ();
				dic["total"] = enumerable.Count (); // For ExtJS
				dic["entities"] = subset;

				var res = Response.AsJson (dic);

				return res;
			};

			Post["/delete"] = parameters =>
			{
				var coreSession = this.GetCoreSession ();
				var context = coreSession.GetBusinessContext ();

				string paramEntityKey = (string) Request.Form.entityId;

				var entityKey = EntityKey.Parse (paramEntityKey);
				AbstractEntity entity = context.DataContext.ResolveEntity (entityKey);

				var ok = context.DeleteEntity (entity);

				context.SaveChanges ();

				return Response.AsCoreBoolean (ok);
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
