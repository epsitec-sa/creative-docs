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
	/// This module is used to provide the enumeration values for the javascript client.
	/// </summary>
	public class EnumModule : AbstractAuthenticatedModule
	{


		public EnumModule(CoreServer coreServer)
			: base (coreServer, "/enum")
		{
			// Gets the id/text pairs for the values of an enumeration.
			// URL arguments: 
			// - name:   The id of the enumeration type whose data to get, as used by the TypeCache
			//           class.
			Get["/get/{name}"] = p =>
				this.GetEnum (p);
		}


		private Response GetEnum(dynamic parameters)
		{
			string typeId = parameters.name;
			var type = this.CoreServer.Caches.TypeCache.GetItem ((string) typeId);

			return EnumModule.GetEnumResponse (type);
		}


		public static Response GetEnumResponse(Type type)
		{
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
