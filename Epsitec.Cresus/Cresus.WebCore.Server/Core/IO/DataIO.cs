using Epsitec.Common.Support;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Controllers;

using System;


namespace Epsitec.Cresus.WebCore.Server.Core.IO
{


	/// <summary>
	/// This class provides methods to convert some data back and forth from the server to the
	/// javascript client.
	/// </summary>
	internal static class DataIO
	{


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
