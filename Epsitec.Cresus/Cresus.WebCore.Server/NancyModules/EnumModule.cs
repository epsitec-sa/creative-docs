using Epsitec.Cresus.Core.Library;

using Epsitec.Cresus.WebCore.Server.CoreServer;

using Nancy;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{


	/// <summary>
	/// Used to provide Enum values to the ExtJS store
	/// </summary>
	public class EnumModule : AbstractCoreSessionModule
	{


		public EnumModule(ServerContext serverContext)
			: base (serverContext, "/enum")
		{
			Post["/"] = parameters => this.ExecuteWithCoreSession (coreSession =>
			{
				string typeName = Request.Form.name;
				var type = Type.GetType (typeName);
				var fetcherType = typeof (Fetcher<>).MakeGenericType (type);
				var fetcherInst = (Fetcher) Activator.CreateInstance (fetcherType);
				var list = fetcherInst.GetValues ().ToList ();

				return Response.AsJson (list);
			});
		}


		// NOTE Here we need this weird class & reflexion stuff because we need the generic type
		// parameter in order to invoke the FromEnum<T> method, and we don't have that from scratch.
		// Too bad, especially because the FromEnum<T> converts back the type parameter to a
		// System.Type instance. I should come back here to correct this when I'll have more time.


		private abstract class Fetcher
		{
			public abstract IEnumerable<object> GetValues();
		}


		private sealed class Fetcher<T> : Fetcher
			where T : struct
		{


			public override IEnumerable<object> GetValues()
			{
				foreach (var enumKeyValues in EnumKeyValues.FromEnum<T> ())
				{
					foreach (var value in enumKeyValues.Values)
					{
						string id = enumKeyValues.Key.ToString ();
						string name = value.ToString ();

						yield return new { id = id, name = name };
					}
				}
			}


		}


	}


}
