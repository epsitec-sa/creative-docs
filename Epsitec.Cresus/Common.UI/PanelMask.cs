//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.UI;

using System.Collections.Generic;

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>PanelMask</c> class defines a translucent mask which covers part
	/// of a <see cref="PanelStack"/>. It is used with the <c>Panels.EditPanel</c>
	/// class to focus the user's attention on the important areas of the display.
	/// </summary>
	public class PanelMask : Widgets.Widget
	{
		public PanelMask()
		{
		}

		public PanelMask(Widgets.Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}

		public Drawing.Rectangle Aperture
		{
			get
			{
				return this.aperture;
			}
			set
			{
				if (this.aperture != value)
				{
					this.Invalidate (Drawing.Rectangle.Union (this.aperture, value));

					if (this.aperturePath != null)
					{
						this.aperturePath.Dispose ();
						this.aperturePath = null;
					}
					
					this.aperture     = value;
					this.aperturePath = new Drawing.Path (this.aperture);
				}
			}
		}

		public Drawing.Path AperturePath
		{
			get
			{
				return this.aperturePath;
			}
			set
			{
				if (this.aperturePath != value)
				{
					if (this.aperturePath != null)
					{
						this.aperturePath.Dispose ();
						this.aperturePath = null;
					}
					
					if (value != null)
					{
						Drawing.Rectangle bounds = value.ComputeBounds ();
						
						this.Invalidate (Drawing.Rectangle.Union (this.aperture, bounds));
						
						this.aperture     = bounds;
						this.aperturePath = value;
					}
					else
					{
						this.aperture     = Drawing.Rectangle.Empty;
						this.aperturePath = null;
					}
				}
			}
		}


		public Drawing.Color MaskColor
		{
			get
			{
				return (Drawing.Color) this.GetValue (PanelMask.MaskColorProperty);
			}
			set
			{
				this.SetValue (PanelMask.MaskColorProperty, value);
			}
		}

		public Drawing.Color ArrowColor
		{
			get
			{
				return (Drawing.Color) this.GetValue (PanelMask.ArrowColorProperty);
			}
			set
			{
				this.SetValue (PanelMask.ArrowColorProperty, value);
			}
		}

		public Widgets.Direction Arrow
		{
			get
			{
				return (Widgets.Direction) this.GetValue (PanelMask.ArrowProperty);
			}
			set
			{
				this.SetValue (PanelMask.ArrowProperty, value);
			}
		}


		protected override void ProcessMessage(Widgets.Message message, Drawing.Point pos)
		{
			if (message.MessageType == Widgets.MessageType.MouseDown)
			{
				message.Swallowed = true;
				this.OnMaskPressed ();
			}
			
			message.Consumer = this;
		}
		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			if (this.aperturePath == null)
			{
				graphics.AddFilledRectangle (this.Client.Bounds);
				graphics.RenderSolid (this.MaskColor);
			}
			else
			{
				Drawing.Path path  = new Drawing.Path (this.Client.Bounds);
				Drawing.Path shape = Drawing.Path.Combine (path, this.aperturePath, Drawing.PathOperation.AMinusB);

				graphics.Rasterizer.AddSurface (shape);
				graphics.RenderSolid (this.MaskColor);

				shape.Dispose ();
				path.Dispose ();
			}

			Drawing.Path arrowPath = null;

			switch (this.Arrow)
			{
				case Widgets.Direction.Right:
					arrowPath = this.GetRightArrowPath ();
					break;

					//	TODO: add other arrow paths
			}

			if (arrowPath != null)
			{
				graphics.Rasterizer.AddSurface (arrowPath);
				graphics.RenderSolid (this.ArrowColor);
				arrowPath.Dispose ();
			}
		}

		private Drawing.Path GetRightArrowPath()
		{
			Drawing.Path path = null;
			Drawing.Rectangle aperture = this.Aperture;

			double h = PanelMask.arrowBreadth / 2;
			
			double cy = aperture.Center.Y;
			double x1 = 0;
			double x2 = aperture.Left - 5;

			//	TODO: handle possible malformed and/or clipped arrows

			path = new Drawing.Path ();

			path.MoveTo (x1, cy+h);
			path.LineTo (x2-3*h, cy+h);
			path.LineTo (x2-3*h, cy+3*h);
			path.LineTo (x2, cy);
			path.LineTo (x2-3*h, cy-3*h);
			path.LineTo (x2-3*h, cy-h);
			path.LineTo (x1, cy-h);
			path.Close ();
			
			return path;
		}

		protected virtual void OnMaskPressed()
		{
			if (this.MaskPressed != null)
			{
				this.MaskPressed (this);
			}
		}

		public event EventHandler MaskPressed;

		public static readonly DependencyProperty MaskColorProperty = DependencyProperty.Register ("MaskColor", typeof (Drawing.Color), typeof (PanelMask), new Widgets.Helpers.VisualPropertyMetadata (Drawing.Color.FromAlphaRgb (0.5, 0.8, 0.8, 1.0), Widgets.Helpers.VisualPropertyMetadataOptions.AffectsDisplay));
		public static readonly DependencyProperty ArrowColorProperty = DependencyProperty.Register ("ArrowColor", typeof (Drawing.Color), typeof (PanelMask), new Widgets.Helpers.VisualPropertyMetadata (Drawing.Color.FromRgb (1.0, 1.0, 1.0), Widgets.Helpers.VisualPropertyMetadataOptions.AffectsDisplay));
		public static readonly DependencyProperty ArrowProperty = DependencyProperty.Register ("Arrow", typeof (Widgets.Direction), typeof (PanelMask), new Widgets.Helpers.VisualPropertyMetadata (Widgets.Direction.None, Epsitec.Common.Widgets.Helpers.VisualPropertyMetadataOptions.AffectsDisplay));

		private const double arrowBreadth = 20;

		private Drawing.Rectangle aperture;
		private Drawing.Path aperturePath;
	}
}
