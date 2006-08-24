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
			BottomLeft,
			BottomRight,
			TopLeft,
			TopRight,
			Showed,
		}


		public Cropper()
		{
			this.AutoEngage = true;
			this.AutoRepeat = true;
			this.InternalState |= InternalState.Engageable;

			int tabIndex = 0;

			this.fieldCropLeft = new TextFieldReal(this);
			Cropper.InitTextFieldReal(this.fieldCropLeft);
			this.fieldCropLeft.MaxValue = (decimal) 1000;
			this.fieldCropLeft.EditionAccepted += new EventHandler(this.HandleFieldChanged);
			this.fieldCropLeft.TabIndex = tabIndex++;
			this.fieldCropLeft.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldCropLeft, Res.Strings.Panel.Image.Tooltip.CropLeft);

			this.fieldCropRight = new TextFieldReal(this);
			Cropper.InitTextFieldReal(this.fieldCropRight);
			this.fieldCropRight.MaxValue = (decimal) 1000;
			this.fieldCropRight.EditionAccepted += new EventHandler(this.HandleFieldChanged);
			this.fieldCropRight.TabIndex = tabIndex++;
			this.fieldCropRight.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldCropRight, Res.Strings.Panel.Image.Tooltip.CropRight);

			this.fieldCropBottom = new TextFieldReal(this);
			Cropper.InitTextFieldReal(this.fieldCropBottom);
			this.fieldCropBottom.MaxValue = (decimal) 1000;
			this.fieldCropBottom.EditionAccepted += new EventHandler(this.HandleFieldChanged);
			this.fieldCropBottom.TabIndex = tabIndex++;
			this.fieldCropBottom.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldCropBottom, Res.Strings.Panel.Image.Tooltip.CropBottom);

			this.fieldCropTop = new TextFieldReal(this);
			Cropper.InitTextFieldReal(this.fieldCropTop);
			this.fieldCropTop.MaxValue = (decimal) 1000;
			this.fieldCropTop.EditionAccepted += new EventHandler(this.HandleFieldChanged);
			this.fieldCropTop.TabIndex = tabIndex++;
			this.fieldCropTop.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldCropTop, Res.Strings.Panel.Image.Tooltip.CropTop);

			this.buttonReset = new Button(this);
			this.buttonReset.Text = Res.Strings.Panel.Image.Button.Reset;
			this.buttonReset.TabIndex = tabIndex++;
			this.buttonReset.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.buttonReset.Clicked += new MessageEventHandler(this.HandleButtonReset);
			ToolTip.Default.SetToolTip(this.buttonReset, Res.Strings.Panel.Image.Tooltip.Reset);
		}

		public Cropper(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.fieldCropLeft.EditionAccepted -= new EventHandler(this.HandleFieldChanged);
				this.fieldCropRight.EditionAccepted -= new EventHandler(this.HandleFieldChanged);
				this.fieldCropBottom.EditionAccepted -= new EventHandler(this.HandleFieldChanged);
				this.fieldCropTop.EditionAccepted -= new EventHandler(this.HandleFieldChanged);

				this.buttonReset.Clicked -= new MessageEventHandler(this.HandleButtonReset);

				this.fieldCropLeft = null;
				this.fieldCropRight = null;
				this.fieldCropBottom = null;
				this.fieldCropTop = null;
				this.buttonReset = null;
			}

			base.Dispose(disposing);
		}


		static protected void InitTextFieldReal(TextFieldReal field)
		{
			field.UnitType = RealUnitType.Scalar;
			field.MinValue = (decimal) 0;
			field.Step = 1.0M;
			field.Resolution = 1.0M;
			field.DefocusAction = DefocusAction.AutoAcceptOrRejectEdition;
			field.AutoSelectOnFocus = true;
			field.SwallowEscape = true;
			field.SwallowReturn = true;
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
					this.InvalidateBounds();
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
					this.UpdateField();
					this.InvalidateBounds();
				}
			}
		}


		protected void UpdateField()
		{
			//	Met à jour tous les widgets, suite à une modification.
			this.ignoreChanged = true;

			this.fieldCropLeft.MaxValue = (decimal) this.size.Width;
			this.fieldCropRight.MaxValue = (decimal) this.size.Width;
			this.fieldCropBottom.MaxValue = (decimal) this.size.Height;
			this.fieldCropTop.MaxValue = (decimal) this.size.Height;

			this.fieldCropLeft.InternalValue = (decimal) crop.Left;
			this.fieldCropRight.InternalValue = (decimal) crop.Right;
			this.fieldCropBottom.InternalValue = (decimal) crop.Bottom;
			this.fieldCropTop.InternalValue = (decimal) crop.Top;

			this.buttonReset.Enable = (this.crop != Margins.Zero);
			
			this.ignoreChanged = false;
		}


		protected Size ImageSize
		{
			//	Retourne la taille de l'image. Si elle est inconnue, retourne une taille
			//	par défaut arbitraire de 1000 x 1000.
			get
			{
				if (this.size.IsEmpty)  // taille inconnue ?
				{
					return new Size(1000, 1000);
				}

				return this.size;
			}
		}

		protected Rectangle BoundsRectangle
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
			//	Retourne la zone pour la partie recadrée dans la partie interactive.
			get
			{
				Rectangle bounds = this.BoundsRectangle;
				Size size = this.ImageSize;

				Margins crop = Margins.Zero;
				crop.Left = this.crop.Left*bounds.Width/size.Width;
				crop.Right = this.crop.Right*bounds.Width/size.Width;
				crop.Bottom = this.crop.Bottom*bounds.Height/size.Height;
				crop.Top = this.crop.Top*bounds.Height/size.Height;
				bounds.Deflate(crop);

				if (bounds.IsSurfaceZero)
				{
					return Rectangle.Empty;
				}

				return bounds;
			}
		}

		protected void InvalidateBounds()
		{
			//	Invalide la partie interactive.
			Rectangle bounds = this.BoundsRectangle;
			bounds.Inflate(1);
			this.Invalidate(bounds);
		}

		protected Point ConvWidgetToImage(Point mouse)
		{
			//	Conversion d'une position de la souris dans un widget en position dans l'image.
			Rectangle bounds = this.BoundsRectangle;
			Size size = this.ImageSize;

			mouse -= bounds.BottomLeft;
			Point pos = new Point(mouse.X*size.Width/bounds.Width, mouse.Y*size.Height/bounds.Height);

			pos.X = System.Math.Max(pos.X, 0);
			pos.X = System.Math.Min(pos.X, size.Width);
			pos.Y = System.Math.Max(pos.Y, 0);
			pos.Y = System.Math.Min(pos.Y, size.Height);

			return pos;
		}

		protected Part DetectPart(Point pos)
		{
			//	Détecte la partie survolée par la souris.
			Rectangle crop = this.CropRectangle;
			double m = 2;

			bool left   = (pos.X >= crop.Left-m   && pos.X <= crop.Left+m);
			bool right  = (pos.X >= crop.Right-m  && pos.X <= crop.Right+m);
			bool bottom = (pos.Y >= crop.Bottom-m && pos.Y <= crop.Bottom+m);
			bool top    = (pos.Y >= crop.Top-m    && pos.Y <= crop.Top+m);

			if (bottom && left)  return Part.BottomLeft;
			if (bottom && right) return Part.BottomRight;
			if (top && left)     return Part.TopLeft;
			if (top && right)    return Part.TopRight;
			if (left)            return Part.Left;
			if (right)           return Part.Right;
			if (bottom)          return Part.Bottom;
			if (top)             return Part.Top;

			if (crop.Contains(pos) && this.crop != Margins.Zero)
			{
				return Part.Showed;
			}

			return Part.None;
		}

		protected void MovePart(Point pos, bool isConstrain)
		{
			//	Déplace un élément selon la souris.
			Point m = pos-this.initialPos;
			bool isHorizontal = System.Math.Abs(m.X) > System.Math.Abs(m.Y);

			pos = this.ConvWidgetToImage(pos);
			Margins crop = this.Crop;
			Size size = this.ImageSize;

			if (this.hilited == Part.Left || this.hilited == Part.BottomLeft || this.hilited == Part.TopLeft)
			{
				crop.Left = pos.X;
			}

			if (this.hilited == Part.Right || this.hilited == Part.BottomRight || this.hilited == Part.TopRight)
			{
				crop.Right = size.Width-pos.X;
			}

			if (this.hilited == Part.Bottom || this.hilited == Part.BottomLeft || this.hilited == Part.BottomRight)
			{
				crop.Bottom = pos.Y;
			}

			if (this.hilited == Part.Top || this.hilited == Part.TopLeft || this.hilited == Part.TopRight)
			{
				crop.Top = size.Height-pos.Y;
			}

			if (this.hilited == Part.Showed)
			{
				Point move = pos - this.ConvWidgetToImage(this.initialPos);

				if (isConstrain)
				{
					if (isHorizontal)  move.Y = 0;
					else               move.X = 0;
				}

				crop = this.initialCrop;
				crop.Left   += move.X;
				crop.Right  -= move.X;
				crop.Bottom += move.Y;
				crop.Top    -= move.Y;

				if (crop.Left < 0)
				{
					crop.Right += crop.Left;
					crop.Left = 0;
				}

				if (crop.Right < 0)
				{
					crop.Left += crop.Right;
					crop.Right = 0;
				}

				if (crop.Bottom < 0)
				{
					crop.Top += crop.Bottom;
					crop.Bottom = 0;
				}

				if (crop.Top < 0)
				{
					crop.Bottom += crop.Top;
					crop.Top = 0;
				}
			}

			this.Crop = crop;
		}

		protected Part Hilited
		{
			//	Partie mise en évidence.
			get
			{
				return this.hilited;
			}
			set
			{
				if (this.hilited != value)
				{
					this.hilited = value;
					this.InvalidateBounds();
				}
			}
		}


		public override Drawing.Margins GetShapeMargins()
		{
			//	Marges supplémentaires utiles lorsqu'il y a des hilited.
			return new Drawing.Margins(0, 1, 1, 1);
		}

		protected override void ProcessMessage(Message message, Point pos)
		{
			//	Gestion des événements.
			switch (message.Type)
			{
				case MessageType.MouseDown:
					if (this.hilited != Part.None)
					{
						this.initialPos = pos;
						this.initialCrop = this.crop;
						this.mouseDown = true;
					}
					message.Captured = true;
					message.Consumer = this;
					break;

				case MessageType.MouseMove:
					if (this.mouseDown)
					{
						this.MovePart(pos, message.IsControlPressed || message.IsShiftPressed);
					}
					else
					{
						this.Hilited = this.DetectPart(pos);
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
						this.Hilited = Part.None;
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
			rect.Width -= rect.Height+5;  // place pour la partie interactive de droite

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

				if (rect.Height < 70)  // disposition 2.2 ?
				{
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

					this.buttonReset.Visibility = false;
				}
				else  // disposition 1.2.1 ?
				{
					double w = (rect.Width-5)*0.5;
					Rectangle r = rect;
					r.Bottom = r.Top-20;

					r.Left = rect.Left+w/2;
					r.Width = w;
					this.fieldCropTop.SetManualBounds(r);

					r.Offset(0, -25);
					r.Left = rect.Left;
					r.Width = w;
					this.fieldCropLeft.SetManualBounds(r);
					r.Offset(w+5, 0);
					this.fieldCropRight.SetManualBounds(r);

					r.Offset(0, -25);
					r.Left = rect.Left+w/2;
					r.Width = w;
					this.fieldCropBottom.SetManualBounds(r);

					r.Left = rect.Right-20;
					r.Right = rect.Right;
					this.buttonReset.SetManualBounds(r);
					this.buttonReset.Visibility = true;
				}
			}
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			//	Dessine la partie interactive de droite.
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			Rectangle bounds = this.BoundsRectangle;
			graphics.Align(ref bounds);
			bounds.Deflate(0.5);

			Rectangle crop = this.CropRectangle;
			if (crop.IsEmpty)  // aucune partie recadrée ?
			{
				graphics.Rasterizer.AddOutline(Misc.GetHatchPath(bounds, 4, bounds.BottomLeft));
				graphics.AddRectangle(bounds);
				graphics.RenderSolid(adorner.ColorBorder);
			}
			else
			{
				graphics.Align(ref crop);
				crop.Deflate(0.5);

				if (bounds == crop)  // partie recadrée = toute l'image ?
				{
					graphics.AddRectangle(bounds);
					graphics.RenderSolid(adorner.ColorBorder);
				}
				else
				{
					Rectangle left   = new Rectangle(bounds.Left, bounds.Bottom, crop.Left-bounds.Left, bounds.Height);
					Rectangle right  = new Rectangle(crop.Right, bounds.Bottom, bounds.Right-crop.Right, bounds.Height);
					Rectangle bottom = new Rectangle(crop.Left, bounds.Bottom, crop.Width, crop.Bottom-bounds.Bottom);
					Rectangle top    = new Rectangle(crop.Left, crop.Top, crop.Width, bounds.Top-crop.Top);

					graphics.Rasterizer.AddOutline(Misc.GetHatchPath(left, 4, bounds.BottomLeft));
					graphics.Rasterizer.AddOutline(Misc.GetHatchPath(right, 4, bounds.BottomLeft));
					graphics.Rasterizer.AddOutline(Misc.GetHatchPath(bottom, 4, bounds.BottomLeft));
					graphics.Rasterizer.AddOutline(Misc.GetHatchPath(top, 4, bounds.BottomLeft));

					graphics.AddRectangle(bounds);
					graphics.AddRectangle(crop);

					graphics.RenderSolid(adorner.ColorBorder);
				}

				if (this.hilited == Part.Left || this.hilited == Part.BottomLeft || this.hilited == Part.TopLeft)
				{
					graphics.LineWidth = 3;
					graphics.AddLine(crop.Left, bounds.Bottom, crop.Left, bounds.Top);
					graphics.RenderSolid(adorner.ColorCaption);
					graphics.LineWidth = 1;
				}

				if (this.hilited == Part.Right || this.hilited == Part.BottomRight || this.hilited == Part.TopRight)
				{
					graphics.LineWidth = 3;
					graphics.AddLine(crop.Right, bounds.Bottom, crop.Right, bounds.Top);
					graphics.RenderSolid(adorner.ColorCaption);
					graphics.LineWidth = 1;
				}

				if (this.hilited == Part.Bottom || this.hilited == Part.BottomLeft || this.hilited == Part.BottomRight)
				{
					graphics.LineWidth = 3;
					graphics.AddLine(bounds.Left, crop.Bottom, bounds.Right, crop.Bottom);
					graphics.RenderSolid(adorner.ColorCaption);
					graphics.LineWidth = 1;
				}

				if (this.hilited == Part.Top || this.hilited == Part.TopLeft || this.hilited == Part.TopRight)
				{
					graphics.LineWidth = 3;
					graphics.AddLine(bounds.Left, crop.Top, bounds.Right, crop.Top);
					graphics.RenderSolid(adorner.ColorCaption);
					graphics.LineWidth = 1;
				}

				if (this.hilited == Part.Showed)
				{
					graphics.AddFilledRectangle(crop);
					graphics.RenderSolid(adorner.ColorCaption);
				}
			}
		}


		private void HandleFieldChanged(object sender)
		{
			//	Une valeur numérique éditable a été changée.
			if (this.ignoreChanged)
			{
				return;
			}

			TextFieldReal field = sender as TextFieldReal;
			Margins crop = this.Crop;

			if (field == this.fieldCropLeft)
			{
				crop.Left = (double) field.InternalValue;
			}

			if (field == this.fieldCropRight)
			{
				crop.Right = (double) field.InternalValue;
			}

			if (field == this.fieldCropBottom)
			{
				crop.Bottom = (double) field.InternalValue;
			}

			if (field == this.fieldCropTop)
			{
				crop.Top = (double) field.InternalValue;
			}

			this.Crop = crop;
		}

		private void HandleButtonReset(object sender, MessageEventArgs e)
		{
			//	Le bouton "T" a été actionné.
			this.Crop = Margins.Zero;
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

		protected TextFieldReal				fieldCropLeft;
		protected TextFieldReal				fieldCropRight;
		protected TextFieldReal				fieldCropBottom;
		protected TextFieldReal				fieldCropTop;
		protected Button					buttonReset;

		protected bool						ignoreChanged = false;
		protected bool						mouseDown = false;
		protected Part						hilited = Part.None;
		protected Point						initialPos;
		protected Margins					initialCrop;
	}
}
