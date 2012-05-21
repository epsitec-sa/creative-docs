using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;

namespace Epsitec.Common.DocumentEditor.Dialogs
{
	using GlobalSettings = Common.Document.Settings.GlobalSettings;

	/// <summary>
	/// Dialogue général d'exportation.
	/// </summary>
	public class Export : Abstract
	{
		public Export(DocumentEditor editor) : base(editor)
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
				this.WindowInit("Export", 300, 246);
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
				bookGeneric.TabTitle = Res.Strings.Dialog.Export.TabPage.Generic;
				bookDoc.Items.Add(bookGeneric);

				TabPage bookColor = new TabPage();
				bookColor.Name = "Format";
				bookColor.TabTitle = Res.Strings.Dialog.Export.TabPage.Format;
				bookDoc.Items.Add(bookColor);

				TabPage bookImage = new TabPage();
				bookImage.Name = "Filter";
				bookImage.TabTitle = Res.Strings.Dialog.Export.TabPage.Filter;
				bookDoc.Items.Add(bookImage);

				bookDoc.ActivePage = bookGeneric;

				//	Boutons de fermeture.
				Button buttonOk = new Button(this.window.Root);
				buttonOk.PreferredWidth = 75;
				buttonOk.Text = Res.Strings.Dialog.Export.Button.OK;
				buttonOk.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonOk.Anchor = AnchorStyles.BottomRight;
				buttonOk.Margins = new Margins(0, 6+75+6, 0, 6);
				buttonOk.Clicked += this.HandleExportButtonOkClicked;
				buttonOk.TabIndex = 10;
				buttonOk.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(buttonOk, Res.Strings.Dialog.Export.Tooltip.OK);

				Button buttonCancel = new Button(this.window.Root);
				buttonCancel.PreferredWidth = 75;
				buttonCancel.Text = Res.Strings.Dialog.Export.Button.Cancel;
				buttonCancel.ButtonStyle = ButtonStyle.DefaultCancel;
				buttonCancel.Anchor = AnchorStyles.BottomRight;
				buttonCancel.Margins = new Margins(0, 6, 0, 6);
				buttonCancel.Clicked += this.HandleExportButtonCancelClicked;
				buttonCancel.TabIndex = 11;
				buttonCancel.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(buttonCancel, Res.Strings.Dialog.Export.Tooltip.Cancel);
			}

			if ( this.editor.HasCurrentDocument )
			{
				this.editor.CurrentDocument.Dialogs.BuildExport(this.window);
			}

			this.window.Text = string.Format(Res.Strings.Dialog.Export.Title2, System.IO.Path.GetFileName(filename));
			this.window.ShowDialog();
		}

		public override void Save()
		{
			//	Enregistre la position de la fenêtre du dialogue.
			this.WindowSave("Export");
		}

		public override void Rebuild()
		{
			//	Reconstruit le dialogue.
			if ( !this.editor.HasCurrentDocument )  return;
			if ( this.window == null )  return;
			this.editor.CurrentDocument.Dialogs.BuildExport(this.window);
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

			string filename = System.IO.Path.Combine(this.editor.CurrentDocument.ExportDirectory, this.editor.CurrentDocument.ExportFilename);
			string err = this.editor.CurrentDocument.Export(filename);
			this.editor.DialogError(err);
		}

	}
}
