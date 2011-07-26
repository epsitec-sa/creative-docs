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

			Get["/login"] = parameters =>
			{
				GetSession (this);

				return "logged in";
			};

			Get["/persons.json"] = parameters =>
			{
				var coreSession = GetSession(this);

				var context = coreSession.GetBusinessContext ();
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

				obj.Add (new
				{
					LastUpdate = Session["last"] as string
				});

				Session["last"] = System.DateTime.Now.ToString ();

				var json = writer.Write (obj);

				//return json;

				var res = Response.AsJson (obj);
				res.Headers["Access-Control-Allow-Origin"] = "*";

				return res;

			};
		}

		private static CoreSession GetSession(MainModule m)
		{
			var sessionId = m.Session["CoreSession"] as string;
			var session = CoreServer.Instance.GetCoreSession (sessionId);

			if (session == null)
			{
				var server = CoreServer.Instance;
				session = server.CreateSession ();

				m.Session["CoreSession"] = session.Id;
			}

			return session;
		}
	}
}
