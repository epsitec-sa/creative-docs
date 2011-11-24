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
		}


		public IEnumerable<ActionItemLayout> GenerateLayouts()
		{
			foreach (var item in this.tileDataItems)
			{
				foreach (var actionItem in this.GenerateActionItems (item))
				{
					yield return ActionItemLayout.Create (item, actionItem);
				}
			}
		}


		public IEnumerable<ActionItem> GenerateActionItems(TileDataItem item)
		{
			if ((item.HideAddButton == false) &&
				(item.AddNewItem != null))
			{
				yield return new ActionItem (ActionClasses.Create, item.AddNewItem);
			}

			if ((item.HideRemoveButton == false) &&
				(item.DeleteItem != null))
			{
				yield return new ActionItem (ActionClasses.Delete, item.DeleteItem);
			}

			if (item.EntityMarshaler != null)
			{
				var entity        = item.EntityMarshaler.GetValue<AbstractEntity> ();
				var entityType    = entity == null ? item.EntityMarshaler.MarshaledType : entity.GetType ();
				var entityActions = ActionDispatcher.GetActionInfos (entityType);

				foreach (var actionInfo in entityActions)
				{
					var info    = actionInfo;
					var caption = TextFormatter.GetCurrentCultureCaption (actionInfo.CaptionId);

					yield return new ActionItem (info.ActionClass, () => info.ExecuteAction (entity), caption, weight: info.Weight);
				}

				foreach (var transition in WorkflowController.GetEnabledTransitions (this.context, entity as IWorkflowHost))
				{
					var action  = Epsitec.Cresus.Core.Workflows.WorkflowAction.Parse (transition.Edge.TransitionActions);
					var source  = action.SourceLines.FirstOrDefault () ?? "";
					var command = source.Split ('.').Skip (1).FirstOrDefault () ?? "";

					System.Diagnostics.Debug.WriteLine ("Workflow command : " + command);
				}
			}
		}


		private readonly BusinessContext context;
		private readonly List<TileDataItem> tileDataItems;
		private readonly Dictionary<string, TileDataItem> tileDataItemLookupTable;
	}
}
