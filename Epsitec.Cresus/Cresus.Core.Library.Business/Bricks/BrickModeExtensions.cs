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
				case BrickMode.SpecialController4:
				case BrickMode.SpecialController5:
				case BrickMode.SpecialController6:
				case BrickMode.SpecialController7:
				case BrickMode.SpecialController8:
				case BrickMode.SpecialController9:
				case BrickMode.SpecialController10:
				case BrickMode.SpecialController11:
				case BrickMode.SpecialController12:
				case BrickMode.SpecialController13:
				case BrickMode.SpecialController14:
				case BrickMode.SpecialController15:
				case BrickMode.SpecialController16:
				case BrickMode.SpecialController17:
				case BrickMode.SpecialController18:
				case BrickMode.SpecialController19:
					return true;

				default:
					return false;
			}
		}
	}
}
