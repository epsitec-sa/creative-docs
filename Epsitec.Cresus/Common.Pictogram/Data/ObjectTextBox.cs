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

			PropertyJustif textJustif = new PropertyJustif();
			textJustif.Type = PropertyType.TextJustif;
			this.AddProperty(textJustif);

			PropertyFont font = new PropertyFont();
			font.Type = PropertyType.TextFont;
			this.AddProperty(font);

			this.textLayout = new TextLayout();
			this.textNavigator = new TextNavigator(this.textLayout);
			this.textLayout.BreakMode = Drawing.TextBreakMode.Hyphenate;
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


		public string Content
		{
			get
			{
				return this.textLayout.Text;
			}

			set
			{
				this.textLayout.Text = value;
			}
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

		// Lie l'objet éditable à une règle.
		public override void EditRulerLink(TextRuler ruler)
		{
			ruler.AttachToText(this.textLayout, this.textNavigator);
		}


		// Reprend toutes les caractéristiques d'un objet.
		public override void CloneObject(AbstractObject src)
		{
			base.CloneObject(src);

			ObjectTextBox obj = src as ObjectTextBox;
			this.textLayout.Text = obj.textLayout.Text;
		}


		// Gestion d'un événement pendant l'édition.
		public override bool EditProcessMessage(Message message, Drawing.Point pos)
		{
			if ( this.transform == null )  return false;

			pos = this.transform.TransformInverse(pos);
			if ( !this.textNavigator.ProcessMessage(message, pos) )  return false;
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
		protected void DrawText(Drawing.IPaintPort port, IconContext iconContext)
		{
			Drawing.Point p1, p2, p3, p4;
			switch ( this.PropertyJustif(4).Orientation )
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
			if ( !this.PropertyJustif(4).DeflateBox(ref p1, ref p2, ref p3, ref p4) )  return;

			Drawing.Size size = new Drawing.Size();
			size.Width  = Drawing.Point.Distance(p1,p2);
			size.Height = Drawing.Point.Distance(p1,p3);
			this.textLayout.LayoutSize = size;

			this.textLayout.DefaultFont     = this.PropertyFont(5).GetFont();
			this.textLayout.DefaultFontSize = this.PropertyFont(5).FontSize;
			this.textLayout.DefaultColor    = this.PropertyFont(5).FontColor;

			JustifVertical   jv = this.PropertyJustif(4).Vertical;
			JustifHorizontal jh = this.PropertyJustif(4).Horizontal;

			if ( jv == JustifVertical.Top )
			{
				     if ( jh == JustifHorizontal.Center )  this.textLayout.Alignment = Drawing.ContentAlignment.TopCenter;
				else if ( jh == JustifHorizontal.Right  )  this.textLayout.Alignment = Drawing.ContentAlignment.TopRight;
				else                                       this.textLayout.Alignment = Drawing.ContentAlignment.TopLeft;
			}
			if ( jv == JustifVertical.Center )
			{
				     if ( jh == JustifHorizontal.Center )  this.textLayout.Alignment = Drawing.ContentAlignment.MiddleCenter;
				else if ( jh == JustifHorizontal.Right  )  this.textLayout.Alignment = Drawing.ContentAlignment.MiddleRight;
				else                                       this.textLayout.Alignment = Drawing.ContentAlignment.MiddleLeft;
			}
			if ( jv == JustifVertical.Bottom )
			{
				     if ( jh == JustifHorizontal.Center )  this.textLayout.Alignment = Drawing.ContentAlignment.BottomCenter;
				else if ( jh == JustifHorizontal.Right  )  this.textLayout.Alignment = Drawing.ContentAlignment.BottomRight;
				else                                       this.textLayout.Alignment = Drawing.ContentAlignment.BottomLeft;
			}

			     if ( jh == JustifHorizontal.Justif )  this.textLayout.JustifMode = Drawing.TextJustifMode.AllButLast;
			else if ( jh == JustifHorizontal.All    )  this.textLayout.JustifMode = Drawing.TextJustifMode.All;
			else                                       this.textLayout.JustifMode = Drawing.TextJustifMode.NoLine;

			Drawing.Transform ot = port.Transform;

			double angle = Drawing.Point.ComputeAngleDeg(p1, p2);
			this.transform = new Drawing.Transform();
			this.transform.Translate(p1);
			this.transform.RotateDeg(angle, p1);
			port.MergeTransform(transform);

			if ( port is Drawing.Graphics &&
				 iconContext.IsFocused &&
				 this.edited &&
				 this.textNavigator.Context.CursorFrom != this.textNavigator.Context.CursorTo )
			{
				Drawing.Graphics graphics = port as Drawing.Graphics;
				int from = System.Math.Min(this.textNavigator.Context.CursorFrom, this.textNavigator.Context.CursorTo);
				int to   = System.Math.Max(this.textNavigator.Context.CursorFrom, this.textNavigator.Context.CursorTo);
				TextLayout.SelectedArea[] areas = this.textLayout.FindTextRange(new Drawing.Point(0,0), from, to);
				for ( int i=0 ; i<areas.Length ; i++ )
				{
					graphics.Align(ref areas[i].Rect);
					graphics.AddFilledRectangle(areas[i].Rect);
					graphics.RenderSolid(IconContext.ColorSelectEdit);
				}
			}

			this.textLayout.ShowLineBreak = this.edited;
			this.textLayout.Paint(new Drawing.Point(0,0), port);

			if ( port is Drawing.Graphics &&
				 iconContext.IsFocused &&
				 this.edited &&
				 this.textNavigator.Context.CursorTo != -1 )
			{
				Drawing.Graphics graphics = port as Drawing.Graphics;
				Drawing.Point c1, c2;
				if ( this.textLayout.FindTextCursor(this.textNavigator.Context, out c1, out c2) )
				{
					graphics.LineWidth = 1.0/iconContext.ScaleX;
					graphics.AddLine(c1, c2);
					graphics.RenderSolid(IconContext.ColorFrameEdit);
				}
			}

			port.Transform = ot;
		}

		// Dessine l'objet.
		public override void DrawGeometry(Drawing.Graphics graphics, IconContext iconContext, IconObjects iconObjects)
		{
			if ( base.IsFullHide(iconContext) )  return;
			base.DrawGeometry(graphics, iconContext, iconObjects);

			if ( this.TotalHandle < 2 )  return;

			Drawing.Path path = this.PathBuild();
			this.PropertyGradient(3).Render(graphics, iconContext, path, this.BoundingBoxThin);
			this.PropertyLine(1).DrawPath(graphics, iconContext, iconObjects, path, this.PropertyColor(2).Color);

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
					this.PropertyLine(1).AddOutline(graphics, path, 0.0);
					graphics.RenderSolid(iconContext.HiliteOutlineColor);
				}
			}
		}

		// Imprime l'objet.
		public override void PrintGeometry(Printing.PrintPort port, IconContext iconContext, IconObjects iconObjects)
		{
			base.PrintGeometry(port, iconContext, iconObjects);

			if ( this.TotalHandle < 2 )  return;

			Drawing.Path path = this.PathBuild();

			if ( this.PropertyGradient(3).PaintColor(port, iconContext) )
			{
				port.PaintSurface(path);
			}

			if ( this.PropertyColor(2).PaintColor(port, iconContext) )
			{
				this.PropertyLine(1).PaintOutline(port, iconContext, path);
			}

			if ( this.TotalHandle >= 4 )
			{
				this.DrawText(port, iconContext);
			}
		}


		protected TextLayout			textLayout;
		protected TextNavigator			textNavigator;
		protected Drawing.Transform		transform;
	}
}
