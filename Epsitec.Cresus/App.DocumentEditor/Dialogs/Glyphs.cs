using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;
using Epsitec.Common.Text;
using Epsitec.Common.OpenType;

namespace Epsitec.App.DocumentEditor.Dialogs
{
	using GlobalSettings = Common.Document.Settings.GlobalSettings;

	/// <summary>
	/// Dialogue permettant de choisir un caractère quelconque à insérer dans
	/// un texte en édition.
	/// </summary>
	public class Glyphs : Abstract
	{
		public Glyphs(DocumentEditor editor) : base(editor)
		{
			this.fontFace = "Arial";
			this.fontStyle = Misc.DefaultFontStyle(this.fontFace);

			this.listSelectedIndex = new int[this.maxFamiliy];
			for ( int i=0 ; i<this.maxFamiliy ; i++ )
			{
				this.listSelectedIndex[i] = 0;
			}
		}

		// Crée et montre la fenêtre du dialogue.
		public override void Show()
		{
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.MakeSecondaryWindow();
				this.window.PreventAutoClose = true;
				this.WindowInit("Glyphs", 300, 260, true);
				this.window.Text = Res.Strings.Dialog.Glyphs.Title;
				this.window.Owner = this.editor.Window;
				this.window.Icon = Bitmap.FromManifestResource("Epsitec.App.DocumentEditor.Images.Application.icon", this.GetType().Assembly);
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);
				this.window.Root.MinSize = new Size(200, 200);

				ResizeKnob resize = new ResizeKnob(this.window.Root);
				resize.Anchor = AnchorStyles.BottomRight;
				resize.AnchorMargins = new Margins(0, 0, 0, 0);
				ToolTip.Default.SetToolTip(resize, Res.Strings.Dialog.Tooltip.Resize);

				// Crée les onglets.
				this.book = new TabBook(this.window.Root);
				this.book.Arrows = TabBookArrows.Stretch;
				this.book.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.TopAndBottom;
				this.book.ActivePageChanged += new EventHandler(this.HandleBookActivePageChanged);
				this.book.AnchorMargins = new Margins(6, 6, 6, 34);

				TabPage bookList = new TabPage();
				bookList.Name = "List";
				bookList.TabTitle = Res.Strings.Dialog.Glyphs.TabPage.List;
				this.book.Items.Add(bookList);

				TabPage bookArray = new TabPage();
				bookArray.Name = "Array";
				bookArray.TabTitle = Res.Strings.Dialog.Glyphs.TabPage.Array;
				this.book.Items.Add(bookArray);

				TabPage bookAlternates = new TabPage();
				bookAlternates.Name = "Alternates";
				bookAlternates.TabTitle = Res.Strings.Dialog.Glyphs.TabPage.Alternates;
				this.book.Items.Add(bookAlternates);

				this.book.ActivePage = bookList;

				// Onglet List.
				int tabIndex = 0;

				StaticText label = new StaticText(bookList);
				label.Text = Res.Strings.Dialog.Glyphs.Family.List;
				label.Width = 50;
				label.Anchor = AnchorStyles.TopLeft;
				label.AnchorMargins = new Margins(6, 0, 6+3, 0);

				this.family = new TextFieldCombo(bookList);
				this.family.Anchor = AnchorStyles.Top|AnchorStyles.LeftAndRight;
				this.family.AnchorMargins = new Margins(6+50, 6, 6, 0);
				this.family.IsReadOnly = true;
				this.family.TabIndex = tabIndex++;
				this.family.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.family.Items.Add(Res.Strings.Dialog.Glyphs.Family.Typo);
				this.family.Items.Add(Res.Strings.Dialog.Glyphs.Family.Space);
				this.family.Items.Add(Res.Strings.Dialog.Glyphs.Family.Business);
				this.family.Items.Add(Res.Strings.Dialog.Glyphs.Family.Math);
				this.family.Items.Add(Res.Strings.Dialog.Glyphs.Family.GreekLower);
				this.family.Items.Add(Res.Strings.Dialog.Glyphs.Family.GreekUpper);
				this.family.Items.Add(Res.Strings.Dialog.Glyphs.Family.Symbol);
				System.Diagnostics.Debug.Assert(this.family.Items.Count < this.maxFamiliy);
				this.family.SelectedIndex = 0;
				this.family.SelectedIndexChanged += new EventHandler(this.HandleFamilyChanged);

