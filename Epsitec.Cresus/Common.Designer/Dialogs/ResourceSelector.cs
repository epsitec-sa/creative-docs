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
			InheritEntity,
		}


		public ResourceSelector(DesignerApplication designerApplication) : base(designerApplication)
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
				this.WindowInit("ResourceSelector", 500, 300, true);
				this.window.Text = Res.Strings.Dialog.ResourceSelector.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				ResizeKnob resize = new ResizeKnob(this.window.Root);
				resize.Anchor = AnchorStyles.BottomRight;
				resize.Margins = new Margins(0, -8, 0, -8);
				ToolTip.Default.SetToolTip(resize, Res.Strings.Dialog.Tooltip.Resize);

				//	Titre supérieur.
				Widget header = new Widget(this.window.Root);
				header.PreferredHeight = 34;
				header.Dock = DockStyle.Top;

				this.title = new StaticText(header);
				this.title.ContentAlignment = ContentAlignment.TopLeft;
				this.title.Dock = DockStyle.Fill;

				this.radioEntities = new RadioButton(header);
				this.radioEntities.Text = "Entités";
				this.radioEntities.Name = "Entities";
				this.radioEntities.PreferredWidth = 70;
				this.radioEntities.Dock = DockStyle.Right;
				this.radioEntities.Clicked += new MessageEventHandler(this.HandleRadioClicked);

				this.radioTypes = new RadioButton(header);
				this.radioTypes.Text = "Types";
				this.radioTypes.Name = "Types";
				this.radioTypes.PreferredWidth = 70;
				this.radioTypes.Dock = DockStyle.Right;
				this.radioTypes.Clicked += new MessageEventHandler(this.HandleRadioClicked);

				this.checkInterface = new CheckButton(header);
				this.checkInterface.AutoToggle = false;
				this.checkInterface.Text = "<font size=\"130%\"><b>Interface</b></font>";
				this.checkInterface.Name = "Interface";
				this.checkInterface.PreferredWidth = 110;
				this.checkInterface.Dock = DockStyle.Left;
				this.checkInterface.Clicked += new MessageEventHandler(this.HandleRadioClicked);

				this.radioAlone = new RadioButton(header);
				this.radioAlone.Text = "<font size=\"130%\"><b>Pas d'héritage</b></font>";
				this.radioAlone.Name = "Alone";
				this.radioAlone.PreferredWidth = 130;
				this.radioAlone.Dock = DockStyle.Left;
				this.radioAlone.Clicked += new MessageEventHandler(this.HandleRadioClicked);

				this.radioInherit = new RadioButton(header);
				this.radioInherit.Text = "<font size=\"130%\"><b>Hérite de l'entité ci-dessous :</b></font>";
				this.radioInherit.Name = "Inherit";
				this.radioInherit.PreferredWidth = 240;
				this.radioInherit.Dock = DockStyle.Left;
				this.radioInherit.Clicked += new MessageEventHandler(this.HandleRadioClicked);

				Separator sep = new Separator(this.window.Root);  // trait horizontal de séparation
				sep.PreferredHeight = 1;
				sep.Dock = DockStyle.Top;

				//	Corps principal.
				Widget body = new Widget(this.window.Root);
				body.Dock = DockStyle.Fill;

				Widget left = new Widget(body);
				left.MinWidth = 150;
				left.MinHeight = 100;
				left.Dock = DockStyle.Left;

				this.splitter = new VSplitter(body);
				this.splitter.Dock = DockStyle.Left;
				this.splitter.Margins = new Margins(8, 8, 0, 0);

				Widget right = new Widget(body);
				right.MinWidth = 150;
				right.MinHeight = 100;
				right.Dock = DockStyle.Fill;

				//	Partie gauche.
				this.header1 = new StaticText(left);
				this.header1.Text = "Modules";
				this.header1.Dock = DockStyle.Top;
				this.header1.Margins = new Margins(0, 0, 5, 5);

				this.listModules = new ScrollList(left);
				this.listModules.Dock = DockStyle.Fill;
				this.listModules.Margins = new Margins(0, 0, 0, 8);
				this.listModules.TabIndex = 1;
				this.listModules.SelectedIndexChanged += new EventHandler(this.HandleListModulesSelected);

				//	Partie droite.
				this.header2 = new StaticText(right);
				this.header2.Text = "Ressources";
				this.header2.Dock = DockStyle.Top;
				this.header2.Margins = new Margins(0, 0, 5, 5);

				this.listResources = new ScrollList(right);
				this.listResources.Dock = DockStyle.Fill;
				this.listResources.Margins = new Margins(0, 0, 0, 8);
				this.listResources.TabIndex = 2;
				this.listResources.SelectedIndexChanged += new EventHandler(this.HandleListResourcesSelected);
				this.listResources.DoubleClicked += new MessageEventHandler(this.HandleListResourcesDoubleClicked);

				//	Boutons de fermeture.
				Widget footer = new Widget(this.window.Root);
				footer.PreferredHeight = 22;
				footer.Margins = new Margins(0, 0, 8, 0);
				footer.Dock = DockStyle.Bottom;

				this.buttonCancel = new Button(footer);
				this.buttonCancel.PreferredWidth = 75;
				this.buttonCancel.Text = Res.Strings.Dialog.Button.Cancel;
				this.buttonCancel.ButtonStyle = ButtonStyle.DefaultCancel;
				this.buttonCancel.Dock = DockStyle.Right;
				this.buttonCancel.Clicked += new MessageEventHandler(this.HandleButtonCloseClicked);
				this.buttonCancel.TabIndex = 11;
				this.buttonCancel.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.buttonUse = new Button(footer);
				this.buttonUse.PreferredWidth = 75;
				this.buttonUse.Text = Res.Strings.Dialog.ResourceSelector.Button.Use;
				this.buttonUse.ButtonStyle = ButtonStyle.DefaultAccept;
				this.buttonUse.Dock = DockStyle.Right;
				this.buttonUse.Margins = new Margins(0, 6, 0, 0);
				this.buttonUse.Clicked += new MessageEventHandler(this.HandleButtonUseClicked);
				this.buttonUse.TabIndex = 10;
				this.buttonUse.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				sep = new Separator(this.window.Root);  // trait horizontal de séparation
				sep.PreferredHeight = 1;
				sep.Dock = DockStyle.Bottom;

			}

			this.UpdateTitle();
			this.UpdateArray();
			this.UpdateButtons();
			this.UpdateRadios();
			this.UpdateInherit();

			this.window.ShowDialog();
		}


		public void AccessOpen(Operation operation, Module baseModule, ResourceAccess.Type type, Druid resource, List<Druid> exclude)
		{
			//	Début de l'accès aux ressources pour le dialogue.
			//	Le type peut être inconnu ou la ressource inconnue, mais pas les deux.
			System.Diagnostics.Debug.Assert(type == ResourceAccess.Type.Unknow || type == ResourceAccess.Type.Captions || type == ResourceAccess.Type.Fields || type == ResourceAccess.Type.Commands || type == ResourceAccess.Type.Types || type == ResourceAccess.Type.Entities || type == ResourceAccess.Type.Panels);
			System.Diagnostics.Debug.Assert(resource.Type != Common.Support.DruidType.ModuleRelative);

			this.operation = operation;
			this.resourceType = type;
			this.resource = resource;
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

		protected void AccessChange(Module module)
		{
			//	Change l'accès aux ressources dans un autre module.
			this.module = module;
			this.lastModule = module;

			this.UpdateAccess();
		}

		protected bool BestAccess()
		{
			//	Si le type courant ne contient aucune ressource, mais que l'autre type en contient,
			//	bascule sur l'autre type (Types2 ou Entities). L'idée est d'anticiper sur l'utilisateur,
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

		protected void UpdateAccess()
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
			this.collectionView.SortDescriptions.Add(new SortDescription("Name"));
		}

		public Common.Dialogs.DialogResult AccessClose(out Druid resource)
		{
			//	Fin de l'accès aux ressources pour le dialogue.
			resource = this.resource;
			return this.result;
		}


		protected void UpdateTitle()
		{
			//	Met à jour le titre qui dépend du type des ressources éditées.
			string text = string.Concat("<font size=\"200%\"><b>", ResourceAccess.TypeDisplayName(this.resourceType), "</b></font>");
			this.title.Text = text;

			this.listModules.Items.Clear();

			List<Module> list = this.designerApplication.OpeningListModule;
			foreach (Module module in list)
			{
				text = module.ModuleInfo.Name;

				if (module == this.baseModule)
				{
					text = Misc.Bold(text);
				}

				this.listModules.Items.Add(text);
			}

			text = this.module.ModuleInfo.Name;

			if (this.module == this.baseModule)
			{
				text = Misc.Bold(text);
			}

			this.ignoreChanged = true;
			this.listModules.SelectedItem = text;
			this.ignoreChanged = false;
		}

		protected void UpdateArray()
		{
			//	Met à jour tout le contenu du tableau et sélectionne la ressource actuelle.
			this.listResources.Items.Clear();

			int sel = -1;
			for (int i=0; i<this.collectionView.Items.Count; i++)
			{
				CultureMap cultureMap = this.collectionView.Items[i] as CultureMap;

				this.listResources.Items.Add(cultureMap.Name);

				if (cultureMap.Id == this.resource)
				{
					sel = i;
				}
			}

			this.ignoreChanged = true;
			this.listResources.SelectedIndex = sel;
			this.ignoreChanged = false;
		}

		protected void UpdateButtons()
		{
			//	Met à jour le bouton "Utiliser".
			if (this.operation == Operation.InheritEntity)
			{
				this.buttonUse.Enable = (this.listResources.SelectedIndex != -1 || !this.IsInherit);
			}
			else
			{
				this.buttonUse.Enable = (this.listResources.SelectedIndex != -1);
			}
		}

		protected void UpdateRadios()
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

				this.buttonUse.Text = Res.Strings.Dialog.ResourceSelector.Button.Use;

				this.radioTypes.ActiveState = (this.resourceType == ResourceAccess.Type.Types) ? ActiveState.Yes : ActiveState.No;
				this.radioEntities.ActiveState = (this.resourceType == ResourceAccess.Type.Entities) ? ActiveState.Yes : ActiveState.No;
			}
			else if (this.operation == Operation.InheritEntity)
			{
				this.title.Visibility = false;
				this.radioTypes.Visibility = false;
				this.radioEntities.Visibility = false;
				this.checkInterface.Visibility = true;
				this.radioAlone.Visibility = true;
				this.radioInherit.Visibility = true;

				this.buttonUse.Text = Res.Strings.Dialog.Button.OK;
			}
			else
			{
				this.title.Visibility = true;
				this.radioTypes.Visibility = false;
				this.radioEntities.Visibility = false;
				this.checkInterface.Visibility = false;
				this.radioAlone.Visibility = false;
				this.radioInherit.Visibility = false;

				this.buttonUse.Text = Res.Strings.Dialog.ResourceSelector.Button.Use;
			}
		}

		protected void UpdateInherit()
		{
			//	Met à jour en fonction du bouton pour l'héritage.
			if (this.operation == Operation.InheritEntity)
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

				this.header1.Enable = this.IsInherit;
				this.header2.Enable = this.IsInherit;
				this.listModules.Enable = this.IsInherit;
				this.listResources.Enable = this.IsInherit;
			}
			else
			{
				this.header1.Enable = true;
				this.header2.Enable = true;
				this.listModules.Enable = true;
				this.listResources.Enable = true;
			}
		}

		protected bool IsInherit
		{
			get
			{
				return this.typeClass != StructuredTypeClass.Interface && this.isInherit;
			}
		}

		protected Druid SelectedResource
		{
			//	Retourne le Druid de la ressource actuellement sélectionnée.
			get
			{
				if (this.operation == Operation.InheritEntity && !this.IsInherit)
				{
					return Druid.Empty;
				}

				if (this.listResources.SelectedIndex == -1)
				{
					return Druid.Empty;
				}
				else
				{
					CultureMap cultureMap = this.collectionView.Items[this.listResources.SelectedIndex] as CultureMap;
					return cultureMap.Id;
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
			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();

			this.resource = this.SelectedResource;
			this.result = Common.Dialogs.DialogResult.Yes;
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

			this.resource = Druid.Empty;
			this.result = Common.Dialogs.DialogResult.Cancel;
		}

		private void HandleButtonUseClicked(object sender, MessageEventArgs e)
		{
			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();

			this.resource = this.SelectedResource;
			this.result = Common.Dialogs.DialogResult.Yes;
		}


		protected Module						baseModule;
		protected Module						lastModule;
		protected Module						module;
		protected Operation						operation;
		protected ResourceAccess.Type			resourceType;
		protected StructuredTypeClass			typeClass;
		protected bool							isInherit;
		protected ResourceAccess				access;
		protected Druid							resource;
		protected CollectionView				collectionView;
		protected Common.Dialogs.DialogResult	result;

		protected StaticText					title;
		protected RadioButton					radioTypes;
		protected RadioButton					radioEntities;
		protected CheckButton					checkInterface;
		protected RadioButton					radioAlone;
		protected RadioButton					radioInherit;
		protected StaticText					header1;
		protected ScrollList					listModules;
		protected VSplitter						splitter;
		protected StaticText					header2;
		protected ScrollList					listResources;
		protected Button						buttonUse;
		protected Button						buttonCancel;
	}
}
