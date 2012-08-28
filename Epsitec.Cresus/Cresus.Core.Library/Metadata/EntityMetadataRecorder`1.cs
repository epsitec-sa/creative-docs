//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.DataLayer.Expressions;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Metadata
{
	/// <summary>
	/// The <c>EntityMetadataRecorder</c> class is used to record column definitions for
	/// an entity of type <typeparamref name="TEntity"/>.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	public sealed class EntityMetadataRecorder<TEntity> : EntityMetadataRecorder
			where TEntity : AbstractEntity, new()
	{
		public EntityMetadataRecorder()
			: base (EntityInfo<TEntity>.GetTypeId ())
		{
			this.columns = new List<EntityColumnMetadata> ();
		}


		public override IEnumerable<EntityColumnMetadata> Columns
		{
			get
			{
				return this.columns;
			}
		}

		public override int						ColumnCount
		{
			get
			{
				return this.columns.Count;
			}
		}


		public static Expression<System.Func<TEntity, TField>> Expression<TField>(Expression<System.Func<TEntity, TField>> expression)
			where TField : struct
		{
			return expression;
		}

		public static Expression<System.Func<TEntity, TField?>> Expression<TField>(Expression<System.Func<TEntity, TField?>> expression)
			where TField : struct
		{
			return expression;
		}

		public static Expression<System.Func<TEntity, string>> Expression(Expression<System.Func<TEntity, string>> expression)
		{
			return expression;
		}

		
		/// <summary>
		/// Declares a column accessor.
		/// </summary>
		/// <typeparam name="TField">The type of the field.</typeparam>
		/// <param name="expression">The expression.</param>
		/// <param name="sortOrder">The sort order.</param>
		/// <param name="name">The user-friendly name of the column.</param>
		/// <returns>
		/// Self, so that fluent interfaces can be used.
		/// </returns>
		public EntityMetadataRecorder<TEntity> Column<TField>(Expression<System.Func<TEntity, TField>> expression, ColumnSortOrder sortOrder = ColumnSortOrder.Ascending, FormattedText name = default (FormattedText))
			where TField : struct
		{
			this.Add (new EntityColumnMetadata (expression, name, sortOrder));
			return this;
		}

		/// <summary>
		/// Declares a column accessor.
		/// </summary>
		/// <typeparam name="TField">The type of the field.</typeparam>
		/// <param name="expression">The expression.</param>
		/// <param name="sortOrder">The sort order.</param>
		/// <param name="name">The user-friendly name of the column.</param>
		/// <returns>
		/// Self, so that fluent interfaces can be used.
		/// </returns>
		public EntityMetadataRecorder<TEntity> Column<TField>(Expression<System.Func<TEntity, TField?>> expression, ColumnSortOrder sortOrder = ColumnSortOrder.Ascending, FormattedText name = default (FormattedText))
			where TField : struct
		{
			this.Add (new EntityColumnMetadata (expression, name, sortOrder));
			return this;
		}

		/// <summary>
		/// Declares a column accessor.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <param name="sortOrder">The sort order.</param>
		/// <param name="name">The user-friendly name of the column.</param>
		/// <returns>
		/// Self, so that fluent interfaces can be used.
		/// </returns>
		public EntityMetadataRecorder<TEntity> Column(Expression<System.Func<TEntity, string>> expression, ColumnSortOrder sortOrder = ColumnSortOrder.Ascending, FormattedText name = default (FormattedText))
		{
			this.Add (new EntityColumnMetadata (expression, name, sortOrder));
			return this;
		}


		private void Add(EntityColumnMetadata column)
		{
			this.columns.Add (column);
		}


		private readonly List<EntityColumnMetadata> columns;
	}
}
