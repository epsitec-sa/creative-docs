//	Copyright � 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.UI;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (PanelStack))]

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>PanelStack</c> class manages a stack of <c>Panel</c> instances
	/// (used when the panels are switched to <c>PanelMode.Edition</c>).
	/// </summary>
	public class PanelStack : Widgets.AbstractGroup
	{
		public PanelStack()
		{
			this.editPanels = new Stack<Panel> ();
			this.miniPanels = new List<MiniPanel> ();
			this.mask = new PanelMask (this);
			this.mask.Hide ();
			this.mask.MaskPressed += delegate (object sender)
			{
				this.EndEdition ();
			};
		}

		public PanelStack(Widgets.Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}

		public PanelMask Mask
		{
			get
			{
				return this.mask;
			}
		}
		
		public void StartEdition(Panel panel, string focusWidgetName)
		{
			this.StartEdition (panel);
			
			if (! string.IsNullOrEmpty (focusWidgetName))
			{
				Widgets.Widget focus = panel.FindChild (focusWidgetName);
				
				if (focus != null)
				{
					focus.SetFocusOnTabWidget ();
				}
			}
		}

		public void StartEdition(Panel panel)
		{
			Panel oldValue = this.editPanels.Count == 0 ? null : this.editPanels.Peek ();
			
			this.editPanels.Push (panel);

			panel.Owner.BoundsChanged += this.HandlePanelOwnerBoundsChanged;
			panel.SetParent (this);
			panel.ZOrder = 0;
			
			this.UpdateEditPanelBounds (panel);
			
			panel.Show ();
			panel.SetFocusOnTabWidget ();

			Panel newValue = this.editPanels.Count == 0 ? null : this.editPanels.Peek ();

			this.OnEditPanelChanged (oldValue, newValue);
		}

		public void EndEdition()
		{
			this.EndEdition (this.editPanels.Peek ());
		}

		public bool EndEdition(Panel panel)
		{
			if (this.editPanels.Contains (panel))
			{
				Panel oldValue = this.editPanels.Count == 0 ? null : this.editPanels.Peek ();
				Panel item;
				
				do
				{
					item = this.editPanels.Pop ();

					item.Hide ();
					
					item.Owner.BoundsChanged -= this.HandlePanelOwnerBoundsChanged;
				}
				while (item != panel);

				this.UpdateMask ();
				panel.Owner.SetFocusOnTabWidget ();

				Panel newValue = this.editPanels.Count == 0 ? null : this.editPanels.Peek ();

				this.OnEditPanelChanged (oldValue, newValue);
				return true;
			}
			else
			{
				return false;
			}
		}

		internal void Add(MiniPanel panel)
		{
			this.miniPanels.Add (panel);

			panel.ApertureChanged += this.HandleMiniPanelApertureChanged;
			panel.SetParent (this);
			panel.ZOrder = 0;

			this.UpdateMiniPanelBounds (panel);

			panel.Show ();
			panel.SetFocusOnTabWidget ();
		}

		internal void Remove(MiniPanel panel)
		{
			if (this.miniPanels.Contains (panel))
			{
				this.miniPanels.Remove (panel);

				panel.ApertureChanged -= this.HandleMiniPanelApertureChanged;
			}
		}

		public static PanelStack GetPanelStack(Widgets.Visual widget)
		{
			while (widget != null)
			{
				PanelStack stack = widget as PanelStack;
				
				if (stack != null)
				{
					return stack;
				}

				Widgets.ILogicalTree logicalTree = widget as Widgets.ILogicalTree;

				widget = logicalTree == null ? widget.Parent : logicalTree.Parent;
			}
			
			return null;
		}

		protected override void SetBoundsOverride(Drawing.Rectangle oldRect, Drawing.Rectangle newRect)
		{
			base.SetBoundsOverride (oldRect, newRect);

			this.mask.SetManualBounds (new Drawing.Rectangle (0, 0, newRect.Width, newRect.Height));
		}

		private void HandlePanelOwnerBoundsChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			foreach (Panel panel in this.editPanels)
			{
				if (panel.Owner == sender)
				{
					this.UpdateEditPanelBounds (panel);
					break;
				}
			}
		}

		private void HandleMiniPanelApertureChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			foreach (MiniPanel panel in this.miniPanels)
			{
				if (panel == sender)
				{
					this.UpdateMiniPanelBounds (panel);
					break;
				}
			}
		}

		private void UpdateEditPanelBounds(Panel panel)
		{
			System.Diagnostics.Debug.Assert (this.editPanels.Count > 0);
			System.Diagnostics.Debug.Assert (this.editPanels.Contains (panel));
			
			Panel owner = panel.Owner;
			
			Drawing.Rectangle stackBounds = this.Client.Bounds;
			Drawing.Rectangle panelBounds = new Drawing.Rectangle (0, 0, System.Math.Max (panel.PreferredWidth, panel.MinWidth), System.Math.Max (panel.PreferredHeight, panel.MinHeight));
			Drawing.Rectangle ownerBounds = Widgets.Helpers.VisualTree.MapVisualToAncestor (owner, this, owner.Client.Bounds);

			stackBounds.Deflate (this.Padding);
			panelBounds.Offset (ownerBounds.Center-panelBounds.Center);

			panel.Dock   = Widgets.DockStyle.None;
			panel.Anchor = Widgets.AnchorStyles.None;
			
			panel.SetManualBounds (Drawing.Rectangle.Constrain (panelBounds, stackBounds));

			if (panel == this.editPanels.Peek ())
			{
				this.UpdateMask ();
			}
		}

		private void UpdateMiniPanelBounds(MiniPanel panel)
		{
			System.Diagnostics.Debug.Assert (this.miniPanels.Count > 0);
			System.Diagnostics.Debug.Assert (this.miniPanels.Contains (panel));

			Drawing.Rectangle stackBounds = this.Client.Bounds;
			Drawing.Rectangle panelBounds = new Drawing.Rectangle (0, 0, System.Math.Max (panel.PreferredWidth, panel.MinWidth), System.Math.Max (panel.PreferredHeight, panel.MinHeight));
			Drawing.Rectangle ownerBounds = panel.Aperture;

			panelBounds.Offset (ownerBounds.Left, ownerBounds.Center.Y-panelBounds.Center.Y);

			panel.SetManualBounds (Drawing.Rectangle.Constrain (panelBounds, stackBounds));
		}

		private void UpdateMask()
		{
			if (this.editPanels.Count > 0)
			{
				Panel topPanel = this.editPanels.Peek ();

				Drawing.Path path = topPanel.CreateAperturePath (true);

				if (path == null)
				{
					this.mask.Aperture = topPanel.ActualBounds;
				}
				else
				{
					this.mask.AperturePath = path;
				}
				
				this.mask.ZOrder = 0;
				this.mask.ZOrder = topPanel.ZOrder;
				this.mask.Show ();
			}
			else
			{
				this.mask.Hide ();
			}
		}

		private void OnEditPanelChanged(Panel oldValue, Panel newValue)
		{
			if (oldValue != newValue)
			{
				this.InvalidateProperty (PanelStack.EditPanelProperty, oldValue, newValue);
			}
		}


		private static object GetEditPanelValue(DependencyObject obj)
		{
			PanelStack stack = (PanelStack) obj;
			return stack.editPanels.Count == 0 ? null : stack.editPanels.Peek ();
		}
		
		public static readonly DependencyProperty EditPanelProperty = DependencyProperty.RegisterReadOnly ("EditPanel", typeof (Panel), typeof (PanelStack), new DependencyPropertyMetadata (PanelStack.GetEditPanelValue));

		private Stack<Panel> editPanels;
		private List<MiniPanel> miniPanels;
		private PanelMask mask;
	}
}
