using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support.Extensions;
using Epsitec.Cresus.Core.Business.Finance;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Server.AdditionalResponses;
using Nancy;

namespace Epsitec.Cresus.Core.Server.Modules
{
	public class ListModule : CoreModule
	{

		public ListModule()
			: base ("/list")
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

			Get["/{name}"] = parameters =>
			{

				//var typeName = string.Format ("Epsitec.Cresus.Core.Business.Finance.{0}", parameters.name);
				//System.Runtime.Remoting.ObjectHandle e = System.Activator.CreateInstance ("Cresus.Core.Library.Finance", typeName);

				//var type = e.Unwrap ().GetType ();
				//var method = typeof (EnumKeyValues).GetMethod ("FromEnum");
				//var m = method.MakeGenericMethod (type);
				//var o = m.Invoke (new
				//{
				//}, new object[] { });

				//var possibleItems = o as IEnumerable<EnumKeyValues<TaxMode>>;


				IEnumerable<EnumKeyValues<TaxMode>> possibleItems = EnumKeyValues.FromEnum<TaxMode> ();

				var list = new List<object> ();

				possibleItems.ForEach (c =>
				{
					c.Values.ForEach (v =>
					{
						list.Add (new
						{
							id = c.Key.ToString (),
							name = v.ToString ()
						});
					});
				});

				var res = Response.AsJson (list);

				return res;
			};
		}
	}
}
