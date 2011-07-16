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

		ArticlePricesFrozen				= 0x00000080,

		FixedUnitPrice					= 0x00000100,
		FixedLinePrice					= 0x00000200,
		FixedPriceIncludesTaxes			= 0x00000400,

		DirtyArticlePrices				= 0x00010000,
		DirtyArticleNotDiscountable		= 0x00020000,

	}
}
