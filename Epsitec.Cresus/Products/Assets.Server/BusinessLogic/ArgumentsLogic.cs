//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class ArgumentsLogic
	{
		public static string GetSummary(DataAccessor accessor, Guid guid)
		{
			//	Retourne le nom court d'une argument d'une méthode d'amortissement.
			var obj = accessor.GetObject (BaseType.Arguments, guid);
			return ArgumentsLogic.GetSummary (obj);
		}

		public static string GetSummary(DataObject obj)
		{
			//	Retourne le nom court d'une argument d'une méthode d'amortissement.
			if (obj == null)
			{
				return null;
			}

			return ObjectProperties.GetObjectPropertyString (obj, null, ObjectField.Name);
		}


		public static string GetDotNetCode(DataObject obj)
		{
			//	Retourne une ligne de code C# permettant de déclarer la variable
			//	correspondant à l'argument. Par exemple:
			//	"decimal Rate = 0.1m; // Taux d'amortissement"
			if (obj == null)
			{
				return null;
			}

			var type = (ArgumentType) ObjectProperties.GetObjectPropertyInt (obj, null, ObjectField.ArgumentType);
			var nullable = ObjectProperties.GetObjectPropertyInt (obj, null, ObjectField.ArgumentNullable) == 1;
			var variable = ObjectProperties.GetObjectPropertyString (obj, null, ObjectField.ArgumentVariable);
			var def = ObjectProperties.GetObjectPropertyString (obj, null, ObjectField.ArgumentDefault);
			var desc = ObjectProperties.GetObjectPropertyString (obj, null, ObjectField.Description);

			var builder = new System.Text.StringBuilder();

			builder.Append (EnumDictionaries.GetArgumentTypeName (type));

			if (nullable && type != ArgumentType.String)
			{
				builder.Append ("?");
			}

			builder.Append (" ");
			builder.Append (variable);

			if (!string.IsNullOrEmpty (def))
			{
				builder.Append (" = ");
				builder.Append (def);

				if (type == ArgumentType.Decimal && def.Last () != '!')
				{
					builder.Append ("m");
				}
			}

			builder.Append (";");

			if (!string.IsNullOrEmpty (desc))
			{
				builder.Append (" // ");
				builder.Append (desc);
			}

			return builder.ToString ();
		}
	}
}
