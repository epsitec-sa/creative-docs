//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

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

			System.Diagnostics.Debug.Assert (this.Orchestrator.DataViewController.Root != null);
			System.Diagnostics.Debug.Assert (this.Orchestrator.DataViewController.Root.Window != null);

			this.viewRoot   = this.Orchestrator.DataViewController.Root;
			this.windowRoot = this.viewRoot.Window.Root;

			this.viewRoot.IsFence = true;

			this.frameRoot = new FrameBox
			{
				Parent     = this.viewRoot,
				Anchor     = AnchorStyles.All,
				Visibility = false,
			};

			this.frameRoot.Pressed  += new Common.Support.EventHandler<MessageEventArgs> (this.HandleFrameRoot_Pressed);
			this.frameRoot.Released += new Common.Support.EventHandler<MessageEventArgs> (this.HandleFrameRoot_Released);
			
			this.windowRoot.AltModifierChanged += this.HandleWindowRootAltModifierChanged;
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
					this.UpdateFrameRoot ();
				}
			}
		}


		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}


		public void Refresh(IEnumerable<TileDataItem> items)
		{
			var generator = new ActionItemGenerator (this.Orchestrator, this.BusinessContext, items);

			this.layouts.Clear ();
			this.layouts.AddRange (generator.GenerateLayouts ());

			ActionItemLayout.UpdateLayout (this.layouts);
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
			this.UpdateFrameRoot ();
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
				layout.Item.ExecuteAction ();
				this.ShowMode = ActionViewControllerMode.Hide;
			};

			var tooltip = FormattedText.Empty;

			if (!layout.Item.Description.IsNullOrEmpty)
			{
				tooltip = layout.Item.Description;
			}
			else if (layout.IsTextTooLarge)
			{
				tooltip = layout.Item.Label;
			}

			if (!tooltip.IsNullOrEmpty && (tooltip != layout.Item.Label || layout.IsTextTooLarge))
			{
				ToolTip.Default.SetToolTip (button, tooltip);
			}
		}


		private void UpdateFrameRoot()
		{
			switch (this.showMode)
			{
				case ActionViewControllerMode.Hide:
				this.SetFrameRootButtonsAlpha (0.0);
					break;

				case ActionViewControllerMode.Dimmed:
				this.SetFrameRootButtonsAlpha (0.2);
					break;

				case ActionViewControllerMode.Full:
				this.SetFrameRootButtonsAlpha (1.0);
					break;
			}
		}

		private void SetFrameRootButtonsAlpha(double alpha)
		{
			if (alpha == 0.0)
			{
				this.frameRoot.Visibility = false;
			}
			else if (alpha == 1.0)
			{
				this.frameRoot.Visibility = true;
				this.frameRoot.SetFrozen (false);
				//?this.frameRoot.BackColor = new Color (0.6, 1.0, 1.0, 1.0);  // voile blanc laissant le texte bien lisible
				this.frameRoot.BackColor = new Color (0.6, 0.98, 0.98, 0.98);  // voile légèrement gris laissant le texte bien lisible
			}
			else
			{
				this.frameRoot.Visibility = true;
				this.frameRoot.SetFrozen (true);
				this.frameRoot.BackColor = Color.Empty;
			}

			if (alpha > 0.0)
			{
				foreach (var widget in this.frameRoot.Children)
				{
					var button = widget as ActionButton;

					if (button != null)
					{
						button.Alpha = alpha;
					}
				}
			}
		}

		private void HandleFrameRoot_Pressed(object sender, MessageEventArgs e)
		{
			if (this.showMode == ActionViewControllerMode.Full)
			{
				this.ShowMode = ActionViewControllerMode.Hide;
			}

			e.Message.Swallowed = true;
		}

		private void HandleFrameRoot_Released(object sender, MessageEventArgs e)
		{
			e.Message.Swallowed = true;
		}

		private void HandleWindowRootAltModifierChanged(object sender, MessageEventArgs e)
		{
			if (e.Message.IsAltPressed)
			{
				this.ShowMode = ActionViewControllerMode.Full;
			}
			else
			{
				this.ShowMode = ActionViewControllerMode.Hide;
			}
		}


		protected override void Dispose(bool disposing)
		{
			this.windowRoot.AltModifierChanged -= this.HandleWindowRootAltModifierChanged;
			
			this.frameRoot.Pressed  -= new Common.Support.EventHandler<MessageEventArgs> (this.HandleFrameRoot_Pressed);
			this.frameRoot.Released -= new Common.Support.EventHandler<MessageEventArgs> (this.HandleFrameRoot_Released);

			this.frameRoot.Children.Clear ();
			
			this.viewRoot.Children.Remove (this.frameRoot);
			
			base.Dispose (disposing);
		}


		private readonly List<ActionItemLayout>	layouts;
		private readonly Widget					viewRoot;
		private readonly FrameBox				frameRoot;
		private readonly WindowRoot				windowRoot;

		private ActionViewControllerMode		showMode;
	}
}
