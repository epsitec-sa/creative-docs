//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Core.Bricks
{
	/// <summary>
	/// The <c>BrickModeExtensions</c> class implements extension methods for the
	/// <see cref="BrickMode"/> enumeration.
	/// </summary>
	public static class BrickModeExtensions
	{
		public static bool IsSpecialController(this BrickMode mode)
		{
			switch (mode)
			{
				case BrickMode.SpecialController0:
				case BrickMode.SpecialController1:
				case BrickMode.SpecialController2:
				case BrickMode.SpecialController3:
					return true;

				default:
					return false;
			}
		}
	}
}
