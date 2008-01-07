using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Objects
{
	/// <summary>
	/// La classe TextBox est la classe de l'objet graphique "pav� de texte".
	/// </summary>
	[System.Serializable()]
	public class TextBox : Objects.Abstract
	{
		public TextBox(Document document, Objects.Abstract model) : base(document, model)
		{
			if ( this.document == null )  return;  // objet factice ?
			this.CreateProperties(model, false);
			this.Initialize();
		}

		protected void Initialize()
		{
			this.textLayout = new TextLayout();
			this.textNavigator = new TextNavigator(this.textLayout);
			this.textNavigator.MaxChar = 10000;
			this.textLayout.BreakMode = TextBreakMode.Hyphenate;
			if ( this.document.Modifier != null )
			{
				this.textNavigator.OpletQueue = this.document.Modifier.OpletQueue;
			}
			this.cursorBox = Drawing.Rectangle.Empty;
			this.selectBox = Drawing.Rectangle.Empty;
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


		public override string IconName
		{
			//	Nom de l'ic�ne.
			get { return Misc.Icon("ObjectTextBox"); }
		}


		public string SimpleText
		{
			//	Donne le texte simple (sans commandes xml). Utilis� pour la conversion vers TextBox2.
			get
			{
				return TextLayout.ConvertToSimpleText(this.textLayout.Text);
			}
		}


		public override DetectEditType DetectEdit(Point pos)
		{
			//	D�tecte si la souris est sur l'objet pour l'�diter.
			if ( this.Detect(pos) )  return DetectEditType.Body;
			return DetectEditType.Out;
		}


		public override void MoveHandleStarting(int rank, Point pos, DrawingContext drawingContext)
		{
			//	D�but du d�placement d'une poign�e.
			base.MoveHandleStarting(rank, pos, drawingContext);

			if ( rank < this.handles.Count )  // poign�e de l'objet ?
			{
				drawingContext.ConstrainClear();

				Handle handle = this.Handle(rank);
				if ( handle.PropertyType == Properties.Type.None )
				{
						 if ( rank == 0 )  drawingContext.ConstrainAddRect(this.Handle(0).Position, this.Handle(1).Position, this.Handle(2).Position, this.Handle(3).Position, false, -1);
					else if ( rank == 1 )  drawingContext.ConstrainAddRect(this.Handle(1).Position, this.Handle(0).Position, this.Handle(3).Position, this.Handle(2).Position, false, -1);
					else if ( rank == 2 )  drawingContext.ConstrainAddRect(this.Handle(2).Position, this.Handle(3).Position, this.Handle(0).Position, this.Handle(1).Position, false, -1);
					else if ( rank == 3 )  drawingContext.ConstrainAddRect(this.Handle(3).Position, this.Handle(2).Position, this.Handle(1).Position, this.Handle(0).Position, false, -1);
				}
				else
				{
					Properties.Abstract property = this.Property(handle.PropertyType);
					property.MoveHandleStarting(this, handle.PropertyRank, pos, drawingContext);
				}

				drawingContext.MagnetClearStarting();
			}
		}

		public override void MoveHandleProcess(int rank, Point pos, DrawingContext drawingContext)
		{
			//	D�place une poign�e.
			if ( rank >= 4 )  // poign�e d'une propri�t� ?
			{
				base.MoveHandleProcess(rank, pos, drawingContext);
				return;
			}

			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapPos(ref pos);

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

			this.HandlePropertiesUpdate();
			this.SetDirtyBbox();
			this.TextInfoModifRect();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override void CreateMouseDown(Point pos, DrawingContext drawingContext)
		{
			//	D�but de la cr�ation d'un objet.
			drawingContext.ConstrainClear();
			drawingContext.ConstrainAddHomo(pos, false, -1);
			this.HandleAdd(pos, HandleType.Primary);  // rang = 0
			this.HandleAdd(pos, HandleType.Primary);  // rang = 1
			this.document.Notifier.NotifyArea(this.BoundingBox);
			this.isCreating = true;
		}

		public override void CreateMouseMove(Point pos, DrawingContext drawingContext)
		{
			//	D�placement pendant la cr�ation d'un objet.
			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapPos(ref pos);
			this.Handle(1).Position = pos;
			this.SetDirtyBbox();
			this.TextInfoModifRect();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override void CreateMouseUp(Point pos, DrawingContext drawingContext)
		{
			//	Fin de la cr�ation d'un objet.
			this.document.Notifier.NotifyArea(this.BoundingBox);
			this.isCreating = false;
			this.document.Modifier.TextInfoModif = "";

			drawingContext.SnapPos(ref pos);
			this.Handle(1).Position = pos;
			drawingContext.ConstrainDelStarting();
			drawingContext.MagnetClearStarting();

			//	Cr�e les 2 autres poign�es dans les coins oppos�s.
			Drawing.Rectangle rect = Drawing.Rectangle.FromPoints(this.Handle(0).Position, this.Handle(1).Position);
			Point p1 = rect.BottomLeft;
			Point p2 = rect.TopRight;
			this.Handle(0).Position = p1;
			this.Handle(1).Position = p2;
			this.HandleAdd(new Point(p1.X, p2.Y), HandleType.Primary);  // rang = 2
			this.HandleAdd(new Point(p2.X, p1.Y), HandleType.Primary);  // rang = 3

			this.HandlePropertiesCreate();
			this.HandlePropertiesUpdate();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override bool CreateIsExist(DrawingContext drawingContext)
		{
			//	Indique si l'objet doit exister. Retourne false si l'objet ne peut
			//	pas exister et doit �tre d�truit.
			double len = Point.Distance(this.Handle(0).Position, this.Handle(1).Position);
			return ( len > drawingContext.MinimalSize );
		}

		public override bool EditAfterCreation()
		{
			//	Indique s'il faut s�lectionner l'objet apr�s sa cr�ation.
			return true;
		}

		public override void FillFontFaceList(List<OpenType.FontName> list)
		{
			//	Ajoute toutes les fontes utilis�es par l'objet dans une liste.
			this.textLayout.FillFontFaceList(list);
		}

		public override void FillOneCharList(IPaintPort port, DrawingContext drawingContext, System.Collections.Hashtable table)
		{
			//	Ajoute tous les caract�res utilis�s par l'objet dans une table.
			Point p1 = new Point();
			Point p2 = new Point();
			Point p3 = new Point();
			Point p4 = new Point();
			if ( !this.InitTextLayout(ref p1, ref p2, ref p3, ref p4, null) )  return;
			TextLayout.OneCharStructure[] fix = this.textLayout.ComputeStructure();

			foreach ( TextLayout.OneCharStructure oneChar in fix )
			{
				PDF.CharacterList cl = new PDF.CharacterList(oneChar);
				if ( !table.ContainsKey(cl) )
				{
					table.Add(cl, null);
				}
			}
		}

		public override bool IsEditable
		{
			//	Indique si un objet est �ditable.
			get { return false; }
		}


		public override void CloneObject(Objects.Abstract src)
		{
			//	Reprend toutes les caract�ristiques d'un objet.
			base.CloneObject(src);

			TextBox obj = src as TextBox;
			this.textLayout.Text = obj.textLayout.Text;
			obj.textLayout.Style.TabCopyTo(this.textLayout.Style);
			obj.textNavigator.Context.CopyTo(this.textNavigator.Context);
			this.textLayout.Simplify(this.textNavigator.Context);
		}


		#region TextFormat
		public bool EditBold()
		{
			//	Met en gras pendant l'�dition.
			this.textNavigator.SelectionBold = !this.textNavigator.SelectionBold;
			return true;
		}

		public bool EditItalic()
		{
			//	Met en italique pendant l'�dition.
			this.textNavigator.SelectionItalic = !this.textNavigator.SelectionItalic;
			return true;
		}

		public bool EditUnderline()
		{
			//	Souligne pendant l'�dition.
			this.textNavigator.SelectionUnderline = !this.textNavigator.SelectionUnderline;
			return true;
		}
		#endregion


		public override Shape[] ShapesBuild(IPaintPort port, DrawingContext drawingContext, bool simplify)
		{
			//	Constuit les formes de l'objet.
			Path path = this.PathBuild();

			Shape[] shapes = new Shape[4];
			int i = 0;
			
			//	Forme de la surface.
			shapes[i] = new Shape();
			shapes[i].Path = path;
			shapes[i].SetPropertySurface(port, this.PropertyFillGradient);
			i ++;

			//	Trait du rectangle.
			shapes[i] = new Shape();
			shapes[i].Path = path;
			shapes[i].SetPropertyStroke(port, this.PropertyLineMode, this.PropertyLineColor);
			i ++;

			//	Caract�res du texte.
			shapes[i] = new Shape();
			shapes[i].SetTextObject(this);
			i ++;

			//	Rectangle complet pour bbox et d�tection.
			shapes[i] = new Shape();
			shapes[i].Path = path;
			shapes[i].Type = Type.Surface;
			shapes[i].Aspect = Aspect.InvisibleBox;
			i ++;

			return shapes;
		}

		protected Path PathBuild()
		{
			//	Cr�e le chemin de l'objet.
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

		protected void Corners(ref Point p1, ref Point p2, ref Point p3, ref Point p4)
		{
			//	Calcules les 4 coins.
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
		}

		protected bool InitTextLayout(ref Point p1, ref Point p2, ref Point p3, ref Point p4,
									  DrawingContext drawingContext)
		{
			//	Initialise TextLayout.
			this.Corners(ref p1, ref p2, ref p3, ref p4);
			if ( !this.PropertyTextJustif.DeflateBox(ref p1, ref p2, ref p3, ref p4) )
			{
				return false;
			}

			Size size = new Size();
			size.Width  = Point.Distance(p1,p2);
			size.Height = Point.Distance(p1,p3);
			this.textLayout.LayoutSize = size;

			if ( drawingContext != null )
			{
				this.textLayout.DrawingScale = drawingContext.ScaleX;
			}

			this.textLayout.DefaultFont      = this.PropertyTextFont.GetFont();
			this.textLayout.DefaultFontSize  = this.PropertyTextFont.FontSize;
			this.textLayout.DefaultRichColor = this.PropertyTextFont.FontColor;

			return true;
		}

		public override void DrawText(IPaintPort port, DrawingContext drawingContext)
		{
			//	Dessine le texte du pav�.
			if ( this.handles.Count < 4 )  return;

			this.cursorBox = Drawing.Rectangle.Empty;
			this.selectBox = Drawing.Rectangle.Empty;

			Point p1 = new Point();
			Point p2 = new Point();
			Point p3 = new Point();
			Point p4 = new Point();
			if ( !this.InitTextLayout(ref p1, ref p2, ref p3, ref p4, drawingContext) )  return;

			Properties.JustifVertical   jv = this.PropertyTextJustif.Vertical;
			Properties.JustifHorizontal jh = this.PropertyTextJustif.Horizontal;

			if ( jv == Properties.JustifVertical.Top )
			{
				     if ( jh == Properties.JustifHorizontal.Center )  this.textLayout.Alignment = ContentAlignment.TopCenter;
				else if ( jh == Properties.JustifHorizontal.Right  )  this.textLayout.Alignment = ContentAlignment.TopRight;
				else                                                  this.textLayout.Alignment = ContentAlignment.TopLeft;
			}
			if ( jv == Properties.JustifVertical.Center )
			{
				     if ( jh == Properties.JustifHorizontal.Center )  this.textLayout.Alignment = ContentAlignment.MiddleCenter;
				else if ( jh == Properties.JustifHorizontal.Right  )  this.textLayout.Alignment = ContentAlignment.MiddleRight;
				else                                                  this.textLayout.Alignment = ContentAlignment.MiddleLeft;
			}
			if ( jv == Properties.JustifVertical.Bottom )
			{
				     if ( jh == Properties.JustifHorizontal.Center )  this.textLayout.Alignment = ContentAlignment.BottomCenter;
				else if ( jh == Properties.JustifHorizontal.Right  )  this.textLayout.Alignment = ContentAlignment.BottomRight;
				else                                                  this.textLayout.Alignment = ContentAlignment.BottomLeft;
			}

			     if ( jh == Properties.JustifHorizontal.Justif )  this.textLayout.JustifMode = TextJustifMode.AllButLast;
			else if ( jh == Properties.JustifHorizontal.All    )  this.textLayout.JustifMode = TextJustifMode.All;
			else                                                  this.textLayout.JustifMode = TextJustifMode.NoLine;

			Transform ot = port.Transform;

			double angle = Point.ComputeAngleDeg(p1, p2);
			this.transform = new Transform();
			this.transform.Translate(p1);
			this.transform.RotateDeg(angle, p1);
			port.MergeTransform(transform);

			bool active = true;
			if ( this.document.Modifier != null )
			{
				active = (this.document.Modifier.ActiveViewer.DrawingContext == drawingContext &&
						  this.document.Modifier.ActiveViewer.IsFocused);
			}

			if ( port is Graphics &&
				 this.edited &&
				 this.textNavigator.Context.CursorFrom != this.textNavigator.Context.CursorTo )
			{
				Graphics graphics = port as Graphics;
				int from = System.Math.Min(this.textNavigator.Context.CursorFrom, this.textNavigator.Context.CursorTo);
				int to   = System.Math.Max(this.textNavigator.Context.CursorFrom, this.textNavigator.Context.CursorTo);
				TextLayout.SelectedArea[] areas = this.textLayout.FindTextRange(Point.Zero, from, to);
				for ( int i=0 ; i<areas.Length ; i++ )
				{
					Drawing.Rectangle box = graphics.Align (areas[i].Rect);
					graphics.AddFilledRectangle(box);
					graphics.RenderSolid(DrawingContext.ColorSelectEdit(active));

					Drawing.Rectangle r = new Drawing.Rectangle(this.transform.TransformDirect(box.BottomLeft), this.transform.TransformDirect(box.TopRight));
					this.selectBox.MergeWith(r);
				}
			}

			this.textLayout.ShowLineBreak = this.edited;
			this.textLayout.ShowTab       = this.edited;
			this.textLayout.Paint(new Point(0,0), port);  // dessine le texte

			if ( port is Graphics &&
				 this.edited &&
				 this.textNavigator.Context.CursorTo != -1 )
			{
				Graphics graphics = port as Graphics;
				Point c1, c2;
				if ( this.textLayout.FindTextCursor(this.textNavigator.Context, out c1, out c2) )
				{
					graphics.LineWidth = 1.0/drawingContext.ScaleX;
					graphics.AddLine(c1, c2);
					graphics.RenderSolid(DrawingContext.ColorCursorEdit(active));

					c1 = this.transform.TransformDirect(c1);
					c2 = this.transform.TransformDirect(c2);
					this.ComputeAutoScroll(c1, c2);
					this.cursorBox.MergeWith(c1);
					this.cursorBox.MergeWith(c2);
					this.selectBox.MergeWith(c1);
					this.selectBox.MergeWith(c2);
				}
			}

			port.Transform = ot;
		}


		public override Path GetMagnetPath()
		{
			//	Retourne le chemin g�om�trique de l'objet pour les constructions
			//	magn�tiques.
			return this.PathBuild();
		}


		#region Serialization
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	S�rialise l'objet.
			base.GetObjectData(info, context);
			info.AddValue("Text", this.textLayout.Text);
			info.AddValue("TabArray", this.textLayout.Style.GetTabArray());
		}

		protected TextBox(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui d�s�rialise l'objet.
			this.Initialize();
			this.textLayout.Text = info.GetString("Text");

			Drawing.TextStyle.Tab[] tabs = (Drawing.TextStyle.Tab[]) info.GetValue("TabArray", typeof(Drawing.TextStyle.Tab[]));
			this.textLayout.Style.TabCopyFrom(tabs);
		}

		public override void ReadCheckWarnings(System.Collections.ArrayList warnings)
		{
			//	V�rifie si tous les fichiers existent.
			Common.Document.Objects.Abstract.ReadCheckFonts(warnings, this.textLayout);
		}
		#endregion

		
		protected TextLayout				textLayout;
		protected TextNavigator				textNavigator;
		protected Transform					transform;
		protected Drawing.Rectangle			cursorBox;
		protected Drawing.Rectangle			selectBox;
	}
}
