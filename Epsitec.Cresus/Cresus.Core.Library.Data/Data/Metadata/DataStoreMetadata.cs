//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Data.Metadata
{
	/// <summary>
	/// The <c>DataStoreMetadata</c> class defines metadata at the global level: tables,
	/// data sets, etc.
	/// </summary>
	public class DataStoreMetadata
	{
		public DataStoreMetadata()
		{
			this.tables = new List<EntityMetadata> ();
			this.dataSets = new List<DataSetMetadata> ();
		}


		public IList<EntityMetadata>			Tables
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


		public XElement Save(string xmlNodeName)
		{
			return new XElement (xmlNodeName,
								 this.SerializeTables (),
								 this.SerializeDataSets ());
		}

		public static DataStoreMetadata Restore(XElement xml)
		{
			var xmlTables   = xml.Element (Strings.Tables).Elements ();
			var xmlDataSets = xml.Element (Strings.DataSets).Elements ();
			
			var metadata  = new DataStoreMetadata ();

			metadata.tables.AddRange (xmlTables.Select (x => EntityMetadata.Restore (x)));
			metadata.dataSets.AddRange (xmlDataSets.Select (x => DataSetMetadata.Restore (x)));

			return metadata;
		}


		private XElement SerializeTables()
		{
			return new XElement (Strings.Tables, this.tables.Select (x => x.Save (Strings.Table)));
		}

		private XElement SerializeDataSets()
		{
			return new XElement (Strings.DataSets, this.dataSets.Select (x => x.Save (Strings.DataSet)));
		}


		#region Strings Class

		private static class Strings
		{
			public static readonly string		Tables   = "tables";
			public static readonly string		DataSets = "dataSets";

			public static readonly string		Table   = "table";
			public static readonly string		DataSet = "dataSet";
		}

		#endregion


		private readonly List<EntityMetadata>	tables;
		private readonly List<DataSetMetadata>	dataSets;
	}
}
