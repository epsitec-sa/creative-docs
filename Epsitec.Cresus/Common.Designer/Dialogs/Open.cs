using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

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

				int tabIndex = 0;

				StaticText label = new StaticText(this.window.Root);
				label.Text = Res.Strings.Dialog.Open.Label;
				label.ContentAlignment = ContentAlignment.MiddleLeft;
				label.Dock = DockStyle.Top;
				label.Margins = new Margins(0, 0, 0, 6);

				//	Tableau principal.
				this.array = new MyWidgets.StringArray(this.window.Root);
				this.array.Columns = 3;
				this.array.SetColumnsRelativeWidth(0, 0.78);
				this.array.SetColumnsRelativeWidth(1, 0.15);
				this.array.SetColumnsRelativeWidth(2, 0.07);
				this.array.SetColumnAlignment(0, ContentAlignment.MiddleLeft);
				this.array.SetColumnAlignment(1, ContentAlignment.MiddleLeft);
				this.array.SetColumnAlignment(2, ContentAlignment.MiddleCenter);
				this.array.LineHeight = 25;
				this.array.CellCountChanged += new EventHandler(this.HandleArrayCellCountChanged);
				this.array.CellsContentChanged += new EventHandler(this.HandleArrayCellsContentChanged);
				this.array.SelectedRowChanged += new EventHandler(this.HandleArraySelectedRowChanged);
				this.array.SelectedRowDoubleClicked += new EventHandler(this.HandleArraySelectedRowDoubleClicked);
				this.array.TabIndex = tabIndex++;
				this.array.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				this.array.Dock = DockStyle.Fill;

				//	Boutons de fermeture.
				Widget footer = new Widget(this.window.Root);
				footer.PreferredHeight = 22;
				footer.Margins = new Margins(0, 0, 8, 0);
				footer.Dock = DockStyle.Bottom;

				this.buttonOpen = new Button(footer);
				this.buttonOpen.PreferredWidth = 75;
				this.buttonOpen.Text = Res.Strings.Dialog.Open.Button.Open;
				this.buttonOpen.ButtonStyle = ButtonStyle.DefaultAccept;
				this.buttonOpen.Dock = DockStyle.Left;
				this.buttonOpen.Margins = new Margins(0, 6, 0, 0);
				this.buttonOpen.Clicked += new MessageEventHandler(this.HandleButtonOpenClicked);
				this.buttonOpen.TabIndex = tabIndex++;
				this.buttonOpen.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.buttonCancel = new Button(footer);
				this.buttonCancel.PreferredWidth = 75;
				this.buttonCancel.Text = Res.Strings.Dialog.Button.Cancel;
				this.buttonCancel.ButtonStyle = ButtonStyle.DefaultCancel;
				this.buttonCancel.Dock = DockStyle.Left;
				this.buttonCancel.Margins = new Margins(0, 6, 0, 0);
				this.buttonCancel.Clicked += new MessageEventHandler(this.HandleButtonCloseClicked);
				this.buttonCancel.TabIndex = tabIndex++;
				this.buttonCancel.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.checkOpened = new CheckButton(footer);
				this.checkOpened.AutoToggle = false;
				this.checkOpened.Text = "Modules ouverts";
				this.checkOpened.PreferredWidth = 110;
				this.checkOpened.Dock = DockStyle.Left;
				this.checkOpened.Margins = new Margins(20, 0, 0, 0);
				this.checkOpened.Clicked += new MessageEventHandler(this.HandleCheckClicked);
				this.checkOpened.TabIndex = tabIndex++;
				this.checkOpened.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.checkLocked = new CheckButton(footer);
				this.checkLocked.AutoToggle = false;
				this.checkLocked.Text = "Modules bloqués";
				this.checkLocked.PreferredWidth = 110;
				this.checkLocked.Dock = DockStyle.Left;
				this.checkLocked.Margins = new Margins(0, 0, 0, 0);
				this.checkLocked.Clicked += new MessageEventHandler(this.HandleCheckClicked);
				this.checkLocked.TabIndex = tabIndex++;
				this.checkLocked.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			}

			this.UpdateModules(true);
			this.UpdateArray();
			this.array.SelectedRow = -1;
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
					return this.moduleInfosShowed[this.indexToOpen].FullId;
				}
			}
		}


		protected void UpdateModules(bool scan)
		{
			//	Met à jour la liste des modules ouvrables/ouverts.
			if (scan)
			{
				this.mainWindow.ResourceManagerPool.ScanForAllModules();
				this.moduleInfosAll = this.mainWindow.ResourceManagerPool.FindReferenceModules();
			}

			this.moduleInfosShowed = new List<ResourceModuleInfo>();
			for (int i=0; i<this.moduleInfosAll.Count; i++)
			{
				ModuleState state = this.GetModuleState(i, this.moduleInfosAll);

				if (state == ModuleState.Openable)
				{
					this.moduleInfosShowed.Add(this.moduleInfosAll[i]);
				}
				else if ((state == ModuleState.Opening || state == ModuleState.OpeningAndDirty) && this.showOpened)
				{
					this.moduleInfosShowed.Add(this.moduleInfosAll[i]);
				}
				else if (state == ModuleState.Locked && this.showLocked)
				{
					this.moduleInfosShowed.Add(this.moduleInfosAll[i]);
				}
			}
		}

		protected void UpdateArray()
		{
			//	Met à jour tout le contenu du tableau.
			this.array.TotalRows = this.moduleInfosShowed.Count;
			ResourceManagerPool pool = this.mainWindow.ResourceManagerPool;

			int first = this.array.FirstVisibleRow;
			for (int i=0; i<this.array.LineCount; i++)
			{
				if (first+i < this.moduleInfosShowed.Count)
				{
					ModuleState state = this.GetModuleState(first+i);

					if (state == ModuleState.Openable)
					{
						this.array.SetLineState(0, first+i, MyWidgets.StringList.CellState.Normal);
						this.array.SetLineState(1, first+i, MyWidgets.StringList.CellState.Normal);
						this.array.SetLineState(2, first+i, MyWidgets.StringList.CellState.Normal);

						this.array.SetLineString(0, first+i, GetModulePath(first+i));
						this.array.SetLineString(1, first+i, Res.Strings.Dialog.Open.State.Openable);
						this.array.SetLineString(2, first+i, Misc.Image("Open"));
					}
					else
					{
						this.array.SetLineState(0, first+i, MyWidgets.StringList.CellState.Disabled);
						this.array.SetLineState(1, first+i, MyWidgets.StringList.CellState.Disabled);
						this.array.SetLineState(2, first+i, MyWidgets.StringList.CellState.Disabled);

						if (state == ModuleState.OpeningAndDirty)
						{
							this.array.SetLineString(0, first+i, Misc.Bold(GetModulePath(first+i)));
							this.array.SetLineString(1, first+i, Res.Strings.Dialog.Open.State.Opening);
							this.array.SetLineString(2, first+i, Misc.Image("Save"));
						}
						else if (state == ModuleState.Locked)
						{
							this.array.SetLineString(0, first+i, Misc.Italic(GetModulePath(first+i)));
							this.array.SetLineString(1, first+i, "Bloqué");
							this.array.SetLineString(2, first+i, Misc.Image("Locked"));
						}
						else
						{
							this.array.SetLineString(0, first+i, Misc.Italic(GetModulePath(first+i)));
							this.array.SetLineString(1, first+i, Res.Strings.Dialog.Open.State.Opening);
							this.array.SetLineString(2, first+i, Misc.Image("Opened"));
						}
					}
				}
				else
				{
					this.array.SetLineState(0, first+i, MyWidgets.StringList.CellState.Disabled);
					this.array.SetLineState(1, first+i, MyWidgets.StringList.CellState.Disabled);
					this.array.SetLineState(2, first+i, MyWidgets.StringList.CellState.Disabled);

					this.array.SetLineString(0, first+i, "");
					this.array.SetLineString(1, first+i, "");
					this.array.SetLineString(2, first+i, "");
				}
			}
		}

		protected void UpdateButtons()
		{
			//	Met à jour tous les boutons.
			int sel = this.array.SelectedRow;

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

		protected string GetModulePath(int index)
		{
			//	Retourne le nom du chemin d'un module.
			ResourceManagerPool pool = this.mainWindow.ResourceManagerPool;
			string path = pool.GetRootRelativePath(this.moduleInfosShowed[index].FullId.Path);

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
			return this.GetModuleState(index, this.moduleInfosShowed);
		}

		protected ModuleState GetModuleState(int index, IList<ResourceModuleInfo> list)
		{
			//	Retourne l'état d'un module.
			Module module = this.mainWindow.SearchModuleId(list[index].FullId);
			if (module == null)
			{
				return this.IsModuleAlreadyOpened(list[index].FullId.Id) ? ModuleState.Locked : ModuleState.Openable;
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
			this.indexToOpen = this.array.SelectedRow;

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
			this.indexToOpen = this.array.SelectedRow;

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
			this.array.SelectedRow = -1;
			this.indexToOpen = -1;
			this.UpdateButtons();
		}


		protected string						resourcePrefix;
		protected IList<ResourceModuleInfo>		moduleInfosAll;
		protected List<ResourceModuleInfo>		moduleInfosShowed;
		protected Button						buttonOpen;
		protected Button						buttonCancel;
		protected CheckButton					checkOpened;
		protected CheckButton					checkLocked;
		protected MyWidgets.StringArray			array;
		protected int							indexToOpen;
		protected bool							showOpened;
		protected bool							showLocked;
	}
}
