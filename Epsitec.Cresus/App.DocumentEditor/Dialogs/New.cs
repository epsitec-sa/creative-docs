using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;
using Epsitec.Common.IO;

namespace Epsitec.App.DocumentEditor.Dialogs
{
	using GlobalSettings = Common.Document.Settings.GlobalSettings;
	using FontFaceCombo  = Common.Document.Widgets.FontFaceCombo;

	/// <summary>
	/// Dialogue permettant de choisir un caractère quelconque à insérer dans
	/// un texte en édition.
	/// </summary>
	public class New : Abstract
	{
		public New(DocumentEditor editor) : base(editor)
		{
		}

		public override void Show()
		{
			//	Crée et montre la fenêtre du dialogue.
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.MakeSecondaryWindow();
				this.window.PreventAutoClose = true;
				this.WindowInit("New", 300, 300, true);
				this.window.Text = Res.Strings.Dialog.New.Title;
				this.window.Owner = this.editor.Window;
				this.window.Icon = Bitmap.FromManifestResource("Epsitec.App.DocumentEditor.Images.Application.icon", this.GetType().Assembly);
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);
				this.window.Root.MinSize = new Size(200, 200);
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				ResizeKnob resize = new ResizeKnob(this.window.Root);
				resize.Anchor = AnchorStyles.BottomRight;
				resize.Margins = new Margins(0, -8, 0, -8);
				ToolTip.Default.SetToolTip(resize, Res.Strings.Dialog.Tooltip.Resize);

				int tabIndex = 0;

				this.table = new CellTable(this.window.Root);
				this.table.DefHeight = 80;
				this.table.StyleH = CellArrayStyles.Stretch | CellArrayStyles.Separator;
				this.table.StyleV = CellArrayStyles.ScrollNorm | CellArrayStyles.Separator | CellArrayStyles.SelectLine;
				this.table.Margins = new Margins(0, 0, 0, 0);
				this.table.Dock = DockStyle.Fill;

				//	Pied.
				Widget footer = new Widget(this.window.Root);
				footer.PreferredHeight = 22;
				footer.Margins = new Margins(0, 0, 8, 0);
				footer.Dock = DockStyle.Bottom;

				Button buttonOpen = new Button(footer);
				buttonOpen.PreferredWidth = 75;
				buttonOpen.Text = Res.Strings.Dialog.New.Button.Open;
				buttonOpen.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonOpen.Dock = DockStyle.Left;
				buttonOpen.Margins = new Margins(0, 6, 0, 0);
				buttonOpen.Clicked += new MessageEventHandler(this.HandleButtonOpenClicked);
				buttonOpen.TabIndex = tabIndex++;
				buttonOpen.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				Button buttonCancel = new Button(footer);
				buttonCancel.PreferredWidth = 75;
				buttonCancel.Text = Res.Strings.Dialog.Button.Cancel;
				buttonCancel.ButtonStyle = ButtonStyle.DefaultCancel;
				buttonCancel.Dock = DockStyle.Left;
				buttonCancel.Margins = new Margins(0, 6, 0, 0);
				buttonCancel.Clicked += new MessageEventHandler(this.HandleButtonCancelClicked);
				buttonCancel.TabIndex = tabIndex++;
				buttonCancel.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			}

			this.selectedFilename = null;
			this.UpdateTable(-1);

			this.window.ShowDialog();
		}

		public override void Save()
		{
			//	Enregistre la position de la fenêtre du dialogue.
			this.WindowSave("New");
		}


		public string Filename
		{
			get
			{
				return this.selectedFilename;
			}
		}


		protected void UpdateTable(int sel)
		{
			//	Met à jour la table des fichiers.
			this.listedFilenames = this.GetFilenames();
			int rows = (this.listedFilenames == null) ? 0 : this.listedFilenames.Length;

			this.table.SetArraySize(4, rows);
			this.table.SetWidthColumn(0, 50);
			this.table.SetWidthColumn(1, 100);
			this.table.SetWidthColumn(2, 80);
			this.table.SetWidthColumn(3, 20);

			StaticText st;
			for (int row=0; row<rows; row++)
			{
				for (int column=0; column<this.table.Columns; column++)
				{
					if (this.table[column, row].IsEmpty)
					{
						if (column == 0)  // miniature ?
						{
							st = new StaticText();
							st.ContentAlignment = ContentAlignment.MiddleLeft;
							st.Dock = DockStyle.Fill;
							st.Margins = new Margins(6, 0, 0, 0);
							this.table[column, row].Insert(st);
						}
						else if (column == 1)  // résumé ?
						{
							st = new StaticText();
							st.ContentAlignment = ContentAlignment.MiddleLeft;
							st.Dock = DockStyle.Fill;
							st.Margins = new Margins(6, 0, 0, 0);
							this.table[column, row].Insert(st);
						}
						else if (column == 2)  // filename ?
						{
							st = new StaticText();
							st.ContentAlignment = ContentAlignment.MiddleLeft;
							st.Dock = DockStyle.Fill;
							st.Margins = new Margins(6, 0, 0, 0);
							this.table[column, row].Insert(st);
						}
						else if (column == 3)  // taille ?
						{
							st = new StaticText();
							st.ContentAlignment = ContentAlignment.MiddleLeft;
							st.Dock = DockStyle.Fill;
							st.Margins = new Margins(6, 0, 0, 0);
							this.table[column, row].Insert(st);
						}
					}
				}

				st = this.table[2, row].Children[0] as StaticText;
				st.Text = System.IO.Path.GetFileNameWithoutExtension(this.listedFilenames[row]);

				this.table.SelectRow(row, row==sel);
			}

			if (sel != -1)
			{
				this.table.ShowSelect();  // montre la ligne sélectionnée
			}
		}

		protected string[] GetFilenames()
		{
			//	Retourne la liste des fichiers .crmod contenus dans le dossier adhoc.
			try
			{
				string path = System.IO.Path.GetDirectoryName(this.globalSettings.NewDocument);
				return System.IO.Directory.GetFiles(path, "*.crmod", System.IO.SearchOption.TopDirectoryOnly);
			}
			catch
			{
				return null;
			}
		}



		private void HandleWindowCloseClicked(object sender)
		{
			this.editor.Window.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandleButtonCancelClicked(object sender, MessageEventArgs e)
		{
			this.editor.Window.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandleButtonOpenClicked(object sender, MessageEventArgs e)
		{
			this.editor.Window.MakeActive();
			this.window.Hide();
			this.OnClosed();

			int sel = this.table.SelectedRow;
			if (sel == -1)
			{
				this.selectedFilename = null;
			}
			else
			{
				this.selectedFilename = this.listedFilenames[sel];
			}
		}


		protected CellTable					table;
		protected string[]					listedFilenames;
		protected string					selectedFilename;
	}
}
