//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Layouts
{
	/// <summary>
	/// AnchorLayoutEngine.
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
				
				AnchorStyles anchorX = child.Anchor & AnchorStyles.LeftAndRight;
				AnchorStyles anchorY = child.Anchor & AnchorStyles.TopAndBottom;
				
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
					dx = child.ActualWidth;		//	TODO: améliorer
				}
				if (double.IsNaN (dy))
				{
					dy = child.ActualHeight;		//	TODO: améliorer
				}
				
				switch (anchorX)
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
						x1 = child.ActualBounds.Left;
						x2 = child.ActualBounds.Right;
						break;
					case AnchorStyles.LeftAndRight:					//	[x1] fixe à gauche, [x2] fixe à droite
						x1 = client.Left + margins.Left;
						x2 = client.Right - margins.Right;
						break;
					default:
						throw new System.NotSupportedException (string.Format ("AnchorStyle {0} not supported", anchorX));
				}
				
				switch (anchorY)
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
						y1 = child.ActualBounds.Bottom;
						y2 = child.ActualBounds.Top;
						break;
					case AnchorStyles.TopAndBottom:					//	[y1] fixe en bas, [y2] fixe en haut
						y1 = client.Bottom + margins.Bottom;
						y2 = client.Top - margins.Top;
						break;
					default:
						throw new System.NotSupportedException (string.Format ("AnchorStyle {0} not supported", anchorY));
				}

				DockLayoutEngine.SetChildBounds (child, Drawing.Rectangle.FromPoints (x1, y1, x2, y2));
			}
		}

		public void UpdateMinMax(Visual container, LayoutContext context, IEnumerable<Visual> children, ref Drawing.Size minSize, ref Drawing.Size maxSize)
		{
			double minDx = minSize.Width;
			double minDy = minSize.Height;
			double maxDx = maxSize.Width;
			double maxDy = maxSize.Height;

			foreach (Visual child in children)
			{
				if ((child.Dock != DockStyle.None) ||
					(child.Anchor == AnchorStyles.None))
				{
					//	Saute les widgets qui sont "docked" dans le parent, car ils sont traités
					//	ailleurs. Ceux qui ne sont pas ancrés ne contribuent pas non plus.

					continue;
				}

				if (child.Visibility == false)
				{
					continue;
				}

				if (LayoutEngine.GetIgnoreMeasure (child))
				{
					continue;
				}

				Drawing.Margins margins = child.Margins;

				Layouts.LayoutMeasure measureDx = Layouts.LayoutMeasure.GetWidth (child);
				Layouts.LayoutMeasure measureDy = Layouts.LayoutMeasure.GetHeight (child);

				if ((measureDx == null) ||
					(measureDy == null))
				{
					throw new System.InvalidOperationException ();
				}

				AnchorStyles anchor = child.Anchor;

				switch (anchor & AnchorStyles.LeftAndRight)
				{
					case AnchorStyles.Left:
						minDx = System.Math.Max (minDx, margins.Left + System.Math.Max (measureDx.Min, measureDx.Desired));
						maxDx = System.Math.Min (maxDx, margins.Left + measureDx.Max);
						break;

					case AnchorStyles.Right:
						minDx = System.Math.Max (minDx, margins.Right + System.Math.Max (measureDx.Min, measureDx.Desired));
						maxDx = System.Math.Min (maxDx, margins.Right + measureDx.Max);
						break;

					case AnchorStyles.LeftAndRight:
						minDx = System.Math.Max (minDx, margins.Width + measureDx.Min);
						maxDx = System.Math.Min (maxDx, margins.Width + measureDx.Max);
						break;
				}

				switch (anchor & AnchorStyles.TopAndBottom)
				{
					case AnchorStyles.Bottom:
						minDy = System.Math.Max (minDy, margins.Bottom + System.Math.Max (measureDy.Min, measureDy.Desired));
						maxDy = System.Math.Min (maxDy, margins.Bottom + measureDy.Max);
						break;

					case AnchorStyles.Top:
						minDy = System.Math.Max (minDy, margins.Top + System.Math.Max (measureDy.Min, measureDy.Desired));
						maxDy = System.Math.Min (maxDy, margins.Top + measureDy.Max);
						break;

					case AnchorStyles.TopAndBottom:
						minDy = System.Math.Max (minDy, margins.Height + measureDy.Min);
						maxDy = System.Math.Min (maxDy, margins.Height + measureDy.Max);
						break;
				}
			}

			minSize = new Drawing.Size (minDx, minDy);
			maxSize = new Drawing.Size (maxDx, maxDy);
		}
		
		public LayoutMode LayoutMode
		{
			get
			{
				return LayoutMode.Anchored;
			}
		}
	}
}
