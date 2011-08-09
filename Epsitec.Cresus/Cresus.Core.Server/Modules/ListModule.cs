using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support.Extensions;
using Epsitec.Cresus.Core.Business.Finance;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Library;
using Nancy;

namespace Epsitec.Cresus.Core.Server.Modules
{
	public class ListModule : CoreModule
	{

		public ListModule()
			: base ("/list")
		{

			Get["/persons"] = parameters =>
			{
				var coreSession = GetCoreSession ();
				var context = coreSession.GetBusinessContext ();

				var customers = from x in context.GetAllEntities<CustomerEntity> ()
								select x;

				var obj = new List<object> ();

				customers.ForEach (c => obj.Add (new
				{
					name = c.GetCompactSummary ().ToSimpleText (),
					uniqueId = coreSession.GetBusinessContext ().DataContext.GetNormalizedEntityKey (c).Value.ToString ()
				}));

				var res = Response.AsJson (obj);

				return res;

			};

			Get["/articles"] = parameters =>
			{
				var coreSession = GetCoreSession ();
				var context = coreSession.GetBusinessContext ();

				var articles = from x in context.GetAllEntities<ArticleDefinitionEntity> ()
								select x;

				var obj = new List<object> ();

				articles.ForEach (c => obj.Add (new
				{
					name = c.GetCompactSummary ().ToSimpleText (),
					uniqueId = coreSession.GetBusinessContext ().DataContext.GetNormalizedEntityKey (c).Value.ToString ()
				}));

				var res = Response.AsJson (obj);

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

				var obj = new List<object> ();

				possibleItems.ForEach (c =>
				{
					c.Values.ForEach (v =>
					{
						obj.Add (new
						{
							id = c.Key.ToString (),
							name = v.ToString ()
						});
					});
				});

				var res = Response.AsJson (obj);

				return res;
			};
		}
	}
}
