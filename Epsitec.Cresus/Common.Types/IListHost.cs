//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The IListHost interface is implemented by the class which hosts a
	/// HostedList (usually through an Items property), so that the HostedList
	/// can notify the host of its changes.
	/// </summary>
	/// <typeparam name="T">Data type used by HostedList</typeparam>
	public interface IListHost<T>
	{
		Collections.HostedList<T> Items
		{
			get;
		}
		
		void NotifyListInsertion(T item);
		void NotifyListRemoval(T item);
	}
}
