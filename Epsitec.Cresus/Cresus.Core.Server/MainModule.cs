using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support.Extensions;
using Epsitec.Cresus.Core.Entities;
using Nancy;

namespace Epsitec.Cresus.Core.Server
{
	public class MainModule : NancyModule
	{
		public MainModule()
		{
			Get["/"] = parameters =>
			{
				return "Hello World";
			};

			Get["persons.json"] = parameters =>
			{
				var server = CoreServer.Instance;
				var session = server.CreateSession ();

				var context = session.GetBusinessContext ();
				var writer = new JsonFx.Json.JsonWriter ();

				var customers = from x in context.GetAllEntities<CustomerEntity> ()
								select x;

				// Stackoverflow
				//var json = writer.Write (customer);

				var obj = new List<object> ();

				customers.ForEach (c => obj.Add (new
				{
					firstName = c.IdA,
					lastName = c.IdB
				}));

				var json = writer.Write (obj);

				//return json;

				var res = Response.AsJson (obj);
				res.Headers["Access-Control-Allow-Origin"] = "*";

				return res;

			};
		}
	}
}
