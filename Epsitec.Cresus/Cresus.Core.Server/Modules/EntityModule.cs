using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Common.Support.EntityEngine;
using System.Linq;

namespace Epsitec.Cresus.Core.Server.Modules
{
	public class EntityModule : CoreModule
	{
		public EntityModule()
			: base ("/entity")
		{
			Post["/{id}"] = parameters =>
			{
				var coreSession = GetCoreSession ();
				var context = coreSession.GetBusinessContext ();

				var entityKey = EntityKey.Parse (parameters.id);
				AbstractEntity entity = context.DataContext.ResolveEntity (entityKey);

				foreach (var key in Request.Form.GetDynamicMemberNames ())
				{
					var k = string.Format ("[{0}]", key);
					var v = Request.Form[key].ToString ().Trim ();
					try
					{
						entity.SetField (k, v);
					}
					catch (System.Exception)
					{
						// TODO Enlever à la fin des tests
					}
				}
				context.SaveChanges ();

				return "ok";
			};
		}
	}
}
