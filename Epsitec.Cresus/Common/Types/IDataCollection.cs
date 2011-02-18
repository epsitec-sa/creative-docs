//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// L'interface IDataCollection donne accès à une collection de IDataItem.
	/// </summary>
	public interface IDataCollection : IEnumerable<IDataItem>, ICollection<IDataItem>
	{
		IDataItem		this[int index]		{ get; }
		IDataItem		this[string name]	{ get; }
		int IndexOf(IDataItem item);
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
		
		
	}
}

