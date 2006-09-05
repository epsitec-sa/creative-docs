using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;
using Epsitec.Common.IO;
using System.IO;

namespace Epsitec.App.DocumentEditor.Dialogs
{
	/// <summary>
	/// Dialogue pour ouvrir un document existant.
	/// </summary>
	public class FileOpen : AbstractFile
	{
		public FileOpen(DocumentEditor editor) : base(editor)
		{
			this.fileExtension = ".crdoc";
			this.isNavigationEnabled = true;
		}

		public override void Show()
		{
			//	Crée et montre la fenêtre du dialogue.
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.MakeSecondaryWindow();
				this.window.PreventAutoClose = true;
				this.WindowInit("FileOpen", 400, 400, true);
				this.window.Text = Res.Strings.Dialog.Open.TitleDoc;
				this.window.Owner = this.editor.Window;
				this.window.Icon = Bitmap.FromManifestResource("Epsitec.App.DocumentEditor.Images.Application.icon", this.GetType().Assembly);
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);
				this.window.Root.MinSize = new Size(300, 200);
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				this.CreateResizer();

				//	Chemin d'accès.
				Widget access = new Widget(this.window.Root);
				access.PreferredHeight = 20;
				access.Margins = new Margins(0, 0, 0, 8);
				access.Dock = DockStyle.Top;

				StaticText label = new StaticText(access);
				label.Text = Res.Strings.Dialog.Open.LabelPath;
				label.PreferredWidth = 80;
				label.Dock = DockStyle.Left;

				this.fieldPath = new TextFieldCombo(access);
				this.fieldPath.IsReadOnly = true;
				this.fieldPath.Dock = DockStyle.Fill;
				this.fieldPath.Margins = new Margins(0, 5, 0, 0);
				this.fieldPath.ComboOpening += new EventHandler<CancelEventArgs>(this.HandleFieldPathComboOpening);
				this.fieldPath.ComboClosed += new EventHandler(this.HandleFieldPathComboClosed);
				this.fieldPath.TextChanged += new EventHandler(this.HandleFieldPathTextChanged);

				this.buttonDelete = new IconButton(access);
				this.buttonDelete.IconName = Misc.Icon("FileDelete");
				this.buttonDelete.Dock = DockStyle.Right;
				this.buttonDelete.Clicked += new MessageEventHandler(this.HandleButtonDeleteClicked);
				ToolTip.Default.SetToolTip(this.buttonDelete, Res.Strings.Dialog.Open.Delete);

				this.buttonRename = new IconButton(access);
				this.buttonRename.IconName = Misc.Icon("FileRename");
				this.buttonRename.Dock = DockStyle.Right;
				this.buttonRename.Clicked += new MessageEventHandler(this.HandleButtonRenameClicked);
				ToolTip.Default.SetToolTip(this.buttonRename, Res.Strings.Dialog.Open.Rename);

				this.buttonNew = new IconButton(access);
				this.buttonNew.IconName = Misc.Icon("NewDirectory");
				this.buttonNew.Dock = DockStyle.Right;
				this.buttonNew.Clicked += new MessageEventHandler(this.HandleButtonNewClicked);
				ToolTip.Default.SetToolTip(this.buttonNew, Res.Strings.Dialog.Open.NewDirectory);

				this.buttonParent = new IconButton(access);
				this.buttonParent.IconName = Misc.Icon("ParentDirectory");
				this.buttonParent.Dock = DockStyle.Right;
				this.buttonParent.Clicked += new MessageEventHandler(this.HandleButtonParentClicked);
				ToolTip.Default.SetToolTip(this.buttonParent, Res.Strings.Dialog.Open.ParentDirectory);

				//	Liste centrale principale.
				this.CreateTable(20);
				this.CreateRename();

				//	Boutons en bas.
				this.CreateFooter();

				//	Nom du fichier.
				Widget file = new Widget(this.window.Root);
				file.PreferredHeight = 20;
				file.Margins = new Margins(0, 0, 8, 0);
				file.Dock = DockStyle.Bottom;

				label = new StaticText(file);
				label.Text = Res.Strings.Dialog.Open.LabelDoc;
				label.PreferredWidth = 80;
				label.Dock = DockStyle.Left;

