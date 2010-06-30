//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Database;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Replication
{
	/// <summary>
	/// The <c>ReplicationData</c> class stores bulk replication data in an easily
	/// serializable format.
	/// </summary>
	
	[System.Serializable]
	public sealed class ReplicationData : System.Runtime.Serialization.ISerializable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReplicationData"/> class.
		/// </summary>
		public ReplicationData()
		{
			this.packedTableList = new List<PackedTableData> ();
		}


		/// <summary>
		/// Gets the collection of packed table data.
		/// </summary>
		/// <value>The collection of packed table data.</value>
		public IEnumerable<PackedTableData>		PackedTableData
		{
			get
			{
				return this.packedTableList;
			}
		}


		/// <summary>
		/// Adds the specified packed table data.
		/// </summary>
		/// <param name="tableData">The packed table data.</param>
		public void Add(PackedTableData tableData)
		{
			this.packedTableList.Add (tableData);
		}
		
		
		#region ISerializable Members
		
		private ReplicationData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			this.packedTableList = new List<PackedTableData> ((PackedTableData[]) info.GetValue (Strings.Tables, typeof (PackedTableData[])));
		}
		
		public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue (Strings.Tables, this.PackedTableData.ToArray ());
		}

		private static class Strings
		{
			public const string Tables = "Tables";
		}

		#endregion

		readonly List<PackedTableData>			packedTableList;
	}
}
