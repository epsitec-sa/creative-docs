//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using System.Collections.Generic;

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>IEntityCollection</c> interface is used by the generic
	/// <see cref="EntityCollection&lt;T&gt;"/> class to give access to
	/// the copy-on-write mechanisms.
	/// </summary>
	public interface IEntityCollection : INotifyCollectionChangedProvider
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

		System.Type GetItemType();
	}
}
