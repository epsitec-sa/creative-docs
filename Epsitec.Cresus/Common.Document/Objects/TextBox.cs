using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Objects
{
	/// <summary>
	/// La classe TextBox est la classe de l'objet graphique "pavé de texte".
	/// </summary>
	[System.Serializable()]
	public class TextBox : Objects.Abstract
	{
		public TextBox(Document document, Objects.Abstract model) : base(document, model)
		{
			if ( this.document == null )  return;  // objet factice ?
			this.CreateProperties(model, false);
			this.Initialise();
		}

		protected void Initialise()
		{
			this.textLayout = new TextLayout();
			this.textNavigator = new TextNavigator(this.textLayout);
			this.textLayout.BreakMode = TextBreakMode.Hyphenate;
			if ( this.document.Modifier != null )
			{
				this.textNavigator.OpletQueue = this.document.Modifier.OpletQueue;
			}
		}

		protected override bool ExistingProperty(Properties.Type type)
		{
			if ( type == Properties.Type.Name )  return true;
			if ( type == Properties.Type.LineMode )  return true;
			if ( type == Properties.Type.LineColor )  return true;
			if ( type == Properties.Type.FillGradient )  return true;
			if ( type == Properties.Type.TextJustif )  return true;
			if ( type == Properties.Type.TextFont )  return true;
			return false;
		}

		protected override Objects.Abstract CreateNewObject(Document document, Objects.Abstract model)
		{
			return new TextBox(document, model);
		}

		public override void Dispose()
		{
			base.Dispose();
		}


		// Nom de l'icône.
		public override string IconName
		{
			get { return "manifest:Epsitec.App.DocumentEditor.Images.TextBox.icon"; }
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
		public override bool Detect(Point pos)
		{
			if ( this.isHide )  return false;

			Drawing.Rectangle bbox = this.BoundingBox;
			if ( !bbox.Contains(pos) )  return false;

			Path path = this.PathBuild();

			double width = this.PropertyLineMode.Width/2;
			if ( width > 0 && Geometry.DetectOutline(path, width, pos) )  return true;
			
			if ( Geometry.DetectSurface(path, pos) )  return true;

			return false;
		}


		// Détecte si la souris est sur l'objet pour l'éditer.
		public override bool DetectEdit(Point pos)
		{
			if ( this.isHide )  return false;

			Drawing.Rectangle bbox = this.BoundingBox;
			if ( !bbox.Contains(pos) )  return false;

			Path path = this.PathBuild();
			return Geometry.DetectSurface(path, pos);
		}


		// Déplace une poignée.
		public override void MoveHandleProcess(int rank, Point pos, DrawingContext drawingContext)
		{
			if ( rank >= 4 )  // poignée d'une propriété ?
			{
				base.MoveHandleProcess(rank, pos, drawingContext);
				return;
			}

			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.ConstrainSnapPos(ref pos);
			drawingContext.SnapGrid(ref pos);

			if ( Geometry.IsRectangular(this.Handle(0).Position, this.Handle(1).Position, this.Handle(2).Position, this.Handle(3).Position) )
			{
				this.Handle(rank).Position = pos;

				if ( rank == 0 )
				{
					this.Handle(2).Position = Point.Projection(this.Handle(2).Position, this.Handle(1).Position, pos);
					this.Handle(3).Position = Point.Projection(this.Handle(3).Position, this.Handle(1).Position, pos);
				}
				if ( rank == 1 )
				{
					this.Handle(2).Position = Point.Projection(this.Handle(2).Position, this.Handle(0).Position, pos);
					this.Handle(3).Position = Point.Projection(this.Handle(3).Position, this.Handle(0).Position, pos);
				}
				if ( rank == 2 )
				{
					this.Handle(0).Position = Point.Projection(this.Handle(0).Position, this.Handle(3).Position, pos);
					this.Handle(1).Position = Point.Projection(this.Handle(1).Position, this.Handle(3).Position, pos);
				}
				if ( rank == 3 )
				{
					this.Handle(0).Position = Point.Projection(this.Handle(0).Position, this.Handle(2).Position, pos);
					this.Handle(1).Position = Point.Projection(this.Handle(1).Position, this.Handle(2).Position, pos);
				}
			}
			else
			{
				this.Handle(rank).Position = pos;
			}

			this.HandlePropertiesUpdatePosition();
			this.dirtyBbox = true;
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// Début de la création d'un objet.
		public override void CreateMouseDown(Point pos, DrawingContext drawingContext)
		{
			drawingContext.ConstrainFixStarting(pos);
			drawingContext.ConstrainFixType(ConstrainType.Square);
			this.HandleAdd(pos, HandleType.Primary);  // rang = 0
			this.HandleAdd(pos, HandleType.Primary);  // rang = 1
			this.document.Notifier.NotifyArea(this.BoundingBox);
			this.isCreating = true;
		}

		// Déplacement pendant la création d'un objet.
		public override void CreateMouseMove(Point pos, DrawingContext drawingContext)
		{
			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapGrid(ref pos);
			drawingContext.ConstrainSnapPos(ref pos);
			this.Handle(1).Position = pos;
			this.dirtyBbox = true;
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// Fin de la création d'un objet.
		public override void CreateMouseUp(Point pos, DrawingContext drawingContext)
		{
			this.document.Notifier.NotifyArea(this.BoundingBox);
			this.isCreating = false;

			drawingContext.SnapGrid(ref pos);
			drawingContext.ConstrainSnapPos(ref pos);
			this.Handle(1).Position = pos;
			drawingContext.ConstrainDelStarting();

			// Crée les 2 autres poignées dans les coins opposés.
			Drawing.Rectangle rect = Drawing.Rectangle.FromCorners(this.Handle(0).Position, this.Handle(1).Position);
			Point p1 = rect.BottomLeft;
			Point p2 = rect.TopRight;
			this.Handle(0).Position = p1;
			this.Handle(1).Position = p2;
			this.HandleAdd(new Point(p1.X, p2.Y), HandleType.Primary);  // rang = 2
			this.HandleAdd(new Point(p2.X, p1.Y), HandleType.Primary);  // rang = 3

			this.HandlePropertiesCreate();
			this.HandlePropertiesUpdatePosition();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// Indique si l'objet doit exister. Retourne false si l'objet ne peut
		// pas exister et doit être détruit.
		public override bool CreateIsExist(DrawingContext drawingContext)
		{
			double len = Point.Distance(this.Handle(0).Position, this.Handle(1).Position);
			return ( len > drawingContext.MinimalSize );
		}

		// Indique s'il faut sélectionner l'objet après sa création.
		public override bool EditAfterCreation()
		{
			return true;
		}

		// Ajoute toutes les fontes utilisées par l'objet dans une liste.
		public override void FillFontFaceList(System.Collections.ArrayList list)
		{
			this.textLayout.FillFontFaceList(list);
		}

		// Indique si un objet est éditable.
		public override bool IsEditable
		{
			get { return true; }
		}

		// Lie l'objet éditable à une règle.
		public override bool EditRulerLink(TextRuler ruler, DrawingContext drawingContext)
		{
			ruler.TabCapability = true;
			ruler.AttachToText(this.textNavigator);

			double mx = this.PropertyTextJustif.MarginH;
			ruler.LeftMargin  = mx*drawingContext.ScaleX;
			ruler.RightMargin = mx*drawingContext.ScaleX;
			return true;
		}


		// Reprend toutes les caractéristiques d'un objet.
		public override void CloneObject(Objects.Abstract src)
		{
			base.CloneObject(src);

			TextBox obj = src as TextBox;
			this.textLayout.Text = obj.textLayout.Text;
			obj.textNavigator.Context.CopyTo(this.textNavigator.Context);
		}


		// Gestion d'un événement pendant l'édition.
		public override bool EditProcessMessage(Message message, Point pos)
		{
			if ( this.transform == null )  return false;

			pos = this.transform.TransformInverse(pos);
			if ( !this.textNavigator.ProcessMessage(message, pos) )  return false;
			return true;
		}

		// Gestion d'un événement pendant l'édition.
		public override void EditMouseDownMessage(Point pos)
		{
			pos = this.transform.TransformInverse(pos);
			this.textNavigator.MouseDownMessage(pos);
		}


		// Met à jour le rectangle englobant l'objet.
		protected override void UpdateBoundingBox()
		{
			if ( this.handles.Count < 2 )  return;

			Path path = this.PathBuild();

			Path[] paths = new Path[1];
			paths[0] = path;

			bool[] lineModes = new bool[1];
			lineModes[0] = true;

			bool[] lineColors = new bool[1];
			lineColors[0] = true;

			bool[] fillGradients = new bool[1];
			fillGradients[0] = true;

			this.ComputeBoundingBox(paths, lineModes, lineColors, fillGradients);

			if ( this.TotalHandle >= 4 )
			{
				this.InflateBoundingBox(this.Handle(0).Position, false);
				this.InflateBoundingBox(this.Handle(1).Position, false);
				this.InflateBoundingBox(this.Handle(2).Position, false);
				this.InflateBoundingBox(this.Handle(3).Position, false);
			}
		}

		// Crée le chemin de l'objet.
		protected Path PathBuild()
		{
			Point p1 = this.Handle(0).Position;
			Point p2 = new Point();
			Point p3 = this.Handle(1).Position;
			Point p4 = new Point();

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

			Path path = new Path();
			path.MoveTo(p1);
			path.LineTo(p2);
			path.LineTo(p3);
			path.LineTo(p4);
			path.Close();
			return path;
		}

		// Dessine le texte du pavé.
		protected void DrawText(IPaintPort port, DrawingContext drawingContext)
		{
			Point p1, p2, p3, p4;
			switch ( this.PropertyTextJustif.Orientation )
			{
				case Properties.JustifOrientation.RightToLeft:  // <-
					p1 = this.Handle(1).Position;
					p2 = this.Handle(2).Position;
					p3 = this.Handle(3).Position;
					p4 = this.Handle(0).Position;
					break;
				case Properties.JustifOrientation.BottomToTop:  // ^
					p1 = this.Handle(3).Position;
					p2 = this.Handle(1).Position;
					p3 = this.Handle(0).Position;
					p4 = this.Handle(2).Position;
					break;
				case Properties.JustifOrientation.TopToBottom:  // v
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
			if ( !this.PropertyTextJustif.DeflateBox(ref p1, ref p2, ref p3, ref p4) )  return;

			Size size = new Size();
			size.Width  = Point.Distance(p1,p2);
			size.Height = Point.Distance(p1,p3);
			this.textLayout.LayoutSize = size;
			this.textLayout.DrawingScale = drawingContext.ScaleX;

			this.textLayout.DefaultFont     = this.PropertyTextFont.GetFont();
			this.textLayout.DefaultFontSize = this.PropertyTextFont.FontSize;
			this.textLayout.DefaultColor    = this.PropertyTextFont.FontColor;

			Properties.JustifVertical   jv = this.PropertyTextJustif.Vertical;
			Properties.JustifHorizontal jh = this.PropertyTextJustif.Horizontal;

			if ( jv == Properties.JustifVertical.Top )
			{
				     if ( jh == Properties.JustifHorizontal.Center )  this.textLayout.Alignment = ContentAlignment.TopCenter;
				else if ( jh == Properties.JustifHorizontal.Right  )  this.textLayout.Alignment = ContentAlignment.TopRight;
				else                                       this.textLayout.Alignment = ContentAlignment.TopLeft;
			}
			if ( jv == Properties.JustifVertical.Center )
			{
				     if ( jh == Properties.JustifHorizontal.Center )  this.textLayout.Alignment = ContentAlignment.MiddleCenter;
				else if ( jh == Properties.JustifHorizontal.Right  )  this.textLayout.Alignment = ContentAlignment.MiddleRight;
				else                                       this.textLayout.Alignment = ContentAlignment.MiddleLeft;
			}
			if ( jv == Properties.JustifVertical.Bottom )
			{
				     if ( jh == Properties.JustifHorizontal.Center )  this.textLayout.Alignment = ContentAlignment.BottomCenter;
				else if ( jh == Properties.JustifHorizontal.Right  )  this.textLayout.Alignment = ContentAlignment.BottomRight;
				else                                       this.textLayout.Alignment = ContentAlignment.BottomLeft;
			}

			     if ( jh == Properties.JustifHorizontal.Justif )  this.textLayout.JustifMode = TextJustifMode.AllButLast;
			else if ( jh == Properties.JustifHorizontal.All    )  this.textLayout.JustifMode = TextJustifMode.All;
			else                                       this.textLayout.JustifMode = TextJustifMode.NoLine;

			Transform ot = port.Transform;

			double angle = Point.ComputeAngleDeg(p1, p2);
			this.transform = new Transform();
			this.transform.Translate(p1);
			this.transform.RotateDeg(angle, p1);
			port.MergeTransform(transform);

			bool active = (this.document.Modifier.ActiveViewer.DrawingContext == drawingContext);

			if ( port is Graphics &&
				 active &&
				 this.edited &&
				 this.textNavigator.Context.CursorFrom != this.textNavigator.Context.CursorTo )
			{
				Graphics graphics = port as Graphics;
				int from = System.Math.Min(this.textNavigator.Context.CursorFrom, this.textNavigator.Context.CursorTo);
				int to   = System.Math.Max(this.textNavigator.Context.CursorFrom, this.textNavigator.Context.CursorTo);
				TextLayout.SelectedArea[] areas = this.textLayout.FindTextRange(new Point(0,0), from, to);
				for ( int i=0 ; i<areas.Length ; i++ )
				{
					graphics.Align(ref areas[i].Rect);
					graphics.AddFilledRectangle(areas[i].Rect);
					graphics.RenderSolid(DrawingContext.ColorSelectEdit);
				}
			}

			this.textLayout.ShowLineBreak = this.edited;
			this.textLayout.ShowTab       = this.edited;
			this.textLayout.Paint(new Point(0,0), port);

			if ( port is Graphics &&
				 active &&
				 this.edited &&
				 this.textNavigator.Context.CursorTo != -1 )
			{
				Graphics graphics = port as Graphics;
				Point c1, c2;
				if ( this.textLayout.FindTextCursor(this.textNavigator.Context, out c1, out c2) )
				{
					graphics.LineWidth = 1.0/drawingContext.ScaleX;
					graphics.AddLine(c1, c2);
					graphics.RenderSolid(DrawingContext.ColorFrameEdit);
				}
			}

			port.Transform = ot;
		}

		// Dessine l'objet.
		public override void DrawGeometry(Graphics graphics, DrawingContext drawingContext)
		{
			base.DrawGeometry(graphics, drawingContext);

			if ( this.TotalHandle < 2 )  return;

			Path path = this.PathBuild();
			this.PropertyFillGradient.RenderSurface(graphics, drawingContext, path, this.BoundingBoxThin);
			this.PropertyLineMode.DrawPath(graphics, drawingContext, path, this.PropertyLineColor, this.BoundingBoxGeom);

			if ( this.TotalHandle >= 4 )
			{
				this.DrawText(graphics, drawingContext);
			}

			if ( this.edited && drawingContext.IsActive )  // en cours d'édition ?
			{
				graphics.Rasterizer.AddOutline(path, 2.0/drawingContext.ScaleX);
				graphics.RenderSolid(DrawingContext.ColorFrameEdit);
			}
			else
			{
				if ( this.IsSelected || this.isCreating )
				{
					if ( this.PropertyLineMode.Width == 0.0 ||
						 this.PropertyLineColor.Color1.A == 0.0 )
					{
						this.PropertyLineMode.AddOutline(graphics, path, 1.0/drawingContext.ScaleX);
						graphics.RenderSolid(drawingContext.HiliteOutlineColor);
					}
				}

				if ( this.IsHilite && drawingContext.IsActive )
				{
					if ( !this.edited )
					{
						graphics.Rasterizer.AddSurface(path);
						graphics.RenderSolid(drawingContext.HiliteSurfaceColor);
					}
					this.PropertyLineMode.AddOutline(graphics, path, drawingContext.HiliteSize);
					graphics.RenderSolid(drawingContext.HiliteOutlineColor);
				}
			}
		}

		// Imprime l'objet.
		public override void PrintGeometry(Printing.PrintPort port, DrawingContext drawingContext)
		{
			base.PrintGeometry(port, drawingContext);

			if ( this.TotalHandle < 2 )  return;

			Path path = this.PathBuild();

			if ( this.PropertyFillGradient.PaintColor(port, drawingContext) )
			{
				port.PaintSurface(path);
			}

			if ( this.PropertyLineColor.PaintColor(port, drawingContext) )
			{
				this.PropertyLineMode.PaintOutline(port, drawingContext, path);
			}

			if ( this.TotalHandle >= 4 )
			{
				this.DrawText(port, drawingContext);
			}
		}


		#region Serialization
		// Sérialise l'objet.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("Text", this.textLayout.Text);
		}

		// Constructeur qui désérialise l'objet.
		protected TextBox(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.Initialise();
			this.textLayout.Text = info.GetString("Text");
		}

		// Vérifie si tous les fichiers existent.
		public override void ReadCheckWarnings(Font.FaceInfo[] fonts, System.Collections.ArrayList warnings)
		{
			Common.Document.Objects.Abstract.ReadCheckFonts(fonts, warnings, this.textLayout);
		}
		#endregion

		
		protected TextLayout			textLayout;
		protected TextNavigator			textNavigator;
		protected Transform				transform;
	}
}
