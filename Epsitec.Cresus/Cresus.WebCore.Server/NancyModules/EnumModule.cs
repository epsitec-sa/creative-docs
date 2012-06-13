using Epsitec.Common.Types;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.WebCore.Server.CoreServer;
using Epsitec.Cresus.WebCore.Server.NancyHosting;

using Nancy;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;


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
			Post["/"] = p => this.ExecuteWithCoreSession (cs => this.GetEnum ());
		}


		private Response GetEnum()
		{
			string typeName = Request.Form.name;

			var list = EnumModule.GetValues (typeName).ToList ();

			return CoreResponse.AsJson (list);
		}


		private static IEnumerable<object> GetValues(string typeName)
		{
			var type = System.Type.GetType (typeName);

			var isNullable = type.IsNullable ();

			var enumType = isNullable
				? type.GetNullableTypeUnderlyingType ()
				: type;

			var method = typeof (EnumModule).GetMethod ("GetValuesImplementation", BindingFlags.NonPublic | BindingFlags.Static);
			var genericMethod = method.MakeGenericMethod (enumType);

			return (IEnumerable<object>) genericMethod.Invoke (null, new object[] { isNullable });
		}


		private static IEnumerable<object> GetValuesImplementation<T>(bool isNullable) where T : struct
		{
			if (isNullable)
			{
				yield return new
				{
					id = Constants.KeyForNullValue,
					name = Constants.TextForNullValue
				};
			}

			foreach (var enumKeyValues in EnumKeyValues.FromEnum<T> ())
			{
				// NOTE Here we need the double cast because the compiler won't let us cast from
				// T to int directly, so we cast T to object because this is allowed and then we
				// unbox object to the real type.

				var id = (int) (object) enumKeyValues.Key;
				var values = enumKeyValues.Values;

				if (values.Any ())
				{
					var name = values[0].ToString ();

					yield return new { id = id, name = name };
				}
			}
		}


	}


}
