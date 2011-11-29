//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Entities;

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
				root.IsFence = true;
				root.PaintForeground += this.HandleWindowRootPaintForeground;
			}
		}


		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}


		public void Refresh(IEnumerable<TileDataItem> items)
		{
			ActionItemGenerator generator = new ActionItemGenerator (this.BusinessContext, items);

			this.layouts.Clear ();
			this.layouts.AddRange (generator.GenerateLayouts ());

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
					graphics.AddText (rect.X, rect.Y, rect.Width, rect.Height, x.Item.Label.ToSimpleText (), ActionItemLayout.DefaultFont, ActionItemLayout.DefaultFontSize, ContentAlignment.MiddleCenter);
				});

			graphics.Color = Epsitec.Common.Drawing.Color.FromName ("Cyan");
			graphics.RenderSolid ();
		}


		private Epsitec.Common.Widgets.Widget GetWindowRoot()
		{
			return this.Orchestrator.DataViewController.Root;
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
