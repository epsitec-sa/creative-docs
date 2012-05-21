//	Copyright © 2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>EntityContextEventArgs</c> class is used to report changes related to
	/// an entity with respect to an <see cref="EntityContext"/>.
	/// </summary>
	public class EntityContextEventArgs : EntityEventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityContextEventArgs"/> class.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <param name="oldContext">The old context.</param>
		/// <param name="newContext">The new context.</param>
		public EntityContextEventArgs(AbstractEntity entity, EntityContext oldContext, EntityContext newContext)
			: base (entity)
		{
			this.oldContext = oldContext;
			this.newContext = newContext;
		}

		/// <summary>
		/// Gets the old context.
		/// </summary>
		/// <value>The context.</value>
		public EntityContext OldContext
		{
			get
			{
				return this.oldContext;
			}
		}

		/// <summary>
		/// Gets the new context.
		/// </summary>
		/// <value>The context.</value>
		public EntityContext NewContext
		{
			get
			{
				return this.newContext;
			}
		}

		private EntityContext oldContext;
		private EntityContext newContext;
	}
}
