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
				
				AnchorLayoutEngine.SetChildBounds (child, Drawing.Rectangle.FromPoints (x1, y1, x2, y2));
			}
		}
		
		public void UpdateMinMax(Visual container, IEnumerable<Visual> children, ref Drawing.Size min_size, ref Drawing.Size max_size)
		{
			//	Décompose les dimensions comme suit :
			//
			//	|											  |
			//	|<---min_ox1--->| zone de travail |<-min_ox2->|
			//	|											  |
			//	|<-------------------min_dx------------------>|
			//
			//	Idem par analogie pour dy.

			double min_ox1 = 0;
			double min_ox2 = 0;
			double min_oy1 = 0;
			double min_oy2 = 0;

			double min_dx = 0;
			double min_dy = 0;
			double max_dx = 1000000;
			double max_dy = 1000000;

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

				AnchorStyles anchor = child.Anchor;

				switch (anchor & AnchorStyles.LeftAndRight)
				{
					case AnchorStyles.Left:
						min_ox1 = System.Math.Max (min_ox1, margins.Left);
						min_dx  = System.Math.Max (min_dx, margins.Left + measure_dx.Min);
						break;

					case AnchorStyles.Right:
						min_ox2 = System.Math.Max (min_ox2, margins.Right);
						min_dx  = System.Math.Max (min_dx, margins.Left + measure_dx.Min);
						break;

					case AnchorStyles.LeftAndRight:
						min_ox1 = System.Math.Max (min_ox1, margins.Left);
						min_ox2 = System.Math.Max (min_ox2, margins.Right);
						min_dx  = System.Math.Max (min_dx, margins.Width + measure_dx.Min);
						break;
				}

				switch (anchor & AnchorStyles.TopAndBottom)
				{
					case AnchorStyles.Bottom:
						min_oy1 = System.Math.Max (min_oy1, margins.Bottom);
						min_dy  = System.Math.Max (min_dy, margins.Bottom + measure_dy.Min);
						break;

					case AnchorStyles.Top:
						min_oy2 = System.Math.Max (min_oy2, margins.Top);
						min_dy  = System.Math.Max (min_dy, margins.Bottom + measure_dy.Min);
						break;

					case AnchorStyles.TopAndBottom:
						min_oy1 = System.Math.Max (min_oy1, margins.Bottom);
						min_oy2 = System.Math.Max (min_oy2, margins.Top);
						min_dy  = System.Math.Max (min_dy, margins.Height + measure_dy.Min);
						break;
				}
			}

			double pad_width  = container.Padding.Width  + container.InternalPadding.Width;
			double pad_height = container.Padding.Height + container.InternalPadding.Height;

			double min_width  = min_dx + pad_width;
			double min_height = min_dy + pad_height;
			double max_width  = max_dx + pad_width;
			double max_height = max_dy + pad_height;

			//	Tous les calculs ont été faits en coordonnées client, il faut donc encore transformer
			//	ces dimensions en coordonnées parents.

			min_size = Helpers.VisualTree.MapVisualToParent (container, new Drawing.Size (min_width, min_height));
			max_size = Helpers.VisualTree.MapVisualToParent (container, new Drawing.Size (max_width, max_height));

			Widget widget = container as Widget;

			//	TODO: supprimer ce hack (AutoMinSize et AutoMaxSize ne devraient plus exister)

			if (widget != null)
			{
				widget.AutoMinSize = min_size;
				widget.AutoMaxSize = max_size;
			}
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
