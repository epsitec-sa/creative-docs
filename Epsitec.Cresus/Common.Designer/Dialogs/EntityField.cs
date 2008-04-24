using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Dialogue permettant de définir entièrement un champ dans une entité.
	/// </summary>
	public class EntityField : Abstract
	{
		public EntityField(DesignerApplication designerApplication) : base(designerApplication)
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
				this.WindowInit("EntityField", 500, 300, true);
				this.window.Text = "Définition du champ";  // Res.Strings.Dialog.EntityField.Title;
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

				this.buttonIsNullable = new CheckButton(footer);
				this.buttonIsNullable.Text = "Accepte d'être nul";
				this.buttonIsNullable.PreferredWidth = 140;
				this.buttonIsNullable.Dock = DockStyle.Left;
				this.buttonIsNullable.TabIndex = 10;
				this.buttonIsNullable.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.buttonCancel = new Button(footer);
				this.buttonCancel.PreferredWidth = 75;
				this.buttonCancel.Text = Res.Strings.Dialog.Button.Cancel;
				this.buttonCancel.ButtonStyle = ButtonStyle.DefaultCancel;
				this.buttonCancel.Dock = DockStyle.Right;
				this.buttonCancel.Clicked += new MessageEventHandler(this.HandleButtonCloseClicked);
				this.buttonCancel.TabIndex = 12;
				this.buttonCancel.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.buttonUse = new Button(footer);
				this.buttonUse.PreferredWidth = 75;
				this.buttonUse.Text = Res.Strings.Dialog.ResourceSelector.Button.Use;
				this.buttonUse.ButtonStyle = ButtonStyle.DefaultAccept;
				this.buttonUse.Dock = DockStyle.Right;
				this.buttonUse.Margins = new Margins(0, 6, 0, 0);
				this.buttonUse.Clicked += new MessageEventHandler(this.HandleButtonUseClicked);
				this.buttonUse.TabIndex = 11;
				this.buttonUse.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				sep = new Separator(this.window.Root);  // trait horizontal de séparation
				sep.PreferredHeight = 1;
				sep.Dock = DockStyle.Bottom;
			}

			this.UpdateTitle();
			this.UpdateArray();
			this.UpdateButtons();
			this.UpdateRadios();

			this.window.ShowDialog();
		}


		public void AccessOpen(Module baseModule, ResourceAccess.Type type, Druid resource, bool isNullable)
		{
			//	Début de l'accès aux ressources pour le dialogue.
			System.Diagnostics.Debug.Assert(resource.Type != Common.Support.DruidType.ModuleRelative);

			this.resourceType = type;
			this.resource = resource;
			this.isNullable = isNullable;

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

		public bool IsNullable
		{
			get
			{
				return this.buttonIsNullable.ActiveState == ActiveState.Yes;
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
			//	bascule sur l'autre type (Types ou Entities). L'idée est d'anticiper sur l'utilisateur,
			//	qui voudra vraissemblablement changer de type s'il voit une liste vide.
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

			return false;
		}

		protected void UpdateAccess()
		{
			this.access = this.module.GetAccess(this.resourceType);

			this.collectionView = new CollectionView(this.access.Accessor.Collection);
			this.collectionView.Filter = this.CollectionViewFilter;
			this.collectionView.SortDescriptions.Add(new SortDescription("Name"));
		}

		protected bool CollectionViewFilter(object obj)
		{
			//	Méthode passé comme paramètre System.Predicate<object> à CollectionView.Filter.
			//	Retourne false si la ressource doit être exclue.
			return true;
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
			string name = ResourceAccess.TypeDisplayName(this.resourceType);
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
			this.listResources.ShowSelected(ScrollShowMode.Extremity);
			this.ignoreChanged = false;
		}

		protected void UpdateButtons()
		{
			//	Met à jour le bouton "Utiliser".
			this.buttonUse.Enable = (this.listResources.SelectedIndex != -1);
		}

		protected void UpdateRadios()
		{
			//	Met à jour les boutons radio pour changer le type.
			this.title.Visibility = true;
			this.radioTypes.Visibility = true;
			this.radioEntities.Visibility = true;

			this.buttonIsNullable.Visibility = true;
			this.buttonIsNullable.ActiveState = this.isNullable ? ActiveState.Yes : ActiveState.No;

			this.buttonUse.Text = Res.Strings.Dialog.ResourceSelector.Button.Use;

			this.radioTypes.ActiveState = (this.resourceType == ResourceAccess.Type.Types) ? ActiveState.Yes : ActiveState.No;
			this.radioEntities.ActiveState = (this.resourceType == ResourceAccess.Type.Entities) ? ActiveState.Yes : ActiveState.No;
		}

		protected Druid SelectedResource
		{
			//	Retourne le Druid de la ressource actuellement sélectionnée.
			get
			{
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


		protected void Close()
		{
			//	Ferme proprement le dialogue.
			if (this.collectionView != null)
			{
				this.collectionView.Filter = null;  // pour éviter un appel ultérieur de CollectionViewFilter !
				this.collectionView = null;
			}

			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();
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
			this.resource = this.SelectedResource;
			this.result = Common.Dialogs.DialogResult.Yes;

			this.Close();
		}

		private void HandleWindowCloseClicked(object sender)
		{
			this.Close();
		}

		private void HandleButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.resource = Druid.Empty;
			this.result = Common.Dialogs.DialogResult.Cancel;

			this.Close();
		}

		private void HandleButtonUseClicked(object sender, MessageEventArgs e)
		{
			this.resource = this.SelectedResource;
			this.result = Common.Dialogs.DialogResult.Yes;

			this.Close();
		}


		protected Module						baseModule;
		protected Module						lastModule;
		protected Module						module;
		protected ResourceAccess.Type			resourceType;
		protected ResourceAccess				access;
		protected Druid							resource;
		protected bool							isNullable;
		protected CollectionView				collectionView;
		protected Common.Dialogs.DialogResult	result;

		protected StaticText					title;
		protected RadioButton					radioTypes;
		protected RadioButton					radioEntities;
		protected StaticText					header1;
		protected ScrollList					listModules;
		protected VSplitter						splitter;
		protected StaticText					header2;
		protected ScrollList					listResources;
		protected CheckButton					buttonIsNullable;
		protected Button						buttonUse;
		protected Button						buttonCancel;
	}
}
