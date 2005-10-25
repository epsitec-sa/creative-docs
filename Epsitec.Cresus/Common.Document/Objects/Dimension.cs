using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Objects
{
	/// <summary>
	/// La classe Dimension est la classe de l'objet graphique "cotation".
	/// </summary>
	[System.Serializable()]
	public class Dimension : Objects.Abstract
	{
		public Dimension(Document document, Objects.Abstract model) : this(document, model, false)
		{
		}

		public Dimension(Document document, Objects.Abstract model, bool floating) : base(document, model)
		{
			if ( this.document == null )  return;  // objet factice ?
			this.CreateProperties(model, floating);
		}

		protected override bool ExistingProperty(Properties.Type type)
		{
			if ( type == Properties.Type.Name )  return true;
			if ( type == Properties.Type.LineMode )  return true;
			if ( type == Properties.Type.LineDimension )  return true;
			if ( type == Properties.Type.LineColor )  return true;
			if ( type == Properties.Type.DimensionArrow )  return true;
			if ( type == Properties.Type.Dimension )  return true;
			if ( type == Properties.Type.TextFont )  return true;
			return false;
		}

		protected override Objects.Abstract CreateNewObject(Document document, Objects.Abstract model)
		{
			return new Dimension(document, model);
		}

		public override void Dispose()
		{
			base.Dispose();
		}


		// Nom de l'ic�ne.
		public override string IconName
		{
			get { return Misc.Icon("ObjectDimension"); }
		}


		// D�but du d�placement d'une poign�e.
		public override void MoveHandleStarting(int rank, Point pos, DrawingContext drawingContext)
		{
			base.MoveHandleStarting(rank, pos, drawingContext);

			if ( rank < this.handles.Count )  // poign�e de l'objet ?
			{
				drawingContext.ConstrainFlush();

				Handle handle = this.Handle(rank);
				if ( handle.PropertyType == Properties.Type.None )
				{
					if ( rank == 0 || rank == 1 )  // extr�mit� ?
					{
						drawingContext.ConstrainAddHV(this.Handle(0).Position);
						drawingContext.ConstrainAddHV(this.Handle(1).Position);
						drawingContext.ConstrainAddLine(this.Handle(0).Position, this.Handle(1).Position);
					}
					else if ( rank == 2 || rank == 3 )  // support ?
					{
						if ( this.IsRight )
						{
							drawingContext.ConstrainAddLine(this.Handle(rank-2).Position, this.Handle(rank).Position);
							drawingContext.ConstrainAddLine(this.Handle(rank).Position, this.Handle(rank).Position+(this.Handle((rank-2)^1).Position-this.Handle(rank-2).Position));
						}
						else
						{
							drawingContext.ConstrainAddHV(this.Handle(rank-2).Position);
							drawingContext.ConstrainAddHV(this.Handle(rank).Position);
							drawingContext.ConstrainAddHV(this.Handle(rank^1).Position);
							drawingContext.ConstrainAddLine(this.Handle(rank-2).Position, this.Handle(rank).Position);
						}
					}
					else if ( rank == 4 )  // milieu ?
					{
						if ( this.IsRight )
						{
							drawingContext.ConstrainAddLine(this.Handle(4).Position, this.Handle(4).Position+(this.Handle(0).Position-this.Handle(2).Position));
						}
						else
						{
							drawingContext.ConstrainAddHV(this.Handle(4).Position);
							drawingContext.ConstrainAddLine(this.Handle(0).Position, this.Handle(1).Position);
							drawingContext.ConstrainAddLine(this.Handle(4).Position, this.Handle(4).Position+(this.Handle(0).Position-this.Handle(2).Position));
						}
					}
				}
				else
				{
					Properties.Abstract property = this.Property(handle.PropertyType);
					property.MoveHandleStarting(this, handle.PropertyRank, pos, drawingContext);
				}

				if ( rank == 0 )
				{
					drawingContext.MagnetFixStarting(this.Handle(1).Position);
				}
				else if ( rank == 1 )
				{
					drawingContext.MagnetFixStarting(this.Handle(0).Position);
				}
				else
				{
					drawingContext.MagnetClearStarting();
				}
			}
		}

		// D�place une poign�e.
		public override void MoveHandleProcess(int rank, Point pos, DrawingContext drawingContext)
		{
			if ( rank >= 5 )  // poign�e d'une propri�t� ?
			{
				base.MoveHandleProcess(rank, pos, drawingContext);
				return;
			}

			this.document.Notifier.NotifyArea(this.BoundingBox);
			
			if ( rank != 4 )  // pas milieu ?
			{
				drawingContext.SnapPos(ref pos);
			}

			if ( rank == 0 )  // extr�mit� gauche ?
			{
				if ( this.IsRight )
				{
					double ai = Point.ComputeAngleDeg(this.Handle(0).Position, this.Handle(1).Position);
					double a2 = Point.ComputeAngleDeg(this.Handle(0).Position, this.Handle(2).Position);
					double a3 = Point.ComputeAngleDeg(this.Handle(1).Position, this.Handle(3).Position);
					double l2 = Point.Distance(this.Handle(0).Position, this.Handle(2).Position);
					double l3 = Point.Distance(this.Handle(1).Position, this.Handle(3).Position);
					this.Handle(0).Position = pos;
					double af = Point.ComputeAngleDeg(this.Handle(0).Position, this.Handle(1).Position);
					this.Handle(2).Position = Transform.RotatePointDeg(this.Handle(0).Position, a2+(af-ai), this.Handle(0).Position+new Point(l2,0));
					this.Handle(3).Position = Transform.RotatePointDeg(this.Handle(1).Position, a3+(af-ai), this.Handle(1).Position+new Point(l3,0));
					this.Handle(4).Position = Point.Scale(this.Handle(0).Position, this.Handle(1).Position, 0.5);
				}
				else
				{
					Point d = this.Handle(2).Position-this.Handle(0).Position;
					this.Handle(0).Position = pos;
					this.Handle(2).Position = pos+d;
					this.Handle(4).Position = Point.Scale(this.Handle(0).Position, this.Handle(1).Position, 0.5);
				}
			}

			if ( rank == 1 )  // extr�mit� droite ?
			{
				if ( this.IsRight )
				{
					double ai = Point.ComputeAngleDeg(this.Handle(1).Position, this.Handle(0).Position);
					double a2 = Point.ComputeAngleDeg(this.Handle(0).Position, this.Handle(2).Position);
					double a3 = Point.ComputeAngleDeg(this.Handle(1).Position, this.Handle(3).Position);
					double l2 = Point.Distance(this.Handle(0).Position, this.Handle(2).Position);
					double l3 = Point.Distance(this.Handle(1).Position, this.Handle(3).Position);
					this.Handle(1).Position = pos;
					double af = Point.ComputeAngleDeg(this.Handle(1).Position, this.Handle(0).Position);
					this.Handle(2).Position = Transform.RotatePointDeg(this.Handle(0).Position, a2+(af-ai), this.Handle(0).Position+new Point(l2,0));
					this.Handle(3).Position = Transform.RotatePointDeg(this.Handle(1).Position, a3+(af-ai), this.Handle(1).Position+new Point(l3,0));
					this.Handle(4).Position = Point.Scale(this.Handle(0).Position, this.Handle(1).Position, 0.5);
				}
				else
				{
					Point d = this.Handle(3).Position-this.Handle(1).Position;
					this.Handle(1).Position = pos;
					this.Handle(3).Position = pos+d;
					this.Handle(4).Position = Point.Scale(this.Handle(0).Position, this.Handle(1).Position, 0.5);
				}
			}
			
			if ( rank == 2 )  // support gauche ?
			{
				if ( this.IsRight )
				{
					this.Handle(0).Position = Point.Projection(this.Handle(0).Position, this.Handle(1).Position, pos);
					this.Handle(2).Position = pos;
					this.Handle(4).Position = Point.Scale(this.Handle(0).Position, this.Handle(1).Position, 0.5);
				}
				else
				{
					this.Handle(2).Position = pos;
					double angle = Point.ComputeAngleDeg(this.Handle(0).Position, this.Handle(2).Position);
					double len = Point.Distance(this.Handle(1).Position, this.Handle(3).Position);
					Point p = this.Handle(1).Position + new Point(len,0);
					this.Handle(3).Position = Transform.RotatePointDeg(this.Handle(1).Position, angle, p);
				}
			}
			
			if ( rank == 3 )  // support droite ?
			{
				if ( this.IsRight )
				{
					this.Handle(1).Position = Point.Projection(this.Handle(1).Position, this.Handle(0).Position, pos);
					this.Handle(3).Position = pos;
					this.Handle(4).Position = Point.Scale(this.Handle(0).Position, this.Handle(1).Position, 0.5);
				}
				else
				{
					this.Handle(3).Position = pos;
					double angle = Point.ComputeAngleDeg(this.Handle(1).Position, this.Handle(3).Position);
					double len = Point.Distance(this.Handle(0).Position, this.Handle(2).Position);
					Point p = this.Handle(0).Position + new Point(len,0);
					this.Handle(2).Position = Transform.RotatePointDeg(this.Handle(0).Position, angle, p);
				}
			}
			
			if ( rank == 4 )  // milieu ?
			{
				if ( this.IsRight )
				{
#if true
					Point p = Point.Projection(this.Handle(0).Position, this.Handle(1).Position, pos);
					Point move = pos-p;

					Point vector = this.Handle(0).Position-this.Handle(2).Position+move;
					Point adjust = vector;
					drawingContext.SnapGridVectorLength(ref adjust);
					adjust -= vector;

					this.Handle(0).Position += move+adjust;
					this.Handle(1).Position += move+adjust;
#else
					Point p = Point.Projection(this.Handle(0).Position, this.Handle(1).Position, pos);
					Point move = pos-p;
					this.Handle(0).Position += move;
					this.Handle(1).Position += move;
#endif
					this.Handle(4).Position = Point.Scale(this.Handle(0).Position, this.Handle(1).Position, 0.5);
				}
				else
				{
					Point move = pos-this.Handle(4).Position;
					this.Handle(4).Position = pos;
					this.Handle(0).Position += move;
					this.Handle(1).Position += move;
				}
			}

			this.HandlePropertiesUpdate();
			this.SetDirtyBbox();
			this.TextInfoModifLine();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}


		// D�place globalement l'objet.
		public override void MoveGlobalProcess(Selector selector)
		{
			base.MoveGlobalProcess(selector);
			this.HandlePropertiesUpdate();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}


		// D�but de la cr�ation d'un objet.
		public override void CreateMouseDown(Point pos, DrawingContext drawingContext)
		{
			if ( this.creatingPhase == 0 )
			{
				drawingContext.ConstrainFlush();
				drawingContext.ConstrainAddHV(pos);
				this.HandleAdd(pos, HandleType.Primary);  // poign�e 0
				this.HandleAdd(pos, HandleType.Primary);  // poign�e 1
			}
			drawingContext.MagnetFixStarting(pos);
			this.isCreating = true;
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// D�placement pendant la cr�ation d'un objet.
		public override void CreateMouseMove(Point pos, DrawingContext drawingContext)
		{
			this.document.Notifier.NotifyArea(this.BoundingBox);

			if ( this.creatingPhase == 0 )
			{
				drawingContext.SnapPos(ref pos);
				this.Handle(1).Position = pos;
			}

			if ( this.creatingPhase == 1 )
			{
				Point p = Point.Projection(this.Handle(2).Position, this.Handle(3).Position, pos);

				Point vector = pos-p;
				Point adjust = vector;
				drawingContext.SnapGridVectorLength(ref adjust);
				adjust -= vector;

				this.Handle(0).Position = this.Handle(2).Position+pos-p+adjust;
				this.Handle(1).Position = this.Handle(3).Position+pos-p+adjust;

				this.Handle(4).Position = Point.Scale(this.Handle(0).Position, this.Handle(1).Position, 0.5);
			}

			this.SetDirtyBbox();
			this.TextInfoModifLine();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// Fin de la cr�ation d'un objet.
		public override void CreateMouseUp(Point pos, DrawingContext drawingContext)
		{
			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapPos(ref pos);

			if ( this.creatingPhase == 0 )
			{
				this.Handle(1).Position = pos;

				this.HandleAdd(pos, HandleType.Secondary);  // poign�e 2
				this.HandleAdd(pos, HandleType.Secondary);  // poign�e 3
				this.HandleAdd(pos, HandleType.Secondary);  // poign�e 4

				this.Handle(2).Position = this.Handle(0).Position;
				this.Handle(3).Position = this.Handle(1).Position;
				this.Handle(4).Position = Point.Scale(this.Handle(0).Position, this.Handle(1).Position, 0.5);

				drawingContext.ConstrainDelStarting();
				drawingContext.ConstrainFlush();
			}
			else if ( this.creatingPhase == 1 )
			{
				drawingContext.ConstrainDelStarting();
			}
			this.creatingPhase ++;

			drawingContext.MagnetClearStarting();
			this.document.Modifier.TextInfoModif = "";
		}

		// Indique si la cr�ation de l'objet est termin�e.
		public override bool CreateIsEnding(DrawingContext drawingContext)
		{
			if ( this.creatingPhase < 2 )
			{
				return !this.CreateIsExist(drawingContext);
			}

			this.isCreating = false;
			this.document.Modifier.TextInfoModif = "";

			this.Deselect();
			drawingContext.ConstrainDelStarting();

			this.HandlePropertiesCreate();
			this.HandlePropertiesUpdate();
			return true;
		}

		// Indique si l'objet doit exister. Retourne false si l'objet ne peut
		// pas exister et doit �tre d�truit.
		public override bool CreateIsExist(DrawingContext drawingContext)
		{
			double len = Point.Distance(this.Handle(0).Position, this.Handle(1).Position);
			return ( len > drawingContext.MinimalSize );
		}

		// Termine la cr�ation de l'objet. Retourne false si l'objet ne peut
		// pas exister et doit �tre d�truit.
		public override bool CreateEnding(DrawingContext drawingContext)
		{
			this.document.Notifier.NotifyArea(this.BoundingBox);
			this.isCreating = false;
			this.document.Modifier.TextInfoModif = "";

			if ( this.creatingPhase < 2 )  return false;

			this.Deselect();
			drawingContext.ConstrainDelStarting();

			this.HandlePropertiesCreate();
			this.HandlePropertiesUpdate();
			return true;
		}


		// Ajoute toutes les fontes utilis�es par l'objet dans une liste.
		public override void FillFontFaceList(System.Collections.ArrayList list)
		{
			string fontName = this.PropertyTextFont.GetFont().FaceName;

			if ( !list.Contains(fontName) )
			{
				list.Add(fontName);
			}
		}

		// Ajoute tous les caract�res utilis�s par l'objet dans une table.
		public override void FillOneCharList(IPaintPort port, DrawingContext drawingContext, System.Collections.Hashtable table)
		{
			Properties.Font propFont = this.PropertyTextFont;
			Drawing.Font font = propFont.GetFont();
			double fontSize = propFont.FontSize;
			string text = this.GetText;

			for ( int i=0 ; i<text.Length ; i++ )
			{
				TextLayout.OneCharStructure oneChar = new TextLayout.OneCharStructure();
				oneChar.Character = text[i];
				oneChar.Font = font;
				oneChar.FontSize = fontSize;
				oneChar.FontColor = propFont.FontColor;

				PDF.CharacterList cl = new PDF.CharacterList(oneChar);
				if ( !table.ContainsKey(cl) )
				{
					table.Add(cl, null);
				}
			}
		}

		
		// Conversion d'une longueur en cha�ne.
		protected string ToString(double value)
		{
			value *= this.document.Modifier.DimensionScale;
			double precision = 1.0/System.Math.Pow(10.0, this.document.Modifier.DimensionDecimal);

			value /= 10.0;
			value /= precision;
			value = System.Math.Floor(value+0.5);
			value *= precision;
			return value.ToString(System.Globalization.CultureInfo.InvariantCulture);
		}

		// Retourne le texte � mettre sur la cote.
		protected string GetText
		{
			get
			{
				Properties.Dimension dimension = this.PropertyDimension;
				double length = Point.Distance(this.Handle(0).Position, this.Handle(1).Position);
				if ( !System.Globalization.RegionInfo.CurrentRegion.IsMetric )
				{
					length /= 25.4;  // en pouces
				}
				string num = this.ToString(length);

				string text = dimension.DimensionText;
				text = text.Replace("##", "\x0001");  // "##" -> "#"
				text = text.Replace("#", num);        // "#" -> valeur num�rique
				text = text.Replace("\x0001", "#");

				return text;
			}
		}

		// Constuit les formes de l'objet.
		public override Shape[] ShapesBuild(IPaintPort port, DrawingContext drawingContext, bool simplify)
		{
			Path pathStart, pathEnd, pathLine, pathSupport, pathText;
			bool outlineStart, outlineEnd, surfaceStart, surfaceEnd;
			this.PathBuild(drawingContext,
						   out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine, out pathSupport, out pathText);

			int totalShapes = 4;
			if ( surfaceStart )  totalShapes ++;
			if ( surfaceEnd   )  totalShapes ++;
			if ( outlineStart )  totalShapes ++;
			if ( outlineEnd   )  totalShapes ++;
			
			Shape[] shapes = new Shape[totalShapes];
			int i = 0;
			
			// Forme du chemin principal.
			shapes[i] = new Shape();
			shapes[i].Path = pathLine;
			shapes[i].SetPropertyStroke(port, this.PropertyLineMode, this.PropertyLineColor);
			i ++;

			// Forme de la surface de d�part.
			if ( surfaceStart )
			{
				shapes[i] = new Shape();
				shapes[i].Path = pathStart;
				shapes[i].SetPropertySurface(port, this.PropertyLineColor);
				i ++;
			}

			// Forme de la surface d'arriv�e.
			if ( surfaceEnd )
			{
				shapes[i] = new Shape();
				shapes[i].Path = pathEnd;
				shapes[i].SetPropertySurface(port, this.PropertyLineColor);
				i ++;
			}

			// Forme du chemin de d�part.
			if ( outlineStart )
			{
				shapes[i] = new Shape();
				shapes[i].Path = pathStart;
				shapes[i].SetPropertyStroke(port, this.PropertyLineMode, this.PropertyLineColor);
				i ++;
			}

			// Forme du chemin d'arriv�e.
			if ( outlineEnd )
			{
				shapes[i] = new Shape();
				shapes[i].Path = pathEnd;
				shapes[i].SetPropertyStroke(port, this.PropertyLineMode, this.PropertyLineColor);
				i ++;
			}

			// Forme des traits de support.
			shapes[i] = new Shape();
			shapes[i].Path = pathSupport;
			shapes[i].SetPropertyStroke(port, this.PropertyLineDimension, this.PropertyLineColor);
			i ++;

			// Caract�res du texte.
			shapes[i] = new Shape();
			shapes[i].SetTextObject(this);
			i ++;

			// Caract�res du texte pour bbox et d�tection
			shapes[i] = new Shape();
			shapes[i].Path = pathText;
			shapes[i].Type = Type.Surface;
			shapes[i].Aspect = Aspect.InvisibleBox;
			i ++;

			return shapes;
		}

		// Cr�e les 3 chemins de l'objet.
		protected void PathBuild(DrawingContext drawingContext,
								 out Path pathStart, out bool outlineStart, out bool surfaceStart,
								 out Path pathEnd,   out bool outlineEnd,   out bool surfaceEnd,
								 out Path pathLine, out Path pathSupport, out Path pathText)
		{
			pathStart   = new Path();
			pathEnd     = new Path();
			pathLine    = new Path();
			pathSupport = new Path();
			pathText    = new Path();

			Properties.Dimension dimension = this.PropertyDimension;
			Properties.Font propFont = this.PropertyTextFont;
			Drawing.Font font = propFont.GetFont();
			double fontSize = propFont.FontSize;
			string text = this.GetText;

			double textWidth = 0.0;
			for ( int i=0 ; i<text.Length ; i++ )
			{
				textWidth += font.GetCharAdvance(text[i])*fontSize;
			}

			double zoom = Properties.Abstract.DefaultZoom(drawingContext);
			pathStart.DefaultZoom = zoom;
			pathEnd.DefaultZoom = zoom;
			pathLine.DefaultZoom = zoom;

			Point p1 = this.Handle(0).Position;
			Point p2 = this.Handle(1).Position;
			double w = this.PropertyLineMode.Width;
			CapStyle cap = this.PropertyLineMode.Cap;
			Point pp1 = this.PropertyDimensionArrow.PathExtremity(pathStart, 0, w,cap, p1,p2, false, out outlineStart, out surfaceStart);
			Point pp2 = this.PropertyDimensionArrow.PathExtremity(pathEnd,   1, w,cap, p2,p1, false, out outlineEnd,   out surfaceEnd);
			double length = Point.Distance(pp1, pp2);

			Properties.DimensionJustif cj = dimension.DimensionJustif;
			Properties.DimensionForm   cf = dimension.DimensionForm;

			if ( cf == Properties.DimensionForm.Auto )
			{
				cf = (textWidth < length) ? Properties.DimensionForm.Inside : Properties.DimensionForm.Outside;
			}

			if ( cf == Properties.DimensionForm.Outside )  // ->|-12-|<- ?
			{
				length = Point.Distance(p1, p2);
			}

			int justif = 0;
			switch ( cj )
			{
				case Properties.DimensionJustif.CenterOrLeft:
					if ( textWidth >= length )  justif = -1;
					break;
				case Properties.DimensionJustif.CenterOrRight:
					if ( textWidth >= length )  justif = 1;
					break;
				case Properties.DimensionJustif.Left:
					justif = -1;
					break;
				case Properties.DimensionJustif.Right:
					justif = 1;
					break;
			}

			double textPos = Point.Distance(p1, p2)/2;

			if ( cf == Properties.DimensionForm.Inside )  // |<-12->| ?
			{
				if ( justif == 0 )
				{
					if ( dimension.FontOffset > 0.0 )
					{
						pathLine.MoveTo(pp1);
						pathLine.LineTo(pp2);
					}
					else
					{
						double use = (length-textWidth)/2;

						pathLine.MoveTo(pp1);
						pathLine.LineTo(Point.Move(pp1, pp2, use));

						pathLine.MoveTo(pp2);
						pathLine.LineTo(Point.Move(pp2, pp1, use));
					}
				}

				if ( justif == 1 )
				{
					textPos = Point.Distance(p1, p2);
					if ( dimension.FontOffset > 0.0 )
					{
						pathLine.MoveTo(pp1);
						pathLine.LineTo(Point.Move(p2, p1, -textWidth));
					}
					else
					{
						pathLine.MoveTo(pp1);
						pathLine.LineTo(pp2);
					}
				}

				if ( justif == -1 )
				{
					textPos = 0.0;
					if ( dimension.FontOffset > 0.0 )
					{
						pathLine.MoveTo(pp2);
						pathLine.LineTo(Point.Move(p1, p2, -textWidth));
					}
					else
					{
						pathLine.MoveTo(pp1);
						pathLine.LineTo(pp2);
					}
				}
			}
			else	// ->|-12-|<- ?
			{
				pathStart = new Path();
				pathEnd   = new Path();
				pp1 = this.PropertyDimensionArrow.PathExtremity(pathStart, 0, w,cap, p1,Point.Scale(p1,p2,-1), false, out outlineStart, out surfaceStart);
				pp2 = this.PropertyDimensionArrow.PathExtremity(pathEnd,   1, w,cap, p2,Point.Scale(p2,p1,-1), false, out outlineEnd,   out surfaceEnd);
				double lex1 = Point.Distance(p1, pp1);
				double lex2 = Point.Distance(p2, pp2);

				pp1 = Point.Move(p1, p2, -(lex1+dimension.OutLength));
				pp2 = Point.Move(p2, p1, -(lex2+dimension.OutLength));

				if ( justif == 0 )
				{
					if ( dimension.FontOffset > 0.0 )
					{
						pathLine.MoveTo(pp1);
						pathLine.LineTo(pp2);
					}
					else
					{
						length = Point.Distance(pp1, pp2);
						double use = (length-textWidth)/2;

						pathLine.MoveTo(pp1);
						pathLine.LineTo(Point.Move(pp1, pp2, use));

						pathLine.MoveTo(pp2);
						pathLine.LineTo(Point.Move(pp2, pp1, use));
					}
				}

				if ( justif == 1 )
				{
					textPos = Point.Distance(p1, p2)+lex1+dimension.OutLength;
					if ( dimension.FontOffset > 0.0 )
					{
						pathLine.MoveTo(pp1);
						pathLine.LineTo(Point.Move(p2, p1, -(lex1+dimension.OutLength+textWidth)));
					}
					else
					{
						pathLine.MoveTo(pp1);
						pathLine.LineTo(Point.Move(p2, p1, -(lex1+dimension.OutLength)));
					}
				}

				if ( justif == -1 )
				{
					textPos = -(lex2+dimension.OutLength);
					if ( dimension.FontOffset > 0.0 )
					{
						pathLine.MoveTo(pp2);
						pathLine.LineTo(Point.Move(p1, p2, -(lex2+dimension.OutLength+textWidth)));
					}
					else
					{
						pathLine.MoveTo(pp2);
						pathLine.LineTo(Point.Move(p1, p2, -(lex2+dimension.OutLength)));
					}
				}
			}

			if ( this.TotalMainHandle == 5 )
			{
				Point s1 = this.Handle(2).Position;
				Point s2 = this.Handle(3).Position;

				if ( !Geometry.Compare(s1, p1) )
				{
					pathSupport.MoveTo(Point.Move(p1, s1, -dimension.AddLength));
					pathSupport.LineTo(s1);
				}

				if ( !Geometry.Compare(s2, p2) )
				{
					pathSupport.MoveTo(Point.Move(p2, s2, -dimension.AddLength));
					pathSupport.LineTo(s2);
				}
			}

			double angle = Point.ComputeAngleDeg(this.Handle(0).Position, this.Handle(1).Position);
			if ( dimension.RotateText )  angle += 180.0;
			Point center = Point.Move(this.Handle(0).Position, this.Handle(1).Position, textPos);
			double advance = -textWidth/2.0;
			if ( justif ==  1 )  advance = 0;
			if ( justif == -1 )  advance = -textWidth;
			double offset = font.Ascender*fontSize*dimension.FontOffset;

			for ( int i=0 ; i<text.Length ; i++ )
			{
				Transform transform = new Transform();
				transform.Scale(fontSize);
				transform.RotateDeg(angle);
				transform.Translate(center+Transform.RotatePointDeg(angle, new Point(advance, offset)));

				int glyph = font.GetGlyphIndex(text[i]);
				pathText.Append(font, glyph, transform);

				advance += font.GetCharAdvance(text[i])*fontSize;
			}
		}


		// Retourne le chemin g�om�trique de l'objet pour les constructions
		// magn�tiques.
		public override Path GetMagnetPath()
		{
			Path path = new Path();

			path.MoveTo(this.Handle(0).Position);
			path.LineTo(this.Handle(1).Position);

			path.MoveTo(this.Handle(0).Position);
			path.LineTo(this.Handle(2).Position);

			path.MoveTo(this.Handle(1).Position);
			path.LineTo(this.Handle(3).Position);

			return path;
		}

		// Retourne le chemin g�om�trique de l'objet.
		public override Path GetPath(int rank)
		{
			if ( rank > 0 )  return null;

			Path pathStart, pathEnd, pathLine, pathSupport, pathText;
			bool outlineStart, outlineEnd, surfaceStart, surfaceEnd;
			this.PathBuild(null,
						   out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine, out pathSupport, out pathText);

			if ( outlineStart || surfaceStart )
			{
				pathLine.Append(pathStart, 0.0);
			}

			if ( outlineEnd || surfaceEnd )
			{
				pathLine.Append(pathEnd, 0.0);
			}

			pathLine.Append(pathSupport, 0.0);
			pathLine.Append(pathText, 0.0);

			return pathLine;
		}

		// Cr�e une ligne � partir de 2 points.
		public void CreateFromPoints(Point p1, Point p2)
		{
			this.HandleAdd(p1, HandleType.Primary);
			this.HandleAdd(p2, HandleType.Primary);
			this.SetDirtyBbox();
		}


		// Indique si la cote et les 2 traits de supports sont � angles droits.
		protected bool IsRight
		{
			get
			{
				return ( Geometry.IsRight(this.Handle(1).Position, this.Handle(0).Position, this.Handle(2).Position) &&
						 Geometry.IsRight(this.Handle(0).Position, this.Handle(1).Position, this.Handle(3).Position) );
			}
		}


		// Dessine le texte de la cote.
		public override void DrawText(IPaintPort port, DrawingContext drawingContext)
		{
			Path pathStart = new Path();
			Path pathEnd   = new Path();
			bool outlineStart, outlineEnd, surfaceStart, surfaceEnd;

			Properties.Dimension dimension = this.PropertyDimension;
			Properties.Font propFont = this.PropertyTextFont;
			Drawing.Font font = propFont.GetFont();
			double fontSize = propFont.FontSize;
			string text = this.GetText;

			double textWidth = 0.0;
			for ( int i=0 ; i<text.Length ; i++ )
			{
				textWidth += font.GetCharAdvance(text[i])*fontSize;
			}

			Point p1 = this.Handle(0).Position;
			Point p2 = this.Handle(1).Position;
			double w = this.PropertyLineMode.Width;
			CapStyle cap = this.PropertyLineMode.Cap;
			Point pp1 = this.PropertyDimensionArrow.PathExtremity(pathStart, 0, w,cap, p1,p2, false, out outlineStart, out surfaceStart);
			Point pp2 = this.PropertyDimensionArrow.PathExtremity(pathEnd,   1, w,cap, p2,p1, false, out outlineEnd,   out surfaceEnd);
			double length = Point.Distance(pp1, pp2);

			Properties.DimensionJustif cj = dimension.DimensionJustif;
			Properties.DimensionForm   cf = dimension.DimensionForm;

			if ( cf == Properties.DimensionForm.Auto )
			{
				cf = (textWidth < length) ? Properties.DimensionForm.Inside : Properties.DimensionForm.Outside;
			}

			if ( cf == Properties.DimensionForm.Outside )  // ->|-12-|<- ?
			{
				length = Point.Distance(p1, p2);
			}

			int justif = 0;
			switch ( cj )
			{
				case Properties.DimensionJustif.CenterOrLeft:
					if ( textWidth >= length )  justif = -1;
					break;
				case Properties.DimensionJustif.CenterOrRight:
					if ( textWidth >= length )  justif = 1;
					break;
				case Properties.DimensionJustif.Left:
					justif = -1;
					break;
				case Properties.DimensionJustif.Right:
					justif = 1;
					break;
			}

			double textPos = Point.Distance(p1, p2)/2;

			if ( cf == Properties.DimensionForm.Inside )  // |<-12->| ?
			{
				if ( justif == 1 )
				{
					textPos = Point.Distance(p1, p2);
				}

				if ( justif == -1 )
				{
					textPos = 0.0;
				}
			}
			else	// ->|-12-|<- ?
			{
				pathStart = new Path();
				pathEnd   = new Path();
				pp1 = this.PropertyDimensionArrow.PathExtremity(pathStart, 0, w,cap, p1,Point.Scale(p1,p2,-1), false, out outlineStart, out surfaceStart);
				pp2 = this.PropertyDimensionArrow.PathExtremity(pathEnd,   1, w,cap, p2,Point.Scale(p2,p1,-1), false, out outlineEnd,   out surfaceEnd);
				double lex1 = Point.Distance(p1, pp1);
				double lex2 = Point.Distance(p2, pp2);

				pp1 = Point.Move(p1, p2, -(lex1+dimension.OutLength));
				pp2 = Point.Move(p2, p1, -(lex2+dimension.OutLength));

				if ( justif == 1 )
				{
					textPos = Point.Distance(p1, p2)+lex1+dimension.OutLength;
				}

				if ( justif == -1 )
				{
					textPos = -(lex2+dimension.OutLength);
				}
			}

			double angle = Point.ComputeAngleDeg(this.Handle(0).Position, this.Handle(1).Position);
			if ( dimension.RotateText )  angle += 180.0;
			Point center = Point.Move(this.Handle(0).Position, this.Handle(1).Position, textPos);
			double advance = -textWidth/2.0;
			if ( justif ==  1 )  advance = 0;
			if ( justif == -1 )  advance = -textWidth;
			double offset = font.Ascender*fontSize*dimension.FontOffset;

			Transform ot = port.Transform;

			Transform transform = new Transform();
			transform.Scale(fontSize);
			transform.RotateDeg(angle);
			transform.Translate(center+Transform.RotatePointDeg(angle, new Point(advance, offset)));
			port.MergeTransform(transform);

			port.RichColor = propFont.FontColor;
			port.PaintText(0.0, 0.0, text, font, 1.0);

			port.Transform = ot;
		}


		#region Serialization
		// S�rialise l'objet.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		// Constructeur qui d�s�rialise l'objet.
		protected Dimension(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
		#endregion


		protected int					creatingPhase = 0;
	}
}
