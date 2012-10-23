//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
		/// Automatically create a missing entity.
		/// </summary>
		AutoCreateNullEntity,

		/// <summary>
		/// Default the sub-view to <see cref="ViewControllerMode.Summary"/> rather
		/// than <see cref="ViewControllerMode.Edition"/>.
		/// </summary>
		DefaultToSummarySubView,
		DefaultToCreationOrSummarySubView,
		DefaultToCreationOrEditionSubView,

		HideAddButton,
		HideRemoveButton,

		SpecialController0,				//	Also update class BrickModeExtensions ...
		SpecialController1,				//	... if you add more special controllers here !
		SpecialController2,
		SpecialController3,
		SpecialController4,
		SpecialController5,
		SpecialController6,
		SpecialController7,
		SpecialController8,
		SpecialController9,
		SpecialController10,
		SpecialController11,
		SpecialController12,
		SpecialController13,
		SpecialController14,
		SpecialController15,
		SpecialController16,
		SpecialController17,
		SpecialController18,
		SpecialController19,

		FullHeightStretch,
		FullWidthPanel,
	}
}