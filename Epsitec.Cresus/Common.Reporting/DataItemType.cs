//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Reporting
{
	/// <summary>
	/// The <c>DataItemType</c> enumeration defines the different item
	/// types which can be found while navigating through a data graph
	/// in the reporting system.
	/// </summary>
	public enum DataItemType
	{
		/// <summary>
		/// No associated type.
		/// </summary>
		None,

		/// <summary>
		/// The data item represents a table; this is a collection of
		/// items.
		/// </summary>
		Table,

		/// <summary>
		/// The data item represents a group of items.
		/// </summary>
		Group,

		/// <summary>
		/// The data item represents a vector of values (usually a
		/// row in a table).
		/// </summary>
		Vector,

		/// <summary>
		/// The data item represents a leaf value.
		/// </summary>
		Value,

		/// <summary>
		/// Not a data item; this is a break request.
		/// TODO: document better
		/// </summary>
		Break,

		/// <summary>
		/// Not a data item; this is a rewind request.
		/// TODO: document better
		/// </summary>
		Rewind,
		
		/// <summary>
		/// Not a data item; this is a restart request.
		/// TODO: document better
		/// </summary>
		Restart
	}
}
