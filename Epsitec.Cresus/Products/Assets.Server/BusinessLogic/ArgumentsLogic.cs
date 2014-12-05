//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Core.Helpers;
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

			var name     = ObjectProperties.GetObjectPropertyString (obj, null, ObjectField.Name);
			var variable = ObjectProperties.GetObjectPropertyString (obj, null, ObjectField.ArgumentVariable);

			return string.Format ("{0} ({1})", name, variable);
		}


		#region DotNet code generation
		public static string GetArgumentsDotNetCode(DataAccessor accessor, DataObject methodObj)
		{
			return ArgumentsLogic.GetArgumentsDotNetCode (accessor,
				ArgumentsLogic.GetArgumentGuids (accessor, methodObj));
		}

		private static IEnumerable<Guid> GetArgumentGuids(DataAccessor accessor, DataObject methodObj)
		{
			foreach (var field in DataAccessor.ArgumentFields)
			{
				var argumentGuid = ObjectProperties.GetObjectPropertyGuid (methodObj, null, field);

				if (argumentGuid.IsEmpty)
				{
					break;
				}

				yield return argumentGuid;
			}
		}

		public static string GetArgumentsDotNetCode(DataAccessor accessor, IEnumerable<Guid> argumentGuids)
		{
			var list = new List<string> ();

			foreach (var argumentGuid in argumentGuids)
			{
				list.Add (ArgumentsLogic.GetDotNetCode (accessor, argumentGuid));
			}

			return string.Join ("<br/>", list);
		}

		private static string GetDotNetCode(DataAccessor accessor, Guid argumentGuid)
		{
			var argumentObj = accessor.GetObject (BaseType.Arguments, argumentGuid);
			System.Diagnostics.Debug.Assert (argumentObj != null);
			return ArgumentsLogic.GetDotNetCode (argumentObj);
		}

		private static string GetDotNetCode(DataObject argumentObj)
		{
			//	Retourne une ligne de code C# permettant de déclarer la variable
			//	correspondant à l'argument. Par exemple:
			//	"decimal Rate = 0.1m; // Taux d'amortissement"
			//	"string Name = "coucou"; // Message"
			//	"System.DateTime Date = new System.DateTime (2014, 12, 31); // Début"
			if (argumentObj == null)
			{
				return null;
			}

			var builder = new System.Text.StringBuilder ();

			var type = (ArgumentType) ObjectProperties.GetObjectPropertyInt (argumentObj, null, ObjectField.ArgumentType);
			var nullable = ObjectProperties.GetObjectPropertyInt    (argumentObj, null, ObjectField.ArgumentNullable) == 1;
			var variable = ObjectProperties.GetObjectPropertyString (argumentObj, null, ObjectField.ArgumentVariable);
			var def      = ObjectProperties.GetObjectPropertyString (argumentObj, null, ObjectField.ArgumentDefault);
			var desc     = ObjectProperties.GetObjectPropertyString (argumentObj, null, ObjectField.Description);

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

				switch (type)
				{
					case ArgumentType.Decimal:
						builder.Append (def.Replace (",", "."));

						if (def.Last () != '!')
						{
							builder.Append ("m");
						}
						break;

					case ArgumentType.Date:
						var date = TypeConverters.ParseDate (def, System.DateTime.Now, System.DateTime.MinValue, System.DateTime.MaxValue).GetValueOrDefault ();
						builder.Append ("new System.DateTime (");
						builder.Append (date.Year);
						builder.Append (", ");
						builder.Append (date.Month);
						builder.Append (", ");
						builder.Append (date.Day);
						builder.Append (")");
						break;

					case ArgumentType.String:
						builder.Append ("\"");
						builder.Append (def.Replace ("\"", "\\\""));
						builder.Append ("\"");
						break;

					default:
						builder.Append (def);
						break;
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
		#endregion


		#region Sorted fields
		public static IEnumerable<ObjectField> GetSortedFields(DataAccessor accessor)
		{
			//	Retourne la liste des champs, triés par ordre alphabétique des noms
			//	complets des groupes.
			return ArgumentsLogic.GetUsedFields (accessor)
				.OrderBy (x => ArgumentsLogic.GetSortingValue (accessor, x));
		}

		private static string GetSortingValue(DataAccessor accessor, ObjectField field)
		{
			//	Retourne le numéro d'un groupe, en vue du tri.
			var guid = accessor.EditionAccessor.GetFieldGuid (field);
			return ArgumentsLogic.GetSummary (accessor, guid);
		}

		private static IEnumerable<ObjectField> GetUsedFields(DataAccessor accessor)
		{
			//	Retourne la liste des champs utilisés par l'objet en édition, non triée.
			foreach (var field in DataAccessor.ArgumentFields)
			{
				var guid = accessor.EditionAccessor.GetFieldGuid (field);
				if (!guid.IsEmpty)
				{
					yield return field;
				}
			}
		}
		#endregion
	}
}
