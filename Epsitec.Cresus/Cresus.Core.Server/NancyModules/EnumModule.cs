//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Jonas Schmid, Maintainer: -


using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Library;

using Epsitec.Cresus.Core.Server.CoreServer;

using System.Collections.Generic;

using Nancy;


namespace Epsitec.Cresus.Core.Server.NancyModules
{


	/// <summary>
	/// Used to provide Enum values to the ExtJS store
	/// </summary>
	public class EnumModule : AbstractCoreSessionModule
	{


		public EnumModule(ServerContext serverContext)
			: base (serverContext, "/enum")
		{
			Post["/"] = parameters =>
			{
				string typeName = (string) Request.Form.name;
				var type = System.Type.GetType (typeName);
				var fetcherType = typeof (Fetcher<>).MakeGenericType (type);
				var fetcherInst = System.Activator.CreateInstance (fetcherType) as Fetcher;
				var list = fetcherInst.GetValues ();

				var res = Response.AsJson (list);

				return res;
			};
		}


		private abstract class Fetcher
		{
			public abstract List<object> GetValues();
		}


		private sealed class Fetcher<T> : Fetcher
			where T : struct
		{


			public override List<object> GetValues()
			{
				IEnumerable<EnumKeyValues<T>> possibleItems = EnumKeyValues.FromEnum<T> ();

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

				return list;
			}


		}


	}


}
