//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Replication
{
	/// <summary>
	/// The <c>PullArguments</c> class stores a collection of tables and rows
	/// which should be replicated.
	/// </summary>
	public sealed class PullArguments
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PullArguments"/> class.
		/// </summary>
		/// <param name="chunks">The raw replication chunks.</param>
		public PullArguments(IEnumerable<Remoting.PullReplicationChunk> chunks)
		{
			this.chunks = chunks.ToArray ();
		}


		/// <summary>
		/// Gets the <see cref="Epsitec.Cresus.Remoting.PullReplicationChunk"/> matching the
		/// specified table.
		/// </summary>
		/// <value>The replication chunk.</value>
		public Remoting.PullReplicationChunk this[Database.DbId tableId]
		{
			get
			{
				for (int i = 0; i < chunks.Length; i++)
				{
					if (chunks[i].TableId == tableId)
					{
						return chunks[i];
					}
				}

				return Remoting.PullReplicationChunk.Empty;
			}
		}

		/// <summary>
		/// Gets the <see cref="Epsitec.Cresus.Remoting.PullReplicationChunk"/> matching the
		/// specified table.
		/// </summary>
		/// <value>The replication chunk.</value>
		public Remoting.PullReplicationChunk this[Database.DbTable table]
		{
			get
			{
				return this[table.Key.Id];
			}
		}


		/// <summary>
		/// Determines whether the pull arguments contain a definition for the
		/// specified table.
		/// </summary>
		/// <param name="tableId">The table id.</param>
		/// <returns>
		/// 	<c>true</c> if there is such a table; otherwise, <c>false</c>.
		/// </returns>
		public bool Contains(Database.DbId tableId)
		{
			return this.chunks.Any (c => c.TableId == tableId);
		}

		/// <summary>
		/// Determines whether the pull arguments contain a definition for the
		/// specified table.
		/// </summary>
		/// <param name="tableId">The table id.</param>
		/// <returns>
		/// 	<c>true</c> if there is such a table; otherwise, <c>false</c>.
		/// </returns>
		public bool Contains(Database.DbTable table)
		{
			return this.Contains (table.Key.Id);
		}


		readonly Remoting.PullReplicationChunk[]	chunks;
	}
}
