//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Layouts
{
	/// <summary>
	/// Summary description for AnchorLayout.
	/// </summary>
	public sealed class AnchorLayout : ILayout
	{
		public void UpdateLayout(Visual container, System.Collections.ICollection children)
		{
			foreach (Visual child in children)
			{
				if ((child.Dock != DockStyle.None) ||
					(child.Anchor == AnchorStyles.None))
				{
					//	Saute les widgets qui sont "docked" dans le parent, car ils ont déjà été
					//	positionnés. Ceux qui ne sont pas ancrés ne bougent pas non plus.
					
					continue;
				}
				
				AnchorStyles anchor_x = child.Anchor & AnchorStyles.LeftAndRight;
				AnchorStyles anchor_y = child.Anchor & AnchorStyles.TopAndBottom;
				
				Drawing.Rectangle client  = container.Client.Bounds;
				Drawing.Rectangle bounds  = child.Bounds;
				Drawing.Margins   margins = child.AnchorMargins;
				
				double x1 = bounds.Left;
				double x2 = bounds.Right;
				double y1 = bounds.Bottom;
				double y2 = bounds.Top;
				
				switch (anchor_x)
				{
					case AnchorStyles.Left:							//	[x1] fixe à gauche
						x1 = margins.Left;
						x2 = x1 + child.Width;
						break;
					case AnchorStyles.Right:						//	[x2] fixe à droite
						x2 = client.Width - margins.Right;
						x1 = x2 - child.Width;
						break;
					case AnchorStyles.None:							//	ne touche à rien...
						break;
					case AnchorStyles.LeftAndRight:					//	[x1] fixe à gauche, [x2] fixe à droite
						x1 = margins.Left;
						x2 = client.Width - margins.Right;
						break;
				}
				
				switch (anchor_y)
				{
					case AnchorStyles.Bottom:						//	[y1] fixe en bas
						y1 = margins.Bottom;
						y2 = y1 + child.Height;
						break;
					case AnchorStyles.Top:							//	[y2] fixe en haut
						y2 = client.Height - margins.Top;
						y1 = y2 - child.Height;
						break;
					case AnchorStyles.None:							//	ne touche à rien...
						break;
					case AnchorStyles.TopAndBottom:					//	[y1] fixe en bas, [y2] fixe en haut
						y1 = margins.Bottom;
						y2 = client.Height - margins.Top;
						break;
				}
				
				child.SetBounds (Drawing.Rectangle.FromPoints (x1, y1, x2, y2));
			}
		}
	}
}
