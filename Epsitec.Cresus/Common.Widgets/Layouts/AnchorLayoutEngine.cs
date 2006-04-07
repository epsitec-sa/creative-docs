//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Layouts
{
	/// <summary>
	/// Summary description for AnchorLayout.
	/// </summary>
	public sealed class AnchorLayoutEngine : ILayoutEngine
	{
		public void UpdateLayout(Visual container, Drawing.Rectangle rect, IEnumerable<Visual> children)
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
				
				Drawing.Rectangle client  = rect;
				Drawing.Margins   margins = child.Margins;

				double x1, x2, y1, y2;

				Drawing.Size size = LayoutContext.GetResultingMeasuredSize (child);

				if (size == Drawing.Size.NegativeInfinity)
				{
					return;
				}

				double dx = size.Width;
				double dy = size.Height;

				if (double.IsNaN (dx))
				{
					dx = child.Width;		//	TODO: améliorer
				}
				if (double.IsNaN (dy))
				{
					dy = child.Height;		//	TODO: améliorer
				}
				
				switch (anchor_x)
				{
					case AnchorStyles.Left:							//	[x1] fixe à gauche
						x1 = client.Left + margins.Left;
						x2 = x1 + dx;
						break;
					case AnchorStyles.Right:						//	[x2] fixe à droite
						x2 = client.Right - margins.Right;
						x1 = x2 - dx;
						break;
					case AnchorStyles.None:							//	ne touche à rien...
						x1 = child.Left;
						x2 = child.Right;
						break;
					case AnchorStyles.LeftAndRight:					//	[x1] fixe à gauche, [x2] fixe à droite
						x1 = client.Left + margins.Left;
						x2 = client.Right - margins.Right;
						break;
					default:
						throw new System.NotSupportedException (string.Format ("AnchorStyle {0} not supported", anchor_x));
				}
				
				switch (anchor_y)
				{
					case AnchorStyles.Bottom:						//	[y1] fixe en bas
						y1 = client.Bottom + margins.Bottom;
						y2 = y1 + dy;
						break;
					case AnchorStyles.Top:							//	[y2] fixe en haut
						y2 = client.Top - margins.Top;
						y1 = y2 - dy;
						break;
					case AnchorStyles.None:							//	ne touche à rien...
						y1 = child.Bottom;
						y2 = child.Top;
						break;
					case AnchorStyles.TopAndBottom:					//	[y1] fixe en bas, [y2] fixe en haut
						y1 = client.Bottom + margins.Bottom;
						y2 = client.Top - margins.Top;
						break;
					default:
						throw new System.NotSupportedException (string.Format ("AnchorStyle {0} not supported", anchor_y));
				}
				
				child.SetBounds (Drawing.Rectangle.FromPoints (x1, y1, x2, y2));
			}
		}
	}
}
