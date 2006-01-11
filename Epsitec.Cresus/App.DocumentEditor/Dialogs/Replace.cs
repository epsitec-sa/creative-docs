using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;

namespace Epsitec.App.DocumentEditor.Dialogs
{
	using Objects        = Common.Document.Objects;
	using GlobalSettings = Common.Document.Settings.GlobalSettings;

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
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);

				this.tabIndex = 0;

				StaticText labelFind = new StaticText(this.window.Root);
				labelFind.Text = Res.Strings.Dialog.Replace.Label.Find;
				labelFind.Alignment = ContentAlignment.MiddleLeft;
				labelFind.Width = 80;
				labelFind.Anchor = AnchorStyles.TopLeft;
				labelFind.AnchorMargins = new Margins(10, 0, 10+3, 0);

				this.fieldFind = new TextFieldCombo(this.window.Root);
				this.fieldFind.Width = 400-100;
				this.fieldFind.Anchor = AnchorStyles.TopLeft;
				this.fieldFind.AnchorMargins = new Margins(90, 0, 10, 0);
				this.fieldFind.TabIndex = this.tabIndex++;
				this.fieldFind.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				StaticText labelReplace = new StaticText(this.window.Root);
				labelReplace.Text = Res.Strings.Dialog.Replace.Label.Replace;
				labelReplace.Alignment = ContentAlignment.MiddleLeft;
				labelReplace.Width = 80;
				labelReplace.Anchor = AnchorStyles.TopLeft;
				labelReplace.AnchorMargins = new Margins(10, 0, 40+3, 0);

				this.fieldReplace = new TextFieldCombo(this.window.Root);
				this.fieldReplace.Width = 400-100;
				this.fieldReplace.Anchor = AnchorStyles.TopLeft;
				this.fieldReplace.AnchorMargins = new Margins(90, 0, 40, 0);
				this.fieldReplace.TabIndex = this.tabIndex++;
				this.fieldReplace.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				this.checkEqualMaj = new CheckButton(this.window.Root);
				this.checkEqualMaj.Text = Res.Strings.Dialog.Replace.Button.EqualMaj;
				this.checkEqualMaj.Width = 200;
				this.checkEqualMaj.Anchor = AnchorStyles.TopLeft;
				this.checkEqualMaj.AnchorMargins = new Margins(10, 0, 72+18*0, 0);
				this.checkEqualMaj.TabIndex = this.tabIndex++;
				this.checkEqualMaj.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				this.checkEqualAccent = new CheckButton(this.window.Root);
				this.checkEqualAccent.Text = Res.Strings.Dialog.Replace.Button.EqualAccent;
				this.checkEqualAccent.Width = 200;
				this.checkEqualAccent.Anchor = AnchorStyles.TopLeft;
				this.checkEqualAccent.AnchorMargins = new Margins(10, 0, 72+18*1, 0);
				this.checkEqualAccent.TabIndex = this.tabIndex++;
				this.checkEqualAccent.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				this.checkWholeWord = new CheckButton(this.window.Root);
				this.checkWholeWord.Text = Res.Strings.Dialog.Replace.Button.WholeWord;
				this.checkWholeWord.Width = 200;
				this.checkWholeWord.Anchor = AnchorStyles.TopLeft;
				this.checkWholeWord.AnchorMargins = new Margins(10, 0, 72+18*2, 0);
				this.checkWholeWord.TabIndex = this.tabIndex++;
				this.checkWholeWord.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				//	Bouton Chercher.
				this.buttonFind = new Button(this.window.Root);
				this.buttonFind.Width = 75;
				this.buttonFind.Text = Res.Strings.Dialog.Replace.Button.Find;
				this.buttonFind.ButtonStyle = ButtonStyle.DefaultAccept;
				this.buttonFind.Anchor = AnchorStyles.BottomLeft;
				this.buttonFind.AnchorMargins = new Margins(10, 0, 0, 10);
				this.buttonFind.Clicked += new MessageEventHandler(this.HandleButtonFindClicked);
				this.buttonFind.TabIndex = this.tabIndex++;
				this.buttonFind.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				//	Bouton Replacer.
				this.buttonReplace = new Button(this.window.Root);
				//?this.buttonReplace.Command = "GlyphsInsert";
				this.buttonReplace.Width = 75;
				this.buttonReplace.Text = Res.Strings.Dialog.Replace.Button.Replace;
				this.buttonReplace.Anchor = AnchorStyles.BottomLeft;
				this.buttonReplace.AnchorMargins = new Margins(10+75+10, 0, 0, 10);
				this.buttonReplace.Clicked += new MessageEventHandler(this.HandleButtonReplaceClicked);
				this.buttonReplace.TabIndex = this.tabIndex++;
				this.buttonReplace.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				//	Bouton annuler.
				this.buttonClose = new Button(this.window.Root);
				this.buttonClose.Width = 75;
				this.buttonClose.Text = Res.Strings.Dialog.Button.Close;
				this.buttonClose.ButtonStyle = ButtonStyle.DefaultCancel;
				this.buttonClose.Anchor = AnchorStyles.BottomLeft;
				this.buttonClose.AnchorMargins = new Margins(10+75+10+75+10, 0, 0, 10);
				this.buttonClose.Clicked += new MessageEventHandler(this.HandleButtonCloseClicked);
				this.buttonClose.TabIndex = this.tabIndex++;
				this.buttonClose.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			}

			this.window.Show();

			this.fieldFind.SelectAll();
			this.fieldFind.Focus();
		}

		public override void Save()
		{
			//	Enregistre la position de la fenêtre du dialogue.
			this.WindowSave("Replace");
		}


		protected void ComboMemorise(TextFieldCombo combo)
		{
			//	Gère la liste d'un combo pour conserver les derniers textes tapés.
			string text = combo.Text;
			if ( text == "" )  return;

			if ( combo.Items.Contains(text) )  // déjà dans la liste ?
			{
				combo.Items.Remove(text);
			}

			combo.Items.Insert(0, text);  // insère au début de la liste
		}

		protected bool Find(bool skipFirst, string find, string replace, bool ignoreMaj, bool ignoreAccent, bool wholeWord)
		{
			if ( find == "" )
			{
				return false;
			}
			
			Document document = this.editor.CurrentDocument;
			if ( document == null )
			{
				this.editor.DialogError(this.editor.CommandDispatcher, Res.Strings.Dialog.Replace.Error.NoDocument);
				return false;
			}

			if ( document.TextFlows.Count == 0 )
			{
				this.editor.DialogError(this.editor.CommandDispatcher, Res.Strings.Dialog.Replace.Error.NoText);
				return false;
			}
			
			if ( replace == null )
			{
				document.Modifier.OpletQueueBeginAction(string.Format(Res.Strings.Dialog.Replace.Action.Find, Misc.Resume(find)));
			}
			else
			{
				document.Modifier.OpletQueueBeginAction(string.Format(Res.Strings.Dialog.Replace.Action.Replace, Misc.Resume(find), Misc.Resume(replace)));
			}

			TextFlow textFlow = null;
			Objects.AbstractText edit = document.Modifier.RetEditObject();
			if ( edit == null )  // aucun objet en édition ?
			{
				textFlow = document.TextFlows[0] as TextFlow;
				textFlow.MetaNavigator.ClearSelection();
				textFlow.TextNavigator.MoveTo(0, 1);  // démarre la recherche au début du premier TextFlow
				skipFirst = false;
			}
			else	// il existe un objet en édition ?
			{
				textFlow = edit.TextFlow;  // démarre la recherche dans l'objet édité
			}

			if ( !TextFlow.FindText(document, ref textFlow, find, skipFirst, ignoreMaj, ignoreAccent, wholeWord) )
			{
				document.Modifier.OpletQueueValidateAction();
				this.editor.DialogError(this.editor.CommandDispatcher, Res.Strings.Dialog.Replace.Error.NotFound);
				return false;
			}

			edit = TextFlow.FindObject(document, textFlow);
			System.Diagnostics.Debug.Assert(edit != null);
			document.Modifier.SetEditObject(edit);  // édite l'objet trouvé

			if ( replace != null )
			{
				textFlow.TextNavigator.Delete();  // supprime la chaîne cherchée
				textFlow.TextNavigator.Insert(replace);  // insère la chaîne de remplacement

				int pos = textFlow.TextNavigator.CursorPosition;
				textFlow.TextNavigator.MoveTo(pos-replace.Length, 1);
				textFlow.TextNavigator.StartSelection();  // sélectionne la chaîne de remplacement
				textFlow.TextNavigator.MoveTo(pos, 1);
				textFlow.TextNavigator.EndSelection();
			}

			document.Modifier.OpletQueueValidateAction();
			return true;
		}


		private void HandleWindowCloseClicked(object sender)
		{
			this.editor.Window.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandleButtonFindClicked(object sender, MessageEventArgs e)
		{
			this.ComboMemorise(this.fieldFind);
			this.ComboMemorise(this.fieldReplace);

			bool ignoreMaj    = (this.checkEqualMaj   .ActiveState == Common.Widgets.ActiveState.No);
			bool ignoreAccent = (this.checkEqualAccent.ActiveState == Common.Widgets.ActiveState.No);
			bool wholeWord    = (this.checkWholeWord  .ActiveState == Common.Widgets.ActiveState.Yes);
			this.Find(true, this.fieldFind.Text, null, ignoreMaj, ignoreAccent, wholeWord);
		}

		private void HandleButtonReplaceClicked(object sender, MessageEventArgs e)
		{
			this.ComboMemorise(this.fieldFind);
			this.ComboMemorise(this.fieldReplace);

			bool ignoreMaj    = (this.checkEqualMaj   .ActiveState == Common.Widgets.ActiveState.No);
			bool ignoreAccent = (this.checkEqualAccent.ActiveState == Common.Widgets.ActiveState.No);
			bool wholeWord    = (this.checkWholeWord  .ActiveState == Common.Widgets.ActiveState.Yes);
			this.Find(false, this.fieldFind.Text, this.fieldReplace.Text, ignoreMaj, ignoreAccent, wholeWord);
		}

		private void HandleButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.editor.Window.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}


		protected int					tabIndex;
		protected TextFieldCombo		fieldFind;
		protected TextFieldCombo		fieldReplace;
		protected CheckButton			checkEqualMaj;
		protected CheckButton			checkEqualAccent;
		protected CheckButton			checkWholeWord;
		protected Button				buttonFind;
		protected Button				buttonReplace;
		protected Button				buttonClose;
	}
}