				this.list = new ScrollList(bookList);
				this.list.Dock = DockStyle.Fill;
				this.list.DockMargins = new Margins(6, 6, 6+20+4, 6);
				this.list.TabIndex = tabIndex++;
				this.list.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.list.SelectedIndexChanged += new EventHandler(this.HandleGlyphSelected);
				this.list.DoubleClicked += new MessageEventHandler(this.HandleDoubleClicked);

				this.UpdateList();

				// Onglet Array.
				tabIndex = 0;

				StaticText fontFaceLabel = new StaticText(bookArray);
				fontFaceLabel.Text = Res.Strings.Dialog.Glyphs.FontFace;
				fontFaceLabel.Width = 50;
				fontFaceLabel.Anchor = AnchorStyles.TopLeft;
				fontFaceLabel.AnchorMargins = new Margins(6, 0, 6+3, 0);

				this.fieldFontFace = new TextFieldCombo(bookArray);
				this.fieldFontFace.Anchor = AnchorStyles.Top|AnchorStyles.LeftAndRight;
				this.fieldFontFace.AnchorMargins = new Margins(6+50, 6+20+3, 6, 0);
				this.fieldFontFace.IsReadOnly = true;
				this.fieldFontFace.TabIndex = tabIndex++;
				this.fieldFontFace.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				Misc.AddFontList(this.fieldFontFace, true);
				this.fieldFontFace.Text = this.fontFace;
				if ( this.fieldFontFace.SelectedIndex < 0 )
				{
					this.fieldFontFace.Text = "Arial";
				}
				if ( this.fieldFontFace.SelectedIndex < 0 )
				{
					this.fieldFontFace.SelectedIndex = 0;
				}
				this.fieldFontFace.SelectedIndexChanged += new EventHandler(this.HandleFontFaceChanged);

				StaticText fontStyleLabel = new StaticText(bookArray);
				fontStyleLabel.Text = Res.Strings.Dialog.Glyphs.FontStyle;
				fontStyleLabel.Width = 50;
				fontStyleLabel.Anchor = AnchorStyles.TopLeft;
				fontStyleLabel.AnchorMargins = new Margins(6, 0, 6+20+4+3, 0);

				this.fieldFontStyle = new TextFieldCombo(bookArray);
				this.fieldFontStyle.Anchor = AnchorStyles.Top|AnchorStyles.LeftAndRight;
				this.fieldFontStyle.AnchorMargins = new Margins(6+50, 6+20+3, 6+20+4, 0);
				this.fieldFontStyle.IsReadOnly = true;
				this.fieldFontStyle.TabIndex = tabIndex++;
				this.fieldFontStyle.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.UpdateFontStyle();
				this.fieldFontStyle.Text = this.fontStyle;
				this.fieldFontStyle.SelectedIndexChanged += new EventHandler(this.HandleFontStyleChanged);

				this.currentFont = new GlyphButton(bookArray);
				this.currentFont.GlyphShape = GlyphShape.ArrowLeft;
				this.currentFont.Width = 20;
				this.currentFont.Height = 20+5+20;
				this.currentFont.Anchor = AnchorStyles.TopRight;
				this.currentFont.AnchorMargins = new Margins(0, 6, 6, 0);
				this.currentFont.TabIndex = tabIndex++;
				this.currentFont.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.currentFont.Clicked += new MessageEventHandler(this.HandleCurrentFontClicked);
				ToolTip.Default.SetToolTip(this.currentFont, Res.Strings.Dialog.Glyphs.Tooltip.CurrentFont);

