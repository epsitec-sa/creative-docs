using Epsitec.Common.Types;

using Epsitec.Common.Support.EntityEngine;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Aider.Entities.Helpers
{


	public abstract class VirtualEventList<THostEntity, T> : VirtualList<THostEntity, T>
		where T : AbstractEntity, new ()
	{


		public VirtualEventList(THostEntity entity)
		    : base (entity)
		{
		    this.CollectionChanged += this.HandleCollectionChanged;
		}


		public override int MaxCount
		{
			get
			{
				return int.MaxValue;
			}
		}


		private void HandleCollectionChanged(object sender, CollectionChangedEventArgs eventArgs)
		{
			switch (eventArgs.Action)
			{
				case CollectionChangedAction.Add:
					this.HandleCollectionAddition (eventArgs);
					break;

				case CollectionChangedAction.Remove:
					this.HandleCollectionRemoval (eventArgs);
					break;

				case CollectionChangedAction.Replace:
				case CollectionChangedAction.Reset:
				case CollectionChangedAction.Move:
					throw new NotSupportedException ();

				default:
					throw new NotImplementedException ();
			}
		}


		private void HandleCollectionAddition(CollectionChangedEventArgs eventArgs)
		{
			foreach (var item in eventArgs.NewItems)
			{
				this.HandleCollectionAddition ((T) item);
			}
		}


		protected abstract void HandleCollectionAddition(T item);


		private void HandleCollectionRemoval(CollectionChangedEventArgs eventArgs)
		{
			foreach (var item in eventArgs.OldItems)
			{
				this.HandleCollectionRemoval ((T) item);
			}
		}


		protected abstract void HandleCollectionRemoval(T item);


		protected override void ReplaceItems(IList<T> list)
		{
			// Nothing to do here, as we handle this case with the event handlers.
		}


	}


}
