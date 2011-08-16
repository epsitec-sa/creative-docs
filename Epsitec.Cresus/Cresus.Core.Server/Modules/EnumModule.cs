using System.Collections.Generic;
using Epsitec.Common.Support.Extensions;
using Epsitec.Cresus.Core.Business.Finance;
using Epsitec.Cresus.Core.Library;
using Nancy;

namespace Epsitec.Cresus.Core.Server.Modules
{
	public class EnumModule : CoreModule
	{
		public EnumModule()
			: base ("/enum")
		{


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

				string typeName = typeof (TaxMode).AssemblyQualifiedName;
				var type = System.Type.GetType (typeName);
				var fetcherType = typeof (Fetcher<>).MakeGenericType (type);
				var fetcherInst = System.Activator.CreateInstance (fetcherType) as Fetcher;
				var list = fetcherInst.GetValues ();

				var res = Response.AsJson (list);

				return res;
			};

		}
	}

	abstract class Fetcher
	{
		public abstract List<object> GetValues();
	}

	sealed class Fetcher<T> : Fetcher
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
