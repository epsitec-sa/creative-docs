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

namespace Epsitec.Cresus.Core.Data.Metadata
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
				SortIndex = sortIndex
			};

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



		public static IEnumerable<EntityColumnMetadata> GetSortColumns(IEnumerable<EntityColumnMetadata> columns)
		{
			return from column in columns
				   where column.DefaultSort.SortOrder != ColumnSortOrder.None
				   orderby column.DefaultSort.SortIndex ascending
				   select column;
		}


		protected override void Serialize(List<XAttribute> attributes)
		{
			base.Serialize (attributes);
		}

		protected override void Serialize(List<XElement> elements)
		{
			base.Serialize (elements);

			elements.Add (this.defaultSort.Save (Strings.DefaultSort));
			elements.Add (this.defaultFilter.Save (Strings.DefaultFilter));
			elements.Add (this.defaultDisplay.Save (Strings.DefaultDisplay));
		}

		protected override void Deserialize(XElement xml)
		{
			base.Deserialize (xml);

			this.defaultSort    = EntityColumnSort.Restore (xml.Element (Strings.DefaultSort)) ?? this.defaultSort;
			this.defaultFilter  = EntityColumnFilter.Restore (xml.Element (Strings.DefaultFilter)) ?? this.defaultFilter;
			this.defaultDisplay = EntityColumnDisplay.Restore (xml.Element (Strings.DefaultDisplay)) ?? this.defaultDisplay;
		}


		#region Strings Class

		private static class Strings
		{
			public static readonly string		DefaultSort    = "sort";
			public static readonly string		DefaultFilter  = "filter";
			public static readonly string		DefaultDisplay = "display";
		}

		#endregion


		private EntityColumnSort				defaultSort;
		private EntityColumnFilter				defaultFilter;
		private EntityColumnDisplay				defaultDisplay;
	}
}