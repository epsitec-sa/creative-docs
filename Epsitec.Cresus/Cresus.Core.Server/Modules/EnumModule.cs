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


				IEnumerable<EnumKeyValues<TaxMode>> possibleItems = EnumKeyValues.FromEnum<TaxMode> ();

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

				var res = Response.AsJson (list);

				return res;
			};

		}
	}
}
