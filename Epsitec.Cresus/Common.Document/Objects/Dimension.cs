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


		// Nom de l'icône.
		public override string IconName
		{
			get { return "manifest:Epsitec.App.DocumentEditor.Images.Dimension.icon"; }
		}


		// Détecte si la souris est sur l'objet.
		public override bool Detect(Point pos)
		{
			if ( this.isHide )  return false;

			Path pathStart;  bool outlineStart, surfaceStart;
			Path pathEnd;    bool outlineEnd,   surfaceEnd;
			Path pathLine, pathSupport, pathText;
			this.PathBuild(null,
							out pathStart, out outlineStart, out surfaceStart,
							out pathEnd,   out outlineEnd,   out surfaceEnd,
							out pathLine, out pathSupport, out pathText);

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			double width = System.Math.Max(this.PropertyLineMode.Width/2, context.MinimalWidth);
			double wSupp = System.Math.Max(this.PropertyLineDimension.Width/2, context.MinimalWidth);

			if (                 Geometry.DetectOutline(pathLine,    width, pos) )  return true;
			if (                 Geometry.DetectOutline(pathSupport, wSupp, pos) )  return true;
			if ( outlineStart && Geometry.DetectOutline(pathStart,   width, pos) )  return true;
			if ( outlineEnd   && Geometry.DetectOutline(pathEnd,     width, pos) )  return true;

			if ( surfaceStart && Geometry.DetectSurface(pathStart, pos) )  return true;
			if ( surfaceEnd   && Geometry.DetectSurface(pathEnd,   pos) )  return true;

			if (                 Geometry.DetectSurface(pathText,  pos) )  return true;

			return false;
		}


		// Début du déplacement d'une poignée.
		public override void MoveHandleStarting(int rank, Point pos, DrawingContext drawingContext)
		{
			base.MoveHandleStarting(rank, pos, drawingContext);

			if ( rank < this.handles.Count )  // poignée de l'objet ?
			{
				drawingContext.ConstrainFlush();

				Handle handle = this.Handle(rank);
				if ( handle.PropertyType == Properties.Type.None )
				{
					if ( rank == 0 || rank == 1 )  // extrémité ?
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

		// Déplace une poignée.
		public override void MoveHandleProcess(int rank, Point pos, DrawingContext drawingContext)
		{
			if ( rank >= 5 )  // poignée d'une propriété ?
			{
				base.MoveHandleProcess(rank, pos, drawingContext);
				return;
			}

			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapPos(ref pos);

			if ( rank == 0 )  // extrémité gauche ?
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

			if ( rank == 1 )  // extrémité droite ?
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
					Point p = Point.Projection(this.Handle(0).Position, this.Handle(1).Position, pos);
					Point move = pos-p;
					this.Handle(0).Position += move;
					this.Handle(1).Position += move;
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


		// Déplace globalement l'objet.
		public override void MoveGlobalProcess(Selector selector)
		{
			base.MoveGlobalProcess(selector);
			this.HandlePropertiesUpdate();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}


		// Début de la création d'un objet.
		public override void CreateMouseDown(Point pos, DrawingContext drawingContext)
		{
			if ( this.creatingPhase == 0 )
			{
				drawingContext.ConstrainFlush();
				drawingContext.ConstrainAddHV(pos);
				this.HandleAdd(pos, HandleType.Primary);  // poignée 0
				this.HandleAdd(pos, HandleType.Primary);  // poignée 1
			}
			drawingContext.MagnetFixStarting(pos);
			this.isCreating = true;
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// Déplacement pendant la création d'un objet.
		public override void CreateMouseMove(Point pos, DrawingContext drawingContext)
		{
			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapPos(ref pos);

			if ( this.creatingPhase == 0 )
			{
				this.Handle(1).Position = pos;
			}

			if ( this.creatingPhase == 1 )
			{
				Point p = Point.Projection(this.Handle(2).Position, this.Handle(3).Position, pos);
				this.Handle(0).Position = this.Handle(2).Position+pos-p;
				this.Handle(1).Position = this.Handle(3).Position+pos-p;
				this.Handle(4).Position = Point.Scale(this.Handle(0).Position, this.Handle(1).Position, 0.5);
			}

			this.SetDirtyBbox();
			this.TextInfoModifLine();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// Fin de la création d'un objet.
		public override void CreateMouseUp(Point pos, DrawingContext drawingContext)
		{
			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapPos(ref pos);

			if ( this.creatingPhase == 0 )
			{
				this.Handle(1).Position = pos;

				this.HandleAdd(pos, HandleType.Secondary);  // poignée 2
				this.HandleAdd(pos, HandleType.Secondary);  // poignée 3
				this.HandleAdd(pos, HandleType.Secondary);  // poignée 4

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

		// Indique si la création de l'objet est terminée.
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
		// pas exister et doit être détruit.
		public override bool CreateIsExist(DrawingContext drawingContext)
		{
			double len = Point.Distance(this.Handle(0).Position, this.Handle(1).Position);
			return ( len > drawingContext.MinimalSize );
		}

		// Termine la création de l'objet. Retourne false si l'objet ne peut
		// pas exister et doit être détruit.
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


		// Met à jour le rectangle englobant l'objet.
		protected override void UpdateBoundingBox()
		{
			if ( this.handles.Count < 2 )  return;

			this.bboxThin = Drawing.Rectangle.FromCorners(this.Handle(0).Position, this.Handle(1).Position);

			Path pathStart;  bool outlineStart, surfaceStart;
			Path pathEnd;    bool outlineEnd,   surfaceEnd;
			Path pathLine, pathSupport, pathText;
			this.PathBuild(null,
							out pathStart, out outlineStart, out surfaceStart,
							out pathEnd,   out outlineEnd,   out surfaceEnd,
							out pathLine, out pathSupport, out pathText);

			Path[] paths = new Path[4];
			paths[0] = pathLine;
			paths[1] = pathStart;
			paths[2] = pathEnd;
			paths[3] = pathText;

			bool[] lineModes = new bool[4];
			lineModes[0] = true;
			lineModes[1] = outlineStart;
			lineModes[2] = outlineEnd;
			lineModes[3] = false;

			bool[] lineColors = new bool[4];
			lineColors[0] = true;
			lineColors[1] = surfaceStart;
			lineColors[2] = surfaceEnd;
			lineColors[3] = false;

			bool[] fillGradients = new bool[4];
			fillGradients[0] = false;
			fillGradients[1] = false;
			fillGradients[2] = false;
			fillGradients[3] = true;

			this.ComputeBoundingBox(paths, lineModes, lineColors, fillGradients);
			Drawing.Rectangle rectThin = this.bboxThin;
			Drawing.Rectangle rectGeom = this.bboxGeom;
			Drawing.Rectangle rectFull = this.bboxFull;

			Path[] supportPaths = new Path[1];
			supportPaths[0] = pathSupport;

			bool[] supportLineModes = new bool[1];
			supportLineModes[0] = true;

			bool[] supportLineColors = new bool[1];
			supportLineColors[0] = false;

			bool[] supportFillGradients = new bool[1];
			supportFillGradients[0] = false;

			this.ComputeBoundingBox(supportPaths, supportLineModes, supportLineColors, supportFillGradients, this.PropertyLineDimension, null, null);

			this.bboxThin.MergeWith(rectThin);
			this.bboxGeom.MergeWith(rectGeom);
			this.bboxFull.MergeWith(rectFull);
		}

		// Conversion d'une longueur en chaîne.
		protected string ToString(double value)
		{
			value *= this.document.Modifier.DimensionScale;
			double precision = 1.0/System.Math.Pow(10.0, this.document.Modifier.DimensionDecimal);

			value /= 10.0;
			value /= precision;
			value = System.Math.Floor(value+0.5);
			value *= precision;
			return value.ToString();
		}

		// Retourne le texte à mettre sur la cote.
		protected string GetText
		{
			get
			{
				Properties.Dimension dimension = this.PropertyDimension;
				double length = Point.Distance(this.Handle(0).Position, this.Handle(1).Position);
				string num = this.ToString(length);
				return string.Format("{0}{1}{2}", dimension.Prefix, num, dimension.Postfix);
			}
		}

		// Crée les 3 chemins de l'objet.
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
			Drawing.Font font = this.PropertyTextFont.GetFont();
			double fontSize = this.PropertyTextFont.FontSize;
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

		// Dessine l'objet.
		public override void DrawGeometry(Graphics graphics, DrawingContext drawingContext)
		{
			base.DrawGeometry(graphics, drawingContext);

			if ( this.TotalHandle < 2 )  return;

			Path pathStart;  bool outlineStart, surfaceStart;
			Path pathEnd;    bool outlineEnd,   surfaceEnd;
			Path pathLine, pathSupport, pathText;
			this.PathBuild(drawingContext,
							out pathStart, out outlineStart, out surfaceStart,
							out pathEnd,   out outlineEnd,   out surfaceEnd,
							out pathLine, out pathSupport, out pathText);

			if ( outlineStart )
			{
				this.surfaceAnchor.LineUse = true;
				this.PropertyLineMode.DrawPath(graphics, drawingContext, pathStart, this.PropertyLineColor, this.surfaceAnchor);
			}
			if ( surfaceStart )
			{
				this.surfaceAnchor.LineUse = false;
				this.PropertyLineColor.RenderSurface(graphics, drawingContext, pathStart, this.surfaceAnchor);
			}

			if ( outlineEnd )
			{
				this.surfaceAnchor.LineUse = true;
				this.PropertyLineMode.DrawPath(graphics, drawingContext, pathEnd, this.PropertyLineColor, this.surfaceAnchor);
			}
			if ( surfaceEnd )
			{
				this.surfaceAnchor.LineUse = false;
				this.PropertyLineColor.RenderSurface(graphics, drawingContext, pathEnd, this.surfaceAnchor);
			}

			this.surfaceAnchor.LineUse = true;
			this.PropertyLineMode.DrawPath(graphics, drawingContext, pathLine, this.PropertyLineColor, this.surfaceAnchor);
			this.PropertyLineDimension.DrawPath(graphics, drawingContext, pathSupport, this.PropertyLineColor, this.surfaceAnchor);

			graphics.Rasterizer.AddSurface(pathText);
			graphics.RenderSolid(drawingContext.AdaptColor(this.PropertyTextFont.FontColor));

			if ( this.IsHilite && drawingContext.IsActive )
			{
				if ( outlineStart )
				{
					this.PropertyLineMode.AddOutline(graphics, pathStart, drawingContext.HiliteSize);
					graphics.RenderSolid(drawingContext.HiliteOutlineColor);
				}
				if ( surfaceStart )
				{
					graphics.Rasterizer.AddSurface(pathStart);
					graphics.RenderSolid(drawingContext.HiliteOutlineColor);
				}

				if ( outlineEnd )
				{
					this.PropertyLineMode.AddOutline(graphics, pathEnd, drawingContext.HiliteSize);
					graphics.RenderSolid(drawingContext.HiliteOutlineColor);
				}
				if ( surfaceEnd )
				{
					graphics.Rasterizer.AddSurface(pathEnd);
					graphics.RenderSolid(drawingContext.HiliteOutlineColor);
				}

				this.PropertyLineMode.AddOutline(graphics, pathLine, drawingContext.HiliteSize);
				graphics.RenderSolid(drawingContext.HiliteOutlineColor);

				this.PropertyLineDimension.AddOutline(graphics, pathSupport, drawingContext.HiliteSize);
				graphics.RenderSolid(drawingContext.HiliteOutlineColor);
			}

			if ( this.IsDrawDash(drawingContext) )
			{
				this.PropertyLineMode.DrawPathDash(graphics, drawingContext, pathLine, this.PropertyLineColor);
				this.PropertyLineDimension.DrawPathDash(graphics, drawingContext, pathSupport, this.PropertyLineColor);

				if ( outlineStart )
				{
					this.PropertyLineMode.DrawPathDash(graphics, drawingContext, pathStart, this.PropertyLineColor);
				}

				if ( outlineEnd )
				{
					this.PropertyLineMode.DrawPathDash(graphics, drawingContext, pathEnd, this.PropertyLineColor);
				}
			}
		}

		// Imprime l'objet.
		public override void PrintGeometry(Printing.PrintPort port, DrawingContext drawingContext)
		{
			base.PrintGeometry(port, drawingContext);

			if ( this.TotalHandle < 2 )  return;

			Path pathStart;  bool outlineStart, surfaceStart;
			Path pathEnd;    bool outlineEnd,   surfaceEnd;
			Path pathLine, pathSupport, pathText;
			this.PathBuild(drawingContext,
							out pathStart, out outlineStart, out surfaceStart,
							out pathEnd,   out outlineEnd,   out surfaceEnd,
							out pathLine, out pathSupport, out pathText);

			if ( this.PropertyLineColor.PaintColor(port, drawingContext) )
			{
				if ( outlineStart )
				{
					this.PropertyLineMode.PaintOutline(port, drawingContext, pathStart);
				}
				if ( surfaceStart )
				{
					port.PaintSurface(pathStart);
				}

				if ( outlineEnd )
				{
					this.PropertyLineMode.PaintOutline(port, drawingContext, pathEnd);
				}
				if ( surfaceEnd )
				{
					port.PaintSurface(pathEnd);
				}

				this.PropertyLineMode.PaintOutline(port, drawingContext, pathLine);
				this.PropertyLineDimension.PaintOutline(port, drawingContext, pathSupport);

				port.Color = this.PropertyTextFont.FontColor;
				port.PaintSurface(pathText);
			}
		}

		// Exporte en PDF la géométrie de l'objet.
		public override void ExportPDF(PDFPort port, DrawingContext drawingContext)
		{
			if ( this.TotalHandle < 2 )  return;

			Path pathStart;  bool outlineStart, surfaceStart;
			Path pathEnd;    bool outlineEnd,   surfaceEnd;
			Path pathLine, pathSupport, pathText;
			this.PathBuild(drawingContext,
							out pathStart, out outlineStart, out surfaceStart,
							out pathEnd,   out outlineEnd,   out surfaceEnd,
							out pathLine, out pathSupport, out pathText);

			Properties.Line     lineMode    = this.PropertyLineMode;
			Properties.Line     supportMode = this.PropertyLineDimension;
			Properties.Gradient lineColor   = this.PropertyLineColor;
			Properties.Gradient fillColor   = this.PropertyFillGradient;
			Color               textColor   = this.PropertyTextFont.FontColor;

			// Dessine les surfaces aux extrémités.
			if ( lineColor.IsVisible() )
			{
				if ( surfaceStart || surfaceEnd )
				{
					lineColor.ExportPDF(port, drawingContext);

					if ( surfaceStart )
					{
						port.PaintSurface(pathStart);
					}
					if ( surfaceEnd )
					{
						port.PaintSurface(pathEnd);
					}
				}
			}

			// Dessine le trait et les extrémités.
			if ( lineMode.IsVisible() && lineColor.IsVisible() )
			{
				lineMode.ExportPDF(port, drawingContext);
				lineColor.ExportPDF(port, drawingContext);

				if ( outlineStart )
				{
					port.PaintOutline(pathStart);
				}
				if ( outlineEnd )
				{
					port.PaintOutline(pathEnd);
				}

				port.PaintOutline(pathLine);
			}

			// Dessine les supports.
			if ( supportMode.IsVisible() && lineColor.IsVisible() )
			{
				supportMode.ExportPDF(port, drawingContext);
				lineColor.ExportPDF(port, drawingContext);
				port.PaintOutline(pathSupport);
			}

			// Dessine le texte.
			if ( !textColor.IsEmpty )
			{
				port.Color = textColor;
				port.PaintSurface(pathText);
			}
		}


		// Retourne le chemin géométrique de l'objet pour les constructions
		// magnétiques.
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

		// Retourne le chemin géométrique de l'objet.
		public override Path GetPath(int rank)
		{
			if ( rank > 0 )  return null;
			Path pathStart;  bool outlineStart, surfaceStart;
			Path pathEnd;    bool outlineEnd,   surfaceEnd;
			Path pathLine, pathSupport, pathText;
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

		// Crée une ligne à partir de 2 points.
		public void CreateFromPoints(Point p1, Point p2)
		{
			this.HandleAdd(p1, HandleType.Primary);
			this.HandleAdd(p2, HandleType.Primary);
			this.SetDirtyBbox();
		}


		// Indique si la cote et les 2 traits de supports sont à angles droits.
		protected bool IsRight
		{
			get
			{
				return ( Geometry.IsRight(this.Handle(1).Position, this.Handle(0).Position, this.Handle(2).Position) &&
						 Geometry.IsRight(this.Handle(0).Position, this.Handle(1).Position, this.Handle(3).Position) );
			}
		}


		#region Serialization
		// Sérialise l'objet.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		// Constructeur qui désérialise l'objet.
		protected Dimension(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
		#endregion


		protected int					creatingPhase = 0;
	}
}