				this.array = new GlyphArray(bookArray);
				this.array.Dock = DockStyle.Fill;
				this.array.DockMargins = new Margins(6, 6, 6+20+4+20+4, 6+20+4);
				this.array.TabIndex = tabIndex++;
				this.array.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.array.SetFont(this.fontFace, this.fontStyle);
				this.array.SelectedIndex = -1;
				this.array.DoubleClicked += new MessageEventHandler(this.HandleDoubleClicked);
				this.array.ChangeSelected += new EventHandler(this.HandleArraySelected);

				this.status = new TextField(bookArray);
				this.status.Anchor = AnchorStyles.Bottom|AnchorStyles.LeftAndRight;
				this.status.AnchorMargins = new Margins(6, 4+20+20+6, 0, 6);
				this.status.IsReadOnly = true;

				this.minus = new Button(bookArray);
				this.minus.Text = "\u2212";  // caractère "moins"
				this.minus.Width = 20;
				this.minus.Height = 20;
				this.minus.Anchor = AnchorStyles.Bottom|AnchorStyles.Right;
				this.minus.AnchorMargins = new Margins(6, 20+6, 0, 6);
				this.minus.TabIndex = tabIndex++;
				this.minus.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.minus.Clicked += new MessageEventHandler(this.HandleMinusClicked);
				ToolTip.Default.SetToolTip(this.minus, Res.Strings.Dialog.Glyphs.Tooltip.ArrayMinus);

				this.plus = new Button(bookArray);
				this.plus.Text = "+";
				this.plus.Width = 20;
				this.plus.Height = 20;
				this.plus.Anchor = AnchorStyles.Bottom|AnchorStyles.Right;
				this.plus.AnchorMargins = new Margins(6, 6, 0, 6);
				this.plus.TabIndex = tabIndex++;
				this.plus.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.plus.Clicked += new MessageEventHandler(this.HandlePlusClicked);
				ToolTip.Default.SetToolTip(this.plus, Res.Strings.Dialog.Glyphs.Tooltip.ArrayPlus);

				// Onglet Alternates.
				tabIndex = 0;

				label = new StaticText(bookAlternates);
				label.Text = Res.Strings.Dialog.Glyphs.Alternates.Help;
				label.Anchor = AnchorStyles.Top|AnchorStyles.LeftAndRight;
				label.AnchorMargins = new Margins(6, 6, 6, 0);

				this.alternatesArray = new GlyphArray(bookAlternates);
				this.alternatesArray.Dock = DockStyle.Fill;
				this.alternatesArray.DockMargins = new Margins(6, 6, 6+20, 6+20+4);
				this.alternatesArray.TabIndex = tabIndex++;
				this.alternatesArray.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.alternatesArray.SelectedIndex = -1;
				this.alternatesArray.CellSize = 48.828125;  // taille max
				this.alternatesArray.DoubleClicked += new MessageEventHandler(this.HandleDoubleClicked);
				this.alternatesArray.ChangeSelected += new EventHandler(this.HandleArraySelected);

				this.alternatesStatus = new TextField(bookAlternates);
				this.alternatesStatus.Anchor = AnchorStyles.Bottom|AnchorStyles.LeftAndRight;
				this.alternatesStatus.AnchorMargins = new Margins(6, 4+20+20+6, 0, 6);
				this.alternatesStatus.IsReadOnly = true;

				this.alternatesMinus = new Button(bookAlternates);
				this.alternatesMinus.Text = "\u2212";  // caractère "moins"
				this.alternatesMinus.Width = 20;
				this.alternatesMinus.Height = 20;
				this.alternatesMinus.Anchor = AnchorStyles.Bottom|AnchorStyles.Right;
				this.alternatesMinus.AnchorMargins = new Margins(6, 20+6, 0, 6);
				this.alternatesMinus.TabIndex = tabIndex++;
				this.alternatesMinus.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.alternatesMinus.Clicked += new MessageEventHandler(this.HandleMinusClicked);
				ToolTip.Default.SetToolTip(this.alternatesMinus, Res.Strings.Dialog.Glyphs.Tooltip.ArrayMinus);

