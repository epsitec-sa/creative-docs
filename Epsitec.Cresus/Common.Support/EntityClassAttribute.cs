//	Copyright � 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>EntityClassAttribute</c> class defines an <c>[EntityClass]</c>
	/// attribute, which is used by <see cref="EntityEngine.EntityResolver"/>
	/// to map DRUIDs to entity classes.
	/// </summary>
	
	[System.AttributeUsage (System.AttributeTargets.Assembly, AllowMultiple=true)]
	public sealed class EntityClassAttribute : System.Attribute
	{
		public EntityClassAttribute(string entityId, System.Type entityType)
		{
			this.entityId = Druid.Parse (entityId);
			this.entityType = entityType;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityClassAttribute"/> class.
		/// </summary>
		/// <param name="entityId">The DRUID (encoded as a raw <c>long</c> value) of the command.</param>
		public EntityClassAttribute(long entityId, System.Type entityType)
		{
			this.entityId = Druid.FromLong (entityId);
			this.entityType = entityType;
		}


		/// <summary>
		/// Gets the type of the entity class.
		/// </summary>
		/// <value>The type of the entity class.</value>
		public System.Type EntityType
		{
			get
			{
				return this.entityType;
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
		/// Gets the registered attributes for a specified assembly.
		/// </summary>
		/// <param name="assembly">The assembly.</param>
		/// <returns>The registered types.</returns>
		public static IEnumerable<EntityClassAttribute> GetRegisteredAttributes(System.Reflection.Assembly assembly)
		{
			System.Type entityBaseType = typeof (EntityEngine.AbstractEntity);

			foreach (EntityClassAttribute attribute in assembly.GetCustomAttributes (typeof (EntityClassAttribute), false))
			{
				if (attribute.entityType.IsSubclassOf (typeof (EntityEngine.AbstractEntity)))
				{
					yield return attribute;
				}
			}
		}
		
		private Druid entityId;
		private System.Type entityType;
	}
}
