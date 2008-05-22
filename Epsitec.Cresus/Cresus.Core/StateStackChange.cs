//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Cresus.Core
{
	/// <summary>
	/// The <c>StateStackChange</c> enumeration defines how the stack changed
	/// and is used by <see cref="StateStackChangedEventArgs"/>.
	/// </summary>
	public enum StateStackChange
	{
		/// <summary>
		/// No change.
		/// </summary>
		None,

		/// <summary>
		/// A new state was pushed on top of the stack.
		/// </summary>
		Push,
		
		/// <summary>
		/// A state was removed (popped) from the stack.
		/// </summary>
		Pop,
		
		/// <summary>
		/// A state which was already in the stack got promoted to the top
		/// of the stack.
		/// </summary>
		Promotion,

		/// <summary>
		/// A state in the navigation history got promoted to the top of
		/// the stack. Similar to <see cref="Promotion"/>.
		/// </summary>
		Navigation,

		/// <summary>
		/// A state was either hidden or shown.
		/// </summary>
		Visibility,

		/// <summary>
		/// One or more states have been added as the result of a load
		/// operation.
		/// </summary>
		Load,
	}
}
