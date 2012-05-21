using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;

namespace Epsitec.Common.DocumentEditor.Dialogs
{
	using GlobalSettings = Common.Document.Settings.GlobalSettings;

	/// <summary>
    /// Dialogue d'exportation d'un fichier PDF.
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
				this.WindowInit("ExportPDF", 300, 440);
				this.window.PreventAutoClose = true;
				this.window.Owner = this.editor.Window;
				this.window.WindowCloseClicked += this.HandleWindowExportCloseClicked;

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
				buttonOk.PreferredWidth = 75;
				buttonOk.Text = Res.Strings.Dialog.ExportPDF.Button.OK;
				buttonOk.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonOk.Anchor = AnchorStyles.BottomRight;
				buttonOk.Margins = new Margins(0, 6+75+6, 0, 6);
				buttonOk.Clicked += this.HandleExportButtonOkClicked;
				buttonOk.TabIndex = 10;
				buttonOk.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(buttonOk, Res.Strings.Dialog.ExportPDF.Tooltip.OK);

				Button buttonCancel = new Button(this.window.Root);
				buttonCancel.PreferredWidth = 75;
				buttonCancel.Text = Res.Strings.Dialog.ExportPDF.Button.Cancel;
				buttonCancel.ButtonStyle = ButtonStyle.DefaultCancel;
				buttonCancel.Anchor = AnchorStyles.BottomRight;
				buttonCancel.Margins = new Margins(0, 6, 0, 6);
				buttonCancel.Clicked += this.HandleExportButtonCancelClicked;
				buttonCancel.TabIndex = 11;
				buttonCancel.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(buttonCancel, Res.Strings.Dialog.ExportPDF.Tooltip.Cancel);
			}

			if ( this.editor.HasCurrentDocument )
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
			if ( !this.editor.HasCurrentDocument )  return;
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
			this.CloseWindow();
		}

		private void HandleExportButtonCancelClicked(object sender, MessageEventArgs e)
		{
			this.CloseWindow();
		}

		private void HandleExportButtonOkClicked(object sender, MessageEventArgs e)
		{
			this.CloseWindow();

			string filename = System.IO.Path.Combine (this.editor.CurrentDocument.ExportDirectory, this.editor.CurrentDocument.ExportFilename);
			string err = "";

			Common.Dialogs.WorkInProgressDialog.ExecuteCancellable
			(
				Res.Strings.Export.PDF.Progress.Title,
				ProgressIndicatorStyle.UnknownDuration,
				delegate(Common.Dialogs.IWorkInProgressReport report)
				{
					report.DefineOperation(Res.Strings.Export.PDF.Progress.Operation);
					err = this.editor.CurrentDocument.ExportPdf(filename, report);
				},
				this.editor.Window
			);
			
			this.editor.DialogError(err);
		}

	}
}
