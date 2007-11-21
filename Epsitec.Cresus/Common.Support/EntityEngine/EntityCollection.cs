//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using System.Collections.Generic;

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>EntityCollection</c> class is used to store and represent lists
	/// of data for collection fields in a parent entity.
	/// </summary>
	/// <typeparam name="T">The type of the list items.</typeparam>
	public sealed class EntityCollection<T> : ObservableList<T>, IEntityCollection where T : AbstractEntity
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityCollection&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="containerFieldId">The container field id.</param>
		/// <param name="container">The container entity.</param>
		/// <param name="copyOnWrite">If set to <c>true</c>, the list will be set into "copy on write" mode.</param>
		public EntityCollection(string containerFieldId, AbstractEntity container, bool copyOnWrite)
		{
			this.id = containerFieldId;
			this.container = container;
			this.state = copyOnWrite ? State.CopyOnWrite : State.Default;
		}

		/// <summary>
		/// Resets the list to the "copy on write" mode.
		/// </summary>
		public void ResetCopyOnWrite()
		{
			this.state = State.CopyOnWrite;
		}

		/// <summary>
		/// Gets the observable list on which to apply modifications.
		/// If in "copy on write" mode, this will return a writable copy of 
		/// the list instead of the list itself.
		/// </summary>
		/// <returns>The list to use.</returns>
		protected override ObservableList<T> GetWorkingList()
		{
			if (this.container.IsDefiningOriginalValues)
			{
				//	The container is in the special "original value definition mode"
				//	and we need not to worry about copy on write.

				return this;
			}
			else
			{
				//	If this list is in copy on write mode, then we ask the container
				//	to generate a copy, which will be stored in the modified data
				//	version of the container field :

				switch (this.state)
				{
					case State.CopyOnWrite:
						this.state = State.Copied;
						return this.container.CopyFieldCollection<T> (this.id, this);

					case State.Copied:
						throw new System.InvalidOperationException ("Copied collection may not be changed");

					case State.Default:
						return this;

					default:
						throw new System.NotImplementedException ();
				}
			}
		}

		#region IEntityCollection Members

		void IEntityCollection.ResetCopyOnWrite()
		{
			this.ResetCopyOnWrite ();
		}

		void IEntityCollection.CopyOnWrite()
		{
			if (this.state == State.CopyOnWrite)
			{
				this.state = State.Copied;
				this.container.CopyFieldCollection<T> (this.id, this);
			}
		}

		bool IEntityCollection.UsesCopyOnWriteBehavior
		{
			get
			{
				return this.state == State.CopyOnWrite;
			}
		}

		#endregion

		#region Private State Enumeration

		private enum State
		{
			Default,
			CopyOnWrite,
			Copied
		}

		#endregion

		private string id;
		private AbstractEntity container;
		private State state;
	}
}
