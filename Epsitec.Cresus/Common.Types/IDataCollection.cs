//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 26/04/2004

namespace Epsitec.Common.Types
{
	/// <summary>
	/// L'interface IDataCollection donne acc�s � une collection de IDataItem.
	/// </summary>
	public interface IDataCollection : System.Collections.IEnumerable, System.Collections.ICollection
	{
		//	IEnumerable Members:
		//
		//	System.Collection.IEnumerator GetEnumerator();
		
		//	ICollection Members:
		//
		//	int		Count					{ get; }
		//	bool	IsSynchronized			{ get; }
		//	object	SyncRoot				{ get; }
		//
		//	void CopyTo(System.Array array, int index);
		
		IDataItem		this[int index]		{ get; }
		IDataItem		this[string name]	{ get; }
		
		int IndexOf(IDataItem item);
	}
}

