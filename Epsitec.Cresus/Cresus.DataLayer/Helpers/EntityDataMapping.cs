//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Helpers;

using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer.Helpers
{
	/// <summary>
	/// The <c>EntityDataMapping</c> class maps entity instances with their data
	/// row(s) in the <see cref="System.Data.DataSet"/> associated with the live
	/// <see cref="DataContext"/>.
	/// </summary>
	public class EntityDataMapping
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityDataMapping"/> class.
		/// </summary>
		/// <param name="entity">The entity.</param>
		public EntityDataMapping(AbstractEntity entity)
		{
			this.entity = entity;
			this.rowKey = new DbKey ();
		}

		/// <summary>
		/// Gets the associated entity.
		/// </summary>
		/// <value>The entity.</value>
		public AbstractEntity Entity
		{
			get
			{
				return this.entity;
			}
		}

		/// <summary>
		/// Gets the associated entity id.
		/// </summary>
		/// <value>The entity id.</value>
		public Druid EntityId
		{
			get
			{
				if (this.entityId.IsEmpty)
				{
					this.entityId = this.entity == null ? Druid.Empty : this.entity.GetEntityStructuredTypeId ();
				}

				return this.entityId;
			}
		}

		/// <summary>
		/// Gets or sets the row key for the associated entity.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">Thrown when a row
		/// key is modified after it has become definitive.</exception>
		/// <value>The row key.</value>
		public DbKey RowKey
		{
			get
			{
				return this.rowKey;
			}
			set
			{
				if ((this.rowKey.IsEmpty) ||
					(this.rowKey.IsTemporary && value.IsDefinitive))
				{
					this.rowKey = value;
				}
				else
				{
					throw new System.InvalidOperationException ("RowKey cannot be further modified");
				}
			}
		}

		private readonly AbstractEntity entity;
		private DbKey rowKey;
		private Druid entityId;
	}
}
