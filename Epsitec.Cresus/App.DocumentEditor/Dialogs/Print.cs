using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;

namespace Epsitec.App.DocumentEditor.Dialogs
{
	using GlobalSettings = Common.Document.Settings.GlobalSettings;

	/// <summary>
	/// Dialogue d'impression du document.
	/// </summary>
	public class Print : Abstract
	{
		public Print(DocumentEditor editor) : base(editor)
		{
		}

		// Cr�e et montre la fen�tre du dialogue.
		public override void Show()
		{
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.MakeFixedSizeWindow();
				this.window.MakeSecondaryWindow();
				this.WindowInit("Print", 300, 350);
				this.window.Text = "Imprimer...";
				this.window.PreventAutoClose = true;
				this.window.Owner = this.editor.Window;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowPrintCloseClicked);

				// Cr�e les onglets.
				TabBook bookDoc = new TabBook(this.window.Root);
				bookDoc.Name = "Book";
				bookDoc.Arrows = TabBookArrows.Stretch;
				bookDoc.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.TopAndBottom;
				bookDoc.AnchorMargins = new Margins(6, 6, 6, 34);

				TabPage bookFormat = new TabPage();
				bookFormat.Name = "Printer";
				bookFormat.TabTitle = "Imprimante";
				bookDoc.Items.Add(bookFormat);

				TabPage bookGrid = new TabPage();
				bookGrid.Name = "Param";
				bookGrid.TabTitle = "Param�tres";
				bookDoc.Items.Add(bookGrid);

				TabPage bookGuides = new TabPage();
				bookGuides.Name = "Publisher";
				bookGuides.TabTitle = "Pr�-presse";
				bookDoc.Items.Add(bookGuides);

				bookDoc.ActivePage = bookFormat;

				// Boutons de fermeture.
				Button buttonOk = new Button(this.window.Root);
				buttonOk.Width = 75;
				buttonOk.Text = "Imprimer";
				buttonOk.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonOk.Anchor = AnchorStyles.BottomLeft;
				buttonOk.AnchorMargins = new Margins(6, 0, 0, 6);
				buttonOk.Clicked += new MessageEventHandler(this.HandlePrintButtonOkClicked);
				buttonOk.TabIndex = 1000;
				buttonOk.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(buttonOk, "Imprimer le document");

				Button buttonCancel = new Button(this.window.Root);
				buttonCancel.Width = 75;
				buttonCancel.Text = "Fermer";
				buttonCancel.Anchor = AnchorStyles.BottomLeft;
				buttonCancel.AnchorMargins = new Margins(6+75+10, 0, 0, 6);
				buttonCancel.Clicked += new MessageEventHandler(this.HandlePrintButtonCancelClicked);
				buttonCancel.TabIndex = 1001;
				buttonCancel.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(buttonCancel, "Fermer sans imprimmer");
			}

			if ( this.editor.IsCurrentDocument )
			{
				this.editor.CurrentDocument.Dialogs.BuildPrint(this.window);
			}

			this.window.ShowDialog();
		}

		// Enregistre la position de la fen�tre du dialogue.
		public override void Save()
		{
			this.WindowSave("Print");
		}

		// Reconstruit le dialogue.
		public override void Rebuild()
		{
			if ( !this.editor.IsCurrentDocument )  return;
			if ( this.window == null )  return;
			this.editor.CurrentDocument.Dialogs.BuildPrint(this.window);
		}

		// Met � jour le dialogue lorsque les pages ont chang�.
		public void UpdatePages()
		{
			this.editor.CurrentDocument.Dialogs.UpdatePrintPages();
		}


		private void HandleWindowPrintCloseClicked(object sender)
		{
			this.editor.Window.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandlePrintButtonCancelClicked(object sender, MessageEventArgs e)
		{
			this.editor.Window.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandlePrintButtonOkClicked(object sender, MessageEventArgs e)
		{
			this.editor.Window.MakeActive();
			this.window.Hide();
			this.OnClosed();

			Common.Dialogs.Print dialog = this.editor.CurrentDocument.PrintDialog;
			this.editor.CurrentDocument.Print(dialog);
		}
	}
}
