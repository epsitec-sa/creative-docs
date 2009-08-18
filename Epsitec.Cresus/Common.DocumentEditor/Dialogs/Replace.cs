using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;

namespace Epsitec.Common.DocumentEditor.Dialogs
{
	using Widgets        = Common.Widgets;
	using Objects        = Common.Document.Objects;
	using GlobalSettings = Common.Document.Settings.GlobalSettings;
	using Document       = Common.Document.Document;

	/// <summary>
	/// Dialogue chercher/remplacer.
	/// </summary>
	public class Replace : Abstract
	{
		public Replace(DocumentEditor editor) : base(editor)
		{
		}

		public override void Show()
		{
			//	Crée et montre la fenêtre du dialogue.
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.MakeFixedSizeWindow();
				this.window.MakeSecondaryWindow();
				this.window.PreventAutoClose = true;
				this.WindowInit("Replace", 400, 170);
				this.window.Text = Res.Strings.Dialog.Replace.Title;
				this.window.PreventAutoClose = true;
				this.window.Owner = this.editor.Window;
				this.window.WindowCloseClicked += this.HandleWindowCloseClicked;

				this.tabIndex = 0;

				StaticText labelFind = new StaticText(this.window.Root);
				labelFind.Text = Res.Strings.Dialog.Replace.Label.Find;
				labelFind.ContentAlignment = ContentAlignment.MiddleLeft;
				labelFind.PreferredWidth = 80;
				labelFind.Anchor = AnchorStyles.TopLeft;
				labelFind.Margins = new Margins(6, 0, 6+3, 0);

				this.fieldFind = new TextFieldCombo(this.window.Root);
				this.fieldFind.PreferredWidth = 400-100;
				this.fieldFind.Anchor = AnchorStyles.TopLeft;
				this.fieldFind.Margins = new Margins(90, 0, 6, 0);
				this.fieldFind.TabIndex = this.tabIndex++;
				this.fieldFind.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				this.fieldFind.TextChanged += this.HandleWidgetChanged;

				StaticText labelReplace = new StaticText(this.window.Root);
				labelReplace.Text = Res.Strings.Dialog.Replace.Label.Replace;
				labelReplace.ContentAlignment = ContentAlignment.MiddleLeft;
				labelReplace.PreferredWidth = 80;
				labelReplace.Anchor = AnchorStyles.TopLeft;
				labelReplace.Margins = new Margins(6, 0, 40+3, 0);

				this.fieldReplace = new TextFieldCombo(this.window.Root);
				this.fieldReplace.PreferredWidth = 400-100;
				this.fieldReplace.Anchor = AnchorStyles.TopLeft;
				this.fieldReplace.Margins = new Margins(90, 0, 34, 0);
				this.fieldReplace.TabIndex = this.tabIndex++;
				this.fieldReplace.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				this.fieldReplace.TextChanged += this.HandleWidgetChanged;

				this.checkEqualMaj = new CheckButton(this.window.Root);
				this.checkEqualMaj.Text = Res.Strings.Dialog.Replace.Button.EqualMaj;
				this.checkEqualMaj.PreferredWidth = 150;
				this.checkEqualMaj.Anchor = AnchorStyles.TopLeft;
				this.checkEqualMaj.Margins = new Margins(6, 0, 72+18*0, 0);
				this.checkEqualMaj.TabIndex = this.tabIndex++;
				this.checkEqualMaj.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				this.checkEqualMaj.ActiveStateChanged += this.HandleWidgetChanged;

				this.checkEqualAccent = new CheckButton(this.window.Root);
				this.checkEqualAccent.Text = Res.Strings.Dialog.Replace.Button.EqualAccent;
				this.checkEqualAccent.PreferredWidth = 150;
				this.checkEqualAccent.Anchor = AnchorStyles.TopLeft;
				this.checkEqualAccent.Margins = new Margins(6, 0, 72+18*1, 0);
				this.checkEqualAccent.TabIndex = this.tabIndex++;
				this.checkEqualAccent.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				this.checkEqualAccent.ActiveStateChanged += this.HandleWidgetChanged;

				this.checkWholeWord = new CheckButton(this.window.Root);
				this.checkWholeWord.Text = Res.Strings.Dialog.Replace.Button.WholeWord;
				this.checkWholeWord.PreferredWidth = 150;
				this.checkWholeWord.Anchor = AnchorStyles.TopLeft;
				this.checkWholeWord.Margins = new Margins(6, 0, 72+18*2, 0);
				this.checkWholeWord.TabIndex = this.tabIndex++;
				this.checkWholeWord.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				this.checkWholeWord.ActiveStateChanged += this.HandleWidgetChanged;

				this.radioReverse = new RadioButton(this.window.Root, "Direction", 0);
				this.radioReverse.Text = Res.Strings.Dialog.Replace.Button.Reverse;
				this.radioReverse.PreferredWidth = 90;
				this.radioReverse.Anchor = AnchorStyles.TopLeft;
				this.radioReverse.Margins = new Margins(200, 0, 72+18*0, 0);
				this.radioReverse.TabIndex = this.tabIndex++;
				this.radioReverse.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				this.radioReverse.ActiveStateChanged += this.HandleWidgetChanged;

				this.radioNormal = new RadioButton(this.window.Root, "Direction", 1);
				this.radioNormal.Text = Res.Strings.Dialog.Replace.Button.Normal;
				this.radioNormal.PreferredWidth = 90;
				this.radioNormal.Anchor = AnchorStyles.TopLeft;
				this.radioNormal.Margins = new Margins(200+100, 0, 72+18*0, 0);
				this.radioNormal.TabIndex = this.tabIndex++;
				this.radioNormal.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				this.radioNormal.ActiveStateChanged += this.HandleWidgetChanged;

				//	Bouton Chercher.
				this.buttonFind = new Button(this.window.Root);
				this.buttonFind.PreferredWidth = 75;
				this.buttonFind.Text = Res.Strings.Dialog.Replace.Button.Find;
				this.buttonFind.ButtonStyle = ButtonStyle.DefaultAccept;
				this.buttonFind.Anchor = AnchorStyles.BottomRight;
				this.buttonFind.Margins = new Margins(0, 6+75+6+75+6, 0, 6);
				this.buttonFind.Clicked += this.HandleButtonClicked;
				this.buttonFind.TabIndex = this.tabIndex++;
				this.buttonFind.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				//	Bouton Replacer.
				this.buttonReplace = new Button(this.window.Root);
				//?this.buttonReplace.Command = "GlyphsInsert";
				this.buttonReplace.PreferredWidth = 75;
				this.buttonReplace.Text = Res.Strings.Dialog.Replace.Button.Replace;
				this.buttonReplace.Anchor = AnchorStyles.BottomRight;
				this.buttonReplace.Margins = new Margins(0, 6+75+6, 0, 6);
				this.buttonReplace.Clicked += this.HandleButtonClicked;
				this.buttonReplace.TabIndex = this.tabIndex++;
				this.buttonReplace.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				//	Bouton annuler.
				this.buttonClose = new Button(this.window.Root);
				this.buttonClose.PreferredWidth = 75;
				this.buttonClose.Text = Res.Strings.Dialog.Button.Close;
				this.buttonClose.ButtonStyle = ButtonStyle.DefaultCancel;
				this.buttonClose.Anchor = AnchorStyles.BottomRight;
				this.buttonClose.Margins = new Margins(0, 6, 0, 6);
				this.buttonClose.Clicked += this.HandleButtonCloseClicked;
				this.buttonClose.TabIndex = this.tabIndex++;
				this.buttonClose.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.UpdateWidgets();
			}

