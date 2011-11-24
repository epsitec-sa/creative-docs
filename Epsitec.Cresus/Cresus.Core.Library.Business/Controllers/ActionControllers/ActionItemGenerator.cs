//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.ActionControllers
{
	class ActionItemGenerator
	{
		public ActionItemGenerator(BusinessContext context, IEnumerable<TileDataItem> items)
		{
			this.context = context;
			
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
				var entity = item.EntityMarshaler.GetValue<AbstractEntity> ();
				
				this.GenerateEntityActionItems (item, entity);
				this.GenerateWorkflowActionItems (item, entity);
			}
		}

		private void GenerateCreateActionItem(TileDataItem item)
		{
			if ((item.HideAddButton == false) &&
					(item.AddNewItem != null))
			{
				this.CreateLayout (item, new ActionItem (ActionClasses.Create, item.AddNewItem));
			}
		}

		private void GenerateDeleteActionItem(TileDataItem item)
		{
			if ((item.HideRemoveButton == false) &&
					(item.DeleteItem != null))
			{
				this.CreateLayout (item, new ActionItem (ActionClasses.Delete, item.DeleteItem));
			}
		}

		private void GenerateEntityActionItems(TileDataItem item, AbstractEntity entity)
		{
			var entityType    = entity == null ? item.EntityMarshaler.MarshaledType : entity.GetType ();
			var entityActions = ActionDispatcher.GetActionInfos (entityType);

			foreach (var actionInfo in entityActions)
			{
				var info    = actionInfo;
				var caption = TextFormatter.GetCurrentCultureCaption (actionInfo.CaptionId);

				this.CreateLayout (item, new ActionItem (info.ActionClass, () => info.ExecuteAction (entity), caption, weight: info.Weight));
			}
		}

		private void GenerateWorkflowActionItems(TileDataItem item, AbstractEntity entity)
		{
			if (item.IsCompact || item.AutoGroup)
			{
				return;
			}
			
			foreach (var transition in WorkflowController.GetEnabledTransitions (this.context, entity as IWorkflowHost))
			{
				var action  = Epsitec.Cresus.Core.Workflows.WorkflowAction.Parse (transition.Edge.TransitionActions);
				var source  = action.SourceLines.FirstOrDefault () ?? "";
				var command = source.Split ('.').Skip (1).FirstOrDefault () ?? "";

				System.Diagnostics.Debug.WriteLine ("Workflow command : " + command);
			}
		}

		private void CreateLayout(TileDataItem item, ActionItem actionItem)
		{
			this.layouts.Add (ActionItemLayout.Create (item, actionItem));
		}


		private readonly BusinessContext context;
		private readonly List<TileDataItem> tileDataItems;
		private readonly Dictionary<string, TileDataItem> tileDataItemLookupTable;
		private readonly List<ActionItemLayout> layouts;
	}
}
