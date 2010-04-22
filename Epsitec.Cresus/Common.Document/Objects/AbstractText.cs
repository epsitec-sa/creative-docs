
#define SIMPLECOPYPASTE		// copier/coller tout simple

using System.Collections.Generic;
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
			BreakSpace,		// espace sécable
			NoBreakSpace,	// espace insécable
			NewFrame,		// saut au prochain pavé
			NewPage,		// saut à la prochaine page
		}


		public AbstractText(Document document, Objects.Abstract model) : base(document, model)
		{
			if ( this.document == null )  return;  // objet factice ?
			this.CreateProperties(model, false);
			this.Initialize();
		}

		protected override Objects.Abstract CreateNewObject(Document document, Objects.Abstract model)
		{
			return null;
		}

		protected virtual void Initialize()
		{
			this.NewTextFlow();
			this.InitializeInternals();
		}
		
		protected virtual void InitializeInternals()
		{
			this.UpdateTextGrid(false);
			
			System.Diagnostics.Debug.Assert(this.textFlow != null);
			
			this.markerSelected = this.document.TextContext.Markers.Selected;

			this.cursorBox = Drawing.Rectangle.Empty;
			this.selectBox = Drawing.Rectangle.Empty;
		}

		public void NewTextFlow()
		{
			//	Crée un nouveau TextFlow pour l'objet.
			TextFlow flow = new TextFlow(this.document);
			this.document.TextFlows.Add(flow);
			this.TextFlow = flow;
			flow.Add(this, null, true);
		}


		public override void Dispose()
		{
			if ( this.IsEdited )
			{
				this.Select(true, false);  // termine proprement l'édition
			}

			if ( this.textFlow != null )
			{
				if ( this.document.Modifier.OpletQueue.IsActionDefinitionInProgress )
				{
					//	Génère des informations pour pouvoir faire un undo/redo
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
			//	TextFlow associé à l'objet.
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
			//	MetaNavigator associé au TextFlow.
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
			//	Donne le TextFrame associé à l'objet.
			get
			{
				return this.textFrame;
			}
		}


		public override string IconUri
		{
			//	Nom de l'icône.
			get { return Misc.Icon("ObjectTextBox"); }
		}


		public override DetectEditType DetectEdit(Point pos)
		{
			//	Détecte si la souris est sur l'objet pour l'éditer.
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
			//	Gestion d'un événement pendant l'édition.
			if ( message.IsKeyType )
			{
				this.document.Modifier.ActiveViewer.CloseMiniBar(false);
			}

			if ( message.MessageType == MessageType.KeyDown   ||
				 message.MessageType == MessageType.KeyPress  ||
				 message.MessageType == MessageType.MouseDown )
			{
				this.SetAutoScroll();
			}

			if ( message.MessageType == MessageType.KeyPress )
			{
				if ( this.EditProcessKeyPress(message) )  return true;
			}

			if ( message.KeyCodeOnly == KeyCode.Tab )
			{
				if ( message.MessageType == MessageType.KeyDown )
				{
					this.ProcessTabKey();
				}
				return true;
			}

			Objects.AbstractText obj = this;
			if ( !this.IsInTextFrame(pos) )
			{
				//	Si la souris n'est pas dans notre texte frame, on utilise le text
				//	frame correspondant à sa position (s'il y en a un).
				obj = this.textFlow.FindInTextFrame(pos);
				if ( obj == null )  obj = this;
			}
			ITextFrame frame = obj.textFrame;
			Point ppos = obj.ConvertInTextFrame(pos);
			//?System.Diagnostics.Debug.WriteLine(string.Format("name={0} pos={1};{2}", obj.PropertyName.String, ppos.X, ppos.Y));
			
			if ( !this.MetaNavigator.ProcessMessage(message, ppos, frame) )  return false;
			
			if ( message.MessageType == MessageType.MouseDown )
			{
				this.document.Modifier.ActiveViewer.CloseMiniBar(false);
			}

			if ( message.MessageType == MessageType.MouseUp )
			{
				if ( this.textFlow.TextNavigator.SelectionCount > 0 )
				{
					Viewer viewer = this.document.Modifier.ActiveViewer;
					double distance = 0;
					Drawing.Rectangle selbox = this.EditSelectBox;
					if ( !selbox.IsEmpty )
					{
						selbox = viewer.InternalToScreen(selbox);
						double top = System.Math.Min(selbox.Top, viewer.ActualHeight-2);
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
			//	Gestion des événements clavier.
			if ( message.IsControlPressed )
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
			//	Gestion d'un événement pendant l'édition.
		}


		public override void FillOneCharList(IPaintPort port, DrawingContext drawingContext, System.Collections.Hashtable table)
		{
			//	Ajoute tous les caractères utilisés par l'objet dans une table.
			this.charactersTable = table;
			this.DrawText(port, drawingContext, InternalOperation.CharactersTable);
			this.charactersTable = null;
		}

		public override bool IsEditable
		{
			//	Indique si un objet est éditable.
			get { return true; }
		}


		public override void CloneObject(Objects.Abstract src)
		{
			//	Reprend toutes les caractéristiques d'un objet.
			base.CloneObject(src);

			AbstractText srcText = src as AbstractText;
			System.Diagnostics.Debug.Assert(srcText != null);

			if ( srcText.textFlow.Count == 1 ||  // objet solitaire ?
				 srcText.document != this.document )
			{
				this.textFlow.MergeWith(srcText.textFlow);  // copie le texte
			}
			else	// objet d'une chaîne ?
			{
				srcText.textFlow.Add(this, srcText, true);  // met dans la chaîne
			}

			this.UpdateGeometry();
		}


		public override void PutCommands(List<string> list)
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
					this.PutCommands(list, Res.Commands.FontBold.CommandId);
					this.PutCommands(list, Res.Commands.FontItalic.CommandId);
					this.PutCommands(list, Res.Commands.FontUnderline.CommandId);
					this.PutCommands(list, "");
					this.PutCommands(list, Commands.FontSubscript);
					this.PutCommands(list, Commands.FontSuperscript);
					this.PutCommands(list, "");
					this.PutCommands(list, Commands.FontSizeMinus);
					this.PutCommands(list, Commands.FontSizePlus);
					this.PutCommands(list, "");
					this.PutCommands(list, Commands.FontClear);
					this.PutCommands(list, "");
				}
				else
				{
					this.PutCommands(list, "Paste");
				}
			}
		}


		#region CopyPaste
		public bool EditCut()
		{
			this.EditCopy();
			this.MetaNavigator.DeleteSelection();
			return true;
		}

		public bool EditCopy()
		{
			Text.Exchange.ClipboardData clipboard = new Text.Exchange.ClipboardData ();
			
			bool ok = this.EditCopy (clipboard);

			clipboard.CopyToSystemClipboard ();
			
			return ok;
		}
		
		public virtual bool EditCopy(Text.Exchange.ClipboardData clipboard)
		{
#if false
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
#else
			TextFlow flow = this.TextFlow;
			Text.TextStory story = flow.TextStory;
			Text.TextNavigator navigator = flow.TextNavigator;
			
			Text.Exchange.Rosetta.CopyText (story, navigator, clipboard);
			return true;
#endif
		}

		public bool EditPaste()
		{
			Text.Exchange.ClipboardData clipboard = new Text.Exchange.ClipboardData ();
			clipboard.CopyFromSystemClipboard ();
			return this.EditPaste (clipboard);
		}
		
		public virtual bool EditPaste(Text.Exchange.ClipboardData clipboard)
		{
#if SIMPLECOPYPASTE
			if (clipboard.Contains (Common.Text.Exchange.Internal.FormattedText.ClipboardFormat.Name))
			{
				// colle du texte natif
				TextFlow flow = this.TextFlow;
				Text.TextStory story = flow.TextStory;
				Text.TextNavigator navigator = flow.TextNavigator;

				//	TODO: utiliser un texte des ressources
				this.document.Modifier.OpletQueueBeginAction ("** PASTE **");
				this.MetaNavigator.DeleteSelection (); // TODO: ATTENTION plante au undo suivant
				Text.Exchange.Rosetta.PasteNativeText (story, navigator, clipboard);

				// provoquer le raffichage de la liste des styles en haut dans l'onglet "Text"
				this.document.Notifier.NotifyTextStyleListChanged ();

				this.document.Modifier.OpletQueueValidateAction ();
				return true;
			}
			
			string text = clipboard.GetDataText ();
			
			if (string.IsNullOrEmpty (text))
			{
				return false;
			}
			
			text = text.Replace ("\r\n", "\u2029");		//	ParagraphSeparator
			text = text.Replace ("\n", "\u2028");		//	LineSeparator
			text = text.Replace ("\r", "\u2028");		//	LineSeparator
			
			this.MetaNavigator.Insert (text);
			this.textFlow.NotifyAreaFlow ();
			
			return true;
#else
#if false
			Support.Clipboard.ReadData data = Support.Clipboard.GetData();
			bool textInserted = false;

			if (data.IsCompatible (Clipboard.Format.MicrosoftHtml))
			{
				// colle du texte Html
				TextFlow flow = this.TextFlow;
				Text.TextStory story = flow.TextStory;
				Text.TextNavigator navigator = flow.TextNavigator;

				Text.Exchange.Rosetta.PasteHtmlText (story, navigator);
				textInserted = true;
			}
			else if (data.IsCompatible (Clipboard.Format.Text))
			{
				string text = data.ReadText();
				if (text != null)
				{
					text = text.Replace ("\r\n", "\u2029");		//	ParagraphSeparator
					text = text.Replace ("\n", "\u2028");		//	LineSeparator
					text = text.Replace ("\r", "\u2028");		//	LineSeparator

					this.MetaNavigator.Insert (text);
					textInserted = true;
				}
			}

			if (textInserted)
				this.textFlow.NotifyAreaFlow ();

			return textInserted;
#else
			System.Windows.Forms.IDataObject ido = System.Windows.Forms.Clipboard.GetDataObject ();
			bool textInserted = false;

			if (ido.GetDataPresent (Common.Text.Exchange.EpsitecFormat.Format.Name, false))
			{
				// colle du texte natif
				TextFlow flow = this.TextFlow;
				Text.TextStory story = flow.TextStory;
				Text.TextNavigator navigator = flow.TextNavigator;

				//	TODO: utiliser un texte des ressources
				this.document.Modifier.OpletQueueBeginAction ("** PASTE **");
				this.MetaNavigator.DeleteSelection (); // TODO: ATTENTION plante au undo suivant
				Text.Exchange.Rosetta.PasteNativeText (story, navigator);
				
				// provoquer le raffichage de la liste des styles en haut dans l'onglet "Text"
				this.document.Notifier.NotifyTextStyleListChanged ();
				
				this.document.Modifier.OpletQueueValidateAction ();
				textInserted = true;
			}
			else if (ido.GetDataPresent(System.Windows.Forms.DataFormats.Text, false))
			{
				string text = ido.GetData (System.Windows.Forms.DataFormats.Text, false) as string;
				if (text != null)
				{
					text = text.Replace ("\r\n", "\u2029");		//	ParagraphSeparator
					text = text.Replace ("\n", "\u2028");		//	LineSeparator
					text = text.Replace ("\r", "\u2028");		//	LineSeparator

					this.MetaNavigator.Insert (text);
					textInserted = true;
				}
			}

			if (textInserted)
				this.textFlow.NotifyAreaFlow ();

			return textInserted;

#endif
#endif
		}

		public virtual bool EditSelectAll()
		{
			this.MetaNavigator.SelectAll();
			return true;
		}
		#endregion


		public void SampleDefineStyle(Text.TextStyle style)
		{
			//	Change le style pour un échantillon.
			
			if ( this.style == style )  return;
			
			this.EditWrappersAttach();  // attache l'objet aux différents wrappers
			
			Text.TextNavigator navigator = this.TextFlow.TextNavigator;
			
			this.textFlow.ActiveTextBox = this;
			
			if (navigator.TextStory.IsOpletQueueEnabled)
			{
				navigator.TextStory.DisableOpletQueue();
			}
			
			navigator.MoveTo(Text.TextNavigator.Target.TextStart, 1);
			navigator.StartSelection();
			navigator.MoveTo(Text.TextNavigator.Target.TextEnd, 1);
			navigator.EndSelection();
			
			switch ( style.TextStyleClass )
			{
				case Text.TextStyleClass.Paragraph:
					navigator.SetParagraphStyles(style);
					break;
				
				case Text.TextStyleClass.Text:
					navigator.SetTextStyles(style);
					break;
			}
			
			navigator.ClearSelection();

			this.document.Wrappers.WrappersDetach();
			this.textFlow.ActiveTextBox = null;
			this.style = style;
		}

		public void EditInsertText(string text, string fontFace, double fontSize)
		{
			//	Insère un texte en provenance d'un ancien TextBox ou TextLine.
			this.EditInsertText(text, "", "");

			this.EditWrappersAttach();  // attache l'objet aux différents wrappers
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
			//	Insère un texte dans le pavé en édition.
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
			//	Insère un texte dans le pavé en édition.
			this.MetaNavigator.EndSelection();
			this.document.Modifier.OpletQueueBeginActionNoMerge(Res.Strings.Action.Text.Glyphs.Insert);

			this.MetaNavigator.Insert(code);

			this.document.Modifier.OpletQueueValidateAction();
			this.textFlow.NotifyAreaFlow();
			return true;
		}

		public bool EditInsertText(Text.Properties.BreakProperty brk)
		{
			//	Insère un texte dans le pavé en édition.
			this.MetaNavigator.EndSelection();
			this.document.Modifier.OpletQueueBeginActionNoMerge(Res.Strings.Action.Text.Glyphs.Insert);

			this.MetaNavigator.Insert(brk);

			this.document.Modifier.OpletQueueValidateAction();
			this.textFlow.NotifyAreaFlow();
			return true;
		}

		public bool EditInsertGlyph(int code, int glyph, string fontFace, string fontStyle)
		{
			//	Insère un glyphe dans le pavé en édition.
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
			//	Retourne le glyphe du caractère sélectionné.
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


		protected override void UpdatePageAndLayerNumbers()
		{
			//	Met à jour le TextFrame en fonction du numéro de la page.
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

		public static void NewTextTab(Document document, TextFlow textFlow, out string tag, double pos, TextTabType type, bool isStyle)
		{
			//	Crée un nouveau tabulateur dans le texte.
			double dispo = 0.0;
			string dockingMark = Widgets.HRuler.ConvType2Mark(type);
			TabPositionMode positionMode = TabPositionMode.Absolute;

			if ( type == TextTabType.Center )  dispo = 0.5;
			if ( type == TextTabType.Right  )  dispo = 1.0;
			if ( type == TextTabType.Indent )  positionMode = TabPositionMode.AbsoluteIndent;

			document.Modifier.OpletQueueBeginAction(Res.Strings.Action.Text.Tab.Create);
			Text.TabList list = document.TextContext.TabList;
			Text.Properties.TabProperty tab;

			if ( isStyle )  // style ?
			{
				tab = list.NewTab(TabList.GenericSharedName, pos, Text.Properties.SizeUnits.Points, dispo, dockingMark, positionMode);
				tag = tab.TabTag;  // voir (**)
				
				string[] initialTabs = document.Wrappers.StyleParagraphWrapper.Defined.Tabs;
				int length = (initialTabs == null) ? 0 : initialTabs.Length;
				string[] newTabs = new string[length+1];
				for ( int i=0 ; i<length ; i++ )
				{
					newTabs[i] = initialTabs[i];
				}
				newTabs[length] = tab.TabTag;
				document.Wrappers.StyleParagraphWrapper.Defined.Tabs = newTabs;
			}
			else
			{
				tab = list.NewTab(null, pos, Text.Properties.SizeUnits.Points, dispo, dockingMark, positionMode);
				tag = tab.TabTag;  // voir (**)
				Text.Properties.TabsProperty tabs = new Text.Properties.TabsProperty(tab);
				textFlow.MetaNavigator.SetParagraphProperties(Text.Properties.ApplyMode.Combine, tabs);
			}

			document.Modifier.OpletQueueValidateAction();
			document.SetDirtySerialize(CacheBitmapChanging.All);
		}

		public static void DeleteTextTab(Document document, TextFlow textFlow, string tag, bool isStyle)
		{
			//	Supprime un tabulateur du texte.
			document.Modifier.OpletQueueBeginAction(Res.Strings.Action.Text.Tab.Delete);

			if ( isStyle )  // style ?
			{
				string[] initialTabs = document.Wrappers.StyleParagraphWrapper.Defined.Tabs;
				int length = (initialTabs == null) ? 0 : initialTabs.Length;
				string[] newTabs = null;
				if ( length > 1 )
				{
					newTabs = new string[length-1];
					int j = 0;
					for ( int i=0 ; i<length ; i++ )
					{
						if ( initialTabs[i] != tag )
						{
							newTabs[j++] = initialTabs[i];
						}
					}
				}
				document.Wrappers.StyleParagraphWrapper.Defined.Tabs = newTabs;
			}
			else
			{
				textFlow.MetaNavigator.RemoveTab(tag);
			}

			document.Modifier.OpletQueueValidateAction();
			document.SetDirtySerialize(CacheBitmapChanging.All);
		}

		public bool RenameTextTabs(string[] oldTags, string newTag)
		{
			//	Renomme plusieurs tabulateurs du texte.
			return this.textFlow.TextNavigator.RenameTabs(oldTags, newTag);
		}

		public static void GetTextTab(Document document, string tag, out double pos, out TextTabType type)
		{
			//	Donne un tabulateur du texte.
			Text.TabList list = document.TextContext.TabList;
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

		public static void SetTextTab(Document document, TextFlow textFlow, ref string tag, double pos, TextTabType type, bool firstChange, bool isStyle)
		{
			//	Modifie un tabulateur du texte.
			double dispo = 0.0;
			string dockingMark = Widgets.HRuler.ConvType2Mark(type);
			TabPositionMode positionMode = TabPositionMode.Absolute;

			if ( type == TextTabType.Center )  dispo = 0.5;
			if ( type == TextTabType.Right  )  dispo = 1.0;
			if ( type == TextTabType.Indent )  positionMode = TabPositionMode.AbsoluteIndent;
			
			if ( isStyle )  // style ?
			{
				Text.TabList list = document.TextContext.TabList;
				list.RedefineTab(document.Modifier.OpletQueue, tag, pos, Text.Properties.SizeUnits.Points, dispo, dockingMark, positionMode, null);
			}
			else
			{
				if ( firstChange && Text.TabList.GetTabClass(tag) == Text.TabClass.Auto )
				{
					//	Les tabulateurs "automatiques" ne sont pas liés à un style. Leur
					//	modification ne doit toucher que le paragraphe courant (ou la
					//	sélection en cours), c'est pourquoi on crée une copie avant de
					//	procéder à des modifications :
					Text.TabList list = document.TextContext.TabList;
				
					Text.Properties.TabProperty oldTab = list.GetTabProperty(tag);
					Text.Properties.TabProperty newTab = list.NewTab(null, pos, Text.Properties.SizeUnits.Points, dispo, dockingMark, positionMode, null);
				
					// (**)
					//	Le nom du tabulateur doit être mis à jour avant que le wrapper envoie un
					//	événement pour indiquer le changement. C'est nécessaire pour que TextPanels.Tabs
					//	puisse mettre à jour la table en sélectionnant le bon tabulateur.
					tag = newTab.TabTag;  // voir (**)
					textFlow.TextNavigator.RenameTab(oldTab.TabTag, newTab.TabTag);
				}
				else
				{
					textFlow.TextNavigator.RedefineTab(tag, pos, Text.Properties.SizeUnits.Points, dispo, dockingMark, positionMode, null);
				}
			}
			
			document.Modifier.OpletQueueChangeLastNameAction(Res.Strings.Action.Text.Tab.Modify);
			document.SetDirtySerialize(CacheBitmapChanging.All);
		}


		public virtual System.Collections.ArrayList CreateTextPanels(string filter)
		{
			//	Crée tous les panneaux pour l'édition.
			System.Collections.ArrayList list = new System.Collections.ArrayList();

			string[] names =
			{
				"Justif", "Leading", "Margins", "Spaces", "Keep", "Numerator", "Tabs",	// styles de paragraphe
				"Font", "Xline", "Xscript", "Box", "Language"							// styles de caractère
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
			//	Donne la liste des propriétés.
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
			//	Attache l'objet au différents wrappers.
			this.document.Wrappers.WrappersAttach(this.textFlow);
		}

		protected override void UpdateTextRulers()
		{
			//	Met à jour les règles pour le texte en édition.
			if ( this.edited )
			{
				this.textFlow.UpdateTextRulers();
			}
		}

		public virtual double WidthForHRuler
		{
			//	Donne la largeur à utiliser pour la règle horizontale.
			get
			{
				return this.BoundingBoxThin.Width;
			}
		}
		#endregion


		#region FlowHandles
		protected DetectEditType DetectFlowHandle(Point pos)
		{
			//	Détecte la "poignée" du flux de l'objet.
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
			//	Crée le chemin des "poignées" du flux de l'objet.
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
			//	Crée le chemin des "poignées" du flux de l'objet.
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
			//	Crée le chemin d'une "poignée" du flux de l'objet.
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

			if ( plus )  // icône "+" ?
			{
				path.MoveTo(this.PointFlowIcon(p1, p2, p3, p4, 0.25, 0.50));
				path.LineTo(this.PointFlowIcon(p1, p2, p3, p4, 0.75, 0.50));
				path.MoveTo(this.PointFlowIcon(p1, p2, p3, p4, 0.50, 0.25));
				path.LineTo(this.PointFlowIcon(p1, p2, p3, p4, 0.50, 0.75));
			}
			else	// icône flèche ?
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
				Transform transform = Transform.Identity;
				transform = transform.Scale (fontSize);
				transform = transform.RotateDeg (angle);
				transform = transform.Translate (center+Transform.RotatePointDeg (angle, new Point (advance, offset)));

				int glyph = font.GetGlyphIndex(text[i]);
				path.Append(font, glyph, transform);

				advance += font.GetCharAdvance(text[i])*fontSize;
			}
		}

		protected virtual void CornersFlowPrev(out Point p1, out Point p2, out Point p3, out Point p4, DrawingContext drawingContext)
		{
			//	Calcules les 4 coins de la poignée "pavé précédent".
			p1 = Point.Zero;
			p2 = Point.Zero;
			p3 = Point.Zero;
			p4 = Point.Zero;
		}

		protected virtual void CornersFlowNext(out Point p1, out Point p2, out Point p3, out Point p4, DrawingContext drawingContext)
		{
			//	Calcules les 4 coins de la poignée "pavé suivant".
			p1 = Point.Zero;
			p2 = Point.Zero;
			p3 = Point.Zero;
			p4 = Point.Zero;
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
			//	Donne la zone contenant le curseur d'édition.
			get
			{
				return this.cursorBox;
			}
		}

		public Drawing.Rectangle EditSelectBox
		{
			//	Donne la zone contenant le texte sélectionné.
			get
			{
				return this.selectBox;
			}
		}


		protected virtual void UpdateTextFrame()
		{
			//	Met à jour le TextFrame en fonction des dimensions du pavé.
		}
		
		public virtual bool IsInTextFrame(Drawing.Point pos)
		{
			//	Détermine si un point se trouve dans le texte frame.
			return this.Detect(pos);
		}

		public virtual Drawing.Point ConvertInTextFrame(Drawing.Point pos)
		{
			//	Calcule la coordonnée transformée dans le texte frame.
			return pos;
		}

		public override void DrawText(IPaintPort port, DrawingContext drawingContext)
		{
			//	Dessine le texte du pavé.
			this.DrawText(port, drawingContext, InternalOperation.Painting);
		}

		protected virtual void DrawText(IPaintPort port, DrawingContext drawingContext, InternalOperation op)
		{
			//	Effectue une opération quelconque sur le texte du pavé.
		}


		public override Path GetMagnetPath()
		{
			//	Retourne le chemin géométrique de l'objet pour les constructions
			//	magnétiques.
			return null;
		}


		public virtual void UpdateGeometry()
		{
			//	Met à jour après un changement de géométrie de l'objet.
			this.UpdateTextFrame();
			this.UpdateTextLayout();
		}

		public virtual void UpdateTextGrid(bool notify)
		{
			//	Met à jour le pavé en fonction des lignes magnétiques.
		}

		public virtual void UpdateTextLayout()
		{
			//	Met à jour le texte suite à une modification du conteneur.
			if ( this.edited )
			{
				this.textFlow.UpdateTextLayout();
			}
		}

		protected override void SetEdited(bool state)
		{
			//	Modifie le mode d'édition. Il faut obligatoirement utiliser cet appel
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
				this.EditWrappersAttach();  // attache l'objet aux différents wrappers
				this.document.Notifier.NotifyTextChanged();  // pour mettre à jour le ruban TextStyles
				
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

			//	Redessine tout, à cause des "poignées" du flux qui peuvent apparaître
			//	ou disparaître.
			//?this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
		}

		
		protected static bool XlineContains(System.Collections.ArrayList process, Text.Properties.AbstractXlineProperty xline, Text.Properties.FontColorProperty color)
		{
			//	Cherche si une propriété Xline est déjà dans une liste.
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
			//	Retourne les chemins géométriques de l'objet.
			Graphics port = new Graphics();
			Drawing.PathAccumulationRasterizer rasterizer = new PathAccumulationRasterizer();
			port.ReplaceRasterizer(rasterizer);

			this.DrawText(port, null, InternalOperation.GetPath);

			return rasterizer.GetPaths();
		}

		
		#region OpletTextFlow
		protected void InsertOpletTextFlow()
		{
			//	Ajoute un oplet pour mémoriser le flux.
			if ( this.textFlow == null )  return;  // création de l'objet ?
			if ( !this.document.Modifier.OpletQueueEnable )  return;
			OpletTextFlow oplet = new OpletTextFlow(this);
			this.document.Modifier.OpletQueue.Insert(oplet);
		}

		//	Mémorise le flux de l'objet.
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
			//	Sérialise l'objet.
			base.GetObjectData(info, context);
			info.AddValue("TextFlow", this.textFlow);
		}

		protected AbstractText(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui désérialise l'objet.
			this.textFlow = (TextFlow) info.GetValue("TextFlow", typeof(TextFlow));
		}

		public override void ReadCheckWarnings(System.Collections.ArrayList warnings)
		{
			//	Vérifie si tous les fichiers existent.
		}
		
		public override void ReadFinalize()
		{
			base.ReadFinalize();
			this.InitializeInternals();
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
		protected Text.TextStyle				style;
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
