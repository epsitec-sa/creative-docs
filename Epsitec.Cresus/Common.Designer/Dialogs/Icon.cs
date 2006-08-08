using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Dialogue permettant de choisir une icône.
	/// </summary>
	public class Icon : Abstract
	{
		public Icon(MainWindow mainWindow) : base(mainWindow)
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
				this.WindowInit("Icon", 300, 400, true);
				this.window.Text = Res.Strings.Dialog.Icon.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);
				this.window.Root.MinSize = new Size(200, 150);

				int tabIndex = 0;

				this.array = new MyWidgets.StringArray(this.window.Root);
				this.array.Columns = 3;
				this.array.SetColumnsRelativeWidth(0, 0.2);
				this.array.SetColumnsRelativeWidth(1, 0.4);
				this.array.SetColumnsRelativeWidth(2, 0.4);
				this.array.LineHeight = 40;
				this.array.Dock = DockStyle.Fill;
				this.array.Margins = new Margins (6, 6, 6, 6+30);
				this.array.CellCountChanged += new EventHandler(this.HandleArrayCellCountChanged);
				this.array.CellsContentChanged += new EventHandler(this.HandleArrayCellsContentChanged);
				this.array.SelectedRowChanged += new EventHandler(this.HandleArraySelectedRowChanged);
				this.array.SelectedRowDoubleClicked += new EventHandler(this.HandleArraySelectedRowDoubleClicked);
				this.array.TabIndex = tabIndex++;
				this.array.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				//	Boutons de fermeture.
				Button buttonOk = new Button(this.window.Root);
				buttonOk.PreferredWidth = 75;
				buttonOk.Text = Res.Strings.Dialog.Icon.Button.OK;
				buttonOk.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonOk.Anchor = AnchorStyles.BottomLeft;
				buttonOk.Margins = new Margins(6, 0, 0, 6);
				buttonOk.Clicked += new MessageEventHandler(this.HandleButtonInsertClicked);
				buttonOk.TabIndex = tabIndex++;
				buttonOk.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				Button buttonClose = new Button(this.window.Root);
				buttonClose.PreferredWidth = 75;
				buttonClose.Text = Res.Strings.Dialog.Icon.Button.Cancel;
				buttonClose.Anchor = AnchorStyles.BottomLeft;
				buttonClose.Margins = new Margins(6+75+10, 0, 0, 6);
				buttonClose.Clicked += new MessageEventHandler(this.HandleButtonCloseClicked);
				buttonClose.TabIndex = tabIndex++;
				buttonClose.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			}

			this.UpdateArray();
			this.array.ShowSelectedRow();

			this.window.ShowDialog();
		}


		public void SetResourceManager(ResourceManager manager, string moduleName)
		{
			//	Détermine le module pour lequel on désire choisir une icône.
			this.manager = manager;
			this.moduleName = moduleName;

			string[] names = ImageProvider.Default.GetImageNames("manifest", this.manager);
			this.icons = new List<string>();

			for (int i=0; i<names.Length; i++)
			{
				//	Fractionne le nom du type "manifest:Epsitec.Common.Designer.Images.xxx.icon".
				string app, name;
				Icon.GetIconNames(names[i], out app, out name);

				//?if (app == this.moduleName)  // TODO: ne fonctionne pas, voir mail du 08.08.06 11:50
				if (true)
				{
					this.icons.Add(names[i]);
				}
			}
		}

		public string IconValue
		{
			//	Nom complet de l'icône, du type "manifest:Epsitec.Common.Designer.Images.xxx.icon".
			get
			{
				return this.icon;
			}
			set
			{
				this.icon = value;
			}
		}


		protected void UpdateArray()
		{
			//	Met à jour le tableau des icônes.
			if (this.icons == null)
			{
				return;
			}

			this.array.TotalRows = this.icons.Count;

			int first = this.array.FirstVisibleRow;
			for (int i=0; i<this.array.LineCount; i++)
			{
				int row = first+i;

				if (row == 0)
				{
					this.array.SetLineState(0, row, MyWidgets.StringList.CellState.Normal);
					this.array.SetLineState(1, row, MyWidgets.StringList.CellState.Normal);
					this.array.SetLineState(2, row, MyWidgets.StringList.CellState.Normal);
					this.array.SetLineString(0, row, "");
					this.array.SetLineString(1, row, "");
					this.array.SetLineString(2, row, Res.Strings.Dialog.Icon.None);
				}
				else if (row-1 < this.icons.Count)
				{
					string icon = this.icons[row-1];
					string text = string.Format(@"<img src=""{0}""/>", icon);

					string app, name;
					Icon.GetIconNames(icon, out app, out name);

					this.array.SetLineState(0, row, MyWidgets.StringList.CellState.Normal);
					this.array.SetLineState(1, row, MyWidgets.StringList.CellState.Normal);
					this.array.SetLineState(2, row, MyWidgets.StringList.CellState.Normal);
					this.array.SetLineString(0, row, text);
					this.array.SetLineString(1, row, app);
					this.array.SetLineString(2, row, name);
				}
				else
				{
					this.array.SetLineState(0, row, MyWidgets.StringList.CellState.Disabled);
					this.array.SetLineState(1, row, MyWidgets.StringList.CellState.Disabled);
					this.array.SetLineState(2, row, MyWidgets.StringList.CellState.Disabled);
					this.array.SetLineString(0, row, "");
					this.array.SetLineString(1, row, "");
					this.array.SetLineString(2, row, "");
				}
			}

			this.array.SelectedRow = this.SelectedIcon;
		}

		protected int SelectedIcon
		{
			//	Retourne le rang de l'icône choisie dans le tableau.
			get
			{
				if (string.IsNullOrEmpty(this.icon))
				{
					return 0;  // première ligne 'aucune'
				}

				for (int i=0; i<this.icons.Count; i++)
				{
					if (this.icons[i] == this.icon)
					{
						return i+1;
					}
				}

				return -1;
			}
		}

		protected static void GetIconNames(string fullName, out string app, out string shortName)
		{
			//	Fractionne le nom du type "manifest:Epsitec.Common.Designer.Images.xxx.icon".
			//	TODO: faire mieux !
			string[] parts = fullName.Split('.');
			app = parts[parts.Length-4];
			shortName = parts[parts.Length-2];
		}


		void HandleArrayCellCountChanged(object sender)
		{
			//	Le nombre de lignes a changé.
			this.UpdateArray();
			this.array.ShowSelectedRow();
		}

		void HandleArrayCellsContentChanged(object sender)
		{
			//	Le contenu des cellules a changé.
			this.UpdateArray();
		}

		void HandleArraySelectedRowChanged(object sender)
		{
			//	La ligne sélectionnée a changé.
		}

		void HandleArraySelectedRowDoubleClicked(object sender)
		{
			//	La ligne sélectionnée a changé.
			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();

			int sel = this.array.SelectedRow;
			if (sel <= 0)
			{
				this.icon = null;
			}
			else
			{
				this.icon = this.icons[sel-1];
			}
		}

		private void HandleWindowCloseClicked(object sender)
		{
			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandleButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandleButtonInsertClicked(object sender, MessageEventArgs e)
		{
			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();

			int sel = this.array.SelectedRow;
			if (sel <= 0)
			{
				this.icon = null;
			}
			else
			{
				this.icon = this.icons[sel-1];
			}
		}


		protected ResourceManager			manager;
		protected string					moduleName;
		protected List<string>				icons;
		protected string					icon;
		protected MyWidgets.StringArray		array;
	}
}
