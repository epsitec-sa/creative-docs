using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.NancyHosting;

using Nancy;

using System;

using System.Collections.Generic;

using System.Linq;

using System.Reflection;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{


	/// <summary>
	/// Used to provide Enum values to the ExtJS store
	/// </summary>
	public class EnumModule : AbstractAuthenticatedModule
	{


		public EnumModule(CoreServer coreServer)
			: base (coreServer, "/enum")
		{
			Get["/get/{name}"] = p => this.GetEnum (p);
		}


		private Response GetEnum(dynamic parameters)
		{
			string typeName = parameters.name;
			var type = Tools.ParseType (typeName);

			var values = EnumModule.GetValues (type).ToList ();

			var content = new Dictionary<string, object> ()
			{
				{ "values", values },
			};

			return CoreResponse.Success (content);
		}


		private static IEnumerable<object> GetValues(Type type)
		{
			var isNullable = type.IsNullable ();

			var enumType = isNullable
				? type.GetNullableTypeUnderlyingType ()
				: type;

			var method = typeof (EnumModule).GetMethod ("GetValuesImplementation", BindingFlags.NonPublic | BindingFlags.Static);
			var genericMethod = method.MakeGenericMethod (enumType);

			return (IEnumerable<object>) genericMethod.Invoke (null, new object[] { isNullable });
		}


		private static IEnumerable<object> GetValuesImplementation<T>(bool isNullable)
			where T : struct
		{
			if (isNullable)
			{
				yield return new
				{
					id = Constants.KeyForNullValue,
					text = Res.Strings.EmptyValue.ToSimpleText (),
				};
			}

			foreach (var enumKeyValues in EnumKeyValues.FromEnum<T> ())
			{
				var id = InvariantConverter.ToString (enumKeyValues.Key);
				var values = enumKeyValues.Values;

				if (values.Any ())
				{
					var text = values[0].ToString ();

					yield return new { id = id, text = text };
				}
			}
		}


	}


}
