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

				ResizeKnob resize = new ResizeKnob(this.window.Root);
				resize.Anchor = AnchorStyles.BottomRight;
				resize.Margins = new Margins(0, -8, 0, -8);
				ToolTip.Default.SetToolTip(resize, Res.Strings.Dialog.Tooltip.Resize);

				int tabIndex = 0;

				//	Bande horizontale pour la recherche.
				Widget header = new Widget(this.window.Root);
				header.PreferredHeight = 20;
				header.Margins = new Margins(0, 0, 0, 6);
				header.Dock = DockStyle.Top;

				StaticText label = new StaticText(header);
				label.Text = Res.Strings.Dialog.Icon.Label.Search;
				label.PreferredWidth = 70;
				label.Dock = DockStyle.Left;

				this.fieldSearch = new TextFieldCombo(header);
				this.fieldSearch.Margins = new Margins(5, 5, 0, 0);
				this.fieldSearch.Dock = DockStyle.Fill;

				this.searchNext = new IconButton(header);
				this.searchNext.IconName = Misc.Icon("SearchNext");
				this.searchNext.Dock = DockStyle.Right;
				this.searchNext.Clicked += new MessageEventHandler(this.HandleSearchNextClicked);
				ToolTip.Default.SetToolTip(this.searchNext, Res.Strings.Action.SearchNext);

				this.searchPrev = new IconButton(header);
				this.searchPrev.IconName = Misc.Icon("SearchPrev");
				this.searchPrev.Dock = DockStyle.Right;
				this.searchPrev.Clicked += new MessageEventHandler(this.HandleSearchPrevClicked);
				ToolTip.Default.SetToolTip(this.searchPrev, Res.Strings.Action.SearchPrev);

				//	Bande horizontale pour le filtre.
				header = new Widget(this.window.Root);
				header.PreferredHeight = 20;
				header.Margins = new Margins(0, 0, 0, 6);
				header.Dock = DockStyle.Top;

				label = new StaticText(header);
				label.Text = Res.Strings.Dialog.Icon.Label.Filter;
				label.PreferredWidth = 70;
				label.Dock = DockStyle.Left;

				this.fieldFilter = new TextFieldCombo(header);
				this.fieldFilter.Text = Res.Strings.Dialog.Icon.Filter.All;
				this.fieldFilter.IsReadOnly = true;
				this.fieldFilter.Margins = new Margins(5, 5+22+22, 0, 0);
				this.fieldFilter.Dock = DockStyle.Fill;
				this.fieldFilter.ComboClosed += new EventHandler(this.HandleFieldFilterComboClosed);

				//	Tableau principal.
				this.arrayDetail = new MyWidgets.StringArray(this.window.Root);
				this.arrayDetail.Columns = 3;
				this.arrayDetail.SetColumnsRelativeWidth(0, 0.15);  // icône
				this.arrayDetail.SetColumnsRelativeWidth(1, 0.30);  // nom du module
				this.arrayDetail.SetColumnsRelativeWidth(2, 0.55);  // nom de l'icône
				this.arrayDetail.SetColumnAlignment(0, ContentAlignment.MiddleCenter);
				this.arrayDetail.LineHeight = 25;
				this.arrayDetail.Dock = DockStyle.Fill;
				this.arrayDetail.CellCountChanged += new EventHandler(this.HandleArrayCellCountChanged);
				this.arrayDetail.CellsContentChanged += new EventHandler(this.HandleArrayCellsContentChanged);
				this.arrayDetail.SelectedRowChanged += new EventHandler(this.HandleArraySelectedRowChanged);
				this.arrayDetail.SelectedRowDoubleClicked += new EventHandler(this.HandleArraySelectedRowDoubleClicked);
				this.arrayDetail.TabIndex = tabIndex++;
				this.arrayDetail.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				this.arrayCompact = new MyWidgets.IconArray(this.window.Root);
				this.arrayCompact.CellSize = this.arrayDetail.LineHeight;
				this.arrayCompact.Dock = DockStyle.Fill;
				this.arrayCompact.ChangeSelected += new EventHandler(this.HandleArrayCompactChangeSelected);
				this.arrayCompact.TabIndex = tabIndex++;
				this.arrayCompact.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(this.arrayCompact, "*");

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
				buttonClose.ButtonStyle = ButtonStyle.DefaultCancel;
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
				this.slider.Value = (decimal) this.arrayDetail.LineHeight;
				this.slider.ValueChanged += new EventHandler(this.HandleSliderChanged);
				ToolTip.Default.SetToolTip(this.slider, Res.Strings.Dialog.Icon.Tooltip.Size);

				this.buttonMode = new IconButton(footer);
				this.buttonMode.IconName = Misc.Icon("DialogIconMode");
				this.buttonMode.ButtonStyle = ButtonStyle.ActivableIcon;
				this.buttonMode.Dock = DockStyle.Right;
				this.buttonMode.Margins = new Margins(0, 6, 0, 0);
				this.buttonMode.Clicked += new MessageEventHandler(this.HandleButtonModeClicked);
				ToolTip.Default.SetToolTip(this.buttonMode, Res.Strings.Dialog.Icon.Tooltip.Mode);
			}

			this.UpdateMode();
			this.UpdateFilter();
			this.UpdateArray();
			this.Selected = this.SelectedIcon(this.icon);
			this.ShowSelection();

			this.window.ShowDialog();
		}


		public void SetResourceManager(ResourceManager manager, string moduleName)
		{
			//	Détermine le module pour lequel on désire choisir une icône.
			this.manager = manager;
			this.moduleName = moduleName;

			this.modules = new List<string>();
			this.modules.Clear();
			this.modules.Add(Res.Strings.Dialog.Icon.Filter.All);

			string[] names = ImageProvider.Default.GetImageNames("manifest", this.manager);
			for (int i=0; i<names.Length; i++)
			{
				string module, name;
				Misc.GetIconNames(names[i], out module, out name);
				string text = string.Format(Res.Strings.Dialog.Icon.Filter.One, module);

				if (!this.modules.Contains(text))
				{
					this.modules.Add(text);
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


		protected void UpdateMode()
		{
			this.arrayDetail.Visibility = !this.compactMode;
			this.arrayCompact.Visibility = this.compactMode;

			this.buttonMode.ActiveState = this.compactMode ? ActiveState.Yes : ActiveState.No;
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
				if (filter == Res.Strings.Dialog.Icon.Filter.All)
				{
					filter = null;
				}
			}

			this.icons = new List<string>();
			string[] names = ImageProvider.Default.GetImageNames("manifest", this.manager);
			for (int i=0; i<names.Length; i++)
			{
				string module, name;
				Misc.GetIconNames(names[i], out module, out name);
				string text = string.Format(Res.Strings.Dialog.Icon.Filter.One, module);

				if (filter == null || filter == text)
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

			this.arrayDetail.TotalRows = this.icons.Count;

			int first = this.arrayDetail.FirstVisibleRow;
			for (int i=0; i<this.arrayDetail.LineCount; i++)
			{
				int row = first+i;

				if (row == 0)  // première ligne 'pas d'icône' ?
				{
					this.arrayDetail.SetLineState(0, row, MyWidgets.StringList.CellState.Normal);
					this.arrayDetail.SetLineState(1, row, MyWidgets.StringList.CellState.Normal);
					this.arrayDetail.SetLineState(2, row, MyWidgets.StringList.CellState.Normal);
					this.arrayDetail.SetLineString(0, row, "");
					this.arrayDetail.SetLineString(1, row, "");
					this.arrayDetail.SetLineString(2, row, Res.Strings.Dialog.Icon.None);
				}
				else if (row-1 < this.icons.Count)
				{
					string icon = this.icons[row-1];
					string text = Misc.ImageFull(icon);

					string module, name;
					Misc.GetIconNames(icon, out module, out name);

					this.arrayDetail.SetLineState(0, row, MyWidgets.StringList.CellState.Normal);
					this.arrayDetail.SetLineState(1, row, MyWidgets.StringList.CellState.Normal);
					this.arrayDetail.SetLineState(2, row, MyWidgets.StringList.CellState.Normal);
					this.arrayDetail.SetLineString(0, row, text);
					this.arrayDetail.SetLineString(1, row, module);
					this.arrayDetail.SetLineString(2, row, name);
				}
				else
				{
					this.arrayDetail.SetLineState(0, row, MyWidgets.StringList.CellState.Disabled);
					this.arrayDetail.SetLineState(1, row, MyWidgets.StringList.CellState.Disabled);
					this.arrayDetail.SetLineState(2, row, MyWidgets.StringList.CellState.Disabled);
					this.arrayDetail.SetLineString(0, row, "");
					this.arrayDetail.SetLineString(1, row, "");
					this.arrayDetail.SetLineString(2, row, "");
				}
			}

			this.arrayCompact.SetIcons(this.icons);
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

		protected void Search(string searching, int direction)
		{
			//	Cherche dans une direction donnée.
			searching = Searcher.RemoveAccent(searching.ToLower());
			int sel = this.Selected-1;

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
				Misc.GetIconNames(this.icons[sel], out module, out name);
				name = Searcher.RemoveAccent(name.ToLower());

				if (name.Contains(searching))
				{
					this.Selected = sel+1;
					this.ShowSelection();
					return;
				}
			}

			this.mainWindow.DialogMessage(Res.Strings.Dialog.Search.Message.Error);
		}

		protected int Selected
		{
			//	Case sélectionnée dans un tableau (détaillé ou compact).
			get
			{
				if (this.compactMode)
				{
					return this.arrayCompact.SelectedIndex;
				}
				else
				{
					return this.arrayDetail.SelectedRow;
				}
			}

			set
			{
				this.arrayDetail.SelectedRow = value;
				this.arrayCompact.SelectedIndex = value;
			}
		}

		protected void ShowSelection()
		{
			//	Montre la sélection dans un tableau (détaillé ou compact).
			this.arrayDetail.ShowSelectedRow();
			this.arrayCompact.ShowSelectedCell();
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
			int sel = this.Selected;
			if (sel > 0)
			{
				icon = this.icons[sel-1];
			}

			this.UpdateFilter();
			this.UpdateArray();

			this.Selected = this.SelectedIcon(icon);
			this.ShowSelection();
		}

		void HandleArrayCellCountChanged(object sender)
		{
			//	Le nombre de lignes a changé.
			this.UpdateArray();
			this.ShowSelection();
		}

		void HandleArrayCellsContentChanged(object sender)
		{
			//	Le contenu des cellules a changé.
			this.UpdateArray();
		}

		void HandleArraySelectedRowChanged(object sender)
		{
			//	La ligne sélectionnée a changé.
			if (this.ignoreChanged)
			{
				return;
			}

			this.ignoreChanged = true;
			this.arrayCompact.SelectedIndex = this.arrayDetail.SelectedRow;
			this.ShowSelection();
			this.ignoreChanged = false;
		}

		void HandleArrayCompactChangeSelected(object sender)
		{
			//	La cellule sélectionnée a changé.
			if (this.ignoreChanged)
			{
				return;
			}

			this.ignoreChanged = true;
			this.arrayDetail.SelectedRow = this.arrayCompact.SelectedIndex;
			this.ShowSelection();
			this.ignoreChanged = false;
		}

		void HandleArraySelectedRowDoubleClicked(object sender)
		{
			//	La ligne sélectionnée a changé.
			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();

			int sel = this.Selected;
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

			int sel = this.Selected;
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
			this.arrayDetail.LineHeight = (double) slider.Value;
			this.arrayCompact.CellSize = (double) slider.Value;
		}

		void HandleButtonModeClicked(object sender, MessageEventArgs e)
		{
			this.compactMode = !this.compactMode;
			this.UpdateMode();
			this.ShowSelection();
		}



		protected ResourceManager			manager;
		protected string					moduleName;
		protected List<string>				modules;
		protected List<string>				icons;
		protected string					icon;
		protected bool						compactMode = false;

		protected TextFieldCombo			fieldSearch;
		protected TextFieldCombo			fieldFilter;
		protected IconButton				searchPrev;
		protected IconButton				searchNext;
		protected MyWidgets.StringArray		arrayDetail;
		protected MyWidgets.IconArray		arrayCompact;
		protected HSlider					slider;
		protected IconButton				buttonMode;
	}
}
