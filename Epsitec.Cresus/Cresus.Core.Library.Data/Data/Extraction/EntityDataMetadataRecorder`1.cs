//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Data.Extraction
{
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


		public EntityDataMetadataRecorder<TEntity> Column<TField>(Expression<System.Func<TEntity, TField>> expression)
			where TField : struct
		{
			var func = expression.Compile ();
			this.Add (new EntityDataColumn (expression, EntityDataConverter.GetFieldConverter (func)));
			return this;
		}

		public EntityDataMetadataRecorder<TEntity> Column<TField>(Expression<System.Func<TEntity, TField?>> expression)
			where TField : struct
		{
			var func = expression.Compile ();
			this.Add (new EntityDataColumn (expression, EntityDataConverter.GetFieldConverter (func)));
			return this;
		}

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
