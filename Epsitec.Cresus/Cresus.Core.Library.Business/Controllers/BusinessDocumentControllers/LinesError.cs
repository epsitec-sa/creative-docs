//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

namespace Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers
{
	public enum LinesError
	{
		OK,

		EmptySelection,
		InvalidSelection,
		AlreadyOnTop,
		AlreadyOnBottom,
		InvalidQuantity,
		MinDeep,
		MaxDeep,
		AlreadySplited,
		AlreadyCombined,
		Overflow,
		Fixed,
	}
}
