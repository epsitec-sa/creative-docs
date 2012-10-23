//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Orchestrators;
using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;

namespace Epsitec.Cresus.Core.Controllers
{
	public class ActionViewController : Library.ViewControllerComponent<ActionViewController>
	{
		public ActionViewController(DataViewOrchestrator orchestrator)
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
				Anchor     = AnchorStyles.All,
				Visibility = false,
			};

			this.frameRoot.Pressed  += this.HandleFrameRootPressed;
			this.frameRoot.Released += this.HandleFrameRootReleased;

			this.windowRoot.AltModifierChanged += this.HandleWindowRootAltModifierChanged;

			if (ActionButton.SmoothTransition && !ActionButton.HasIcon)
			{
				this.timer = new Timer ();
				this.timer.AutoRepeat = 1.0 / ActionViewController.TimerFps;
				this.timer.TimeElapsed += this.HandleTimer_TimeElapsed;
				this.timer.Start ();
			}
		}


		public ActionViewControllerMode			ShowMode
		{
			get
			{
				return this.showMode;
			}
			set
			{
				if (this.showMode != value)
				{
					this.oldShowMode = this.showMode;
					this.showMode = value;
					this.UpdateFrameRoot ();
				}
			}
		}


		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}


		public void Refresh(TileContainerController controller, IEnumerable<TileDataItem> items)
		{
			var serialId  = controller.SerialId;
			var generator = new ActionItemGenerator (this.Orchestrator, this.BusinessContext, items, serialId);

			this.layouts.RemoveAll (x => x.SerialId == serialId);
			this.layouts.AddRange (generator.GenerateLayouts ());

			ActionItemLayout.UpdateLayout (this.layouts);
			
			this.RemoveDuplicates ();
			this.RefreshUI ();
		}

		
		internal void NotifyRemoval(TileContainerController controller)
		{
			var serialId  = controller.SerialId;
			
			this.layouts.RemoveAll (x => x.SerialId == serialId);
			
			this.RefreshUI ();
		}

		
		private void RemoveDuplicates()
		{
			this.layouts.RemoveAll (x => x.IsDuplicate);
		}

		private void RefreshUI()
		{
			this.frameRoot.Children.Clear ();
			this.layouts.ForEach (x => this.CreateLayoutUI (x));
			this.UpdateFrameRoot ();
		}

		private void CreateLayoutUI(ActionItemLayout layout)
		{
			Point bottomLeft;
			Size size;

			if (ActionButton.HasPastelColor)
			{
				bottomLeft = new Point (layout.Bounds.Left, layout.Bounds.Bottom);
				size       = new Size (layout.Bounds.Width+1, layout.Bounds.Height+1);
			}
			else
			{
				bottomLeft = new Point (layout.Bounds.Left+1, layout.Bounds.Bottom+1);
				size       = new Size (layout.Bounds.Width-1, layout.Bounds.Height-1);
			}

			var button = new ActionButton
			{
				Parent               = this.frameRoot,
				ActionClasses        = layout.Item.ActionClass.Class,
				ActionBackgroudColor = layout.Item.ActionClass.Color,
				FormattedText        = layout.Item.Label,
				IsIcon               = ActionItem.IsIcon (layout.Item.Label),
				Anchor               = AnchorStyles.Left | AnchorStyles.Bottom,
				Margins              = new Margins (bottomLeft.X, 0, 0, bottomLeft.Y),
				PreferredSize        = size,
			};

			button.Clicked += delegate
			{
				layout.Item.ExecuteAction ();
				this.ShowMode = ActionViewControllerMode.Hide;
			};

			var tooltip = FormattedText.Empty;

			if (!layout.Item.Description.IsNullOrEmpty ())
			{
				tooltip = layout.Item.Description;
			}
			else if (layout.IsTextTooLarge)
			{
				tooltip = layout.Item.Label;
			}

			if (!tooltip.IsNullOrEmpty () && (tooltip != layout.Item.Label || layout.IsTextTooLarge))
			{
				ToolTip.Default.SetToolTip (button, tooltip);
			}
		}


		private void UpdateFrameRoot()
		{
			//	Make sure the frame root is a child of the data view controller's root. Every
			//	time the ViewLayoutController refreshes its columns, it removes all children,
			//	including the frame root. That's why we need to reparent it every time:

			this.frameRoot.Parent = this.viewRoot;

			if (ActionButton.SmoothTransition && !ActionButton.HasIcon)
			{
				switch (this.showMode)
				{
					case ActionViewControllerMode.Hide:
						if (this.oldShowMode == ActionViewControllerMode.Full)
						{
							this.ImmediateFrameRootButtonsAlpha (0.0);
						}
						else
						{
							this.finalAlpha = 0.0;
							this.stepAlpha = System.Math.Abs (this.currentAlpha-this.finalAlpha) / ActionViewController.TimerFps / ActionViewController.TimerTransitionDelayOff;
						}
						break;

					case ActionViewControllerMode.Dimmed:
						this.finalAlpha = 0.6;
						this.stepAlpha = System.Math.Abs (this.currentAlpha-this.finalAlpha) / ActionViewController.TimerFps / ActionViewController.TimerTransitionDelayOn;
						break;

					case ActionViewControllerMode.Full:
						this.ImmediateFrameRootButtonsAlpha (1.0);
						break;
				}
			}
			else
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

			this.frameRoot.Invalidate ();
		}

		private void ImmediateFrameRootButtonsAlpha(double alpha)
		{
			if (this.timer.IsDisposed)
			{
				return;
			}

			this.timer.Stop ();

			this.finalAlpha = alpha;
			this.currentAlpha = alpha;
			this.stepAlpha = 1.0;
			this.SetFrameRootButtonsAlpha (this.currentAlpha);
			
			this.timer.Start ();
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


		private void HandleTimer_TimeElapsed(object sender)
		{
			if (this.currentAlpha != this.finalAlpha)
			{
				if (this.currentAlpha < this.finalAlpha)
				{
					this.currentAlpha = System.Math.Min (this.currentAlpha+this.stepAlpha, this.finalAlpha);
				}
				else
				{
					this.currentAlpha = System.Math.Max (this.currentAlpha-this.stepAlpha, this.finalAlpha);
				}

				this.SetFrameRootButtonsAlpha (this.currentAlpha);
			}
		}

		private void HandleFrameRootPressed(object sender, MessageEventArgs e)
		{
			if (this.showMode == ActionViewControllerMode.Full)
			{
				this.ShowMode = ActionViewControllerMode.Hide;
			}

			e.Message.Swallowed = true;
		}

		private void HandleFrameRootReleased(object sender, MessageEventArgs e)
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
			if (this.timer != null)
			{
				this.timer.TimeElapsed -= this.HandleTimer_TimeElapsed;
				this.timer.Dispose ();
			}

			this.windowRoot.AltModifierChanged -= this.HandleWindowRootAltModifierChanged;

			this.frameRoot.Pressed  -= this.HandleFrameRootPressed;
			this.frameRoot.Released -= this.HandleFrameRootReleased;

			this.frameRoot.Children.Clear ();
			
			this.viewRoot.Children.Remove (this.frameRoot);
			
			base.Dispose (disposing);
		}


		private static readonly double			TimerFps				= 10.0;
		private static readonly double			TimerTransitionDelayOn	=  0.5;
		private static readonly double			TimerTransitionDelayOff	=  0.3;

		private readonly List<ActionItemLayout>	layouts;
		private readonly Widget					viewRoot;
		private readonly FrameBox				frameRoot;
		private readonly WindowRoot				windowRoot;
		private readonly Timer					timer;

		private ActionViewControllerMode		showMode;
		private ActionViewControllerMode		oldShowMode;
		private double							finalAlpha;
		private double							currentAlpha;
		private double							stepAlpha;
	}
}
