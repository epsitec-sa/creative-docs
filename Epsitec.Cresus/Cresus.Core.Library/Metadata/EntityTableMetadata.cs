//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Metadata
{
	/// <summary>
	/// The <c>EntityTableMetadata</c> class is a collection of <see cref="EntityDataColum"/>
	/// instances.
	/// </summary>
	public sealed class EntityTableMetadata : CoreMetadata
	{
		internal EntityTableMetadata(Druid entityId, string name, IEnumerable<EntityColumnMetadata> columns)
		{
			this.entityId = entityId;
			this.name = name;
			this.columns  = new List<EntityColumnMetadata> (columns);
		}

		private EntityTableMetadata(IDictionary<string, string> data, IEnumerable<EntityColumnMetadata> columns)
			: this (Druid.Parse (data[Strings.EntityId]), data[Strings.Name], columns)
		{
		}


		public Druid							EntityId
		{
			get
			{
				return this.entityId;
			}
		}

		public string							Name
		{
			get
			{
				return this.name;
			}
		}

		public IList<EntityColumnMetadata>		Columns
		{
			get
			{
				return this.columns;
			}
		}


		public System.Type EntityType
		{
			get
			{
				return EntityInfo.GetType (this.entityId);
			}
		}


		public override void Add(CoreMetadata metadata)
		{
			throw new System.NotImplementedException ();
		}
		
		public IEnumerable<EntityColumnMetadata> GetSortColumns()
		{
			return EntityColumnMetadata.GetSortColumns (this.columns);
		}

		public EntityColumnMetadata FindColumn(string columnId)
		{
			return this.columns.FirstOrDefault (x => x.Id == columnId);
		}


		/// <summary>
		/// Saves the metadata as an XML tree.
		/// </summary>
		/// <param name="xmlNodeName">Name of the XML element.</param>
		/// <returns>The XML tree.</returns>
		public XElement Save(string xmlNodeName)
		{
			var attributes = new List<XAttribute> ();
			var columns = new List<XElement> ();

			this.Serialize (attributes);
			this.Serialize (columns);
			
			return new XElement (xmlNodeName, attributes, columns);
		}

		/// <summary>
		/// Restores the metadata based on the specified XML tree.
		/// </summary>
		/// <param name="xml">The XML tree.</param>
		/// <returns>The metadata.</returns>
		public static EntityTableMetadata Restore(XElement xml)
		{
			var data    = Xml.GetAttributeBag (xml);
			var columns = xml.Elements (Strings.Column).Select (x => EntityColumn.Restore (x)).Cast<EntityColumnMetadata> ();

			return new EntityTableMetadata (data, columns);
		}

		
		private void Serialize(List<XAttribute> attributes)
		{
			attributes.Add (new XAttribute (Strings.EntityId, this.entityId.ToCompactString ()));
			attributes.Add (new XAttribute (Strings.Name, this.name));
		}

		private void Serialize(List<XElement> columns)
		{
			foreach (var column in this.columns)
			{
				columns.Add (column.Save (Strings.Column));
			}
		}


		#region Strings Class

		private static class Strings
		{
			public static readonly string		EntityId = "eid";
			public static readonly string		Name	 = "n";
			public static readonly string		Column   = "col";
		}

		#endregion

		private readonly Druid						entityId;
		private readonly string						name;
		private readonly List<EntityColumnMetadata>	columns;
	}
}