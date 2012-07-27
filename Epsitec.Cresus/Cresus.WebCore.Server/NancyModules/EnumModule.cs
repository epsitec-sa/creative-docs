using Epsitec.Common.Types;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.WebCore.Server.Core;
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
	public class EnumModule : AbstractBusinessContextModule
	{


		public EnumModule(CoreServer coreServer)
			: base (coreServer, "/enum")
		{
			Get["/get/{name}"] = p => this.GetEnum (p);
		}


		private Response GetEnum(dynamic parameters)
		{
			string typeName = parameters.name;

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
				var id = InvariantConverter.ToString (enumKeyValues.Key);
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
