using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Dialogue permettant de choisir le module à ouvrir.
	/// </summary>
	public class Open : Abstract
	{
		protected enum ModuleState
		{
			Openable,
			Opening,
			OpeningAndDirty,
			Locked,
		}

		public Open(MainWindow mainWindow) : base(mainWindow)
		{
			this.moduleInfosLive = new Epsitec.Common.Types.Collections.ObservableList<ResourceModuleInfo> ();
			this.moduleInfosShowed = new CollectionView (this.moduleInfosLive);
		}

		public override void Show()
		{
			//	Crée et montre la fenêtre du dialogue.
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.MakeSecondaryWindow();
				this.window.PreventAutoClose = true;
				this.WindowInit("Open", 500, 300, true);
				this.window.Text = Res.Strings.Dialog.Open.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				ResizeKnob resize = new ResizeKnob(this.window.Root);
				resize.Anchor = AnchorStyles.BottomRight;
				resize.Margins = new Margins(0, -8, 0, -8);
				ToolTip.Default.SetToolTip(resize, Res.Strings.Dialog.Tooltip.Resize);

				StaticText label = new StaticText(this.window.Root);
				label.Text = Res.Strings.Dialog.Open.Label;
				label.ContentAlignment = ContentAlignment.MiddleLeft;
				label.Dock = DockStyle.Top;
				label.Margins = new Margins(0, 0, 0, 6);

				//	Tableau principal.
				StructuredType st = new StructuredType();
				st.Fields.Add("Name",  StringType.Default);
				st.Fields.Add("State", StringType.Default);
				st.Fields.Add("Id",    StringType.Default);
				st.Fields.Add("Icon",  StringType.Default);

				this.table = new UI.ItemTable(this.window.Root);
				this.table.ItemPanel.CustomItemViewFactoryGetter = this.ItemViewFactoryGetter;
				this.table.SourceType = st;
				this.table.Items = this.moduleInfosShowed;
				this.table.Columns.Add("Name",  200);
				this.table.Columns.Add("State", 100);
				this.table.Columns.Add("Id",     30);
				this.table.Columns.Add("Icon",   30);
				this.table.ColumnHeader.SetColumnText(0, "Nom");
				this.table.ColumnHeader.SetColumnText(1, "Etat");
				this.table.ColumnHeader.SetColumnText(2, "No");
				this.table.ColumnHeader.SetColumnText(3, "-");
				//?this.table.ColumnHeader.SetColumnSort(0, ListSortDirection.Ascending);
				//?this.table.HorizontalScrollMode = UI.ItemTableScrollMode.None;
				//?this.table.VerticalScrollMode = UI.ItemTableScrollMode.ItemBased;
				this.table.HeaderVisibility = true;
				this.table.FrameVisibility = true;
				this.table.ItemPanel.Layout = UI.ItemPanelLayout.VerticalList;
				this.table.ItemPanel.ItemSelectionMode = UI.ItemPanelSelectionMode.ExactlyOne;
				this.table.ItemPanel.CurrentItemTrackingMode = UI.CurrentItemTrackingMode.AutoSelect;
				this.table.ItemPanel.SelectionChanged += new EventHandler<UI.ItemPanelSelectionChangedEventArgs>(this.HandleTableSelectionChanged);
				this.table.Dock = Widgets.DockStyle.Fill;
				
				//	Boutons de fermeture.
				Widget footer = new Widget(this.window.Root);
				footer.PreferredHeight = 22;
				footer.Margins = new Margins(0, 0, 8, 0);
				footer.Dock = DockStyle.Bottom;

				this.checkOpened = new CheckButton(footer);
				this.checkOpened.AutoToggle = false;
				this.checkOpened.Text = "Modules ouverts";
				this.checkOpened.PreferredWidth = 110;
				this.checkOpened.Dock = DockStyle.Left;
				this.checkOpened.Margins = new Margins(0, 0, 0, 0);
				this.checkOpened.Clicked += new MessageEventHandler(this.HandleCheckClicked);
				this.checkOpened.TabIndex = 8;
				this.checkOpened.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.checkLocked = new CheckButton(footer);
				this.checkLocked.AutoToggle = false;
				this.checkLocked.Text = "Modules bloqués";
				this.checkLocked.PreferredWidth = 110;
				this.checkLocked.Dock = DockStyle.Left;
				this.checkLocked.Margins = new Margins(0, 0, 0, 0);
				this.checkLocked.Clicked += new MessageEventHandler(this.HandleCheckClicked);
				this.checkLocked.TabIndex = 9;
				this.checkLocked.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.buttonCancel = new Button(footer);
				this.buttonCancel.PreferredWidth = 75;
				this.buttonCancel.Text = Res.Strings.Dialog.Button.Cancel;
				this.buttonCancel.ButtonStyle = ButtonStyle.DefaultCancel;
				this.buttonCancel.Dock = DockStyle.Right;
				this.buttonCancel.Clicked += new MessageEventHandler(this.HandleButtonCloseClicked);
				this.buttonCancel.TabIndex = 11;
				this.buttonCancel.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.buttonOpen = new Button(footer);
				this.buttonOpen.PreferredWidth = 75;
				this.buttonOpen.Text = Res.Strings.Dialog.Open.Button.Open;
				this.buttonOpen.ButtonStyle = ButtonStyle.DefaultAccept;
				this.buttonOpen.Dock = DockStyle.Right;
				this.buttonOpen.Margins = new Margins(0, 6, 0, 0);
				this.buttonOpen.Clicked += new MessageEventHandler(this.HandleButtonOpenClicked);
				this.buttonOpen.TabIndex = 10;
				this.buttonOpen.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			}

			this.UpdateModules(true);
			this.UpdateArray();
			//?this.array.SelectedRow = -1;
			this.indexToOpen = -1;
			this.UpdateButtons();

			this.window.ShowDialog();
		}


		public void SetResourcePrefix(string prefix)
		{
			//	Choix du préfixe à utiliser pour liste des modules.
			this.resourcePrefix = prefix;
		}

		public ResourceModuleId SelectedModule
		{
			//	Retourne les informations sur le module à ouvrir.
			get
			{
				if (this.indexToOpen == -1)
				{
					return new ResourceModuleId(null);
				}
				else
				{
					ResourceModuleInfo info = this.moduleInfosShowed.Items[this.indexToOpen] as ResourceModuleInfo;
					return info.FullId;
				}
			}
		}


		protected void UpdateModules(bool scan)
		{
			//	Met à jour la liste des modules ouvrables/ouverts/bloqués.
			if (scan)
			{
				this.mainWindow.ResourceManagerPool.ScanForAllModules();
				this.moduleInfosAll = this.mainWindow.ResourceManagerPool.FindReferenceModules();
			}

			//	Construit une liste réduite contenant uniquement les modules visibles dans la liste.
			using (this.moduleInfosShowed.DeferRefresh ())
			{
				this.moduleInfosLive.Clear ();
				for (int i=0; i<this.moduleInfosAll.Count; i++)
				{
					ModuleState state = this.GetModuleState (this.moduleInfosAll[i]);

					if (state == ModuleState.Openable)
					{
						this.moduleInfosLive.Add (this.moduleInfosAll[i]);
					}
					else if ((state == ModuleState.Opening || state == ModuleState.OpeningAndDirty) && this.showOpened)
					{
						this.moduleInfosLive.Add (this.moduleInfosAll[i]);
					}
					else if (state == ModuleState.Locked && this.showLocked)
					{
						this.moduleInfosLive.Add (this.moduleInfosAll[i]);
					}
				}

				//	Trie la liste des modules visibles.
				this.moduleInfosLive.Sort (new Comparers.ResourceModuleInfoToOpen ());
			}
		}

		protected void UpdateArray()
		{
			//	Met à jour tout le contenu du tableau.
			this.ignoreChange = true;
			this.table.ItemPanel.Refresh();
			this.ignoreChange = false;
		}

		protected void UpdateButtons()
		{
			//	Met à jour tous les boutons.
			//?int sel = this.array.SelectedRow;
			int sel = -1;

			if (sel == -1)
			{
				this.buttonOpen.Enable = false;
			}
			else
			{
				ModuleState state = this.GetModuleState(sel);
				this.buttonOpen.Enable = (state == ModuleState.Openable);
			}

			this.checkOpened.ActiveState = this.showOpened ? ActiveState.Yes : ActiveState.No;
			this.checkLocked.ActiveState = this.showLocked ? ActiveState.Yes : ActiveState.No;
		}

		protected string GetModulePath(ResourceModuleInfo info)
		{
			//	Retourne le nom du chemin d'un module.
			ResourceManagerPool pool = this.mainWindow.ResourceManagerPool;
			string path = pool.GetRootRelativePath(info.FullId.Path);

#if false
			if (path.StartsWith("%app%\\"))
			{
				path = path.Substring(6);
			}
#endif

			return path;
		}

		protected ModuleState GetModuleState(int index)
		{
			//	Retourne l'état d'un module.
			return this.GetModuleState(this.moduleInfosShowed.Items[index] as ResourceModuleInfo);
		}

		protected ModuleState GetModuleState(ResourceModuleInfo info)
		{
			//	Retourne l'état d'un module.
			Module module = this.mainWindow.SearchModuleId(info.FullId);
			if (module == null)
			{
				return this.IsModuleAlreadyOpened(info.FullId.Id) ? ModuleState.Locked : ModuleState.Openable;
			}
			else
			{
				return module.IsDirty ? ModuleState.OpeningAndDirty : ModuleState.Opening;
			}
		}

		protected bool IsModuleAlreadyOpened(int id)
		{
			List<Module> modules = this.mainWindow.Modules;
			foreach (Module module in modules)
			{
				if (module.ModuleInfo.Id == id)
				{
					return true;
				}
			}
			return false;
		}


		private void HandleTableSelectionChanged(object sender, UI.ItemPanelSelectionChangedEventArgs e)
		{
			//	La ligne sélectionnée dans le tableau a changé.
			if (this.ignoreChange)
			{
				return;
			}

			this.UpdateButtons();
		}

		private void HandleArrayCellCountChanged(object sender)
		{
			//	Le nombre de lignes a changé.
			this.UpdateArray();
		}

		private void HandleArrayCellsContentChanged(object sender)
		{
			//	Le contenu des cellules a changé.
			this.UpdateArray();
		}

		private void HandleArraySelectedRowChanged(object sender)
		{
			//	La ligne sélectionnée a changé.
			this.UpdateButtons();
		}

		private void HandleArraySelectedRowDoubleClicked(object sender)
		{
			//	La ligne sélectionnée a été double cliquée.
			//?this.indexToOpen = this.array.SelectedRow;

			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();
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

		private void HandleButtonOpenClicked(object sender, MessageEventArgs e)
		{
			//?this.indexToOpen = this.array.SelectedRow;

			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandleCheckClicked(object sender, MessageEventArgs e)
		{
			if (sender == this.checkOpened)
			{
				this.showOpened = !this.showOpened;
			}

			if (sender == this.checkLocked)
			{
				this.showLocked = !this.showLocked;
			}

			this.UpdateModules(false);
			this.UpdateArray();
			//?this.array.SelectedRow = -1;
			this.indexToOpen = -1;
			this.UpdateButtons();
		}


		protected UI.IItemViewFactory ItemViewFactoryGetter(UI.ItemView itemView)
		{
			if (itemView == null)
			{
				return null;
			}
			else
			{
				return new ItemViewFactory(this);
			}
		}

		private class ItemViewFactory : UI.AbstractItemViewFactory
		{
			//	Cette classe peuple les colonnes du tableau.
			public ItemViewFactory(Open owner)
			{
				this.owner = owner;
			}

			protected override Widget CreateElement(string name, UI.ItemPanel panel, UI.ItemView view, UI.ItemViewShape shape)
			{
				ResourceModuleInfo item = view.Item as ResourceModuleInfo;

				switch (name)
				{
					case "Name":
						return this.CreateName(item);

					case "State":
						return this.CreateState(item);

					case "Id":
						return this.CreateId(item);

					case "Icon":
						return this.CreateIcon(item);
				}

				return null;
			}

			private Widget CreateName(ResourceModuleInfo item)
			{
				ModuleState state = this.owner.GetModuleState(item);
				string path = TextLayout.ConvertToTaggedText((this.owner.GetModulePath(item)));

				if (state == ModuleState.OpeningAndDirty)
				{
					path = Misc.Bold(path);
				}

				if (state == ModuleState.Locked || state == ModuleState.Opening)
				{
					path = Misc.Italic(path);
				}

				StaticText widget = new StaticText();
				widget.Margins = new Margins(5, 5, 0, 0);
				widget.Text = path;
				widget.TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;
				widget.PreferredSize = widget.GetBestFitSize();

				return widget;
			}

			private Widget CreateState(ResourceModuleInfo item)
			{
				ModuleState state = this.owner.GetModuleState(item);

				string text = Res.Strings.Dialog.Open.State.Opening;

				if (state == ModuleState.Openable)
				{
					text = Res.Strings.Dialog.Open.State.Openable;
				}

				if (state == ModuleState.Locked)
				{
					text = "Bloqué";
				}

				StaticText widget = new StaticText();
				widget.Margins = new Margins(5, 5, 0, 0);
				widget.Text = text;
				widget.TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;
				widget.PreferredSize = widget.GetBestFitSize();

				return widget;
			}

			private Widget CreateId(ResourceModuleInfo item)
			{
				string text = item.FullId.Id.ToString();

				StaticText widget = new StaticText();
				widget.Margins = new Margins(5, 5, 0, 0);
				widget.Text = text;
				widget.ContentAlignment = ContentAlignment.MiddleRight;
				widget.TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;
				widget.PreferredSize = widget.GetBestFitSize();

				return widget;
			}

			private Widget CreateIcon(ResourceModuleInfo item)
			{
				ModuleState state = this.owner.GetModuleState(item);
				string text;

				if (state == ModuleState.Openable)
				{
					text = Misc.Image("Open");
				}
				else if (state == ModuleState.OpeningAndDirty)
				{
					text = Misc.Image("Save");
				}
				else if (state == ModuleState.Locked)
				{
					text = Misc.Image("Locked");
				}
				else
				{
					text = Misc.Image("Opened");
				}

				StaticText widget = new StaticText();
				widget.Margins = new Margins(5, 5, 0, 0);
				widget.Text = text;
				widget.ContentAlignment = ContentAlignment.MiddleCenter;
				widget.TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;
				widget.PreferredSize = widget.GetBestFitSize();

				return widget;
			}


			Open owner;
		}

		
		protected string						resourcePrefix;
		protected IList<ResourceModuleInfo>		moduleInfosAll;
		protected CollectionView				moduleInfosShowed;
		protected Epsitec.Common.Types.Collections.ObservableList<ResourceModuleInfo>		moduleInfosLive;
		protected Button						buttonOpen;
		protected Button						buttonCancel;
		protected CheckButton					checkOpened;
		protected CheckButton					checkLocked;
		protected UI.ItemTable					table;
		protected int							indexToOpen;
		protected bool							showOpened;
		protected bool							showLocked;
		protected bool							ignoreChange;
	}
}
