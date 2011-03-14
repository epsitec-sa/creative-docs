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
		public Icon(DesignerApplication designerApplication) : base(designerApplication)
		{
		}

		public override void Show()
		{
			//	Crée et montre la fenêtre du dialogue.
			if ( this.window == null )
			{
				this.window = new Window ();
				this.window.Icon = this.designerApplication.Icon;
				this.window.MakeSecondaryWindow ();
				this.window.PreventAutoClose = true;
				this.WindowInit ("Icon", 400, 400, true);
				this.window.Text = Res.Strings.Dialog.Icon.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += this.HandleWindowCloseClicked;
				this.window.Root.MinSize = new Size (200, 150);
				this.window.Root.Padding = new Margins (8, 8, 8, 8);

				ResizeKnob resize = new ResizeKnob (this.window.Root);
				resize.Anchor = AnchorStyles.BottomRight;
				resize.Margins = new Margins (0, -8, 0, -8);
				ToolTip.Default.SetToolTip (resize, Res.Strings.Dialog.Tooltip.Resize);

				//	Bande horizontale pour la recherche.
				{
					var topFrame = new FrameBox (this.window.Root);
					topFrame.PreferredHeight = 20;
					topFrame.Margins = new Margins (0, 0, 0, 6);
					topFrame.Dock = DockStyle.Top;

					var label = new StaticText (topFrame);
					label.Text = Res.Strings.Dialog.Icon.Label.Search;
					label.PreferredWidth = 64;
					label.Dock = DockStyle.Left;

					this.fieldSearch = new TextField (topFrame);
					this.fieldSearch.Dock = DockStyle.Fill;
					this.fieldSearch.TextChanged += new EventHandler (this.HandleFieldSearchTextChanged);

					var clearButton = new GlyphButton (topFrame);
					clearButton.GlyphShape = GlyphShape.Close;
					clearButton.Dock = DockStyle.Right;
					clearButton.Margins = new Margins (1, 0, 0, 0);

					clearButton.Clicked += delegate
					{
						this.fieldSearch.Text = null;
						this.fieldSearch.SelectAll ();
						this.fieldSearch.Focus ();
					};
				}

				//	Bande horizontale pour le filtre.
				{
					var topFrame = new Widget (this.window.Root);
					topFrame.PreferredHeight = 20;
					topFrame.Margins = new Margins (0, 0, 0, 6);
					topFrame.Dock = DockStyle.Top;

					var label = new StaticText (topFrame);
					label.Text = Res.Strings.Dialog.Icon.Label.Filter;
					label.PreferredWidth = 64;
					label.Dock = DockStyle.Left;

					this.fieldFilter = new TextFieldCombo (topFrame);
					this.fieldFilter.Text = Res.Strings.Dialog.Icon.Filter.All;
					this.fieldFilter.IsReadOnly = true;
					this.fieldFilter.Dock = DockStyle.Fill;
					this.fieldFilter.ComboClosed += this.HandleFieldFilterComboClosed;
				}

				//	Tableau principal.
				this.arrayDetail = new MyWidgets.StringArray (this.window.Root);
				this.arrayDetail.Columns = 2;
				this.arrayDetail.SetColumnsRelativeWidth (0, 0.15);  // icône
				this.arrayDetail.SetColumnsRelativeWidth (1, 0.85);  // nom du module.icône
				this.arrayDetail.SetColumnAlignment (0, ContentAlignment.MiddleCenter);
				this.arrayDetail.SetColumnBreakMode (1, TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine);
				this.arrayDetail.LineHeight = 25;
				this.arrayDetail.Dock = DockStyle.Fill;
				this.arrayDetail.CellCountChanged += this.HandleArrayCellCountChanged;
				this.arrayDetail.CellsContentChanged += this.HandleArrayCellsContentChanged;
				this.arrayDetail.SelectedRowChanged += this.HandleArraySelectedRowChanged;
				this.arrayDetail.SelectedRowDoubleClicked += this.HandleArraySelectedRowDoubleClicked;
				this.arrayDetail.TabIndex = 1;
				this.arrayDetail.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.arrayCompact = new MyWidgets.IconArray (this.window.Root);
				this.arrayCompact.CellSize = this.arrayDetail.LineHeight;
				this.arrayCompact.Dock = DockStyle.Fill;
				this.arrayCompact.ChangeSelected += this.HandleArrayCompactChangeSelected;
				this.arrayCompact.TabIndex = 2;
				this.arrayCompact.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip (this.arrayCompact, "*");

				//	Pied.
				Widget footer = new Widget (this.window.Root);
				footer.PreferredHeight = 22;
				footer.Margins = new Margins (0, 0, 8, 0);
				footer.Dock = DockStyle.Bottom;

				Button buttonClose = new Button (footer);
				buttonClose.PreferredWidth = 75;
				buttonClose.Text = Res.Strings.Dialog.Icon.Button.Cancel;
				buttonClose.ButtonStyle = ButtonStyle.DefaultCancel;
				buttonClose.Dock = DockStyle.Right;
				buttonClose.Clicked += this.HandleButtonCloseClicked;
				buttonClose.TabIndex = 11;
				buttonClose.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				Button buttonOk = new Button (footer);
				buttonOk.PreferredWidth = 75;
				buttonOk.Text = Res.Strings.Dialog.Icon.Button.OK;
				buttonOk.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonOk.Dock = DockStyle.Right;
				buttonOk.Margins = new Margins (0, 6, 0, 0);
				buttonOk.Clicked += this.HandleButtonInsertClicked;
				buttonOk.TabIndex = 10;
				buttonOk.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.buttonMode = new IconButton (footer);
				this.buttonMode.IconUri = Misc.Icon ("DialogIconMode");
				this.buttonMode.ButtonStyle = ButtonStyle.ActivableIcon;
				this.buttonMode.Dock = DockStyle.Left;
				this.buttonMode.Margins = new Margins (0, 6, 0, 0);
				this.buttonMode.Clicked += this.HandleButtonModeClicked;
				ToolTip.Default.SetToolTip (this.buttonMode, Res.Strings.Dialog.Icon.Tooltip.Mode);

				this.slider = new HSlider (footer);
				this.slider.PreferredWidth = 80;
				this.slider.Dock = DockStyle.Left;
				this.slider.Margins = new Margins (0, 0, 4, 4);
				this.slider.TabIndex = 9;
				this.slider.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				this.slider.MinValue = 20.0M;
				this.slider.MaxValue = 50.0M;
				this.slider.SmallChange = 1.0M;
				this.slider.LargeChange = 5.0M;
				this.slider.Resolution = 1.0M;
				this.slider.Value = (decimal) this.arrayDetail.LineHeight;
				this.slider.ValueChanged += this.HandleSliderChanged;
				ToolTip.Default.SetToolTip (this.slider, Res.Strings.Dialog.Icon.Tooltip.Size);
			}

			this.fieldSearch.Text = null;

			this.UpdateMode();
			this.UpdateFilter();
			this.UpdateArray();
			this.Selected = this.SelectedIcon(this.icon);
			this.ShowSelection();

			this.fieldSearch.SelectAll ();
			this.fieldSearch.Focus ();

			this.window.ShowDialog ();
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


		private void UpdateMode()
		{
			this.arrayDetail.Visibility = !this.compactMode;
			this.arrayCompact.Visibility = this.compactMode;

			this.buttonMode.ActiveState = this.compactMode ? ActiveState.Yes : ActiveState.No;
		}

		private void UpdateFilter()
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

			string search = null;
			if (this.fieldSearch != null && !string.IsNullOrEmpty (this.fieldSearch.Text))
			{
				search = this.fieldSearch.Text.ToLower ();
			}

			for (int i=0; i<names.Length; i++)
			{
				string module, name;
				Misc.GetIconNames(names[i], out module, out name);

				string mdn = Misc.CompactModuleAndName (module, name, tags: false).ToLower ();
				if (!string.IsNullOrEmpty (search))
				{
					if (!mdn.Contains (search))
					{
						continue;
					}
				}

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

		private void UpdateArray()
		{
			//	Met à jour le tableau des icônes.
			if (this.icons == null)
			{
				return;
			}

			this.arrayDetail.TotalRows = this.icons.Count+1;

			int first = this.arrayDetail.FirstVisibleRow;
			for (int i=0; i<this.arrayDetail.LineCount; i++)
			{
				int row = first+i;

				if (row == 0)  // première ligne 'pas d'icône' ?
				{
					this.arrayDetail.SetLineState (0, row, MyWidgets.StringList.CellState.Normal);
					this.arrayDetail.SetLineState (1, row, MyWidgets.StringList.CellState.Normal);
					this.arrayDetail.SetLineString (0, row, "");
					this.arrayDetail.SetLineString (1, row, Res.Strings.Dialog.Icon.None);
				}
				else if (row-1 < this.icons.Count)
				{
					string icon = this.icons[row-1];
					string text = Misc.ImageFull (icon);

					string module, name;
					Misc.GetIconNames (icon, out module, out name);
					string description = Misc.CompactModuleAndName (module, name);

					this.arrayDetail.SetLineState (0, row, MyWidgets.StringList.CellState.Normal);
					this.arrayDetail.SetLineState (1, row, MyWidgets.StringList.CellState.Normal);
					this.arrayDetail.SetLineString (0, row, text);
					this.arrayDetail.SetLineString (1, row, description);
				}
				else
				{
					this.arrayDetail.SetLineState (0, row, MyWidgets.StringList.CellState.Disabled);
					this.arrayDetail.SetLineState (1, row, MyWidgets.StringList.CellState.Disabled);
					this.arrayDetail.SetLineString (0, row, "");
					this.arrayDetail.SetLineString (1, row, "");
				}
			}

			this.arrayCompact.SetIcons(this.icons);
		}

		private int SelectedIcon(string icon)
		{
			//	Retourne le rang d'une icône dans le tableau.
			if (string.IsNullOrEmpty(icon))
			{
				return 0;  // première ligne 'pas d'icône'
			}

			for (int i=0; i<this.icons.Count; i++)
			{
				if (this.icons[i] == icon)
				{
					return i+1;
				}
			}

			return 0;  // première ligne 'pas d'icône'
		}

		private string SelectedIcon(int sel)
		{
			//	Retourne une icône d'après son rang dans le tableau.
			if (sel < 0)
			{
				return "";
			}
			else if (sel == 0)  // pas d'icône ?
			{
				return "";
			}
			else
			{
				return this.icons[sel-1];
			}
		}

		private int Selected
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

		private void ShowSelection()
		{
			//	Montre la sélection dans un tableau (détaillé ou compact).
			this.arrayDetail.ShowSelectedRow();
			this.arrayCompact.ShowSelectedCell();
		}


		private void HandleFieldSearchTextChanged(object sender)
		{
			this.HandleFieldFilterComboClosed (sender);
		}

		void HandleFieldFilterComboClosed(object sender)
		{
			//	Menu pour choisir le filtre fermé.
			string icon = null;
			int sel = this.Selected;
			if (sel-1 >= 0 && sel-1 < this.icons.Count)
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
			this.icon = this.SelectedIcon(this.Selected);
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
			this.icon = this.SelectedIcon(this.Selected);
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



		private ResourceManager					manager;
		private string							moduleName;
		private List<string>					modules;
		private List<string>					icons;
		private string							icon;
		private bool							compactMode = false;

		private TextField						fieldSearch;
		private TextFieldCombo					fieldFilter;
		private MyWidgets.StringArray			arrayDetail;
		private MyWidgets.IconArray				arrayCompact;
		private HSlider							slider;
		private IconButton						buttonMode;
	}
}
