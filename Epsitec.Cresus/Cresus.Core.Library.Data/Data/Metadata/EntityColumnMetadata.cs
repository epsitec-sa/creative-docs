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
		public EntityColumnMetadata(LambdaExpression expression, FormattedText name, SortOrder sortOrder)
			: this (expression, name, EntityColumnMetadata.Convert (sortOrder))
		{
		}

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
			this.sortOrder   = sortOrder;
			this.sortIndex   = sortIndex;
			this.displayMode = displayMode;
		}

		public EntityColumnMetadata(IDictionary<string, string> data)
			: base (data)
		{
			this.sortOrder   = data[Strings.SortOrder].ToEnum<ColumnSortOrder> ();
			this.sortIndex   = InvariantConverter.ToInt (data[Strings.SortIndex]);
			this.displayMode = data[Strings.DisplayMode].ToEnum<ColumnDisplayMode> ();
		}


		public ColumnSortOrder					SortOrder
		{
			get
			{
				return this.sortOrder;
			}
			set
			{
				this.sortOrder = value;
			}
		}

		public int								SortIndex
		{
			get
			{
				return this.sortIndex;
			}
			set
			{
				this.sortIndex = value;
			}
		}

		public ColumnDisplayMode				DisplayMode
		{
			get
			{
				return this.displayMode;
			}
			set
			{
				this.displayMode = value;
			}
		}

		
		public SortClause ToSortClause(AbstractEntity example)
		{
			if (this.SortOrder == ColumnSortOrder.None)
			{
				return null;
			}

			var fieldEntity = this.GetLeafEntity (example, NullNodeAction.CreateMissing);
			var fieldId     = this.GetLeafFieldId ();

			var fieldNode = new ValueField (fieldEntity, fieldId);
			var sortOrder = EntityColumnMetadata.Convert (this.sortOrder);

			return new SortClause (fieldNode, sortOrder);
		}

		protected override void Serialize(List<XAttribute> attributes)
		{
			base.Serialize (attributes);

			attributes.Add (new XAttribute (Strings.SortOrder, this.sortOrder.ToString ()));
			attributes.Add (new XAttribute (Strings.SortIndex, this.sortIndex.ToString (System.Globalization.CultureInfo.InvariantCulture)));
			attributes.Add (new XAttribute (Strings.DisplayMode, this.displayMode.ToString ()));
		}


		public static ColumnSortOrder Convert(SortOrder value)
		{
			switch (value)
			{
				case Epsitec.Cresus.DataLayer.Expressions.SortOrder.Ascending:
					return ColumnSortOrder.Ascending;
				case Epsitec.Cresus.DataLayer.Expressions.SortOrder.Descending:
					return ColumnSortOrder.Descending;
			}

			throw new System.NotImplementedException (string.Format ("{0} not implemented", value.GetQualifiedName ()));
		}

		public static SortOrder Convert(ColumnSortOrder value)
		{
			switch (value)
			{
				case ColumnSortOrder.Ascending:
					return Epsitec.Cresus.DataLayer.Expressions.SortOrder.Ascending;
				case ColumnSortOrder.Descending:
					return Epsitec.Cresus.DataLayer.Expressions.SortOrder.Descending;
			}

			throw new System.NotImplementedException (string.Format ("{0} not implemented", value.GetQualifiedName ()));
		}


		#region Strings Class

		private static class Strings
		{
			public static readonly string		SortOrder = "sort.o";
			public static readonly string		SortIndex = "sort.i";
			public static readonly string		DisplayMode = "disp";
		}

		#endregion

		private ColumnSortOrder					sortOrder;
		private int								sortIndex;
		private ColumnDisplayMode				displayMode;
	}
}