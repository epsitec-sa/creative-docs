using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;

namespace Epsitec.App.DocumentEditor.Dialogs
{
	using GlobalSettings = Common.Document.Settings.GlobalSettings;

	/// <summary>
	/// Dialogue des informations sur le document.
	/// </summary>
	public class ExportPDF : Abstract
	{
		public ExportPDF(DocumentEditor editor) : base(editor)
		{
		}

		public void Show(string filename)
		{
			//	Crée et montre la fenêtre du dialogue.
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.MakeFixedSizeWindow();
				this.window.MakeSecondaryWindow();
				this.WindowInit("ExportPDF", 300, 220);
				this.window.PreventAutoClose = true;
				this.window.Owner = this.editor.Window;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowExportCloseClicked);

				//	Crée les onglets.
				TabBook bookDoc = new TabBook(this.window.Root);
				bookDoc.Name = "Book";
				bookDoc.Arrows = TabBookArrows.Stretch;
				bookDoc.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.TopAndBottom;
				bookDoc.Margins = new Margins(6, 6, 6, 34);

				TabPage bookGeneric = new TabPage();
				bookGeneric.Name = "Generic";
				bookGeneric.TabTitle = Res.Strings.Dialog.ExportPDF.TabPage.Generic;
				bookDoc.Items.Add(bookGeneric);

				TabPage bookColor = new TabPage();
				bookColor.Name = "Color";
				bookColor.TabTitle = Res.Strings.Dialog.ExportPDF.TabPage.Color;
				bookDoc.Items.Add(bookColor);

				TabPage bookImage = new TabPage();
				bookImage.Name = "Image";
				bookImage.TabTitle = Res.Strings.Dialog.ExportPDF.TabPage.Image;
				bookDoc.Items.Add(bookImage);

				TabPage bookPublisher = new TabPage();
				bookPublisher.Name = "Publisher";
				bookPublisher.TabTitle = Res.Strings.Dialog.ExportPDF.TabPage.Publisher;
				bookDoc.Items.Add(bookPublisher);

				bookDoc.ActivePage = bookGeneric;

				//	Boutons de fermeture.
				Button buttonOk = new Button(this.window.Root);
				buttonOk.Width = 75;
				buttonOk.Text = Res.Strings.Dialog.ExportPDF.Button.OK;
				buttonOk.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonOk.Anchor = AnchorStyles.BottomLeft;
				buttonOk.Margins = new Margins(6, 0, 0, 6);
				buttonOk.Clicked += new MessageEventHandler(this.HandleExportButtonOkClicked);
				buttonOk.TabIndex = 10;
				buttonOk.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(buttonOk, Res.Strings.Dialog.ExportPDF.Tooltip.OK);

				Button buttonCancel = new Button(this.window.Root);
				buttonCancel.Width = 75;
				buttonCancel.Text = Res.Strings.Dialog.ExportPDF.Button.Cancel;
				buttonCancel.Anchor = AnchorStyles.BottomLeft;
				buttonCancel.Margins = new Margins(6+75+10, 0, 0, 6);
				buttonCancel.Clicked += new MessageEventHandler(this.HandleExportButtonCancelClicked);
				buttonCancel.TabIndex = 11;
				buttonCancel.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(buttonCancel, Res.Strings.Dialog.ExportPDF.Tooltip.Cancel);
			}

			if ( this.editor.IsCurrentDocument )
			{
				this.editor.CurrentDocument.Dialogs.BuildExportPDF(this.window);
			}

			this.window.Text = string.Format(Res.Strings.Dialog.ExportPDF.Title2, System.IO.Path.GetFileName(filename));
			this.window.ShowDialog();
		}

		public override void Save()
		{
			//	Enregistre la position de la fenêtre du dialogue.
			this.WindowSave("ExportPDF");
		}

		public override void Rebuild()
		{
			//	Reconstruit le dialogue.
			if ( !this.editor.IsCurrentDocument )  return;
			if ( this.window == null )  return;
			this.editor.CurrentDocument.Dialogs.BuildExportPDF(this.window);
		}

		public void UpdatePages()
		{
			//	Met à jour le dialogue lorsque les pages ont changé.
			this.editor.CurrentDocument.Dialogs.UpdateExportPDFPages();
		}


		private void HandleWindowExportCloseClicked(object sender)
		{
			this.editor.Window.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandleExportButtonCancelClicked(object sender, MessageEventArgs e)
		{
			this.editor.Window.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandleExportButtonOkClicked(object sender, MessageEventArgs e)
		{
			this.editor.Window.MakeActive();
			this.window.Hide();
			this.OnClosed();

			string filename = string.Format("{0}\\{1}", this.editor.CurrentDocument.ExportDirectory, this.editor.CurrentDocument.ExportFilename);
			string err = this.editor.CurrentDocument.ExportPDF(filename);
			this.editor.DialogError(this.editor.CommandDispatcher, err);
		}

	}
}
