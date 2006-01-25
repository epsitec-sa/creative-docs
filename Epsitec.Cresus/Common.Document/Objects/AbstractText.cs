using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Objects
{
	/// <summary>
	/// La classe AbstractText est la classe de base pour tous les objets texte.
	/// </summary>
	[System.Serializable()]
	public abstract class AbstractText : Objects.Abstract
	{
		protected enum InternalOperation
		{
			Painting,
			GetPath,
			CharactersTable,
			RealBoundingBox,
			RealSelectPath,
		}

		protected enum SpaceType
		{
			None,			// ce n'est pas un espace
			BreakSpace,		// espace s�cable
			NoBreakSpace,	// espace ins�cable
			NewFrame,		// saut au prochain pav�
			NewPage,		// saut � la prochaine page
		}


		public AbstractText(Document document, Objects.Abstract model) : base(document, model)
		{
			if ( this.document == null )  return;  // objet factice ?
			this.CreateProperties(model, false);
			this.Initialise();
		}

		protected override Objects.Abstract CreateNewObject(Document document, Objects.Abstract model)
		{
			return null;
		}

		protected virtual void Initialise()
		{
			this.NewTextFlow();
			this.InitialiseInternals();
		}
		
		protected virtual void InitialiseInternals()
		{
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


		public override void Dispose()
		{
			if ( this.IsEdited )
			{
				this.Select(true, false);  // termine proprement l'�dition
			}

			if ( this.textFlow != null )
			{
				if ( this.document.Modifier.OpletQueue.IsActionDefinitionInProgress )
				{
					//	G�n�re des informations pour pouvoir faire un undo/redo
					//	du changement dans le flux de texte.
					
					TextFlow flow = this.textFlow;
					
					this.document.Modifier.OpletQueue.Insert(new Modifier.TextFlowChangeOplet(flow));
					this.textFlow.Remove(this);
					this.document.Modifier.OpletQueue.Insert(new Modifier.TextFlowChangeOplet(flow));
				}
				else
				{
					this.textFlow.Remove(this);  // objet seul dans son propre flux
				}
				
				if ( this.textFlow.Count == 1 )  // est-on le dernier et seul utilisateur ?
				{
					this.document.TextFlows.Remove(this.textFlow);
				}
			}

			base.Dispose();
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

		public Text.ITextFrame TextFrame
		{
			//	Donne le TextFrame associ� � l'objet.
			get
			{
				return this.textFrame;
			}
		}


		public override string IconName
		{
			//	Nom de l'ic�ne.
			get { return Misc.Icon("ObjectTextBox"); }
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


		public bool EditProcessMessage(Message message, Point pos)
		{
			//	Gestion d'un �v�nement pendant l'�dition.
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

			Objects.AbstractText obj = this;
			if ( !this.IsInTextFrame(pos) )
			{
				//	Si la souris n'est pas dans notre texte frame, on utilise le text
				//	frame correspondant � sa position (s'il y en a un).
				obj = this.textFlow.FindInTextFrame(pos);
				if ( obj == null )  obj = this;
			}
			ITextFrame frame = obj.textFrame;
			Point ppos = obj.ConvertInTextFrame(pos);
			//?System.Diagnostics.Debug.WriteLine(string.Format("name={0} pos={1};{2}", obj.PropertyName.String, ppos.X, ppos.Y));
			
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
					Viewer.MiniBarDelayed delayed = (message.ButtonDownCount > 1) ? Viewer.MiniBarDelayed.DoubleClick : Viewer.MiniBarDelayed.Immediately;
					viewer.OpenMiniBar(pos, delayed, false, false, distance);
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

		public void EditMouseDownMessage(Point pos)
		{
			//	Gestion d'un �v�nement pendant l'�dition.
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

		public override bool IsEditable
		{
			//	Indique si un objet est �ditable.
			get { return true; }
		}


		public override void CloneObject(Objects.Abstract src)
		{
			//	Reprend toutes les caract�ristiques d'un objet.
			base.CloneObject(src);

			AbstractText srcText = src as AbstractText;
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

			if ( this.document.Modifier.IsToolEdit )
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


		#region CopyPaste
		public virtual bool EditCut()
		{
			this.EditCopy();
			this.MetaNavigator.DeleteSelection();
			return true;
		}
		
		public virtual bool EditCopy()
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
		
		public virtual bool EditPaste()
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

		public virtual bool EditSelectAll()
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

			this.document.Wrappers.TextWrapper.SuspendSynchronizations();
			this.document.Wrappers.TextWrapper.Defined.FontFace = fontFace;
			this.document.Wrappers.TextWrapper.Defined.FontStyle = Misc.DefaultFontStyle(fontFace);
			this.document.Wrappers.TextWrapper.Defined.FontSize = fontSize;
			this.document.Wrappers.TextWrapper.Defined.Units = Text.Properties.SizeUnits.Points;
			this.document.Wrappers.TextWrapper.ResumeSynchronizations();

			this.textFlow.TextNavigator.ClearSelection();

			this.document.Wrappers.WrappersDetach();
			this.textFlow.ActiveTextBox = null;
		}

		public bool EditInsertText(string text, string fontFace, string fontStyle)
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

		public bool EditInsertText(Unicode.Code code)
		{
			//	Ins�re un texte dans le pav� en �dition.
			this.MetaNavigator.EndSelection();
			this.document.Modifier.OpletQueueBeginActionNoMerge(Res.Strings.Action.Text.Glyphs.Insert);

			this.MetaNavigator.Insert(code);

			this.document.Modifier.OpletQueueValidateAction();
			this.textFlow.NotifyAreaFlow();
			return true;
		}

		public bool EditInsertText(Text.Properties.BreakProperty brk)
		{
			//	Ins�re un texte dans le pav� en �dition.
			this.MetaNavigator.EndSelection();
			this.document.Modifier.OpletQueueBeginActionNoMerge(Res.Strings.Action.Text.Glyphs.Insert);

			this.MetaNavigator.Insert(brk);

			this.document.Modifier.OpletQueueValidateAction();
			this.textFlow.NotifyAreaFlow();
			return true;
		}

		public bool EditInsertGlyph(int code, int glyph, string fontFace, string fontStyle)
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

		public bool EditGetSelectedGlyph(out int code, out int glyph, out OpenType.Font font)
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

			string face = this.document.Wrappers.TextWrapper.Defined.FontFace;
			if ( face == null )
			{
				face = this.document.Wrappers.TextWrapper.Active.FontFace;
				if ( face == null )
				{
					face = "";
				}
			}

			string style = this.document.Wrappers.TextWrapper.Defined.FontStyle;
			if ( style == null )
			{
				style = this.document.Wrappers.TextWrapper.Active.FontStyle;
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
		public string[] TextTabTags
		{
			//	Retourne tous les tags des tabulateurs.
			get
			{
				return this.textFlow.TextNavigator.GetAllTabTags();
			}
		}

		public string NewTextTab(double pos, TextTabType type)
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

		public void DeleteTextTab(string tag)
		{
			//	Supprime un tabulateur du texte.
			this.MetaNavigator.RemoveTab(tag);
		}

		public bool RenameTextTabs(string[] oldTags, string newTag)
		{
			//	Renomme plusieurs tabulateurs du texte.
			return this.textFlow.TextNavigator.RenameTabs(oldTags, newTag);
		}

		public void GetTextTab(string tag, out double pos, out TextTabType type)
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

		public void SetTextTab(ref string tag, double pos, TextTabType type, bool firstChange)
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

		public virtual System.Collections.ArrayList CreateTextPanels(string filter)
		{
			//	Cr�e tous les panneaux pour l'�dition.
			System.Collections.ArrayList list = new System.Collections.ArrayList();

			string[] names =
			{
				"Justif", "Leading", "Margins", "Spaces", "Keep",	// styles de paragraphe
				"Font", "Xline", "Xscript", "Box", "Language"		// styles de caract�re
			};

			foreach ( string name in names )
			{
				if ( TextPanels.Abstract.IsFilterShow(name, filter) )
				{
					TextPanels.Abstract panel = TextPanels.Abstract.Create(name, this.document, false);

					if ( panel != null )
					{
						list.Add(panel);
					}
				}
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

		public virtual double WithForHRuler
		{
			//	Donne la largeur � utiliser pour la r�gle horizontale.
			get
			{
				return this.BoundingBoxThin.Width;
			}
		}
		#endregion


		#region FlowHandles
		protected DetectEditType DetectFlowHandle(Point pos)
		{
			//	D�tecte la "poign�e" du flux de l'objet.
			DrawingContext drawingContext = this.document.Modifier.ActiveViewer.DrawingContext;

			Point prevP1, prevP2, prevP3, prevP4;
			this.CornersFlowPrev(out prevP1, out prevP2, out prevP3, out prevP4, drawingContext);

			InsideSurface surf = new InsideSurface(pos, 4);
			surf.AddLine(prevP1, prevP2);
			surf.AddLine(prevP2, prevP4);
			surf.AddLine(prevP4, prevP3);
			surf.AddLine(prevP3, prevP1);
			if ( surf.IsInside() )  return DetectEditType.HandleFlowPrev;

			Point nextP1, nextP2, nextP3, nextP4;
			this.CornersFlowNext(out nextP1, out nextP2, out nextP3, out nextP4, drawingContext);

			surf = new InsideSurface(pos, 4);
			surf.AddLine(nextP1, nextP2);
			surf.AddLine(nextP2, nextP4);
			surf.AddLine(nextP4, nextP3);
			surf.AddLine(nextP3, nextP1);
			if ( surf.IsInside() )  return DetectEditType.HandleFlowNext;

			return DetectEditType.Out;
		}

		protected Path PathFlowHandlesStroke(IPaintPort port, DrawingContext drawingContext)
		{
			//	Cr�e le chemin des "poign�es" du flux de l'objet.
			Point prevP1, prevP2, prevP3, prevP4;
			this.CornersFlowPrev(out prevP1, out prevP2, out prevP3, out prevP4, drawingContext);

			Point nextP1, nextP2, nextP3, nextP4;
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
			Point prevP1, prevP2, prevP3, prevP4;
			this.CornersFlowPrev(out prevP1, out prevP2, out prevP3, out prevP4, drawingContext);

			Point nextP1, nextP2, nextP3, nextP4;
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
			double angle = 0;

			if ( this is TextBox2 )
			{
				angle = this.direction;
			}
			if ( this is TextLine2 )
			{
				angle = Point.ComputeAngleDeg(p1, p2);
			}

			if ( angle%90.0 == 0.0 )
			{
				this.Align(ref p1, port);
				this.Align(ref p2, port);
				this.Align(ref p3, port);
				this.Align(ref p4, port);

				double adjust = 0.5/drawingContext.ScaleX;
				p1.X += adjust;  p1.Y += adjust;
				p2.X += adjust;  p2.Y += adjust;
				p3.X += adjust;  p3.Y += adjust;
				p4.X += adjust;  p4.Y += adjust;
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
			else	// ic�ne fl�che ?
			{
				if ( this is TextBox2 )  // "v" ?
				{
					path.MoveTo(this.PointFlowIcon(p1, p2, p3, p4, 0.25, 0.65));
					path.LineTo(this.PointFlowIcon(p1, p2, p3, p4, 0.50, 0.35));
					path.LineTo(this.PointFlowIcon(p1, p2, p3, p4, 0.75, 0.65));
				}
				if ( this is TextLine2 )  // ">" ?
				{
					path.MoveTo(this.PointFlowIcon(p1, p2, p3, p4, 0.35, 0.25));
					path.LineTo(this.PointFlowIcon(p1, p2, p3, p4, 0.65, 0.50));
					path.LineTo(this.PointFlowIcon(p1, p2, p3, p4, 0.35, 0.75));
				}
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

			if ( this is TextBox2 )
			{
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
			}

			if ( this is TextLine2 )
			{
				if ( rightToLeft )
				{
					advance += Point.Distance(p1, p2) - textWidth;
					offset -= fontSize*1.0;
				}
				else
				{
					offset = -fontSize*1.0;
				}
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

		protected virtual void CornersFlowPrev(out Point p1, out Point p2, out Point p3, out Point p4, DrawingContext drawingContext)
		{
			//	Calcules les 4 coins de la poign�e "pav� pr�c�dent".
			p1 = Point.Empty;
			p2 = Point.Empty;
			p3 = Point.Empty;
			p4 = Point.Empty;
		}

		protected virtual void CornersFlowNext(out Point p1, out Point p2, out Point p3, out Point p4, DrawingContext drawingContext)
		{
			//	Calcules les 4 coins de la poign�e "pav� suivant".
			p1 = Point.Empty;
			p2 = Point.Empty;
			p3 = Point.Empty;
			p4 = Point.Empty;
		}

		protected void Align(ref Point p, IPaintPort port)
		{
			double x = p.X;
			double y = p.Y;
		
			port.Align(ref x, ref y);
		
			p.X = x;
			p.Y = y;
		}
		#endregion
		
		
		public Drawing.Rectangle EditCursorBox
		{
			//	Donne la zone contenant le curseur d'�dition.
			get
			{
				return this.cursorBox;
			}
		}

		public Drawing.Rectangle EditSelectBox
		{
			//	Donne la zone contenant le texte s�lectionn�.
			get
			{
				return this.selectBox;
			}
		}


		protected virtual void UpdateTextFrame()
		{
			//	Met � jour le TextFrame en fonction des dimensions du pav�.
		}
		
		public virtual bool IsInTextFrame(Drawing.Point pos)
		{
			//	D�termine si un point se trouve dans le texte frame.
			return this.Detect(pos);
		}

		public virtual Drawing.Point ConvertInTextFrame(Drawing.Point pos)
		{
			//	Calcule la coordonn�e transform�e dans le texte frame.
			return pos;
		}

		public override void DrawText(IPaintPort port, DrawingContext drawingContext)
		{
			//	Dessine le texte du pav�.
			this.DrawText(port, drawingContext, InternalOperation.Painting);
		}

		protected virtual void DrawText(IPaintPort port, DrawingContext drawingContext, InternalOperation op)
		{
			//	Effectue une op�ration quelconque sur le texte du pav�.
		}


		public override Path GetMagnetPath()
		{
			//	Retourne le chemin g�om�trique de l'objet pour les constructions
			//	magn�tiques.
			return null;
		}


		public virtual void UpdateGeometry()
		{
			//	Met � jour apr�s un changement de g�om�trie de l'objet.
			this.UpdateTextFrame();
			this.UpdateTextLayout();
		}

		public virtual void UpdateTextGrid(bool notify)
		{
			//	Met � jour le pav� en fonction des lignes magn�tiques.
		}

		public virtual void UpdateTextLayout()
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

			if ( this.document.HRuler != null )
			{
				this.document.HRuler.Edited = this.edited;
				this.document.VRuler.Edited = this.edited;
			}

			if ( this.edited )
			{
				if ( this.document.HRuler != null )
				{
					this.document.HRuler.EditObject = this;
					this.document.VRuler.EditObject = this;
					this.document.HRuler.WrappersAttach();
					this.document.VRuler.WrappersAttach();
				}
				this.EditWrappersAttach();  // attache l'objet aux diff�rents wrappers
				this.document.Notifier.NotifyTextChanged();  // pour mettre � jour le ruban TextStyles
				
				this.textFlow.ActiveTextBox = this;
			}
			else
			{
				if ( this.document.HRuler != null )
				{
					this.document.HRuler.EditObject = null;
					this.document.VRuler.EditObject = null;
					this.document.HRuler.WrappersDetach();
					this.document.VRuler.WrappersDetach();
				}
				this.document.Wrappers.WrappersDetach();
				
				if ( this.textFlow.ActiveTextBox == this )
				{
					this.textFlow.ActiveTextBox = null;
				}
			}

			this.UpdateTextRulers();

			//	Redessine tout, � cause des "poign�es" du flux qui peuvent appara�tre
			//	ou dispara�tre.
			//?this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
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
		
		protected class XlineInfo
		{
			public XlineInfo(Text.Properties.AbstractXlineProperty xline, Text.Properties.FontColorProperty color)
			{
				this.xline = xline;
				this.color = color;
			}
			
			public Text.Properties.AbstractXlineProperty Xline
			{
				get { return this.xline; }
			}
			
			public Text.Properties.FontColorProperty Color
			{
				get { return this.color; }
			}
			
			protected Text.Properties.AbstractXlineProperty		xline;
			protected Text.Properties.FontColorProperty			color;
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
			public OpletTextFlow(Objects.AbstractText host)
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

			protected Objects.AbstractText			host;
			protected TextFlow						textFlow;
		}
		#endregion

		
		#region Serialization
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	S�rialise l'objet.
			base.GetObjectData(info, context);
			info.AddValue("TextFlow", this.textFlow);
		}

		protected AbstractText(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui d�s�rialise l'objet.
			this.textFlow = (TextFlow) info.GetValue("TextFlow", typeof(TextFlow));
		}

		public override void ReadCheckWarnings(Font.FaceInfo[] fonts, System.Collections.ArrayList warnings)
		{
			//	V�rifie si tous les fichiers existent.
		}
		
		public override void ReadFinalize()
		{
			base.ReadFinalize();
			this.InitialiseInternals();
		}
		
		public override void ReadFinalizeFlowReady(TextFlow flow)
		{
			System.Diagnostics.Debug.Assert(this.textFlow == flow);
			this.UpdateTextFrame();
		}
		#endregion


		protected bool							isActive;
		protected ulong							markerSelected;
		protected TextFlow						textFlow;
		protected Text.ITextFrame				textFrame;
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
