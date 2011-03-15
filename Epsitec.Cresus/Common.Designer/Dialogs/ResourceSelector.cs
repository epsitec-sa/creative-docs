using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Dialogue permettant de choisir une ressource de type quelconque.
	/// </summary>
	public class ResourceSelector : Abstract
	{
		public enum Operation
		{
			Selection,
			TypesOrEntities,
			InheritEntities,
			InterfaceEntities,
			Entities,
			Form,
		}


		public ResourceSelector(DesignerApplication designerApplication) : base(designerApplication)
		{
			this.allModules = new List<Module> ();
			this.allIndexesInModules = new List<int> ();
		}

		public override void Show()
		{
			//	Crée et montre la fenêtre du dialogue.
			if (this.window == null)
			{
				this.window = new Window();
				this.window.Icon = this.designerApplication.Icon;
				this.window.MakeSecondaryWindow ();
				this.window.PreventAutoClose = true;
				this.WindowInit("ResourceSelector", 560, 306, true);
				this.window.Text = Res.Strings.Dialog.ResourceSelector.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += this.HandleWindowCloseClicked;
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				ResizeKnob resize = new ResizeKnob(this.window.Root);
				resize.Anchor = AnchorStyles.BottomRight;
				resize.Margins = new Margins(0, -8, 0, -8);
				ToolTip.Default.SetToolTip(resize, Res.Strings.Dialog.Tooltip.Resize);

				this.CreateUI (this.window.Root);

				//	Boutons de fermeture.
				var footer = new Widget (this.window.Root);
				footer.PreferredHeight = 22;
				footer.Margins = new Margins (0, 0, 8, 0);
				footer.Dock = DockStyle.Bottom;

				this.buttonCancel = new Button (footer);
				this.buttonCancel.PreferredWidth = 75;
				this.buttonCancel.Text = Res.Strings.Dialog.Button.Cancel;
				this.buttonCancel.ButtonStyle = ButtonStyle.DefaultCancel;
				this.buttonCancel.Dock = DockStyle.Right;
				this.buttonCancel.Clicked += this.HandleButtonCloseClicked;
				this.buttonCancel.TabIndex = 201;
				this.buttonCancel.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.buttonUse = new Button (footer);
				this.buttonUse.PreferredWidth = 75;
				this.buttonUse.Text = Res.Strings.Dialog.ResourceSelector.Button.Use;
				this.buttonUse.ButtonStyle = ButtonStyle.DefaultAccept;
				this.buttonUse.Dock = DockStyle.Right;
				this.buttonUse.Margins = new Margins (0, 6, 0, 0);
				this.buttonUse.Clicked += this.HandleButtonUseClicked;
				this.buttonUse.TabIndex = 200;
				this.buttonUse.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			}

			this.Update ();

			this.window.ShowDialog();

			this.filterController.ClearFilter ();
			this.filterController.SetFocus ();
		}

		public void CreateUI(Widget parent)
		{
			//	Titre supérieur.
			int tabIndex = 1;

			var mainPane = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
			};

			Widget header = new Widget (mainPane);
			header.PreferredHeight = 34;
			header.Dock = DockStyle.Top;

			this.title = new StaticText (header);
			this.title.ContentAlignment = ContentAlignment.TopLeft;
			this.title.Dock = DockStyle.Fill;

			this.radioEntities = new RadioButton (header);
			this.radioEntities.Text = "Entités";
			this.radioEntities.Name = "Entities";
			this.radioEntities.PreferredWidth = 70;
			this.radioEntities.Dock = DockStyle.Right;
			this.radioEntities.Clicked += this.HandleRadioClicked;
			this.radioEntities.TabIndex = tabIndex++;

			this.radioTypes = new RadioButton (header);
			this.radioTypes.Text = "Types";
			this.radioTypes.Name = "Types";
			this.radioTypes.PreferredWidth = 70;
			this.radioTypes.Dock = DockStyle.Right;
			this.radioTypes.Clicked += this.HandleRadioClicked;
			this.radioTypes.TabIndex = tabIndex++;

			this.checkInterface = new CheckButton (header);
			this.checkInterface.AutoToggle = false;
			this.checkInterface.Text = "<font size=\"130%\"><b>Interface</b></font>";
			this.checkInterface.Name = "Interface";
			this.checkInterface.PreferredWidth = 110;
			this.checkInterface.Dock = DockStyle.Left;
			this.checkInterface.Clicked += this.HandleRadioClicked;
			this.checkInterface.TabIndex = tabIndex++;

			this.radioAlone = new RadioButton (header);
			this.radioAlone.Text = "<font size=\"130%\"><b>Pas d'héritage</b></font>";
			this.radioAlone.Name = "Alone";
			this.radioAlone.PreferredWidth = 130;
			this.radioAlone.Dock = DockStyle.Left;
			this.radioAlone.Clicked += this.HandleRadioClicked;
			this.radioAlone.TabIndex = tabIndex++;

			this.radioInherit = new RadioButton (header);
			this.radioInherit.Text = "<font size=\"130%\"><b>Hérite de l'entité ci-dessous :</b></font>";
			this.radioInherit.Name = "Inherit";
			this.radioInherit.PreferredWidth = 240;
			this.radioInherit.Dock = DockStyle.Left;
			this.radioInherit.Clicked += this.HandleRadioClicked;
			this.radioInherit.TabIndex = tabIndex++;

			Separator sep = new Separator (mainPane);  // trait horizontal de séparation
			sep.PreferredHeight = 1;
			sep.Dock = DockStyle.Top;

			//	Corps principal.
			Widget body = new Widget (mainPane);
			body.Dock = DockStyle.Fill;
			body.TabIndex = tabIndex++;

			this.leftContainer = new FrameBox (body);
			this.leftContainer.PreferredWidth = 200;
			this.leftContainer.MinWidth = 150;
			this.leftContainer.MinHeight = 100;
			this.leftContainer.Dock = DockStyle.Left;
			this.leftContainer.TabIndex = tabIndex++;

			this.splitter = new VSplitter (body);
			this.splitter.Dock = DockStyle.Left;
			this.splitter.Margins = new Margins (8, 8, 0, 0);

			this.rightContainer = new FrameBox (body);
			this.rightContainer.MinWidth = 150;
			this.rightContainer.MinHeight = 100;
			this.rightContainer.Dock = DockStyle.Fill;
			this.rightContainer.TabIndex = tabIndex++;

			//	Partie gauche.
			var header1 = new StaticText (this.leftContainer);
			header1.Text = "<font size=\"150%\"><b>Modules</b></font>";
			header1.Dock = DockStyle.Top;
			header1.Margins = new Margins (0, 0, 5, 5);

			this.allModulesButton = new CheckButton (this.leftContainer);
			this.allModulesButton.Text = "Tous les modules";
			this.allModulesButton.AutoToggle = false;
			this.allModulesButton.Dock = DockStyle.Top;
			this.allModulesButton.Margins = new Margins (0, 0, 8+3, 8+2);
			this.allModulesButton.Clicked += new EventHandler<MessageEventArgs> (this.HandleAllModulesButtonClicked);
			this.allModulesButton.TabIndex = tabIndex++;

			this.listModules = new ScrollList (this.leftContainer);
			this.listModules.Dock = DockStyle.Fill;
			this.listModules.Margins = new Margins (0, 0, 0, 8);
			this.listModules.TabIndex = tabIndex++;
			this.listModules.SelectedItemChanged += this.HandleListModulesSelected;
			this.listModules.TabIndex = tabIndex++;

			//	Partie droite.
			var header2 = new StaticText (this.rightContainer);
			header2.Text = "<font size=\"150%\"><b>Ressources</b></font>";
			header2.Dock = DockStyle.Top;
			header2.Margins = new Margins (0, 0, 5, 5);

			//	Bande horizontale pour la recherche.
			{
				this.filterController = new Controllers.FilterController ();

				var frame = this.filterController.CreateUI (this.rightContainer);
				frame.Margins = new Margins (0, 0, 8, 8);
				frame.TabIndex = tabIndex++;
				
				this.filterController.FilterChanged += new EventHandler (this.HandleFilterControllerChanged);
			}

			this.listResources = new ScrollList (this.rightContainer);
			this.listResources.Dock = DockStyle.Fill;
			this.listResources.Margins = new Margins (0, 0, 0, 8);
			this.listResources.TabIndex = tabIndex++;
			this.listResources.SelectedItemChanged += this.HandleListResourcesSelected;
			this.listResources.DoubleClicked += this.HandleListResourcesDoubleClicked;
			this.listResources.TabIndex = tabIndex++;

			//	Pied.
			this.buttonIsNullable = new CheckButton (mainPane);
			this.buttonIsNullable.Text = "Accepte d'être nul";
			this.buttonIsNullable.Dock = DockStyle.Bottom;
			this.buttonIsNullable.Margins = new Margins (0, 0, 5, 0);
			this.buttonIsNullable.TabIndex = tabIndex++;
			this.buttonIsNullable.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.buttonIsNullable.TabIndex = tabIndex++;

			sep = new Separator (mainPane);  // trait horizontal de séparation
			sep.PreferredHeight = 1;
			sep.Dock = DockStyle.Bottom;
		}

		public void Update()
		{
			this.result = Common.Dialogs.DialogResult.Cancel;
			this.closed = false;

			this.UpdateTitle ();
			this.UpdateArray ();
			this.UpdateButtons ();
			this.UpdateRadios ();
			this.UpdateInherit ();
		}


		public void AccessOpen(Operation operation, Module baseModule, ResourceAccess.Type type, Druid resource, bool isNullable, List<Druid> exclude, Druid typeId)
		{
			//	Début de l'accès aux ressources pour le dialogue.
			//	Le type peut être inconnu ou la ressource inconnue, mais pas les deux.
			System.Diagnostics.Debug.Assert(type == ResourceAccess.Type.Unknown || type == ResourceAccess.Type.Captions || type == ResourceAccess.Type.Fields || type == ResourceAccess.Type.Commands || type == ResourceAccess.Type.Types || type == ResourceAccess.Type.Entities || type == ResourceAccess.Type.Panels || type == ResourceAccess.Type.Forms);
			System.Diagnostics.Debug.Assert(resource.Type != Common.Support.DruidType.ModuleRelative);

			this.operation = operation;
			this.resourceType = type;
			this.resource = resource;
			this.isNullable = isNullable;
			this.exclude = exclude;
			this.isInherit = false;

			//	Cherche le module contenant le Druid de la ressource.
			this.baseModule = baseModule;
			this.module = this.designerApplication.SearchModule(this.resource);

			if (this.module == null)  // module inconnu ?
			{
				if (this.lastModule == null)
				{
					this.module = this.baseModule;  // utilise le module de base
				}
				else
				{
					this.module = this.lastModule;  // utilise le dernier module utilisé
				}
			}

			if (this.operation == Operation.Form)
			{
				//	Met dans la liste this.listTypeId le Druid de l'entité de base ainsi que tous les Druids
				//	des entités dont l'entité de base hérite.
				this.typeIds = this.module.AccessEntities.GetInheriedEntities(typeId);
			}
			else if (this.operation == Operation.Entities)
			{
				this.typeIds = null;

				if (!typeId.IsEmpty)
				{
					//	Dans ce cas, typeId correspond au Druid du Form delta. Il ne faudra donc lister
					//	que les entités qui héritent de l'entité de base de ce masque.
					FormEngine.FormDescription form = this.module.AccessForms.GetForm(typeId);
					if (form != null)
					{
						this.typeIds = this.module.AccessEntities.GetInheritedEntitiesBack(form.EntityId);
					}
				}
			}
			else
			{
				this.typeIds = null;
			}

			this.UpdateAccess();

			if (this.operation == Operation.TypesOrEntities)
			{
				if (this.resource.IsValid)
				{
					CultureMap cultureMap = this.access.Accessor.Collection[this.resource];
					if (cultureMap == null)
					{
						this.resourceType = ResourceAccess.Type.Entities;
						this.UpdateAccess();
					}
				}
				else
				{
					this.BestAccess();
				}
			}
		}

		public Druid SelectedResource
		{
			get
			{
				return this.selectedResource;
			}
		}

		public StructuredTypeClass StructuredTypeClass
		{
			get
			{
				return this.typeClass;
			}
			set
			{
				this.typeClass = value;
			}
		}

		public bool IsNullable
		{
			get
			{
				return this.buttonIsNullable.ActiveState == ActiveState.Yes;
			}
		}

		private void AccessChange(Module module)
		{
			//	Change l'accès aux ressources dans un autre module.
			this.module = module;
			this.lastModule = module;

			this.UpdateAccess();
		}

		private bool BestAccess()
		{
			//	Si le type courant ne contient aucune ressource, mais que l'autre type en contient,
			//	bascule sur l'autre type (Types ou Entities). L'idée est d'anticiper sur l'utilisateur,
			//	qui voudra vraissemblablement changer de type s'il voit une liste vide.
			if (this.operation == Operation.TypesOrEntities)
			{
				int totalTypes = this.module.AccessTypes.Accessor.Collection.Count;
				int totalEntities = this.module.AccessEntities.Accessor.Collection.Count;

				if (this.resourceType == ResourceAccess.Type.Types && totalTypes == 0 && totalEntities > 0)
				{
					this.resourceType = ResourceAccess.Type.Entities;
					this.UpdateAccess();
					return true;
				}

				if (this.resourceType == ResourceAccess.Type.Entities && totalEntities == 0 && totalTypes > 0)
				{
					this.resourceType = ResourceAccess.Type.Types;
					this.UpdateAccess();
					return true;
				}
			}

			return false;
		}

		private void UpdateAccess()
		{
			if (this.resourceType == ResourceAccess.Type.Panels)
			{
				this.access = this.module.AccessPanels;
			}
			else
			{
				this.access = this.module.GetAccess(this.resourceType);
			}

			this.collectionView = new CollectionView(this.access.Accessor.Collection);
			this.collectionView.Filter = this.CollectionViewFilter;
			this.collectionView.SortDescriptions.Add(new SortDescription("Name"));
		}

		private bool CollectionViewFilter(object obj)
		{
			//	Méthode passée comme paramètre System.Predicate<object> à CollectionView.Filter.
			//	Retourne false si la ressource doit être exclue.
			CultureMap cultureMap = obj as CultureMap;

			if (this.exclude != null)
			{
				if (this.exclude.Contains(cultureMap.Id))
				{
					return false;  // ne liste pas les Druids exclus
				}
			}

			if (this.operation == Operation.TypesOrEntities && this.resourceType == ResourceAccess.Type.Entities)
			{
				StructuredData data = cultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
				StructuredTypeClass typeClass = (StructuredTypeClass) data.GetValue(Support.Res.Fields.ResourceStructuredType.Class);
				if (typeClass == StructuredTypeClass.Interface)
				{
					return false;  // ne liste pas les interfaces
				}
			}

			if (this.operation == Operation.Entities && this.typeIds != null)
			{
				return this.typeIds.Contains(cultureMap.Id);
			}

			if (this.operation == Operation.InheritEntities)
			{
				StructuredData data = cultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
				StructuredTypeClass typeClass = (StructuredTypeClass) data.GetValue(Support.Res.Fields.ResourceStructuredType.Class);
				if (typeClass == StructuredTypeClass.Interface)
				{
					return false;  // ne liste pas les interfaces
				}
			}

			if (this.operation == Operation.InterfaceEntities)
			{
				StructuredData data = cultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
				StructuredTypeClass typeClass = (StructuredTypeClass) data.GetValue(Support.Res.Fields.ResourceStructuredType.Class);
				if (typeClass != StructuredTypeClass.Interface)
				{
					return false;  // ne liste que les interfaces
				}
			}

			if (this.operation == Operation.Form && this.typeIds != null && this.typeIds.Count != 0)
			{
				if (!this.access.FormSearch(cultureMap, this.typeIds))
				{
					return false;
				}
			}

			return true;
		}
			
		public Common.Dialogs.DialogResult AccessClose()
		{
			//	Fin de l'accès aux ressources pour le dialogue.
			return this.result;
		}


		private void UpdateTitle()
		{
			//	Met à jour le titre qui dépend du type des ressources éditées.
			string name = ResourceAccess.TypeDisplayName(this.resourceType);
			if (this.operation == Operation.InterfaceEntities)
			{
				name = "Interfaces";
			}
			string text = string.Concat("<font size=\"200%\"><b>", name, "</b></font>");
			this.title.Text = text;

			this.listModules.Items.Clear();

			List<Module> list = this.designerApplication.OpeningListModule;
			foreach (Module module in list)
			{
				text = module.ModuleId.Name;

				if (module == this.baseModule)
				{
					text = Misc.Bold(text);
				}

				this.listModules.Items.Add(text);
			}

			text = this.module.ModuleId.Name;

			if (this.module == this.baseModule)
			{
				text = Misc.Bold(text);
			}

			this.ignoreChanged = true;
			this.listModules.SelectedItem = text;
			this.listModules.ShowSelected(ScrollShowMode.Extremity);
			this.ignoreChanged = false;
		}

		private void UpdateArray()
		{
			//	Met à jour tout le contenu du tableau et sélectionne la ressource actuelle.
			this.listResources.Items.Clear();
			this.allModules.Clear ();
			this.allIndexesInModules.Clear ();

			int sel = -1;

			if (ResourceSelector.showAllModules)
			{
				var currentModule = this.module;

				List<Module> list = this.designerApplication.OpeningListModule;
				int index = 0;

				foreach (Module module in list)
				{
					this.AccessChange (module);

					for (int i=0; i<this.collectionView.Items.Count; i++)
					{
						CultureMap cultureMap = this.collectionView.Items[i] as CultureMap;

						string name = Misc.CompactModuleAndName (module.ModuleInfo.FullId.Name, cultureMap.Name, tags: false);
						if (this.IsFiltered (name))
						{
							continue;
						}

						name = Misc.CompactModuleAndName (module.ModuleInfo.FullId.Name, cultureMap.Name);

						this.listResources.Items.Add (name);
						this.allModules.Add (module);
						this.allIndexesInModules.Add (i);

						if (cultureMap.Id == this.resource)
						{
							sel = index;
						}

						index++;
					}
				}

				this.AccessChange (currentModule);
			}
			else
			{
				for (int i=0; i<this.collectionView.Items.Count; i++)
				{
					CultureMap cultureMap = this.collectionView.Items[i] as CultureMap;

					string name = cultureMap.Name;
					if (this.IsFiltered (name))
					{
						continue;
					}

					this.listResources.Items.Add(name);
					this.allModules.Add (this.module);
					this.allIndexesInModules.Add (i);

					if (cultureMap.Id == this.resource)
					{
						sel = i;
					}
				}
			}

			this.ignoreChanged = true;
			this.listResources.SelectedItemIndex = sel;
			this.listResources.ShowSelected(ScrollShowMode.Extremity);
			this.ignoreChanged = false;
		}

		private bool IsFiltered(string name)
		{
			if (this.filterController.HasFilter)
			{
				string filter = this.filterController.Filter.ToLower ();
				return !name.ToLower ().Contains (filter);
			}
			else
			{
				return false;
			}
		}

		private void UpdateButtons()
		{
			//	Met à jour tous les boutons.
			this.allModulesButton.ActiveState = ResourceSelector.showAllModules ? ActiveState.Yes : ActiveState.No;

			//	Met à jour le bouton "Utiliser".
			if (this.buttonUse != null)
			{
				if (this.operation == Operation.InheritEntities)
				{
					this.buttonUse.Enable = (this.listResources.SelectedItemIndex != -1 || !this.IsInherit);
				}
				else
				{
					this.buttonUse.Enable = (this.listResources.SelectedItemIndex != -1);
				}
			}
		}

		private void UpdateRadios()
		{
			//	Met à jour les boutons radio pour changer le type.
			if (this.operation == Operation.TypesOrEntities)
			{
				this.title.Visibility = true;
				this.radioTypes.Visibility = true;
				this.radioEntities.Visibility = true;
				this.checkInterface.Visibility = false;
				this.radioAlone.Visibility = false;
				this.radioInherit.Visibility = false;

				this.buttonIsNullable.Visibility = true;
				this.buttonIsNullable.ActiveState = this.isNullable ? ActiveState.Yes : ActiveState.No;

				if (this.buttonUse != null)
				{
					this.buttonUse.Text = Res.Strings.Dialog.ResourceSelector.Button.Use;
				}

				this.radioTypes.ActiveState = (this.resourceType == ResourceAccess.Type.Types) ? ActiveState.Yes : ActiveState.No;
				this.radioEntities.ActiveState = (this.resourceType == ResourceAccess.Type.Entities) ? ActiveState.Yes : ActiveState.No;
			}
			else if (this.operation == Operation.InheritEntities)
			{
				this.title.Visibility = false;
				this.radioTypes.Visibility = false;
				this.radioEntities.Visibility = false;
				this.checkInterface.Visibility = true;
				this.radioAlone.Visibility = true;
				this.radioInherit.Visibility = true;
				this.buttonIsNullable.Visibility = false;

				if (this.buttonUse != null)
				{
					this.buttonUse.Text = Res.Strings.Dialog.Button.OK;
				}
			}
			else
			{
				this.title.Visibility = true;
				this.radioTypes.Visibility = false;
				this.radioEntities.Visibility = false;
				this.checkInterface.Visibility = false;
				this.radioAlone.Visibility = false;
				this.radioInherit.Visibility = false;
				this.buttonIsNullable.Visibility = false;

				if (this.buttonUse != null)
				{
					this.buttonUse.Text = Res.Strings.Dialog.ResourceSelector.Button.Use;
				}
			}
		}

		private void UpdateInherit()
		{
			//	Met à jour en fonction du bouton pour l'héritage.
			if (this.operation == Operation.InheritEntities)
			{
				this.ignoreChanged = true;

				if (this.typeClass == StructuredTypeClass.Interface)
				{
					this.checkInterface.ActiveState = ActiveState.Yes;
					this.radioAlone.Enable = false;
					this.radioInherit.Enable = false;
				}
				else
				{
					this.checkInterface.ActiveState = ActiveState.No;
					this.radioAlone.Enable = true;
					this.radioInherit.Enable = true;
				}

				this.radioAlone.ActiveState = this.IsInherit ? ActiveState.No : ActiveState.Yes;
				this.radioInherit.ActiveState = this.IsInherit ? ActiveState.Yes : ActiveState.No;

				this.ignoreChanged = false;

				this.leftContainer.Enable = this.IsInherit;
				this.rightContainer.Enable = this.IsInherit;
				this.listModules.Enable = this.IsInherit && !ResourceSelector.showAllModules;
				this.listResources.Enable = this.IsInherit;
			}
			else
			{
				this.leftContainer.Enable = true;
				this.rightContainer.Enable = true;
				this.listModules.Enable = !ResourceSelector.showAllModules;
				this.listResources.Enable = true;
			}
		}

		private bool IsInherit
		{
			get
			{
				return this.typeClass != StructuredTypeClass.Interface && this.isInherit;
			}
		}


		public void Close()
		{
			//	Ferme proprement le dialogue.
			if (this.closed)
			{
				return;
			}

			this.selectedResource = this.CurrentSelectedResource;

			if (this.collectionView != null)
			{
				this.collectionView.Filter = null;  // pour éviter un appel ultérieur de CollectionViewFilter !
				this.collectionView.Dispose ();
				this.collectionView = null;
			}

			if (this.buttonUse != null)  // mode "dialogue" (par opposition au mode "volet") ?
			{
				this.parentWindow.MakeActive ();
				this.window.Hide ();
				this.OnClosed ();
			}

			this.closed = true;
		}

		private Druid CurrentSelectedResource
		{
			//	Retourne le Druid de la ressource actuellement sélectionnée.
			get
			{
				if (this.operation == Operation.InheritEntities && !this.IsInherit)
				{
					return Druid.Empty;
				}

				if (this.listResources.SelectedItemIndex == -1)
				{
					return Druid.Empty;
				}
				else
				{
					int sel = this.listResources.SelectedItemIndex;

					if (ResourceSelector.showAllModules)
					{
						var module = this.allModules[sel];
						this.AccessChange (module);

						CultureMap cultureMap = this.collectionView.Items[this.allIndexesInModules[sel]] as CultureMap;
						return cultureMap.Id;
					}
					else
					{
						CultureMap cultureMap = this.collectionView.Items[sel] as CultureMap;
						return cultureMap.Id;
					}
				}
			}
		}


		private void HandleRadioClicked(object sender, MessageEventArgs e)
		{
			//	Changement du type des ressources par un bouton radio.
			if (this.ignoreChanged)
			{
				return;
			}

			AbstractButton button = sender as AbstractButton;

			if (button.Name == "Types")
			{
				this.resourceType = ResourceAccess.Type.Types;
				this.UpdateAccess();
				this.UpdateTitle();
				this.UpdateArray();
				this.UpdateButtons();
				this.UpdateRadios();
			}

			if (button.Name == "Entities")
			{
				this.resourceType = ResourceAccess.Type.Entities;
				this.UpdateAccess();
				this.UpdateTitle();
				this.UpdateArray();
				this.UpdateButtons();
				this.UpdateRadios();
			}

			if (button.Name == "Interface")
			{
				if (this.checkInterface.ActiveState == ActiveState.Yes)
				{
					this.checkInterface.ActiveState = ActiveState.No;
					this.typeClass = StructuredTypeClass.Entity;
				}
				else
				{
					this.checkInterface.ActiveState = ActiveState.Yes;
					this.typeClass = StructuredTypeClass.Interface;
				}

				this.UpdateInherit();
				this.UpdateButtons();
			}

			if (button.Name == "Alone")
			{
				this.isInherit = false;
				this.UpdateInherit();
				this.UpdateButtons();
			}

			if (button.Name == "Inherit")
			{
				this.isInherit = true;
				this.UpdateInherit();
				this.UpdateButtons();
			}

		}

		private void HandleAllModulesButtonClicked(object sender, MessageEventArgs e)
		{
			ResourceSelector.showAllModules = !ResourceSelector.showAllModules;

			this.UpdateButtons ();
			this.UpdateRadios();
			this.UpdateTitle();
			this.UpdateArray();
			this.UpdateInherit ();

			this.filterController.SetFocus ();
		}

		private void HandleListModulesSelected(object sender)
		{
			//	Choix d'un module dans la liste.
			if (this.ignoreChanged)
			{
				return;
			}

			string name = this.listModules.SelectedItem;
			if (string.IsNullOrEmpty(name))
			{
				return;
			}

			name = Misc.RemoveTags(name);  // nom sans les tags <b> ou <i>
			Module module = this.designerApplication.SearchModule(name);
			if (module != null)
			{
				this.AccessChange(module);
				if (this.BestAccess())
				{
					this.UpdateRadios();
					this.UpdateTitle();
				}

				this.UpdateArray();
				return;
			}
		}

		private void HandleFilterControllerChanged(object sender)
		{
			this.UpdateArray ();
		}

		private void HandleListResourcesSelected(object sender)
		{
			//	La ressource sélectionnée a changé.
			if (this.ignoreChanged)
			{
				return;
			}

			this.UpdateButtons();
		}

		private void HandleListResourcesDoubleClicked(object sender, MessageEventArgs e)
		{
			//	La liste des ressources a été double-cliquée.
			if (this.buttonUse != null)  // mode "dialogue" (par opposition au mode "volet") ?
			{
				this.Close ();
				this.result = Common.Dialogs.DialogResult.Yes;
			}
		}

		private void HandleWindowCloseClicked(object sender)
		{
			this.Close();
		}

		private void HandleButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.Close();
		}

		private void HandleButtonUseClicked(object sender, MessageEventArgs e)
		{
			this.Close();
			this.result = Common.Dialogs.DialogResult.Yes;
		}


		private static bool						showAllModules;

		private Module							baseModule;
		private Module							lastModule;
		private Module							module;
		private Operation						operation;
		private ResourceAccess.Type				resourceType;
		private StructuredTypeClass				typeClass;
		private bool							isInherit;
		private ResourceAccess					access;
		private Druid							resource;
		private Druid							selectedResource;
		private List<Druid>						typeIds;
		private bool							isNullable;
		private List<Druid>						exclude;
		private CollectionView					collectionView;
		private Common.Dialogs.DialogResult		result;
		private bool							closed;
		private List<Module>					allModules;
		private List<int>						allIndexesInModules;

		private StaticText						title;
		private RadioButton						radioTypes;
		private RadioButton						radioEntities;
		private CheckButton						checkInterface;
		private RadioButton						radioAlone;
		private RadioButton						radioInherit;
		private FrameBox						leftContainer;
		private CheckButton						allModulesButton;
		private ScrollList						listModules;
		private VSplitter						splitter;
		private FrameBox						rightContainer;
		private Controllers.FilterController	filterController;
		private ScrollList						listResources;
		private CheckButton						buttonIsNullable;
		private Button							buttonUse;
		private Button							buttonCancel;
	}
}
