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
		protected enum InternalOperation
		{
			Painting,
			CharactersTable,
			RealBoundingBox,
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
			
			this.metaNavigator = new TextNavigator2();
			this.metaNavigator.TextFrame = this.textFrame;

			this.NewTextFlow();
			this.InitialiseInternals();
		}
		
		protected void InitialiseInternals()
		{
			if ( this.textFrame == null )
			{
				this.textFrame = new Text.SimpleTextFrame();
			}
			
			System.Diagnostics.Debug.Assert(this.textFlow != null);
			
			if ( this.metaNavigator == null )
			{
				this.metaNavigator = new TextNavigator2();
				this.metaNavigator.TextFrame = this.textFrame;
			}

			this.metaNavigator.TextChanged += new Support.EventHandler(this.HandleTextChanged);
			this.metaNavigator.TabsChanged += new Support.EventHandler(this.HandleTabsChanged);
			this.metaNavigator.CursorMoved += new Support.EventHandler(this.HandleCursorMoved);
			this.metaNavigator.ActiveStyleChanged += new Support.EventHandler(this.HandleStyleChanged);

			this.markerSelected = this.document.TextContext.Markers.Selected;

			this.cursorBox = Drawing.Rectangle.Empty;
			this.selectBox = Drawing.Rectangle.Empty;
		}

		// Crée un nouveau TextFlow pour l'objet.
		public void NewTextFlow()
		{
			TextFlow flow = new TextFlow(this.document);
			this.document.TextFlows.Add(flow);
			this.TextFlow = flow;
			flow.Add(this, null, true);
		}

		// TextFlow associé à l'objet.
		public TextFlow TextFlow
		{
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
					this.metaNavigator.TextNavigator = this.textFlow.TextNavigator;

					this.UpdateTextLayout();
					this.NotifyAreaFlow();
				}
			}
		}

		// Donne le TextFrame associé à l'objet.
		public Text.ITextFrame TextFrame
		{
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


		// Nom de l'icône.
		public override string IconName
		{
			get { return Misc.Icon("ObjectTextBox"); }
		}


		// Détecte si la souris est sur l'objet pour l'éditer.
		public override DetectEditType DetectEdit(Point pos)
		{
			if ( this.edited )
			{
				DetectEditType handle = this.DetectFlowHandle(pos);
				if ( handle != DetectEditType.Out )  return handle;
			}

			if ( this.Detect(pos) )  return DetectEditType.Body;
			return DetectEditType.Out;
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

			this.NotifyAreaFlow();
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
			this.NotifyAreaFlow();
		}

		// Déplace globalement l'objet.
		public override void MoveGlobalProcess(Selector selector)
		{
			base.MoveGlobalProcess(selector);
			this.UpdateGeometry();
			this.HandlePropertiesUpdate();
			this.NotifyAreaFlow();
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
			this.UpdateGeometry();
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
		public override void FillOneCharList(IPaintPort port, DrawingContext drawingContext, System.Collections.Hashtable table)
		{
			this.charactersTable = table;
			this.DrawText(port, drawingContext, InternalOperation.CharactersTable);
			this.charactersTable = null;
		}

		// Retourne la bounding réelle, en fonction des caractères contenus.
		public override Drawing.Rectangle RealBoundingBox()
		{
			this.mergingBoundingBox = Drawing.Rectangle.Empty;
			this.DrawText(null, null, InternalOperation.RealBoundingBox);

			return this.mergingBoundingBox;
		}

		// Indique si un objet est éditable.
		public override bool IsEditable
		{
			get { return true; }
		}


		// Reprend toutes les caractéristiques d'un objet.
		public override void CloneObject(Objects.Abstract src)
		{
			base.CloneObject(src);

			TextBox2 srcText = src as TextBox2;
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

			if ( message.KeyCodeOnly == KeyCode.Tab )
			{
				if ( message.Type == MessageType.KeyDown )
				{
					this.ProcessTabKey();
				}
				return true;
			}

			pos = this.transform.TransformInverse(pos);
			if ( !this.metaNavigator.ProcessMessage(message, pos) )  return false;
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

		// Gestion de la touche tab.
		protected bool ProcessTabKey()
		{
			Text.ITextFrame frame;
			double cx, cy, ascender, descender, angle;
			this.textFlow.TextNavigator.GetCursorGeometry(out frame, out cx, out cy, out ascender, out descender, out angle);

			double bestDistance = 1000000;
			Text.Properties.TabProperty bestTab = null;
			Text.TabList list = this.document.TextContext.TabList;
			string[] tags = this.textFlow.TextNavigator.GetAllTabTags();
			for ( int i=0 ; i<tags.Length ; i++ )
			{
				Text.Properties.TabProperty tab = list.GetTabProperty(tags[i]);
				double pos = list.GetTabPosition(tab);
				if ( pos > cx )
				{
					if ( bestDistance > pos-cx )
					{
						bestDistance = pos-cx;
						bestTab = tab;
					}
				}
			}
			if ( bestTab == null )  return false;

			this.metaNavigator.Insert(bestTab);
			return true;
		}

		// Détecte la "poignée" du flux de l'objet.
		protected DetectEditType DetectFlowHandle(Point pos)
		{
			DrawingContext drawingContext = this.document.Modifier.ActiveViewer.DrawingContext;

			Point prevP1 = new Point();
			Point prevP2 = new Point();
			Point prevP3 = new Point();
			Point prevP4 = new Point();
			this.CornersFlowPrev(ref prevP1, ref prevP2, ref prevP3, ref prevP4, drawingContext);

			InsideSurface surf = new InsideSurface(pos, 4);
			surf.AddLine(prevP1, prevP2);
			surf.AddLine(prevP2, prevP4);
			surf.AddLine(prevP4, prevP3);
			surf.AddLine(prevP3, prevP1);
			if ( surf.IsInside() )  return DetectEditType.HandleFlowPrev;

			Point nextP1 = new Point();
			Point nextP2 = new Point();
			Point nextP3 = new Point();
			Point nextP4 = new Point();
			this.CornersFlowNext(ref nextP1, ref nextP2, ref nextP3, ref nextP4, drawingContext);

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
			this.metaNavigator.Insert("");
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
				text = data.ReadHtmlFragment();
				if ( text != null )
				{
					text = Support.Clipboard.ConvertHtmlToSimpleXml(text);
				}
				else
				{
					text = data.ReadText();
				}
			}
			if ( text == null )  return false;
			this.metaNavigator.Insert(text);
			this.NotifyAreaFlow();
			return true;
		}

		public override bool EditSelectAll()
		{
			this.metaNavigator.SelectAll();
			return true;
		}
		#endregion

		// Insère un texte dans le pavé en édition.
		public override bool EditInsertText(string text, string fontFace, string fontStyle)
		{
			this.metaNavigator.EndSelection();
			this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.Text.Glyphs.Insert);

			if ( fontFace == "" )
			{
				this.metaNavigator.Insert(text);
			}
			else
			{
				for ( int i=0 ; i<text.Length ; i++ )
				{
					OpenType.Font font = TextContext.GetFont(fontFace, fontStyle);
					Text.Unicode.Code code = (Text.Unicode.Code) text[i];
					int glyph = font.GetGlyphIndex(text[i]);
					Text.Properties.OpenTypeProperty otp = new Text.Properties.OpenTypeProperty(fontFace, fontStyle, glyph);
					this.metaNavigator.Insert(code, otp);
				}
			}

			this.document.Modifier.OpletQueueValidateAction();
			this.NotifyAreaFlow();
			return true;
		}

		// Insère un glyphe dans le pavé en édition.
		public override bool EditInsertGlyph(int code, int glyph, string fontFace, string fontStyle)
		{
			this.metaNavigator.EndSelection();
			this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.Text.Glyphs.Alternate);

			if ( fontFace == "" )
			{
				string text = code.ToString();
				this.metaNavigator.Insert(text);
			}
			else
			{
				OpenType.Font font = TextContext.GetFont(fontFace, fontStyle);
				Text.Properties.OpenTypeProperty otp = new Text.Properties.OpenTypeProperty(fontFace, fontStyle, glyph);
				this.metaNavigator.Insert((Text.Unicode.Code)code, otp);
				this.metaNavigator.SelectInsertedCharacter();
			}

			this.document.Modifier.OpletQueueValidateAction();
			this.NotifyAreaFlow();
			return true;
		}

		// Retourne le glyphe du caractère sélectionné.
		public override bool EditGetSelectedGlyph(out int code, out int glyph, out OpenType.Font font)
		{
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

			string face, style;
			string[] features;
			this.GetTextFont(true, out face, out style, out features);
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


		// Met à jour le TextFrame en fonction du numéro de la page.
		protected override void UpdatePageNumber()
		{
			this.textFrame.PageNumber = this.pageNumber;
		}


		#region TextFormat
		// Modifie la police du texte.
		public override void SetTextFont(string face, string style, string[] features)
		{
			if ( face == "" )  // remet la fonte par défaut ?
			{
//				Text.Properties.FontProperty font = new Text.Properties.FontProperty();
//				this.metaNavigator.SetTextProperties(Text.Properties.ApplyMode.Clear, font);
				Text.TextStyle metaFont = this.document.TextContext.StyleList.CreateOrGetMetaProperty("TextFont", new Text.Property[0]);
				this.metaNavigator.SetMetaProperties(Text.Properties.ApplyMode.Clear, metaFont);
			}
			else
			{
				Text.Properties.FontProperty font;
				if ( features == null )
				{
					font = new Text.Properties.FontProperty(face, style);
				}
				else
				{
					font = new Text.Properties.FontProperty(face, style, features);
				}
				Text.TextStyle metaFont = this.document.TextContext.StyleList.CreateOrGetMetaProperty("TextFont", font);
				this.metaNavigator.SetMetaProperties(Text.Properties.ApplyMode.Set, metaFont);
//				this.metaNavigator.SetTextProperties(Text.Properties.ApplyMode.Combine, font);
			}
		}

		// Donne la police du texte.
		public override void GetTextFont(bool accumulated, out string face, out string style, out string[] features)
		{
#if false
			Text.Property[] properties = this.GetTextProperties(accumulated);
			foreach ( Text.Property property in properties )
			{
				if ( property.WellKnownType == Text.Properties.WellKnownType.Font )
				{
					Text.Properties.FontProperty font = property as Text.Properties.FontProperty;
					System.Diagnostics.Debug.Assert(font != null);
					face = font.FaceName;
					style = Misc.SimplifyFontStyle(font.StyleName);
					features = font.Features;
					return;
				}
			}
#else
			Text.TextStyle[] styles = this.textFlow.TextNavigator.TextStyles;
			foreach ( Text.TextStyle s in styles )
			{
				if ( s.TextStyleClass == Text.TextStyleClass.MetaProperty &&
					 s.MetaId == "TextFont" )
				{
					Text.Properties.FontProperty font = s[Text.Properties.WellKnownType.Font] as Text.Properties.FontProperty;
					System.Diagnostics.Debug.Assert(font != null);
					face = font.FaceName;
					style = Misc.SimplifyFontStyle(font.StyleName);
					features = font.Features;
					return;
				}
			}
#endif

			face = "";
			style = "";
			features = null;
		}

		// Modifie la taille de la police du texte.
		public override void SetTextFontSize(double size, Text.Properties.SizeUnits units, bool combine)
		{
			if ( units == Text.Properties.SizeUnits.None )  // remet la taille par défaut ?
			{
				Text.Properties.FontSizeProperty fs = new Text.Properties.FontSizeProperty();
				this.metaNavigator.SetTextProperties(Text.Properties.ApplyMode.Clear, fs);
			}
			else
			{
				Text.Properties.FontSizeProperty fs = new Text.Properties.FontSizeProperty(size, units);
				Text.Properties.ApplyMode mode = combine ? Text.Properties.ApplyMode.Combine : Text.Properties.ApplyMode.Set;
				this.metaNavigator.SetTextProperties(mode, fs);
			}
		}

		// Donne la taille de la police du texte.
		public override void GetTextFontSize(out double size, out Text.Properties.SizeUnits units, bool accumulated)
		{
			Text.Property[] properties = this.GetTextProperties(accumulated);
			foreach ( Text.Property property in properties )
			{
				if ( property.WellKnownType == Text.Properties.WellKnownType.FontSize )
				{
					Text.Properties.FontSizeProperty fs = property as Text.Properties.FontSizeProperty;
					System.Diagnostics.Debug.Assert(fs != null);

					size = fs.Size;
					units = fs.Units;
					return;
				}
			}
			
			size = 0;
			units = Text.Properties.SizeUnits.None;
		}

		// Modifie l'état d'un style de caractère.
		public override void SetTextStyle(string name, bool state)
		{
			Text.TextStyle style = this.document.TextContext.StyleList[name, Text.TextStyleClass.MetaProperty];
			if ( style != null )
			{
				Text.Properties.ApplyMode mode = state ? Text.Properties.ApplyMode.Set : Text.Properties.ApplyMode.Clear;
				this.metaNavigator.SetMetaProperties(mode, style);
			}
		}

		// Modifie l'état d'un style de paragraphe.
		public override void SetTextStyle(string name, string exclude, bool state)
		{
			this.ApplyParagraphStyle(name, exclude, state);
		}

		// Donne l'état d'un style de paragraphe.
		public override bool GetTextStyle(string name)
		{
			return this.IsExistingStyle(name);
		}

		// Modifie l'interligne du texte.
		public override void SetTextLeading(double size, Text.Properties.SizeUnits units)
		{
			if ( units == Text.Properties.SizeUnits.None )  // remet l'interligne par défaut ?
			{
				Text.Properties.LeadingProperty leading = new Text.Properties.LeadingProperty();
				this.metaNavigator.SetParagraphProperties(Text.Properties.ApplyMode.Clear, leading);
			}
			else
			{
				Text.Properties.LeadingProperty leading = new Text.Properties.LeadingProperty(size, units, Text.Properties.AlignMode.Undefined);
				this.metaNavigator.SetParagraphProperties(Text.Properties.ApplyMode.Set, leading);
			}
		}

		// Donne l'interligne du texte.
		public override void GetTextLeading(out double size, out Text.Properties.SizeUnits units, bool accumulated)
		{
			Text.Property[] properties = this.GetTextProperties(accumulated);
			foreach ( Text.Property property in properties )
			{
				if ( property.WellKnownType == Text.Properties.WellKnownType.Leading )
				{
					Text.Properties.LeadingProperty leading = property as Text.Properties.LeadingProperty;
					System.Diagnostics.Debug.Assert(leading != null);

					size = leading.Leading;
					units = leading.LeadingUnits;
					return;
				}
			}
			
			size = 0;
			units = Text.Properties.SizeUnits.None;
		}

		// Modifie les marges gauche du texte.
		public override void SetTextLeftMargins(double leftFirst, double leftBody, Text.Properties.SizeUnits units, bool enableUndoRedo)
		{
			if ( !enableUndoRedo )
			{
				this.textFlow.TextStory.DisableOpletQueue();
				this.SetTextLeftMargins(leftFirst, leftBody, units, true);
				this.textFlow.TextStory.EnableOpletQueue();
			}
			else
			{
				if ( units == Text.Properties.SizeUnits.None )  // remet l'indentation par défaut ?
				{
					Text.Properties.MarginsProperty margins = new Text.Properties.MarginsProperty();
					this.metaNavigator.SetParagraphProperties(Text.Properties.ApplyMode.Clear, margins);
				}
				else
				{
					Text.Properties.MarginsProperty margins = new Text.Properties.MarginsProperty(leftFirst, leftBody, double.NaN, double.NaN, units, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, Text.Properties.ThreeState.Undefined);
					this.metaNavigator.SetParagraphProperties(Text.Properties.ApplyMode.Combine, margins);
				}
				this.UpdateTextRulers();
			}
		}

		// Donne les msrges gauche du texte.
		public override void GetTextLeftMargins(out double leftFirst, out double leftBody, out Text.Properties.SizeUnits units, bool accumulated)
		{
			Text.Property[] properties = this.GetTextProperties(accumulated);
			foreach ( Text.Property property in properties )
			{
				if ( property.WellKnownType == Text.Properties.WellKnownType.Margins )
				{
					Text.Properties.MarginsProperty margins = property as Text.Properties.MarginsProperty;
					System.Diagnostics.Debug.Assert(margins != null);

					leftFirst = margins.LeftMarginFirstLine;
					leftBody = margins.LeftMarginBody;
					units = margins.Units;
					return;
				}
			}
			
			leftFirst = 0;
			leftBody = 0;
			units = Text.Properties.SizeUnits.None;
		}

		// Modifie la marge droite du texte.
		public override void SetTextRightMargins(double right, Text.Properties.SizeUnits units, bool enableUndoRedo)
		{
			if ( !enableUndoRedo )
			{
				this.textFlow.TextStory.DisableOpletQueue();
				this.SetTextRightMargins(right, units, true);
				this.textFlow.TextStory.EnableOpletQueue();
			}
			else
			{
				if ( units == Text.Properties.SizeUnits.None )  // remet l'indentation par défaut ?
				{
					Text.Properties.MarginsProperty margins = new Text.Properties.MarginsProperty();
					this.metaNavigator.SetParagraphProperties(Text.Properties.ApplyMode.Clear, margins);
				}
				else
				{
					Text.Properties.MarginsProperty margins = new Text.Properties.MarginsProperty(double.NaN, double.NaN, right, right, units, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, Text.Properties.ThreeState.Undefined);
					this.metaNavigator.SetParagraphProperties(Text.Properties.ApplyMode.Combine, margins);
				}
				this.UpdateTextRulers();
			}
		}

		// Donne la marge droite du texte.
		public override void GetTextRightMargins(out double right, out Text.Properties.SizeUnits units, bool accumulated)
		{
			Text.Property[] properties = this.GetTextProperties(accumulated);
			foreach ( Text.Property property in properties )
			{
				if ( property.WellKnownType == Text.Properties.WellKnownType.Margins )
				{
					Text.Properties.MarginsProperty margins = property as Text.Properties.MarginsProperty;
					System.Diagnostics.Debug.Assert(margins != null);

					right = margins.RightMarginBody;
					units = margins.Units;
					return;
				}
			}
			
			right = 0;
			units = Text.Properties.SizeUnits.None;
		}


		// Retourne tous les tags des tabulateurs.
		public override string[] TextTabTags
		{
			get
			{
				return this.textFlow.TextNavigator.GetAllTabTags();
			}
		}

		// Crée un nouveau tabulateur dans le texte.
		public override string NewTextTab(double pos, TextTabType type)
		{
			double dispo = 0.0;
			string dockingMark = Widgets.HRuler.ConvType2Mark(type);
			TabPositionMode positionMode = TabPositionMode.Absolute;

			if ( type == TextTabType.Center )  dispo = 0.5;
			if ( type == TextTabType.Right  )  dispo = 1.0;
			if ( type == TextTabType.Indent )  positionMode = TabPositionMode.AbsoluteIndent;

			Text.TabList list = this.document.TextContext.TabList;
			Text.Properties.TabProperty tab = list.NewTab(null, pos, Text.Properties.SizeUnits.Points, dispo, dockingMark, positionMode);
			Text.Properties.TabsProperty tabs = new Text.Properties.TabsProperty(tab);
			this.metaNavigator.SetParagraphProperties(Text.Properties.ApplyMode.Combine, tabs);
			return tab.TabTag;
		}

		// Supprime un tabulateur du texte.
		public override void DeleteTextTab(string tag)
		{
			this.metaNavigator.RemoveTab(tag);
		}

		// Renomme plusieurs tabulateurs du texte.
		public override bool RenameTextTabs(string[] oldTags, string newTag)
		{
			return this.textFlow.TextNavigator.RenameTabs(oldTags, newTag);
		}

		// Donne un tabulateur du texte.
		public override void GetTextTab(string tag, out double pos, out TextTabType type)
		{
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

		// Modifie un tabulateur du texte.
		public override void SetTextTab(ref string tag, double pos, TextTabType type, bool firstChange)
		{
			Text.TabList list = this.document.TextContext.TabList;
			
			double dispo = 0.0;
			string dockingMark = Widgets.HRuler.ConvType2Mark(type);
			TabPositionMode positionMode = TabPositionMode.Absolute;

			if ( type == TextTabType.Center )  dispo = 0.5;
			if ( type == TextTabType.Right  )  dispo = 1.0;
			if ( type == TextTabType.Indent )  positionMode = TabPositionMode.AbsoluteIndent;
			
			if ( firstChange && Text.TabList.GetTabClass(tag) == Text.TabClass.Auto )
			{
				// Les tabulateurs "automatiques" ne sont pas liés à un style. Leur
				// modification ne doit toucher que le paragraphe courant (ou la
				// sélection en cours), c'est pourquoi on crée une copie avant de
				// procéder à des modifications :
				
				Text.Properties.TabProperty oldTab = list.GetTabProperty(tag);
				Text.Properties.TabProperty newTab = list.NewTab(null, pos, Text.Properties.SizeUnits.Points, dispo, dockingMark, positionMode, null);
				
				this.textFlow.TextNavigator.RenameTab(oldTab.TabTag, newTab.TabTag);
				
				tag = newTab.TabTag;
			}
			else
			{
				Text.Properties.TabProperty tab = list.GetTabProperty(tag);
				list.RedefineTab(tab, pos, Text.Properties.SizeUnits.Points, dispo, dockingMark, positionMode, null);
			}
			
			this.HandleTabsChanged(null);  // TODO: devrait être inutile
		}


		// Donne la liste des propriétés.
		protected Text.Property[] GetTextProperties(bool accumulated)
		{
			if ( accumulated )
			{
				return this.textFlow.TextNavigator.AccumulatedTextProperties;
			}
			else
			{
				return this.textFlow.TextNavigator.TextProperties;
			}
		}

		// Indique l'existance d'un style.
		protected bool IsExistingStyle(string name)
		{
			Text.TextStyle[] styles = this.textFlow.TextNavigator.TextStyles;
			foreach ( Text.TextStyle style in styles )
			{
				if ( style.Name == name )  return true;
			}
			return false;
		}

		// Modifie un style de paragraphe.
		protected void ApplyParagraphStyle(string name, string exclude, bool state)
		{
			Text.TextStyle style = this.document.TextContext.StyleList[name, Text.TextStyleClass.Paragraph];
			if ( style == null )  return;

			System.Collections.ArrayList list = new System.Collections.ArrayList();
			Text.TextStyle[] styles = Text.TextStyle.FilterStyles(this.metaNavigator.TextNavigator.TextStyles, Text.TextStyleClass.Paragraph);
			
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
			this.metaNavigator.SetParagraphStyles(styles);
		}

		// Met à jour les règles pour le texte en édition.
		protected override void UpdateTextRulers()
		{
			if ( this.edited )
			{
				Drawing.Rectangle bbox = this.bboxThin;
				this.document.HRuler.LimitLow  = bbox.Left;
				this.document.HRuler.LimitHigh = bbox.Right;
				this.document.VRuler.LimitLow  = bbox.Bottom;
				this.document.VRuler.LimitHigh = bbox.Top;

				double leftFirst, leftBody, right;
				Text.Properties.SizeUnits units;
				this.GetTextLeftMargins(out leftFirst, out leftBody, out units, true);
				this.GetTextRightMargins(out right, out units, true);
				this.document.HRuler.MarginLeftFirst = bbox.Left+leftFirst;
				this.document.HRuler.MarginLeftBody  = bbox.Left+leftBody;
				this.document.HRuler.MarginRight     = bbox.Right-right;

				Text.TextNavigator.TabInfo[] infos = this.textFlow.TextNavigator.GetTabInfos();

				if ( true )
				{
					System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
					Text.TabList list = this.document.TextContext.TabList;
					
					buffer.Append("UpdateTextRulers:");
					
					foreach ( Text.TextNavigator.TabInfo info in infos )
					{
						string tag = info.Tag;
						Text.Properties.TabProperty tab = list[tag];
						TabStatus tabStatus = info.Status;
						TabClass  tabClass = Text.TabList.GetTabClass(tab);
						buffer.AppendFormat(" {0}->{1}/{2}({5}) p={3:0.0} m={4:0.0}", tag, tabStatus, tabClass, list.GetTabPositionInPoints(tab), list.GetTabDisposition(tab), list.GetTabUserCount(tab));
					}
					
					System.Diagnostics.Debug.WriteLine(buffer.ToString());
				}

				Widgets.Tab[] tabs = new Widgets.Tab[infos.Length];
				for ( int i=0 ; i<infos.Length ; i++ )
				{
					Text.TextNavigator.TabInfo info = infos[i] as Text.TextNavigator.TabInfo;

					double pos;
					TextTabType type;
					this.GetTextTab(info.Tag, out pos, out type);

					tabs[i].Tag = info.Tag;
					tabs[i].Pos = bbox.Left+pos;
					tabs[i].Type = type;
					tabs[i].Zombie = (info.Status == TabStatus.Zombie);
				}
				this.document.HRuler.Tabs = tabs;
			}
		}
		#endregion

		// Donne la zone contenant le curseur d'édition.
		public override Drawing.Rectangle EditCursorBox
		{
			get
			{
				return this.cursorBox;
			}
		}

		// Donne la zone contenant le texte sélectionné.
		public override Drawing.Rectangle EditSelectBox
		{
			get
			{
				return this.selectBox;
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

			bool flowHandles = this.edited && drawingContext != null;

			int totalShapes = 4;
			if ( flowHandles )  totalShapes += 2;

			Shape[] shapes = new Shape[totalShapes];
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

		// Crée le chemin des "poignées" du flux de l'objet.
		protected Path PathFlowHandlesStroke(IPaintPort port, DrawingContext drawingContext)
		{
			Point prevP1 = new Point();
			Point prevP2 = new Point();
			Point prevP3 = new Point();
			Point prevP4 = new Point();
			this.CornersFlowPrev(ref prevP1, ref prevP2, ref prevP3, ref prevP4, drawingContext);

			Point nextP1 = new Point();
			Point nextP2 = new Point();
			Point nextP3 = new Point();
			Point nextP4 = new Point();
			this.CornersFlowNext(ref nextP1, ref nextP2, ref nextP3, ref nextP4, drawingContext);

			int count = this.textFlow.Count;
			int rank = this.textFlow.Rank(this);

			Path path = new Path();
			this.PathFlowIcon(path, prevP1, prevP2, prevP3, prevP4, port, drawingContext, rank == 0);
			this.PathFlowIcon(path, nextP1, nextP2, nextP3, nextP4, port, drawingContext, rank == count-1);
			return path;
		}

		// Crée le chemin des "poignées" du flux de l'objet.
		protected Path PathFlowHandlesSurface(IPaintPort port, DrawingContext drawingContext)
		{
			Point prevP1 = new Point();
			Point prevP2 = new Point();
			Point prevP3 = new Point();
			Point prevP4 = new Point();
			this.CornersFlowPrev(ref prevP1, ref prevP2, ref prevP3, ref prevP4, drawingContext);

			Point nextP1 = new Point();
			Point nextP2 = new Point();
			Point nextP3 = new Point();
			Point nextP4 = new Point();
			this.CornersFlowNext(ref nextP1, ref nextP2, ref nextP3, ref nextP4, drawingContext);

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

		// Crée le chemin d'une "poignée" du flux de l'objet.
		protected void PathFlowIcon(Path path, Point p1, Point p2, Point p3, Point p4, IPaintPort port, DrawingContext drawingContext, bool plus)
		{
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

			if ( plus )  // icône "+" ?
			{
				path.MoveTo(this.PointFlowIcon(p1, p2, p3, p4, 0.25, 0.50));
				path.LineTo(this.PointFlowIcon(p1, p2, p3, p4, 0.75, 0.50));
				path.MoveTo(this.PointFlowIcon(p1, p2, p3, p4, 0.50, 0.25));
				path.LineTo(this.PointFlowIcon(p1, p2, p3, p4, 0.50, 0.75));
			}
			else	// icône "v" ?
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

		// Calcules les 4 coins de la poignée "pavé précédent".
		protected void CornersFlowPrev(ref Point p1, ref Point p2, ref Point p3, ref Point p4, DrawingContext drawingContext)
		{
			Point c1 = new Point();
			Point c2 = new Point();
			Point c3 = new Point();
			Point c4 = new Point();
			this.Corners(ref c1, ref c2, ref c3, ref c4);

			double d = Abstract.EditFlowHandleSize/drawingContext.ScaleX;
			p1 = c3;
			p2 = Point.Move(c3, c4,  d);
			p3 = Point.Move(c3, c1, -d);
			p4 = p3 + (p2-p1);
		}

		// Calcules les 4 coins de la poignée "pavé suivant".
		protected void CornersFlowNext(ref Point p1, ref Point p2, ref Point p3, ref Point p4, DrawingContext drawingContext)
		{
			Point c1 = new Point();
			Point c2 = new Point();
			Point c3 = new Point();
			Point c4 = new Point();
			this.Corners(ref c1, ref c2, ref c3, ref c4);

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

		// Calcules les 4 coins.
		protected void Corners(ref Point p1, ref Point p2, ref Point p3, ref Point p4)
		{
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

		// Met à jour le TextFrame en fonction des dimensions du pavé.
		protected void UpdateTextFrame()
		{
			Point p1 = new Point();
			Point p2 = new Point();
			Point p3 = new Point();
			Point p4 = new Point();
			this.Corners(ref p1, ref p2, ref p3, ref p4);

			this.textFrame.Width  = Point.Distance(p1, p2);
			this.textFrame.Height = Point.Distance(p1, p3);
		}

		// Dessine le texte du pavé.
		public override void DrawText(IPaintPort port, DrawingContext drawingContext)
		{
			this.DrawText(port, drawingContext, InternalOperation.Painting);
		}

		// Effectue une opération quelconque sur le texte du pavé.
		protected void DrawText(IPaintPort port, DrawingContext drawingContext, InternalOperation op)
		{
			this.internalOperation = op;

			this.cursorBox = Drawing.Rectangle.Empty;
			this.selectBox = Drawing.Rectangle.Empty;

			Point p1 = new Point();
			Point p2 = new Point();
			Point p3 = new Point();
			Point p4 = new Point();
			this.Corners(ref p1, ref p2, ref p3, ref p4);

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

			this.hasSelection = false;

			this.redrawArea = Drawing.Rectangle.Empty;
			if ( this.drawingContext != null )
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

			this.textFlow.TextStory.TextContext.ShowControlCharacters = this.edited;
			this.textFlow.TextFitter.RenderTextFrame(this.textFrame, this);

			if ( this.edited && !this.hasSelection && this.graphics != null && this.internalOperation == InternalOperation.Painting )
			{
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
			if ( this.edited == false )  return;

			double x1 = tabOrigin;
			double x2 = tabStop;
			double y  = layout.LineBaseY + layout.LineAscender*0.3;
			double a  = System.Math.Min(layout.LineAscender*0.3, (x2-x1)*0.5);

			Point p1 = new Point(x1, y);
			Point p2 = new Point(x2, y);
			graphics.Align(ref p1);
			graphics.Align(ref p2);
			double adjust = 0.5/drawingContext.ScaleX;
			p1.X += adjust;  p1.Y += adjust;
			p2.X -= adjust;  p2.Y += adjust;
			if ( p1.X >= p2.X )  return;

			Point p2a = new Point(p2.X-a, p2.Y-a*0.75);
			Point p2b = new Point(p2.X-a, p2.Y+a*0.75);

			Color color = isTabDefined ? Drawing.Color.FromBrightness(0.8) : DrawingContext.ColorTabZombie;
			
			if ( (tabCode & this.markerSelected) != 0 )  // tabulateur sélectionné ?
			{
				Drawing.Rectangle rect = new Drawing.Rectangle(x1, layout.LineY1, x2-x1, layout.LineY2-layout.LineY1);
				graphics.Align(ref rect);
				
				this.graphics.AddFilledRectangle(rect);
				this.graphics.RenderSolid(DrawingContext.ColorSelectEdit(this.isActive));

				if ( isTabDefined )  color = Drawing.Color.FromBrightness(0.5);
			}
			
			this.graphics.LineWidth = 1.0/drawingContext.ScaleX;
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

				// Vérifions d'abord que le mapping du texte vers les glyphes est
				// correct et correspond à quelque chose de valide :
				int  offset = 0;
				bool isInSelection = false;
				double selX = 0;

				System.Collections.ArrayList selRectList = null;

				double x1 = 0;
				double x2 = 0;

				int[]    cArray;  // unicodes
				ushort[] gArray;  // glyphes
				ulong[]  tArray;  // textes

				byte[]   iArray = new byte[glyphs.Length];  // insécables
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
								iArray[ii++] = 2;  // espace insécable
							}
							else
							{
								iArray[ii++] = 1;  // espace sécable
							}
						}
						else
						{
							iArray[ii++] = 0;  // pas un espace
						}
					}
					else
					{
						for ( int i=0 ; i<numGlyphs ; i++ )
						{
							iArray[ii++] = 0;  // pas un espace
						}
					}

					for ( int i=0 ; i<numChars ; i++ )
					{
						if ( (tArray[i] & this.markerSelected) != 0 )
						{
							// Le caractère considéré est sélectionné.
							if ( isInSelection == false )
							{
								// C'est le premier caractère d'une tranche. Il faut mémoriser son début :
								double xx = x1 + ((x2 - x1) * i) / numChars;
								isInSelection = true;
								selX = xx;
							}
						}
						else
						{
							if ( isInSelection )
							{
								// Nous avons quitté une tranche sélectionnée. Il faut mémoriser sa fin :
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
					// Nous avons quitté une tranche sélectionnée. Il faut mémoriser sa fin :
					double xx = x2;
					isInSelection = false;

					if ( xx > selX )
					{
						this.MarkSel(layout, ref selRectList, xx, selX);
					}
				}

				if ( this.edited && selRectList != null && this.graphics != null )
				{
					this.hasSelection = true;

					// Dessine les rectangles correspondant à la sélection.
					foreach ( Drawing.Rectangle rect in selRectList )
					{
						this.graphics.AddFilledRectangle(rect);
						this.selectBox.MergeWith(rect);
					}
					this.graphics.RenderSolid(DrawingContext.ColorSelectEdit(this.isActive));
				}

				// Dessine le texte.
				this.RenderText(font, size, glyphs, iArray, x, y, sx, sy, RichColor.FromName(color), isSpace);
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
							if ( i == 1 )  // TODO: césure gérée de façon catastrophique !
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

		// Marque une tranche sélectionnée.
		protected void MarkSel(Text.Layout.Context layout, ref System.Collections.ArrayList selRectList, double x, double selX)
		{
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

		// Effectue le rendu des caractères.
		protected void RenderText(Epsitec.Common.OpenType.Font font, double size, ushort[] glyphs, byte[] insecs, double[] x, double[] y, double[] sx, double[] sy, RichColor color, bool isSpace)
		{
			if ( this.internalOperation == InternalOperation.Painting )
			{
				if ( this.graphics != null )  // affichage sur écran ?
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

							if ( this.edited && isSpace && insecs != null )
							{
								for ( int i=0 ; i<glyphs.Length ; i++ )
								{
									double width = font.GetGlyphWidth(glyphs[i], size);
									double oy = font.GetAscender(size)*0.3;
									if ( insecs[i] == 1 )  // espace sécable ?
									{
										this.graphics.AddFilledCircle(x[i]+width/2, y[i]+oy, size*0.05);
									}
									if ( insecs[i] == 2 )  // espace insécable ?
									{
										this.graphics.AddCircle(x[i]+width/2, y[i]+oy, size*0.08);
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
						Path path = Path.FromLine(x1, y1, records[i].X, records[i].Y + records[i].Descender * 0.8);

						this.port.LineWidth = 1.0;
						this.port.RichColor = RichColor.FromName(color);
						this.port.PaintOutline(path);
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
			info.AddValue("TextFlow", this.textFlow);
		}

		// Constructeur qui désérialise l'objet.
		protected TextBox2(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.textFlow = (TextFlow) info.GetValue("TextFlow", typeof(TextFlow));
		}

		// Vérifie si tous les fichiers existent.
		public override void ReadCheckWarnings(Font.FaceInfo[] fonts, System.Collections.ArrayList warnings)
		{
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
			
			this.metaNavigator.TextNavigator = this.textFlow.TextNavigator;

			this.UpdateTextFrame();
		}
		#endregion


		private void HandleTextChanged(object sender)
		{
			if ( !this.edited )  return;
			this.UpdateTextLayout();
			this.document.Notifier.NotifyTextChanged();
			this.NotifyAreaFlow();
			this.ChangeObjectEdited();
		}
		
		private void HandleCursorMoved(object sender)
		{
			if ( !this.edited )  return;
			this.UpdateTextRulers();
			this.document.Notifier.NotifyTextChanged();
			this.NotifyAreaFlow();
			this.ChangeObjectEdited();
		}

		private void HandleStyleChanged(object sender)
		{
			if ( !this.edited )  return;
			this.UpdateTextLayout();
			this.document.Notifier.NotifyTextChanged();
			this.NotifyAreaFlow();
			this.ChangeObjectEdited();
		}

		private void HandleTabsChanged(object sender)
		{
			if ( !this.edited )  return;
			this.UpdateTextRulers();
			this.UpdateTextLayout();
			this.document.Notifier.NotifyTextChanged();
			this.NotifyAreaFlow();
			this.ChangeObjectEdited();
		}

		// Met à jour après un changement de géométrie de l'objet.
		public void UpdateGeometry()
		{
			this.UpdateTextFrame();
			this.UpdateTextLayout();
		}

		// Met à jour le texte suite à une modification du conteneur.
		public void UpdateTextLayout()
		{
//-			this.textFlow.TextFitter.ClearAllMarks();
			this.textFlow.TextFitter.GenerateMarks();

			this.textFlow.TextStory.NotifyTextChanged();

			// Indique qu'il faudra recalculer les bbox à toute la chaîne des pavés.
			UndoableList chain = this.textFlow.Chain;
			foreach ( Objects.Abstract obj in chain )
			{
				if ( obj == null )  continue;
				obj.SetDirtyBbox();
			}
		}

		// Notifie "à repeindre" toute la chaîne des pavés.
		public void NotifyAreaFlow()
		{
			System.Collections.ArrayList viewers = this.document.Modifier.AttachViewers;
			UndoableList chain = this.textFlow.Chain;

			foreach ( Viewer viewer in viewers )
			{
				int currentPage = viewer.DrawingContext.CurrentPage;
				foreach ( Objects.Abstract obj in chain )
				{
					if ( obj == null )  continue;
					if ( obj.PageNumber != currentPage )  continue;

					this.document.Notifier.NotifyArea(viewer, obj.BoundingBox);
				}
			}
		}

		// Change éventuellement le pavé édité en fonction de la position du curseur.
		protected void ChangeObjectEdited()
		{
			Text.ITextFrame frame;
			double cx, cy, ascender, descender, angle;
			this.textFlow.TextNavigator.GetCursorGeometry(out frame, out cx, out cy, out ascender, out descender, out angle);

			if ( frame != this.textFrame )
			{
				foreach ( TextBox2 obj in this.textFlow.Chain )
				{
					if ( frame == obj.textFrame )
					{
						this.document.Modifier.EditObject(obj);
						this.autoScrollOneShot = true;
						return;
					}
				}
			}
		}

		
		#region OpletTextFlow
		// Ajoute un oplet pour mémoriser le flux.
		protected void InsertOpletTextFlow()
		{
			if ( this.textFlow == null )  return;  // création de l'objet ?
			if ( !this.document.Modifier.OpletQueueEnable )  return;
			OpletTextFlow oplet = new OpletTextFlow(this);
			this.document.Modifier.OpletQueue.Insert(oplet);
		}

		// Mémorise le flux de l'objet.
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

				host.metaNavigator.TextNavigator = host.textFlow.TextNavigator;
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

		
		protected bool							hasSelection;
		protected bool							isActive;
		protected ulong							markerSelected;
		protected TextFlow						textFlow;
		protected Text.SimpleTextFrame			textFrame;
		protected TextNavigator2				metaNavigator;
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
