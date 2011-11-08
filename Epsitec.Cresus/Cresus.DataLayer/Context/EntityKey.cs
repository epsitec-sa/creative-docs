//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Schema;

namespace Epsitec.Cresus.DataLayer.Context
{
	/// <summary>
	/// The <c>EntityKey</c> structure encodes the identity of an <see cref="EntityKey"/> in the
	/// database using a <see cref="DbKey"/> and a <see cref="Druid"/>.
	/// </summary>
	public struct EntityKey : System.IEquatable<EntityKey>
	{
		/// <summary>
		/// Builds a new <c>EntityKey</c> which identifies an <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="entityId">The id of the <see cref="AbstractEntity"/>.</param>
		/// <param name="rowKey">The row key of the <see cref="AbstractEntity"/> in the database.</param>
		public EntityKey(Druid entityId, DbKey rowKey)
		{
			this.entityId = entityId;
			this.rowKey = rowKey;
		}

		/// <summary>
		/// Creates the <c>EntityKey</c> corresponding the <paramref name="entity"/> and
		/// <paramref name="rowKey"/>.
		/// </summary>
		/// <param name="rowKey">The <see cref="DbKey"/> of the <see cref="AbstractEntity"/> in the database.</param>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose <see cref="EntityKey"/> to create.</param>
		/// <returns>The corresponding <see cref="EntityKey"/>.</returns>
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
		public DbKey							RowKey
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
		public Druid							EntityId
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
		public bool								IsEmpty
		{
			get
			{
				return this.RowKey.IsEmpty || this.EntityId.IsEmpty;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this entity defines template data. All template
		/// data can be replaced when the user's database gets upgraded; the user should not
		/// be allowed to edit template data.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the entity defines template data; otherwise, <c>false</c>.
		/// </value>
		public bool								IsTemplate
		{
			get
			{
				if (this.RowKey.IsEmpty)
				{
					return false;
				}
				else
				{
					return this.RowKey.Id < Schema.EntitySchemaBuilder.AutoIncrementStartValue;
				}
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

		/// <summary>
		/// Returns a <c>string</c> that represents this instance. Use <see cref="Parse"/>
		/// to convert it back into an <see cref="EntityKey"/>.
		/// </summary>
		/// <returns>
		/// A <c>string</c> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return string.Concat (this.entityId.ToString (), "/", this.rowKey.Id.ToString ());
		}

		/// <summary>
		/// Parses the value into an <see cref="EntityKey"/>.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The <see cref="EntityKey"/> or <c>null</c> if the value is empty.</returns>
		public static EntityKey? Parse(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return null;
			}

			int separator = value.IndexOf ("]/");

			if (separator < 2)
			{
				throw new System.FormatException ("Invalid entity key format");
			}

			string entityIdValue = value.Substring (0, separator+1);
			string rowKeyValue   = value.Substring (separator+2);

			return new EntityKey (Druid.Parse (entityIdValue), new DbKey (DbId.Parse (rowKeyValue)));
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
		/// Gets the normalized version of this instance. Normalized means that if this instance
		/// targets a sub type of an <see cref="AbstractEntity"/>, the normalized version targets
		/// the corresponding root type.
		/// </summary>
		/// <param name="entityTypeEngine">The <see cref="EntityTypeEngine"/> used for the normalization.</param>
		/// <returns>The normalized version of this instance.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entityTypeEngine"/> is <c>null</c>.</exception>
		internal EntityKey GetNormalizedEntityKey(EntityTypeEngine entityTypeEngine)
		{
			entityTypeEngine.ThrowIfNull ("entityTypeEngine");

			Druid rootEntityId = entityTypeEngine.GetRootType (this.entityId).CaptionId;

			return new EntityKey (rootEntityId, this.rowKey);
		}
		
		/// <summary>
		/// Creates a normalized <c>EntityKey</c>.
		/// </summary>
		/// <param name="entityTypeEngine">The <see cref="EntityContext"/> used for the normalization.</param>
		/// <param name="entityId">The id of the <see cref="AbstractEntity"></see>.</param>
		/// <param name="rowKey">The row key of the <see cref="AbstractEntity"></see> in the database.</param>
		/// <returns>A new normalized <c>EntityKey</c>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entityTypeEngine"/> is <c>null</c>.</exception>
		internal static EntityKey CreateNormalizedEntityKey(EntityTypeEngine entityTypeEngine, Druid entityId, DbKey rowKey)
		{
			return new EntityKey (entityId, rowKey).GetNormalizedEntityKey (entityTypeEngine);
		}

		
		/// <summary>
		/// An instance of the empty <see cref="EntityKey"/>.
		/// </summary>
		public static readonly EntityKey		Empty = new EntityKey ();
		

		/// <summary>
		/// The <see cref="DbKey"/> which tells the id of the <see cref="AbstractEntity"/> represented
		/// by this instance.
		/// </summary>
		private readonly DbKey					rowKey;

		/// <summary>
		/// The <see cref="Druid"/> which represents the type of the <see cref="AbstractEntity"/>
		/// represented by this instance.
		/// </summary>
		private readonly Druid					entityId;
	}
}
