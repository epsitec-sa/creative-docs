using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Cresus.Core.Business;
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
				// Init session
				GetCoreSession ();

				return "logged in";


				//var res = Response.AsJson (obj);
				//res.Headers["Access-Control-Allow-Origin"] = "*";
			};

			Get["/list/persons"] = parameters =>
			{
				var coreSession = GetCoreSession ();
				var context = coreSession.GetBusinessContext ();

				var customers = from x in context.GetAllEntities<CustomerEntity> ()
								select x;

				var obj = new List<object> ();

				customers.ForEach (c => obj.Add (new
				{
					firstName = c.IdA,
					lastName = c.IdB,
					uniqueId = c.GetEntitySerialId ()
				}));

				var res = Response.AsJson (obj);

				return res;

			};

			Get["/layout/{name}/{id}"] = parameters =>
			{
			    var coreSession = MainModule.GetCoreSession ();
			    var context = coreSession.GetBusinessContext ();

				//var type = System.Type.GetType ("Epsitec.Cresus.Core.Entities.AffairEntity");
				//var e = new AffairEntity ();

				var typeName = string.Format("Epsitec.Cresus.Core.Entities.{0}", parameters.name);
				var e = System.Activator.CreateInstance ("Cresus.Core.Library.Business", typeName);
			    var type = e.Unwrap ().GetType ();
			    var method = typeof(BusinessContext).GetMethod ("GetAllEntities");
			    var m = method.MakeGenericMethod (type);
			    var o = m.Invoke (context, new object [] {});

				var i = o as IEnumerable<AbstractEntity>;

				if (i == null)
				{
					return new NotFoundResponse ();
				}

			    var entity = i.Where (x => x.GetEntitySerialId () == parameters.id).FirstOrDefault ();

			    if (entity == null)
			    {
			        return new NotFoundResponse ();
			    }

			    var s = PanelBuilder.BuildController (entity, Controllers.ViewControllerMode.Summary);

			    return Response.AsJson (s);
			};
		}

		internal static CoreSession GetCoreSession()
		{
			var sessionId = DebugSession.Session["CoreSession"] as string;
			var session = CoreServer.Instance.GetCoreSession (sessionId);

			if (session == null)
			{
				var server = CoreServer.Instance;
				session = server.CreateSession ();
				PanelBuilder.CoreSession = session;

				DebugSession.Session["CoreSession"] = session.Id;
			}

			return session;
		}
	}
}
