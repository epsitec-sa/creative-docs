//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Core.Bricks
{
	/// <summary>
	/// The <c>BrickMode</c> enumeration defines several attributes, which may
	/// be used with the <see cref="Bridge"/> class and its <see cref="AttributeValue"/>
	/// properties.
	/// </summary>
	public enum BrickMode
	{
		/// <summary>
		/// Automatically group items together.
		/// </summary>
		AutoGroup,

		/// <summary>
		/// Default the subview to <see cref="ViewControllerMode.Summary"/> rather
		/// than <see cref="ViewControllerMode.Edition"/>.
		/// </summary>
		DefaultToSummarySubview,

		HideAddButton,
		HideRemoveButton,

		SpecialController0,
		SpecialController1,
		SpecialController2,
		SpecialController3,

		FullHeightStretch,
	}
}
