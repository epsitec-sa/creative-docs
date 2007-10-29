using System.Collections.Generic;
using System.Text.RegularExpressions;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Viewers
{
	/// <summary>
	/// Permet de représenter les ressources d'un module.
	/// </summary>
	public class Types : AbstractCaptions
	{
		public Types(Module module, PanelsContext context, ResourceAccess access, DesignerApplication designerApplication) : base (module, context, access, designerApplication)
		{
			MyWidgets.StackedPanel leftContainer;

			//	Séparateur.
			this.CreateBand(out leftContainer, "", BandMode.Separator, GlyphShape.None, false, 0.0);

			//	Choix du contrôleur.
			this.buttonSuiteCompact = this.CreateBand(out leftContainer, Res.Strings.Viewers.Types.Controller.Title, BandMode.SuiteView, GlyphShape.ArrowUp, true, 0.1);
			this.buttonSuiteCompact.Clicked += new MessageEventHandler(this.HandleButtonCompactOrExtendClicked);

			this.groupController = new MyWidgets.ResetBox(leftContainer.Container);
			this.groupController.IsPatch = this.module.IsPatch;
			this.groupController.Dock = DockStyle.Fill;
			this.groupController.ResetButton.Clicked += new MessageEventHandler(this.HandleResetButtonClicked);

			StaticText label = new StaticText(this.groupController.GroupBox);
			label.Text = Res.Strings.Viewers.Types.Controller.Title;
			label.MinHeight = 20;  // attention, très important !
			label.PreferredHeight = 20;
			label.PreferredWidth = 60;
			label.ContentAlignment = ContentAlignment.MiddleRight;
			label.Margins = new Margins(0, 5, 0, 0);
			label.Dock = DockStyle.Left;

			this.fieldController = new TextFieldCombo(this.groupController.GroupBox);
			this.fieldController.IsReadOnly = true;
			this.fieldController.MinHeight = 20;  // attention, très important !
			this.fieldController.PreferredWidth = 200;
			this.fieldController.HorizontalAlignment = HorizontalAlignment.Left;
			this.fieldController.Dock = DockStyle.Left;
			this.fieldController.TextChanged += new EventHandler(this.HandleControllerTextChanged);
			this.fieldController.TabIndex = this.tabIndex++;
			this.fieldController.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			//	Zone 'nullable'.
			this.CreateBand(out leftContainer, Res.Strings.Viewers.Types.Nullable.Title, BandMode.SuiteView, GlyphShape.None, false, 0.1);

			this.groupNullable = new MyWidgets.ResetBox(leftContainer.Container);
			this.groupNullable.IsPatch = this.module.IsPatch;
			this.groupNullable.Dock = DockStyle.Fill;
			this.groupNullable.ResetButton.Clicked += new MessageEventHandler(this.HandleResetButtonClicked);

			this.primaryNullable = new CheckButton(this.groupNullable.GroupBox);
			this.primaryNullable.Text = Res.Strings.Viewers.Types.Nullable.CheckButton;
			this.primaryNullable.Dock = DockStyle.StackBegin;
			this.primaryNullable.Pressed += new MessageEventHandler(this.HandleNullablePressed);
			this.primaryNullable.TabIndex = this.tabIndex++;
			this.primaryNullable.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			//	Editeur du type.
			this.CreateBand(out this.container, Res.Strings.Viewers.Types.Editor.Title, BandMode.SuiteView, GlyphShape.None, false, 0.1);

			//	Résumé des paramètres.
			this.buttonSuiteExtend = this.CreateBand(out leftContainer, "Résumé", BandMode.SuiteSummary, GlyphShape.ArrowDown, true, 0.1);
			this.buttonSuiteExtend.Clicked += new MessageEventHandler(this.HandleButtonCompactOrExtendClicked);

			this.primarySuiteSummary = new StaticText(leftContainer.Container);
			this.primarySuiteSummary.MinHeight = 30;
			this.primarySuiteSummary.Dock = DockStyle.Fill;

			this.UpdateAll();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.fieldController.TextChanged -= new EventHandler(this.HandleControllerTextChanged);
				this.primaryNullable.Pressed -= new MessageEventHandler(this.HandleNullablePressed);
			}

			base.Dispose(disposing);
		}

		public override ResourceAccess.Type ResourceType
		{
			get
			{
				return ResourceAccess.Type.Types;
			}
		}


		protected override void UpdateEdit()
		{
			//	Met à jour les lignes éditables en fonction de la sélection dans le tableau.
			base.UpdateEdit();

			bool iic = this.ignoreChange;
			this.ignoreChange = true;

			this.groupController.Enable = !this.designerApplication.IsReadonly;
			this.groupNullable.Enable = !this.designerApplication.IsReadonly;

			CultureMap item = this.access.CollectionView.CurrentItem as CultureMap;
			StructuredData data = null;
			object value;
			bool usesOriginalData;
			
			if (item != null)
			{
				data = item.GetCultureData(this.GetTwoLetters(0));
			}

			TypeCode typeCode = TypeCode.Invalid;
			string controller = null;
			string controllerParameter = null;
			bool nullable = false;

			if (data != null)
			{
				value = data.GetValue(Support.Res.Fields.ResourceBaseType.TypeCode, out usesOriginalData);
				if (!UndefinedValue.IsUndefinedValue(value))
				{
					typeCode = (TypeCode) value;
				}

				controller = data.GetValue(Support.Res.Fields.ResourceBaseType.DefaultController, out usesOriginalData) as string;
				Abstract.ColorizeResetBox(this.groupController, usesOriginalData);
				controllerParameter = data.GetValue(Support.Res.Fields.ResourceBaseType.DefaultControllerParameter) as string;

				value = data.GetValue(Support.Res.Fields.ResourceBaseType.Nullable, out usesOriginalData);
				Abstract.ColorizeResetBox(this.groupNullable, usesOriginalData);
				if (!UndefinedValue.IsUndefinedValue(value))
				{
					nullable = (bool) value;
				}
			}

			this.UpdateController(typeCode);
			this.fieldController.Text = controller;

			this.primaryNullable.ActiveState = nullable ? ActiveState.Yes : ActiveState.No;

			if (this.typeCode != typeCode)  // autre type ?
			{
				this.typeCode = typeCode;

				if (this.editor != null)
				{
					//	Supprime l'éditeur actuel.
					this.container.Container.Children.Clear();

					this.editor.ContentChanged -= new EventHandler(this.HandleEditorContentChanged);
					this.editor = null;
				}

				this.editor = MyWidgets.AbstractTypeEditor.Create(this.typeCode, this.module);

				if (this.editor != null)
				{
					//	Crée le nouvel éditeur.
					this.editor.SetParent(this.container.Container);
					this.editor.TypeCode = this.typeCode;
					this.editor.DesignerApplication = this.designerApplication;
					this.editor.ResourceAccess = this.access;
					this.editor.Dock = DockStyle.StackBegin;
					this.editor.ContentChanged += new EventHandler(this.HandleEditorContentChanged);
				}
			}

			if (this.editor == null)
			{
				this.primarySuiteSummary.Text = "";
			}
			else
			{
				this.editor.Enable = !this.designerApplication.IsReadonly;
				this.editor.CultureMap = item;
				this.primarySuiteSummary.Text = this.editor.GetSummary();
			}

			this.ignoreChange = iic;
		}

		protected void UpdateController(TypeCode typeCode)
		{
			this.fieldController.Items.Clear();
			this.fieldController.Items.Add("Normal");

			if (typeCode == TypeCode.Decimal    ||
				typeCode == TypeCode.Double     ||
				typeCode == TypeCode.Integer    ||
				typeCode == TypeCode.LongInteger)
			{
				this.fieldController.Items.Add("Simply");
				this.fieldController.Items.Add("Slider");
				this.fieldController.Items.Add("UpDown");
			}

			if (typeCode == TypeCode.Enum)
			{
				this.fieldController.Items.Add("Icons");
				this.fieldController.Items.Add("RadioButtons");
				this.fieldController.Items.Add("ScrollList");
				this.fieldController.Items.Add("Combo");
			}
		}

		
		protected override void InitializeTable()
		{
			//	Initialise la table.
			StructuredType cultureMapType = new StructuredType();
			cultureMapType.Fields.Add("Name", StringType.Default);
			cultureMapType.Fields.Add("Type", StringType.Default);
			cultureMapType.Fields.Add("Primary", StringType.Default);
			cultureMapType.Fields.Add("Secondary", StringType.Default);
			cultureMapType.Fields.Add("Source", StringType.Default);
			cultureMapType.Fields.Add("Druid", StringType.Default);
			cultureMapType.Fields.Add("Local", StringType.Default);
			cultureMapType.Fields.Add("Identity", StringType.Default);

			this.table.SourceType = cultureMapType;

			this.table.Columns.Add(new UI.ItemTableColumn("Name", new Widgets.Layouts.GridLength(this.GetColumnWidth(0), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Type", new Widgets.Layouts.GridLength(this.GetColumnWidth(1), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Primary", new Widgets.Layouts.GridLength(this.GetColumnWidth(2), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Secondary", new Widgets.Layouts.GridLength(this.GetColumnWidth(3), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Source", new Widgets.Layouts.GridLength(this.GetColumnWidth(4), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Druid", new Widgets.Layouts.GridLength(this.GetColumnWidth(5), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Local", new Widgets.Layouts.GridLength(this.GetColumnWidth(6), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Identity", new Widgets.Layouts.GridLength(this.GetColumnWidth(7), Widgets.Layouts.GridUnitType.Proportional)));

			this.table.ColumnHeader.SetColumnComparer(1, Types.CompareTypeColumns);
			this.table.ColumnHeader.SetColumnComparer(2, this.ComparePrimary);
			this.table.ColumnHeader.SetColumnComparer(3, this.CompareSecondary);
			this.table.ColumnHeader.SetColumnComparer(4, this.CompareSource);
			this.table.ColumnHeader.SetColumnComparer(5, this.CompareDruid);
			this.table.ColumnHeader.SetColumnComparer(6, this.CompareLocal);
			this.table.ColumnHeader.SetColumnComparer(7, this.CompareIdentity);

			this.table.ColumnHeader.SetColumnText(0, "Nom");
			this.table.ColumnHeader.SetColumnText(1, "Type");
			this.table.ColumnHeader.SetColumnText(5, "Druid");
			this.table.ColumnHeader.SetColumnText(6, "Local");
			this.table.ColumnHeader.SetColumnText(7, "Identité");
			
			this.table.ColumnHeader.SetColumnSort(0, ListSortDirection.Ascending);
			this.table.ColumnHeader.SetColumnSort(1, ListSortDirection.Ascending);
		}

		protected override int PrimaryColumn
		{
			//	Retourne le rang de la colonne pour la culture principale.
			get
			{
				return 2;
			}
		}

		protected override int SecondaryColumn
		{
			//	Retourne le rang de la colonne pour la culture secondaire.
			get
			{
				return 3;
			}
		}


		protected override double GetColumnWidth(int column)
		{
			//	Retourne la largeur à utiliser pour une colonne de la liste de gauche.
			if (this.designerApplication.DisplayModeState == DesignerApplication.DisplayMode.Horizontal)
			{
				return Types.columnWidthHorizontal[column];
			}
			else
			{
				return Types.columnWidthVertical[column];
			}
		}

		protected override void SetColumnWidth(int column, double value)
		{
			//	Mémorise la largeur à utiliser pour une colonne de la liste de gauche.
			if (this.designerApplication.DisplayModeState == DesignerApplication.DisplayMode.Horizontal)
			{
				Types.columnWidthHorizontal[column] = value;
			}
			else
			{
				Types.columnWidthVertical[column] = value;
			}
		}
	
	
		private static int CompareTypeColumns(object a, object b)
		{
			CultureMap itemA = a as CultureMap;
			CultureMap itemB = b as CultureMap;

			StructuredData dataA = itemA.GetCultureData(Common.Support.Resources.DefaultTwoLetterISOLanguageName);
			StructuredData dataB = itemB.GetCultureData(Common.Support.Resources.DefaultTwoLetterISOLanguageName);

			string codeA = dataA.GetValue(Support.Res.Fields.ResourceBaseType.TypeCode).ToString();
			string codeB = dataB.GetValue(Support.Res.Fields.ResourceBaseType.TypeCode).ToString();

			return codeA.CompareTo(codeB);
		}


		private void HandleControllerTextChanged(object sender)
		{
			//	Le texte éditable pour le contrôleur a changé.
			if (this.ignoreChange)
			{
				return;
			}

			CultureMap item = this.access.CollectionView.CurrentItem as CultureMap;
			StructuredData data = item.GetCultureData(this.GetTwoLetters(0));

			data.SetValue(Support.Res.Fields.ResourceBaseType.DefaultController, this.fieldController.Text);

			bool usesOriginalData;
			data.GetValue(Support.Res.Fields.ResourceBaseType.DefaultController, out usesOriginalData);
			Abstract.ColorizeResetBox(this.groupController, usesOriginalData);

			this.editor.OnContentChanged();
			this.editor.UpdateContent();
			this.module.AccessTypes.SetLocalDirty();
		}

		private void HandleNullablePressed(object sender, MessageEventArgs e)
		{
			//	Bouton à cocher 'Nullable' pressé.
			if (this.ignoreChange)
			{
				return;
			}

			CultureMap item = this.access.CollectionView.CurrentItem as CultureMap;
			StructuredData data = item.GetCultureData(this.GetTwoLetters(0));

			data.SetValue(Support.Res.Fields.ResourceBaseType.Nullable, this.primaryNullable.ActiveState == ActiveState.No);

			bool usesOriginalData;
			data.GetValue(Support.Res.Fields.ResourceBaseType.Nullable, out usesOriginalData);
			Abstract.ColorizeResetBox(this.groupNullable, usesOriginalData);

			this.editor.OnContentChanged();
			this.editor.UpdateContent();
			this.module.AccessTypes.SetLocalDirty();
		}

		private void HandleEditorContentChanged(object sender)
		{
			//	Le contenu de l'éditeur de type a changé.
		}

		private void HandleResetButtonClicked(object sender, MessageEventArgs e)
		{
			AbstractButton button = sender as AbstractButton;

			if (button == this.groupController.ResetButton)
			{
				this.editor.ResetToOriginalValue(Support.Res.Fields.ResourceBaseType.DefaultController);
			}

			if (button == this.groupNullable.ResetButton)
			{
				this.editor.ResetToOriginalValue(Support.Res.Fields.ResourceBaseType.Nullable);
			}

			this.UpdateEdit();
			this.module.AccessTypes.SetLocalDirty();
		}


		private static double[]					columnWidthHorizontal = {140, 60, 100, 100, 20, 80, 50, 100};
		private static double[]					columnWidthVertical = {250, 60, 270, 270, 20, 80, 50, 100};

		protected MyWidgets.StackedPanel		container;
		protected TypeCode						typeCode = TypeCode.Invalid;
		protected MyWidgets.ResetBox			groupController;
		protected TextFieldCombo				fieldController;
		protected MyWidgets.ResetBox			groupNullable;
		protected CheckButton					primaryNullable;
		protected MyWidgets.AbstractTypeEditor	editor;
		protected StaticText					primarySuiteSummary;
	}
}
