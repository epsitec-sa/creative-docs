//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.DataProperties;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class MethodsLogic
	{
		public static IEnumerable<Guid> GetReferencedCategories(DataAccessor accessor, Guid methodGuid)
		{
			//	Vérifie quels sont les catégories d'amortissement qui référencent une
			//	méthode donnée. Retourne les Guid des catégories concernées, ou aucun
			//	si la méthode n'est pas référencée.
			var hash = new HashSet<Guid> ();

			foreach (var category in accessor.Mandat.GetData (BaseType.Categories))
			{
				var e = category.Events.FirstOrDefault ();
				if (e != null)
				{
					var p = e.GetProperty (ObjectField.MethodGuid) as DataGuidProperty;
					if (p != null && p.Value == methodGuid)
					{
						hash.Add (category.Guid);
					}
				}
			}

			return hash;
		}


		public static string GetSummary(DataAccessor accessor, Guid guid)
		{
			//	Retourne le nom court d'une méthode d'amortissement.
			var obj = accessor.GetObject (BaseType.Methods, guid);
			return MethodsLogic.GetSummary (obj);
		}

		public static string GetSummary(DataObject obj)
		{
			//	Retourne le nom court d'une méthode d'amortissement.
			if (obj == null)
			{
				return null;
			}

			return ObjectProperties.GetObjectPropertyString (obj, null, ObjectField.Name);
		}


		public static string GetExpressionSummary(DataAccessor accessor, Guid guid)
		{
			//	Retourne le résumé du code C# d'une méthode d'amortissement, sous la forme
			//	de la liste des arguments.
			var obj = accessor.GetObject (BaseType.Methods, guid);
			if (obj == null)
			{
				return null;
			}

			var list = new List<string> ();

			for (var argumentField = ObjectField.ArgumentFirst; argumentField <= ObjectField.ArgumentLast; argumentField++)
			{
				var arguemntGuid = ObjectProperties.GetObjectPropertyGuid (obj, null, argumentField);
				if (arguemntGuid != null)
				{
					var argument = accessor.GetObject (BaseType.Arguments, arguemntGuid);
					if (argument != null)
					{
						var name = ObjectProperties.GetObjectPropertyString (argument, null, ObjectField.Name);
						if (!string.IsNullOrEmpty (name))
						{
							list.Add (name);
						}
					}
				}
			}

			if (list.Any ())
			{
				return string.Join (", ", list);
			}
			else
			{
				return Res.Strings.MethodsLogic.NoArgument.ToString ();
			}
		}

		public static string GetExpression(DataAccessor accessor, Guid guid)
		{
			//	Retourne le code C# d'une méthode d'amortissement.
			var obj = accessor.GetObject (BaseType.Methods, guid);
			if (obj == null)
			{
				return null;
			}

			return ObjectProperties.GetObjectPropertyString (obj, null, ObjectField.Expression);
		}


		public static bool IsHidden(DataAccessor accessor, Guid methodGuid, ObjectField field)
		{
			var method = accessor.GetObject (BaseType.Methods, methodGuid);
			return MethodsLogic.IsHidden (method, field);
		}

		public static bool IsHidden(DataObject method, ObjectField field)
		{
			if (method != null)
			{
				return ObjectProperties.GetObjectPropertyGuid (method, null, field).IsEmpty;
			}

			return true;
		}
	}
}