				this.alternatesPlus = new Button(bookAlternates);
				this.alternatesPlus.Text = "+";
				this.alternatesPlus.Width = 20;
				this.alternatesPlus.Height = 20;
				this.alternatesPlus.Anchor = AnchorStyles.Bottom|AnchorStyles.Right;
				this.alternatesPlus.AnchorMargins = new Margins(6, 6, 0, 6);
				this.alternatesPlus.TabIndex = tabIndex++;
				this.alternatesPlus.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.alternatesPlus.Clicked += new MessageEventHandler(this.HandlePlusClicked);
				ToolTip.Default.SetToolTip(this.alternatesPlus, Res.Strings.Dialog.Glyphs.Tooltip.ArrayPlus);

				// Boutons de fermeture.
				Button buttonOk = new Button(this.window.Root);
				buttonOk.Width = 75;
				buttonOk.Text = Res.Strings.Dialog.Glyphs.Button.Insert;
				buttonOk.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonOk.Anchor = AnchorStyles.BottomLeft;
				buttonOk.AnchorMargins = new Margins(6, 0, 0, 6);
				buttonOk.Clicked += new MessageEventHandler(this.HandleButtonInsertClicked);
				buttonOk.TabIndex = 1000;
				buttonOk.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(buttonOk, Res.Strings.Dialog.Glyphs.Tooltip.Insert);

				Button buttonClose = new Button(this.window.Root);
				buttonClose.Width = 75;
				buttonClose.Text = Res.Strings.Dialog.Button.Close;
				buttonClose.Anchor = AnchorStyles.BottomLeft;
				buttonClose.AnchorMargins = new Margins(6+75+10, 0, 0, 6);
				buttonClose.Clicked += new MessageEventHandler(this.HandleButtonCloseClicked);
				buttonClose.TabIndex = 1001;
				buttonClose.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(buttonClose, Res.Strings.Dialog.Tooltip.Close);
			}

