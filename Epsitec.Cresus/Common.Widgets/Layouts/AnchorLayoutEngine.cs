//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
					dx = child.ActualWidth;		//	TODO: améliorer
				}
				if (double.IsNaN (dy))
				{
					dy = child.ActualHeight;		//	TODO: améliorer
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
						x1 = child.ActualBounds.Left;
						x2 = child.ActualBounds.Right;
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
						y1 = child.ActualBounds.Bottom;
						y2 = child.ActualBounds.Top;
						break;
					case AnchorStyles.TopAndBottom:					//	[y1] fixe en bas, [y2] fixe en haut
						y1 = client.Bottom + margins.Bottom;
						y2 = client.Top - margins.Top;
						break;
					default:
						throw new System.NotSupportedException (string.Format ("AnchorStyle {0} not supported", anchor_y));
				}
				
				DockLayoutEngine.SetChildBounds (child, Drawing.Rectangle.FromPoints (x1, y1, x2, y2));
			}
		}

		public void UpdateMinMax(Visual container, LayoutContext context, IEnumerable<Visual> children, ref Drawing.Size min_size, ref Drawing.Size max_size)
		{
			double min_dx = min_size.Width;
			double min_dy = min_size.Height;
			double max_dx = max_size.Width;
			double max_dy = max_size.Height;

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

				Drawing.Margins margins = child.Margins;

				Layouts.LayoutMeasure measure_dx = Layouts.LayoutMeasure.GetWidth (child);
				Layouts.LayoutMeasure measure_dy = Layouts.LayoutMeasure.GetHeight (child);

				if ((measure_dx == null) ||
					(measure_dy == null))
				{
					throw new System.InvalidOperationException ();
				}

				AnchorStyles anchor = child.Anchor;

				switch (anchor & AnchorStyles.LeftAndRight)
				{
					case AnchorStyles.Left:
						min_dx = System.Math.Max (min_dx, margins.Left + System.Math.Max (measure_dx.Min, measure_dx.Desired));
						max_dx = System.Math.Min (max_dx, margins.Left + measure_dx.Max);
						break;

					case AnchorStyles.Right:
						min_dx = System.Math.Max (min_dx, margins.Right + System.Math.Max (measure_dx.Min, measure_dx.Desired));
						max_dx = System.Math.Min (max_dx, margins.Right + measure_dx.Max);
						break;

					case AnchorStyles.LeftAndRight:
						min_dx = System.Math.Max (min_dx, margins.Width + measure_dx.Min);
						max_dx = System.Math.Min (max_dx, margins.Width + measure_dx.Max);
						break;
				}

				switch (anchor & AnchorStyles.TopAndBottom)
				{
					case AnchorStyles.Bottom:
						min_dy = System.Math.Max (min_dy, margins.Bottom + System.Math.Max (measure_dy.Min, measure_dy.Desired));
						max_dy = System.Math.Min (max_dy, margins.Bottom + measure_dy.Max);
						break;

					case AnchorStyles.Top:
						min_dy = System.Math.Max (min_dy, margins.Top + System.Math.Max (measure_dy.Min, measure_dy.Desired));
						max_dy = System.Math.Min (max_dy, margins.Top + measure_dy.Max);
						break;

					case AnchorStyles.TopAndBottom:
						min_dy = System.Math.Max (min_dy, margins.Height + measure_dy.Min);
						max_dy = System.Math.Min (max_dy, margins.Height + measure_dy.Max);
						break;
				}
			}

			min_size = new Drawing.Size (min_dx, min_dy);
			max_size = new Drawing.Size (max_dx, max_dy);
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
