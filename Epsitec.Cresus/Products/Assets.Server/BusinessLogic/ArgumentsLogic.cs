//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.DataProperties;
using Epsitec.Cresus.Assets.Server.Expression;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class ArgumentsLogic
	{
		public static IEnumerable<Guid> GetReferencedMethods(DataAccessor accessor, Guid argumentGuid)
		{
			//	Vérifie quels sont les méthodes d'amortissement qui référencent un
			//	argument donné. Retourne les Guid des méthodes concernées, ou aucun
			//	si l'argument n'est pas référencé.
			var hash = new HashSet<Guid> ();

			foreach (var method in accessor.Mandat.GetData (BaseType.Methods))
			{
				foreach (var e in method.Events)
				{
					foreach (var field in DataAccessor.ArgumentFields)
					{
						var p = e.GetProperty (field) as DataGuidProperty;
						if (p != null && p.Value == argumentGuid)
						{
							hash.Add (method.Guid);
						}
					}
				}
			}

			return hash;
		}


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

		public static ArgumentType GetArgumentType(DataAccessor accessor, ObjectField field)
		{
			var argument = ArgumentsLogic.GetArgument (accessor, field);
			return ArgumentsLogic.GetArgumentType (argument);
		}

		public static ArgumentType GetArgumentType(DataObject argument)
		{
			if (argument != null)
			{
				var i = ObjectProperties.GetObjectPropertyInt (argument, null, ObjectField.ArgumentType);

				if (i.HasValue)
				{
					return (ArgumentType) i.Value;
				}
			}

			return ArgumentType.Unknown;
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
		public static string GetArgumentsDotNetCode(DataAccessor accessor, DataObject methodObj,
			DataObject asset = null, Timestamp timestamp = new Timestamp ())
		{
			//	Retourne le code C# des arguments pour calculer l'amortissement d'un objet
			//	d'immobilisation à un instant donné.
			return ArgumentsLogic.GetArgumentsDotNetCode (accessor,
				ArgumentsLogic.GetArgumentGuids (methodObj), asset, timestamp);
		}

		private static IEnumerable<Guid> GetArgumentGuids(DataObject methodObj)
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

		public static string GetArgumentsDotNetCode(DataAccessor accessor, IEnumerable<Guid> argumentGuids,
			DataObject asset = null, Timestamp timestamp = new Timestamp ())
		{
			//	Retourne le code C# des arguments. S'il n'y a pas d'objet d'immobilisation,
			//	on utilise les valeurs par défaut définies dans les arguments.
			var list = new List<string> ();

			foreach (var argumentGuid in argumentGuids)
			{
				list.Add (ArgumentsLogic.GetDotNetCode (accessor, argumentGuid, asset, timestamp));
			}

			return string.Join ("<br/>", list);
		}

		private static string GetDotNetCode(DataAccessor accessor, Guid argumentGuid, DataObject asset, Timestamp timestamp)
		{
			var argumentObj = accessor.GetObject (BaseType.Arguments, argumentGuid);
			System.Diagnostics.Debug.Assert (argumentObj != null);

			return ArgumentsLogic.GetDotNetCode (argumentObj, asset, timestamp);
		}

		private static string GetDotNetCode(DataObject argumentObj, DataObject asset, Timestamp timestamp)
		{
			//	Retourne une ligne de code C# permettant de déclarer la variable correspondant à l'argument.
			//	asset/timestamp déterminent où piocher la valeur de l'argument.
			if (argumentObj == null)
			{
				return null;
			}

			var type     = (ArgumentType) ObjectProperties.GetObjectPropertyInt (argumentObj, null, ObjectField.ArgumentType);
			var field    = (ObjectField)  ObjectProperties.GetObjectPropertyInt (argumentObj, null, ObjectField.ArgumentField);
			var nullable = ObjectProperties.GetObjectPropertyInt    (argumentObj, null, ObjectField.ArgumentNullable) == 1;
			var variable = ObjectProperties.GetObjectPropertyString (argumentObj, null, ObjectField.ArgumentVariable);
			var def      = ObjectProperties.GetObjectPropertyString (argumentObj, null, ObjectField.ArgumentDefault);
			var desc     = ObjectProperties.GetObjectPropertyString (argumentObj, null, ObjectField.Description);

			switch (type)
			{
				case ArgumentType.Decimal:
				case ArgumentType.Amount:
				case ArgumentType.Rate:
				case ArgumentType.Years:
					var d = ObjectProperties.GetObjectPropertyDecimal (asset, timestamp, field);
					if (d.HasValue)
					{
						def = TypeConverters.DecimalToString (d);
					}
					break;

				case ArgumentType.Int:
					var i = ObjectProperties.GetObjectPropertyInt (asset, timestamp, field);
					if (i.HasValue)
					{
						def = TypeConverters.IntToString (i);
					}
					break;

				case ArgumentType.Bool:
					var b = ObjectProperties.GetObjectPropertyInt (asset, timestamp, field);
					if (b.HasValue)
					{
						def = b == 1 ? "true" : "false";
					}
					break;

				case ArgumentType.Date:
					var date = ObjectProperties.GetObjectPropertyDate (asset, timestamp, field);
					if (date.HasValue)
					{
						def = TypeConverters.DateToString (date);
					}
					break;

				case ArgumentType.String:
					var s = ObjectProperties.GetObjectPropertyString (asset, timestamp, field);
					if (!string.IsNullOrEmpty (s))
					{
						def = s;
					}
					break;

				default:
					throw new System.InvalidOperationException (string.Format ("Invalid ArgumentType {0}", type));
			}

			return AmortizationExpression.GetArgumentCode (type, nullable, variable, def, desc);
		}
		#endregion


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
	}
}