			this.window.Show();

			this.window.MakeFocused();
			this.fieldFind.SelectAll();
			this.fieldFind.Focus();
		}

		public override void Save()
		{
			//	Enregistre la position de la fenêtre du dialogue.
			this.WindowSave("Replace");
		}


		public string FindText
		{
			//	Texte cherché.
			get
			{
				return this.findText;
			}

			set
			{
				if ( this.findText != value )
				{
					this.findText = value;
					this.UpdateWidgets();
				}
			}
		}

		public string ReplaceText
		{
			//	Texte de remplacement.
			get
			{
				return this.replaceText;
			}

			set
			{
				if ( this.replaceText != value )
				{
					this.replaceText = value;
					this.UpdateWidgets();
				}
			}
		}

		public Misc.StringSearch Mode
		{
			//	Mode de recherche.
			get
			{
				return this.mode;
			}

			set
			{
				if ( this.mode != value )
				{
					this.mode = value;
					this.UpdateWidgets();
				}
			}
		}

		public void MemorizeTexts()
		{
			//	Mémorise les textes de recherche et de remplacement.
			if ( this.fieldFind == null )  return;

			this.ComboMemorize(this.fieldFind);
			this.ComboMemorize(this.fieldReplace);
		}


		protected void UpdateWidgets()
		{
			//	Met à jour les widgets en fonction des variables.
			if ( this.fieldFind == null )  return;

			this.ignoreChange = true;

			if ( this.fieldFind.Text != this.findText )
			{
				this.fieldFind.Text = this.findText;
			}

			if ( this.fieldReplace.Text != this.replaceText )
			{
				this.fieldReplace.Text = this.replaceText;
			}

			this.SetActiveState(this.checkEqualMaj,    (this.mode&Misc.StringSearch.IgnoreMaj   ) == 0);
			this.SetActiveState(this.checkEqualAccent, (this.mode&Misc.StringSearch.IgnoreAccent) == 0);
			this.SetActiveState(this.checkWholeWord,   (this.mode&Misc.StringSearch.WholeWord   ) != 0);
			this.SetActiveState(this.radioReverse,     (this.mode&Misc.StringSearch.EndToStart  ) != 0);
			this.SetActiveState(this.radioNormal,      (this.mode&Misc.StringSearch.EndToStart  ) == 0);
			
			this.ignoreChange = false;
		}

