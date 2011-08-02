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

			Get["/layout/person/{id}"] = parameters =>
			{
				var coreSession = MainModule.GetCoreSession ();
				var context = coreSession.GetBusinessContext ();

				var customer = (from x in context.GetAllEntities<CustomerEntity> ()
								where x.GetEntitySerialId () == parameters.id
								select x).FirstOrDefault ();

				if (customer == null)
				{
					return new NotFoundResponse ();
				}

				var s = PanelBuilder.BuildController (customer, Controllers.ViewControllerMode.Summary);

				return Response.AsJson (s);
			};

			Get["/layout/affair/{id}"] = parameters =>
			{
				var coreSession = MainModule.GetCoreSession ();
				var context = coreSession.GetBusinessContext ();

				var affair = (from x in context.GetAllEntities<AffairEntity> ()
							  where x.GetEntitySerialId () == parameters.id
							  select x).FirstOrDefault ();

				if (affair == null)
				{
					return new NotFoundResponse ();
				}

				var s = PanelBuilder.BuildController (affair, Controllers.ViewControllerMode.Summary);

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
