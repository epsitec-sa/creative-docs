using System.Collections.Generic;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Dialogue permettant de choisir le module � ouvrir.
	/// </summary>
	public class OpenDialog : AbstractDialog
	{
		private enum ModuleState
		{
			Openable,
			Opening,
			OpeningAndDirty,
			Locked,
		}

		public OpenDialog(DesignerApplication designerApplication) : base(designerApplication)
		{
			this.moduleInfosLive   = new Types.Collections.ObservableList<ResourceModuleInfo>();
			this.moduleInfosShowed = new CollectionView(this.moduleInfosLive);
		}

		public override void Show()
		{
			//	Cr�e et montre la fen�tre du dialogue.
			if ( this.window == null )
			{
				this.window = new Window ()
				{
					Icon             = this.designerApplication.Icon,
					PreventAutoClose = true,
					Text             = Res.Strings.Dialog.Open.Title,
					Owner            = this.parentWindow,
				};
				this.window.MakeSecondaryWindow ();
				this.WindowInit ("Open", 600, 440, true);
				this.window.WindowCloseClicked += this.HandleWindowCloseClicked;
				this.window.Root.Padding = new Margins (8);

				var resize=new ResizeKnob (this.window.Root)
				{
					Anchor  = AnchorStyles.BottomRight,
					Margins = new Margins (0, -8, 0, -8)
				};

				ToolTip.Default.SetToolTip (resize, Res.Strings.Dialog.Tooltip.Resize);

				//	Bande horizontale pour la recherche.
				{
					this.filterController = new Controllers.FilterController (OpenDialog.FilterUnifier);
					
					var frame = this.filterController.CreateUI (this.window.Root);
					frame.Margins = new Margins (0, 0, 0, 8);
					frame.TabIndex = 1;
					
					this.filterController.FilterChanged += new EventHandler (this.HandleFilterControllerChanged);
				}

				//	Tableau principal.
				var st = new StructuredType ();
				st.Fields.Add ("Name",  StringType.NativeDefault);
				st.Fields.Add ("Id",    StringType.NativeDefault);
				st.Fields.Add ("State", StringType.NativeDefault);
				st.Fields.Add ("Icon",  StringType.NativeDefault);
				st.Fields.Add ("Patch", StringType.NativeDefault);

				this.table = new UI.ItemTable (this.window.Root)
				{
					SourceType        = st,
					HeaderVisibility  = true,
					FrameVisibility   = true,
					TabIndex          = 2,
					TabNavigationMode = TabNavigationMode.ActivateOnTab,
					Dock              = DockStyle.Fill,
				};
				this.table.ItemPanel.CustomItemViewFactoryGetter = this.ItemViewFactoryGetter;
				this.table.Items = this.moduleInfosShowed;
				this.table.Columns.Add ("Name",  400);
				this.table.Columns.Add ("Id",     35);
				this.table.Columns.Add ("State",  70);
				this.table.Columns.Add ("Icon",   30);
				this.table.Columns.Add ("Patch",  30);
				this.table.ColumnHeader.SetColumnText (0, "Nom");
				this.table.ColumnHeader.SetColumnText (1, "No");
				this.table.ColumnHeader.SetColumnText (2, "Etat");
				this.table.ColumnHeader.SetColumnText (3, "");
				this.table.ColumnHeader.SetColumnText (4, "");
				this.table.ColumnHeader.SetColumnComparer (0, this.CompareName);
				this.table.ColumnHeader.SetColumnComparer (1, this.CompareId);
				this.table.ColumnHeader.SetColumnComparer (2, this.CompareState);
				this.table.ColumnHeader.SetColumnComparer (3, this.CompareIcon);
				this.table.ColumnHeader.SetColumnComparer (4, this.ComparePatch);
				this.table.ItemPanel.Layout                  = UI.ItemPanelLayout.VerticalList;
				this.table.ItemPanel.ItemSelectionMode       = UI.ItemPanelSelectionMode.ExactlyOne;
				this.table.ItemPanel.CurrentItemTrackingMode = UI.CurrentItemTrackingMode.AutoSelectAndDeselect;
				this.table.ItemPanel.SelectionChanged += new EventHandler<UI.ItemPanelSelectionChangedEventArgs> (this.HandleTableSelectionChanged);
				this.table.ItemPanel.DoubleClicked    += new EventHandler<MessageEventArgs> (this.HandleTableDoubleClicked);

				//	Boutons de fermeture.
				var footer = new Widget (this.window.Root)
				{
					PreferredHeight   = 22,
					Margins           = new Margins (0, 0, 8, 0),
					Dock              = DockStyle.Bottom,
					TabIndex          = 3,
					TabNavigationMode = TabNavigationMode.ForwardTabPassive
				};

				this.checkOpened = new CheckButton (footer)
				{
					AutoToggle        = false,
					Text              = "Modules ouverts",
					PreferredWidth    = 110,
					Dock              = DockStyle.Left,
					Margins           = new Margins (0, 0, 0, 0),
					TabIndex          = 8,
					TabNavigationMode = TabNavigationMode.ActivateOnTab,
				};
				this.checkOpened.Clicked += this.HandleCheckClicked;

				this.checkLocked = new CheckButton (footer)
				{
					AutoToggle        = false,
					Text              = "Modules bloqu�s",
					PreferredWidth    = 115,
					Dock              = DockStyle.Left,
					Margins           = new Margins (0, 0, 0, 0),
					TabIndex          = 9,
					TabNavigationMode = TabNavigationMode.ActivateOnTab,
				};
				this.checkLocked.Clicked += this.HandleCheckClicked;

				this.checkSecondary = new CheckButton (footer)
				{
					AutoToggle        = false,
					Text              = "Modules secondaires",
					PreferredWidth    = 130,
					Dock              = DockStyle.Left,
					Margins           = new Margins (0, 0, 0, 0),
					TabIndex          = 9,
					TabNavigationMode = TabNavigationMode.ActivateOnTab,
				};
				this.checkSecondary.Clicked += this.HandleCheckClicked;

				this.buttonCancel = new Button (footer)
				{
					PreferredWidth    = 75,
					Text              = Res.Strings.Dialog.Button.Cancel,
					ButtonStyle       = ButtonStyle.DefaultCancel,
					Dock              = DockStyle.Right,
					TabIndex          = 11,
					TabNavigationMode = TabNavigationMode.ActivateOnTab,
				};
				this.buttonCancel.Clicked += this.HandleButtonCloseClicked;

				this.buttonOpen = new Button (footer)
				{
					PreferredWidth    = 75,
					Text              = Res.Strings.Dialog.Open.Button.Open,
					ButtonStyle       = ButtonStyle.DefaultAccept,
					Dock              = DockStyle.Right,
					Margins           = new Margins (0, 6, 0, 0),
					TabIndex          = 10,
					TabNavigationMode = TabNavigationMode.ActivateOnTab,
				};
				this.buttonOpen.Clicked += this.HandleButtonOpenClicked;
			}

			this.filterController.ClearFilter ();

			this.UpdateModules(true);
			this.moduleInfosShowed.MoveCurrentToPosition(-1);
			this.UpdateButtons();

			this.window.Root.SetFocusOnTabWidget ();
			this.window.ShowDialog();

			this.filterController.SetFocus ();
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


		private void UpdateModules(bool scan)
		{
			//	Met � jour la liste des modules ouvrables/ouverts/bloqu�s.
			if (scan)
			{
				this.designerApplication.ResourceManagerPool.ScanForAllModules ();
				this.moduleInfosAll = Collection.ToList (this.designerApplication.ResourceManagerPool.Modules);
			}

			//	Construit une liste r�duite contenant uniquement les modules visibles dans la liste.
			using (this.moduleInfosShowed.DeferRefresh ())
			{
				this.moduleInfosLive.Clear ();
				for (int i=0; i<this.moduleInfosAll.Count; i++)
				{
					ResourceModuleInfo moduleInfo = this.moduleInfosAll[i];
					ModuleState state = this.GetModuleState (moduleInfo);

					if (this.IsFiltered (moduleInfo))
					{
						continue;
					}

					if (state == ModuleState.Openable)
					{
						this.moduleInfosLive.Add (moduleInfo);
					}
					else if ((state == ModuleState.Opening || state == ModuleState.OpeningAndDirty) && this.showOpened)
					{
						this.moduleInfosLive.Add (moduleInfo);
					}
					else if (state == ModuleState.Locked && this.showLocked)
					{
						this.moduleInfosLive.Add (moduleInfo);
					}
				}

				//	Trie la liste des modules visibles.
				this.moduleInfosLive.Sort (new Comparers.ResourceModuleInfoToOpen ());
			}
		}

		private void UpdateButtons()
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

			this.checkOpened.ActiveState    = this.showOpened    ? ActiveState.Yes : ActiveState.No;
			this.checkLocked.ActiveState    = this.showLocked    ? ActiveState.Yes : ActiveState.No;
			this.checkSecondary.ActiveState = this.showSecondary ? ActiveState.Yes : ActiveState.No;
		}

		private int CompareName(object a, object b)
		{
			ResourceModuleInfo itemA = a as ResourceModuleInfo;
			ResourceModuleInfo itemB = b as ResourceModuleInfo;

			string sA = this.GetColumnText(itemA, "Name");
			string sB = this.GetColumnText(itemB, "Name");

			return sA.CompareTo(sB);
		}

		private int CompareState(object a, object b)
		{
			ResourceModuleInfo itemA = a as ResourceModuleInfo;
			ResourceModuleInfo itemB = b as ResourceModuleInfo;

			string sA = this.GetColumnText(itemA, "State");
			string sB = this.GetColumnText(itemB, "State");

			return sA.CompareTo(sB);
		}

		private int CompareId(object a, object b)
		{
			ResourceModuleInfo itemA = a as ResourceModuleInfo;
			ResourceModuleInfo itemB = b as ResourceModuleInfo;

			int idA = itemA.FullId.Id;
			int idB = itemB.FullId.Id;

			return idA.CompareTo(idB);
		}

		private int CompareIcon(object a, object b)
		{
			ResourceModuleInfo itemA = a as ResourceModuleInfo;
			ResourceModuleInfo itemB = b as ResourceModuleInfo;

			string sA = this.GetColumnText(itemA, "SortedIcon");
			string sB = this.GetColumnText(itemB, "SortedIcon");

			return sA.CompareTo(sB);
		}

		private int ComparePatch(object a, object b)
		{
			ResourceModuleInfo itemA = a as ResourceModuleInfo;
			ResourceModuleInfo itemB = b as ResourceModuleInfo;

			string sA = this.GetColumnText(itemA, "Patch");
			string sB = this.GetColumnText(itemB, "Patch");

			return sA.CompareTo(sB);
		}

		private string GetColumnText(ResourceModuleInfo item, string columnName)
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

				var result = DesignerApplication.IsOriginalModule(item.FullId);
				if (result == 1)
				{
					text = Misc.Bold (text);
				}
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

			if (columnName == "Patch")
			{
				if (item.IsPatchModule)
				{
					text = "P";
				}
				else
				{
					text = "";
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

		private string GetModulePath(ResourceModuleInfo info)
		{
			//	Retourne le nom du chemin d'un module.
			return Misc.GetModulePath (this.designerApplication, info);
		}

		private ModuleState GetModuleState(ResourceModuleInfo info)
		{
			//	Retourne l'�tat d'un module.
			Module module = this.designerApplication.SearchModuleId(info.FullId);

			if (module == null)
			{
				return this.IsModuleAlreadyOpened(info.FullId.Id) ? ModuleState.Locked : ModuleState.Openable;
			}
			else
			{
				return module.IsGlobalDirty ? ModuleState.OpeningAndDirty : ModuleState.Opening;
			}
		}

		private bool IsModuleAlreadyOpened(int id)
		{
			List<Module> modules = this.designerApplication.Modules;

			foreach (Module module in modules)
			{
				if (module.ModuleId.Id == id)
				{
					return true;
				}
			}
			
			return false;
		}


		private void HandleFilterControllerChanged(object sender)
		{
			this.UpdateModules (false);
		}

		private void HandleTableDoubleClicked(object sender, MessageEventArgs e)
		{
			this.parentWindow.MakeActive ();
			this.window.Hide ();
			this.OnClosed ();
		}

		private void HandleTableSelectionChanged(object sender, UI.ItemPanelSelectionChangedEventArgs e)
		{
			//	La ligne s�lectionn�e dans le tableau a chang�.
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

			if (sender == this.checkSecondary)
			{
				this.showSecondary = !this.showSecondary;
			}

			this.UpdateModules (false);
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


		private bool IsFiltered(ResourceModuleInfo moduleInfo)
		{
			int result = DesignerApplication.IsOriginalModule (moduleInfo.FullId);

			if (result == -1)
			{
				return true;
			}

			if (this.showSecondary == false)
			{
				if (result != 1)
				{
					return true;
				}
			}

			string name = this.GetModulePath (moduleInfo);
			return this.filterController.IsFiltered (name);
		}

		private static string FilterUnifier(string text)
		{
			return text.Replace ('\\', '/');
		}


		private UI.IItemViewFactory ItemViewFactoryGetter(UI.ItemView itemView)
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
			public ItemViewFactory(OpenDialog owner)
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
				else if (name == "Icon" || name == "Patch")
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

			OpenDialog owner;
		}


		private string									resourcePrefix;
		private IList<ResourceModuleInfo>				moduleInfosAll;
		private CollectionView							moduleInfosShowed;
		private Types.Collections.ObservableList<ResourceModuleInfo> moduleInfosLive;
		private UI.ItemTable							table;
		private Button									buttonOpen;
		private Button									buttonCancel;
		private CheckButton								checkOpened;
		private CheckButton								checkLocked;
		private CheckButton								checkSecondary;
		private bool									showOpened;
		private bool									showLocked;
		private bool									showSecondary;

		private Controllers.FilterController			filterController;
	}
}
