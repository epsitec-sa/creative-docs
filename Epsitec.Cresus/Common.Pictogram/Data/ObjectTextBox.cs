using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe ObjectTextBox est la classe de l'objet graphique "pavé de texte".
	/// </summary>
	public class ObjectTextBox : AbstractObject
	{
		public ObjectTextBox()
		{
			PropertyName name = new PropertyName();
			name.Type = PropertyType.Name;
			this.AddProperty(name);

			PropertyLine lineMode = new PropertyLine();
			lineMode.Type = PropertyType.LineMode;
			this.AddProperty(lineMode);

			PropertyColor lineColor = new PropertyColor();
			lineColor.Type = PropertyType.LineColor;
			this.AddProperty(lineColor);

			PropertyGradient fillGradient = new PropertyGradient();
			fillGradient.Type = PropertyType.FillGradient;
			this.AddProperty(fillGradient);

			PropertyString textString = new PropertyString();
			textString.Type = PropertyType.TextString;
			this.AddProperty(textString);

			PropertyFont textFont = new PropertyFont();
			textFont.Type = PropertyType.TextFont;
			this.AddProperty(textFont);

			PropertyJustif textJustif = new PropertyJustif();
			textJustif.Type = PropertyType.TextJustif;
			this.AddProperty(textJustif);
		}

		protected override AbstractObject CreateNewObject()
		{
			return new ObjectTextBox();
		}

		public override void Dispose()
		{
			base.Dispose();
		}


		// Nom de l'icône.
		public override string IconName
		{
			get { return @"file:images/textbox.icon"; }
		}


		// Détecte si la souris est sur l'objet.
		public override bool Detect(Drawing.Point pos)
		{
			if ( this.isHide )  return false;

			Drawing.Rectangle bbox = this.BoundingBox;
			if ( !bbox.Contains(pos) )  return false;

			Drawing.Path path = this.PathBuild();

			double width = this.PropertyLine(1).Width/2;
			if ( width > 0 && AbstractObject.DetectOutline(path, width, pos) )  return true;
			
			if ( AbstractObject.DetectSurface(path, pos) )  return true;

			return false;
		}


		// Détecte si la souris est sur l'objet pour l'éditer.
		public override bool DetectEdit(Drawing.Point pos)
		{
			if ( this.isHide )  return false;

			Drawing.Rectangle bbox = this.BoundingBox;
			if ( !bbox.Contains(pos) )  return false;

			Drawing.Path path = this.PathBuild();
			return AbstractObject.DetectSurface(path, pos);
		}


		// Déplace une poignée.
		public override void MoveHandleProcess(int rank, Drawing.Point pos, IconContext iconContext)
		{
			if ( rank >= this.handles.Count )  // poignée d'une propriété ?
			{
				base.MoveHandleProcess(rank, pos, iconContext);
				return;
			}

			iconContext.ConstrainSnapPos(ref pos);
			iconContext.SnapGrid(ref pos);

			if ( AbstractObject.IsRectangular(this.Handle(0).Position, this.Handle(1).Position, this.Handle(2).Position, this.Handle(3).Position) )
			{
				this.Handle(rank).Position = pos;

				if ( rank == 0 )
				{
					this.Handle(2).Position = Drawing.Point.Projection(this.Handle(2).Position, this.Handle(1).Position, pos);
					this.Handle(3).Position = Drawing.Point.Projection(this.Handle(3).Position, this.Handle(1).Position, pos);
				}
				if ( rank == 1 )
				{
					this.Handle(2).Position = Drawing.Point.Projection(this.Handle(2).Position, this.Handle(0).Position, pos);
					this.Handle(3).Position = Drawing.Point.Projection(this.Handle(3).Position, this.Handle(0).Position, pos);
				}
				if ( rank == 2 )
				{
					this.Handle(0).Position = Drawing.Point.Projection(this.Handle(0).Position, this.Handle(3).Position, pos);
					this.Handle(1).Position = Drawing.Point.Projection(this.Handle(1).Position, this.Handle(3).Position, pos);
				}
				if ( rank == 3 )
				{
					this.Handle(0).Position = Drawing.Point.Projection(this.Handle(0).Position, this.Handle(2).Position, pos);
					this.Handle(1).Position = Drawing.Point.Projection(this.Handle(1).Position, this.Handle(2).Position, pos);
				}
			}
			else
			{
				this.Handle(rank).Position = pos;
			}
			this.dirtyBbox = true;
		}

		// Début de la création d'un objet.
		public override void CreateMouseDown(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainFixStarting(pos, ConstrainType.Square);
			this.HandleAdd(pos, HandleType.Primary);  // rang = 0
			this.HandleAdd(pos, HandleType.Primary);  // rang = 1
		}

		// Déplacement pendant la création d'un objet.
		public override void CreateMouseMove(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainSnapPos(ref pos);
			iconContext.SnapGrid(ref pos);
			this.Handle(1).Position = pos;
			this.dirtyBbox = true;
		}

		// Fin de la création d'un objet.
		public override void CreateMouseUp(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainSnapPos(ref pos);
			iconContext.SnapGrid(ref pos);
			this.Handle(1).Position = pos;
			iconContext.ConstrainDelStarting();

			// Crée les 2 autres poignées dans les coins opposés.
			Drawing.Rectangle rect = Drawing.Rectangle.FromCorners(this.Handle(0).Position, this.Handle(1).Position);
			Drawing.Point p1 = rect.BottomLeft;
			Drawing.Point p2 = rect.TopRight;
			this.Handle(0).Position = p1;
			this.Handle(1).Position = p2;
			this.HandleAdd(new Drawing.Point(p1.X, p2.Y), HandleType.Primary);  // rang = 2
			this.HandleAdd(new Drawing.Point(p2.X, p1.Y), HandleType.Primary);  // rang = 3
		}

		// Indique si l'objet doit exister. Retourne false si l'objet ne peut
		// pas exister et doit être détruit.
		public override bool CreateIsExist(IconContext iconContext)
		{
			double len = Drawing.Point.Distance(this.Handle(0).Position, this.Handle(1).Position);
			return ( len > this.minimalSize );
		}

		// Indique s'il faut sélectionner l'objet après sa création.
		public override bool EditAfterCreation()
		{
			return true;
		}


		// Indique si un objet est éditable.
		public override bool IsEditable()
		{
			return true;
		}


		// Gestion d'un événement pendant l'édition.
		public override bool EditProcessMessage(Message message, Drawing.Point pos)
		{
			if ( this.transform == null )  return false;

			pos = this.transform.TransformInverse(pos);
			if ( !this.textNavigator.ProcessMessage(message, pos) )  return false;

			this.PropertyString(4).String = this.textLayout.Text;
			return true;
		}

		// Gestion d'un événement pendant l'édition.
		public override void EditMouseDownMessage(Drawing.Point pos)
		{
			pos = this.transform.TransformInverse(pos);
			this.textNavigator.MouseDownMessage(pos);
		}


		// Met à jour le rectangle englobant l'objet.
		protected override void UpdateBoundingBox()
		{
			Drawing.Path path = this.PathBuild();
			this.bboxThin = path.ComputeBounds();

			this.bboxGeom = this.bboxThin;
			this.PropertyLine(1).InflateBoundingBox(ref this.bboxGeom);

			this.bboxFull = this.bboxGeom;
			this.bboxGeom.MergeWith(this.PropertyGradient(3).BoundingBoxGeom(this.bboxThin));
			this.bboxFull.MergeWith(this.PropertyGradient(3).BoundingBoxFull(this.bboxThin));
			this.bboxFull.MergeWith(this.bboxGeom);
		}

		// Crée le chemin de l'objet.
		protected Drawing.Path PathBuild()
		{
			Drawing.Point p1 = this.Handle(0).Position;
			Drawing.Point p2 = new Drawing.Point();
			Drawing.Point p3 = this.Handle(1).Position;
			Drawing.Point p4 = new Drawing.Point();

			if ( this.handles.Count < 4 )
			{
				p2.X = p1.X;
				p2.Y = p3.Y;
				p4.X = p3.X;
				p4.Y = p1.Y;
			}
			else
			{
				p2 = this.Handle(2).Position;
				p4 = this.Handle(3).Position;
			}

			Drawing.Path path = new Drawing.Path();
			path.MoveTo(p1);
			path.LineTo(p2);
			path.LineTo(p3);
			path.LineTo(p4);
			path.Close();
			return path;
		}

		// Dessine le texte du pavé.
		protected void DrawText(Drawing.Graphics graphics, IconContext iconContext)
		{
			string text = this.PropertyString(4).String;

			if ( this.textLayout == null )
			{
				this.textLayout = new TextLayout();
				this.textNavigator = new TextNavigator(this.textLayout);
				this.textLayout.BreakMode = Drawing.TextBreakMode.Hyphenate;
			}
			this.textLayout.Text = text;

			Drawing.Point p1, p2, p3, p4;
			switch ( this.PropertyJustif(6).Orientation )
			{
				case JustifOrientation.RightToLeft:  // <-
					p1 = this.Handle(1).Position;
					p2 = this.Handle(2).Position;
					p3 = this.Handle(3).Position;
					p4 = this.Handle(0).Position;
					break;
				case JustifOrientation.BottomToTop:  // ^
					p1 = this.Handle(3).Position;
					p2 = this.Handle(1).Position;
					p3 = this.Handle(0).Position;
					p4 = this.Handle(2).Position;
					break;
				case JustifOrientation.TopToBottom:  // v
					p1 = this.Handle(2).Position;
					p2 = this.Handle(0).Position;
					p3 = this.Handle(1).Position;
					p4 = this.Handle(3).Position;
					break;
				default:							// -> (normal)
					p1 = this.Handle(0).Position;
					p2 = this.Handle(3).Position;
					p3 = this.Handle(2).Position;
					p4 = this.Handle(1).Position;
					break;
			}
			if ( !this.PropertyJustif(6).DeflateBox(ref p1, ref p2, ref p3, ref p4) )  return;

			Drawing.Size size = new Drawing.Size();
			size.Width  = Drawing.Point.Distance(p1,p2);
			size.Height = Drawing.Point.Distance(p1,p3);
			this.textLayout.LayoutSize = size;

			this.textLayout.Font     = this.PropertyFont(5).GetFont();
			this.textLayout.FontSize = this.PropertyFont(5).FontSize;

			JustifVertical   jv = this.PropertyJustif(6).Vertical;
			JustifHorizontal jh = this.PropertyJustif(6).Horizontal;

			if ( jv == JustifVertical.Top )
			{
				if ( jh == JustifHorizontal.Left   )  this.textLayout.Alignment = Drawing.ContentAlignment.TopLeft;
				if ( jh == JustifHorizontal.Center )  this.textLayout.Alignment = Drawing.ContentAlignment.TopCenter;
				if ( jh == JustifHorizontal.Right  )  this.textLayout.Alignment = Drawing.ContentAlignment.TopRight;
			}
			if ( jv == JustifVertical.Center )
			{
				if ( jh == JustifHorizontal.Left   )  this.textLayout.Alignment = Drawing.ContentAlignment.MiddleLeft;
				if ( jh == JustifHorizontal.Center )  this.textLayout.Alignment = Drawing.ContentAlignment.MiddleCenter;
				if ( jh == JustifHorizontal.Right  )  this.textLayout.Alignment = Drawing.ContentAlignment.MiddleRight;
			}
			if ( jv == JustifVertical.Bottom )
			{
				if ( jh == JustifHorizontal.Left   )  this.textLayout.Alignment = Drawing.ContentAlignment.BottomLeft;
				if ( jh == JustifHorizontal.Center )  this.textLayout.Alignment = Drawing.ContentAlignment.BottomCenter;
				if ( jh == JustifHorizontal.Right  )  this.textLayout.Alignment = Drawing.ContentAlignment.BottomRight;
			}

			Drawing.Transform ot = graphics.SaveTransform();

			double angle = Drawing.Point.ComputeAngle(p1, p2);
			angle *= 180.0/System.Math.PI;  // radians -> degrés
			this.transform = new Drawing.Transform();
			transform.Translate(p1);
			transform.Rotate(angle, p1);
			graphics.MergeTransform(transform);

			if ( this.edited && this.textNavigator.Context.CursorFrom != this.textNavigator.Context.CursorTo )
			{
				int from = System.Math.Min(this.textNavigator.Context.CursorFrom, this.textNavigator.Context.CursorTo);
				int to   = System.Math.Max(this.textNavigator.Context.CursorFrom, this.textNavigator.Context.CursorTo);
				Drawing.Rectangle[] rects = this.textLayout.FindTextRange(new Drawing.Point(0,0), from, to);
				for ( int i=0 ; i<rects.Length ; i++ )
				{
					graphics.Align(ref rects[i]);
					graphics.AddFilledRectangle(rects[i]);
					graphics.RenderSolid(IconContext.ColorSelectEdit);
				}
			}

			Drawing.Color color = iconContext.AdaptColor(this.PropertyFont(5).FontColor);
			this.textLayout.Paint(new Drawing.Point(0,0), graphics, Drawing.Rectangle.Empty, color, Drawing.GlyphPaintStyle.Normal);

			if ( this.edited && this.textNavigator.Context.CursorTo != -1 )
			{
				Drawing.Rectangle rect = this.textLayout.FindTextCursor(this.textNavigator.Context.CursorTo, out this.textNavigator.Context.CursorLine);
				if ( !rect.IsEmpty )
				{
					this.textNavigator.Context.CursorPosX = (rect.Left+rect.Right)/2;

					rect.Left  -= 0.5/iconContext.ScaleX;
					rect.Right += 0.5/iconContext.ScaleX;
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(IconContext.ColorFrameEdit);
				}
			}

			graphics.Transform = ot;
		}

		// Dessine l'objet.
		public override void DrawGeometry(Drawing.Graphics graphics, IconContext iconContext, IconObjects iconObjects)
		{
			if ( base.IsFullHide(iconContext) )  return;
			base.DrawGeometry(graphics, iconContext, iconObjects);

			if ( this.TotalHandle < 2 )  return;

			Drawing.Path path = this.PathBuild();
			this.PropertyGradient(3).Render(graphics, iconContext, path, this.BoundingBoxThin);

			graphics.Rasterizer.AddOutline(path, this.PropertyLine(1).Width, this.PropertyLine(1).Cap, this.PropertyLine(1).Join, this.PropertyLine(1).Limit);
			graphics.RenderSolid(iconContext.AdaptColor(this.PropertyColor(2).Color));

			if ( this.TotalHandle >= 4 )
			{
				this.DrawText(graphics, iconContext);
			}

			if ( this.edited && iconContext.IsEditable )  // en cours d'édition ?
			{
				graphics.Rasterizer.AddOutline(path, 2.0/iconContext.ScaleX);
				graphics.RenderSolid(IconContext.ColorFrameEdit);
			}
			else
			{
				if ( this.IsHilite && iconContext.IsEditable )
				{
					if ( !this.edited )
					{
						graphics.Rasterizer.AddSurface(path);
						graphics.RenderSolid(iconContext.HiliteSurfaceColor);
					}
					graphics.Rasterizer.AddOutline(path, this.PropertyLine(1).Width, this.PropertyLine(1).Cap, this.PropertyLine(1).Join, this.PropertyLine(1).Limit);
					graphics.RenderSolid(iconContext.HiliteOutlineColor);
				}
			}
		}


		protected TextLayout			textLayout;
		protected TextNavigator			textNavigator;
		protected int					cursorFrom = -1;
		protected int					cursorTo   = -1;
		protected Drawing.Transform		transform;
	}
}
