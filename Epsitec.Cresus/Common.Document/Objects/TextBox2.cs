using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Objects
{
	/// <summary>
	/// La classe TextBox2 est la classe de l'objet graphique "pavé de texte".
	/// </summary>
	[System.Serializable()]
	public class TextBox2 : Objects.Abstract, Text.ITextRenderer
	{
		public TextBox2(Document document, Objects.Abstract model) : base(document, model)
		{
			if ( this.document == null )  return;  // objet factice ?
			this.CreateProperties(model, false);
			this.Initialise();
		}

		protected void Initialise()
		{
			this.textContext = new Text.TextContext();

			if ( this.document.Modifier == null )
			{
				this.textStory = new Text.TextStory(this.textContext);
			}
			else
			{
				this.textStory = new Text.TextStory(this.document.Modifier.OpletQueue, this.textContext);
			}

			this.textFitter    = new Text.TextFitter(this.textStory);
			this.textNavigator = new Text.TextNavigator(this.textFitter);
			this.textFrame     = new Text.SimpleTextFrame();
			
			this.navigator = new TextNavigator2();
			
			this.navigator.TextNavigator = this.textNavigator;
			this.navigator.TextFrame     = this.textFrame;
			
			this.textFitter.FrameList.Add(this.textFrame);
			
			this.textFitter.ClearAllMarks();
			this.textFitter.GenerateAllMarks();
			
			//?this.navigator.TextChanged += new Support.EventHandler (this.HandleTextChanged);
			//?this.navigator.CursorMoved += new Support.EventHandler (this.HandleCursorMoved);
			
			this.markerSelected = this.textContext.Markers.Selected;
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
			return new TextBox2(document, model);
		}

		public override void Dispose()
		{
			base.Dispose();
		}


		// Nom de l'icône.
		public override string IconName
		{
			get { return Misc.Icon("ObjectTextBox"); }
		}


		public string Content
		{
			get
			{
				return "";  //?
			}

			set
			{
				//?
			}
		}


		// Détecte si la souris est sur l'objet pour l'éditer.
		public override bool DetectEdit(Point pos)
		{
			return this.Detect(pos);
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

		// Déplace une poignée.
		public override void MoveHandleProcess(int rank, Point pos, DrawingContext drawingContext)
		{
			if ( rank >= 4 )  // poignée d'une propriété ?
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

		// Début de la création d'un objet.
		public override void CreateMouseDown(Point pos, DrawingContext drawingContext)
		{
			drawingContext.ConstrainFlush();
			drawingContext.ConstrainAddHomo(pos);
			this.HandleAdd(pos, HandleType.Primary);  // rang = 0
			this.HandleAdd(pos, HandleType.Primary);  // rang = 1
			this.document.Notifier.NotifyArea(this.BoundingBox);
			this.isCreating = true;
		}

		// Déplacement pendant la création d'un objet.
		public override void CreateMouseMove(Point pos, DrawingContext drawingContext)
		{
			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapPos(ref pos);
			this.Handle(1).Position = pos;
			this.SetDirtyBbox();
			this.TextInfoModifRect();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// Fin de la création d'un objet.
		public override void CreateMouseUp(Point pos, DrawingContext drawingContext)
		{
			this.document.Notifier.NotifyArea(this.BoundingBox);
			this.isCreating = false;
			this.document.Modifier.TextInfoModif = "";

			drawingContext.SnapPos(ref pos);
			this.Handle(1).Position = pos;
			drawingContext.ConstrainDelStarting();
			drawingContext.MagnetClearStarting();

			// Crée les 2 autres poignées dans les coins opposés.
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
			//?this.textLayout.FillFontFaceList(list);
		}

		// Ajoute tous les caractères utilisés par l'objet dans une table.
		public override void FillOneCharList(System.Collections.Hashtable table)
		{
#if false
			Point p1 = new Point();
			Point p2 = new Point();
			Point p3 = new Point();
			Point p4 = new Point();
			if ( !this.InitTextLayout(ref p1, ref p2, ref p3, ref p4, null) )  return;
			TextLayout.OneCharStructure[] fix = this.textLayout.ComputeStructure();

			foreach ( TextLayout.OneCharStructure oneChar in fix )
			{
				if ( oneChar == null )  continue;

				PDF.CharacterList cl = new PDF.CharacterList(oneChar);
				if ( !table.ContainsKey(cl) )
				{
					table.Add(cl, null);
				}
			}
#endif
		}

		// Indique si un objet est éditable.
		public override bool IsEditable
		{
			get { return true; }
		}

		// Lie l'objet éditable à une règle.
		public override bool EditRulerLink(TextRuler ruler, DrawingContext drawingContext)
		{
			return false;  //?
#if false
			ruler.ListCapability = true;
			ruler.TabCapability = true;
			ruler.AttachToText(this.textNavigator);

			double mx = this.PropertyTextJustif.MarginH;
			ruler.LeftMargin  = mx*drawingContext.ScaleX;
			ruler.RightMargin = mx*drawingContext.ScaleX;
			return true;
#endif
		}


		// Reprend toutes les caractéristiques d'un objet.
		public override void CloneObject(Objects.Abstract src)
		{
			base.CloneObject(src);

#if false
			TextBox2 obj = src as TextBox2;
			this.textLayout.Text = obj.textLayout.Text;
			obj.textLayout.Style.TabCopyTo(this.textLayout.Style);
			obj.textNavigator.Context.CopyTo(this.textNavigator.Context);
			this.textLayout.Simplify(this.textNavigator.Context);
#endif
		}


		// Gestion d'un événement pendant l'édition.
		public override bool EditProcessMessage(Message message, Point pos)
		{
			if ( this.transform == null )  return false;

			if ( message.Type == MessageType.KeyDown   ||
				 message.Type == MessageType.KeyPress  ||
				 message.Type == MessageType.MouseDown )
			{
				this.autoScrollOneShot = true;
			}

			if ( message.Type == MessageType.KeyPress )
			{
				if ( this.EditProcessKeyPress(message) )  return true;
			}

			pos = this.transform.TransformInverse(pos);
			if ( !this.navigator.ProcessMessage(message, pos) )  return false;
			return true;
		}

		// Gestion des événements clavier.
		protected bool EditProcessKeyPress(Message message)
		{
			if ( message.IsCtrlPressed )
			{
				switch ( message.KeyCode )
				{
					case KeyCode.AlphaX:  return this.EditCut();
					case KeyCode.AlphaC:  return this.EditCopy();
					case KeyCode.AlphaV:  return this.EditPaste();
					case KeyCode.AlphaB:  return this.EditBold();
					case KeyCode.AlphaI:  return this.EditItalic();
					case KeyCode.AlphaU:  return this.EditUnderlined();
					case KeyCode.AlphaA:  return this.EditSelectAll();
				}
			}
			return false;
		}

		#region CopyPaste
		public override bool EditCut()
		{
#if false
			string text = this.textNavigator.Selection;
			if ( text == "" )  return false;
			Support.Clipboard.WriteData data = new Support.Clipboard.WriteData();
			data.WriteHtmlFragment(text);
			data.WriteTextLayout(text);
			Support.Clipboard.SetData(data);
			this.textNavigator.Selection = "";
			this.document.Notifier.NotifyArea(this.BoundingBox);
#endif
			return true;
		}
		
		public override bool EditCopy()
		{
#if false
			string text = this.textNavigator.Selection;
			Support.Clipboard.WriteData data = new Support.Clipboard.WriteData();
			if ( text == "" )  return false;
			data.WriteHtmlFragment(text);
			data.WriteTextLayout(text);
			Support.Clipboard.SetData(data);
#endif
			return true;
		}
		
		public override bool EditPaste()
		{
#if false
			Support.Clipboard.ReadData data = Support.Clipboard.GetData();
			string html = data.ReadTextLayout();
			if ( html == null )
			{
				html = data.ReadHtmlFragment();
				if ( html != null )
				{
					html = Support.Clipboard.ConvertHtmlToSimpleXml(html);
				}
				else
				{
					html = TextLayout.ConvertToTaggedText(data.ReadText());
				}
			}
			if ( html == null )  return false;
			this.textNavigator.Selection = html;
			this.document.Notifier.NotifyArea(this.BoundingBox);
#endif
			return true;
		}

		public override bool EditSelectAll()
		{
			this.navigator.SelectAll();
			return true;
		}
		#endregion

		// Insère un glyphe dans le pavé en édition.
		public override bool EditInsertGlyph(string text)
		{
			this.navigator.Insert(text);
			this.document.Notifier.NotifyArea(this.BoundingBox);
			return true;
		}

		// Donne la fonte actullement utilisée.
		public override string EditGetFontName()
		{
			return "";  //?
			//?return this.textNavigator.SelectionFontName;
		}

		#region TextFormat
		// Met en gras pendant l'édition.
		public override bool EditBold()
		{
			this.navigator.SetTextProperties(Common.Text.Properties.ApplyMode.Combine, new Common.Text.Properties.FontProperty(null, "!Bold"));
			return true;
		}

		// Met en italique pendant l'édition.
		public override bool EditItalic()
		{
			this.navigator.SetTextProperties(Common.Text.Properties.ApplyMode.Combine, new Common.Text.Properties.FontProperty(null, "!Italic"));
			return true;
		}

		// Souligne pendant l'édition.
		public override bool EditUnderlined()
		{
			this.navigator.SetTextProperties(Common.Text.Properties.ApplyMode.Combine, new Common.Text.Properties.FontProperty(null, "!Underlined"));
			return true;
		}
		#endregion

		// Donne la zone contenant le curseur d'édition.
		public override Drawing.Rectangle EditCursorBox
		{
			get
			{
				return Drawing.Rectangle.Empty;  //?
			}
		}

		// Donne la zone contenant le texte sélectionné.
		public override Drawing.Rectangle EditSelectBox
		{
			get
			{
				return Drawing.Rectangle.Empty;  //?
			}
		}

		// Gestion d'un événement pendant l'édition.
		public override void EditMouseDownMessage(Point pos)
		{
			//?pos = this.transform.TransformInverse(pos);
			//?this.textNavigator.MouseDownMessage(pos);
		}


		// Constuit les formes de l'objet.
		public override Shape[] ShapesBuild(IPaintPort port, DrawingContext drawingContext, bool simplify)
		{
			Path path = this.PathBuild();

			Shape[] shapes = new Shape[4];
			int i = 0;
			
			// Forme de la surface.
			shapes[i] = new Shape();
			shapes[i].Path = path;
			shapes[i].SetPropertySurface(port, this.PropertyFillGradient);
			i ++;

			// Trait du rectangle.
			shapes[i] = new Shape();
			shapes[i].Path = path;
			shapes[i].SetPropertyStroke(port, this.PropertyLineMode, this.PropertyLineColor);
			i ++;

			// Caractères du texte.
			shapes[i] = new Shape();
			shapes[i].SetTextObject(this);
			i ++;

			// Rectangle complet pour bbox et détection.
			shapes[i] = new Shape();
			shapes[i].Path = path;
			shapes[i].Type = Type.Surface;
			shapes[i].Aspect = Aspect.InvisibleBox;
			i ++;

			return shapes;
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

		// Calcules les 4 coins.
		protected void Corners(ref Point p1, ref Point p2, ref Point p3, ref Point p4)
		{
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

		// Initialise.
		protected bool InitTextFrame(ref Point p1, ref Point p2, ref Point p3, ref Point p4,
									 DrawingContext drawingContext)
		{
			this.Corners(ref p1, ref p2, ref p3, ref p4);
			if ( !this.PropertyTextJustif.DeflateBox(ref p1, ref p2, ref p3, ref p4) )
			{
				return false;
			}

			this.textFrame.Width  = Point.Distance(p1,p2);
			this.textFrame.Height = Point.Distance(p1,p3);

			return true;
		}

		// Dessine le texte du pavé.
		public override void DrawText(IPaintPort port, DrawingContext drawingContext)
		{
			if ( this.handles.Count < 4 )  return;

			Point p1 = new Point();
			Point p2 = new Point();
			Point p3 = new Point();
			Point p4 = new Point();
			if ( !this.InitTextFrame(ref p1, ref p2, ref p3, ref p4, drawingContext) )  return;

			Transform ot = port.Transform;

			double angle = Point.ComputeAngleDeg(p1, p2);
			this.transform = new Transform();
			this.transform.Translate(p1);
			this.transform.RotateDeg(angle, p1);
			port.MergeTransform(transform);

			this.graphics = port as Graphics;
			if ( this.graphics != null )
			{
				this.textFitter.RenderTextFrame(this.textFrame, this);
			}

			port.Transform = ot;
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
			double ox = context.LineCurrentX;
			double oy = context.LineBaseY;
			double dx = context.TextWidth;
			
			this.graphics.LineWidth = 0.3;
			this.graphics.AddLine(ox, oy, ox + dx, oy);
			this.graphics.RenderSolid(Drawing.Color.FromName("Green"));
			
			context.DisableSimpleRendering();
		}
		
		public void Render(Text.Layout.Context layout, Epsitec.Common.OpenType.Font font, double size, Drawing.Color color, Text.Layout.TextToGlyphMapping mapping, ushort[] glyphs, double[] x, double[] y, double[] sx, double[] sy, bool isLastRun)
		{
			Text.ITextFrame frame = layout.Frame;
			
			System.Diagnostics.Debug.Assert(mapping != null);
			
			// Vérifions d'abord que le mapping du texte vers les glyphes est
			// correct et correspond à quelque chose de valide :
			int  offset = 0;
			bool isInSelection = false;
			
			double selX = 0;
			
			System.Collections.ArrayList selRectList = null;
			Drawing.Rectangle            selBbox     = Drawing.Rectangle.Empty;
			
			int[]    cArray;
			ulong[]  tArray;
			ushort[] gArray;
			
			double x1 = 0;
			double x2 = 0;
			
			while ( mapping.GetNextMapping(out cArray, out gArray, out tArray) )
			{
				int numGlyphs = gArray.Length;
				int numChars  = cArray.Length;
				
				System.Diagnostics.Debug.Assert((numGlyphs == 1) || (numChars == 1));
				
				x1 = x[offset+0];
				x2 = x[offset+numGlyphs];
				
				for ( int i=0 ; i<numChars ; i++ )
				{
					if ( (tArray[i] & this.markerSelected) != 0 )
					{
						// Le caractère considéré est sélectionné.
						if ( isInSelection == false )
						{
							// C'est le premier caractère d'une tranche. Il faut
							// mémoriser son début :
							double xx = x1 + ((x2 - x1) * i) / numChars;
							isInSelection = true;
							selX = xx;
						}
					}
					else
					{
						if ( isInSelection )
						{
							// Nous avons quitté une tranche sélectionnée. Il faut
							// mémoriser sa fin :
							double xx = x1 + ((x2 - x1) * i) / numChars;
							isInSelection = false;
							
							if ( xx > selX )
							{
								if ( selRectList == null )
								{
									selRectList = new System.Collections.ArrayList();
								}
								
								double dx = xx - selX;
								double dy = layout.LineY2 - layout.LineY1;
								
								Drawing.Rectangle rect = new Drawing.Rectangle(selX, layout.LineY1, dx, dy);
								
								selBbox = Drawing.Rectangle.Union(selBbox, rect);
								
								double px1 = rect.Left;
								double px2 = rect.Right;
								double py1 = rect.Bottom;
								double py2 = rect.Top;
								
								this.graphics.Rasterizer.Transform.TransformDirect(ref px1, ref py1);
								this.graphics.Rasterizer.Transform.TransformDirect(ref px2, ref py2);
								
								selRectList.Add(Drawing.Rectangle.FromCorners(px1, py1, px2, py2));
							}
						}
					}
				}
				
				offset += numGlyphs;
			}
			
			if ( isInSelection )
			{
				// Nous avons quitté une tranche sélectionnée. Il faut
				// mémoriser sa fin :
				double xx = x2;
				isInSelection = false;
				
				if ( xx > selX )
				{
					if ( selRectList == null )
					{
						selRectList = new System.Collections.ArrayList();
					}
					
					double dx = xx - selX;
					double dy = layout.LineY2 - layout.LineY1;
					
					Drawing.Rectangle rect = new Drawing.Rectangle(selX, layout.LineY1, dx, dy);
					
					selBbox = Drawing.Rectangle.Union(selBbox, rect);
					
					double px1 = rect.Left;
					double px2 = rect.Right;
					double py1 = rect.Bottom;
					double py2 = rect.Top;
					
					this.graphics.Rasterizer.Transform.TransformDirect(ref px1, ref py1);
					this.graphics.Rasterizer.Transform.TransformDirect(ref px2, ref py2);
					
					selRectList.Add(Drawing.Rectangle.FromCorners(px1, py1, px2, py2));
				}
			}
			
			if ( font.FontManagerType == OpenType.FontManagerType.System )
			{
				Drawing.NativeTextRenderer.Draw(this.graphics.Pixmap, font, size, glyphs, x, y, color);
			}
			else
			{
				Drawing.Font drawingFont = Drawing.Font.GetFont(font.FontIdentity.InvariantFaceName, font.FontIdentity.InvariantStyleName);
				
				if ( drawingFont != null )
				{
					for ( int i=0 ; i<glyphs.Length ; i++ )
					{
						if ( glyphs[i] < 0xffff )
						{
							this.graphics.Rasterizer.AddGlyph(drawingFont, glyphs[i], x[i], y[i], size, sx == null ? 1.0 : sx[i], sy == null ? 1.0 : sy[i]);
						}
					}
				}
				
				this.graphics.RenderSolid(color);
			}
			
			if ( selRectList != null )
			{
				this.hasSelection = true;
				
				Drawing.Rectangle saveClip = this.graphics.SaveClippingRectangle();
				
				this.graphics.SetClippingRectangles(selRectList);
				this.graphics.AddFilledRectangle(selBbox);
				this.graphics.RenderSolid(Drawing.Color.FromName("Highlight"));
				
				if ( font.FontManagerType == OpenType.FontManagerType.System )
				{
					Drawing.NativeTextRenderer.Draw(this.graphics.Pixmap, font, size, glyphs, x, y, color);
				}
				else
				{
					Drawing.Font drawingFont = Drawing.Font.GetFont(font.FontIdentity.InvariantFaceName, font.FontIdentity.InvariantStyleName);
					
					if ( drawingFont != null )
					{
						for ( int i=0 ; i<glyphs.Length ; i++ )
						{
							if ( glyphs[i] < 0xffff )
							{
								this.graphics.Rasterizer.AddGlyph(drawingFont, glyphs[i], x[i], y[i], size, sx == null ? 1.0 : sx[i], sy == null ? 1.0 : sy[i]);
							}
						}
					}
					
					this.graphics.RenderSolid(Drawing.Color.FromName("HighlightText"));
				}
				
				this.graphics.RestoreClippingRectangle(saveClip);
			}
		}
		
		public void Render(Text.Layout.Context layout, Text.IGlyphRenderer glyphRenderer, Drawing.Color color, double x, double y, bool isLastRun)
		{
			glyphRenderer.RenderGlyph(layout.Frame, x, y);
		}
		
		public void RenderEndLine(Text.Layout.Context context)
		{
		}
		
		public void RenderEndParagraph(Text.Layout.Context context)
		{
			Text.Layout.UnderlineRecord[] records = context.UnderlineRecords;
			
			double x1 = 0;
			double y1 = 0;
			
			// Dans ce test, la couleur est stockée directement comme LineStyle pour
			// la propriété "underline".
			string color = "Yellow";
			
			if ( records.Length > 0 )
			{
				for ( int i=0 ; i<records.Length ; i++ )
				{
					if ( (records[i].Type == Common.Text.Layout.UnderlineRecord.RecordType.LineEnd) ||
						 (records[i].Underlines.Length == 0) )
					{
						this.graphics.LineWidth = 1.0;
						this.graphics.AddLine(x1, y1, records[i].X, records[i].Y + records[i].Descender * 0.8);
						this.graphics.RenderSolid(Drawing.Color.FromName(color));
					}
					
					x1 = records[i].X;
					y1 = records[i].Y + records[i].Descender * 0.8;
					
					if ( records[i].Underlines.Length > 0 )
					{
						color = records[i].Underlines[0].LineStyle;
					}
				}
			}
		}
		#endregion


		// Retourne le chemin géométrique de l'objet pour les constructions
		// magnétiques.
		public override Path GetMagnetPath()
		{
			return this.PathBuild();
		}


		#region Serialization
		// Sérialise l'objet.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		// Constructeur qui désérialise l'objet.
		protected TextBox2(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.Initialise();
		}

		// Vérifie si tous les fichiers existent.
		public override void ReadCheckWarnings(Font.FaceInfo[] fonts, System.Collections.ArrayList warnings)
		{
			//?Common.Document.Objects.Abstract.ReadCheckFonts(fonts, warnings, this.textLayout);
		}
		#endregion


		protected bool							hasSelection;
		protected ulong							markerSelected;
		
		protected Text.TextContext				textContext;
		protected Text.TextStory				textStory;
		protected Text.TextFitter				textFitter;
		protected Text.TextNavigator			textNavigator;
		protected Text.SimpleTextFrame			textFrame;
		protected TextNavigator2				navigator;
		protected Graphics						graphics;
		protected Transform						transform;
	}
}
