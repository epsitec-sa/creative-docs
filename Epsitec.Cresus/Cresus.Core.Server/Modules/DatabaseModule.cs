using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support.Extensions;
using Epsitec.Cresus.Core.Entities;
using Nancy;

namespace Epsitec.Cresus.Core.Server.Modules
{
	public class DatabasesModule : CoreModule
	{

		public DatabasesModule()
			: base ("/database")
		{

			Get["/customers"] = parameters =>
			{
				var coreSession = GetCoreSession ();
				var context = coreSession.GetBusinessContext ();

				var customers = from x in context.GetAllEntities<CustomerEntity> ()
								select x;

				var list = new List<object> ();

				customers.ForEach (c => list.Add (new
				{
					name = c.GetCompactSummary ().ToSimpleText (),
					uniqueId = coreSession.GetBusinessContext ().DataContext.GetNormalizedEntityKey (c).Value.ToString ()
				}));

				var res = Response.AsJson (list);

				return res;

			};

			Get["/articles"] = parameters =>
			{
				var coreSession = GetCoreSession ();
				var context = coreSession.GetBusinessContext ();

				var articles = from x in context.GetAllEntities<ArticleDefinitionEntity> ()
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
	}
}
