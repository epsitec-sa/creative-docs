using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;

namespace Epsitec.App.DocumentEditor.Dialogs
{
	using GlobalSettings = Common.Document.Settings.GlobalSettings;

	/// <summary>
	/// Dialogue g�n�ral d'exportation.
	/// </summary>
	public class Export : Abstract
	{
		public Export(DocumentEditor editor) : base(editor)
		{
		}

		public void Show(string filename)
		{
			//	Cr�e et montre la fen�tre du dialogue.
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.MakeFixedSizeWindow();
				this.window.MakeSecondaryWindow();
				this.WindowInit("Export", 300, 340);
				this.window.PreventAutoClose = true;
				this.window.Owner = this.editor.Window;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowExportCloseClicked);

				Panel panel = new Panel(this.window.Root);
				panel.Name = "Panel";
				panel.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.TopAndBottom;
				panel.Margins = new Margins(10, 10, 10, 40);

				//	Boutons de fermeture.
				Button buttonOk = new Button(this.window.Root);
				buttonOk.PreferredWidth = 75;
				buttonOk.Text = Res.Strings.Dialog.Export.Button.OK;
				buttonOk.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonOk.Anchor = AnchorStyles.BottomLeft;
				buttonOk.Margins = new Margins(10, 0, 0, 10);
				buttonOk.Clicked += new MessageEventHandler(this.HandleExportButtonOkClicked);
				buttonOk.TabIndex = 10;
				buttonOk.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(buttonOk, Res.Strings.Dialog.Export.Tooltip.OK);

				Button buttonCancel = new Button(this.window.Root);
				buttonCancel.PreferredWidth = 75;
				buttonCancel.Text = Res.Strings.Dialog.Export.Button.Cancel;
				buttonCancel.ButtonStyle = ButtonStyle.DefaultCancel;
				buttonCancel.Anchor = AnchorStyles.BottomLeft;
				buttonCancel.Margins = new Margins(10+75+10, 0, 0, 10);
				buttonCancel.Clicked += new MessageEventHandler(this.HandleExportButtonCancelClicked);
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
			//	Enregistre la position de la fen�tre du dialogue.
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

			string filename = System.IO.Path.Combine (this.editor.CurrentDocument.ExportDirectory, this.editor.CurrentDocument.ExportFilename);
			string err = this.editor.CurrentDocument.Export (filename);
			this.editor.DialogError(err);
		}

	}
}
