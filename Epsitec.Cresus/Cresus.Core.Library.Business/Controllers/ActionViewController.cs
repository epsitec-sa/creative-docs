//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	public class ActionViewController : Library.ViewControllerComponent<ActionViewController>
	{
		public ActionViewController(Orchestrators.DataViewOrchestrator orchestrator)
			: base (orchestrator)
		{
			this.layouts = new List<ActionItemLayout> ();

			var root = this.GetWindowRoot ();

			if (root != null)
			{
				root.PaintForeground += this.HandleWindowRootPaintForeground;
			}
		}


		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}


		public void Refresh(IEnumerable<TileDataItem> items)
		{
			this.layouts.Clear ();

			foreach (var item in items)
			{
				foreach (var actionItem in this.GenerateActionItems (item))
				{
					this.layouts.Add (ActionItemLayout.Create (item, actionItem));
				}
			}

			ActionItemLayout.UpdateLayout (this.layouts);
			this.RemoveDuplicates ();
		}

		private void RemoveDuplicates()
		{
			this.layouts.RemoveAll (x => x.IsDuplicate);
		}

		private void HandleWindowRootPaintForeground(object sender, Epsitec.Common.Widgets.PaintEventArgs e)
		{
			this.ExperimentalPaintOverlay (e.Graphics);
		}

		private void ExperimentalPaintOverlay(Graphics graphics)
		{
			this.layouts.ForEach (
				x =>
				{
					var rect = Rectangle.Deflate (x.Bounds, 0.5, 0.5);
					graphics.AddRectangle (rect);
					graphics.AddText (rect.X, rect.Y, rect.Width, rect.Height, x.Item.Label.ToString (), ActionItemLayout.DefaultFont, ActionItemLayout.DefaultFontSize, ContentAlignment.MiddleCenter);
				});

			graphics.Color = Epsitec.Common.Drawing.Color.FromName ("Cyan");
			graphics.RenderSolid ();
		}

		private IEnumerable<ActionItem> GenerateActionItems(TileDataItem item)
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
			}
		}

		private Epsitec.Common.Widgets.WindowRoot GetWindowRoot()
		{
			var window = this.Orchestrator.Host.Window;
			return window == null ? null : window.Root;
		}
		
		protected override void Dispose(bool disposing)
		{
			var root = this.GetWindowRoot ();			
			
			if (root != null)
			{
				root.PaintForeground -= this.HandleWindowRootPaintForeground;
			}

			base.Dispose (disposing);
		}

		private readonly List<ActionItemLayout>	layouts;
	}
}
