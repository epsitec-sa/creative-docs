using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Core.Controllers;

using Epsitec.Cresus.DataLayer.Context;

using System;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Core.IO
{


	internal static class DataIO
	{


		public static Type ParseType(string value)
		{
			var type = Type.GetType (value, throwOnError: false, ignoreCase: false);

			if (type == null)
			{
				throw new ArgumentException ();
			}

			return type;
		}


		public static string TypeToString(Type type)
		{
			return type.AssemblyQualifiedName;
		}


		public static Druid ParseDruid(string value)
		{
			return Druid.Parse (value);
		}


		public static string DruidToString(Druid value)
		{
			return value.ToCompactString ();
		}


		public static ViewControllerMode ParseViewMode(string value)
		{
			var viewMode = InvariantConverter.ToEnum<ViewControllerMode> (value);

			if (!Enum.IsDefined (typeof (ViewControllerMode), viewMode))
			{
				throw new ArgumentException ();
			}

			return viewMode;
		}


		public static string ViewModeToString(ViewControllerMode mode)
		{
			return InvariantConverter.ToString (mode);
		}


		public static int? ParseViewId(string value)
		{
			int viewId;

			if (value != "null" && int.TryParse (value, out viewId))
			{
				return viewId;
			}
			else
			{
				return null;
			}
		}


		public static string ViewIdToString(int? id)
		{
			if (id.HasValue)
			{
				return InvariantConverter.ToString (id.Value);
			}
			else
			{
				return "null";
			}
		}


	}


}
