using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;
using Epsitec.Common.Text;
using Epsitec.Common.OpenType;

namespace Epsitec.Common.DocumentEditor.Dialogs
{
	using GlobalSettings = Common.Document.Settings.GlobalSettings;
	using FontFaceCombo  = Common.Document.Widgets.FontFaceCombo;
	using Document       = Common.Document.Document;

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

		public override void Show()
		{
			//	Crée et montre la fenêtre du dialogue.
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.MakeSecondaryWindow();
				this.window.PreventAutoClose = true;
				this.WindowInit("Glyphs", 300, 260, true);
				this.window.Text = Res.Strings.Dialog.Glyphs.Title;
				this.window.Owner = this.editor.Window;
				this.window.Icon = Bitmap.FromManifestResource ("Epsitec.Common.DocumentEditor.Images.Application.icon", this.GetType ().Assembly);
				this.window.WindowCloseClicked += this.HandleWindowCloseClicked;
				this.window.Root.MinSize = new Size(200, 200);

				ResizeKnob resize = new ResizeKnob(this.window.Root);
				resize.Anchor = AnchorStyles.BottomRight;
				resize.Margins = new Margins(0, 0, 0, 0);
				ToolTip.Default.SetToolTip(resize, Res.Strings.Dialog.Tooltip.Resize);

				//	Crée les onglets.
				this.book = new TabBook(this.window.Root);
				this.book.Arrows = TabBookArrows.Stretch;
				this.book.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.TopAndBottom;
				this.book.ActivePageChanged += new EventHandler<CancelEventArgs>(this.HandleBookActivePageChanged);
				this.book.Margins = new Margins(6, 6, 6, 34);

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

				//	Onglet List.
				int tabIndex = 0;

				StaticText label = new StaticText(bookList);
				label.Text = Res.Strings.Dialog.Glyphs.Family.List;
				label.PreferredWidth = 50;
				label.Anchor = AnchorStyles.TopLeft;
				label.Margins = new Margins(6, 0, 6+3, 0);

				this.family = new TextFieldCombo(bookList);
				this.family.Anchor = AnchorStyles.Top|AnchorStyles.LeftAndRight;
				this.family.Margins = new Margins(6+50, 6, 6, 0);
				this.family.IsReadOnly = true;
				this.family.TabIndex = tabIndex++;
				this.family.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				this.family.Items.Add(Res.Strings.Dialog.Glyphs.Family.Typo);
				this.family.Items.Add(Res.Strings.Dialog.Glyphs.Family.Space);
				this.family.Items.Add(Res.Strings.Dialog.Glyphs.Family.Business);
				this.family.Items.Add(Res.Strings.Dialog.Glyphs.Family.Math);
				this.family.Items.Add(Res.Strings.Dialog.Glyphs.Family.GreekLower);
				this.family.Items.Add(Res.Strings.Dialog.Glyphs.Family.GreekUpper);
				this.family.Items.Add(Res.Strings.Dialog.Glyphs.Family.Symbol);
				this.family.Items.Add(Res.Strings.Dialog.Glyphs.Family.Substitute);
				System.Diagnostics.Debug.Assert(this.family.Items.Count < this.maxFamiliy);
				this.family.SelectedItemIndex = 0;
				this.family.SelectedItemChanged += this.HandleFamilyChanged;

				this.list = new ScrollList(bookList);
				this.list.Dock = DockStyle.Fill;
				this.list.Margins = new Margins (6, 6, 6+20+4, 6);
				this.list.TabIndex = tabIndex++;
				this.list.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				this.list.SelectedItemChanged += this.HandleGlyphSelected;
				this.list.DoubleClicked += this.HandleDoubleClicked;

				this.UpdateList();

				//	Onglet Array.
				tabIndex = 0;

				StaticText fontFaceLabel = new StaticText(bookArray);
				fontFaceLabel.Text = Res.Strings.Dialog.Glyphs.FontFace;
				fontFaceLabel.PreferredWidth = 50;
				fontFaceLabel.Anchor = AnchorStyles.TopLeft;
				fontFaceLabel.Margins = new Margins(6, 0, 6+3, 0);

				this.fieldFontFace = new FontFaceCombo(bookArray);
				this.fieldFontFace.Anchor = AnchorStyles.Top|AnchorStyles.LeftAndRight;
				this.fieldFontFace.Margins = new Margins(6+50, 6+20+21+3, 6, 0);
				this.fieldFontFace.IsReadOnly = true;
				this.fieldFontFace.TabIndex = tabIndex++;
				this.fieldFontFace.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				this.fieldFontFace.Text = TextLayout.ConvertToTaggedText(this.fontFace);
				this.fieldFontFace.ComboOpening += new EventHandler<CancelEventArgs>(this.HandleFontFaceComboOpening);
				this.fieldFontFace.TextChanged += this.HandleFontFaceChanged;

				this.buttonFilter = new IconButton(bookArray);
				this.buttonFilter.PreferredWidth = 21;
				this.buttonFilter.PreferredHeight = 21;
				this.buttonFilter.CommandObject = Command.Get("TextFontFilter");
				this.buttonFilter.IconUri = Misc.Icon("TextFontFilter");
				this.buttonFilter.PreferredIconSize = Misc.IconPreferredSize("Normal");
				this.buttonFilter.AutoFocus = false;
				this.buttonFilter.ButtonStyle = ButtonStyle.ActivableIcon;
				this.buttonFilter.Anchor = AnchorStyles.Top|AnchorStyles.Right;
				this.buttonFilter.Margins = new Margins(6+50, 6+20+3, 6, 0);
				this.buttonFilter.TabIndex = tabIndex++;
				this.buttonFilter.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(this.buttonFilter, DocumentEditor.GetRes("Action.TextFontFilter"));

				StaticText fontStyleLabel = new StaticText(bookArray);
				fontStyleLabel.Text = Res.Strings.Dialog.Glyphs.FontStyle;
				fontStyleLabel.PreferredWidth = 50;
				fontStyleLabel.Anchor = AnchorStyles.TopLeft;
				fontStyleLabel.Margins = new Margins(6, 0, 6+20+4+3, 0);

				this.fieldFontStyle = new TextFieldCombo(bookArray);
				this.fieldFontStyle.Anchor = AnchorStyles.Top|AnchorStyles.LeftAndRight;
				this.fieldFontStyle.Margins = new Margins(6+50, 6+20+21+3, 6+20+4, 0);
				this.fieldFontStyle.IsReadOnly = true;
				this.fieldFontStyle.TabIndex = tabIndex++;
				this.fieldFontStyle.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				this.UpdateFontStyle();
				this.fieldFontStyle.Text = this.fontStyle;
				this.fieldFontStyle.SelectedItemChanged += this.HandleFontStyleChanged;

				this.currentFont = new GlyphButton(bookArray);
				this.currentFont.GlyphShape = GlyphShape.ArrowLeft;
				this.currentFont.PreferredWidth = 20;
				this.currentFont.PreferredHeight = 20+5+20;
				this.currentFont.Anchor = AnchorStyles.TopRight;
				this.currentFont.Margins = new Margins(0, 6, 6, 0);
				this.currentFont.TabIndex = tabIndex++;
				this.currentFont.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				this.currentFont.Clicked += this.HandleCurrentFontClicked;
				ToolTip.Default.SetToolTip(this.currentFont, Res.Strings.Dialog.Glyphs.Tooltip.CurrentFont);

				this.array = new Widgets.GlyphArray(bookArray);
				this.array.Dock = DockStyle.Fill;
				this.array.Margins = new Margins (6, 6, 6+20+4+20+4, 6+20+4);
				this.array.TabIndex = tabIndex++;
				this.array.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				this.array.SetFont(this.fontFace, this.fontStyle);
				this.array.SelectedItemIndex = -1;
				this.array.DoubleClicked += this.HandleDoubleClicked;
				this.array.ChangeSelected += this.HandleArraySelected;
				ToolTip.Default.SetToolTip(this.array, "*");

				this.status = new TextField(bookArray);
				this.status.Anchor = AnchorStyles.Bottom|AnchorStyles.LeftAndRight;
				this.status.Margins = new Margins(6, 4+80+6, 0, 6);
				this.status.IsReadOnly = true;

				this.slider = new HSlider(bookArray);
				this.slider.PreferredWidth = 80;
				this.slider.PreferredHeight = 14;
				this.slider.Anchor = AnchorStyles.Bottom|AnchorStyles.Right;
				this.slider.Margins = new Margins(6, 6, 0, 9);
				this.slider.TabIndex = tabIndex++;
				this.slider.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				this.slider.MinValue = 20.0M;
				this.slider.MaxValue = 50.0M;
				this.slider.SmallChange = 1.0M;
				this.slider.LargeChange = 10.0M;
				this.slider.Resolution = 1.0M;
				this.slider.Value = (decimal) this.array.CellSize;
				this.slider.ValueChanged += this.HandleSliderChanged;
				ToolTip.Default.SetToolTip(this.slider, Res.Strings.Dialog.Glyphs.Tooltip.ArraySize);

				//	Onglet Alternates.
				tabIndex = 0;

				label = new StaticText(bookAlternates);
				label.Text = Res.Strings.Dialog.Glyphs.Alternates.Help;
				label.Anchor = AnchorStyles.Top|AnchorStyles.LeftAndRight;
				label.Margins = new Margins(6, 6, 6, 0);

				this.alternatesArray = new Widgets.GlyphArray(bookAlternates);
				this.alternatesArray.Dock = DockStyle.Fill;
				this.alternatesArray.Margins = new Margins (6, 6, 6+20, 6+20+4);
				this.alternatesArray.TabIndex = tabIndex++;
				this.alternatesArray.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				this.alternatesArray.SelectedItemIndex = -1;
				this.alternatesArray.CellSize = 50;  // taille max
				this.alternatesArray.DoubleClicked += this.HandleDoubleClicked;
				this.alternatesArray.ChangeSelected += this.HandleArraySelected;

				this.alternatesStatus = new TextField(bookAlternates);
				this.alternatesStatus.Anchor = AnchorStyles.Bottom|AnchorStyles.LeftAndRight;
				this.alternatesStatus.Margins = new Margins(6, 4+80+6, 0, 6);
				this.alternatesStatus.IsReadOnly = true;

				this.alternatesSlider = new HSlider(bookAlternates);
				this.alternatesSlider.PreferredWidth = 80;
				this.alternatesSlider.PreferredHeight = 14;
				this.alternatesSlider.Anchor = AnchorStyles.Bottom|AnchorStyles.Right;
				this.alternatesSlider.Margins = new Margins(6, 6, 0, 9);
				this.alternatesSlider.TabIndex = tabIndex++;
				this.alternatesSlider.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				this.alternatesSlider.MinValue = 20.0M;
				this.alternatesSlider.MaxValue = 50.0M;
				this.alternatesSlider.SmallChange = 1.0M;
				this.alternatesSlider.LargeChange = 10.0M;
				this.alternatesSlider.Resolution = 1.0M;
				this.alternatesSlider.Value = (decimal) this.alternatesArray.CellSize;
				this.alternatesSlider.ValueChanged += this.HandleSliderChanged;
				ToolTip.Default.SetToolTip(this.alternatesSlider, Res.Strings.Dialog.Glyphs.Tooltip.ArraySize);

				//	Boutons de fermeture.
				Button buttonOk = new Button(this.window.Root);
				buttonOk.CommandObject = Command.Get("GlyphsInsert");
				buttonOk.PreferredWidth = 75;
				buttonOk.Text = Res.Strings.Dialog.Glyphs.Button.Insert;
				buttonOk.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonOk.Anchor = AnchorStyles.BottomRight;
				buttonOk.Margins = new Margins(0, 6+75+6, 0, 6);
				buttonOk.Clicked += this.HandleButtonInsertClicked;
				buttonOk.TabIndex = 1000;
				buttonOk.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(buttonOk, Res.Strings.Dialog.Glyphs.Tooltip.Insert);

				Button buttonClose = new Button(this.window.Root);
				buttonClose.PreferredWidth = 75;
				buttonClose.Text = Res.Strings.Dialog.Button.Close;
				buttonClose.ButtonStyle = ButtonStyle.DefaultCancel;
				buttonClose.Anchor = AnchorStyles.BottomRight;
				buttonClose.Margins = new Margins(0, 6, 0, 6);
				buttonClose.Clicked += this.HandleButtonCloseClicked;
				buttonClose.TabIndex = 1001;
				buttonClose.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(buttonClose, Res.Strings.Dialog.Tooltip.Close);
			}

