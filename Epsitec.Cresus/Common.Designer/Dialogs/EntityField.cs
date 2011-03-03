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
				this.window.Icon = this.designerApplication.Icon;
				this.window.MakeSecondaryWindow ();
				this.window.PreventAutoClose = true;
				this.WindowInit("EntityField", 500, 300, true);
				this.window.Text = Res.Strings.Dialog.EntityField.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += this.HandleWindowCloseClicked;
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				ResizeKnob resize = new ResizeKnob(this.window.Root);
				resize.Anchor = AnchorStyles.BottomRight;
				resize.Margins = new Margins(0, -8, 0, -8);
				ToolTip.Default.SetToolTip(resize, Res.Strings.Dialog.Tooltip.Resize);

				//	Titre supérieur.
				Widget header = new Widget(this.window.Root);
				header.Margins = new Margins(0, 0, 0, 8);
				header.Dock = DockStyle.Top;

				StaticText label = new StaticText(header);
				label.Text = "Nom";
				label.ContentAlignment = ContentAlignment.MiddleRight;
				label.PreferredWidth = 40;
				label.Margins = new Margins(0, 5, 0, 0);
				label.Dock = DockStyle.Left;

				this.editFieldName = new TextField(header);
				this.editFieldName.Dock = DockStyle.Fill;
				this.editFieldName.TabIndex = 1;
				this.editFieldName.TextChanged += this.HandleFieldNameChanged;

				this.radioEntities = new RadioButton(header);
				this.radioEntities.AutoToggle = false;
				this.radioEntities.Text = "Entités";
				this.radioEntities.PreferredWidth = 70;
				this.radioEntities.Dock = DockStyle.Right;
				this.radioEntities.Clicked += this.HandleRadioClicked;

				this.radioTypes = new RadioButton(header);
				this.radioTypes.AutoToggle = false;
				this.radioTypes.Text = "Types";
				this.radioTypes.PreferredWidth = 70;
				this.radioTypes.Dock = DockStyle.Right;
				this.radioTypes.Clicked += this.HandleRadioClicked;

				this.glyphFieldName = new GlyphButton(header);
				this.glyphFieldName.ButtonStyle = ButtonStyle.ToolItem;
				this.glyphFieldName.PreferredWidth = 22;
				this.glyphFieldName.Margins = new Margins(-1, 50, 0, 0);
				this.glyphFieldName.Dock = DockStyle.Right;

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
				this.listModules.TabIndex = 2;
				this.listModules.SelectedItemChanged += this.HandleListModulesSelected;

				//	Partie droite.
				this.header2 = new StaticText(right);
				this.header2.Text = "Ressources";
				this.header2.Dock = DockStyle.Top;
				this.header2.Margins = new Margins(0, 0, 5, 5);

				this.listResources = new ScrollList(right);
				this.listResources.Dock = DockStyle.Fill;
				this.listResources.Margins = new Margins(0, 0, 0, 8);
				this.listResources.TabIndex = 3;
				this.listResources.SelectedItemChanged += this.HandleListResourcesSelected;
				this.listResources.DoubleClicked += this.HandleListResourcesDoubleClicked;

				//	Boutons de fermeture.
				Widget footer = new Widget(this.window.Root);
				footer.Margins = new Margins(0, 0, 8, 0);
				footer.Dock = DockStyle.Bottom;

				Widget leftFooter = new Widget(footer);
				leftFooter.Margins = new Margins(0, 0, 0, 0);
				leftFooter.Dock = DockStyle.Left;

				this.buttonIsNullable = new CheckButton(leftFooter);
				this.buttonIsNullable.AutoToggle = false;
				this.buttonIsNullable.Text = "Accepte d'être nul";
				this.buttonIsNullable.PreferredWidth = 140;
				this.buttonIsNullable.Dock = DockStyle.Top;
				this.buttonIsNullable.TabIndex = 10;
				this.buttonIsNullable.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				this.buttonIsNullable.Clicked += this.HandleRadioClicked;

				this.buttonIsPrivate = new CheckButton (leftFooter);
				this.buttonIsPrivate.AutoToggle = false;
				this.buttonIsPrivate.Text = "Relation privée";
				this.buttonIsPrivate.PreferredWidth = 140;
				this.buttonIsPrivate.Margins = new Margins (0, 0, 0, 4);
				this.buttonIsPrivate.Dock = DockStyle.Top;
				this.buttonIsPrivate.TabIndex = 11;
				this.buttonIsPrivate.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				this.buttonIsPrivate.Clicked += this.HandleRadioClicked;

				this.buttonIndexAscending = new CheckButton (leftFooter);
				this.buttonIndexAscending.AutoToggle = false;
				this.buttonIndexAscending.Text = "Index ascendant";
				this.buttonIndexAscending.PreferredWidth = 140;
				this.buttonIndexAscending.Dock = DockStyle.Top;
				this.buttonIndexAscending.TabIndex = 12;
				this.buttonIndexAscending.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				this.buttonIndexAscending.Clicked += this.HandleRadioClicked;

				this.buttonIndexDescending = new CheckButton (leftFooter);
				this.buttonIndexDescending.AutoToggle = false;
				this.buttonIndexDescending.Text = "Index descendant";
				this.buttonIndexDescending.PreferredWidth = 140;
				this.buttonIndexDescending.Dock = DockStyle.Top;
				this.buttonIndexDescending.TabIndex = 13;
				this.buttonIndexDescending.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				this.buttonIndexDescending.Clicked += this.HandleRadioClicked;

				Widget middleFooter = new Widget (footer);
				middleFooter.Margins = new Margins(0, 0, 0, 0);
				middleFooter.Dock = DockStyle.Left;

				this.buttonIsReference = new RadioButton(middleFooter);
				this.buttonIsReference.AutoToggle = false;
				this.buttonIsReference.Text = "Référence";
				this.buttonIsReference.PreferredWidth = 90;
				this.buttonIsReference.Dock = DockStyle.Top;
				this.buttonIsReference.TabIndex = 14;
				this.buttonIsReference.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				this.buttonIsReference.Clicked += this.HandleRadioClicked;

				this.buttonIsCollection = new RadioButton(middleFooter);
				this.buttonIsCollection.AutoToggle = false;
				this.buttonIsCollection.Text = "Collection";
				this.buttonIsCollection.PreferredWidth = 90;
				this.buttonIsCollection.Dock = DockStyle.Top;
				this.buttonIsCollection.TabIndex = 15;
				this.buttonIsCollection.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				this.buttonIsCollection.Clicked += this.HandleRadioClicked;

				this.relationSample = new MyWidgets.RelationSample(footer);
				this.relationSample.PreferredWidth = 40;
				this.relationSample.Margins = new Margins(0, 20, 0, 20);
				this.relationSample.Dock = DockStyle.Left;

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
				this.buttonUse.Text = Res.Strings.Dialog.ResourceSelector.Button.Use;
				this.buttonUse.ButtonStyle = ButtonStyle.DefaultAccept;
				this.buttonUse.Dock = DockStyle.Right;
				this.buttonUse.Margins = new Margins(0, 6, 0, 0);
				this.buttonUse.Clicked += this.HandleButtonUseClicked;
				this.buttonUse.TabIndex = 20;
				this.buttonUse.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				sep = new Separator(this.window.Root);  // trait horizontal de séparation
				sep.PreferredHeight = 1;
				sep.Dock = DockStyle.Bottom;
			}

			this.UpdateFieldName();
			this.UpdateGlyphFieldName();
			this.UpdateTitle();
			this.UpdateArray();
			this.UpdateButtons();
			this.UpdateRadios();
			this.UpdateRelationSample();

			this.window.ShowDialog();
		}


		public void AccessOpen(Module baseModule, ResourceAccess.Type type, string prefix, string fieldName, Druid resource, bool isNullable, bool isCollection, bool isPrivate, bool isIndexAscending, bool isIndexDescending)
		{
			//	Début de l'accès aux ressources pour le dialogue.
			System.Diagnostics.Debug.Assert(resource.Type != Common.Support.DruidType.ModuleRelative);

			this.prefix = prefix;
			this.initialFieldName = fieldName;
			this.fieldName = fieldName;
			this.resourceType = type;
			this.resource = resource;
			this.isNullable = isNullable;
			this.isCollection = isCollection;
			this.isPrivate = isPrivate;
			this.isIndexAscending = isIndexAscending;
			this.isIndexDescending = isIndexDescending;

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

		public string FieldName
		{
			get
			{
				if (this.fieldName == this.initialFieldName)  // nom inchangé ?
				{
					return this.fieldName;
				}
				else
				{
					string name = this.fieldName;
					string err = this.module.AccessFields.CheckNewName(this.prefix, ref name);
					return string.IsNullOrEmpty(err) ? name : null;
				}
			}
		}

		public bool IsNullable
		{
			get
			{
				return this.isNullable;
			}
		}

		public bool IsCollection
		{
			get
			{
				return this.isCollection;
			}
		}

		public bool IsPrivate
		{
			get
			{
				return this.isPrivate;
			}
		}

		public bool IsIndexAscending
		{
			get
			{
				return this.isIndexAscending;
			}
		}

		public bool IsIndexDescending
		{
			get
			{
				return this.isIndexDescending;
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
			CultureMap cultureMap = obj as CultureMap;

			StructuredData data = cultureMap.GetCultureData (Resources.DefaultTwoLetterISOLanguageName);
			StructuredTypeClass typeClass = (StructuredTypeClass) data.GetValue (Support.Res.Fields.ResourceStructuredType.Class);
			if (typeClass == StructuredTypeClass.Interface)
			{
				return false;  // ne liste pas les interfaces
			}

			return true;
		}
			
		public Common.Dialogs.DialogResult AccessClose(out Druid resource)
		{
			//	Fin de l'accès aux ressources pour le dialogue.
			resource = this.resource;
			return this.result;
		}


		protected void UpdateFieldName()
		{
			this.editFieldName.Text = this.fieldName;
			this.editFieldName.SelectAll();
			this.editFieldName.Focus();
		}

		protected void UpdateGlyphFieldName()
		{
			bool ok = false;
			string name = this.editFieldName.Text;
			string err = null;

			if (!string.IsNullOrEmpty(this.initialFieldName) && name == this.initialFieldName)
			{
				ok = true;
			}
			else
			{
				err = this.module.AccessFields.CheckNewName(this.prefix, ref name);
				ok = string.IsNullOrEmpty(err);
			}

			this.glyphFieldName.GlyphShape = ok ? GlyphShape.Accept : GlyphShape.Reject;
			this.editFieldName.SetError(!ok);
			ToolTip.Default.SetToolTip(this.glyphFieldName, err);
		}

		protected void UpdateRelationSample()
		{
			if (this.resourceType == ResourceAccess.Type.Entities)
			{
				this.relationSample.Relation = this.isCollection ? FieldRelation.Collection : FieldRelation.Reference;
				this.relationSample.IsPrivateRelation = this.IsPrivate;
			}
			else
			{
				this.relationSample.Relation = FieldRelation.None;
			}
		}

		protected void UpdateTitle()
		{
			//	Met à jour le titre qui dépend du type des ressources éditées.
			this.listModules.Items.Clear();

			List<Module> list = this.designerApplication.OpeningListModule;
			string text;
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
			this.listResources.SelectedItemIndex = sel;
			this.listResources.ShowSelected(ScrollShowMode.Extremity);
			this.ignoreChanged = false;
		}

		protected void UpdateButtons()
		{
			//	Met à jour le bouton "Utiliser".
			this.buttonUse.Enable = (this.listResources.SelectedItemIndex != -1 && this.glyphFieldName.GlyphShape == GlyphShape.Accept);
		}

		protected void UpdateRadios()
		{
			//	Met à jour les boutons radio pour changer le type.
			this.radioTypes.ActiveState = (this.resourceType == ResourceAccess.Type.Types) ? ActiveState.Yes : ActiveState.No;
			this.radioEntities.ActiveState = (this.resourceType == ResourceAccess.Type.Entities) ? ActiveState.Yes : ActiveState.No;

			this.buttonIsNullable.ActiveState = this.isNullable ? ActiveState.Yes : ActiveState.No;
			this.buttonIsPrivate.ActiveState = this.isPrivate ? ActiveState.Yes : ActiveState.No;
			this.buttonIndexAscending.ActiveState = this.isIndexAscending ? ActiveState.Yes : ActiveState.No;
			this.buttonIndexDescending.ActiveState = this.isIndexDescending ? ActiveState.Yes : ActiveState.No;
			this.buttonIsReference.ActiveState = this.isCollection ? ActiveState.No : ActiveState.Yes;
			this.buttonIsCollection.ActiveState = this.isCollection ? ActiveState.Yes : ActiveState.No;

			this.buttonIsPrivate.Enable = (this.resourceType == ResourceAccess.Type.Entities);
			this.buttonIsReference.Enable = (this.resourceType == ResourceAccess.Type.Entities);
			this.buttonIsCollection.Enable = (this.resourceType == ResourceAccess.Type.Entities);
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


		protected void Close()
		{
			//	Ferme proprement le dialogue.
			if (this.collectionView != null)
			{
				this.collectionView.Dispose ();
				this.collectionView.Filter = null;  // pour éviter un appel ultérieur de CollectionViewFilter !
				this.collectionView = null;
			}

			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}


		private void HandleFieldNameChanged(object sender)
		{
			this.fieldName = this.editFieldName.Text;
			this.UpdateGlyphFieldName();
			this.UpdateButtons();
		}

		private void HandleRadioClicked(object sender, MessageEventArgs e)
		{
			//	Changement du type des ressources par un bouton radio.
			if (this.ignoreChanged)
			{
				return;
			}

			AbstractButton button = sender as AbstractButton;

			if (button == this.radioTypes)
			{
				this.resourceType = ResourceAccess.Type.Types;
				this.UpdateAccess();
				this.UpdateTitle();
				this.UpdateArray();
				this.UpdateButtons();
				this.UpdateRadios();
				this.UpdateRelationSample();
			}

			if (button == this.radioEntities)
			{
				this.resourceType = ResourceAccess.Type.Entities;
				this.UpdateAccess();
				this.UpdateTitle();
				this.UpdateArray();
				this.UpdateButtons();
				this.UpdateRadios();
				this.UpdateRelationSample();
			}

			if (button == this.buttonIsNullable)
			{
				this.isNullable = !this.isNullable;
				this.UpdateRadios();
			}

			if (button == this.buttonIsPrivate)
			{
				this.isPrivate = !this.isPrivate;
				this.UpdateRadios ();
				this.UpdateRelationSample ();
			}

			if (button == this.buttonIndexAscending)
			{
				this.isIndexAscending = !this.isIndexAscending;
				this.UpdateRadios ();
				this.UpdateRelationSample ();
			}

			if (button == this.buttonIndexDescending)
			{
				this.isIndexDescending = !this.isIndexDescending;
				this.UpdateRadios ();
				this.UpdateRelationSample ();
			}

			if (button == this.buttonIsReference)
			{
				this.isCollection = false;
				this.UpdateRadios();
				this.UpdateRelationSample();
			}

			if (button == this.buttonIsCollection)
			{
				this.isCollection = true;
				this.UpdateRadios();
				this.UpdateRelationSample();
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
		protected string						prefix;
		protected string						initialFieldName;
		protected string						fieldName;
		protected bool							isNullable;
		protected bool							isCollection;
		protected bool							isPrivate;
		protected bool							isIndexAscending;
		protected bool							isIndexDescending;
		protected CollectionView				collectionView;
		protected Common.Dialogs.DialogResult	result;

		protected TextField						editFieldName;
		protected GlyphButton					glyphFieldName;
		protected RadioButton					radioTypes;
		protected RadioButton					radioEntities;
		protected StaticText					header1;
		protected ScrollList					listModules;
		protected VSplitter						splitter;
		protected StaticText					header2;
		protected ScrollList					listResources;
		protected CheckButton					buttonIsNullable;
		protected CheckButton					buttonIsPrivate;
		protected CheckButton					buttonIndexAscending;
		protected CheckButton					buttonIndexDescending;
		protected RadioButton					buttonIsReference;
		protected RadioButton					buttonIsCollection;
		protected MyWidgets.RelationSample		relationSample;
		protected Button						buttonUse;
		protected Button						buttonCancel;
	}
}
