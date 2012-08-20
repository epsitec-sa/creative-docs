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
	/// The <c>StoreMetadata</c> class defines metadata at the global level: tables, databases,
	/// etc.
	/// </summary>
	public class StoreMetadata
	{
		public StoreMetadata()
		{
			this.tables = new List<EntityMetadata> ();
			
		}


		public IList<EntityMetadata> Tables
		{
			get
			{
				return this.tables;
			}
		}


		public XElement Save(string xmlNodeName)
		{
			return new XElement (xmlNodeName,
								 this.SerializeTables ());
		}

		public static StoreMetadata Restore(XElement xml)
		{
			var xmlTables = xml.Element (Strings.Tables).Elements ();
			var metadata  = new StoreMetadata ();

			metadata.tables.AddRange (xmlTables.Select (x => EntityMetadata.Restore (x)));

			return metadata;
		}


		private XElement SerializeTables()
		{
			return new XElement (Strings.Tables, this.tables.Select (x => x.Save (Strings.Table)));
		}


		#region Strings Class

		private static class Strings
		{
			public static readonly string		Tables = "tables";
			public static readonly string		Table  = "table";
		}

		#endregion


		private readonly List<EntityMetadata>	tables;
	}
}
