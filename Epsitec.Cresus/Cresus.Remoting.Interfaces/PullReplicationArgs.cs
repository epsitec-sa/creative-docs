//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// La classe PullReplicationArgs décrit une table et des lignes qui doivent
	/// être répliquées.
	/// </summary>
	
	[System.Serializable]
	
	public struct PullReplicationArgs
	{
		public PullReplicationArgs(long table_id, System.Collections.ICollection list)
		{
			this.table_id = table_id;
			this.row_ids  = new long[list.Count];
			list.CopyTo (this.row_ids, 0);
		}
		
		
		public long								TableId
		{
			get
			{
				return this.table_id;
			}
		}
		
		public long[]							RowIds
		{
			get
			{
				return this.row_ids;
			}
		}

		public bool								IsEmpty
		{
			get
			{
				return this.row_ids == null;
			}
		}


		public static PullReplicationArgs		Empty
		{
			get
			{
				PullReplicationArgs args = new PullReplicationArgs ();
				return args;
			}
		}
		
		private long							table_id;
		private long[]							row_ids;
	}
}
