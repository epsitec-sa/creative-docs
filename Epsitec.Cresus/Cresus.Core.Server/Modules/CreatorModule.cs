using System.Collections.Generic;
using Epsitec.Cresus.Core.Server.AdditionalResponses;
using Nancy;
using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Common.Support.Extensions;
using System.Linq;

namespace Epsitec.Cresus.Core.Server.Modules
{
	public class CreatorModule : CoreModule
	{
		public CreatorModule()
		{
			Post["/delete"] = parameters =>
			{
				var coreSession = GetCoreSession ();
				var context = coreSession.GetBusinessContext ();

				string parentEntity = Request.Form.parentEntity;
				var parentKey = EntityKey.Parse (parentEntity);
				AbstractEntity entity = context.DataContext.ResolveEntity (parentKey);

				string deleteEntity = Request.Form.deleteEntity;
				var deleteKey = EntityKey.Parse (deleteEntity);

				var customer = entity as CustomerEntity;
				var contacts = customer.Relation.Person.Contacts;
				//var collection = customer.GetFieldCollection<AbstractEntity> ("[FVA2]");

				var toDelete = contacts.Where (c => context.DataContext.GetNormalizedEntityKey (c).Equals (deleteKey));

				if (toDelete.Any ())
				{
					contacts.Remove (toDelete.First ());
				}

				// TODO activate when not in debug
				//context.SaveChanges ();

				return Response.AsSuccessExtJsForm ();
			};


			Post["/create"] = parameters =>
			{
				return Response.AsSuccessExtJsForm ();
			};



		}
	}
}
