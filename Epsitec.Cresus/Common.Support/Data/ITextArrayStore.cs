//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support.Data
{
	/// <summary>
	/// L'interface ITextArrayStore permet d'accéder à des textes stockés dans
	/// une table bidimensionnelle, en lecture et en écriture.
	/// </summary>
	public interface ITextArrayStore
	{
		int GetRowCount();
		int GetColumnCount();
		
		string GetCellText(int row, int column);
		void SetCellText(int row, int column, string value);
		void InsertRows(int row, int num);
		void RemoveRows(int row, int num);
		void MoveRow(int row, int distance);
		
		bool CheckSetRow(int row);
		bool CheckInsertRows(int row, int num);
		bool CheckRemoveRows(int row, int num);
		bool CheckMoveRow(int row, int distance);
		bool CheckEnabledCell(int row, int column);
		
		event Support.EventHandler		StoreContentsChanged;
	}
}
