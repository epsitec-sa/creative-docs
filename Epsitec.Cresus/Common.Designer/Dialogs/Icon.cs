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
				this.WindowInit("Icon", 400, 400, true);
				this.window.Text = Res.Strings.Dialog.Icon.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);
				this.window.Root.MinSize = new Size(200, 150);
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				int tabIndex = 0;

				//	Bande horizontale pour la recherche.
				Widget header = new Widget(this.window.Root);
				header.PreferredHeight = 20;
				header.Margins = new Margins(0, 0, 0, 6);
				header.Dock = DockStyle.Top;

				StaticText label = new StaticText(header);
				label.Text = Res.Strings.Dialog.Search.Button.Search;
				label.PreferredWidth = 70;
				label.Dock = DockStyle.Left;

				this.fieldSearch = new TextFieldCombo(header);
				this.fieldSearch.Margins = new Margins(5, 5, 0, 0);
				this.fieldSearch.Dock = DockStyle.Fill;

				this.searchNext = new IconButton(header);
				this.searchNext.IconName = Misc.Icon("AccessNext");
				this.searchNext.Dock = DockStyle.Right;
				this.searchNext.Clicked += new MessageEventHandler(this.HandleSearchNextClicked);
				ToolTip.Default.SetToolTip(this.searchNext, Res.Strings.Action.SearchNext);

				this.searchPrev = new IconButton(header);
				this.searchPrev.IconName = Misc.Icon("AccessPrev");
				this.searchPrev.Dock = DockStyle.Right;
				this.searchPrev.Clicked += new MessageEventHandler(this.HandleSearchPrevClicked);
				ToolTip.Default.SetToolTip(this.searchPrev, Res.Strings.Action.SearchPrev);

				//	Bande horizontale pour le filtre.
				header = new Widget(this.window.Root);
				header.PreferredHeight = 20;
				header.Margins = new Margins(0, 0, 0, 6);
				header.Dock = DockStyle.Top;

				label = new StaticText(header);
				label.Text = Res.Strings.Dialog.Filter.Button.Insert;
				label.PreferredWidth = 70;
				label.Dock = DockStyle.Left;

				this.fieldFilter = new TextFieldCombo(header);
				this.fieldFilter.Text = Res.Strings.Dialog.Filter.Button.All;
				this.fieldFilter.IsReadOnly = true;
				this.fieldFilter.Margins = new Margins(5, 5+22+22, 0, 0);
				this.fieldFilter.Dock = DockStyle.Fill;
				this.fieldFilter.ComboClosed += new EventHandler(this.HandleFieldFilterComboClosed);

				//	Tableau principal.
				this.array = new MyWidgets.StringArray(this.window.Root);
				this.array.Columns = 3;
				this.array.SetColumnsRelativeWidth(0, 0.15);  // icône
				this.array.SetColumnsRelativeWidth(1, 0.30);  // nom du module
				this.array.SetColumnsRelativeWidth(2, 0.55);  // nom de l'icône
				this.array.SetColumnAlignment(0, ContentAlignment.MiddleCenter);
				this.array.LineHeight = 25;
				this.array.Dock = DockStyle.Fill;
				this.array.CellCountChanged += new EventHandler(this.HandleArrayCellCountChanged);
				this.array.CellsContentChanged += new EventHandler(this.HandleArrayCellsContentChanged);
				this.array.SelectedRowChanged += new EventHandler(this.HandleArraySelectedRowChanged);
				this.array.SelectedRowDoubleClicked += new EventHandler(this.HandleArraySelectedRowDoubleClicked);
				this.array.TabIndex = tabIndex++;
				this.array.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				//	Pied.
				Widget footer = new Widget(this.window.Root);
				footer.PreferredHeight = 22;
				footer.Margins = new Margins(0, 0, 8, 0);
				footer.Dock = DockStyle.Bottom;

				Button buttonOk = new Button(footer);
				buttonOk.PreferredWidth = 75;
				buttonOk.Text = Res.Strings.Dialog.Icon.Button.OK;
				buttonOk.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonOk.Dock = DockStyle.Left;
				buttonOk.Margins = new Margins(0, 6, 0, 0);
				buttonOk.Clicked += new MessageEventHandler(this.HandleButtonInsertClicked);
				buttonOk.TabIndex = tabIndex++;
				buttonOk.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				Button buttonClose = new Button(footer);
				buttonClose.PreferredWidth = 75;
				buttonClose.Text = Res.Strings.Dialog.Icon.Button.Cancel;
				buttonClose.Dock = DockStyle.Left;
				buttonClose.Margins = new Margins(0, 6, 0, 0);
				buttonClose.Clicked += new MessageEventHandler(this.HandleButtonCloseClicked);
				buttonClose.TabIndex = tabIndex++;
				buttonClose.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				this.slider = new HSlider(footer);
				this.slider.PreferredWidth = 80;
				this.slider.Dock = DockStyle.Right;
				this.slider.Margins = new Margins(0, 0, 4, 4);
				this.slider.TabIndex = tabIndex++;
				this.slider.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.slider.MinValue = 20.0M;
				this.slider.MaxValue = 50.0M;
				this.slider.SmallChange = 1.0M;
				this.slider.LargeChange = 5.0M;
				this.slider.Resolution = 1.0M;
				this.slider.Value = (decimal) this.array.LineHeight;
				this.slider.ValueChanged += new EventHandler(this.HandleSliderChanged);
			}

			this.UpdateFilter();
			this.UpdateArray();
			this.array.SelectedRow = this.SelectedIcon(this.icon);
			this.array.ShowSelectedRow();

			this.window.ShowDialog();
		}


		public void SetResourceManager(ResourceManager manager, string moduleName)
		{
			//	Détermine le module pour lequel on désire choisir une icône.
			this.manager = manager;
			this.moduleName = moduleName;

			this.modules = new List<string>();
			this.modules.Clear();
			this.modules.Add(Res.Strings.Dialog.Filter.Button.All);

			string[] names = ImageProvider.Default.GetImageNames("manifest", this.manager);
			for (int i=0; i<names.Length; i++)
			{
				string module, name;
				Icon.GetIconNames(names[i], out module, out name);

				if (!this.modules.Contains(module))
				{
					this.modules.Add(module);
				}
			}

			this.UpdateFilter();
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


		protected void UpdateFilter()
		{
			string filter = null;

			if (this.fieldFilter != null)
			{
				this.fieldFilter.Items.Clear();
				foreach (string module in this.modules)
				{
					this.fieldFilter.Items.Add(module);
				}

				filter = this.fieldFilter.Text;
				if (filter == Res.Strings.Dialog.Filter.Button.All)
				{
					filter = null;
				}
			}

			this.icons = new List<string>();
			string[] names = ImageProvider.Default.GetImageNames("manifest", this.manager);
			for (int i=0; i<names.Length; i++)
			{
				string module, name;
				Icon.GetIconNames(names[i], out module, out name);

				if (filter == null || filter == module)
				{
					this.icons.Add(names[i]);
				}
			}

			if (!string.IsNullOrEmpty(this.icon) && this.SelectedIcon(this.icon) == 0)
			{
				//	Si l'icône n'existe pas avec le filtre actuel, on l'ajoute en
				//	première position, juste après 'pas d'icône'.
				this.icons.Insert(0, this.icon);
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

				if (row == 0)  // première ligne 'pas d'icône' ?
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

					string module, name;
					Icon.GetIconNames(icon, out module, out name);

					this.array.SetLineState(0, row, MyWidgets.StringList.CellState.Normal);
					this.array.SetLineState(1, row, MyWidgets.StringList.CellState.Normal);
					this.array.SetLineState(2, row, MyWidgets.StringList.CellState.Normal);
					this.array.SetLineString(0, row, text);
					this.array.SetLineString(1, row, module);
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
		}

		protected int SelectedIcon(string icon)
		{
			//	Retourne le rang d'une icône dans le tableau.
			if (!string.IsNullOrEmpty(icon))
			{
				for (int i=0; i<this.icons.Count; i++)
				{
					if (this.icons[i] == icon)
					{
						return i+1;
					}
				}
			}

			return 0;  // première ligne 'pas d'icône'
		}

		public static void GetIconNames(string fullName, out string moduleName, out string shortName)
		{
			//	Fractionne un nom du type "manifest:Epsitec.Common.Designer.Images.xxx.icon".
			//	TODO: faire mieux !
			if (string.IsNullOrEmpty(fullName))
			{
				moduleName = null;
				shortName = null;
			}
			else
			{
				string[] parts = fullName.Split('.');
				moduleName = parts[parts.Length-4];
				shortName = parts[parts.Length-2];
			}
		}

		protected void Search(string searching, int direction)
		{
			//	Cherche dans une direction donnée.
			searching = Searcher.RemoveAccent(searching.ToLower());
			int sel = this.array.SelectedRow-1;

			for (int i=0; i<this.icons.Count; i++)
			{
				sel += direction;  // suivant, en avant ou en arrière

				if (sel >= this.icons.Count)  // fin dépassée ?
				{
					sel = 0;  // revient au début
				}

				if (sel < 0)  // début dépassé ?
				{
					sel = this.icons.Count-1;  // va à la fin
				}

				string module, name;
				Icon.GetIconNames(this.icons[sel], out module, out name);
				name = Searcher.RemoveAccent(name.ToLower());

				if (name.Contains(searching))
				{
					this.array.SelectedRow = sel+1;
					this.array.ShowSelectedRow();
					return;
				}
			}

			this.mainWindow.DialogMessage(Res.Strings.Dialog.Search.Message.Error);
		}


		void HandleSearchPrevClicked(object sender, MessageEventArgs e)
		{
			//	Cherche l'occurence précédente.
			Misc.ComboMenuAdd(this.fieldSearch);
			this.Search(this.fieldSearch.Text, -1);
		}

		void HandleSearchNextClicked(object sender, MessageEventArgs e)
		{
			//	Cherche l'occurence suivante.
			Misc.ComboMenuAdd(this.fieldSearch);
			this.Search(this.fieldSearch.Text, 1);
		}

		void HandleFieldFilterComboClosed(object sender)
		{
			//	Menu pour choisir le filtre fermé.
			string icon = null;
			int sel = this.array.SelectedRow;
			if (sel > 0)
			{
				icon = this.icons[sel-1];
			}

			this.UpdateFilter();
			this.UpdateArray();

			this.array.SelectedRow = this.SelectedIcon(icon);
			this.array.ShowSelectedRow();
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

		private void HandleSliderChanged(object sender)
		{
			HSlider slider = sender as HSlider;
			if ( slider == null )  return;
			this.array.LineHeight = (double) slider.Value;
		}


		protected ResourceManager			manager;
		protected string					moduleName;
		protected List<string>				modules;
		protected List<string>				icons;
		protected string					icon;

		protected TextFieldCombo			fieldSearch;
		protected TextFieldCombo			fieldFilter;
		protected IconButton				searchPrev;
		protected IconButton				searchNext;
		protected MyWidgets.StringArray		array;
		protected HSlider					slider;
	}
}
