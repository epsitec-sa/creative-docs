using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// La classe Cropper permet de définir le recadrage d'une image.
	/// </summary>
	public class Cropper : AbstractGroup
	{
		protected enum Part
		{
			None,
			Left,
			Right,
			Bottom,
			Top,
		}


		public Cropper()
		{
			this.AutoEngage = true;
			this.AutoRepeat = true;
			this.InternalState |= InternalState.Engageable;

			int tabIndex = 0;

			this.fieldCropLeft = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldCropLeft.LabelShortText = Res.Strings.Panel.Image.Short.CropLeft;
			this.fieldCropLeft.LabelLongText  = Res.Strings.Panel.Image.Long.CropLeft;
			this.fieldCropLeft.TextFieldReal.UnitType = RealUnitType.Scalar;
			this.fieldCropLeft.TextFieldReal.Step = 1.0M;
			this.fieldCropLeft.TextFieldReal.Resolution = 1.0M;
			this.fieldCropLeft.TextFieldReal.MinValue = (decimal) 0;
			this.fieldCropLeft.TextFieldReal.MaxValue = (decimal) 1000;
			this.fieldCropLeft.TextFieldReal.EditionAccepted += new EventHandler(this.HandleFieldChanged);
			this.fieldCropLeft.TabIndex = tabIndex++;
			this.fieldCropLeft.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldCropLeft, Res.Strings.Panel.Image.Tooltip.CropLeft);

			this.fieldCropRight = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldCropRight.LabelShortText = Res.Strings.Panel.Image.Short.CropRight;
			this.fieldCropRight.LabelLongText  = Res.Strings.Panel.Image.Long.CropRight;
			this.fieldCropRight.TextFieldReal.UnitType = RealUnitType.Scalar;
			this.fieldCropRight.TextFieldReal.Step = 1.0M;
			this.fieldCropRight.TextFieldReal.Resolution = 1.0M;
			this.fieldCropRight.TextFieldReal.MinValue = (decimal) 0;
			this.fieldCropRight.TextFieldReal.MaxValue = (decimal) 1000;
			this.fieldCropRight.TextFieldReal.EditionAccepted += new EventHandler(this.HandleFieldChanged);
			this.fieldCropRight.TabIndex = tabIndex++;
			this.fieldCropRight.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldCropRight, Res.Strings.Panel.Image.Tooltip.CropRight);

			this.fieldCropBottom = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldCropBottom.LabelShortText = Res.Strings.Panel.Image.Short.CropBottom;
			this.fieldCropBottom.LabelLongText  = Res.Strings.Panel.Image.Long.CropBottom;
			this.fieldCropBottom.TextFieldReal.UnitType = RealUnitType.Scalar;
			this.fieldCropBottom.TextFieldReal.Step = 1.0M;
			this.fieldCropBottom.TextFieldReal.Resolution = 1.0M;
			this.fieldCropBottom.TextFieldReal.MinValue = (decimal) 0;
			this.fieldCropBottom.TextFieldReal.MaxValue = (decimal) 1000;
			this.fieldCropBottom.TextFieldReal.EditionAccepted += new EventHandler(this.HandleFieldChanged);
			this.fieldCropBottom.TabIndex = tabIndex++;
			this.fieldCropBottom.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldCropBottom, Res.Strings.Panel.Image.Tooltip.CropBottom);

			this.fieldCropTop = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldCropTop.LabelShortText = Res.Strings.Panel.Image.Short.CropTop;
			this.fieldCropTop.LabelLongText  = Res.Strings.Panel.Image.Long.CropTop;
			this.fieldCropTop.TextFieldReal.UnitType = RealUnitType.Scalar;
			this.fieldCropTop.TextFieldReal.Step = 1.0M;
			this.fieldCropTop.TextFieldReal.Resolution = 1.0M;
			this.fieldCropTop.TextFieldReal.MinValue = (decimal) 0;
			this.fieldCropTop.TextFieldReal.MaxValue = (decimal) 1000;
			this.fieldCropTop.TextFieldReal.EditionAccepted += new EventHandler(this.HandleFieldChanged);
			this.fieldCropTop.TabIndex = tabIndex++;
			this.fieldCropTop.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldCropTop, Res.Strings.Panel.Image.Tooltip.CropTop);
		}

		public Cropper(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.fieldCropLeft.TextFieldReal.EditionAccepted -= new EventHandler(this.HandleFieldChanged);
				this.fieldCropRight.TextFieldReal.EditionAccepted -= new EventHandler(this.HandleFieldChanged);
				this.fieldCropBottom.TextFieldReal.EditionAccepted -= new EventHandler(this.HandleFieldChanged);
				this.fieldCropTop.TextFieldReal.EditionAccepted -= new EventHandler(this.HandleFieldChanged);

				this.fieldCropLeft = null;
				this.fieldCropRight = null;
				this.fieldCropBottom = null;
				this.fieldCropTop = null;
			}

			base.Dispose(disposing);
		}


		public Margins Crop
		{
			//	Marges de recadrage.
			get
			{
				return this.crop;
			}
			set
			{
				if (this.crop != value)
				{
					this.crop = value;
					this.UpdateField();
					this.Invalidate();
					this.OnCropChanged();
				}
			}
		}

		public Size Size
		{
			//	Taille de l'image.
			get
			{
				return this.size;
			}
			set
			{
				if (this.size != value)
				{
					this.size = value;
					this.Invalidate();
				}
			}
		}


		protected void UpdateField()
		{
			this.ignoreChanged = true;

			this.fieldCropLeft.TextFieldReal.InternalValue = (decimal) crop.Left;
			this.fieldCropRight.TextFieldReal.InternalValue = (decimal) crop.Right;
			this.fieldCropBottom.TextFieldReal.InternalValue = (decimal) crop.Bottom;
			this.fieldCropTop.TextFieldReal.InternalValue = (decimal) crop.Top;
			
			this.ignoreChanged = false;
		}


		protected Size ImageSize
		{
			//	Retourne la taille de l'image. Si elle est inconnue, retourne une taille par défaut.
			get
			{
				if (this.size.IsEmpty)  // taille inconnue ?
				{
					return new Size(1000, 1000);
				}

				return this.size;
			}
		}

		protected Rectangle UsedRectangle
		{
			//	Retourne la zone pour la partie interactive.
			get
			{
				Rectangle rect = this.Client.Bounds;
				rect.Left = rect.Right-rect.Height;  // doit être carré

				if (rect.Width < 0)
				{
					return Rectangle.Empty;
				}

				return rect;
			}
		}

		protected Rectangle CropRectangle
		{
			get
			{
				Rectangle rect = this.UsedRectangle;
				Size size = this.ImageSize;
				Margins crop = Margins.Zero;

				crop.Left = this.crop.Left*rect.Width/size.Width;
				crop.Right = this.crop.Right*rect.Width/size.Width;
				crop.Bottom = this.crop.Bottom*rect.Height/size.Height;
				crop.Top = this.crop.Top*rect.Height/size.Height;
				rect.Deflate(crop);

				if (rect.IsSurfaceZero)
				{
					return Rectangle.Empty;
				}

				return rect;
			}
		}

		
		protected override void ProcessMessage(Message message, Point pos)
		{
			switch (message.Type)
			{
				case MessageType.MouseDown:
					message.Captured = true;
					message.Consumer = this;
					break;

				case MessageType.MouseMove:
					if (this.mouseDown)
					{
					}
					else
					{
					}
					message.Consumer = this;
					break;

				case MessageType.MouseUp:
					this.mouseDown = false;
					message.Consumer = this;
					break;

				case MessageType.MouseLeave:
					if (!this.mouseDown)
					{
					}
					break;
			}
		}

		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.fieldCropLeft == null )  return;

			Rectangle rect = this.Client.Bounds;
			rect.Width -= rect.Height+5;

			if (rect.Width < 100 || rect.Height < 45)
			{
				this.fieldCropLeft.Visibility = false;
				this.fieldCropRight.Visibility = false;
				this.fieldCropBottom.Visibility = false;
				this.fieldCropTop.Visibility = false;
			}
			else
			{
				this.fieldCropLeft.Visibility = true;
				this.fieldCropRight.Visibility = true;
				this.fieldCropBottom.Visibility = true;
				this.fieldCropTop.Visibility = true;

				Rectangle r = rect;
				r.Bottom = r.Top-20;
				r.Width = (r.Width-5)*0.5;
				this.fieldCropLeft.SetManualBounds(r);
				r.Offset(r.Width+5, 0);
				this.fieldCropRight.SetManualBounds(r);
				r.Offset(-(r.Width+5), -25);
				this.fieldCropBottom.SetManualBounds(r);
				r.Offset(r.Width+5, 0);
				this.fieldCropTop.SetManualBounds(r);
			}
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			Rectangle bounds = this.UsedRectangle;
			graphics.Align(ref bounds);
			bounds.Deflate(0.5);

			Rectangle inside = this.CropRectangle;
			if (inside.IsEmpty)
			{
				graphics.Rasterizer.AddOutline(Misc.GetHatchPath(bounds, 4, bounds.BottomLeft));
				graphics.AddRectangle(bounds);
				graphics.RenderSolid(adorner.ColorBorder);
			}
			else
			{
				graphics.Align(ref inside);
				inside.Deflate(0.5);

				if (bounds == inside)
				{
					graphics.AddRectangle(bounds);
					graphics.RenderSolid(adorner.ColorBorder);
				}
				else
				{
					Rectangle left   = new Rectangle(bounds.Left, bounds.Bottom, inside.Left-bounds.Left, bounds.Height);
					Rectangle right  = new Rectangle(inside.Right, bounds.Bottom, bounds.Right-inside.Right, bounds.Height);
					Rectangle bottom = new Rectangle(inside.Left, bounds.Bottom, inside.Width, inside.Bottom-bounds.Bottom);
					Rectangle top    = new Rectangle(inside.Left, inside.Top, inside.Width, bounds.Top-inside.Top);

					graphics.Rasterizer.AddOutline(Misc.GetHatchPath(left, 4, bounds.BottomLeft));
					graphics.Rasterizer.AddOutline(Misc.GetHatchPath(right, 4, bounds.BottomLeft));
					graphics.Rasterizer.AddOutline(Misc.GetHatchPath(bottom, 4, bounds.BottomLeft));
					graphics.Rasterizer.AddOutline(Misc.GetHatchPath(top, 4, bounds.BottomLeft));

					graphics.AddRectangle(bounds);
					graphics.AddRectangle(inside);

					graphics.RenderSolid(adorner.ColorBorder);
				}
			}
		}


		private void HandleFieldChanged(object sender)
		{
			//	Un champ a été changé.
			if (this.ignoreChanged)
			{
				return;
			}

			TextFieldReal field = sender as TextFieldReal;
			Margins crop = this.Crop;

			if (field == this.fieldCropLeft.TextFieldReal)
			{
				crop.Left = (double) field.InternalValue;
			}

			if (field == this.fieldCropRight.TextFieldReal)
			{
				crop.Right = (double) field.InternalValue;
			}

			if (field == this.fieldCropBottom.TextFieldReal)
			{
				crop.Bottom = (double) field.InternalValue;
			}

			if (field == this.fieldCropTop.TextFieldReal)
			{
				crop.Top = (double) field.InternalValue;
			}

			this.Crop = crop;
		}



		#region Events handler
		protected virtual void OnCropChanged()
		{
			//	Génère un événement pour dire que l'offset a changé.
			EventHandler handler = (EventHandler) this.GetUserEventHandler("CropChanged");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event Support.EventHandler CropChanged
		{
			add
			{
				this.AddUserEventHandler("CropChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("CropChanged", value);
			}
		}
		#endregion


		protected Margins					crop;
		protected Size						size;

		protected Widgets.TextFieldLabel	fieldCropLeft;
		protected Widgets.TextFieldLabel	fieldCropRight;
		protected Widgets.TextFieldLabel	fieldCropBottom;
		protected Widgets.TextFieldLabel	fieldCropTop;

		protected bool						ignoreChanged = false;
		protected bool						mouseDown = false;
	}
}
