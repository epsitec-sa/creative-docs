//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using Epsitec.Cresus.Database;

using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.DataLayer.Context
{


	/// <summary>
	/// The <c>EntityKey</c> structure encodes the identity of an entity in
	/// the database using a <see cref="DbKey"/> and entity id pair.
	/// </summary>
	public struct EntityKey : System.IEquatable<EntityKey>
	{
		
		
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityKey"/> struct.
		/// </summary>
		/// <param name="rowKey">The row key in the database.</param>
		/// <param name="entityId">The entity id.</param>
		public EntityKey(DbKey rowKey, Druid entityId)
		{
			this.rowKey   = rowKey;
			this.entityId = entityId;
		}


		/// <summary>
		/// Gets the row key in the database.
		/// </summary>
		/// <value>The row key.</value>
		public DbKey RowKey
		{
			get
			{
				return this.rowKey;
			}
		}


		/// <summary>
		/// Gets the entity id.
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
		/// Gets a value indicating whether this key is empty.
		/// </summary>
		/// <value><c>true</c> if this key is empty; otherwise, <c>false</c>.</value>
		public bool IsEmpty
		{
			get
			{
				return this.RowKey.IsEmpty && this.EntityId.IsEmpty;
			}
		}


		#region IEquatable<EntityKey> Members


		public bool Equals(EntityKey other)
		{
			return (this.rowKey == other.RowKey) && (this.entityId == other.EntityId);
		}


		#endregion


		public override bool Equals(object obj)
		{
			if (obj is EntityKey)
			{
				return base.Equals ((EntityKey) obj);
			}
			else
			{
				return false;
			}
		}


		public override int GetHashCode()
		{
			return this.rowKey.GetHashCode () ^ this.entityId.GetHashCode ();
		}


		public static bool operator==(EntityKey a, EntityKey b)
		{
			return a.Equals (b);
		}


		public static bool operator!=(EntityKey a, EntityKey b)
		{
			return !a.Equals (b);
		}


		private readonly DbKey rowKey;


		private readonly Druid entityId;


		public static readonly EntityKey Empty = new EntityKey ();


	}


}
