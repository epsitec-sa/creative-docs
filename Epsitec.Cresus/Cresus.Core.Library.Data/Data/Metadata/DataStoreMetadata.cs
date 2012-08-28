//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Data.Metadata
{
	/// <summary>
	/// The <c>DataStoreMetadata</c> class defines metadata at the global level: tables,
	/// data sets, etc.
	/// </summary>
	public class DataStoreMetadata : CoreMetadata
	{
		public DataStoreMetadata()
		{
			this.tables          = new List<EntityTableMetadata> ();
			this.dataSets        = new List<DataSetMetadata> ();
			this.displayGroupIds = new List<Druid> ();
		}


		public IList<EntityTableMetadata>		Tables
		{
			get
			{
				return this.tables;
			}
		}

		public IList<DataSetMetadata>			DataSets
		{
			get
			{
				return this.dataSets;
			}
		}

		public IList<Druid>						DisplayGroupIds
		{
			get
			{
				return this.displayGroupIds;
			}
		}
		
		/// <summary>
		/// Retrieves the current data store metdata. This relies on <see cref="CoreContext"/>
		/// to find the required information.
		/// </summary>
		public static DataStoreMetadata			Current
		{
			get
			{
				return CoreContext.GetMetadata<DataStoreMetadata> ();
			}
		}



		public IEnumerable<DataSetDisplayGroup> GetSortedDisplayGroups()
		{
			var groups = new Dictionary<Druid, DataSetDisplayGroup> ();

			foreach (var dataSet in this.dataSets)
			{
				DataSetDisplayGroup group;
				if (groups.TryGetValue (dataSet.DisplayGroupId, out group) == false)
				{
					group = new DataSetDisplayGroup (dataSet.DisplayGroupId);
					groups[dataSet.DisplayGroupId] = group;
				}
			}
			
			return this.DisplayGroupIds.Where (x => groups.ContainsKey (x)).Select (x => groups[x]);
		}


		/// <summary>
		/// Adds the definitions from the specified metadata into the current data store
		/// metadata. NOTE: for now, duplicates are not handled.
		/// </summary>
		/// <param name="metadata">The metadata.</param>
		public void Add(DataStoreMetadata metadata)
		{
			this.tables.AddRange (metadata.tables);
			this.dataSets.AddRange (metadata.dataSets);
			this.displayGroupIds.AddRange (metadata.displayGroupIds);
		}

		public override void Add(CoreMetadata metadata)
		{
			var dataStoreMetadata = metadata as DataStoreMetadata;

			if (dataStoreMetadata != null)
			{
				this.Add (dataStoreMetadata);
				return;
			}

			var entityTableMetadata = metadata as EntityTableMetadata;

			if (entityTableMetadata != null)
			{
				this.tables.Add (entityTableMetadata);
				return;
			}

			var dataSetMetadata = metadata as DataSetMetadata;

			if (dataSetMetadata != null)
			{
				this.dataSets.Add (dataSetMetadata);
				return;
			}
		}


		public EntityTableMetadata FindTable(Druid entityId)
		{
			return this.tables.FirstOrDefault (x => x.EntityId == entityId);
		}

		public DataSetMetadata FindDataSet(string name)
		{
			return this.dataSets.FirstOrDefault (x => x.DataSetName == name);
		}


		public XElement Save(string xmlNodeName)
		{
			return new XElement (xmlNodeName,
								 this.SerializeTables (),
								 this.SerializeDataSets (),
								 this.SerializeDisplayGroupIds ());
		}

		public static DataStoreMetadata Restore(XElement xml)
		{
			var xmlTables   = xml.Element (Xml.Tables).Elements ();
			var xmlDataSets = xml.Element (Xml.DataSets).Elements ();
			var xmlGroupIds = xml.Element (Xml.DisplayGroups).Elements ();
			
			var metadata  = new DataStoreMetadata ();

			metadata.tables.AddRange (xmlTables.Select (x => EntityTableMetadata.Restore (x)));
			metadata.dataSets.AddRange (xmlDataSets.Select (x => DataSetMetadata.Restore (x)));
			metadata.displayGroupIds.AddRange (xmlGroupIds.Select (x => Druid.Parse (x.Attribute (Xml.Id).Value)));

			return metadata;
		}



		private XElement SerializeTables()
		{
			return new XElement (Xml.Tables, this.tables.Select (x => x.Save (Xml.Table)));
		}

		private XElement SerializeDataSets()
		{
			return new XElement (Xml.DataSets, this.dataSets.Select (x => x.Save (Xml.DataSet)));
		}

		private XElement SerializeDisplayGroupIds()
		{
			return new XElement (Xml.DisplayGroups,
				this.displayGroupIds.Select (x =>
					new XElement (Xml.Group,
						new XAttribute (Xml.Id,
							x.ToCompactString ()))));
		}


		#region Xml Class

		private static class Xml
		{
			public static readonly string		Tables   = "T";
			public static readonly string		DataSets = "D";
			public static readonly string		DisplayGroups = "G";

			public static readonly string		Table   = "t";
			public static readonly string		DataSet = "d";
			public static readonly string		Group = "g";
			public static readonly string		Id = "id";
		}

		#endregion


		private readonly List<EntityTableMetadata>	tables;
		private readonly List<DataSetMetadata>	dataSets;
		private readonly List<Druid>			displayGroupIds;
	}
}
