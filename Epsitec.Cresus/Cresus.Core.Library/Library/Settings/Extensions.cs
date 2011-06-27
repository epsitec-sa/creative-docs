//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Core.Library.Settings
{
	public static class Extensions
	{
		/// <summary>
		/// Simplifies the specified visibility mode by selecting the most meaningful value.
		/// </summary>
		/// <param name="mode">The mode.</param>
		/// <returns>The simplified visibility mode.</returns>
		public static TileVisibilityMode Simplify(this TileVisibilityMode mode)
		{
			switch (mode)
			{
				case TileVisibilityMode.Visible:
					return TileVisibilityMode.Visible;

				case TileVisibilityMode.NeverVisible:
				case TileVisibilityMode.NeverVisible | TileVisibilityMode.Visible:
				case TileVisibilityMode.NeverVisible | TileVisibilityMode.Hidden:
				case TileVisibilityMode.NeverVisible | TileVisibilityMode.Visible | TileVisibilityMode.Hidden:
					return TileVisibilityMode.NeverVisible;

				case TileVisibilityMode.Hidden:
				case TileVisibilityMode.Hidden | TileVisibilityMode.Visible:
					return TileVisibilityMode.Hidden;

				default:
					return TileVisibilityMode.Undefined;
			}
		}

		/// <summary>
		/// Simplifies the specified edition mode by selecting the most meaningful value.
		/// </summary>
		/// <param name="mode">The mode.</param>
		/// <returns>The simplified edition mode.</returns>
		public static TileEditionMode Simplify(this TileEditionMode mode)
		{
			switch (mode)
			{
				case TileEditionMode.ReadWrite:
					return TileEditionMode.ReadWrite;

				case TileEditionMode.ReadOnly:
				case TileEditionMode.ReadOnly | TileEditionMode.ReadWrite:
					return TileEditionMode.ReadOnly;

				default:
					return TileEditionMode.Undefined;
			}
		}
	}
}
