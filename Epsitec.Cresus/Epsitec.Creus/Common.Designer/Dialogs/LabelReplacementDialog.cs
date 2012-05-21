using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Dialogue permettant de créer ou de choisir un Caption de remplacement pour l'éditeur de Forms.
	/// </summary>
	public class LabelReplacementDialog : AbstractDialog
	{
		public LabelReplacementDialog(DesignerApplication designerApplication) : base(designerApplication)
		{
		}

		public override void Show()
		{
			//	Crée et montre la fenêtre du dialogue.
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.Icon = this.designerApplication.Icon;
				this.window.MakeSecondaryWindow ();
				this.window.PreventAutoClose = true;
				this.WindowInit("LabelReplacement", 500, 300, true);
				this.window.Text = "Légende de remplacement"; //Res.Strings.Dialog.LabelReplacement.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += this.HandleWindowCloseClicked;
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				ResizeKnob resize = new ResizeKnob(this.window.Root);
				resize.Anchor = AnchorStyles.BottomRight;
				resize.Margins = new Margins(0, -8, 0, -8);
				ToolTip.Default.SetToolTip(resize, Res.Strings.Dialog.Tooltip.Resize);

				this.tabBook = new TabBook(this.window.Root);
				this.tabBook.Dock = DockStyle.Fill;
				this.tabBook.ActivePageChanged += new EventHandler<CancelEventArgs>(this.HandleTabBookActivePageChanged);

				this.tabCreate = new TabPage();
				this.tabCreate.TabTitle = "Création d'une nouvelle légende";
				this.tabCreate.Padding = new Margins(8, 8, 8, 8);
				this.tabBook.Items.Add(this.tabCreate);

				this.tabUse = new TabPage();
				this.tabUse.TabTitle = "Utilisation d'une légende existante";
				this.tabUse.Padding = new Margins(0, 0, 0, 0);
				this.tabBook.Items.Add(this.tabUse);

				this.tabBook.ActivePage = this.tabCreate;

				//	Onglet "créer".
				FrameBox createBox;
				StaticText label;

				createBox = new FrameBox(this.tabCreate);
				createBox.Dock = DockStyle.Top;
				createBox.Margins = new Margins(0, 0, 50, 0);

				label = new StaticText(createBox);
				label.Text = "Nom";
				label.PreferredWidth = 40;
				label.ContentAlignment = ContentAlignment.MiddleRight;
				label.Dock = DockStyle.Left;
				label.Margins = new Margins(0, 8, 0, 0);

				this.fieldNameToCreate = new TextField(createBox);
				this.fieldNameToCreate.IsReadOnly = true;
				this.fieldNameToCreate.Dock = DockStyle.Fill;

				createBox = new FrameBox(this.tabCreate);
				createBox.Dock = DockStyle.Top;
				createBox.Margins = new Margins(0, 0, 5, 0);

				label = new StaticText(createBox);
				label.Text = "Texte";
				label.PreferredWidth = 40;
				label.ContentAlignment = ContentAlignment.MiddleRight;
				label.Dock = DockStyle.Left;
				label.Margins = new Margins(0, 8, 0, 0);

				this.fieldTextToCreate = new TextField(createBox);
				this.fieldTextToCreate.Dock = DockStyle.Fill;
				this.fieldTextToCreate.TextChanged += this.HandleTextToCreateChanged;

				//	Onglet "utiliser".
				Widget left = new Widget(this.tabUse);
				left.MinWidth = 150;
				left.MinHeight = 100;
				left.Padding = new Margins(8, 8, 8, 8);
				left.Dock = DockStyle.Left;

				this.splitter = new VSplitter(this.tabUse);
				this.splitter.Dock = DockStyle.Left;

				Widget right = new Widget(this.tabUse);
				right.MinWidth = 150;
				right.MinHeight = 100;
				right.Padding = new Margins(8, 8, 8, 8);
				right.Dock = DockStyle.Fill;

				//	Partie gauche.
				this.header1 = new StaticText(left);
				this.header1.Text = "Modules";
				this.header1.Dock = DockStyle.Top;
				this.header1.Margins = new Margins(0, 0, 0, 5);

				this.listModules = new ScrollList(left);
				this.listModules.Dock = DockStyle.Fill;
				this.listModules.Margins = new Margins(0, 0, 0, 0);
				this.listModules.TabIndex = 2;
				this.listModules.SelectedItemChanged += this.HandleListModulesSelected;

				//	Partie droite.
				this.header2 = new StaticText(right);
				this.header2.Text = "Légendes";
				this.header2.Dock = DockStyle.Top;
				this.header2.Margins = new Margins(0, 0, 0, 5);

				this.listResources = new ScrollList(right);
				this.listResources.Dock = DockStyle.Fill;
				this.listResources.Margins = new Margins(0, 0, 0, 0);
				this.listResources.TabIndex = 3;
				this.listResources.SelectedItemChanged += this.HandleListResourcesSelected;
				this.listResources.DoubleClicked += this.HandleListResourcesDoubleClicked;

				this.textResource = new TextField(right);
				this.textResource.IsReadOnly = true;
				this.textResource.Dock = DockStyle.Bottom;
				this.textResource.Margins = new Margins(0, 0, 4, 0);

				//	Boutons de fermeture.
				Widget footer = new Widget(this.window.Root);
				footer.Margins = new Margins(0, 0, 8, 0);
				footer.Dock = DockStyle.Bottom;

				Widget leftFooter = new Widget(footer);
				leftFooter.Margins = new Margins(0, 0, 0, 0);
				leftFooter.Dock = DockStyle.Left;

				Widget middleFooter = new Widget(footer);
				middleFooter.Margins = new Margins(0, 0, 0, 0);
				middleFooter.Dock = DockStyle.Left;

				Widget rightFooter = new Widget(footer);
				rightFooter.Margins = new Margins(0, 0, 0, 0);
				rightFooter.Dock = DockStyle.Right;

				Widget buttons = new Widget(rightFooter);
				buttons.Margins = new Margins(0, 0, 0, 0);
				buttons.Dock = DockStyle.Bottom;

				this.buttonCancel = new Button(buttons);
				this.buttonCancel.PreferredWidth = 75;
				this.buttonCancel.Text = Res.Strings.Dialog.Button.Cancel;
				this.buttonCancel.ButtonStyle = ButtonStyle.DefaultCancel;
				this.buttonCancel.Dock = DockStyle.Right;
				this.buttonCancel.Clicked += this.HandleButtonCloseClicked;
				this.buttonCancel.TabIndex = 21;
				this.buttonCancel.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.buttonUse = new Button(buttons);
				this.buttonUse.PreferredWidth = 75;
				this.buttonUse.ButtonStyle = ButtonStyle.DefaultAccept;
				this.buttonUse.Dock = DockStyle.Right;
				this.buttonUse.Margins = new Margins(0, 6, 0, 0);
				this.buttonUse.Clicked += this.HandleButtonUseClicked;
				this.buttonUse.TabIndex = 20;
				this.buttonUse.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			}

			this.fieldTextToCreate.Text = "";

			bool notExisting = this.existingCaption.IsEmpty;

			this.UpdateTitle();
			this.UpdateArray();
			this.UpdateText();
			this.UpdateName();
			this.UpdateButtons();

			if (this.resource.IsEmpty && notExisting)
			{
				this.tabBook.ActivePage = this.tabCreate;
				this.fieldTextToCreate.Focus();
			}
			else
			{
				this.tabBook.ActivePage = this.tabUse;
			}

			this.window.ShowDialog();
		}


		public void AccessOpen(string nameToCreate, Druid resource)
		{
			//	Début de l'accès aux ressources pour le dialogue.
			System.Diagnostics.Debug.Assert(resource.Type != Common.Support.DruidType.ModuleRelative);

			//	Initialise this.existingCaption s'il existe déjà un Caption ayant le nom de celui à créer.
			//	C'est probablement un Caption qui avait déjà été créé pour cette ressource, puis que
			//	l'utilisateur avait abandonné, pour revenir au label par défaut.
			this.existingCaption = this.SearchCaption(nameToCreate);

			Module module = this.designerApplication.CurrentModule;
			if (!module.AccessCaptions.IsCorrectNewName(ref nameToCreate))
			{
				nameToCreate = module.AccessCaptions.GetDuplicateName(nameToCreate);
			}
	
			this.nameToCreate = nameToCreate;
			this.resource = resource;

			//	Cherche le module contenant le Druid de la ressource.
			if (this.resource.IsEmpty)
			{
				this.module = this.designerApplication.CurrentModule;
			}
			else
			{
				this.module = this.designerApplication.SearchModule(this.resource);
			}

			this.UpdateAccess();
		}

		protected Druid SearchCaption(string nameToCreate)
		{
			//	Cherche s'il existe déjà un Caption d'après le nom de la ressource.
			Module module = this.designerApplication.CurrentModule;
			for (int i=0; i<module.AccessCaptions.CollectionView.Items.Count; i++)
			{
				CultureMap cultureMap = module.AccessCaptions.CollectionView.Items[i] as CultureMap;

				if (cultureMap.Name == nameToCreate)
				{
					return cultureMap.Id;
				}
			}

			return Druid.Empty;
		}

		protected void UpdateAccess()
		{
			this.access = this.module.AccessCaptions;

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
			this.listModules.Items.Clear();

			List<Module> list = this.designerApplication.OpeningListModule;
			foreach (Module module in list)
			{
				this.listModules.Items.Add(module.ModuleId.Name);
			}

			this.ignoreChanged = true;
			this.listModules.SelectedItem = this.module.ModuleId.Name;
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

				//	Essaie de sélectionner la ressource actuelle, ou celle qui avait été créée puis abandonnée pour ce champ.
				if (cultureMap.Id == this.resource || cultureMap.Id == this.existingCaption)
				{
					sel = i;
				}
			}

			//	On ne cherche la ressource créée puis abandonnée pour ce champ qu'une seule fois.
			this.existingCaption = Druid.Empty;

			this.ignoreChanged = true;
			this.listResources.SelectedItemIndex = sel;
			this.listResources.ShowSelected(ScrollShowMode.Extremity);
			this.ignoreChanged = false;
		}

		protected void UpdateName()
		{
			//	Met à jour le nom de la ressource à créer.
			this.fieldNameToCreate.Text = this.nameToCreate;
		}

		protected void UpdateText()
		{
			//	Met à jour le texte de la ressource sélectionnée.
			string text = null;

			if (this.listResources.SelectedItemIndex != -1)
			{
				CultureMap cultureMap = this.collectionView.Items[this.listResources.SelectedItemIndex] as CultureMap;
				StructuredData data = cultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
				text = ResourceAccess.GetCaptionNiceDescription(data, 0);
			}

			this.textResource.Text = text;
		}

		protected void UpdateButtons()
		{
			//	Met à jour le bouton "Utiliser".
			if (this.buttonUse == null)
			{
				return;
			}

			if (this.tabBook.ActivePage == this.tabCreate)
			{
				this.buttonUse.Text = Res.Strings.Dialog.ResourceSelector.Button.Create;
				this.buttonUse.Enable = !string.IsNullOrEmpty(this.fieldTextToCreate.Text);
			}

			if (this.tabBook.ActivePage == this.tabUse)
			{
				this.buttonUse.Text = Res.Strings.Dialog.ResourceSelector.Button.Use;
				this.buttonUse.Enable = (this.listResources.SelectedItemIndex != -1);
			}
		}

		protected Druid SelectedResource
		{
			//	Retourne le Druid de la ressource actuellement sélectionnée.
			get
			{
				if (this.listResources.SelectedItemIndex == -1)
				{
					return Druid.Empty;
				}
				else
				{
					CultureMap cultureMap = this.collectionView.Items[this.listResources.SelectedItemIndex] as CultureMap;
					return cultureMap.Id;
				}
			}
		}

		protected Druid CreateCaption()
		{
			//	Crée une ressource Caption qui sera utilisée comme légende de remplacement.
			//	Retourne son Druid.
			CultureMap newItem = this.module.AccessCaptions.Accessor.CreateItem();
			newItem.Name = this.nameToCreate;

			StructuredData data = newItem.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
			data.SetValue(Support.Res.Fields.ResourceCaption.Description, this.fieldTextToCreate.Text);

			List<string> list = new List<string>();
			list.Add(this.fieldTextToCreate.Text);
			ResourceAccess.SetStructuredDataValue(this.module.AccessCaptions.Accessor, newItem, data, Support.Res.Fields.ResourceCaption.Labels.ToString(), list);

			this.module.AccessCaptions.Accessor.Collection.Add(newItem);
			this.module.AccessCaptions.CollectionView.MoveCurrentTo(newItem);
			this.module.AccessCaptions.PersistChanges();

			return newItem.Id;
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


		private void HandleTabBookActivePageChanged(object sender, CancelEventArgs e)
		{
			//	Appelé après un changement d'onglet.
			this.UpdateButtons();

			if (this.fieldTextToCreate != null && this.tabBook.ActivePage == this.tabCreate)
			{
				if (this.fieldTextToCreate != null)
				{
					this.fieldTextToCreate.Focus();
				}
			}
		}

		private void HandleTextToCreateChanged(object sender)
		{
			//	Appelé lorsque le texte du Caption à créer a changé.
			this.UpdateButtons();
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
				this.module = module;
				this.UpdateAccess();
				this.UpdateArray();
				this.UpdateText();
			}
		}

		private void HandleListResourcesSelected(object sender)
		{
			//	La ressource sélectionnée a changé.
			if (this.ignoreChanged)
			{
				return;
			}

			this.UpdateText();
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
			//	Bouton "Créer" ou "Utiliser" cliqué.
			if (this.tabBook.ActivePage == this.tabCreate)
			{
				this.resource = this.CreateCaption();  // crée la ressource Caption
			}

			if (this.tabBook.ActivePage == this.tabUse)
			{
				this.resource = this.SelectedResource;  // utilise la ressource choisie
			}

			this.result = Common.Dialogs.DialogResult.Yes;

			this.Close();
		}


		protected Module						module;
		protected ResourceAccess				access;
		protected Druid							resource;
		protected Druid							existingCaption;
		protected string						nameToCreate;
		protected CollectionView				collectionView;
		protected Common.Dialogs.DialogResult	result;

		protected TabBook						tabBook;

		protected TabPage						tabCreate;
		protected TextField						fieldNameToCreate;
		protected TextField						fieldTextToCreate;

		protected TabPage						tabUse;
		protected StaticText					header1;
		protected ScrollList					listModules;
		protected VSplitter						splitter;
		protected StaticText					header2;
		protected ScrollList					listResources;
		protected TextField						textResource;
		protected Button						buttonUse;
		protected Button						buttonCancel;
	}
}
