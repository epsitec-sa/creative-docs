using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Dialogue permettant de choisir le module � ouvrir.
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

		public Open(DesignerApplication mainWindow) : base(mainWindow)
		{
			this.moduleInfosLive = new Types.Collections.ObservableList<ResourceModuleInfo>();
			this.moduleInfosShowed = new CollectionView(this.moduleInfosLive);
		}

		public override void Show()
		{
			//	Cr�e et montre la fen�tre du dialogue.
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
				st.Fields.Add("Id",    StringType.Default);
				st.Fields.Add("State", StringType.Default);
				st.Fields.Add("Icon",  StringType.Default);

				this.table = new UI.ItemTable(this.window.Root);
				this.table.ItemPanel.CustomItemViewFactoryGetter = this.ItemViewFactoryGetter;
				this.table.SourceType = st;
				this.table.Items = this.moduleInfosShowed;
				this.table.Columns.Add("Name",  335);
				this.table.Columns.Add("Id",     30);
				this.table.Columns.Add("State",  70);
				this.table.Columns.Add("Icon",   30);
				this.table.ColumnHeader.SetColumnText(0, "Nom");
				this.table.ColumnHeader.SetColumnText(1, "No");
				this.table.ColumnHeader.SetColumnText(2, "Etat");
				this.table.ColumnHeader.SetColumnText(3, "");
				this.table.ColumnHeader.SetColumnComparer(0, this.CompareName);
				this.table.ColumnHeader.SetColumnComparer(1, this.CompareId);
				this.table.ColumnHeader.SetColumnComparer(2, this.CompareState);
				this.table.ColumnHeader.SetColumnComparer(3, this.CompareIcon);
				this.table.HeaderVisibility = true;
				this.table.FrameVisibility = true;
				this.table.ItemPanel.Layout = UI.ItemPanelLayout.VerticalList;
				this.table.ItemPanel.ItemSelectionMode = UI.ItemPanelSelectionMode.ExactlyOne;
				this.table.ItemPanel.CurrentItemTrackingMode = UI.CurrentItemTrackingMode.AutoSelectAndDeselect;
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
				this.checkLocked.Text = "Modules bloqu�s";
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
			this.moduleInfosShowed.MoveCurrentToPosition(-1);
			this.UpdateButtons();

			this.window.ShowDialog();
		}


		public void SetResourcePrefix(string prefix)
		{
			//	Choix du pr�fixe � utiliser pour liste des modules.
			this.resourcePrefix = prefix;
		}

		public ResourceModuleId SelectedModule
		{
			//	Retourne les informations sur le module � ouvrir.
			get
			{
				if (this.moduleInfosShowed.CurrentPosition == -1)
				{
					return new ResourceModuleId(null);
				}
				else
				{
					ResourceModuleInfo info = this.moduleInfosShowed.CurrentItem as ResourceModuleInfo;
					return info.FullId;
				}
			}
		}


		protected void UpdateModules(bool scan)
		{
			//	Met � jour la liste des modules ouvrables/ouverts/bloqu�s.
			if (scan)
			{
				this.mainWindow.ResourceManagerPool.ScanForAllModules();
				this.moduleInfosAll = this.mainWindow.ResourceManagerPool.FindReferenceModules();
			}

			//	Construit une liste r�duite contenant uniquement les modules visibles dans la liste.
			using (this.moduleInfosShowed.DeferRefresh())
			{
				this.moduleInfosLive.Clear();
				for (int i=0; i<this.moduleInfosAll.Count; i++)
				{
					ModuleState state = this.GetModuleState(this.moduleInfosAll[i]);

					if (state == ModuleState.Openable)
					{
						this.moduleInfosLive.Add(this.moduleInfosAll[i]);
					}
					else if ((state == ModuleState.Opening || state == ModuleState.OpeningAndDirty) && this.showOpened)
					{
						this.moduleInfosLive.Add(this.moduleInfosAll[i]);
					}
					else if (state == ModuleState.Locked && this.showLocked)
					{
						this.moduleInfosLive.Add(this.moduleInfosAll[i]);
					}
				}

				//	Trie la liste des modules visibles.
				this.moduleInfosLive.Sort(new Comparers.ResourceModuleInfoToOpen());
			}
		}

		protected void UpdateArray()
		{
			//	Met � jour tout le contenu du tableau.
			//?this.ignoreChange = true;
			//?this.table.ItemPanel.Refresh();
			//?this.ignoreChange = false;
		}

		protected void UpdateButtons()
		{
			//	Met � jour tous les boutons.
			ResourceModuleInfo info = this.moduleInfosShowed.CurrentItem as ResourceModuleInfo;

			if (info == null)
			{
				this.buttonOpen.Enable = false;
			}
			else
			{
				ModuleState state = this.GetModuleState(info);
				this.buttonOpen.Enable = (state == ModuleState.Openable);
			}

			this.checkOpened.ActiveState = this.showOpened ? ActiveState.Yes : ActiveState.No;
			this.checkLocked.ActiveState = this.showLocked ? ActiveState.Yes : ActiveState.No;
		}

		protected int CompareName(object a, object b)
		{
			ResourceModuleInfo itemA = a as ResourceModuleInfo;
			ResourceModuleInfo itemB = b as ResourceModuleInfo;

			string sA = this.GetColumnText(itemA, "Name");
			string sB = this.GetColumnText(itemB, "Name");

			return sA.CompareTo(sB);
		}

		protected int CompareState(object a, object b)
		{
			ResourceModuleInfo itemA = a as ResourceModuleInfo;
			ResourceModuleInfo itemB = b as ResourceModuleInfo;

			string sA = this.GetColumnText(itemA, "State");
			string sB = this.GetColumnText(itemB, "State");

			return sA.CompareTo(sB);
		}

		protected int CompareId(object a, object b)
		{
			ResourceModuleInfo itemA = a as ResourceModuleInfo;
			ResourceModuleInfo itemB = b as ResourceModuleInfo;

			int idA = itemA.FullId.Id;
			int idB = itemB.FullId.Id;

			return idA.CompareTo(idB);
		}

		protected int CompareIcon(object a, object b)
		{
			ResourceModuleInfo itemA = a as ResourceModuleInfo;
			ResourceModuleInfo itemB = b as ResourceModuleInfo;

			string sA = this.GetColumnText(itemA, "SortedIcon");
			string sB = this.GetColumnText(itemB, "SortedIcon");

			return sA.CompareTo(sB);
		}

		protected string GetColumnText(ResourceModuleInfo item, string columnName)
		{
			//	Retourne le texte contenu dans une colonne.
			ModuleState state = this.GetModuleState(item);
			string text = null;

			if (columnName == "Name")
			{
				text = TextLayout.ConvertToTaggedText((this.GetModulePath(item)));

				if (state == ModuleState.OpeningAndDirty)
				{
					text = Misc.Bold(text);
				}

				if (state == ModuleState.Locked || state == ModuleState.Opening)
				{
					text = Misc.Italic(text);
				}
			}

			if (columnName == "State")
			{
				text = Res.Strings.Dialog.Open.State.Opening;

				if (state == ModuleState.Openable)
				{
					text = Res.Strings.Dialog.Open.State.Openable;
				}

				if (state == ModuleState.Locked)
				{
					text = "Bloqu�";
				}
			}

			if (columnName == "Id")
			{
				text = item.FullId.Id.ToString();
			}

			if (columnName == "Icon")
			{
				if (state == ModuleState.Openable)
				{
					text = Misc.Image("Open");  // dossier avec fl�che
				}
				else if (state == ModuleState.OpeningAndDirty)
				{
					text = Misc.Image("Save");  // disquette violette
				}
				else if (state == ModuleState.Opening)
				{
					text = Misc.Image("Opened");  // vu bleu
				}
				else if (state == ModuleState.Locked)
				{
					text = Misc.Image("Locked");  // x bleu
				}
			}

			if (columnName == "SortedIcon")
			{
				if (state == ModuleState.Openable)
				{
					text = "A";
				}
				else if (state == ModuleState.OpeningAndDirty)
				{
					text = "B";
				}
				else if (state == ModuleState.Opening)
				{
					text = "C";
				}
				else if (state == ModuleState.Locked)
				{
					text = "D";
				}
			}

			return text;
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

		protected ModuleState GetModuleState(ResourceModuleInfo info)
		{
			//	Retourne l'�tat d'un module.
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
			//	La ligne s�lectionn�e dans le tableau a chang�.
			if (this.ignoreChange)
			{
				return;
			}

			this.UpdateButtons();
		}

		private void HandleWindowCloseClicked(object sender)
		{
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
			this.UpdateButtons();
		}

		private void HandleButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.moduleInfosShowed.MoveCurrentToPosition(-1);

			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandleButtonOpenClicked(object sender, MessageEventArgs e)
		{
			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();
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
				if (shape == UI.ItemViewShape.ToolTip)
				{
					return null;
				}

				ResourceModuleInfo item = view.Item as ResourceModuleInfo;
				ModuleState state = this.owner.GetModuleState(item);

				UI.ItemViewText main, text;
				if (state == ModuleState.Openable)
				{
					main = text = new UI.ItemViewText();
				}
				else
				{
					main = new UI.ItemViewText();
					main.BackColor = Color.FromAlphaRgb(0.1, 0,0,0);  // fond gris clair

					text = new UI.ItemViewText(main);
					text.Dock = DockStyle.Fill;
				}

				text.Margins = new Margins(5, 5, 0, 0);
				text.Text = this.owner.GetColumnText(item, name);

				if (name == "Id")
				{
					text.ContentAlignment = ContentAlignment.MiddleRight;
				}
				else if (name == "Icon")
				{
					text.ContentAlignment = ContentAlignment.MiddleCenter;
				}
				else
				{
					text.ContentAlignment = ContentAlignment.MiddleLeft;
				}

				text.TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;
				text.PreferredSize = text.GetBestFitSize();

				return main;
			}

			Open owner;
		}

		
		protected string						resourcePrefix;
		protected IList<ResourceModuleInfo>		moduleInfosAll;
		protected CollectionView				moduleInfosShowed;
		protected Types.Collections.ObservableList<ResourceModuleInfo> moduleInfosLive;
		protected UI.ItemTable					table;
		protected Button						buttonOpen;
		protected Button						buttonCancel;
		protected CheckButton					checkOpened;
		protected CheckButton					checkLocked;
		protected bool							showOpened;
		protected bool							showLocked;
		protected bool							ignoreChange;
	}
}
