//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.DataAccessors
{
	/// <summary>
	/// The <c>ICollectionModificationCapabilities</c> interface is used to provide information
	/// about a collection's modification capabilities.
	/// </summary>
	public interface ICollectionModificationCapabilities
	{
		/// <summary>
		/// Determines whether this collection can insert an item at the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>
		///   <c>true</c> if this collection can insert the item at the specified index; otherwise, <c>false</c>.
		/// </returns>
		bool CanInsert(int index);

		/// <summary>
		/// Determines whether this collection can remove an item at the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>
		///   <c>true</c> if this collection can remove an item at the specified index; otherwise, <c>false</c>.
		/// </returns>
		bool CanRemove(int index);

		/// <summary>
		/// Gets a value indicating whether this collection can be reordered.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this collection can be reordered; otherwise, <c>false</c>.
		/// </value>
		bool CanBeReordered
		{
			get;
		}
	}
}
