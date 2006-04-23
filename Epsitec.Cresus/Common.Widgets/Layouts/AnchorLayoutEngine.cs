//	Copyright � 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
					//	Saute les widgets qui sont "docked" dans le parent, car ils ont d�j� �t�
					//	positionn�s. Ceux qui ne sont pas ancr�s ne bougent pas non plus.
					
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
					dx = child.Width;		//	TODO: am�liorer
				}
				if (double.IsNaN (dy))
				{
					dy = child.Height;		//	TODO: am�liorer
				}
				
				switch (anchor_x)
				{
					case AnchorStyles.Left:							//	[x1] fixe � gauche
						x1 = client.Left + margins.Left;
						x2 = x1 + dx;
						break;
					case AnchorStyles.Right:						//	[x2] fixe � droite
						x2 = client.Right - margins.Right;
						x1 = x2 - dx;
						break;
					case AnchorStyles.None:							//	ne touche � rien...
						x1 = child.Left;
						x2 = child.Right;
						break;
					case AnchorStyles.LeftAndRight:					//	[x1] fixe � gauche, [x2] fixe � droite
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
					case AnchorStyles.None:							//	ne touche � rien...
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
				
				AnchorLayoutEngine.SetChildBounds (child, Drawing.Rectangle.FromPoints (x1, y1, x2, y2));
			}
		}
		
		public void UpdateMinMax(Visual container, IEnumerable<Visual> children, ref Drawing.Size min_size, ref Drawing.Size max_size)
		{
		}
		
		private static void SetChildBounds(Visual child, Drawing.Rectangle bounds)
		{
			double dx = child.PreferredWidth;
			double dy = child.PreferredHeight;

			switch (child.VerticalAlignment)
			{
				case VerticalAlignment.Stretch:
					break;
				case VerticalAlignment.Top:
					bounds.Bottom = bounds.Top - dy;
					break;
				case VerticalAlignment.Center:
					double h = bounds.Height;
					bounds.Top = bounds.Top - (h - dy) / 2;
					bounds.Bottom = bounds.Bottom + (h - dy) / 2;
					break;
				case VerticalAlignment.Bottom:
					bounds.Top = bounds.Bottom + dy;
					break;
			}

			switch (child.HorizontalAlignment)
			{
				case HorizontalAlignment.Stretch:
					break;
				case HorizontalAlignment.Left:
					bounds.Right = bounds.Left + dx;
					break;
				case HorizontalAlignment.Center:
					double w = bounds.Width;
					bounds.Left = bounds.Left + (w - dx) / 2;
					bounds.Right = bounds.Right - (w - dx) / 2;
					break;
				case HorizontalAlignment.Right:
					bounds.Left = bounds.Right - dx;
					break;
			}

			child.SetBounds (bounds);
		}
	}
}