				this.fieldFilename = new TextField(file);
				this.fieldFilename.Dock = DockStyle.Fill;
				this.fieldFilename.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleKeyboardFocusChanged);
			}

			this.selectedFilename = null;
			this.UpdateTable(-1);
			this.UpdateInitialDirectory();

			this.fieldFilename.Text = "";
			this.fieldFilename.Focus();

			this.window.ShowDialog();
		}

		public override void Save()
		{
			//	Enregistre la position de la fenêtre du dialogue.
			this.WindowSave("FileOpen");
		}


		protected override void UpdateButtons()
		{
			//	Met à jour les boutons en fonction du fichier sélectionné dans la liste.
			int sel = this.table.SelectedRow;
			this.buttonRename.Enable = (sel != -1);
			this.buttonDelete.Enable = (sel != -1);
		}

		protected override void UpdateInitialDirectory()
		{
			//	Met à jour le chemin d'accès.
			if (this.fieldPath != null)
			{
				this.ignoreChanged = true;
				this.fieldPath.Text = AbstractFile.RemoveStartingSpaces(AbstractFile.GetIllustredPath(this.initialDirectory));
				this.ignoreChanged = false;
			}
		}


		private void HandleFieldPathComboOpening(object sender, CancelEventArgs e)
		{
			//	Le menu pour le chemin d'accès va être ouvert.
			this.comboTexts = new List<string>();
			this.comboDirectories = new List<string>();

			System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();

			this.fieldPath.Items.Clear();

			foreach (System.IO.DriveInfo drive in drives)
			{
				string text = AbstractFile.GetIllustredDriveString(drive);
				this.fieldPath.Items.Add(text);
				this.comboTexts.Add(text);
				this.comboDirectories.Add(drive.Name);

				if (this.initialDirectory.Length > 3 && this.initialDirectory.StartsWith(drive.Name))
				{
					string[] dirs = this.initialDirectory.Split('\\');
					for (int i=1; i<dirs.Length; i++)
					{
						string dir = "";
						text = "";
						for (int j=0; j<=i; j++)
						{
							dir += dirs[j]+"\\";
							text += "   ";
						}
						text += Misc.Image("FileTypeDirectory");
						text += " ";
						text += dirs[i];

						this.fieldPath.Items.Add(text);
						this.comboTexts.Add(text);

						if (dir.Length > 3 && dir.EndsWith("\\"))
						{
							dir = dir.Substring(0, dir.Length-1);
						}
						this.comboDirectories.Add(dir);
					}
				}
			}

			this.comboSelected = -1;
		}

		private void HandleFieldPathComboClosed(object sender)
		{
			//	Le menu pour le chemin d'accès a été fermé.
			if (this.comboSelected != -1)
			{
				this.InitialDirectory = this.comboDirectories[this.comboSelected];
				this.UpdateTable(-1);
			}
		}

		void HandleFieldPathTextChanged(object sender)
		{
			//	Le texte pour le chemin d'accès a changé.
			if (this.ignoreChanged)
			{
				return;
			}

			this.ignoreChanged = true;
			this.comboSelected = this.comboTexts.IndexOf(this.fieldPath.Text);
			this.fieldPath.Text = AbstractFile.RemoveStartingSpaces(this.fieldPath.Text);
			this.ignoreChanged = false;
		}

		private void HandleButtonParentClicked(object sender, MessageEventArgs e)
		{
			//	Le bouton 'dossier parent' a été cliqué.
			this.ParentDirectory();
		}

		private void HandleButtonNewClicked(object sender, MessageEventArgs e)
		{
			//	Le bouton 'nouveau dossier' a été cliqué.
			this.NewDirectory();
		}

		private void HandleButtonRenameClicked(object sender, MessageEventArgs e)
		{
			//	Le bouton 'renommer' a été cliqué.
			this.RenameStarting();
		}

		private void HandleButtonDeleteClicked(object sender, MessageEventArgs e)
		{
			//	Le bouton 'supprimer' a été cliqué.
			this.FileDelete();
		}


		protected TextFieldCombo				fieldPath;
		protected IconButton					buttonParent;
		protected IconButton					buttonNew;
		protected IconButton					buttonRename;
		protected IconButton					buttonDelete;
		protected TextField						fieldFilename;
		protected List<string>					comboDirectories;
		protected List<string>					comboTexts;
		protected int							comboSelected;
	}
}
