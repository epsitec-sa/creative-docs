//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.UI.PanelStack))]

namespace Epsitec.Common.UI
{
	public class PanelStack : Widgets.AbstractGroup
	{
		public PanelStack()
		{
			this.editPanels = new Stack<Panel> ();
			this.mask = new PanelMask (this);
			this.mask.Hide ();
		}


		public void StartEdition(Panel panel)
		{
			this.editPanels.Push (panel);

			panel.Owner.BoundsChanged += this.HandleOwnerBoundsChanged;
			panel.SetParent (this);
			
			this.UpdateEditPanelBounds (panel);
			
			panel.Show ();
			panel.SetFocusOnTabWidget ();
		}

		private void HandleOwnerBoundsChanged(object sender, Types.DependencyPropertyChangedEventArgs e)
		{
			foreach (Panel panel in this.editPanels)
			{
				if (panel.Owner == sender)
				{
					this.UpdateEditPanelBounds (panel);
				}
			}
		}

		public void EndEdition()
		{
			this.EndEdition (this.editPanels.Peek ());
		}

		public bool EndEdition(Panel panel)
		{
			if (this.editPanels.Contains (panel))
			{
				Panel item;
				
				do
				{
					item = this.editPanels.Pop ();

					item.Hide ();
					
					item.Owner.BoundsChanged -= this.HandleOwnerBoundsChanged;
				}
				while (item != panel);

				this.UpdateMask ();
				panel.Owner.SetFocusOnTabWidget ();
				
				return true;
			}
			else
			{
				return false;
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

				widget = widget.Parent;
			}
			
			return null;
		}

		protected override void SetBoundsOverride(Drawing.Rectangle oldRect, Drawing.Rectangle newRect)
		{
			base.SetBoundsOverride (oldRect, newRect);

			this.mask.SetManualBounds (new Drawing.Rectangle (0, 0, newRect.Width, newRect.Height));
		}

		private void UpdateEditPanelBounds(Panel panel)
		{
			System.Diagnostics.Debug.Assert (this.editPanels.Count > 0);
			System.Diagnostics.Debug.Assert (this.editPanels.Contains (panel));
			
			Panel owner = panel.Owner;
			
			Drawing.Rectangle stackBounds = this.Client.Bounds;
			Drawing.Rectangle panelBounds = new Drawing.Rectangle (0, 0, System.Math.Max (panel.PreferredWidth, panel.MinWidth), System.Math.Max (panel.PreferredHeight, panel.MinHeight));
			Drawing.Rectangle ownerBounds = Widgets.Helpers.VisualTree.MapVisualToAncestor (owner, this, owner.Client.Bounds);

			panelBounds.Offset (ownerBounds.Center-panelBounds.Center);

			panel.SetManualBounds (Drawing.Rectangle.Constrain (panelBounds, stackBounds));

			if (panel == this.editPanels.Peek ())
			{
				this.UpdateMask ();
			}
		}

		private void UpdateMask()
		{
			if (this.editPanels.Count > 0)
			{
				Panel topPanel = this.editPanels.Peek ();
				
				this.mask.Aperture = topPanel.ActualBounds;
				this.mask.ZOrder = 1;
				this.mask.Show ();
			}
			else
			{
				this.mask.Hide ();
			}
		}

		private Stack<Panel> editPanels;
		private PanelMask mask;
	}
}
