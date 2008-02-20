//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.UI.Panels;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (EditPanel))]

namespace Epsitec.Common.UI.Panels
{
	internal class EditPanel : Panel, Widgets.ILogicalTree
	{
		public EditPanel()
		{
		}
		
		public EditPanel(Panel owner)
			: this ()
		{
			this.owner = owner;
		}

		public override Panel Owner
		{
			get
			{
				return this.owner;
			}
		}

		public override DataSource DataSource
		{
			get
			{
				return this.owner.DataSource;
			}
			set
			{
				this.owner.DataSource = value;
			}
		}

		public override DataSourceMetadata DataSourceMetadata
		{
			get
			{
				return this.owner.DataSourceMetadata;
			}
		}

		public override Panel GetPanel(PanelMode mode)
		{
			return this.owner.GetPanel (mode);
		}

		public override PanelMode PanelMode
		{
			get
			{
				return PanelMode.Edition;
			}
		}

		public override Drawing.Path CreateAperturePath(bool parentRelative)
		{
			if (this.Parent is PanelStack)
			{
				Drawing.Rectangle bounds = parentRelative ? this.ActualBounds : this.Client.Bounds;
				Drawing.Path path   = new Drawing.Path ();

				bounds.Deflate (1.0);

				double radius = 3;

				double x1 = bounds.Left;
				double x2 = bounds.Right;
				double y1 = bounds.Bottom;
				double y2 = bounds.Top;
				double dr = radius * 0.7;

				path.MoveTo (x1, y1+radius);
				path.LineTo (x1, y2-radius);
				path.CurveTo (x1, y2-radius+dr, x1+radius-dr, y2, x1+radius, y2);
				path.LineTo (x2-radius, y2);
				path.CurveTo (x2-radius+dr, y2, x2, y2-radius+dr, x2, y2-radius);
				path.LineTo (x2, y1+radius);
				path.CurveTo (x2, y1+radius-dr, x2-radius+dr, y1, x2-radius, y1);
				path.LineTo (x1+radius, y1);
				path.CurveTo (x1+radius-dr, y1, x1, y1+radius-dr, x1, y1+radius);
				path.Close ();

				return path;
			}
			else
			{
				return null;
			}
		}

		#region ILogicalTree Members

		Widgets.Visual Widgets.ILogicalTree.Parent
		{
			get
			{
				return this.owner ?? this.Parent;
			}
		}

		#endregion

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			Drawing.Path path = this.CreateAperturePath (false);

			if (path == null)
			{
				graphics.AddFilledRectangle (Drawing.Rectangle.Intersection (clipRect, this.Client.Bounds));
				graphics.RenderSolid (Drawing.Color.FromAlphaRgb (0.5, 1, 1, 1));
			}
			else
			{
				graphics.Rasterizer.AddSurface (path);
				graphics.RenderSolid (Drawing.Color.FromAlphaRgb (0.9, 1, 1, 1));

				graphics.Rasterizer.AddOutline (path, 2.0, Epsitec.Common.Drawing.CapStyle.Round, Epsitec.Common.Drawing.JoinStyle.Round);
				graphics.RenderSolid (Drawing.Color.FromName ("ActiveCaption"));
			}

			base.PaintBackgroundImplementation (graphics, clipRect);
		}
		
		protected override bool ProcessTabChildrenExit(Widgets.TabNavigationDir dir, Widgets.TabNavigationMode mode, out Widgets.Widget focus)
		{
			PanelStack stack = PanelStack.GetPanelStack (this);
			
			if (stack != null)
			{
				stack.EndEdition ();
				focus = this.Owner;
				return true;
			}

			return base.ProcessTabChildrenExit (dir, mode, out focus);
		}

		private static object GetOwnerValue(DependencyObject obj)
		{
			EditPanel panel = (EditPanel) obj;
			return panel.Owner;
		}

		private static void SetOwnerValue(DependencyObject obj, object value)
		{
			EditPanel panel = (EditPanel) obj;
			panel.owner = (Panel) value;
		}

		public static DependencyProperty OwnerProperty = DependencyProperty.Register ("Owner", typeof (Panel), typeof (EditPanel), new DependencyPropertyMetadata (EditPanel.GetOwnerValue, EditPanel.SetOwnerValue));
		
		private Panel owner;
	}
}