		protected void ReadWidgets()
		{
			//	Met à jour les variables en fonction des widgets.
			this.findText    = this.fieldFind.Text;
			this.replaceText = this.fieldReplace.Text;

			Misc.StringSearch mode = 0;
			if ( !this.GetActiveState(this.checkEqualMaj)    )  mode |= Misc.StringSearch.IgnoreMaj;
			if ( !this.GetActiveState(this.checkEqualAccent) )  mode |= Misc.StringSearch.IgnoreAccent;
			if (  this.GetActiveState(this.checkWholeWord)   )  mode |= Misc.StringSearch.WholeWord;
			if (  this.GetActiveState(this.radioReverse)     )  mode |= Misc.StringSearch.EndToStart;
			this.mode = mode;
		}


		protected bool GetActiveState(AbstractButton button)
		{
			return (button.ActiveState == Widgets.ActiveState.Yes);
		}

		protected void SetActiveState(AbstractButton button, bool active)
		{
			button.ActiveState = active ? Widgets.ActiveState.Yes : Widgets.ActiveState.No;
		}

		protected void ComboMemorize(TextFieldCombo combo)
		{
			//	Gère la liste d'un combo pour conserver les derniers textes tapés.
			string text = combo.Text;
			if ( text == "" )  return;

			if ( combo.Items.Contains(text) )  // déjà dans la liste ?
			{
				combo.Items.Remove(text);
			}

			combo.Items.Insert(0, text);  // insère au début de la liste

			if ( combo.Items.Count > 15 )  // liste trop longue ?
			{
				combo.Items.RemoveAt(combo.Items.Count-1);  // supprime le plus ancien
			}
		}

		protected bool Find(string find, string replace, Misc.StringSearch mode)
		{
			Document document = this.editor.CurrentDocument;
			if ( document == null )  return false;
			return document.Modifier.TextReplace(find, replace, mode);
		}


		private void HandleWidgetChanged(object sender)
		{
			if ( this.ignoreChange )  return;
			this.ReadWidgets();
		}

		private void HandleWindowCloseClicked(object sender)
		{
			this.CloseWindow();
		}

		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			this.MemorizeTexts();

			string replace = null;
			if ( sender == this.buttonReplace )
			{
				replace = TextLayout.ConvertToSimpleText(this.replaceText);  // conversion des &quot; en "
			}

			string find = TextLayout.ConvertToSimpleText(this.findText);  // conversion des &quot; en "
			if (this.Find(find, replace, this.mode))
			{
				this.editor.Window.MakeFocused();
				this.editor.Window.RestoreLogicalFocus();
			}
		}

		private void HandleButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.CloseWindow();
		}


		protected int					tabIndex;
		protected TextFieldCombo		fieldFind;
		protected TextFieldCombo		fieldReplace;
		protected CheckButton			checkEqualMaj;
		protected CheckButton			checkEqualAccent;
		protected CheckButton			checkWholeWord;
		protected RadioButton			radioReverse;
		protected RadioButton			radioNormal;
		protected Button				buttonFind;
		protected Button				buttonReplace;
		protected Button				buttonClose;
		protected bool					ignoreChange = false;

		protected string				findText = "";
		protected string				replaceText = "";
		protected Misc.StringSearch		mode = Misc.StringSearch.IgnoreMaj | Misc.StringSearch.IgnoreAccent;
	}
}
