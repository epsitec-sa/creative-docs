//	Copyright © 2007-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>IEntityCollection</c> interface is used by the generic
	/// <see cref="EntityCollection&lt;T&gt;"/> class to give access to
	/// the copy-on-write mechanisms.
	/// </summary>
	public interface IEntityCollection : INotifyCollectionChangedProvider, ISuspendCollectionChanged
	{
		/// <summary>
		/// Resets the collection to the unchanged copy on write state.
		/// </summary>
		void ResetCopyOnWrite();

		/// <summary>
		/// Copies the collection to a writable instance if the collection is
		/// still in the unchanged copy on write state.
		/// </summary>
		void CopyOnWrite();

		/// <summary>
		/// Gets a value indicating whether this collection will create a copy
		/// before being modified.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the collection has the copy on write state; otherwise, <c>false</c>.
		/// </value>
		bool HasCopyOnWriteState
		{
			get;
		}

		/// <summary>
		/// Gets the type of the items stored in this collection.
		/// </summary>
		/// <returns>The type of the items.</returns>
		System.Type GetItemType();

		/// <summary>
		/// Temporarily disables all change notifications. Any changes which
		/// happen until <c>Dispose</c> is called on the returned object will
		/// not generate events; they are simply lost.
		/// </summary>
		/// <returns>An object you will have to <c>Dispose</c> in order to re-enable
		/// the notifications.</returns>
		System.IDisposable DisableNotifications();
	}
}