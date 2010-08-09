//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;
using Epsitec.Common.Support.EntityEngine;

namespace Epsitec.Cresus.DataLayer.Context
{
	/// <summary>
	/// The <c>EntityKey</c> structure encodes the identity of an <see cref="EntityKey"/> in the
	/// database using a <see cref="DbKey"/> and a <see cref="Druid"/>.
	/// </summary>
	public struct EntityKey : System.IEquatable<EntityKey>
	{
		/// <summary>
		/// Builds a new <see cref="EntityKey"></see> which identifies an <see cref="AbstractEntity"></see>.
		/// </summary>
		/// <param name="entityId">The id of the <see cref="AbstractEntity"></see>.</param>
		/// <param name="rowKey">The row key of the <see cref="AbstractEntity"></see> in the database.</param>
		public EntityKey(Druid entityId, DbKey rowKey)
		{
			this.entityId = entityId;
			this.rowKey = rowKey;
		}

		/// <summary>
		/// Creates the <see cref="EntityKey"></see> corresponding the <paramref name="entity"></paramref> and
		/// <paramref name="rowKey"></paramref>.
		/// </summary>
		/// <param name="rowKey">The <see cref="DbKey"></see> of the <see cref="AbstractEntity"></see> in the database.</param>
		/// <param name="entity">The <see cref="AbstractEntity"></see> whose <see cref="EntityKey"></see> to create.</param>
		/// <returns>The corresponding <see cref="EntityKey"></see>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entity"/> is null.</exception>
		public EntityKey(AbstractEntity entity, DbKey rowKey)
		{
			entity.ThrowIfNull ("entity");

			this.entityId = entity.GetEntityStructuredTypeId ();
			this.rowKey = rowKey;
		}


		/// <summary>
		/// Gets the <see cref="DbKey"/> of the <see cref="AbstractEntity"/> in the database.
		/// </summary>
		/// <value>The <see cref="DbKey"/>.</value>
		public DbKey RowKey
		{
			get
			{
				return this.rowKey;
			}
		}

		/// <summary>
		/// Gets the <see cref="Druid"/> that represents the type of the <see cref="AbstractEntity"/>.
		/// </summary>
		/// <value>The <see cref="Druid"/>.</value>
		public Druid EntityId
		{
			get
			{
				return this.entityId;
			}
		}

		/// <summary>
		/// Tells whether this <see cref="EntityKey"/> is empty.
		/// </summary>
		/// <value><c>true</c> if this key is empty, <c>false</c> if it is not.</value>
		public bool IsEmpty
		{
			get
			{
				return this.RowKey.IsEmpty || this.EntityId.IsEmpty;
			}
		}


		#region IEquatable<EntityKey> Members


		/// <summary>
		/// Tells whether this <see cref="EntityKey"/> is equal to another.
		/// </summary>
		/// <param name="that">The other <see cref="EntityKey"/>.</param>
		/// <returns><c>true</c> if both <see cref="EntityKey"/> are equal, false if they are not.</returns>
		public bool Equals(EntityKey that)
		{
			return (this.rowKey == that.RowKey) && (this.entityId == that.EntityId);
		}


		#endregion


		/// <summary>
		/// Tells whether this <see cref="EntityKey"/> is equal to another object.
		/// </summary>
		/// <param name="that">The other <see cref="object"/>.</param>
		/// <returns><c>true</c> if both <see cref="object"/> are equal, false if they are not.</returns>
		public override bool Equals(object that)
		{
			return (that is EntityKey) && this.Equals ((EntityKey) that);
		}


		/// <summary>
		/// Computes the hash code of this instance.
		/// </summary>
		/// <returns>The hash code of this instance.</returns>
		public override int GetHashCode()
		{
			return this.rowKey.GetHashCode () ^ this.entityId.GetHashCode ();
		}

		public override string ToString()
		{
			return string.Concat (this.entityId.ToString (), "/", this.rowKey.Id.ToString ());
		}


		/// <summary>
		/// Tells whether two <see cref="EntityKey"/> are equal.
		/// </summary>
		/// <param name="a">The first <see cref="EntityKey"/>.</param>
		/// <param name="b">The second <see cref="EntityKey"/>.</param>
		/// <returns><c>true</c> if both <see cref="EntityKey"/> are equal, false if they are not.</returns>
		public static bool operator==(EntityKey a, EntityKey b)
		{
			return a.Equals (b);
		}

		/// <summary>
		/// Tells whether two <see cref="EntityKey"/> are different.
		/// </summary>
		/// <param name="a">The first <see cref="EntityKey"/>.</param>
		/// <param name="b">The second <see cref="EntityKey"/>.</param>
		/// <returns><c>true</c> if both <see cref="EntityKey"/> are different, false if they are not.</returns>
		public static bool operator!=(EntityKey a, EntityKey b)
		{
			return !a.Equals (b);
		}


		/// <summary>
		/// The <see cref="DbKey"/> which tells the id of the <see cref="AbstractEntity"/> represented
		/// by this instance.
		/// </summary>
		private readonly DbKey rowKey;

		/// <summary>
		/// The <see cref="Druid"/> which represents the type of the <see cref="AbstractEntity"/>
		/// represented by this instance.
		/// </summary>
		private readonly Druid entityId;


		/// <summary>
		/// An instance of the empty <see cref="EntityKey"/>.
		/// </summary>
		public static readonly EntityKey Empty = new EntityKey ();
	}
}
