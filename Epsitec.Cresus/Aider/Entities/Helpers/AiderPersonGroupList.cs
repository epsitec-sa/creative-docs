using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Expressions;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Aider.Entities.Helpers
{
	
	public sealed class AiderPersonGroupList : VirtualList<AiderPersonEntity, AiderGroupParticipantEntity>
	{


		public AiderPersonGroupList(AiderPersonEntity entity)
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


		protected override IEnumerable<AiderGroupParticipantEntity> GetItems()
		{
			var businessContext = BusinessContextPool.GetCurrentContext (this.entity);
			var dataContext = businessContext.DataContext;

			var example = new AiderGroupParticipantEntity ()
			{
				Group = new AiderGroupEntity (),
			};

			var request = new Request ()
			{
				RootEntity = example,
			};

			request.AddCondition (dataContext, example, g => g.Person == this.entity);
			request.AddCondition (dataContext, example, g => g.StartDate == null || g.StartDate <= Date.Today);
			request.AddCondition (dataContext, example, g => g.EndDate == null || g.EndDate > Date.Today);

			request.AddSortClause (ValueField.Create (example.Group, g => g.Name), SortOrder.Ascending);

			return dataContext.GetByRequest<AiderGroupParticipantEntity> (request);
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
			// Here we simply setup the newly created object so that it will appear in this list,
			// otherwise we'll have an empty group participation with no group and person that we
			// won't know that it was created with the intent of being attached to this person.

			var newItem = (AiderGroupParticipantEntity) eventArgs.NewItems[0];

			newItem.Person = this.entity;
			newItem.StartDate = Date.Today;
		}


		private void HandleCollectionRemoval(CollectionChangedEventArgs eventArgs)
		{
			// Here we create a new group participation because the one removed is also deleted from
			// the database, but we want to keep that information around.

			var oldItem = (AiderGroupParticipantEntity) eventArgs.OldItems[0];

			var businessContext = BusinessContextPool.GetCurrentContext (this.entity);

			var newParticipation = businessContext.CreateEntity<AiderGroupParticipantEntity> ();

			newParticipation.Person = oldItem.Person;
			newParticipation.Group = oldItem.Group;
			newParticipation.StartDate = oldItem.StartDate;
			newParticipation.EndDate = Date.Today;
		}


		protected override void ReplaceItems(IList<AiderGroupParticipantEntity> list)
		{
			// Nothing to do here, as we handle this case with the event handlers.
		}


	}


}
