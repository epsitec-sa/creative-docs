//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Data.Extraction
{
	/// <summary>
	/// The <c>EntityDataMetadata</c> class is a collection of <see cref="EntityDataColum"/>
	/// instances.
	/// </summary>
	public sealed class EntityDataMetadata
	{
		internal EntityDataMetadata(Druid entityId, IEnumerable<EntityDataColumn> columns)
		{
			this.entityId = entityId;
			this.columns = columns.ToArray ();
		}

		private EntityDataMetadata(IDictionary<string, string> data, IEnumerable<EntityDataColumn> columns)
		{
			this.entityId = Druid.Parse (data[Strings.EntityId]);
			this.columns = columns.ToArray ();
		}


		public Druid							EntityId
		{
			get
			{
				return this.entityId;
			}
		}

		public IEnumerable<EntityDataColumn>	Columns
		{
			get
			{
				return this.columns;
			}
		}
		
		public int								ColumnCount
		{
			get
			{
				return this.columns.Length;
			}
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
		public static EntityDataMetadata Restore(XElement xml)
		{
			var data    = xml.Attributes ().ToDictionary (x => x.Name.LocalName, x => x.Value);
			var columns = xml.Elements (Strings.Column).Select (x => EntityColumn.Restore (x)).Cast<EntityDataColumn> ();

			return new EntityDataMetadata (data, columns);
		}

		
		private void Serialize(List<XAttribute> attributes)
		{
			attributes.Add (new XAttribute (Strings.EntityId, this.entityId.ToCompactString ()));
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
			public static readonly string		Column = "col";
		}

		#endregion

		private readonly Druid					entityId;
		private readonly EntityDataColumn[]		columns;
	}
}