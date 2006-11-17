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
				this.WindowInit("Open", 350, 250, true);
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
				this.array.SetColumnsRelativeWidth(0, 0.6);
				this.array.SetColumnsRelativeWidth(1, 0.3);
				this.array.SetColumnsRelativeWidth(2, 0.1);
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
			}

			this.UpdateModules();
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

		public ResourceModuleInfo SelectedModule
		{
			//	Retourne les informations sur le module à ouvrir.
			get
			{
				if (this.indexToOpen == -1)
				{
					return new ResourceModuleInfo(null);
				}
				else
				{
					return this.moduleInfos[this.indexToOpen];
				}
			}
		}


		protected void UpdateModules()
		{
			//	Met à jour la liste des modules ouvrables/ouverts.
			this.moduleInfos = new List<ResourceModuleInfo>();

			Resources.DefaultManager.RefreshModuleInfos(this.resourcePrefix);
			foreach (ResourceModuleInfo item in Resources.DefaultManager.GetModuleInfos(this.resourcePrefix))
			{
				this.moduleInfos.Add(item);
			}
		}

		protected void UpdateArray()
		{
			//	Met à jour tout le contenu du tableau.
			this.array.TotalRows = this.moduleInfos.Count;

			int first = this.array.FirstVisibleRow;
			for (int i=0; i<this.array.LineCount; i++)
			{
				if (first+i < this.moduleInfos.Count)
				{
					ModuleState state = this.GetModuleState(first+i);

					if (state == ModuleState.Openable)
					{
						this.array.SetLineState(0, first+i, MyWidgets.StringList.CellState.Normal);
						this.array.SetLineState(1, first+i, MyWidgets.StringList.CellState.Normal);
						this.array.SetLineState(2, first+i, MyWidgets.StringList.CellState.Normal);

						this.array.SetLineString(0, first+i, this.moduleInfos[first+i].Name);
						this.array.SetLineString(1, first+i, Res.Strings.Dialog.Open.State.Openable);
						this.array.SetLineString(2, first+i, Misc.Image("Open"));
					}
					else
					{
						this.array.SetLineState(0, first+i, MyWidgets.StringList.CellState.Warning);
						this.array.SetLineState(1, first+i, MyWidgets.StringList.CellState.Warning);
						this.array.SetLineState(2, first+i, MyWidgets.StringList.CellState.Warning);

						if (state == ModuleState.OpeningAndDirty)
						{
							this.array.SetLineString(0, first+i, Misc.Bold(this.moduleInfos[first+i].Name));
							this.array.SetLineString(2, first+i, Misc.Image("Save"));
						}
						else
						{
							this.array.SetLineString(0, first+i, Misc.Italic(this.moduleInfos[first+i].Name));
							this.array.SetLineString(2, first+i, "");
						}

						this.array.SetLineString(1, first+i, Res.Strings.Dialog.Open.State.Opening);
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
		}

		protected ModuleState GetModuleState(int index)
		{
			//	Retourne l'état d'un module.
			Module module = this.mainWindow.SearchModuleId(this.moduleInfos[index].Id);
			if (module == null)
			{
				return ModuleState.Openable;
			}
			else
			{
				return module.IsDirty ? ModuleState.OpeningAndDirty : ModuleState.Opening;
			}
		}


		protected void HandleArrayCellCountChanged(object sender)
		{
			//	Le nombre de lignes a changé.
			this.UpdateArray();
		}

		protected void HandleArrayCellsContentChanged(object sender)
		{
			//	Le contenu des cellules a changé.
			this.UpdateArray();
		}

		protected void HandleArraySelectedRowChanged(object sender)
		{
			//	La ligne sélectionnée a changé.
			this.UpdateButtons();
		}

		protected void HandleArraySelectedRowDoubleClicked(object sender)
		{
			//	La ligne sélectionnée a été double cliquée.
			this.indexToOpen = this.array.SelectedRow;

			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		protected void HandleWindowCloseClicked(object sender)
		{
			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		protected void HandleButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		protected void HandleButtonOpenClicked(object sender, MessageEventArgs e)
		{
			this.indexToOpen = this.array.SelectedRow;

			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}


		protected string						resourcePrefix;
		protected List<ResourceModuleInfo>		moduleInfos;
		protected Button						buttonOpen;
		protected Button						buttonCancel;
		protected MyWidgets.StringArray			array;
		protected int							indexToOpen;
	}
}
