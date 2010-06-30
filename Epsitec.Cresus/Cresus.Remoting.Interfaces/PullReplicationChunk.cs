//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// The <c>PullReplicationChunk</c> structure describes which rows of a given
	/// table must be replicated by <see cref="IReplicationService.PullReplication"/>.
	/// </summary>
	[System.Serializable]
	public struct PullReplicationChunk
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PullReplicationChunk"/> struct.
		/// </summary>
		/// <param name="tableId">The table id.</param>
		/// <param name="rowIds">The row ids.</param>
		public PullReplicationChunk(long tableId, IEnumerable<long> rowIds)
		{
			this.tableId = tableId;
			this.rowIds  = rowIds.ToArray ();
		}


		/// <summary>
		/// Gets the table id.
		/// </summary>
		/// <value>The table id.</value>
		public long								TableId
		{
			get
			{
				return this.tableId;
			}
		}

		/// <summary>
		/// Gets the row ids.
		/// </summary>
		/// <value>The row ids.</value>
		public long[]							RowIds
		{
			get
			{
				long[] copy = new long[this.rowIds.Length];
				this.rowIds.CopyTo (copy, 0);
				return copy;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is empty.
		/// </summary>
		/// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
		public bool								IsEmpty
		{
			get
			{
				return this.rowIds == null;
			}
		}


		public static readonly PullReplicationChunk	Empty = new PullReplicationChunk ();
		
		readonly long							tableId;
		readonly long[]							rowIds;
	}
}
