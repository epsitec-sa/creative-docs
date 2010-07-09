//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer.Context
{
	/// <summary>
	/// The <c>EntityDataMapping</c> class maps entity instances with their data
	/// row(s) in the <see cref="System.Data.DataSet"/> associated with the live
	/// <see cref="DataContext"/>.
	/// </summary>
	internal sealed class EntityDataMapping : System.IEquatable<EntityDataMapping>, IReadOnly
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityDataMapping"/> class.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <param name="entityId">The entity id.</param>
		/// <param name="rootEntityId">The root entity id.</param>
		public EntityDataMapping(AbstractEntity entity, Druid entityId, Druid rootEntityId)
		{
			this.entity = entity;
			this.entityId = entityId;
			this.rootEntityId = rootEntityId;
			this.rowKey = new DbKey ();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityDataMapping"/> class.
		/// </summary>
		/// <param name="rowKey">The row key.</param>
		/// <param name="entityId">The entity id.</param>
		/// <param name="rootEntityId">The root entity id.</param>
		public EntityDataMapping(DbKey rowKey, Druid entityId, Druid rootEntityId)
		{
			this.entityId = entityId;
			this.rootEntityId = rootEntityId;
			this.rowKey = rowKey;
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
				return this.entityId;
			}
		}

		/// <summary>
		/// Gets the associated root entity id.
		/// </summary>
		/// <value>The root entity id.</value>
		public Druid RootEntityId
		{
			get
			{
				return this.rootEntityId;
			}
		}

		/// <summary>
		/// Gets or sets the serialization generation.
		/// </summary>
		/// <value>The serialization generation.</value>
		public long SerialGeneration
		{
			get;
			set;
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

		/// <summary>
		/// Gets a value indicating whether this instance is read only.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is read only; otherwise, <c>false</c>.
		/// </value>
		public bool IsReadOnly
		{
			get
			{
				return this.rowKey.IsDefinitive;
			}
		}

		#region IEquatable<EntityDataMapping> Members

		public bool Equals(EntityDataMapping other)
		{
			return this.rootEntityId == other.rootEntityId
				&& this.rowKey == other.rowKey;
		}

		#endregion

		#region IReadOnly Members

		bool IReadOnly.IsReadOnly
		{
			get
			{
				return this.IsReadOnly;
			}
		}

		#endregion

		/// <summary>
		/// Compares this instance with the specified row key and entity id pair.
		/// </summary>
		/// <param name="rowKey">The row key.</param>
		/// <param name="rootEntityId">The root entity id.</param>
		/// <returns><c>true</c> if this instance matches the row key and entity
		/// id pair; otherwise, <c>false</c>.</returns>
		public bool Equals(DbKey rowKey, Druid rootEntityId)
		{
			return this.rowKey.Id == rowKey.Id
				&& this.rootEntityId == rootEntityId;
		}
		
		public override bool Equals(object obj)
		{
			EntityDataMapping other = obj as EntityDataMapping;
			
			if (other != null)
			{
				return this.Equals (other);
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			if (this.IsReadOnly)
			{
				return this.rowKey.GetHashCode () ^ this.rootEntityId.GetHashCode ();
			}
			else
			{
				throw new System.InvalidOperationException ("Unstable hash value: object is still mutable");
			}
		}

		private readonly AbstractEntity entity;
		private readonly Druid entityId;
		private readonly Druid rootEntityId;
		private DbKey rowKey;
	}
}
