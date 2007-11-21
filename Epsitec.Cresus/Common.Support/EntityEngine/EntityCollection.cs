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
	public sealed class EntityCollection<T> : ObservableList<T>, IEntityCollection, INotifyCollectionChangedProvider where T : AbstractEntity
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityCollection&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="containerFieldId">The container field id.</param>
		/// <param name="container">The container entity.</param>
		/// <param name="copyOnWrite">If set to <c>true</c>, the list will be set into "copy on write" mode.</param>
		public EntityCollection(string containerFieldId, AbstractEntity container, bool copyOnWrite)
		{
			this.containerFieldId = containerFieldId;
			this.container = container;
			this.state = copyOnWrite ? State.CopyOnWrite : State.Default;
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
						return this.container.CopyFieldCollection<T> (this.containerFieldId, this);

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

		/// <summary>
		/// Resets the collection to the unchanged copy on write state.
		/// </summary>
		public void ResetCopyOnWrite()
		{
			this.state = State.CopyOnWrite;
		}

		/// <summary>
		/// Copies the collection to a writable instance if the collection is
		/// still in the unchanged copy on write state.
		/// </summary>
		void IEntityCollection.CopyOnWrite()
		{
			if (this.state == State.CopyOnWrite)
			{
				this.state = State.Copied;
				this.container.CopyFieldCollection<T> (this.containerFieldId, this);
			}
		}

		/// <summary>
		/// Gets a value indicating whether this collection will create a copy
		/// before being modified.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the collection has the copy on write state; otherwise, <c>false</c>.
		/// </value>
		bool IEntityCollection.HasCopyOnWriteState
		{
			get
			{
				return this.state == State.CopyOnWrite;
			}
		}

		#endregion

		#region INotifyCollectionChangedProvider Members

		public INotifyCollectionChanged GetNotifyCollectionChangedSource()
		{
			return this;
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

		private readonly string containerFieldId;
		private readonly AbstractEntity container;
		private State state;
	}
}
