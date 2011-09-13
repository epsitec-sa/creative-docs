//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Business
{
	[DesignerVisible]
	[System.Flags]
	public enum ArticleDocumentItemAttributes
	{
		None							= 0,
		
		NeverApplyDiscount				= 0x00000001,

		ArticleNotDiscountable			= 0x00000002,
		ArticlePriceIncludesTaxes		= 0x00000004,

		PartialQuantities				= 0x00000008,

		FixedUnitPrice1					= 0x00000100,
		FixedUnitPrice2					= 0x00000200,
		FixedLinePrice1					= 0x00000400,
		FixedLinePrice2					= 0x00000800,
		FixedTotalRevenue				= 0x00001000,

		Dirty							= 0x01000000,
	}
}
