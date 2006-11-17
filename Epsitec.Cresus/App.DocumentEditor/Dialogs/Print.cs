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

		public override void Show()
		{
			//	Crée et montre la fenêtre du dialogue.
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.MakeFixedSizeWindow();
				this.window.MakeSecondaryWindow();
				this.WindowInit("Print", 300, 350);
				this.window.Text = Res.Strings.Dialog.Print.Title;
				this.window.PreventAutoClose = true;
				this.window.Owner = this.editor.Window;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowPrintCloseClicked);

				//	Crée les onglets.
				TabBook bookDoc = new TabBook(this.window.Root);
				bookDoc.Name = "Book";
				bookDoc.Arrows = TabBookArrows.Stretch;
				bookDoc.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.TopAndBottom;
				bookDoc.Margins = new Margins(6, 6, 6, 34);

				TabPage bookPrinter = new TabPage();
				bookPrinter.Name = "Printer";
				bookPrinter.TabTitle = Res.Strings.Dialog.Print.TabPage.Printer;
				bookDoc.Items.Add(bookPrinter);

				TabPage bookParam = new TabPage();
				bookParam.Name = "Param";
				bookParam.TabTitle = Res.Strings.Dialog.Print.TabPage.Param;
				bookDoc.Items.Add(bookParam);

				//	L'onglet 'Image' est provisoirement caché, puisque les choix pour les filtres
				//	des groupes A et B ne sont pas supportés par le port d'impression.
				TabPage bookImage = new TabPage();
				bookImage.Name = "Image";
				bookImage.TabTitle = Res.Strings.Dialog.Print.TabPage.Image;
				bookDoc.Items.Add(bookImage);

				TabPage bookPublisher = new TabPage();
				bookPublisher.Name = "Publisher";
				bookPublisher.TabTitle = Res.Strings.Dialog.Print.TabPage.Publisher;
				bookDoc.Items.Add(bookPublisher);

				bookDoc.ActivePage = bookPrinter;

				//	Boutons de fermeture.
				Button buttonOk = new Button(this.window.Root);
				buttonOk.PreferredWidth = 75;
				buttonOk.Text = Res.Strings.Dialog.Print.Button.OK;
				buttonOk.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonOk.Anchor = AnchorStyles.BottomLeft;
				buttonOk.Margins = new Margins(6, 0, 0, 6);
				buttonOk.Clicked += new MessageEventHandler(this.HandlePrintButtonOkClicked);
				buttonOk.TabIndex = 1000;
				buttonOk.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(buttonOk, Res.Strings.Dialog.Print.Tooltip.OK);

				Button buttonCancel = new Button(this.window.Root);
				buttonCancel.PreferredWidth = 75;
				buttonCancel.Text = Res.Strings.Dialog.Print.Button.Cancel;
				buttonCancel.ButtonStyle = ButtonStyle.DefaultCancel;
				buttonCancel.Anchor = AnchorStyles.BottomLeft;
				buttonCancel.Margins = new Margins(6+75+10, 0, 0, 6);
				buttonCancel.Clicked += new MessageEventHandler(this.HandlePrintButtonCancelClicked);
				buttonCancel.TabIndex = 1001;
				buttonCancel.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(buttonCancel, Res.Strings.Dialog.Print.Tooltip.Cancel);
			}

			if ( this.editor.IsCurrentDocument )
			{
				this.editor.CurrentDocument.Dialogs.BuildPrint(this.window);
			}

			this.window.ShowDialog();
		}

		public override void Save()
		{
			//	Enregistre la position de la fenêtre du dialogue.
			this.WindowSave("Print");
		}

		public override void Rebuild()
		{
			//	Reconstruit le dialogue.
			if ( !this.editor.IsCurrentDocument )  return;
			if ( this.window == null )  return;
			this.editor.CurrentDocument.Dialogs.BuildPrint(this.window);
		}

		public void UpdatePages()
		{
			//	Met à jour le dialogue lorsque les pages ont changé.
			this.editor.CurrentDocument.Dialogs.UpdatePrintPages();
		}


		private void HandleWindowPrintCloseClicked(object sender)
		{
			this.CloseWindow();
		}

		private void HandlePrintButtonCancelClicked(object sender, MessageEventArgs e)
		{
			this.CloseWindow();
		}

		private void HandlePrintButtonOkClicked(object sender, MessageEventArgs e)
		{
			this.CloseWindow();

			Common.Dialogs.Print dialog = this.editor.CurrentDocument.PrintDialog;
			this.editor.CurrentDocument.Print(dialog);
		}
	}
}
