//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;

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

			this.layouts.RemoveAll (x => x.IsDuplicate);

			this.Orchestrator.Host.Window.Root.PaintForeground
				+= (sender, e)
					=>
					{
						this.layouts.ForEach (
							x =>
							{
								var rect = Rectangle.Deflate (x.Bounds, 0.5, 0.5);
								e.Graphics.AddRectangle (rect);
								e.Graphics.AddText (rect.X, rect.Y, rect.Width, rect.Height, x.Item.Label.ToString (), ActionItemLayout.DefaultFont, ActionItemLayout.DefaultFontSize, ContentAlignment.MiddleCenter);
							});

						e.Graphics.Color = Epsitec.Common.Drawing.Color.FromName ("Cyan");
						e.Graphics.RenderSolid ();
					};

		}


		private IEnumerable<ActionItem> GenerateActionItems(TileDataItem item)
		{
			if ((item.HideAddButton == false) &&
				(item.AddNewItem != null))
			{
				yield return new ActionItem (ActionClasses.Create, item.AddNewItem, "Ajouter");
			}

			if ((item.HideRemoveButton == false) &&
				(item.DeleteItem != null))
			{
				yield return new ActionItem (ActionClasses.Delete, item.DeleteItem, "Supprimer");
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose (disposing);
		}

		private readonly List<ActionItemLayout>	layouts;
	}
}
