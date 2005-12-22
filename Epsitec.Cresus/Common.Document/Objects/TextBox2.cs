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
	public class TextBox2 : Objects.Abstract, Text.ITextRenderer
	{
		protected enum InternalOperation
		{
			Painting,
			CharactersTable,
			RealBoundingBox,
		}

		protected enum SpaceType
		{
			None,				// ce n'est pas un espace
			BreakSpace,			// espace s�cable
			NoBreakSpace,		// espace ins�cable
			NewFrame,			// saut au prochain pav�
			NewPage,			// saut � la prochaine page
		}


		public TextBox2(Document document, Objects.Abstract model) : base(document, model)
		{
			if ( this.document == null )  return;  // objet factice ?
			this.CreateProperties(model, false);
			this.Initialise();
		}

		protected void Initialise()
		{
			this.textFrame = new Text.SimpleTextFrame();
			
			this.NewTextFlow();
			this.InitialiseInternals();
		}
		
		protected void InitialiseInternals()
		{
			if ( this.textFrame == null )
			{
				this.textFrame = new Text.SimpleTextFrame();
			}

			this.UpdateTextGrid(false);
			
			System.Diagnostics.Debug.Assert(this.textFlow != null);
			
			this.markerSelected = this.document.TextContext.Markers.Selected;

			this.cursorBox = Drawing.Rectangle.Empty;
			this.selectBox = Drawing.Rectangle.Empty;
		}

		public void NewTextFlow()
		{
			//	Cr�e un nouveau TextFlow pour l'objet.
			TextFlow flow = new TextFlow(this.document);
			this.document.TextFlows.Add(flow);
			this.TextFlow = flow;
			flow.Add(this, null, true);
		}

		public TextFlow TextFlow
		{
			//	TextFlow associ� � l'objet.
			get
			{
				return this.textFlow;
			}

			set
			{
				if ( this.textFlow != value )
				{
					this.InsertOpletTextFlow();
					this.textFlow = value;

					this.UpdateTextLayout();
					this.textFlow.NotifyAreaFlow();
				}
			}
		}

		public Text.ITextFrame TextFrame
		{
			//	Donne le TextFrame associ� � l'objet.
			get
			{
				return this.textFrame;
			}
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
			return new TextBox2(document, model);
		}

		public override void Dispose()
		{
			if ( this.textFlow != null )
			{
				this.textFlow.Remove(this);  // objet seul dans son propre flux

				if ( this.textFlow.Count == 1 )  // est-on le dernier et seul utilisateur ?
				{
					this.document.TextFlows.Remove(this.textFlow);
				}
			}

			base.Dispose();
		}


		public override string IconName
		{
			//	Nom de l'ic�ne.
			get { return Misc.Icon("ObjectTextBox"); }
		}

		protected TextNavigator2 MetaNavigator
		{
			//	MetaNavigator associ� au TextFlow.
			get
			{
				if ( this.textFlow == null )
				{
					return null;
				}
				else
				{
					return this.textFlow.MetaNavigator;
				}
			}
		}

		public override DetectEditType DetectEdit(Point pos)
		{
			//	D�tecte si la souris est sur l'objet pour l'�diter.
			if ( this.edited )
			{
				DetectEditType handle = this.DetectFlowHandle(pos);
				if ( handle != DetectEditType.Out )  return handle;
			}

			if ( this.Detect(pos) )  return DetectEditType.Body;
			return DetectEditType.Out;
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

		public override void FillFontFaceList(System.Collections.ArrayList list)
		{
			//	Ajoute toutes les fontes utilis�es par l'objet dans une liste.
			//?this.textLayout.FillFontFaceList(list);
		}

		public override void FillOneCharList(IPaintPort port, DrawingContext drawingContext, System.Collections.Hashtable table)
		{
			//	Ajoute tous les caract�res utilis�s par l'objet dans une table.
			this.charactersTable = table;
			this.DrawText(port, drawingContext, InternalOperation.CharactersTable);
			this.charactersTable = null;
		}

		public override Drawing.Rectangle RealBoundingBox()
		{
			//	Retourne la bounding r�elle, en fonction des caract�res contenus.
			this.mergingBoundingBox = Drawing.Rectangle.Empty;
			this.DrawText(null, null, InternalOperation.RealBoundingBox);

			return this.mergingBoundingBox;
		}

		public override bool IsEditable
		{
			//	Indique si un objet est �ditable.
			get { return true; }
		}


		public override void CloneObject(Objects.Abstract src)
		{
			//	Reprend toutes les caract�ristiques d'un objet.
			base.CloneObject(src);

			TextBox2 srcText = src as TextBox2;
			System.Diagnostics.Debug.Assert(srcText != null);

			if ( srcText.textFlow.Count == 1 ||  // objet solitaire ?
				 srcText.document != this.document )
			{
				this.textFlow.MergeWith(srcText.textFlow);  // copie le texte
			}
			else	// objet d'une cha�ne ?
			{
				srcText.textFlow.Add(this, srcText, true);  // met dans la cha�ne
			}

			this.UpdateGeometry();
		}


		public override void PutCommands(System.Collections.ArrayList list)
		{
			//	Met les commandes pour l'objet dans une liste.
			base.PutCommands(list);

			if ( this.document.Modifier.Tool == "ToolEdit" )
			{
				bool sel = (this.textFlow.TextNavigator.SelectionCount != 0);
				if ( sel )
				{
					this.PutCommands(list, "Cut");
					this.PutCommands(list, "Copy");
					this.PutCommands(list, "Paste");
					this.PutCommands(list, "");
					this.PutCommands(list, "FontQuick1");
					this.PutCommands(list, "FontQuick2");
					this.PutCommands(list, "FontQuick3");
					this.PutCommands(list, "FontQuick4");
					this.PutCommands(list, "");
					this.PutCommands(list, "FontBold");
					this.PutCommands(list, "FontItalic");
					this.PutCommands(list, "FontUnderlined");
					this.PutCommands(list, "");
					this.PutCommands(list, "FontSubscript");
					this.PutCommands(list, "FontSuperscript");
					this.PutCommands(list, "");
					this.PutCommands(list, "FontSizeMinus");
					this.PutCommands(list, "FontSizePlus");
					this.PutCommands(list, "");
					this.PutCommands(list, "FontClear");
					this.PutCommands(list, "");
				}
				else
				{
					this.PutCommands(list, "Paste");
				}
			}
		}


		public override bool EditProcessMessage(Message message, Point pos)
		{
			//	Gestion d'un �v�nement pendant l'�dition.
			if ( this.transform == null )  return false;

			if ( message.IsKeyType )
			{
				this.document.Modifier.ActiveViewer.CloseMiniBar(false);
			}

			if ( message.Type == MessageType.KeyDown   ||
				 message.Type == MessageType.KeyPress  ||
				 message.Type == MessageType.MouseDown )
			{
				this.SetAutoScroll();
			}

			if ( message.Type == MessageType.KeyPress )
			{
				if ( this.EditProcessKeyPress(message) )  return true;
			}

			if ( message.KeyCodeOnly == KeyCode.Tab )
			{
				if ( message.Type == MessageType.KeyDown )
				{
					this.ProcessTabKey();
				}
				return true;
			}

			Point ppos;
			ITextFrame frame;
			
			if ( this.IsInTextFrame(pos, out ppos) )
			{
				frame = this.textFrame;
			}
			else
			{
				//	Si la souris n'est pas dans notre texte frame, on utilise le text
				//	frame correspondant � sa position (s'il y en a un).
				
				frame = this.textFlow.FindTextFrame(pos, out ppos);
				
				if ( frame == null )  frame = this.textFrame;
			}
			
			if ( !this.MetaNavigator.ProcessMessage(message, ppos, frame) )  return false;
			
			if ( message.Type == MessageType.MouseDown )
			{
				this.document.Modifier.ActiveViewer.CloseMiniBar(false);
			}

			if ( message.Type == MessageType.MouseUp )
			{
				if ( this.textFlow.TextNavigator.SelectionCount > 0 )
				{
					Viewer viewer = this.document.Modifier.ActiveViewer;
					double distance = 0;
					Drawing.Rectangle selbox = this.EditSelectBox;
					if ( !selbox.IsEmpty )
					{
						selbox = viewer.InternalToScreen(selbox);
						double top = System.Math.Min(selbox.Top, viewer.Height-2);
						Point mouse = viewer.InternalToScreen(pos);
						distance = System.Math.Max(top-mouse.Y, 0);
					}
					viewer.OpenMiniBar(pos, false, false, false, distance);
				}
			}

			return true;
		}

		protected bool EditProcessKeyPress(Message message)
		{
			//	Gestion des �v�nements clavier.
			if ( message.IsCtrlPressed )
			{
				switch ( message.KeyCode )
				{
					case KeyCode.AlphaX:  return this.EditCut();
					case KeyCode.AlphaC:  return this.EditCopy();
					case KeyCode.AlphaV:  return this.EditPaste();
					case KeyCode.AlphaA:  return this.EditSelectAll();
				}
			}
			return false;
		}

		protected bool ProcessTabKey()
		{
			//	Gestion de la touche tab.
			string tag = this.textFlow.TextNavigator.FindInsertionTabTag();
			
			if ( tag == null )  return false;

			this.MetaNavigator.Insert(new Text.Properties.TabProperty(tag));
			return true;
		}

		protected DetectEditType DetectFlowHandle(Point pos)
		{
			//	D�tecte la "poign�e" du flux de l'objet.
			DrawingContext drawingContext = this.document.Modifier.ActiveViewer.DrawingContext;

			Point prevP1;
			Point prevP2;
			Point prevP3;
			Point prevP4;
			this.CornersFlowPrev(out prevP1, out prevP2, out prevP3, out prevP4, drawingContext);

			InsideSurface surf = new InsideSurface(pos, 4);
			surf.AddLine(prevP1, prevP2);
			surf.AddLine(prevP2, prevP4);
			surf.AddLine(prevP4, prevP3);
			surf.AddLine(prevP3, prevP1);
			if ( surf.IsInside() )  return DetectEditType.HandleFlowPrev;

			Point nextP1;
			Point nextP2;
			Point nextP3;
			Point nextP4;
			this.CornersFlowNext(out nextP1, out nextP2, out nextP3, out nextP4, drawingContext);

			surf = new InsideSurface(pos, 4);
			surf.AddLine(nextP1, nextP2);
			surf.AddLine(nextP2, nextP4);
			surf.AddLine(nextP4, nextP3);
			surf.AddLine(nextP3, nextP1);
			if ( surf.IsInside() )  return DetectEditType.HandleFlowNext;

			return DetectEditType.Out;
		}

		#region CopyPaste
		public override bool EditCut()
		{
			this.EditCopy();
			this.MetaNavigator.DeleteSelection();
			return true;
		}
		
		public override bool EditCopy()
		{
			string[] texts = this.textFlow.TextNavigator.GetSelectedTexts();
			if ( texts == null || texts.Length == 0 )  return false;

			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			foreach ( string part in texts )
			{
				builder.Append(part);
			}
			string text = builder.ToString();

			Support.Clipboard.WriteData data = new Support.Clipboard.WriteData();
			data.WriteHtmlFragment(text);
			data.WriteTextLayout(text);
			Support.Clipboard.SetData(data);
			return true;
		}
		
		public override bool EditPaste()
		{
			Support.Clipboard.ReadData data = Support.Clipboard.GetData();
			string text = data.ReadTextLayout();
			if ( text == null )
			{
				text = data.ReadText();
				if ( text != null )
				{
					text = text.Replace("\r\n", "\u2029");		//	ParagraphSeparator
					text = text.Replace("\n", "\u2028");		//	LineSeparator
					text = text.Replace("\r", "\u2028");		//	LineSeparator
				}
			}
			if ( text == null )  return false;
			this.MetaNavigator.Insert(text);
			this.textFlow.NotifyAreaFlow();
			return true;
		}

		public override bool EditSelectAll()
		{
			this.MetaNavigator.SelectAll();
			return true;
		}
		#endregion

		public void EditInsertText(string text, string fontFace, double fontSize)
		{
			//	Ins�re un texte en provenance d'un ancien TextBox ou TextLine.
			this.EditInsertText(text, "", "");

			this.EditWrappersAttach();  // attache l'objet aux diff�rents wrappers
			this.textFlow.ActiveTextBox = this;

			this.MetaNavigator.SelectAll();

			this.document.TextWrapper.SuspendSynchronisations();
			this.document.TextWrapper.Defined.FontFace = fontFace;
			this.document.TextWrapper.Defined.FontStyle = Misc.DefaultFontStyle(fontFace);
			this.document.TextWrapper.Defined.FontSize = fontSize;
			this.document.TextWrapper.Defined.Units = Text.Properties.SizeUnits.Points;
			this.document.TextWrapper.ResumeSynchronisations();

			this.textFlow.TextNavigator.ClearSelection();

			this.document.Wrappers.WrappersDetach();
			this.textFlow.ActiveTextBox = null;
		}

		public override bool EditInsertText(string text, string fontFace, string fontStyle)
		{
			//	Ins�re un texte dans le pav� en �dition.
			this.MetaNavigator.EndSelection();
			this.document.Modifier.OpletQueueBeginActionNoMerge(Res.Strings.Action.Text.Glyphs.Insert);

			if ( fontFace == "" )
			{
				this.MetaNavigator.Insert(text);
			}
			else
			{
				for ( int i=0 ; i<text.Length ; i++ )
				{
					OpenType.Font font = TextContext.GetFont(fontFace, fontStyle);
					Text.Unicode.Code code = (Text.Unicode.Code) text[i];
					int glyph = font.GetGlyphIndex(text[i]);
					Text.Properties.OpenTypeProperty otp = new Text.Properties.OpenTypeProperty(fontFace, fontStyle, glyph);
					this.MetaNavigator.Insert(code, otp);
				}
			}

			this.document.Modifier.OpletQueueValidateAction();
			this.textFlow.NotifyAreaFlow();
			return true;
		}

		public override bool EditInsertText(Unicode.Code code)
		{
			//	Ins�re un texte dans le pav� en �dition.
			this.MetaNavigator.EndSelection();
			this.document.Modifier.OpletQueueBeginActionNoMerge(Res.Strings.Action.Text.Glyphs.Insert);

			this.MetaNavigator.Insert(code);

			this.document.Modifier.OpletQueueValidateAction();
			this.textFlow.NotifyAreaFlow();
			return true;
		}

		public override bool EditInsertText(Text.Properties.BreakProperty brk)
		{
			//	Ins�re un texte dans le pav� en �dition.
			this.MetaNavigator.EndSelection();
			this.document.Modifier.OpletQueueBeginActionNoMerge(Res.Strings.Action.Text.Glyphs.Insert);

			this.MetaNavigator.Insert(brk);

			this.document.Modifier.OpletQueueValidateAction();
			this.textFlow.NotifyAreaFlow();
			return true;
		}

		public override bool EditInsertGlyph(int code, int glyph, string fontFace, string fontStyle)
		{
			//	Ins�re un glyphe dans le pav� en �dition.
			this.MetaNavigator.EndSelection();
			this.document.Modifier.OpletQueueBeginActionNoMerge(Res.Strings.Action.Text.Glyphs.Alternate);

			if ( fontFace == "" )
			{
				string text = code.ToString();
				this.MetaNavigator.Insert(text);
			}
			else
			{
				OpenType.Font font = TextContext.GetFont(fontFace, fontStyle);
				Text.Properties.OpenTypeProperty otp = new Text.Properties.OpenTypeProperty(fontFace, fontStyle, glyph);
				this.MetaNavigator.Insert((Text.Unicode.Code)code, otp);
				this.MetaNavigator.SelectInsertedCharacter();
			}

			this.document.Modifier.OpletQueueValidateAction();
			this.textFlow.NotifyAreaFlow();
			return true;
		}

		public override bool EditGetSelectedGlyph(out int code, out int glyph, out OpenType.Font font)
		{
			//	Retourne le glyphe du caract�re s�lectionn�.
			code = 0;
			glyph = 0;
			font = null;

			int n = this.textFlow.TextNavigator.SelectionCount;
			if ( n != 1 )  return false;

			ulong[] sel = this.textFlow.TextNavigator.GetRawSelection(0);
			if ( sel.Length != 1 )  return false;

			code = Unicode.Bits.GetCode(sel[0]);

			Text.Properties.OpenTypeProperty otp;
			this.document.TextContext.GetOpenType(sel[0], out otp);

			string face = this.document.TextWrapper.Defined.FontFace;
			if ( face == null )
			{
				face = this.document.TextWrapper.Active.FontFace;
				if ( face == null )
				{
					face = "";
				}
			}

			string style = this.document.TextWrapper.Defined.FontStyle;
			if ( style == null )
			{
				style = this.document.TextWrapper.Active.FontStyle;
				if ( style == null )
				{
					style = "";
				}
			}

			font = TextContext.GetFont(face, style);

			if ( otp == null )
			{
				glyph = font.GetGlyphIndex(code);
			}
			else
			{
				glyph = otp.GlyphIndex;
			}

			return true;
		}


		protected override void UpdatePageNumber()
		{
			//	Met � jour le TextFrame en fonction du num�ro de la page.
			this.textFrame.PageNumber = this.pageNumber+1;
		}


		#region TextFormat
		public override string[] TextTabTags
		{
			//	Retourne tous les tags des tabulateurs.
			get
			{
				return this.textFlow.TextNavigator.GetAllTabTags();
			}
		}

		public override string NewTextTab(double pos, TextTabType type)
		{
			//	Cr�e un nouveau tabulateur dans le texte.
			double dispo = 0.0;
			string dockingMark = Widgets.HRuler.ConvType2Mark(type);
			TabPositionMode positionMode = TabPositionMode.Absolute;

			if ( type == TextTabType.Center )  dispo = 0.5;
			if ( type == TextTabType.Right  )  dispo = 1.0;
			if ( type == TextTabType.Indent )  positionMode = TabPositionMode.AbsoluteIndent;

			Text.TabList list = this.document.TextContext.TabList;
			Text.Properties.TabProperty tab = list.NewTab(null, pos, Text.Properties.SizeUnits.Points, dispo, dockingMark, positionMode);
			Text.Properties.TabsProperty tabs = new Text.Properties.TabsProperty(tab);
			this.MetaNavigator.SetParagraphProperties(Text.Properties.ApplyMode.Combine, tabs);
			return tab.TabTag;
		}

		public override void DeleteTextTab(string tag)
		{
			//	Supprime un tabulateur du texte.
			this.MetaNavigator.RemoveTab(tag);
		}

		public override bool RenameTextTabs(string[] oldTags, string newTag)
		{
			//	Renomme plusieurs tabulateurs du texte.
			return this.textFlow.TextNavigator.RenameTabs(oldTags, newTag);
		}

		public override void GetTextTab(string tag, out double pos, out TextTabType type)
		{
			//	Donne un tabulateur du texte.
			Text.TabList list = this.document.TextContext.TabList;
			Text.Properties.TabProperty tab = list.GetTabProperty(tag);

			pos = list.GetTabPosition(tab);
			string mark = list.GetTabDockingMark(tab);

			type = TextTabType.Left;

			if ( list.GetTabPositionMode(tab) == TabPositionMode.AbsoluteIndent )
			{
				type = TextTabType.Indent;
			}
			else if ( mark != null )
			{
				type = Widgets.HRuler.ConvMark2Type(mark);
			}
			else
			{
				double dispo = list.GetTabDisposition(tab);
				if ( dispo == 0.5 )  type = TextTabType.Center;
				if ( dispo == 1.0 )  type = TextTabType.Right;
			}
		}

		public override void SetTextTab(ref string tag, double pos, TextTabType type, bool firstChange)
		{
			//	Modifie un tabulateur du texte.
			double dispo = 0.0;
			string dockingMark = Widgets.HRuler.ConvType2Mark(type);
			TabPositionMode positionMode = TabPositionMode.Absolute;

			if ( type == TextTabType.Center )  dispo = 0.5;
			if ( type == TextTabType.Right  )  dispo = 1.0;
			if ( type == TextTabType.Indent )  positionMode = TabPositionMode.AbsoluteIndent;
			
			if ( firstChange && Text.TabList.GetTabClass(tag) == Text.TabClass.Auto )
			{
				//	Les tabulateurs "automatiques" ne sont pas li�s � un style. Leur
				//	modification ne doit toucher que le paragraphe courant (ou la
				//	s�lection en cours), c'est pourquoi on cr�e une copie avant de
				//	proc�der � des modifications :
				
				Text.TabList list = this.document.TextContext.TabList;
				
				Text.Properties.TabProperty oldTab = list.GetTabProperty(tag);
				Text.Properties.TabProperty newTab = list.NewTab(null, pos, Text.Properties.SizeUnits.Points, dispo, dockingMark, positionMode, null);
				
				this.textFlow.TextNavigator.RenameTab(oldTab.TabTag, newTab.TabTag);
				
				tag = newTab.TabTag;
			}
			else
			{
				this.textFlow.TextNavigator.RedefineTab(tag, pos, Text.Properties.SizeUnits.Points, dispo, dockingMark, positionMode, null);
			}
			
			this.textFlow.UpdateTabs();  // TODO: devrait �tre inutile
		}

		public override System.Collections.ArrayList CreateTextPanels(string filter)
		{
			//	Cr�e tous les panneaux pour l'�dition.
			System.Collections.ArrayList list = new System.Collections.ArrayList();

			if ( TextPanels.Abstract.IsFilterShow("Justif", filter) )
			{
				TextPanels.Justif justif = new TextPanels.Justif(this.document);
				list.Add(justif);
			}

			if ( TextPanels.Abstract.IsFilterShow("Leading", filter) )
			{
				TextPanels.Leading leading = new TextPanels.Leading(this.document);
				list.Add(leading);
			}

			if ( TextPanels.Abstract.IsFilterShow("Margins", filter) )
			{
				TextPanels.Margins margins = new TextPanels.Margins(this.document);
				list.Add(margins);
			}

			if ( TextPanels.Abstract.IsFilterShow("Spaces", filter) )
			{
				TextPanels.Spaces spaces = new TextPanels.Spaces(this.document);
				list.Add(spaces);
			}

			if ( TextPanels.Abstract.IsFilterShow("Keep", filter) )
			{
				TextPanels.Keep keep = new TextPanels.Keep(this.document);
				list.Add(keep);
			}

			if ( TextPanels.Abstract.IsFilterShow("Font", filter) )
			{
				TextPanels.Font font = new TextPanels.Font(this.document);
				list.Add(font);
			}

			if ( TextPanels.Abstract.IsFilterShow("Xline", filter) )
			{
				TextPanels.Xline xline = new TextPanels.Xline(this.document);
				list.Add(xline);
			}

			if ( TextPanels.Abstract.IsFilterShow("Xscript", filter) )
			{
				TextPanels.Xscript xscript = new TextPanels.Xscript(this.document);
				list.Add(xscript);
			}

			if ( TextPanels.Abstract.IsFilterShow("Box", filter) )
			{
				TextPanels.Box box = new TextPanels.Box(this.document);
				list.Add(box);
			}

			if ( TextPanels.Abstract.IsFilterShow("Language", filter) )
			{
				TextPanels.Language language = new TextPanels.Language(this.document);
				list.Add(language);
			}

			return list;
		}

		protected Text.Property[] GetTextProperties(bool accumulated)
		{
			//	Donne la liste des propri�t�s.
			if ( accumulated )
			{
				return this.textFlow.TextNavigator.AccumulatedTextProperties;
			}
			else
			{
				return this.textFlow.TextNavigator.TextProperties;
			}
		}

		protected bool IsExistingStyle(string name)
		{
			//	Indique l'existance d'un style.
			Text.TextStyle[] styles = this.textFlow.TextNavigator.TextStyles;
			foreach ( Text.TextStyle style in styles )
			{
				if ( style.Name == name )  return true;
			}
			return false;
		}

		protected void ApplyParagraphStyle(string name, string exclude, bool state)
		{
			//	Modifie un style de paragraphe.
			Text.TextStyle style = this.document.TextContext.StyleList[name, Text.TextStyleClass.Paragraph];
			if ( style == null )  return;

			System.Collections.ArrayList list = new System.Collections.ArrayList();
			Text.TextStyle[] styles = Text.TextStyle.FilterStyles(this.MetaNavigator.TextNavigator.TextStyles, Text.TextStyleClass.Paragraph);
			
			for ( int i=0 ; i<styles.Length ; i++ )
			{
				if ( exclude.Length == 0 || !styles[i].Name.StartsWith(exclude) )
				{
					list.Add(styles[i]);
				}
			}
			
			if ( state )
			{
				list.Add(style);
			}
			
			styles = (Text.TextStyle[]) list.ToArray(typeof(Text.TextStyle));
			this.MetaNavigator.SetParagraphStyles(styles);
		}

		protected override void EditWrappersAttach()
		{
			//	Attache l'objet au diff�rents wrappers.
			this.document.Wrappers.WrappersAttach(this.textFlow);
		}

		protected override void UpdateTextRulers()
		{
			//	Met � jour les r�gles pour le texte en �dition.
			if ( this.edited )
			{
				this.textFlow.UpdateTextRulers();
			}
		}
		#endregion

		public override Drawing.Rectangle EditCursorBox
		{
			//	Donne la zone contenant le curseur d'�dition.
			get
			{
				return this.cursorBox;
			}
		}

		public override Drawing.Rectangle EditSelectBox
		{
			//	Donne la zone contenant le texte s�lectionn�.
			get
			{
				return this.selectBox;
			}
		}

		public override void EditMouseDownMessage(Point pos)
		{
			//	Gestion d'un �v�nement pendant l'�dition.
			//?pos = this.transform.TransformInverse(pos);
			//?this.textNavigator.MouseDownMessage(pos);
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

			//	Trait du rectangle.
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

		protected Path PathFlowHandlesStroke(IPaintPort port, DrawingContext drawingContext)
		{
			//	Cr�e le chemin des "poign�es" du flux de l'objet.
			Point prevP1;
			Point prevP2;
			Point prevP3;
			Point prevP4;
			this.CornersFlowPrev(out prevP1, out prevP2, out prevP3, out prevP4, drawingContext);

			Point nextP1;
			Point nextP2;
			Point nextP3;
			Point nextP4;
			this.CornersFlowNext(out nextP1, out nextP2, out nextP3, out nextP4, drawingContext);

			int count = this.textFlow.Count;
			int rank = this.textFlow.Rank(this);

			Path path = new Path();
			this.PathFlowIcon(path, prevP1, prevP2, prevP3, prevP4, port, drawingContext, rank == 0);
			this.PathFlowIcon(path, nextP1, nextP2, nextP3, nextP4, port, drawingContext, rank == count-1);
			return path;
		}

		protected Path PathFlowHandlesSurface(IPaintPort port, DrawingContext drawingContext)
		{
			//	Cr�e le chemin des "poign�es" du flux de l'objet.
			Point prevP1;
			Point prevP2;
			Point prevP3;
			Point prevP4;
			this.CornersFlowPrev(out prevP1, out prevP2, out prevP3, out prevP4, drawingContext);

			Point nextP1;
			Point nextP2;
			Point nextP3;
			Point nextP4;
			this.CornersFlowNext(out nextP1, out nextP2, out nextP3, out nextP4, drawingContext);

			int count = this.textFlow.Count;
			int rank = this.textFlow.Rank(this);

			Path path = new Path();

			if ( rank > 0 )
			{
				this.PathNumber(path, prevP1, prevP2, prevP3, prevP4, rank-1, false);
			}

			if ( rank < count-1 )
			{
				this.PathNumber(path, nextP1, nextP2, nextP3, nextP4, rank+1, true);
			}

			return path;
		}

		protected void PathFlowIcon(Path path, Point p1, Point p2, Point p3, Point p4, IPaintPort port, DrawingContext drawingContext, bool plus)
		{
			//	Cr�e le chemin d'une "poign�e" du flux de l'objet.
			if ( this.direction%90.0 == 0.0 )
			{
				this.Align(ref p1, port);
				this.Align(ref p2, port);
				this.Align(ref p3, port);
				this.Align(ref p4, port);

				double adjust = 0.5/drawingContext.ScaleX;
				p1.X += adjust;  p1.Y += adjust;
				p2.X -= adjust;  p2.Y += adjust;
				p3.X += adjust;  p3.Y -= adjust;
				p4.X -= adjust;  p4.Y -= adjust;
			}

			path.MoveTo(p1);
			path.LineTo(p2);
			path.LineTo(p4);
			path.LineTo(p3);
			path.Close();

			if ( plus )  // ic�ne "+" ?
			{
				path.MoveTo(this.PointFlowIcon(p1, p2, p3, p4, 0.25, 0.50));
				path.LineTo(this.PointFlowIcon(p1, p2, p3, p4, 0.75, 0.50));
				path.MoveTo(this.PointFlowIcon(p1, p2, p3, p4, 0.50, 0.25));
				path.LineTo(this.PointFlowIcon(p1, p2, p3, p4, 0.50, 0.75));
			}
			else	// ic�ne "v" ?
			{
				path.MoveTo(this.PointFlowIcon(p1, p2, p3, p4, 0.25, 0.65));
				path.LineTo(this.PointFlowIcon(p1, p2, p3, p4, 0.50, 0.35));
				path.LineTo(this.PointFlowIcon(p1, p2, p3, p4, 0.75, 0.65));
			}
		}

		protected Point PointFlowIcon(Point p1, Point p2, Point p3, Point p4, double dx, double dy)
		{
			Point x1 = Point.Scale(p1, p2, dx);
			Point x2 = Point.Scale(p3, p4, dx);
			return Point.Scale(x1, x2, dy);
		}

		protected void PathNumber(Path path, Point p1, Point p2, Point p3, Point p4, int number, bool rightToLeft)
		{
			number ++;  // 1..n
			string text = number.ToString(System.Globalization.CultureInfo.InvariantCulture);

			Font font = Font.DefaultFont;
			double fontSize = Point.Distance(p1, p3)*0.8;

			double textWidth = 0.0;
			for ( int i=0 ; i<text.Length ; i++ )
			{
				textWidth += font.GetCharAdvance(text[i])*fontSize;
			}

			double advance = 0;
			double offset = 0;
			double angle = Point.ComputeAngleDeg(p1, p2);
			Point center = p1;

			if ( rightToLeft )
			{
				advance -= textWidth + fontSize*0.3;
				offset = fontSize*0.1;
			}
			else
			{
				advance += Point.Distance(p1, p2) + fontSize*0.3;
				offset = fontSize*0.4;
			}

			for ( int i=0 ; i<text.Length ; i++ )
			{
				Transform transform = new Transform();
				transform.Scale(fontSize);
				transform.RotateDeg(angle);
				transform.Translate(center+Transform.RotatePointDeg(angle, new Point(advance, offset)));

				int glyph = font.GetGlyphIndex(text[i]);
				path.Append(font, glyph, transform);

				advance += font.GetCharAdvance(text[i])*fontSize;
			}
		}

		protected void CornersFlowPrev(out Point p1, out Point p2, out Point p3, out Point p4, DrawingContext drawingContext)
		{
			//	Calcules les 4 coins de la poign�e "pav� pr�c�dent".
			Point c1, c2, c3, c4;
			this.Corners(out c1, out c2, out c3, out c4);

			double d = Abstract.EditFlowHandleSize/drawingContext.ScaleX;
			p1 = c3;
			p2 = Point.Move(c3, c4,  d);
			p3 = Point.Move(c3, c1, -d);
			p4 = p3 + (p2-p1);
		}

		protected void CornersFlowNext(out Point p1, out Point p2, out Point p3, out Point p4, DrawingContext drawingContext)
		{
			//	Calcules les 4 coins de la poign�e "pav� suivant".
			Point c1, c2, c3, c4;
			this.Corners(out c1, out c2, out c3, out c4);

			double d = Abstract.EditFlowHandleSize/drawingContext.ScaleX;
			p4 = c2;
			p3 = Point.Move(c2, c1,  d);
			p2 = Point.Move(c2, c4, -d);
			p1 = p2 + (p3-p4);
		}

		protected void Align(ref Point p, IPaintPort port)
		{
			double x = p.X;
			double y = p.Y;
		
			port.Align(ref x, ref y);
		
			p.X = x;
			p.Y = y;
		}

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

		protected void UpdateTextFrame()
		{
			//	Met � jour le TextFrame en fonction des dimensions du pav�.
			Point p1, p2, p3, p4;
			this.Corners(out p1, out p2, out p3, out p4);
			
			double width  = Point.Distance(p1, p2);
			double height = Point.Distance(p1, p3);
			
			if ( this.textFrame.Width   != width  ||
				 this.textFrame.Height  != height ||
				 this.textFrame.OriginY != p4.Y   )
			{
				this.textFrame.OriginY = p4.Y;
				this.textFrame.Width   = width;
				this.textFrame.Height  = height;
				
				this.textFlow.TextStory.NotifyTextChanged();
			}
		}
		
		public bool IsInTextFrame(Drawing.Point pos, out Drawing.Point ppos)
		{
			//	D�termine si un point se trouve dans le texte frame.
			if ( this.transform == null )
			{
				ppos = Drawing.Point.Empty;
				return false;
			}
			
			ppos = this.transform.TransformInverse(pos);
			
			if ( ppos.X < 0 || ppos.Y < 0 || ppos.X > this.textFrame.Width || ppos.Y > this.textFrame.Height )
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public override void DrawText(IPaintPort port, DrawingContext drawingContext)
		{
			//	Dessine le texte du pav�.
			this.DrawText(port, drawingContext, InternalOperation.Painting);
		}

		protected void DrawText(IPaintPort port, DrawingContext drawingContext, InternalOperation op)
		{
			//	Effectue une op�ration quelconque sur le texte du pav�.
			this.internalOperation = op;

			this.cursorBox = Drawing.Rectangle.Empty;
			this.selectBox = Drawing.Rectangle.Empty;

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
#if false
			double ox = context.LineCurrentX;
			double oy = context.LineBaseY;
			double dx = context.TextWidth;
			
			this.graphics.LineWidth = 0.3;
			this.graphics.AddLine(ox, oy, ox + dx, oy);
			this.graphics.RenderSolid(Drawing.Color.FromName("Green"));
#endif
			
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
					if ( font.FontManagerType == OpenType.FontManagerType.System )
					{
						Drawing.NativeTextRenderer.Draw(this.graphics.Pixmap, font, size, glyphs, x, y, color.Basic);
					}
					else
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
										this.graphics.Rasterizer.AddGlyph(drawingFont, glyphs[i], x[i], y[i], size, sx == null ? 1.0 : sx[i], sy == null ? 1.0 : sy[i]);
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
										Point p1 = new Point(x[i],                 y[i]+oy);
										Point p2 = new Point(this.textFrame.Width, y[i]+oy);
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

		protected static bool XlineContains(System.Collections.ArrayList process, Text.Properties.AbstractXlineProperty xline, Text.Properties.FontColorProperty color)
		{
			//	Cherche si une propri�t� Xline est d�j� dans une liste.
			foreach ( XlineInfo existing in process )
			{
				if ( Text.Property.CompareEqualContents(existing.Xline, xline) &&
					 Text.Property.CompareEqualContents(existing.Color, color) )
				{
					return true;
				}
			}
			return false;
		}
		
		private class XlineInfo
		{
			public XlineInfo(Text.Properties.AbstractXlineProperty xline, Text.Properties.FontColorProperty color)
			{
				this.xline = xline;
				this.color = color;
			}
			
			
			public Text.Properties.AbstractXlineProperty Xline
			{
				get
				{
					return this.xline;
				}
			}
			
			public Text.Properties.FontColorProperty Color
			{
				get
				{
					return this.color;
				}
			}
			
			
			Text.Properties.AbstractXlineProperty	xline;
			Text.Properties.FontColorProperty		color;
		}
		#endregion


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
			info.AddValue("TextFlow", this.textFlow);
		}

		protected TextBox2(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui d�s�rialise l'objet.
			this.textFlow = (TextFlow) info.GetValue("TextFlow", typeof(TextFlow));
		}

		public override void ReadCheckWarnings(Font.FaceInfo[] fonts, System.Collections.ArrayList warnings)
		{
			//	V�rifie si tous les fichiers existent.
			//?Common.Document.Objects.Abstract.ReadCheckFonts(fonts, warnings, this.textLayout);
		}
		
		public override void ReadFinalize()
		{
			base.ReadFinalize ();
			
			this.InitialiseInternals();
		}
		
		public void ReadFinalizeFlowReady(TextFlow flow)
		{
			System.Diagnostics.Debug.Assert(this.textFlow == flow);
			
			this.UpdateTextFrame();
		}
		#endregion


		public void UpdateGeometry()
		{
			//	Met � jour apr�s un changement de g�om�trie de l'objet.
			this.UpdateTextFrame();
			this.UpdateTextLayout();
		}

		public void UpdateTextGrid(bool notify)
		{
			//	Met � jour le pav� en fonction des lignes magn�tiques.
			this.textFrame.GridStep   = this.document.Modifier.ActiveViewer.DrawingContext.TextGridStep;
			this.textFrame.GridOffset = this.document.Modifier.ActiveViewer.DrawingContext.TextGridOffset;

			if ( notify )
			{
				this.textFlow.TextStory.NotifyTextChanged();
				this.SetDirtyBbox();
				this.document.Notifier.NotifyArea(this.BoundingBox);
			}
		}

		public void UpdateTextLayout()
		{
			//	Met � jour le texte suite � une modification du conteneur.
			if ( this.edited )
			{
				this.textFlow.UpdateTextLayout();
			}
		}

		protected override void SetEdited(bool state)
		{
			//	Modifie le mode d'�dition. Il faut obligatoirement utiliser cet appel
			//	pour modifier this.edited !
			if ( this.edited == state )  return;

			this.edited = state;

			if ( this.document.HRuler == null )  return;

			this.document.HRuler.Edited = this.edited;
			this.document.VRuler.Edited = this.edited;

			if ( this.edited )
			{
				this.document.HRuler.EditObject = this;
				this.document.VRuler.EditObject = this;
				this.document.HRuler.WrappersAttach();
				this.document.VRuler.WrappersAttach();
				this.EditWrappersAttach();  // attache l'objet aux diff�rents wrappers
				
				this.textFlow.ActiveTextBox = this;
			}
			else
			{
				this.document.HRuler.EditObject = null;
				this.document.VRuler.EditObject = null;
				this.document.HRuler.WrappersDetach();
				this.document.VRuler.WrappersDetach();
				this.document.Wrappers.WrappersDetach();
				
				if ( this.textFlow.ActiveTextBox == this )
				{
					this.textFlow.ActiveTextBox = null;
				}
			}

			this.UpdateTextRulers();

			//	Redessine tout, � cause des "poign�es" du flux qui peuvent appara�tre
			//	ou dispara�tre.
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
		}

		
		#region OpletTextFlow
		protected void InsertOpletTextFlow()
		{
			//	Ajoute un oplet pour m�moriser le flux.
			if ( this.textFlow == null )  return;  // cr�ation de l'objet ?
			if ( !this.document.Modifier.OpletQueueEnable )  return;
			OpletTextFlow oplet = new OpletTextFlow(this);
			this.document.Modifier.OpletQueue.Insert(oplet);
		}

		//	M�morise le flux de l'objet.
		protected class OpletTextFlow : AbstractOplet
		{
			public OpletTextFlow(Objects.TextBox2 host)
			{
				System.Diagnostics.Debug.Assert(host.textFlow != null);
				this.host = host;
				this.textFlow = host.textFlow;
			}

			protected void Swap()
			{
				System.Diagnostics.Debug.Assert(host.textFlow != null);
				System.Diagnostics.Debug.Assert(this.textFlow != null);

				TextFlow temp = host.textFlow;
				host.textFlow = this.textFlow;
				this.textFlow = temp;

				host.UpdateTextLayout();
			}

			public override IOplet Undo()
			{
				this.Swap();
				return this;
			}

			public override IOplet Redo()
			{
				this.Swap();
				return this;
			}

			protected Objects.TextBox2				host;
			protected TextFlow						textFlow;
		}
		#endregion

		
		protected bool							isActive;
		protected ulong							markerSelected;
		protected TextFlow						textFlow;
		protected Text.SimpleTextFrame			textFrame;
		protected IPaintPort					port;
		protected Graphics						graphics;
		protected DrawingContext				drawingContext;
		protected Transform						transform;
		protected Drawing.Rectangle				redrawArea;
		protected Drawing.Rectangle				cursorBox;
		protected Drawing.Rectangle				selectBox;
		protected InternalOperation				internalOperation = InternalOperation.Painting;
		protected System.Collections.Hashtable	charactersTable = null;
		protected Drawing.Rectangle				mergingBoundingBox;
	}
}
