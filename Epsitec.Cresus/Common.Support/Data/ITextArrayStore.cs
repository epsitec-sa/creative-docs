//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 17/02/2004

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
		
		event Epsitec.Common.Support.EventHandler StoreChanged;
	}
}
