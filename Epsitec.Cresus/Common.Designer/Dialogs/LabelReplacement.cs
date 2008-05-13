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
	public class LabelReplacement : Abstract
	{
		public LabelReplacement(DesignerApplication designerApplication) : base(designerApplication)
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
				this.WindowInit("LabelReplacement", 500, 300, true);
				this.window.Text = "Légende de remplacement"; //Res.Strings.Dialog.LabelReplacement.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				ResizeKnob resize = new ResizeKnob(this.window.Root);
				resize.Anchor = AnchorStyles.BottomRight;
				resize.Margins = new Margins(0, -8, 0, -8);
				ToolTip.Default.SetToolTip(resize, Res.Strings.Dialog.Tooltip.Resize);

				this.ribbonBook = new RibbonBook(this.window.Root);
				this.ribbonBook.Dock = DockStyle.Fill;

				this.ribbonCreate = new RibbonPage();
				this.ribbonCreate.RibbonTitle = "Création d'une nouvelle légende";
				this.ribbonCreate.Padding = new Margins(8, 8, 8, 8);
				this.ribbonBook.Items.Add(this.ribbonCreate);

				this.ribbonUse = new RibbonPage();
				this.ribbonUse.RibbonTitle = "Utilisation d'une légende existante";
				this.ribbonUse.Padding = new Margins(1, 1, 1, 1);
				this.ribbonBook.Items.Add(this.ribbonUse);

				this.ribbonBook.ActivePage = this.ribbonUse;
				
				//	Corps principal.
				Widget body = new Widget(this.ribbonUse);
				body.Dock = DockStyle.Fill;

				Widget left = new Widget(body);
				left.MinWidth = 150;
				left.MinHeight = 100;
				left.Padding = new Margins(8, 8, 8, 8);
				left.Dock = DockStyle.Left;

				this.splitter = new VSplitter(body);
				this.splitter.Dock = DockStyle.Left;

				Widget right = new Widget(body);
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
				this.listModules.SelectedIndexChanged += new EventHandler(this.HandleListModulesSelected);

				//	Partie droite.
				this.header2 = new StaticText(right);
				this.header2.Text = "Légendes";
				this.header2.Dock = DockStyle.Top;
				this.header2.Margins = new Margins(0, 0, 0, 5);

				this.listResources = new ScrollList(right);
				this.listResources.Dock = DockStyle.Fill;
				this.listResources.Margins = new Margins(0, 0, 0, 0);
				this.listResources.TabIndex = 3;
				this.listResources.SelectedIndexChanged += new EventHandler(this.HandleListResourcesSelected);
				this.listResources.DoubleClicked += new MessageEventHandler(this.HandleListResourcesDoubleClicked);

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
				this.buttonCancel.Clicked += new MessageEventHandler(this.HandleButtonCloseClicked);
				this.buttonCancel.TabIndex = 21;
				this.buttonCancel.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.buttonUse = new Button(buttons);
				this.buttonUse.PreferredWidth = 75;
				this.buttonUse.Text = Res.Strings.Dialog.ResourceSelector.Button.Use;
				this.buttonUse.ButtonStyle = ButtonStyle.DefaultAccept;
				this.buttonUse.Dock = DockStyle.Right;
				this.buttonUse.Margins = new Margins(0, 6, 0, 0);
				this.buttonUse.Clicked += new MessageEventHandler(this.HandleButtonUseClicked);
				this.buttonUse.TabIndex = 20;
				this.buttonUse.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			}

			this.UpdateTitle();
			this.UpdateArray();
			this.UpdateButtons();

			this.window.ShowDialog();
		}


		public void AccessOpen(string prefix, Druid resource)
		{
			//	Début de l'accès aux ressources pour le dialogue.
			System.Diagnostics.Debug.Assert(resource.Type != Common.Support.DruidType.ModuleRelative);

			this.prefix = prefix;
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


		protected Module						module;
		protected ResourceAccess				access;
		protected Druid							resource;
		protected string						prefix;
		protected CollectionView				collectionView;
		protected Common.Dialogs.DialogResult	result;

		protected RibbonBook					ribbonBook;
		protected RibbonPage					ribbonCreate;
		protected RibbonPage					ribbonUse;

		protected StaticText					header1;
		protected ScrollList					listModules;
		protected VSplitter						splitter;
		protected StaticText					header2;
		protected ScrollList					listResources;
		protected Button						buttonUse;
		protected Button						buttonCancel;
	}
}
