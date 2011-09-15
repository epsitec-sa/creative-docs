//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

namespace Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers
{
	public enum LineError
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
		AlreadyFlat,
		Overflow,
		Fixed,
		OnlyQuantity,
	}
}
