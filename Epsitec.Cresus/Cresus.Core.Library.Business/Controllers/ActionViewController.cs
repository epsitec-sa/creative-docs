//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.Core.Controllers
{
	public class ActionViewController : Library.ViewControllerComponent<ActionViewController>
	{
		public ActionViewController(Orchestrators.DataViewOrchestrator orchestrator)
			: base (orchestrator)
		{
			this.showMode = ActionViewControllerMode.Hide;
			this.layouts = new List<ActionItemLayout> ();

			this.windowRoot = this.Orchestrator.DataViewController.Root;
			System.Diagnostics.Debug.Assert (this.windowRoot != null);

			this.windowRoot.IsFence = true;
			this.windowRoot.PaintForeground += this.HandleWindowRootPaintForeground;
			this.windowRoot.PreProcessing   += this.HandleWindowRootPreProcessing;

			this.frameRoot = new FrameBox
			{
				Parent     = this.windowRoot,
				Anchor     = AnchorStyles.All,
				Visibility = false,
			};

			//?this.ShowMode = ActionViewControllerMode.Full;
		}


		public ActionViewControllerMode ShowMode
		{
			get
			{
				return this.showMode;
			}
			set
			{
				if (this.showMode != value)
				{
					this.showMode = value;
					this.Update ();
				}
			}
		}


		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}


		public void Refresh(IEnumerable<TileDataItem> items)
		{
			var generator = new ActionItemGenerator (this.BusinessContext, items);

			this.layouts.Clear ();
			this.layouts.AddRange (generator.GenerateLayouts ());

			ActionItemLayout.UpdateLayout (this.layouts, 8);
			this.RemoveDuplicates ();

			this.CreateUI ();
		}

		private void RemoveDuplicates()
		{
			this.layouts.RemoveAll (x => x.IsDuplicate);
		}


		private void CreateUI()
		{
			this.frameRoot.Children.Clear ();
			this.layouts.ForEach (x => this.CreateLayoutUI (x));
			this.UpdateButtons ();
		}

		private void CreateLayoutUI(ActionItemLayout layout)
		{
			var button = new ActionButton
			{
				Parent               = this.frameRoot,
				ActionClasses        = layout.Item.ActionClass.Class,
				ActionBackgroudColor = layout.Item.ActionClass.Color,
				FormattedText        = layout.Item.Label,
				IsIcon               = ActionItem.IsIcon (layout.Item.Label),
				Anchor               = AnchorStyles.Left | AnchorStyles.Bottom,
				Margins              = new Margins (layout.Bounds.Left, 0, 0, layout.Bounds.Bottom),
				PreferredSize        = new Size (layout.Bounds.Width+1, layout.Bounds.Height+1),
			};

			button.Clicked += delegate
			{
				layout.Item.Action ();
			};

			if (!layout.Item.Description.IsNullOrEmpty)
			{
				ToolTip.Default.SetToolTip (button, layout.Item.Description);
			}
		}


		private void Update()
		{
			if (this.showMode == ActionViewControllerMode.Hide)
			{
				this.frameRoot.Visibility = false;
			}
			else if (this.showMode == ActionViewControllerMode.Dimmed)
			{
				this.frameRoot.Visibility = true;
				this.frameRoot.BackColor = Color.Empty;
				// TODO: Rendre this.frameRoot insensible aux événements !

				this.UpdateButtons ();
			}
			else if (this.showMode == ActionViewControllerMode.Full)
			{
				this.frameRoot.Visibility = true;
				this.frameRoot.BackColor = new Color (0.6, 1.0, 1.0, 1.0);  // voile blanc laissant le texte bien lisible
				// TODO: Rendre this.frameRoot sensible aux événements !

				this.UpdateButtons ();
			}
		}

		private void UpdateButtons()
		{
			foreach (var widget in this.frameRoot.Children)
			{
				var button = widget as ActionButton;

				if (button != null)
				{
					button.Alpha = (this.showMode == ActionViewControllerMode.Dimmed) ? 0.3 : 1.0;
				}
			}
		}


		private void HandleWindowRootPaintForeground(object sender, PaintEventArgs e)
		{
			this.ExperimentalPaintOverlay (e.Graphics);
		}

		private void HandleWindowRootPreProcessing(object sender, MessageEventArgs e)
		{
			//?System.Diagnostics.Debug.WriteLine (e.Message.ToString ());
		}

		private void ExperimentalPaintOverlay(Graphics graphics)
		{
#if false
			this.layouts.ForEach (
				x =>
				{
					var rect = Rectangle.Deflate (x.Bounds, 0.5, 0.5);
					graphics.AddRectangle (rect);
					graphics.AddText (rect.X, rect.Y, rect.Width, rect.Height, x.Item.Label.ToSimpleText (), ActionItemLayout.DefaultFont, ActionItemLayout.DefaultFontSize, ContentAlignment.MiddleCenter);
				});

			graphics.Color = Epsitec.Common.Drawing.Color.FromName ("Blue");
			graphics.RenderSolid ();
#endif
		}


		protected override void Dispose(bool disposing)
		{
			this.frameRoot.Children.Clear ();
			this.windowRoot.Children.Remove (this.frameRoot);

			this.windowRoot.PaintForeground -= this.HandleWindowRootPaintForeground;
			this.windowRoot.PreProcessing   -= this.HandleWindowRootPreProcessing;

			base.Dispose (disposing);
		}


		private readonly List<ActionItemLayout>		layouts;
		private readonly Widget						windowRoot;
		private readonly FrameBox					frameRoot;

		private ActionViewControllerMode			showMode;
	}
}
