//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 17/02/2004

namespace Epsitec.Common.Support.Data
{
	/// <summary>
	/// L'interface IArrayStore permet d'acc�der � des donn�es stock�es dans
	/// une table bidimensionnelle, en lecture et en �criture.
	/// </summary>
	public interface IArrayStore
	{
		int GetRowCount();
		int GetColumnCount();
		
		object GetCellValue(int row, int column);
		void SetCellValue(int row, int column, object value);
		void InsertRows(int row, int num);
		void RemoveRows(int row, int num);
	}
}
