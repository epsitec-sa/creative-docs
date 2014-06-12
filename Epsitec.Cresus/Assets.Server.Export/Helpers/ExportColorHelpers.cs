//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.Server.Export;

namespace Epsitec.Cresus.Assets.Export.Helpers
{
	public static class ExportColorHelpers
	{
		public static Color GetColor(this ExportColor exportColor)
		{
			switch (exportColor)
			{
				case ExportColor.White:
					return Color.FromBrightness (1.0);

				case ExportColor.LightGrey:
					return Color.FromBrightness (0.95);

				case ExportColor.Grey:
					return Color.FromBrightness (0.9);

				case ExportColor.DarkGrey:
					return Color.FromBrightness (0.8);

				case ExportColor.Black:
					return Color.FromBrightness (0.0);

				case ExportColor.LightRed:
					return Color.FromHexa ("ffd4d4");

				case ExportColor.LightGreen:
					return Color.FromHexa ("d7ffd4");

				case ExportColor.LightBlue:
					return Color.FromHexa ("d4e8ff");

				case ExportColor.LightYellow:
					return Color.FromHexa ("fffdd4");

				case ExportColor.LightPurple:
					return Color.FromHexa ("dcc7ff");

				default:
					return Color.Empty;
			}
		}
	}
}
