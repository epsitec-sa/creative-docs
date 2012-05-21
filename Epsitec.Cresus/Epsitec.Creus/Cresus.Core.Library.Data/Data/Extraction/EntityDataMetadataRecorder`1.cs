//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Data.Extraction
{
	/// <summary>
	/// The <c>EntityDataMetadataRecorder</c> class is used to record column definitions for
	/// an entity of type <typeparamref name="TEntity"/>.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	public sealed class EntityDataMetadataRecorder<TEntity> : EntityDataMetadataRecorder
			where TEntity : AbstractEntity
	{
		public EntityDataMetadataRecorder()
		{
			this.columns = new List<EntityDataColumn> ();
		}


		public override IEnumerable<EntityDataColumn> Columns
		{
			get
			{
				return this.columns;
			}
		}


		/// <summary>
		/// Declares a column accessor.
		/// </summary>
		/// <typeparam name="TField">The type of the field.</typeparam>
		/// <param name="expression">The expression.</param>
		/// <returns>Self, so that fluent interfaces can be used.</returns>
		public EntityDataMetadataRecorder<TEntity> Column<TField>(Expression<System.Func<TEntity, TField>> expression)
			where TField : struct
		{
			var func = expression.Compile ();
			this.Add (new EntityDataColumn (expression, EntityDataConverter.GetFieldConverter (func)));
			return this;
		}

		/// <summary>
		/// Declares a column accessor.
		/// </summary>
		/// <typeparam name="TField">The type of the field.</typeparam>
		/// <param name="expression">The expression.</param>
		/// <returns>Self, so that fluent interfaces can be used.</returns>
		public EntityDataMetadataRecorder<TEntity> Column<TField>(Expression<System.Func<TEntity, TField?>> expression)
			where TField : struct
		{
			var func = expression.Compile ();
			this.Add (new EntityDataColumn (expression, EntityDataConverter.GetFieldConverter (func)));
			return this;
		}

		/// <summary>
		/// Declares a column accessor.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns>Self, so that fluent interfaces can be used.</returns>
		public EntityDataMetadataRecorder<TEntity> Column(Expression<System.Func<TEntity, string>> expression)
		{
			var func = expression.Compile ();
			this.Add (new EntityDataColumn (expression, EntityDataConverter.GetFieldConverter (func)));
			return this;
		}


		private void Add(EntityDataColumn column)
		{
			this.columns.Add (column);
		}


		private readonly List<EntityDataColumn> columns;
	}
}
