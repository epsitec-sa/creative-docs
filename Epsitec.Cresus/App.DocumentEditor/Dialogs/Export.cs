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
	public class Export : Abstract
	{
		public Export(DocumentEditor editor) : base(editor)
		{
		}

		// Cr�e et montre la fen�tre du dialogue.
		public void Show(string filename)
		{
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.MakeFixedSizeWindow();
				this.window.MakeSecondaryWindow();
				this.WindowInit("Export", 300, 300);
				this.window.PreventAutoClose = true;
				this.window.Owner = this.editor.Window;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowExportCloseClicked);

				Panel panel = new Panel(this.window.Root);
				panel.Name = "Panel";
				panel.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.TopAndBottom;
				panel.AnchorMargins = new Margins(10, 10, 10, 40);

				// Boutons de fermeture.
				Button buttonOk = new Button(this.window.Root);
				buttonOk.Width = 75;
				buttonOk.Text = Res.Strings.Dialog.Export.Button.OK;
				buttonOk.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonOk.Anchor = AnchorStyles.BottomLeft;
				buttonOk.AnchorMargins = new Margins(10, 0, 0, 10);
				buttonOk.Clicked += new MessageEventHandler(this.HandleExportButtonOkClicked);
				buttonOk.TabIndex = 10;
				buttonOk.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(buttonOk, Res.Strings.Dialog.Export.Tooltip.OK);

				Button buttonCancel = new Button(this.window.Root);
				buttonCancel.Width = 75;
				buttonCancel.Text = Res.Strings.Dialog.Export.Button.Cancel;
				buttonCancel.Anchor = AnchorStyles.BottomLeft;
				buttonCancel.AnchorMargins = new Margins(10+75+10, 0, 0, 10);
				buttonCancel.Clicked += new MessageEventHandler(this.HandleExportButtonCancelClicked);
				buttonCancel.TabIndex = 11;
				buttonCancel.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(buttonCancel, Res.Strings.Dialog.Export.Tooltip.Cancel);
			}

			if ( this.editor.IsCurrentDocument )
			{
				this.editor.CurrentDocument.Dialogs.BuildExport(this.window);
			}

			this.window.Text = string.Format(Res.Strings.Dialog.Export.Title, System.IO.Path.GetFileName(filename));
			this.window.Show();
		}

		// Enregistre la position de la fen�tre du dialogue.
		public override void Save()
		{
			this.WindowSave("Export");
		}

		// Reconstruit le dialogue.
		public override void Rebuild()
		{
			if ( !this.editor.IsCurrentDocument )  return;
			if ( this.window == null )  return;
			this.editor.CurrentDocument.Dialogs.BuildExport(this.window);
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
			string err = this.editor.CurrentDocument.Export(filename);
			this.editor.DialogError(this.editor.CommandDispatcher, err);
		}

	}
}
