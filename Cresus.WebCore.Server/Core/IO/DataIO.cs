//	Copyright © 2012-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Factories;

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

			if (!System.Enum.IsDefined (typeof (ViewControllerMode), viewMode))
			{
				throw new System.ArgumentException ();
			}

			return viewMode;
		}

		public static string ViewModeToString(ViewControllerMode mode)
		{
			return InvariantConverter.ToString (mode);
		}

		public static ViewId ParseViewId(string value)
		{
			if (value == "null")
            {
                return ViewId.Empty;
            }

            var pos = value.IndexOf (' ');

            if (pos < 0)
            {
                if (int.TryParse (value, out var num))
                {
                    return new ViewId (num, null);
                }
            }
            else
            {
                if (int.TryParse (value.Substring (0, pos), out var num))
                {
                    return new ViewId (num, value.Substring (pos+1));
                }
            }

            return ViewId.Empty;
		}

		public static string ViewIdToString(ViewId id)
		{
			return id.ToString ();
		}
	}
}