			this.window.Show();
			this.editor.CurrentDocument.Dialogs.BuildGlyphs(this.window);
		}

		// Enregistre la position de la fenêtre du dialogue.
		public override void Save()
		{
			this.WindowSave("Glyphs");
		}

		// Reconstruit le dialogue.
		public override void Rebuild()
		{
			if ( !this.editor.IsCurrentDocument )  return;
			if ( this.window == null )  return;
			this.editor.CurrentDocument.Dialogs.BuildGlyphs(this.window);
		}

		// Indique que l'onglet "caractères alternatifs" n'est plus à jour.
		public void SetAlternatesDirty()
		{
			this.alternatesDirty = true;
			this.UpdateAlternates();
		}

		// Met à jour l'onglet "caractères alternatifs".
		protected void UpdateAlternates()
		{
			if ( this.window == null )  return;
			if ( this.book.ActivePage.Name != "Alternates" )  return;
			if ( !this.alternatesDirty )  return;

			Common.OpenType.Font font = null;
			int code = 0;
			int glyph = 0;
			ushort[] alternates = null;
			Document document = this.editor.CurrentDocument;
			if ( document != null )
			{
				Common.Document.Objects.TextBox2 edit = document.Modifier.RetEditObject() as Common.Document.Objects.TextBox2;
				if ( edit != null )
				{
					if ( edit.EditGetSelectedGlyph(out code, out glyph, out font) )
					{
						font.PushActiveFeatures();

						System.Collections.ArrayList list = new System.Collections.ArrayList();
						string[] supported = font.GetSupportedFeatures();
						foreach ( string feature in supported )
						{
							Common.OpenType.LookupTable[] tables = font.GetLookupTables(feature);
							foreach ( Common.OpenType.LookupTable table in tables )
							{
								if ( table.LookupType == Common.OpenType.LookupType.Alternate || table.LookupType == Common.OpenType.LookupType.Single )
								{
									list.Add(feature);
									break;
								}
							}
						}
						string[] fix = (string[]) list.ToArray(typeof(string));
						font.SelectFeatures(fix);

						ushort normalGlyph = font.GetGlyphIndex(code);
						if ( !font.GetAlternates(normalGlyph, out alternates) )
						{
							alternates = null;
						}

						font.PopActiveFeatures();
					}
				}
			}
			this.alternatesArray.SetGlyphAlternates(font, code, glyph, alternates);

			this.alternatesDirty = false;
		}


		// Met à jour la liste des glyphs selon la famille choisie.
		protected void UpdateList()
		{
			int family = this.family.SelectedIndex;

			this.ignoreChanged = true;
			this.list.Items.Clear();  // vide le contenu précédent

			int[] codes = this.UnicodeList(family);
			if ( codes != null )
			{
				foreach ( int code in codes )
				{
					string text = Misc.GetUnicodeName(code);
					char c = (char) code;

					string complet = string.Format("  <font face=\"Arial\" size=\"120%\">{0}</font><tab/>{1}", c.ToString(), text);
					this.list.Items.Add(complet);
				}
			}

			this.list.SelectedIndex = this.listSelectedIndex[family];
			this.list.ShowSelected(ScrollShowMode.Extremity);
			this.ignoreChanged = false;
		}

		// Donne la liste des caractères Unicode d'une famille.
		protected int[] UnicodeList(int family)
		{
			if ( family == 0 )  return UnicodeTypo;
			if ( family == 1 )  return UnicodeSpace;
			if ( family == 2 )  return UnicodeBusiness;
			if ( family == 3 )  return UnicodeMath;
			if ( family == 4 )  return UnicodeGreekLower;
			if ( family == 5 )  return UnicodeGreekUpper;
			if ( family == 6 )  return UnicodeForm;
			return null;
		}

		protected int[] UnicodeTypo =
		{
			0x2013, 0x2014, 0x2015,  // tirets
			0x2026,  // ...
			0x2030,  // pour mille
			0x2116,  // No
			0x00AB, 0x00BB,  // guillemets
			0x2039, 0x203A,  // guillemets
			0x201C, 0x201D, 0x201E,  // guillemets
			0x2018, 0x2019, 0x201A, 0x201B,  // guillemets
			0x00C7,  // ç maj
			0x00DF,  // double s allemand
			0x0153,  // oe
			0x0152,  // OE
			0x00E6,  // ae
			0x00C6,  // AE
		};

		protected int[] UnicodeSpace =
		{
			0x00A0, 0x202F,  // NoBreakSpace, NarrowNoBreakSpace
			0x2001, 0x2000,  // cadratin
			0x2004, 0x2005, 0x2006,  // 1/3, 1/4, 1/6 cadratin
			0x2007,  // espace numérique
			0x2008,  // espace ponctuation
			0x2009,  // espace fine
			0x200A,  // espace untra-fine
			0x2002, 0x2003,  // cadrat
		};

		protected int[] UnicodeMath =
		{
			0x00B1,  // +/-
			0x2212,  // -
			0x00D7,  // x
			0x00F7,  // /
			0x2248,  // environ égal
			0x2260,  // différent
			0x2261,  // identique
			0x2264, 0x2265,  // <=, >=
			0x00B0,  // degré
			0x221E,  // infini
			0x00B5,  // micro
			0x03C0,  // pi
			0x2126,  // ohm
			0x221A,  // racine
			0x2211,  // somme
			0x220F,  // produit
			0x2206,  // incrément
			0x2202,  // dérivée partielle
			0x222B,  // intégrale
			0x2219,  // .
			0x2032, 0x2033,  // prime
			0x00BD,  // 1/2
			0x2153,  // 1/3
			0x2154,  // 2/3
			0x00BC,  // 1/4
			0x00BE,  // 3/4
			0x215B,  // 1/8
			0x215C,  // 3/8
			0x215D,  // 5/8
			0x215E,  // 7/8
			//0x2070,  // ^0
			0x00B9,  // ^1
			0x00B2,  // ^2
			0x00B3,  // ^3
			//0x2074,  // ^4
			//0x2075,  // ^5
			//0x2076,  // ^6
			//0x2077,  // ^7
			//0x2078,  // ^8
			//0x2079,  // ^9
			0x207F,  // ^n
		};

		protected int[] UnicodeGreekLower =
		{
			0x03B1, 0x03B2, 0x03B3, 0x03B4, 0x03B5, 0x03B6, 0x03B7, 0x03B8,
			0x03B9, 0x03BA, 0x03BB, 0x03BC, 0x03BD, 0x03BE, 0x03BF, 0x03C0,
			0x03C1, 0x03C2, 0x03C3, 0x03C4, 0x03C5, 0x03C6, 0x03C7, 0x03C8,
			0x03C9,
		};

		protected int[] UnicodeGreekUpper =
		{
			0x0391, 0x0392, 0x0393, 0x0394, 0x0395, 0x0396, 0x0397, 0x0398,
			0x0399, 0x039A, 0x039B, 0x039C, 0x039D, 0x039E, 0x039F, 0x03A0,
			0x03A1,         0x03A3, 0x03A4, 0x03A5, 0x03A6, 0x03A7, 0x03A8,
			0X03A9,
		};

		protected int[] UnicodeBusiness =
		{
			0x00A9,  // (c)
			0x00AE,  // (r)
			0x2122,  // TM
			0x20AC,  // Euro
			0x00A2,  // centime
			0x20A3,  // Franc français
			0x00A3,  // Livre
			0x00A5,  // Yen
			0x20A4,  // Lire
			0x20A7,  // Peseta
		};

		protected int[] UnicodeForm =
		{
			0x00A7,  // paragraphe
			0x2022,  // puce
			0x25A0, 0x25A1,  // carrés
			0x25AA, 0x25AB,  // petits carrés
			0x25CF, 0x25CB,  // cercles
			0x25CA,  // losange
			0x2190, 0x2192, 0x2191, 0x2193, 0x2194, 0x2195,  // flèches
			0x25C4, 0x25BA, 0x25B2, 0x25BC,  // triangles
			0x00AC,  // négation
			0x00B6,  // pied de mouche
			0x2020, 0x2021,  // obèle
			0x00BA, 0x00AA,  // indicateurs ordinal masculin/féminin
			0x2642, 0x2640,  // mâle/femelle
		};


		// Met à jour les boutons -/+.
		protected void UpdateMinusPlus()
		{
			if ( this.book.ActivePage.Name == "List" )  return;

			double size = this.Array.CellSize;
			this.Minus.SetEnabled(size > 20.0);
			this.Plus.SetEnabled(size < 48.0);
		}

		// Insère le glyphe sélectionné dans le texte en édition.
		protected void EditInsertGlyph()
		{
			if ( !this.editor.IsCurrentDocument )  return;

			if ( this.book.ActivePage.Name == "List" )
			{
				int family = this.family.SelectedIndex;

				int[] codes = this.UnicodeList(family);
				int sel = this.listSelectedIndex[family];
				char c = (char) codes[sel];
				string insert = c.ToString();
				this.editor.CurrentDocument.Modifier.EditInsertText(insert, "", "");
			}

			if ( this.book.ActivePage.Name == "Array" )
			{
				if ( this.array.SelectedIndex == -1 )  return;

				int code = this.array.IndexToUnicode(array.SelectedIndex);
				char c = (char) code;
				string insert = c.ToString();

				string fontFace, fontStyle;
				this.editor.CurrentDocument.Modifier.EditGetFont(out fontFace, out fontStyle);
				if ( fontFace != this.fontFace || fontStyle != this.fontStyle )
				{
					fontFace  = this.fontFace;
					fontStyle = this.fontStyle;
				}
				else
				{
					fontFace  = "";
					fontStyle = "";
				}
				this.editor.CurrentDocument.Modifier.EditInsertText(insert, fontFace, fontStyle);
			}

			if ( this.book.ActivePage.Name == "Alternates" )
			{
				if ( this.alternatesArray.SelectedIndex == -1 )  return;

				int code         = this.alternatesArray.Code;
				int glyph        = this.alternatesArray.SelectedGlyph;
				string fontFace  = this.alternatesArray.FontFace;
				string fontStyle = this.alternatesArray.FontStyle;

				this.editor.CurrentDocument.Modifier.EditInsertGlyph(code, glyph, fontFace, fontStyle);
			}
		}


		// Met à jour le TextFieldCombo des styles de police.
		protected void UpdateFontStyle()
		{
			this.fieldFontStyle.Items.Clear();  // vide la liste

			Common.OpenType.FontIdentity[] list = TextContext.GetAvailableFontIdentities(this.fontFace);
			foreach ( Common.OpenType.FontIdentity id in list )
			{
				this.fieldFontStyle.Items.Add(id.InvariantStyleName);
			}
		}

		// Change la police.
		protected void SetFontFace(string fontFace)
		{
			if ( this.array.SelectedIndex != -1 )
			{
				int code = this.array.IndexToUnicode(this.array.SelectedIndex);
				if ( code != 0 )
				{
					this.arrayProofCode = code;
				}
			}

			this.fontFace = fontFace;
			this.fieldFontFace.Text = this.fontFace;

			this.UpdateFontStyle();
			this.fontStyle = Misc.DefaultFontStyle(this.fontFace);
			this.fieldFontStyle.Text = this.fontStyle;

			this.array.SetFont(this.fontFace, this.fontStyle);

			this.array.SelectedIndex = this.array.UnicodeToIndex(this.arrayProofCode);
			this.array.ShowSelectedCell();
		}

		// Change le style de la police.
		protected void SetFontStyle(string fontStyle)
		{
			if ( this.array.SelectedIndex != -1 )
			{
				int code = this.array.IndexToUnicode(this.array.SelectedIndex);
				if ( code != 0 )
				{
					this.arrayProofCode = code;
				}
			}

			this.fontStyle = fontStyle;
			this.fieldFontStyle.Text = this.fontStyle;

			this.array.SetFont(this.fontFace, this.fontStyle);

			this.array.SelectedIndex = this.array.UnicodeToIndex(this.arrayProofCode);
			this.array.ShowSelectedCell();
		}

		// L'onglet actif a changé.
		private void HandleBookActivePageChanged(object sender)
		{
			this.UpdateAlternates();
			this.UpdateMinusPlus();
		}

		// Police changée.
		private void HandleFontFaceChanged(object sender)
		{
			this.SetFontFace(this.fieldFontFace.Text);
		}

		// Bouton "<-" cliqué.
		private void HandleCurrentFontClicked(object sender, MessageEventArgs e)
		{
			if ( !this.editor.IsCurrentDocument )  return;

			string fontFace, fontStyle;
			this.editor.CurrentDocument.Modifier.EditGetFont(out fontFace, out fontStyle);
			if ( fontFace == "" )  return;
			this.SetFontFace(fontFace);
			this.SetFontStyle(fontStyle);
		}

		// Style de la police changé.
		private void HandleFontStyleChanged(object sender)
		{
			this.SetFontStyle(this.fieldFontStyle.Text);
		}

		// Famille changée.
		private void HandleFamilyChanged(object sender)
		{
			this.UpdateList();
		}

		// Le glyphe dans la liste est sélectionné.
		private void HandleGlyphSelected(object sender)
		{
			if ( this.ignoreChanged )  return;

			int family = this.family.SelectedIndex;
			this.listSelectedIndex[family] = this.list.SelectedIndex;
		}

		// Le glyphe dans le tableau est sélectionné.
		private void HandleArraySelected(object sender)
		{
			string text = "";
			if ( this.Array.SelectedIndex != -1 )
			{
				if ( this.book.ActivePage.Name == "Array" )
				{
					int code = this.array.IndexToUnicode(this.Array.SelectedIndex);
					text = string.Format("{0}: {1}", code.ToString("X4"), Misc.GetUnicodeName(code, this.fontFace, this.fontStyle));
				}

				if ( this.book.ActivePage.Name == "Alternates" )
				{
					int code         = this.alternatesArray.Code;
					int glyph        = this.alternatesArray.SelectedGlyph;
					string fontFace  = this.alternatesArray.FontFace;
					string fontStyle = this.alternatesArray.FontStyle;
					text = string.Format("{0} [{1}]: {2}", code.ToString("X4"), glyph.ToString("X4"), Misc.GetUnicodeName(code, fontFace, fontStyle));
				}
			}
			this.Status.Text = text;
		}

		// Le glyphe est double-cliqué.
		private void HandleDoubleClicked(object sender, MessageEventArgs e)
		{
			this.EditInsertGlyph();
		}

		private void HandleMinusClicked(object sender, MessageEventArgs e)
		{
			double size = this.Array.CellSize;
			size = System.Math.Max(20.0, size/1.25);
			this.Array.CellSize = size;

			this.UpdateMinusPlus();
		}

		private void HandlePlusClicked(object sender, MessageEventArgs e)
		{
			double size = this.Array.CellSize;
			size = System.Math.Min(48.828125, size*1.25);
			this.Array.CellSize = size;

			this.UpdateMinusPlus();
		}

		private void HandleWindowCloseClicked(object sender)
		{
			this.editor.Window.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandleButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.editor.Window.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandleButtonInsertClicked(object sender, MessageEventArgs e)
		{
			this.EditInsertGlyph();
		}


		// Donne le tableau en fonction de l'onglet actif.
		protected GlyphArray Array
		{
			get
			{
				if ( this.book.ActivePage.Name == "Array"      )  return this.array;
				if ( this.book.ActivePage.Name == "Alternates" )  return this.alternatesArray;
				return null;
			}
		}

		// Donne le statuts en fonction de l'onglet actif.
		protected TextField Status
		{
			get
			{
				if ( this.book.ActivePage.Name == "Array"      )  return this.status;
				if ( this.book.ActivePage.Name == "Alternates" )  return this.alternatesStatus;
				return null;
			}
		}

		// Donne le bouton "-" en fonction de l'onglet actif.
		protected Button Minus
		{
			get
			{
				if ( this.book.ActivePage.Name == "Array"      )  return this.minus;
				if ( this.book.ActivePage.Name == "Alternates" )  return this.alternatesMinus;
				return null;
			}
		}

		// Donne le bouton "+" en fonction de l'onglet actif.
		protected Button Plus
		{
			get
			{
				if ( this.book.ActivePage.Name == "Array"      )  return this.plus;
				if ( this.book.ActivePage.Name == "Alternates" )  return this.alternatesPlus;
				return null;
			}
		}


		protected string			fontFace;
		protected string			fontStyle;
		protected readonly int		maxFamiliy = 10;
		protected int[]				listSelectedIndex;
		protected int				arrayProofCode = -1;
		protected bool				ignoreChanged = false;

		protected TabBook			book;

		protected TextFieldCombo	family;
		protected ScrollList		list;

		protected TextFieldCombo	fieldFontFace;
		protected TextFieldCombo	fieldFontStyle;
		protected GlyphButton		currentFont;
		protected GlyphArray		array;
		protected TextField			status;
		protected Button			minus;
		protected Button			plus;

		protected GlyphArray		alternatesArray;
		protected TextField			alternatesStatus;
		protected Button			alternatesMinus;
		protected Button			alternatesPlus;
		protected bool				alternatesDirty = true;
	}
}
