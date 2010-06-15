//	Copyright © 2007-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>EntityEventArgs</c> class is used to report changes related to
	/// an entity.
	/// </summary>
	public class EntityEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityEventArgs"/> class.
		/// </summary>
		/// <param name="entity">The entity.</param>
		public EntityEventArgs(AbstractEntity entity)
		{
			this.entity = entity;
		}

		/// <summary>
		/// Gets the entity.
		/// </summary>
		/// <value>The entity.</value>
		public AbstractEntity Entity
		{
			get
			{
				return this.entity;
			}
		}

		private AbstractEntity entity;
	}
}
