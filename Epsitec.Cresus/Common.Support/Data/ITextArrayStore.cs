//	Copyright � 2004-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support.Data
{
	/// <summary>
	/// L'interface ITextArrayStore permet d'acc�der � des textes stock�s dans
	/// une table bidimensionnelle, en lecture et en �criture.
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