			this.window.Show();
			this.editor.CurrentDocument.Dialogs.BuildGlyphs(this.window);
		}

		public override void Save()
		{
			//	Enregistre la position de la fenêtre du dialogue.
			this.WindowSave("Glyphs");
		}

		public override void Rebuild()
		{
			//	Reconstruit le dialogue.
			if ( !this.editor.HasCurrentDocument )  return;
			if ( this.window == null )  return;
			this.editor.CurrentDocument.Dialogs.BuildGlyphs(this.window);
		}

		public void SetAlternatesDirty()
		{
			//	Indique que l'onglet "caractères alternatifs" n'est plus à jour.
			this.alternatesDirty = true;
			this.UpdateAlternates();
		}

		protected void UpdateAlternates()
		{
			//	Met à jour l'onglet "caractères alternatifs".
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
			this.HandleArraySelected(null);

			this.alternatesDirty = false;
		}


		protected void UpdateList()
		{
			//	Met à jour la liste des glyphs selon la famille choisie.
			int family = this.family.SelectedItemIndex;

			this.ignoreChanged = true;
			this.list.Items.Clear();  // vide le contenu précédent

			if ( family == 7 )  // textes de substitutions ?
			{
				this.list.Items.Add(Res.Strings.Dialog.Glyphs.Substitute.Title.Latin);
				this.list.Items.Add(Res.Strings.Dialog.Glyphs.Substitute.Title.French);
				this.list.Items.Add(Res.Strings.Dialog.Glyphs.Substitute.Title.English);
			}
			else
			{
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
			}

			this.list.SelectedItemIndex = this.listSelectedIndex[family];
			this.list.ShowSelected(ScrollShowMode.Extremity);
			this.ignoreChanged = false;
		}

		protected int[] UnicodeList(int family)
		{
			//	Donne la liste des caractères Unicode d'une famille.
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


		protected void EditInsert()
		{
			//	Insère le glyphe selon l'onglet actif dans le texte en édition.
			if ( !this.editor.HasCurrentDocument )  return;

			if ( this.book.ActivePage.Name == "List" )
			{
				int family = this.family.SelectedItemIndex;
				int sel = this.listSelectedIndex[family];

				if ( family == 7 )  // textes de substitutions ?
				{
					string text = "";
					switch ( sel )
					{
						case 0:  text = Res.Strings.Dialog.Glyphs.Substitute.Text.Latin;    break;
						case 1:  text = Res.Strings.Dialog.Glyphs.Substitute.Text.French;   break;
						case 2:  text = Res.Strings.Dialog.Glyphs.Substitute.Text.English;  break;
					}

					text = TextLayout.ConvertToSimpleText(text);  // conversion des &quot; en "
					this.editor.CurrentDocument.Modifier.EditInsertText(text, "", "");
					this.editor.CurrentDocument.Modifier.EditInsertText(Unicode.Code.ParagraphSeparator);
				}
				else
				{
					int[] codes = this.UnicodeList(family);
					char c = (char) codes[sel];
					string insert = c.ToString();
					this.editor.CurrentDocument.Modifier.EditInsertText(insert, "", "");
				}
			}

			if ( this.book.ActivePage.Name == "Array" )
			{
				if (this.array.SelectedItemIndex == -1)
					return;

				int code = this.array.IndexToUnicode (array.SelectedItemIndex);
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
				if (this.alternatesArray.SelectedItemIndex == -1)
					return;

				int code         = this.alternatesArray.Code;
				int glyph        = this.alternatesArray.SelectedGlyph;
				string fontFace  = this.alternatesArray.FontFace;
				string fontStyle = this.alternatesArray.FontStyle;

				this.editor.CurrentDocument.Modifier.EditInsertGlyph(code, glyph, fontFace, fontStyle);
			}
			
			this.editor.Window.MakeFocused();
			this.editor.Window.RestoreLogicalFocus();
		}


		protected void UpdateFontStyle()
		{
			//	Met à jour le TextFieldCombo des styles de police.
			this.fieldFontStyle.Items.Clear();  // vide la liste

			Common.OpenType.FontIdentity[] list = TextContext.GetAvailableFontIdentities(this.fontFace);
			foreach ( Common.OpenType.FontIdentity id in list )
			{
				this.fieldFontStyle.Items.Add(id.InvariantStyleName);
			}
		}

		protected void SetFontFace(string fontFace)
		{
			//	Change la police.
			if (this.array.SelectedItemIndex != -1)
			{
				int code = this.array.IndexToUnicode (this.array.SelectedItemIndex);
				if ( code != 0 )
				{
					this.arrayProofCode = code;
				}
			}

			this.ignoreChanged = true;

			this.fontFace = fontFace;
			this.fieldFontFace.Text = TextLayout.ConvertToTaggedText(this.fontFace);

			this.UpdateFontStyle();
			this.fontStyle = Misc.DefaultFontStyle(this.fontFace);
			this.fieldFontStyle.Text = this.fontStyle;

			this.array.SetFont(this.fontFace, this.fontStyle);

			this.array.SelectedItemIndex = this.array.UnicodeToIndex (this.arrayProofCode);
			this.array.ShowSelectedCell();

			this.ignoreChanged = false;
		}

		protected void SetFontStyle(string fontStyle)
		{
			//	Change le style de la police.
			if (this.array.SelectedItemIndex != -1)
			{
				int code = this.array.IndexToUnicode (this.array.SelectedItemIndex);
				if ( code != 0 )
				{
					this.arrayProofCode = code;
				}
			}

			this.fontStyle = fontStyle;
			this.fieldFontStyle.Text = this.fontStyle;

			this.array.SetFont(this.fontFace, this.fontStyle);

			this.array.SelectedItemIndex = this.array.UnicodeToIndex (this.arrayProofCode);
			this.array.ShowSelectedCell();
		}

		private void HandleBookActivePageChanged(object sender, CancelEventArgs e)
		{
			//	L'onglet actif a changé.
			this.UpdateAlternates();
		}

		private void HandleFontFaceComboOpening(object sender, CancelEventArgs e)
		{
			//	Le combo pour les polices va être ouvert.
			bool quickOnly = false;
			System.Collections.ArrayList quickFonts = new System.Collections.ArrayList();
			double height = 30;
			bool abc = false;
			if ( this.editor.HasCurrentDocument )
			{
				quickOnly  = this.editor.CurrentDocument.Modifier.ActiveViewer.DrawingContext.TextFontFilter;
				quickFonts = this.editor.CurrentDocument.Settings.QuickFonts;
				height     = this.editor.CurrentDocument.Modifier.ActiveViewer.DrawingContext.TextFontSampleHeight;
				abc        = this.editor.CurrentDocument.Modifier.ActiveViewer.DrawingContext.TextFontSampleAbc;
			}
			int quickCount;
			System.Collections.ArrayList fontList = Misc.MergeFontList(Misc.GetFontList(true), quickFonts, quickOnly, this.fontFace, out quickCount);

			this.fieldFontFace.FontList     = fontList;
			this.fieldFontFace.QuickCount   = quickCount;
			this.fieldFontFace.SampleHeight = height;
			this.fieldFontFace.SampleAbc    = abc;
		}

		private void HandleFontFaceChanged(object sender)
		{
			//	Police changée.
			if ( this.ignoreChanged )  return;
			this.SetFontFace(TextLayout.ConvertToSimpleText(this.fieldFontFace.Text));
		}

		private void HandleCurrentFontClicked(object sender, MessageEventArgs e)
		{
			//	Bouton "<-" cliqué.
			if ( !this.editor.HasCurrentDocument )  return;

			string fontFace, fontStyle;
			this.editor.CurrentDocument.Modifier.EditGetFont(out fontFace, out fontStyle);
			if ( fontFace == "" )  return;
			this.SetFontFace(fontFace);
			this.SetFontStyle(fontStyle);
		}

		private void HandleFontStyleChanged(object sender)
		{
			//	Style de la police changé.
			if ( this.ignoreChanged )  return;
			this.SetFontStyle(this.fieldFontStyle.Text);
		}

		private void HandleFamilyChanged(object sender)
		{
			//	Famille changée.
			this.UpdateList();
		}

		private void HandleGlyphSelected(object sender)
		{
			//	Le glyphe dans la liste est sélectionné.
			if ( this.ignoreChanged )  return;

			int family = this.family.SelectedItemIndex;
			this.listSelectedIndex[family] = this.list.SelectedItemIndex;
		}

		private void HandleArraySelected(object sender)
		{
			//	Le glyphe dans le tableau est sélectionné.
			string text = "";
			if (this.Array.SelectedItemIndex != -1)
			{
				if ( this.book.ActivePage.Name == "Array" )
				{
					int code = this.array.IndexToUnicode (this.Array.SelectedItemIndex);
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

		private void HandleDoubleClicked(object sender, MessageEventArgs e)
		{
			//	Le glyphe est double-cliqué.
			this.EditInsert();
		}

		private void HandleSliderChanged(object sender)
		{
			HSlider slider = sender as HSlider;
			if ( slider == null )  return;
			this.Array.CellSize = (double) slider.Value;
		}

		private void HandleWindowCloseClicked(object sender)
		{
			this.CloseWindow();
		}

		private void HandleButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.CloseWindow();
		}

		private void HandleButtonInsertClicked(object sender, MessageEventArgs e)
		{
			this.EditInsert();
		}


		protected Widgets.GlyphArray Array
		{
			//	Donne le tableau en fonction de l'onglet actif.
			get
			{
				if ( this.book.ActivePage.Name == "Array"      )  return this.array;
				if ( this.book.ActivePage.Name == "Alternates" )  return this.alternatesArray;
				return null;
			}
		}

		protected TextField Status
		{
			//	Donne le statuts en fonction de l'onglet actif.
			get
			{
				if ( this.book.ActivePage.Name == "Array"      )  return this.status;
				if ( this.book.ActivePage.Name == "Alternates" )  return this.alternatesStatus;
				return null;
			}
		}


		protected string					fontFace;
		protected string					fontStyle;
		protected readonly int				maxFamiliy = 10;
		protected int[]						listSelectedIndex;
		protected int						arrayProofCode = -1;
		protected bool						ignoreChanged = false;

		protected TabBook					book;

		protected TextFieldCombo			family;
		protected ScrollList				list;

		protected FontFaceCombo				fieldFontFace;
		protected IconButton				buttonFilter;
		protected TextFieldCombo			fieldFontStyle;
		protected GlyphButton				currentFont;
		protected Widgets.GlyphArray		array;
		protected TextField					status;
		protected HSlider					slider;

		protected Widgets.GlyphArray		alternatesArray;
		protected TextField					alternatesStatus;
		protected HSlider					alternatesSlider;
		protected bool						alternatesDirty = true;
	}
}
