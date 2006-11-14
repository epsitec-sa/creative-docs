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
		}


		public void StartEdition(Panel panel)
		{
			this.editPanels.Push (panel);

			Panel owner = panel.Owner;
			Drawing.Rectangle bounds1 = Drawing.Rectangle.Empty;
			Drawing.Rectangle bounds2 = Widgets.Helpers.VisualTree.MapVisualToAncestor (owner, this, owner.Client.Bounds);

			bounds1.Width  = System.Math.Max (panel.PreferredWidth, panel.MinWidth);
			bounds1.Height = System.Math.Max (panel.PreferredHeight, panel.MinHeight);

			Drawing.Point center1 = bounds1.Center;
			Drawing.Point center2 = bounds2.Center;

			bounds1.Offset (center2-center1);

			//	TODO: fit into this.Client.Bounds...

			panel.SetManualBounds (bounds1);
			panel.SetParent (this);
			panel.Show ();
			
			this.UpdateMask ();
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
					item.SetParent (null);
				}
				while (item != panel);

				this.UpdateMask ();
				
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

		private void UpdateMask()
		{
			if (this.editPanels.Count > 0)
			{
				Panel topPanel = this.editPanels.Peek ();
				
				this.mask.Aperture = topPanel.Client.Bounds;
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
