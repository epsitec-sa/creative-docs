//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets.Tiles
{
	/// <summary>
	/// The <c>TileColors</c> class defines the various colors used to paint the tiles,
	/// based on the active <see cref="IAdorner"/>.
	/// </summary>
	public static class TileColors
	{
		public static IEnumerable<Color> BorderColors
		{
			get
			{
				IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
				yield return adorner.ColorBorder;
			}
		}

		public static IEnumerable<Color> SurfaceDefaultColors
		{
			get
			{
				yield return Color.FromHexa ("f4f9ff");  // bleuté
			}
		}
		
		public static IEnumerable<Color> SurfaceSummaryColors
		{
			get
			{
				// TODO: Adapter aux autres adorners
				yield return Color.FromHexa ("ffffff");  // blanc
			}
		}

		public static IEnumerable<Color> SurfaceEditingColors
		{
			get
			{
				// TODO: Adapter aux autres adorners
				yield return Color.FromHexa ("daebff");  // bleu très clair
				yield return Color.FromHexa ("ffffff");  // blanc
			}
		}

		public static IEnumerable<Color> SurfaceSelectedGroupingColors
		{
			get
			{
				// TODO: Adapter aux autres adorners
				yield return Color.FromHexa ("ffba49");  // orange
				yield return Color.FromHexa ("fcd123");  // jaune-orange clair
			}
		}

		public static IEnumerable<Color> SurfaceSelectedContainerColors
		{
			get
			{
				// TODO: Adapter aux autres adorners
				yield return Color.FromHexa ("ffd672");  // orange clair
				yield return Color.FromHexa ("fcd123");  // jaune-orange clair
			}
		}

		public static IEnumerable<Color> SurfaceHilitedColors
		{
			get
			{
				// TODO: Adapter aux autres adorners
				yield return Color.FromHexa ("eef6ff");  // bleu pâle
				yield return Color.FromHexa ("ffffff");  // blanc
			}
		}

		public static IEnumerable<Color> SurfaceHilitedSelectedColors
		{
			get
			{
				// TODO: Adapter aux autres adorners
				yield return Color.FromHexa ("ffd673");  // orange clair
				yield return Color.FromHexa ("ffffff");  // blanc
			}
		}

		public static IEnumerable<Color> ThicknessHilitedColors
		{
			get
			{
				// TODO: Adapter aux autres adorners
				yield return Color.FromHexa ("b3d7ff");  // bleu
			}
		}
	}
}
