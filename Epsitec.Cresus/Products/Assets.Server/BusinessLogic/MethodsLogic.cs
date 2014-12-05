//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class MethodsLogic
	{
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
			//	Retourne le résumé du code C# d'une méthode d'amortissement.
			var exp = MethodsLogic.GetExpression (accessor, guid);

			if (!string.IsNullOrEmpty (exp))
			{
				var n = exp.Split (new string[] { "<br/>" }, System.StringSplitOptions.RemoveEmptyEntries).Length;
				return string.Format (Res.Strings.MethodsLogic.ExpressionSummary.ToString (), TypeConverters.IntToString (n));
			}

			return null;
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
	}
}
