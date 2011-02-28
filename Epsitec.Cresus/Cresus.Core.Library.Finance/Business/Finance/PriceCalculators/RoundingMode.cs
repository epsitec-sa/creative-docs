//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{
	/// <summary>
	/// The <c>RoundingMode</c> enum defines the different rounding strategies that a
	/// <see cref="NumericDimension"/> can have to round values.
	/// </summary>
	[DesignerVisible]
	public enum RoundingMode
	{
		/// <summary>
		/// The values are not rounded.
		/// </summary>
		None,

		/// <summary>
		/// The values are rounded down to the nearest value defined which is lower than the given
		/// one.
		/// </summary>
		Down,

		/// <summary>
		/// The values are rounded up or down to the nearest value defined. If there is a tie, the
		/// value is rounded up.
		/// </summary>
		Nearest,
		
		/// <summary>
		/// The values are rounded up to the nearest value defined which is greater than the given
		/// one.
		/// </summary>
		Up,
	}
}
