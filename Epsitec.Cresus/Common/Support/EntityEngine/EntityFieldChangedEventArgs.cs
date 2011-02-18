//	Copyright © 2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>EntityChangedEventArgs</c> class is used to report changes related to
	/// an entity contents.
	/// </summary>
	public class EntityFieldChangedEventArgs : EntityEventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityFieldChangedEventArgs"/> class.
		/// </summary>
		/// <param name="entity">The entity.</param>
		public EntityFieldChangedEventArgs(AbstractEntity entity, string id, object oldValue, object newValue)
			: base (entity)
		{
			this.id = id;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}


		/// <summary>
		/// Gets the id of the field which changed.
		/// </summary>
		/// <value>The id of the field.</value>
		public string Id
		{
			get
			{
				return this.id;
			}
		}

		/// <summary>
		/// Gets the old value, if any.
		/// </summary>
		/// <value>The old value.</value>
		public object OldValue
		{
			get
			{
				return this.oldValue;
			}
		}

		/// <summary>
		/// Gets the new value, if any.
		/// </summary>
		/// <value>The new value.</value>
		public object NewValue
		{
			get
			{
				return this.newValue;
			}
		}

		
		private readonly string id;
		private readonly object oldValue;
		private readonly object newValue;
	}
}
