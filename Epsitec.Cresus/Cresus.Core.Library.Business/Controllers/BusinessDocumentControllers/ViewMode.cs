//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

namespace Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers
{
	public enum ViewMode
	{
		Unknown,

		Compact,
		Default,
		Full,
		Debug,
	}
	public interface ILineProvider
	{
		LineInformations GetLineInformations(int index);
		
		CellContent GetCellContent(int index, ColumnType columnType);
		
		ViewMode CurrentViewMode
		{
			get;
		}
		
		EditMode CurrentEditMode
		{
			get;
		}

		int Count
		{
			get;
		}
	}
}
