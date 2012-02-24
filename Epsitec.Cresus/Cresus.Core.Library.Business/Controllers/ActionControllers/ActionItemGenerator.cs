//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Workflows;
using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Orchestrators;

namespace Epsitec.Cresus.Core.Controllers.ActionControllers
{
	/// <summary>
	/// The <c>ActionItemGenerator</c> class produces <see cref="ActionItemLayout"/> instances
	/// based on a collection of <see cref="TileDataItem"/> items.
	/// </summary>
	public sealed class ActionItemGenerator
	{
		public ActionItemGenerator(DataViewOrchestrator orchestrator, BusinessContext context, IEnumerable<TileDataItem> items)
		{
			this.orchestrator = orchestrator;
			this.context      = context;
			
			this.tileDataItems = new List<TileDataItem> (items);
			this.tileDataItemLookupTable = new Dictionary<string, TileDataItem> (this.tileDataItems.ToDictionary (x => x.Name));
			this.layouts = new List<ActionItemLayout> ();
		}


		public IEnumerable<ActionItemLayout> GenerateLayouts()
		{
			this.layouts.Clear ();

			foreach (var item in this.tileDataItems)
			{
				this.GenerateActionItems (item);
			}

			return this.layouts;
		}


		private void GenerateActionItems(TileDataItem item)
		{
			this.GenerateCreateActionItem (item);
			this.GenerateDeleteActionItem (item);

			if (item.EntityMarshaler != null)
			{
				var entity      = item.EntityMarshaler.GetValue<AbstractEntity> ();
				var defaultType = item.EntityMarshaler.MarshaledType;
				
				this.GenerateEntityActionItems (item, defaultType, entity);
				this.GenerateWorkflowActionItems (item, entity);
			}
		}

		private void GenerateCreateActionItem(TileDataItem item)
		{
			if (item.HideAddButton == false && item.AddNewItem != null)
			{
				if (ActionButton.HasIcon)
				{
					this.CreateLayout (item, new ActionItem (ActionClasses.Create, item.AddNewItem, ActionItem.GetIcon ("Action.Create")));
				}
				else
				{
					this.CreateLayout (item, new ActionItem (ActionClasses.Create, item.AddNewItem));
				}
			}
		}

		private void GenerateDeleteActionItem(TileDataItem item)
		{
			if (item.HideRemoveButton == false && item.DeleteItem != null)
			{
				if (ActionButton.HasIcon)
				{
					this.CreateLayout (item, new ActionItem (ActionClasses.Delete, item.DeleteItem, ActionItem.GetIcon ("Action.Delete")));
				}
				else
				{
					this.CreateLayout (item, new ActionItem (ActionClasses.Delete, item.DeleteItem));
				}
			}
		}

		/// <summary>
		/// Generates the <see cref="ActionLayoutItem"/> items for the actions that can be
		/// executed on the specified entity.
		/// </summary>
		/// <param name="item">The tile data item.</param>
		/// <param name="defaultType">The default type.</param>
		/// <param name="entity">The entity.</param>
		private void GenerateEntityActionItems(TileDataItem item, System.Type defaultType, AbstractEntity entity)
		{
			var entityType    = entity == null ? defaultType : entity.GetType ();
			var entityActions = ActionDispatcher.GetActionInfos (entityType);

			foreach (var actionInfo in entityActions)
			{
				this.GenerateEntityActionItemForActionInfo (item, entity, actionInfo);
			}
		}

		/// <summary>
		/// Generates one <see cref="ActionLayoutItem"/> item for a given <see cref="ActionInfo"/>.
		/// </summary>
		/// <param name="item">The tile data item.</param>
		/// <param name="entity">The entity.</param>
		/// <param name="info">The action information (caption, callback and weight).</param>
		private void GenerateEntityActionItemForActionInfo(TileDataItem item, AbstractEntity entity, ActionInfo info)
		{
			this.CreateLayout (item, new ActionItem (info.ActionClass, () => info.ExecuteAction (entity), info.CaptionId, weight: info.Weight));
		}

		/// <summary>
		/// Generates the <see cref="ActionLayoutItem"/> items for the workflows hosted by
		/// the specified entity.
		/// </summary>
		/// <param name="item">The tile data item.</param>
		/// <param name="entity">The entity.</param>
		private void GenerateWorkflowActionItems(TileDataItem item, AbstractEntity entity)
		{
			if (item.IsCompact || item.AutoGroup)
			{
				return;
			}
			
			foreach (var transition in WorkflowController.GetEnabledTransitions (this.context, entity as IWorkflowHost))
			{
				this.GenerateWorkflowTransitionActionItem (item, transition);
			}
		}

		private void GenerateWorkflowTransitionActionItem(TileDataItem item, WorkflowTransition transition)
		{
			var lines  = WorkflowActionCompiler.GetSourceLines (transition.Edge.TransitionActions);
			var result = WorkflowActionCompiler.Validate (lines);
			var action = result.WorkflowAction;

			if (action != null && !action.IsInvalid)
			{
				if (this.CreateWorflowTransitionActionItem (transition, action))
				{
					return;
				}
			}

			var label       = transition.Edge.GetLabel (LabelDetailLevel.Compact);
			var description = transition.Edge.GetLabel (LabelDetailLevel.Detailed);

			this.CreateLayout (item, new ActionItem (ActionClasses.NextStep, transition.CreateAction (this.orchestrator.Navigator), label, description));
		}

		private bool CreateWorflowTransitionActionItem(WorkflowTransition transition, WorkflowAction action)
		{
			var verb      = action.ActionVerbs.First ();
			var attribute = verb.Attribute;
			var command   = verb.Name;

			if (attribute != null)
			{
				command = attribute.PublishedName ?? command;
				var collectionItemName = EntityInfo.GetEntityName (attribute.CollectionItemType);

				if (collectionItemName != null)
				{
					var affairTile = this.FindTileDataItem (collectionItemName);

					if (affairTile != null)
					{
						var label       = transition.Edge.GetLabel (LabelDetailLevel.Compact);
						var description = transition.Edge.GetLabel (LabelDetailLevel.Detailed);

						this.CreateLayout (affairTile, new ActionItem (ActionClasses.Create, transition.CreateAction (this.orchestrator.Navigator), label, description));

						return true;
					}
				}
			}

			return false;
		}

		private void CreateLayout(TileDataItem item, ActionItem actionItem)
		{
			var layout = ActionItemLayout.Create (item, actionItem);

			if (layout != null)
			{
				this.layouts.Add (layout);
			}
		}

		private TileDataItem FindTileDataItem(string name)
		{
			TileDataItem item;

			if (this.tileDataItemLookupTable.TryGetValue (name, out item) ||
				this.tileDataItemLookupTable.TryGetValue (name + ".0", out item))
			{
				return item;
			}
			else
			{
				return null;
			}
		}

		private readonly DataViewOrchestrator				orchestrator;
		private readonly BusinessContext					context;
		private readonly List<TileDataItem>					tileDataItems;
		private readonly Dictionary<string, TileDataItem>	tileDataItemLookupTable;
		private readonly List<ActionItemLayout>				layouts;
	}
}
