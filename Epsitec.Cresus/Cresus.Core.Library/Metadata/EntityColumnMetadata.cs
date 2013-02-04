//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.DataLayer.Expressions;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;
using Epsitec.Common.Support;

namespace Epsitec.Cresus.Core.Metadata
{
	/// <summary>
	/// The <c>EntityColumnMetadata</c> class defines a column (i.e. a direct or indirect field
	/// of an entity), including its sort order and display mode.
	/// </summary>
	public sealed class EntityColumnMetadata : EntityColumn
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityColumnMetadata"/> class. This should
		/// not be called directly. Use <see cref="EntityMetadataRecorder.Column"/> instead.
		/// </summary>
		/// <param name="expression">The lambda expression (as an expression, not as compiled code).</param>
		/// <param name="name">The name associated with the column.</param>
		/// <param name="sortOrder">The sort order.</param>
		/// <param name="displayMode">The display mode.</param>
		/// <param name="sortIndex">The sort index (0 = most important criterion).</param>
		public EntityColumnMetadata(LambdaExpression expression, FormattedText name, ColumnSortOrder sortOrder = ColumnSortOrder.None, ColumnDisplayMode displayMode = ColumnDisplayMode.Visible, int sortIndex = 0)
			: base (expression, name)
		{
			this.defaultSort    = new EntityColumnSort
			{
				SortOrder = sortOrder,
			};

			this.DefaultSortIndex = sortIndex;

			this.defaultFilter  = new EntityColumnFilter ();
			this.defaultDisplay = new EntityColumnDisplay
			{
				Mode = displayMode
			};
		}

		public EntityColumnMetadata(IDictionary<string, string> data)
			: base (data)
		{
			this.defaultSort    = new EntityColumnSort ();
			this.defaultFilter  = new EntityColumnFilter ();
			this.defaultDisplay = new EntityColumnDisplay ();
		}


		public EntityColumnSort					DefaultSort
		{
			get
			{
				return this.defaultSort;
			}
			set
			{
				this.defaultSort = value;
			}
		}

		public EntityColumnFilter				DefaultFilter
		{
			get
			{
				return this.defaultFilter;
			}
			set
			{
				this.defaultFilter = value;
			}
		}

		public EntityColumnDisplay				DefaultDisplay
		{
			get
			{
				return this.defaultDisplay;
			}
			set
			{
				this.defaultDisplay = value;
			}
		}

		public int								DefaultSortIndex
		{
			get;
			set;
		}

		public int?								DefaultWidth
		{
			get;
			set;
		}


		public static EntityColumnMetadata Resolve(Druid entityId, string columnId)
		{
			//	TODO: should we add a cache to speed up the resolution?

			var table = DataStoreMetadata.Current.FindTable (entityId);

			if (table == null)
			{
				return null;
			}

			return table.FindColumn (columnId);
		}

		public static IEnumerable<EntityColumnMetadata> GetSortColumns(IEnumerable<EntityColumnMetadata> columns)
		{
			return from column in columns
				   where column.DefaultSort.SortOrder != ColumnSortOrder.None
				   orderby column.DefaultSortIndex ascending
				   select column;
		}


		protected override void Serialize(List<XAttribute> attributes)
		{
			base.Serialize (attributes);

			attributes.Add (new XAttribute (Xml.DefaultSortIndex, this.DefaultSortIndex.ToString (System.Globalization.CultureInfo.InvariantCulture)));
			attributes.Add (new XAttribute (Xml.DefaultWidth, this.DefaultWidth.ToString ()));
		}

		protected override void Serialize(List<XElement> elements)
		{
			base.Serialize (elements);

			elements.Add (this.defaultSort.Save (Xml.DefaultSort));
			elements.Add (this.defaultFilter.Save (Xml.DefaultFilter));
			elements.Add (this.defaultDisplay.Save (Xml.DefaultDisplay));
		}

		protected override void Deserialize(XElement xml)
		{
			base.Deserialize (xml);

			this.DefaultSortIndex = InvariantConverter.ToInt (xml.Attribute (Xml.DefaultSortIndex));

			var xDefaultWitdh = xml.Attribute (Xml.DefaultWidth);
			this.DefaultWidth = xDefaultWitdh == null
				? (int?) null
				: InvariantConverter.ToInt (xDefaultWitdh);

			this.defaultSort    = EntityColumnSort.Restore (xml.Element (Xml.DefaultSort)) ?? this.defaultSort;
			this.defaultFilter  = EntityColumnFilter.Restore (xml.Element (Xml.DefaultFilter)) ?? this.defaultFilter;
			this.defaultDisplay = EntityColumnDisplay.Restore (xml.Element (Xml.DefaultDisplay)) ?? this.defaultDisplay;
		}


		#region Xml Class

		private static class Xml
		{
			public const string					DefaultSortIndex = "si";
			public const string					DefaultSort    = "sort";
			public const string					DefaultFilter  = "filt";
			public const string					DefaultDisplay = "disp";
			public const string					DefaultWidth   = "dw";
		}

		#endregion


		private EntityColumnSort				defaultSort;
		private EntityColumnFilter				defaultFilter;
		private EntityColumnDisplay				defaultDisplay;
	}
}