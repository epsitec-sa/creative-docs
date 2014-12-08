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
			//	Retourne le nom complet d'une argument d'une méthode d'amortissement.
			var obj = accessor.GetObject (BaseType.Arguments, guid);
			return ArgumentsLogic.GetSummary (obj);
		}

		public static string GetSummary(DataObject obj)
		{
			//	Retourne le nom complet d'une argument d'une méthode d'amortissement.
			if (obj == null)
			{
				return null;
			}

			var name     = ObjectProperties.GetObjectPropertyString (obj, null, ObjectField.Name);
			var variable = ObjectProperties.GetObjectPropertyString (obj, null, ObjectField.ArgumentVariable);

			return string.Format ("{0} ({1})", name, variable);
		}

		public static string GetShortName(DataObject obj)
		{
			//	Retourne le nom court d'une argument d'une méthode d'amortissement.
			if (obj == null)
			{
				return null;
			}

			return ObjectProperties.GetObjectPropertyString (obj, null, ObjectField.Name);
		}


		public static ObjectField GetObjectField(DataAccessor accessor, Guid guid)
		{
			var obj = accessor.GetObject (BaseType.Arguments, guid);

			if (obj == null)
			{
				return ObjectField.Unknown;
			}

			return (ObjectField) ObjectProperties.GetObjectPropertyInt (obj, null, ObjectField.ArgumentField).GetValueOrDefault ();
		}

		public static DataObject GetArgument(DataAccessor accessor, ObjectField field)
		{
			foreach (var obj in accessor.Mandat.GetData (BaseType.Arguments))
			{
				var f = (ObjectField) ObjectProperties.GetObjectPropertyInt (obj, null, ObjectField.ArgumentField).GetValueOrDefault ();

				if (f == field)
				{
					return obj;
				}
			}

			return null;
		}

		public static ObjectField GetUnusedField(DataAccessor accessor)
		{
			//	Retourne un champ ObjectField.ArgumentFirst+n pas encore utilisé.
			var field = ObjectField.ArgumentFirst;

			foreach (var obj in accessor.Mandat.GetData (BaseType.Arguments))
			{
				var f = ObjectProperties.GetObjectPropertyInt (obj, null, ObjectField.ArgumentField).GetValueOrDefault ();
				field = (ObjectField) System.Math.Max ((int) field, f+1);
			}

			return field;
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

				if (!argumentGuid.IsEmpty)
				{
					yield return argumentGuid;
				}
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

			builder.Append (EnumDictionaries.GetArgumentTypeDotNet (type));

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
					case ArgumentType.Amount:
					case ArgumentType.Rate:
						if (def.Last () == '%')
						{
							var n = TypeConverters.ParseRate (def);
							def = TypeConverters.DecimalToString (n);
						}
						else
						{
							var n = TypeConverters.ParseAmount (def);
							def = TypeConverters.DecimalToString (n);
						}
						builder.Append (def);

						if (def.Last () != 'm')
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
		public static IEnumerable<DataObject> GetSortedArguments(DataAccessor accessor)
		{
			var dico = new Dictionary<DataObject, string> ();

			foreach (var obj in accessor.Mandat.GetData (BaseType.Arguments))
			{
				var name = ObjectProperties.GetObjectPropertyString (obj, null, ObjectField.Name);

				dico.Add (obj, name);
			}

			return dico.OrderBy (x => x.Value).Select (x => x.Key);
		}

		public static IEnumerable<ObjectField> GetSortedFields(DataAccessor accessor)
		{
			//	Retourne la liste des champs, triés par ordre alphabétique des noms
			//	des arguments en édition.
			var dico = new Dictionary<ObjectField, string> ();

			foreach (var obj in accessor.Mandat.GetData (BaseType.Arguments))
			{
				var field = (ObjectField) ObjectProperties.GetObjectPropertyInt (obj, null, ObjectField.ArgumentField);
				var name = ObjectProperties.GetObjectPropertyString (obj, null, ObjectField.Name);

				dico.Add (field, name);
			}

			return dico.OrderBy (x => x.Value).Select (x => x.Key);
		}
		#endregion
	}
}
