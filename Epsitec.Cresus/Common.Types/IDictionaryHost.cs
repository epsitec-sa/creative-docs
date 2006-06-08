//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The IDictionaryHost interface is implemented by the class which hosts a
	/// HostedDictionary (usually through an Items property), so that the HostedDictionary
	/// can notify the host of its changes.
	/// </summary>
	/// <typeparam name="K">Key type used by HostedDictionary</typeparam>
	/// <typeparam name="V">Value type used by HostedDictionary</typeparam>
	public interface IDictionaryHost<K, V>
	{
		Collections.HostedDictionary<K, V> Items
		{
			get;
		}

		void NotifyDictionaryInsertion(K key, V value);
		void NotifyDictionaryRemoval(K key, V value);
	}
}
