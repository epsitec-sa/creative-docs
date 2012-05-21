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
			this.InternalState |= WidgetInternalState.Engageable;

			int tabIndex = 0;

			this.title = new StaticText(this);
			this.title.Text = Res.Strings.Panel.Image.Crop.Title;

			this.buttonReset = new Button(this);
			this.buttonReset.Text = Res.Strings.Panel.Image.Crop.Button.Reset;
			this.buttonReset.TabIndex = tabIndex++;
			this.buttonReset.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.buttonReset.Clicked += this.HandleButtonReset;
			ToolTip.Default.SetToolTip(this.buttonReset, Res.Strings.Panel.Image.Crop.Tooltip.Reset);

			this.buttonFill = new Button(this);
			this.buttonFill.Text = Res.Strings.Panel.Image.Crop.Button.Fill;
			this.buttonFill.TabIndex = tabIndex++;
			this.buttonFill.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.buttonFill.Clicked += this.HandleButtonFill;
			ToolTip.Default.SetToolTip(this.buttonFill, Res.Strings.Panel.Image.Crop.Tooltip.Fill);

			this.fieldCropTop = new TextFieldReal(this);
			Cropper.InitTextFieldReal(this.fieldCropTop);
			this.fieldCropTop.EditionAccepted += this.HandleFieldChanged;
			this.fieldCropTop.TabIndex = tabIndex++;
			this.fieldCropTop.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldCropTop, Res.Strings.Panel.Image.Crop.Tooltip.CropTop);

			this.fieldCropLeft = new TextFieldReal(this);
			Cropper.InitTextFieldReal(this.fieldCropLeft);
			this.fieldCropLeft.EditionAccepted += this.HandleFieldChanged;
			this.fieldCropLeft.TabIndex = tabIndex++;
			this.fieldCropLeft.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldCropLeft, Res.Strings.Panel.Image.Crop.Tooltip.CropLeft);

			this.fieldCropRight = new TextFieldReal(this);
			Cropper.InitTextFieldReal(this.fieldCropRight);
			this.fieldCropRight.EditionAccepted += this.HandleFieldChanged;
			this.fieldCropRight.TabIndex = tabIndex++;
			this.fieldCropRight.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldCropRight, Res.Strings.Panel.Image.Crop.Tooltip.CropRight);

			this.fieldCropBottom = new TextFieldReal(this);
			Cropper.InitTextFieldReal(this.fieldCropBottom);
			this.fieldCropBottom.EditionAccepted += this.HandleFieldChanged;
			this.fieldCropBottom.TabIndex = tabIndex++;
			this.fieldCropBottom.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldCropBottom, Res.Strings.Panel.Image.Crop.Tooltip.CropBottom);

			this.sliderZoom = new VSlider(this);
			this.sliderZoom.MinValue = 1.0M;
			this.sliderZoom.MaxValue = 4.0M;
			this.sliderZoom.SmallChange = 0.1M;
			this.sliderZoom.LargeChange = 0.5M;
			this.sliderZoom.Resolution = 0.000000001M;
			this.sliderZoom.ValueChanged += this.HandleSliderZoomChanged;
			this.sliderZoom.TabIndex = tabIndex++;
			this.sliderZoom.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.sliderZoom, Res.Strings.Panel.Image.Crop.Tooltip.Zoom);
		}

		public Cropper(Widget embedder)
			: this()
		{
			this.SetEmbedder(embedder);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.fieldCropLeft.EditionAccepted -= this.HandleFieldChanged;
				this.fieldCropRight.EditionAccepted -= this.HandleFieldChanged;
				this.fieldCropBottom.EditionAccepted -= this.HandleFieldChanged;
				this.fieldCropTop.EditionAccepted -= this.HandleFieldChanged;

				this.buttonReset.Clicked -= this.HandleButtonReset;
				this.buttonFill.Clicked -= this.HandleButtonFill;
				this.sliderZoom.ValueChanged -= this.HandleSliderZoomChanged;

				this.fieldCropLeft = null;
				this.fieldCropRight = null;
				this.fieldCropBottom = null;
				this.fieldCropTop = null;
				this.buttonReset = null;
				this.buttonFill = null;
				this.sliderZoom = null;
			}

			base.Dispose(disposing);
		}


		public Document Document
		{
			//	Document contenant l'image sélectionnée dont on modifie le cadrage.
			get
			{
				return this.document;
			}
			set
			{
				this.document = value;
			}
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
					this.cropBeforeZoom = value;
					this.UpdateField();
					this.InvalidateBounds();
					this.OnCropChanged();
				}
			}
		}


		protected bool IsSingleSelection
		{
			//	Indique si un seul objet est sélectionné.
			get
			{
				return (this.document.Modifier.TotalSelected == 1);
			}
		}

		protected Objects.Image SelectedObjectImage
		{
			//	Retourne l'objet Image sélectionné dans le document.
			get
			{
				Objects.Abstract objectSelected = this.document.Modifier.RetFirstSelectedObject();
				if (objectSelected != null)
				{
					return objectSelected as Objects.Image;
				}

				return null;
			}
		}

		protected Properties.Image SelectedPropertyImage
		{
			//	Retourne la propriété Image de l'objet sélectionné dans le document.
			get
			{
				Objects.Image objectImage = this.SelectedObjectImage;
				if (objectImage != null)
				{
					return objectImage.PropertyImage;
				}

				return null;
			}
		}

		protected Size ImageSize
		{
			//	Taille de l'image.
			//	Si la taille de l'image est inconnue, retourne une taille
			//	par défaut arbitraire de 1000 x 1000.
			get
			{
				Properties.Image pi = this.SelectedPropertyImage;
				if (pi != null)
				{
					ImageCache.Item item = this.document.ImageCache.Find(pi.FileName, pi.FileDate);
					if (item != null)
					{
						return item.Size;
					}
				}

				return new Size(1000, 1000);
			}
		}

		protected Size ObjectSize
		{
			//	Taille de l'objet Image dans le document.
			//	Si la taille de l'objet est inconnue, retourne une taille
			//	par défaut arbitraire de 1000 x 1000.
			get
			{
				Objects.Image objectImage = this.SelectedObjectImage;
				if (objectImage != null)
				{
					return objectImage.ImageBitmapMaxFill;
				}

				return new Size(1000, 1000);
			}
		}

		protected bool IsFillEnabled
		{
			//	Indique si le bouton 'fill' est activé.
			get
			{
				Properties.Image pi = this.SelectedPropertyImage;
				
				if (pi == null || !pi.Homo)
				{
					return false;
				}

				Size imageSize = this.ImageSize;
				Size objectSize = this.ObjectSize;

				Rectangle rect = new Rectangle(Point.Zero, imageSize);
				rect.Deflate(this.crop);

				return System.Math.Abs(rect.Width/rect.Height - objectSize.Width/objectSize.Height) > 0.0001;
			}
		}

		public void UpdateField()
		{
			//	Met à jour tous les widgets, suite à une modification.
			this.ignoreChanged = true;

			this.Enable = this.IsSingleSelection;

			Size size = Size.Zero;
			Margins crop = Margins.Zero;
			bool isFillEnabled = false;
			double zoom = 1.0;

			if (this.Enable)
			{
				size = this.ImageSize;
				crop = this.crop;
				isFillEnabled = this.IsFillEnabled;

				double dx = size.Width-crop.Width;
				double dy = size.Height-crop.Height;

				double zoomx = dx > 0 ? size.Width/dx : 1;
				double zoomy = dy > 0 ? size.Height/dy : 1;
				zoom = System.Math.Min(zoomx, zoomy);
			}

			this.fieldCropLeft.MaxValue = (decimal) size.Width;
			this.fieldCropRight.MaxValue = (decimal) size.Width;
			this.fieldCropBottom.MaxValue = (decimal) size.Height;
			this.fieldCropTop.MaxValue = (decimal) size.Height;

			this.fieldCropLeft.InternalValue = (decimal) crop.Left;
			this.fieldCropRight.InternalValue = (decimal) crop.Right;
			this.fieldCropBottom.InternalValue = (decimal) crop.Bottom;
			this.fieldCropTop.InternalValue = (decimal) crop.Top;

			this.buttonReset.Enable = (crop != Margins.Zero);
			this.buttonFill.Enable = isFillEnabled;
			this.sliderZoom.Value = (decimal) zoom;
			
			this.ignoreChanged = false;
		}


		protected Rectangle BoundsRectangle
		{
			//	Retourne la zone pour la partie interactive.
			get
			{
				Rectangle rect = this.Client.Bounds;
				rect.Deflate(0, 0, 30, 5);
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
				crop = this.CropRotate(crop, 1);
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

		protected Point ConvWidgetToImage(Point mouse, Size imageSize)
		{
			//	Conversion d'une position de la souris dans un widget en position dans l'image.
			Rectangle bounds = this.BoundsRectangle;
			mouse -= bounds.BottomLeft;
			Point pos = new Point(mouse.X/bounds.Width, mouse.Y/bounds.Height);

			pos.X = System.Math.Max(pos.X, 0);
			pos.X = System.Math.Min(pos.X, 1);
			pos.Y = System.Math.Max(pos.Y, 0);
			pos.Y = System.Math.Min(pos.Y, 1);

			return new Point(pos.X*imageSize.Width, pos.Y*imageSize.Height);
		}

		protected Part DetectPart(Point pos)
		{
			//	Détecte la partie survolée par la souris.
			Rectangle crop = this.CropRectangle;
			double m = 2;  // marge de détection

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

			Size size = this.ImageSize;
			int angle = this.AngleRotate(1);

			if (angle == 90 || angle == 270)
			{
				double t = size.Width;
				size.Width = size.Height;
				size.Height = t;
			}

			pos = this.ConvWidgetToImage(pos, size);
			Margins crop = this.CropRotate(this.Crop, 1);

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
				Point move = pos - this.ConvWidgetToImage(this.initialPos, size);

				if (isConstrain)
				{
					if (isHorizontal)  move.Y = 0;
					else               move.X = 0;
				}

				crop = this.CropRotate(this.initialCrop, 1);
				crop.Left   += move.X;
				crop.Right  -= move.X;
				crop.Bottom += move.Y;
				crop.Top    -= move.Y;
				crop = Cropper.CropAdjust (crop);
			}

			this.Crop = this.CropRotate(crop, -1);
		}

		static Margins CropAdjust(Margins crop)
		{
			//	Ajuste le recadrage pour n'avoir jamais de marges négatives.
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

			return crop;
		}

		protected Margins CropFillObject(Margins crop)
		{
			//	Ajuste le recadrage pour remplir tout l'objet.
			return Cropper.CropFillObject (crop, this.ObjectSize, this.ImageSize);
		}

		public static Margins CropFillObject(Margins crop, Size objectSize, Size imageSize)
		{
			Rectangle rect = new Rectangle(Point.Zero, imageSize);
			rect.Deflate(crop);
			double w = rect.Width;
			double h = rect.Height;

			if (rect.Width/rect.Height > objectSize.Width/objectSize.Height)
			{
				h = w/(objectSize.Width/objectSize.Height);

				if (h > imageSize.Height)
				{
					w /= h/imageSize.Height;
					h = imageSize.Height;
				}
			}
			else if (rect.Width/rect.Height < objectSize.Width/objectSize.Height)
			{
				w = h*(objectSize.Width/objectSize.Height);

				if (w > imageSize.Width)
				{
					h /= w/imageSize.Width;
					w = imageSize.Width;
				}
			}

			crop.Left   = rect.Center.X-w/2;
			crop.Right  = imageSize.Width-(rect.Center.X+w/2);
			crop.Bottom = rect.Center.Y-h/2;
			crop.Top    = imageSize.Height-(rect.Center.Y+h/2);
			crop = Cropper.CropAdjust(crop);

			return crop;
		}

		protected int AngleRotate(int direction)
		{
			//	Retourne l'angle de la rotation du recadrage, selon les propriétés de l'image.
			//	direction =  1  ->  rotation CCW (normale)
			//	direction = -1  ->  rotation CW
			int angle = 0;

			Properties.Image pi = this.SelectedPropertyImage;
			if (pi != null)
			{
				if (pi.RotationMode == Properties.Image.Rotation.Angle90 )  angle =  90;
				if (pi.RotationMode == Properties.Image.Rotation.Angle180)  angle = 180;
				if (pi.RotationMode == Properties.Image.Rotation.Angle270)  angle = 270;

				angle *= direction;  // inverse éventuellement le sens
				if (angle < 0)
				{
					angle = 360+angle;  // 0..270
				}
			}

			return angle;
		}

		protected Margins CropRotate(Margins crop, int direction)
		{
			//	Effectue une rotation du recadrage, selon les propriétés de l'image.
			int angle = this.AngleRotate(direction);
			Margins icrop = crop;

			if (angle == 90)  // quart de tour à gauche ?
			{
				crop.Left   = icrop.Top;
				crop.Right  = icrop.Bottom;
				crop.Bottom = icrop.Left;
				crop.Top    = icrop.Right;
			}

			if (angle == 180)  // demi-tour ?
			{
				crop.Left   = icrop.Right;
				crop.Right  = icrop.Left;
				crop.Bottom = icrop.Top;
				crop.Top    = icrop.Bottom;
			}

			if (angle == 270)  // quart de tour à droite ?
			{
				crop.Left   = icrop.Bottom;
				crop.Right  = icrop.Top;
				crop.Bottom = icrop.Right;
				crop.Top    = icrop.Left;
			}

			return crop;
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
					this.UpdateCursorPart();
					this.InvalidateBounds();
				}
			}
		}

		protected void UpdateCursorPart()
		{
			//	Utilise le curseur de souris adéquat en fonction de la partie hilited.
			switch (this.hilited)
			{
				case Part.Left:
				case Part.Right:
					this.MouseCursor = MouseCursor.AsSizeWE;
					break;

				case Part.Bottom:
				case Part.Top:
					this.MouseCursor = MouseCursor.AsSizeNS;
					break;

				case Part.BottomLeft:
				case Part.TopRight:
					this.MouseCursor = MouseCursor.AsSizeNESW;
					break;

				case Part.BottomRight:
				case Part.TopLeft:
					this.MouseCursor = MouseCursor.AsSizeNWSE;
					break;

				case Part.Showed:
					this.MouseCursor = MouseCursor.AsSizeAll;
					break;

				default:
					this.MouseCursor = MouseCursor.AsArrow;
					break;
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
			switch (message.MessageType)
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

			if ( this.title == null )  return;

			Rectangle rect = this.Client.Bounds;
			Rectangle r;

			if (rect.Width < 160 || rect.Height < 105)
			{
				this.title.Visibility = false;
				this.buttonReset.Visibility = false;
				this.buttonFill.Visibility = false;
				this.fieldCropLeft.Visibility = false;
				this.fieldCropRight.Visibility = false;
				this.fieldCropBottom.Visibility = false;
				this.fieldCropTop.Visibility = false;
				this.sliderZoom.Visibility = false;
			}
			else
			{
				this.title.Visibility = true;
				this.buttonReset.Visibility = true;
				this.buttonFill.Visibility = true;
				this.fieldCropLeft.Visibility = true;
				this.fieldCropRight.Visibility = true;
				this.fieldCropBottom.Visibility = true;
				this.fieldCropTop.Visibility = true;
				this.sliderZoom.Visibility = true;

				rect = this.Client.Bounds;
				rect.Top -= 5;
				rect.Bottom = rect.Top-20;

				double w = System.Math.Floor((rect.Width-10)/3);
				r = rect;
				r.Width = w;
				this.title.SetManualBounds(r);
				r.Offset(w+5, 0);
				this.buttonReset.SetManualBounds(r);
				r.Offset(w+5, 0);
				r.Right = rect.Right;
				this.buttonFill.SetManualBounds(r);

				rect = this.Client.Bounds;
				rect.Deflate(0, 0, 30, 5);
				rect.Width -= rect.Height+12+5;  // place pour la partie interactive de droite

				r = rect;
				r.Left = rect.Right+3;
				r.Right = rect.Right+3+12;
				this.sliderZoom.SetManualBounds(r);

				w = System.Math.Floor((rect.Width-5)/2);
				r = rect;
				r.Bottom = r.Top-20;

				r.Left = rect.Left+w/2+2;
				r.Width = w;
				this.fieldCropTop.SetManualBounds(r);

				r.Offset(0, -25);
				r.Left = rect.Left;
				r.Width = w;
				this.fieldCropLeft.SetManualBounds(r);
				r.Offset(w+5, 0);
				this.fieldCropRight.SetManualBounds(r);

				r.Offset(0, -25);
				r.Left = rect.Left+w/2+2;
				r.Width = w;
				this.fieldCropBottom.SetManualBounds(r);
			}
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			//	Dessine la partie interactive de droite.
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			if (this.Enable)
			{
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

			Rectangle rect = this.Client.Bounds;
			rect.Deflate(0.5);
			graphics.AddLine(rect.TopLeft, rect.TopRight);
			graphics.AddLine(rect.BottomLeft, rect.BottomRight);
			graphics.RenderSolid(adorner.ColorBorder);
		}


		static protected void InitTextFieldReal(TextFieldReal field)
		{
			field.UnitType = RealUnitType.Scalar;
			field.MinValue = (decimal) 0;
			field.Step = 1.0M;
			field.Resolution = 1.0M;
			field.DefocusAction = DefocusAction.AutoAcceptOrRejectEdition;
			field.AutoSelectOnFocus = true;
			field.SwallowEscapeOnRejectEdition = true;
			field.SwallowReturnOnAcceptEdition = true;
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
			//	Le bouton "Tout" a été actionné.
			this.Crop = Margins.Zero;
		}

		private void HandleButtonFill(object sender, MessageEventArgs e)
		{
			//	Le bouton "Remplir" a été actionné.
			this.Crop = this.CropFillObject(this.crop);
		}

		private void HandleSliderZoomChanged(object sender)
		{
			//	Le slider pour le zoom a été déplacé.
			if (this.ignoreChanged)
			{
				return;
			}

			double zoom = (double) this.sliderZoom.Value;  // zoom souhaité

			this.CropZoom (zoom);
		}

		protected void CropZoom(double zoom)
		{
			Size size = this.ImageSize;  // taille de l'image sélectionnée
			Margins crop = this.cropBeforeZoom;  // crop avant de commencer à zoomer

			Rectangle rect = new Rectangle(Point.Zero, size);
			rect.Deflate(crop);  // rectangle effectif actuel

			double zoomx = size.Width/rect.Width;  // zoom horizontal actuel
			double zoomy = size.Height/rect.Height;  // zoom vertical actuel

			double w, h;

			if (zoomx < zoomy)
			{
				w = size.Width/zoom;  // nouvelle largeur
				h = w*rect.Height/rect.Width;  // garde les mêmes proportions
			}
			else
			{
				h = size.Height/zoom;  // nouvelle hauteur
				w = h*rect.Width/rect.Height;  // garde les mêmes proportions
			}

			Margins newCrop = Margins.Zero;
			newCrop.Left   = rect.Center.X-w/2;
			newCrop.Right  = size.Width-(rect.Center.X+w/2);
			newCrop.Bottom = rect.Center.Y-h/2;
			newCrop.Top    = size.Height-(rect.Center.Y+h/2);
			newCrop = Cropper.CropAdjust (newCrop);

			Margins cbz = this.cropBeforeZoom;
			this.Crop = newCrop;
			this.cropBeforeZoom = cbz;  // conserve le crop avant de commencer à zoomer
		}

		protected virtual void OnCropChanged()
		{
			//	Génère un événement pour dire que l'offset a changé.
			var handler = this.GetUserEventHandler("CropChanged");
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
		
		
		protected Document					document;
		protected Margins					crop = Margins.NaN;

		protected StaticText				title;
		protected Button					buttonReset;
		protected Button					buttonFill;
		protected TextFieldReal				fieldCropLeft;
		protected TextFieldReal				fieldCropRight;
		protected TextFieldReal				fieldCropBottom;
		protected TextFieldReal				fieldCropTop;
		protected VSlider					sliderZoom;

		protected bool						ignoreChanged;
		protected bool						mouseDown;
		protected Part						hilited;
		protected Point						initialPos;
		protected Margins					initialCrop;
		protected Margins					cropBeforeZoom;
	}
}
