using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Objects
{
	/// <summary>
	/// La classe TextBox2 est la classe de l'objet graphique "pav� de texte".
	/// </summary>
	[System.Serializable()]
	public class TextBox2 : Objects.AbstractText, Text.ITextRenderer
	{
		public TextBox2(Document document, Objects.Abstract model) : base(document, model)
		{
		}

		protected override bool ExistingProperty(Properties.Type type)
		{
			if ( type == Properties.Type.Name )  return true;
			if ( type == Properties.Type.LineMode )  return true;
			if ( type == Properties.Type.LineColor )  return true;
			if ( type == Properties.Type.FillGradient )  return true;
			return false;
		}

		protected override Objects.Abstract CreateNewObject(Document document, Objects.Abstract model)
		{
			//	Cr�e une instance de l'objet.
			return new TextBox2(document, model);
		}

		protected override void Initialise()
		{
			this.textFrame = new Text.SimpleTextFrame();
			base.Initialise();
		}
		
		protected override void InitialiseInternals()
		{
			if ( this.textFrame == null )
			{
				this.textFrame = new Text.SimpleTextFrame();
			}

			base.InitialiseInternals();
		}


		public override string IconName
		{
			//	Nom de l'ic�ne.
			get { return Misc.Icon("ObjectTextBox"); }
		}


		public override void MoveHandleStarting(int rank, Point pos, DrawingContext drawingContext)
		{
			//	D�but du d�placement d'une poign�e.
			base.MoveHandleStarting(rank, pos, drawingContext);

			if ( rank < this.handles.Count )  // poign�e de l'objet ?
			{
				drawingContext.ConstrainFlush();

				Handle handle = this.Handle(rank);
				if ( handle.PropertyType == Properties.Type.None )
				{
						 if ( rank == 0 )  drawingContext.ConstrainAddRect(this.Handle(0).Position, this.Handle(1).Position, this.Handle(2).Position, this.Handle(3).Position);
					else if ( rank == 1 )  drawingContext.ConstrainAddRect(this.Handle(1).Position, this.Handle(0).Position, this.Handle(3).Position, this.Handle(2).Position);
					else if ( rank == 2 )  drawingContext.ConstrainAddRect(this.Handle(2).Position, this.Handle(3).Position, this.Handle(0).Position, this.Handle(1).Position);
					else if ( rank == 3 )  drawingContext.ConstrainAddRect(this.Handle(3).Position, this.Handle(2).Position, this.Handle(1).Position, this.Handle(0).Position);
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

			this.textFlow.NotifyAreaFlow();
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

			this.UpdateGeometry();
			this.HandlePropertiesUpdate();
			this.SetDirtyBbox();
			this.TextInfoModifRect();
			this.textFlow.NotifyAreaFlow();
		}

		public override void MoveAllProcess(Point move)
		{
			//	Effectue le d�placement de tout l'objet.
			base.MoveAllProcess(move);
			this.UpdateGeometry();
		}

		public override void MoveGlobalProcess(Selector selector)
		{
			//	D�place globalement l'objet.
			base.MoveGlobalProcess(selector);
			this.UpdateGeometry();
			this.HandlePropertiesUpdate();
			this.textFlow.NotifyAreaFlow();
		}

		public override void CreateMouseDown(Point pos, DrawingContext drawingContext)
		{
			//	D�but de la cr�ation d'un objet.
			drawingContext.ConstrainFlush();
			drawingContext.ConstrainAddHomo(pos);
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
			this.UpdateGeometry();
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
			Drawing.Rectangle rect = Drawing.Rectangle.FromCorners(this.Handle(0).Position, this.Handle(1).Position);
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


		public override Drawing.Rectangle RealBoundingBox()
		{
			//	Retourne la bounding r�elle, en fonction des caract�res contenus.
			this.mergingBoundingBox = Drawing.Rectangle.Empty;
			this.DrawText(null, null, InternalOperation.RealBoundingBox);

			return this.mergingBoundingBox;
		}


		public override Shape[] ShapesBuild(IPaintPort port, DrawingContext drawingContext, bool simplify)
		{
			//	Construit les formes de l'objet.
			Path path = this.PathBuild();

			bool flowHandles = this.edited && drawingContext != null;

			int totalShapes = 4;
			if ( flowHandles )  totalShapes += 2;

			Shape[] shapes = new Shape[totalShapes];
			int i = 0;
			
			//	Forme de la surface.
			shapes[i] = new Shape();
			shapes[i].Path = path;
			shapes[i].SetPropertySurface(port, this.PropertyFillGradient);
			i ++;

			//	Traits du rectangle.
			shapes[i] = new Shape();
			shapes[i].Path = path;
			shapes[i].SetPropertyStroke(port, this.PropertyLineMode, this.PropertyLineColor);
			i ++;

			//	Caract�res du texte.
			shapes[i] = new Shape();
			shapes[i].SetTextObject(this);
			i ++;

			if ( flowHandles )
			{
				shapes[i] = new Shape();
				shapes[i].Path = this.PathFlowHandlesStroke(port, drawingContext);
				shapes[i].SetPropertyStroke(port, this.PropertyLineMode, this.PropertyLineColor);
				shapes[i].Aspect = Aspect.Support;
				shapes[i].IsVisible = true;
				i ++;

				shapes[i] = new Shape();
				shapes[i].Path = this.PathFlowHandlesSurface(port, drawingContext);
				shapes[i].SetPropertySurface(port, this.PropertyLineColor);
				shapes[i].Aspect = Aspect.Support;
				shapes[i].IsVisible = true;
				i ++;
			}

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


		#region FlowHandles
		protected override void CornersFlowPrev(out Point p1, out Point p2, out Point p3, out Point p4, DrawingContext drawingContext)
		{
			//	Calcules les 4 coins de la poign�e "pav� pr�c�dent".
			Point c1, c2, c3, c4;
			this.Corners(out c1, out c2, out c3, out c4);

			double d = AbstractText.EditFlowHandleSize/drawingContext.ScaleX;
			p1 = c3;
			p2 = Point.Move(c3, c4,  d);
			p3 = Point.Move(c3, c1, -d);
			p4 = p3 + (p2-p1);
		}

		protected override void CornersFlowNext(out Point p1, out Point p2, out Point p3, out Point p4, DrawingContext drawingContext)
		{
			//	Calcules les 4 coins de la poign�e "pav� suivant".
			Point c1, c2, c3, c4;
			this.Corners(out c1, out c2, out c3, out c4);

			double d = AbstractText.EditFlowHandleSize/drawingContext.ScaleX;
			p4 = c2;
			p3 = Point.Move(c2, c1,  d);
			p2 = Point.Move(c2, c4, -d);
			p1 = p2 + (p3-p4);
		}
		#endregion


		protected void Corners(out Point p1, out Point p2, out Point p3, out Point p4)
		{
			//	Calcules les 4 coins.
			Point h0, h1, h2, h3;

			if ( this.handles.Count < 4 )
			{
				Drawing.Rectangle rect = Drawing.Rectangle.FromCorners(this.Handle(0).Position, this.Handle(1).Position);
				h0 = rect.BottomLeft;
				h1 = rect.TopRight;
				h2 = rect.TopLeft;
				h3 = rect.BottomRight;
			}
			else
			{
				h0 = this.Handle(0).Position;
				h1 = this.Handle(1).Position;
				h2 = this.Handle(2).Position;
				h3 = this.Handle(3).Position;
			}

#if false
			switch ( this.PropertyTextJustif.Orientation )
			{
				case Properties.JustifOrientation.RightToLeft:  // <-
					p1 = h1;
					p2 = h2;
					p3 = h3;
					p4 = h0;
					break;
				case Properties.JustifOrientation.BottomToTop:  // ^
					p1 = h3;
					p2 = h1;
					p3 = h0;
					p4 = h2;
					break;
				case Properties.JustifOrientation.TopToBottom:  // v
					p1 = h2;
					p2 = h0;
					p3 = h1;
					p4 = h3;
					break;
				default:							// -> (normal)
					p1 = h0;
					p2 = h3;
					p3 = h2;
					p4 = h1;
					break;
			}
#else
			p1 = h0;
			p2 = h3;
			p3 = h2;
			p4 = h1;
#endif
		}

		protected override void UpdateTextFrame()
		{
			//	Met � jour le TextFrame en fonction des dimensions du pav�.
			Text.SimpleTextFrame frame = this.textFrame as Text.SimpleTextFrame;
			
			Point p1, p2, p3, p4;
			this.Corners(out p1, out p2, out p3, out p4);
			
			double width  = Point.Distance(p1, p2);
			double height = Point.Distance(p1, p3);

			if ( frame.Width   != width  ||
				 frame.Height  != height ||
				 frame.OriginY != p4.Y   )
			{
				frame.OriginY = p4.Y;
				frame.Width   = width;
				frame.Height  = height;
				
				this.textFlow.TextStory.NotifyTextChanged();
			}
		}
		
		public override bool IsInTextFrame(Drawing.Point pos, out Drawing.Point ppos)
		{
			//	D�termine si un point se trouve dans le texte frame.
			if ( this.transform == null )
			{
				ppos = Drawing.Point.Empty;
				return false;
			}
			
			ppos = this.transform.TransformInverse(pos);
			
			Text.SimpleTextFrame frame = this.textFrame as Text.SimpleTextFrame;
			if ( ppos.X < 0 || ppos.Y < 0 || ppos.X > frame.Width || ppos.Y > frame.Height )
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		protected override void DrawText(IPaintPort port, DrawingContext drawingContext, InternalOperation op)
		{
			//	Effectue une op�ration quelconque sur le texte du pav�.
			this.internalOperation = op;

			if ( this.internalOperation == InternalOperation.Painting )
			{
				this.cursorBox = Drawing.Rectangle.Empty;
				this.selectBox = Drawing.Rectangle.Empty;
			}

			Point p1, p2, p3, p4;
			this.Corners(out p1, out p2, out p3, out p4);

			Transform ot = null;
			if ( port != null )
			{
				ot = port.Transform;
			}

			//?double angle = Point.ComputeAngleDeg(p1, p2);
			double angle = this.direction;

			Point pp1 = Transform.RotatePointDeg(p1, -angle, p1);
			Point pp2 = Transform.RotatePointDeg(p1, -angle, p2);
			Point pp3 = Transform.RotatePointDeg(p1, -angle, p3);

			double sx = (pp1.X <= pp2.X) ? 1.0 : -1.0;
			double sy = (pp1.Y <= pp3.Y) ? 1.0 : -1.0;

			this.transform = new Transform();
			this.transform.Translate(p1);
			this.transform.Scale(sx, sy, p1.X, p1.Y);
			this.transform.RotateDeg(angle, p1);
			if ( port != null )
			{
				port.MergeTransform(transform);
			}

			this.port = port;
			this.graphics = port as Graphics;
			this.drawingContext = drawingContext;

			this.isActive = true;
			if ( this.document.Modifier != null )
			{
				this.isActive = (this.document.Modifier.ActiveViewer.DrawingContext == drawingContext &&
								 this.document.Modifier.ActiveViewer.IsFocused);
			}

			this.redrawArea = Drawing.Rectangle.Empty;
			if ( this.drawingContext != null && this.drawingContext.Viewer != null )
			{
				Point pbl = this.transform.TransformInverse(this.drawingContext.Viewer.RedrawArea.BottomLeft);
				Point pbr = this.transform.TransformInverse(this.drawingContext.Viewer.RedrawArea.BottomRight);
				Point ptl = this.transform.TransformInverse(this.drawingContext.Viewer.RedrawArea.TopLeft);
				Point ptr = this.transform.TransformInverse(this.drawingContext.Viewer.RedrawArea.TopRight);
				this.redrawArea.MergeWith(pbl);
				this.redrawArea.MergeWith(pbr);
				this.redrawArea.MergeWith(ptl);
				this.redrawArea.MergeWith(ptr);
			}

			this.textFlow.TextStory.TextContext.ShowControlCharacters = this.textFlow.HasActiveTextBox && this.drawingContext != null && this.drawingContext.TextShowControlCharacters;
			this.textFlow.TextFitter.RenderTextFrame(this.textFrame, this);

			if ( this.textFlow.HasActiveTextBox && !this.textFlow.TextNavigator.HasRealSelection && this.graphics != null && this.internalOperation == InternalOperation.Painting )
			{
				//	Peint le curseur uniquement si l'objet est en �dition, qu'il n'y a pas
				//	de s�lection et que l'on est en train d'afficher � l'�cran.
				Text.ITextFrame frame;
				double cx, cy, ascender, descender;
				this.textFlow.TextNavigator.GetCursorGeometry(out frame, out cx, out cy, out ascender, out descender, out angle);
			
				if ( frame == this.textFrame )
				{
					double tan = System.Math.Tan(System.Math.PI/2.0 - angle);
					Point c1 = new Point(cx+tan*descender, cy+descender);
					Point c2 = new Point(cx+tan*ascender,  cy+ascender);
				
					this.graphics.LineWidth = 1.0/drawingContext.ScaleX;
					this.graphics.AddLine(c1, c2);
					this.graphics.RenderSolid(DrawingContext.ColorCursorEdit(this.isActive));

					c1 = this.transform.TransformDirect(c1);
					c2 = this.transform.TransformDirect(c2);
					this.ComputeAutoScroll(c1, c2);
					this.cursorBox.MergeWith(c1);
					this.cursorBox.MergeWith(c2);
					this.selectBox.MergeWith(c1);
					this.selectBox.MergeWith(c2);
				}
			}

			this.port = null;
			this.graphics = null;
			this.drawingContext = null;

			if ( port != null )
			{
				port.Transform = ot;
			}
		}

		#region ITextRenderer Members
		public bool IsFrameAreaVisible(Text.ITextFrame frame, double x, double y, double width, double height)
		{
			return true;
		}
		
		public void RenderStartParagraph(Text.Layout.Context context)
		{
		}
		
		public void RenderStartLine(Text.Layout.Context context)
		{
			context.DisableSimpleRendering();
		}
		
		public void RenderTab(Text.Layout.Context layout, string tag, double tabOrigin, double tabStop, ulong tabCode, bool isTabDefined)
		{
			if ( this.graphics == null )  return;
			if ( this.drawingContext == null )  return;
			if ( this.drawingContext.TextShowControlCharacters == false )  return;
			if ( this.textFlow.HasActiveTextBox == false )  return;

			double x1 = tabOrigin;
			double x2 = tabStop;
			double y  = layout.LineBaseY + layout.LineAscender*0.3;
			double a  = System.Math.Min(layout.LineAscender*0.3, (x2-x1)*0.5);

			Point p1 = new Point(x1, y);
			Point p2 = new Point(x2, y);
			graphics.Align(ref p1);
			graphics.Align(ref p2);
			double adjust = 0.5/this.drawingContext.ScaleX;
			p1.X += adjust;  p1.Y += adjust;
			p2.X -= adjust;  p2.Y += adjust;
			if ( p1.X >= p2.X )  return;

			Point p2a = new Point(p2.X-a, p2.Y-a*0.75);
			Point p2b = new Point(p2.X-a, p2.Y+a*0.75);

			Color color = isTabDefined ? Drawing.Color.FromBrightness(0.8) : DrawingContext.ColorTabZombie;
			
			if ( (tabCode & this.markerSelected) != 0 )  // tabulateur s�lectionn� ?
			{
				Drawing.Rectangle rect = new Drawing.Rectangle(x1, layout.LineY1, x2-x1, layout.LineY2-layout.LineY1);
				graphics.Align(ref rect);
				
				this.graphics.AddFilledRectangle(rect);
				this.graphics.RenderSolid(DrawingContext.ColorSelectEdit(this.isActive));

				if ( isTabDefined )  color = Drawing.Color.FromBrightness(0.5);
			}
			
			this.graphics.LineWidth = 1.0/this.drawingContext.ScaleX;
			this.graphics.AddLine(p1, p2);
			this.graphics.AddLine(p2, p2a);
			this.graphics.AddLine(p2, p2b);
			this.graphics.RenderSolid(color);
		}
		
		public void Render(Text.Layout.Context layout, Epsitec.Common.OpenType.Font font, double size, string color, Text.Layout.TextToGlyphMapping mapping, ushort[] glyphs, double[] x, double[] y, double[] sx, double[] sy, bool isLastRun)
		{
			if ( this.internalOperation == InternalOperation.Painting )
			{
				System.Diagnostics.Debug.Assert(mapping != null);
				Text.ITextFrame frame = layout.Frame;

				//	V�rifions d'abord que le mapping du texte vers les glyphes est
				//	correct et correspond � quelque chose de valide :
				int  offset = 0;
				bool isInSelection = false;
				double selX = 0;

				System.Collections.ArrayList selRectList = null;

				double x1 = 0;
				double x2 = 0;

				int[]    cArray;  // unicodes
				ushort[] gArray;  // glyphes
				ulong[]  tArray;  // textes

				SpaceType[] iArray = new SpaceType[glyphs.Length];
				int ii = 0;
				bool isSpace = false;

				while ( mapping.GetNextMapping(out cArray, out gArray, out tArray) )
				{
					int numChars  = cArray.Length;
					int numGlyphs = gArray.Length;
					System.Diagnostics.Debug.Assert(numChars == 1 || numGlyphs == 1);

					x1 = x[offset+0];
					x2 = x[offset+numGlyphs];

					if ( numChars == 1 && numGlyphs == 1 )
					{
						int code = cArray[0];
						if ( code == 0x20 || code == 0xA0 || code == 0x202F || (code >= 0x2000 && code <= 0x200F) )  // espace ?
						{
							isSpace = true;  // contient au moins un espace
							if ( code == 0xA0 || code == 0x2007 || code == 0x200D || code == 0x202F || code == 0x2060 )
							{
								iArray[ii++] = SpaceType.NoBreakSpace;  // espace ins�cable
							}
							else
							{
								iArray[ii++] = SpaceType.BreakSpace;  // espace s�cable
							}
						}
						else if ( code == 0x0C )  // saut ?
						{
							isSpace = true;  // contient au moins un espace

							Text.Properties.BreakProperty prop;
							this.document.TextContext.GetBreak(tArray[0], out prop);
							if ( prop.ParagraphStartMode == Text.Properties.ParagraphStartMode.NewFrame )
							{
								iArray[ii++] = SpaceType.NewFrame;
							}
							else
							{
								iArray[ii++] = SpaceType.NewPage;
							}
						}
						else
						{
							iArray[ii++] = SpaceType.None;  // pas un espace
						}
					}
					else
					{
						for ( int i=0 ; i<numGlyphs ; i++ )
						{
							iArray[ii++] = SpaceType.None;  // pas un espace
						}
					}

					for ( int i=0 ; i<numChars ; i++ )
					{
						if ( (tArray[i] & this.markerSelected) != 0 )
						{
							//	Le caract�re consid�r� est s�lectionn�.
							if ( isInSelection == false )
							{
								//	C'est le premier caract�re d'une tranche. Il faut m�moriser son d�but :
								double xx = x1 + ((x2 - x1) * i) / numChars;
								isInSelection = true;
								selX = xx;
							}
						}
						else
						{
							if ( isInSelection )
							{
								//	Nous avons quitt� une tranche s�lectionn�e. Il faut m�moriser sa fin :
								double xx = x1 + ((x2 - x1) * i) / numChars;
								isInSelection = false;

								if ( xx > selX )
								{
									this.MarkSel(layout, ref selRectList, xx, selX);
								}
							}
						}
					}

					offset += numGlyphs;
				}

				if ( isInSelection )
				{
					//	Nous avons quitt� une tranche s�lectionn�e. Il faut m�moriser sa fin :
					double xx = x2;
					isInSelection = false;

					if ( xx > selX )
					{
						this.MarkSel(layout, ref selRectList, xx, selX);
					}
				}

				if ( this.textFlow.HasActiveTextBox && selRectList != null && this.graphics != null )
				{
					//	Dessine les rectangles correspondant � la s�lection.
					foreach ( Drawing.Rectangle rect in selRectList )
					{
						this.graphics.AddFilledRectangle(rect);

						Point c1 = this.transform.TransformDirect(rect.BottomLeft);
						Point c2 = this.transform.TransformDirect(rect.TopRight);
						this.selectBox.MergeWith(c1);
						this.selectBox.MergeWith(c2);
					}
					this.graphics.RenderSolid(DrawingContext.ColorSelectEdit(this.isActive));
				}

				//	Dessine le texte.
				this.RenderText(font, size, glyphs, iArray, x, y, sx, sy, RichColor.Parse(color), isSpace);
			}
			
			if ( this.internalOperation == InternalOperation.GetPath )
			{
				this.RenderText(font, size, glyphs, null, x, y, sx, sy, RichColor.Empty, false);
			}

			if ( this.internalOperation == InternalOperation.CharactersTable )
			{
				int[]    cArray;
				ushort[] gArray;
				ulong[]  tArray;
				while ( mapping.GetNextMapping(out cArray, out gArray, out tArray) )
				{
					int numChars  = cArray.Length;
					int numGlyphs = gArray.Length;
					System.Diagnostics.Debug.Assert(numChars == 1 || numGlyphs == 1);

					for ( int i=0 ; i<numGlyphs ; i++ )
					{
						if ( gArray[i] >= 0xffff )  continue;

						PDF.CharacterList cl;
						if ( numChars == 1 )
						{
							if ( i == 1 )  // TODO: c�sure g�r�e de fa�on catastrophique !
							{
								cl = new PDF.CharacterList(gArray[i], (int)'-', font);
							}
							else
							{
								cl = new PDF.CharacterList(gArray[i], cArray[0], font);
							}
						}
						else
						{
							cl = new PDF.CharacterList(gArray[i], cArray, font);
						}

						if ( !this.charactersTable.ContainsKey(cl) )
						{
							this.charactersTable.Add(cl, null);
						}
					}
				}
			}

			if ( this.internalOperation == InternalOperation.RealBoundingBox )
			{
				this.RenderText(font, size, glyphs, null, x, y, sx, sy, RichColor.Empty, false);
			}
		}

		protected void MarkSel(Text.Layout.Context layout, ref System.Collections.ArrayList selRectList, double x, double selX)
		{
			//	Marque une tranche s�lectionn�e.
			if ( this.graphics == null )  return;

			double dx = x - selX;
			double dy = layout.LineY2 - layout.LineY1;
			Drawing.Rectangle rect = new Drawing.Rectangle(selX, layout.LineY1, dx, dy);
			graphics.Align(ref rect);

			if ( selRectList == null )
			{
				selRectList = new System.Collections.ArrayList();
			}

			selRectList.Add(rect);
		}

		protected void RenderText(Epsitec.Common.OpenType.Font font, double size, ushort[] glyphs, SpaceType[] insecs, double[] x, double[] y, double[] sx, double[] sy, RichColor color, bool isSpace)
		{
			//	Effectue le rendu des caract�res.
			if ( this.internalOperation == InternalOperation.Painting )
			{
				if ( this.graphics != null )  // affichage sur �cran ?
				{
					Drawing.Font drawingFont = Drawing.Font.GetFont(font);
					if ( drawingFont != null )
					{
						if ( sy == null )
						{
							this.graphics.Rasterizer.AddGlyphs(drawingFont, size, glyphs, x, y, sx);
						}
						else
						{
							for ( int i=0 ; i<glyphs.Length ; i++ )
							{
								if ( glyphs[i] < 0xffff )
								{
									this.graphics.Rasterizer.AddGlyph(drawingFont, glyphs[i], x[i], y[i], size, (sx == null) ? 1.0 : sx[i], (sy == null) ? 1.0 : sy[i]);
								}
							}
						}

						if ( this.textFlow.HasActiveTextBox && isSpace && insecs != null &&
							 this.drawingContext != null && this.drawingContext.TextShowControlCharacters )
						{
							for ( int i=0 ; i<glyphs.Length ; i++ )
							{
								double width = font.GetGlyphWidth(glyphs[i], size);
								double oy = font.GetAscender(size)*0.3;

								if ( insecs[i] == SpaceType.BreakSpace )  // espace s�cable ?
								{
									this.graphics.AddFilledCircle(x[i]+width/2, y[i]+oy, size*0.05);
								}

								if ( insecs[i] == SpaceType.NoBreakSpace )  // espace ins�cable ?
								{
									this.graphics.AddCircle(x[i]+width/2, y[i]+oy, size*0.08);
								}

								if ( insecs[i] == SpaceType.NewFrame ||
									 insecs[i] == SpaceType.NewPage  )  // saut ?
								{
									Text.SimpleTextFrame frame = this.textFrame as Text.SimpleTextFrame;
									Point p1 = new Point(x[i],        y[i]+oy);
									Point p2 = new Point(frame.Width, y[i]+oy);
									Path path = Path.FromLine(p1, p2);

									double w    = (insecs[i] == SpaceType.NewFrame) ? 0.8 : 0.5;
									double dash = (insecs[i] == SpaceType.NewFrame) ? 0.0 : 8.0;
									double gap  = (insecs[i] == SpaceType.NewFrame) ? 3.0 : 2.0;
									Drawer.DrawPathDash(this.graphics, this.drawingContext, path, w, dash, gap, color.Basic);
								}
							}
						}
					}
			
					this.graphics.RenderSolid(color.Basic);
				}
				else if ( this.port is Printing.PrintPort )  // impression ?
				{
					Printing.PrintPort printPort = port as Printing.PrintPort;
					Drawing.Font drawingFont = Drawing.Font.GetFont(font);
					printPort.RichColor = color;
					printPort.PaintGlyphs(drawingFont, size, glyphs, x, y, sx, sy);
				}
				else if ( this.port is PDF.Port )  // exportation PDF ?
				{
					PDF.Port pdfPort = port as PDF.Port;
					Drawing.Font drawingFont = Drawing.Font.GetFont(font);
					pdfPort.RichColor = color;
					pdfPort.PaintGlyphs(drawingFont, size, glyphs, x, y, sx, sy);
				}
			}

			if ( this.internalOperation == InternalOperation.GetPath )
			{
				Drawing.Font drawingFont = Drawing.Font.GetFont(font);
				this.graphics.PaintGlyphs(drawingFont, size, glyphs, x, y, sx, sy);
			}

			if ( this.internalOperation == InternalOperation.RealBoundingBox )
			{
				Drawing.Font drawingFont = Drawing.Font.GetFont(font);
				if ( drawingFont != null )
				{
					for ( int i=0 ; i<glyphs.Length ; i++ )
					{
						if ( glyphs[i] < 0xffff )
						{
							Drawing.Rectangle bounds = drawingFont.GetGlyphBounds(glyphs[i], size);

							if ( sx != null )  bounds.Scale(sx[i], 1.0);
							if ( sy != null )  bounds.Scale(1.0, sy[i]);

							bounds.Offset(x[i], y[i]);

							this.mergingBoundingBox.MergeWith(this.transform.TransformDirect(bounds.BottomLeft));
							this.mergingBoundingBox.MergeWith(this.transform.TransformDirect(bounds.BottomRight));
							this.mergingBoundingBox.MergeWith(this.transform.TransformDirect(bounds.TopLeft));
							this.mergingBoundingBox.MergeWith(this.transform.TransformDirect(bounds.TopRight));
						}
					}
				}
			}
		}

		public void Render(Text.Layout.Context layout, Text.IGlyphRenderer glyphRenderer, string color, double x, double y, bool isLastRun)
		{
			glyphRenderer.RenderGlyph(layout.Frame, x, y);
		}
		
		public void RenderEndLine(Text.Layout.Context context)
		{
		}
		
		public void RenderEndParagraph(Text.Layout.Context context)
		{
			if ( this.internalOperation != InternalOperation.Painting )  return;

			Text.Layout.XlineRecord[] records = context.XlineRecords;
			if ( records.Length == 0 )  return;

			System.Collections.ArrayList process = new System.Collections.ArrayList();
			
			for ( int lineStart=0 ; lineStart<records.Length ; )
			{
				bool found;
				
				do
				{
					Text.Properties.AbstractXlineProperty xline = null;
					Text.Properties.FontColorProperty color = null;
					
					Text.Layout.XlineRecord starting = null;
					Text.Layout.XlineRecord ending   = null;

					found = false;
					
					for ( int i=lineStart ; i<records.Length ; i++ )
					{
						if ( records[i].Type == Text.Layout.XlineRecord.RecordType.LineEnd )
						{
							if ( starting != null )
							{
								System.Diagnostics.Debug.Assert(xline != null);
								
								ending = records[i];
								found  = true;
								this.RenderXline(context, xline, starting, ending);  // dessine le trait
								process.Add(new XlineInfo(xline, color));  // le trait est fait
							}
							break;
						}
						
						ending = records[i];
						
						for ( int j=0 ; j<records[i].Xlines.Length ; j++ )
						{
							if ( xline == null )  // cherche le d�but ?
							{
								if ( TextBox2.XlineContains(process, records[i].Xlines[j], records[i].TextColor) )  continue;

								xline    = records[i].Xlines[j];
								color    = records[i].TextColor;
								starting = records[i];
								ending   = null;  // la fin ne peut pas �tre dans ce record
								break;
							}
							else if ( starting == null )	// cherche un autre d�but ?
							{
								if ( !Text.Property.CompareEqualContents(xline, records[i].Xlines[j]) ||
									 !Text.Property.CompareEqualContents(color, records[i].TextColor) )
								{
									continue;
								}
								
								starting = records[i];
								ending   = null;  // la fin ne peut pas �tre dans ce record
								break;
							}
							else	// cherche la fin ?
							{
								if ( Text.Property.CompareEqualContents(xline, records[i].Xlines[j]) &&
									 Text.Property.CompareEqualContents(color, records[i].TextColor) )
								{
									ending = null;  // la fin ne peut pas �tre dans ce record
									break;
								}
							}
						}
						
						if ( starting != null && ending != null )  // fin trouv�e ?
						{
							System.Diagnostics.Debug.Assert(xline != null);
							
							this.RenderXline(context, xline, starting, ending);  // dessine le trait
							process.Add(new XlineInfo(xline, color));  // le trait est fait
							
							//	Cherche encore d'autres occurrences de la m�me propri�t� dans
							//	la m�me ligne...
							
							starting = null;
							ending   = null;
							found    = true;
						}
					}
				}
				while ( found );
				
				//	Saute les enregistrements de la ligne courante et reprend tout depuis
				//	le d�but de la ligne suivante:
				
				while ( lineStart<records.Length )
				{
					if ( records[lineStart++].Type == Text.Layout.XlineRecord.RecordType.LineEnd )  break;
				}
				
				process.Clear();
			}
		}

		protected void RenderXline(Text.Layout.Context context, Text.Properties.AbstractXlineProperty xline, Text.Layout.XlineRecord starting, Text.Layout.XlineRecord ending)
		{
			//	Dessine un trait soulign�, surlign� ou biff�.
			if ( ending.X <= starting.X )  return;
			
			double y = starting.Y;

			if ( xline.WellKnownType == Text.Properties.WellKnownType.Underline )
			{
				y -= xline.Position;
			}
			if ( xline.WellKnownType == Text.Properties.WellKnownType.Overline )
			{
				y += context.LineAscender;
				y -= xline.Position;
			}
			if ( xline.WellKnownType == Text.Properties.WellKnownType.Strikeout )
			{
				y += xline.Position;
			}

			Path path = Path.FromRectangle(starting.X, y-xline.Thickness/2, ending.X-starting.X, xline.Thickness);

			string color = xline.DrawStyle;
			if ( color == null )  // couleur par d�faut (comme le texte) ?
			{
				color = starting.TextColor.TextColor;
			}
			this.port.RichColor = RichColor.Parse(color);

			this.port.PaintSurface(path);
		}
		#endregion


		public override void UpdateTextGrid(bool notify)
		{
			//	Met � jour le pav� en fonction des lignes magn�tiques.
			Text.SimpleTextFrame frame = this.textFrame as Text.SimpleTextFrame;
			frame.GridStep   = this.document.Modifier.ActiveViewer.DrawingContext.TextGridStep;
			frame.GridOffset = this.document.Modifier.ActiveViewer.DrawingContext.TextGridOffset;

			if ( notify )
			{
				this.textFlow.TextStory.NotifyTextChanged();
				this.SetDirtyBbox();
				this.document.Notifier.NotifyArea(this.BoundingBox);
			}
		}

		
		public override Path GetMagnetPath()
		{
			//	Retourne le chemin g�om�trique de l'objet pour les constructions
			//	magn�tiques.
			return this.PathBuild();
		}

		public override Path[] GetPaths()
		{
			//	Retourne les chemins g�om�triques de l'objet.
			Graphics port = new Graphics();
			Drawing.PathAccumulationRasterizer rasterizer = new PathAccumulationRasterizer();
			port.ReplaceRasterizer(rasterizer);

			this.DrawText(port, null, InternalOperation.GetPath);

			return rasterizer.GetPaths();
		}


		#region Serialization
		protected TextBox2(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui d�s�rialise l'objet.
		}
		#endregion
		
	}
}
