using System.Collections.Generic;
using System.Text.RegularExpressions;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.Viewers
{
	/// <summary>
	/// Permet de représenter les ressources d'un module.
	/// </summary>
	public class Types : AbstractCaptions
	{
		public Types(Module module, PanelsContext context, ResourceAccess access, MainWindow mainWindow) : base(module, context, access, mainWindow)
		{
			//	Editeur contenant toutes les définitions.
			MyWidgets.StackedPanel leftContainer;

			//	Choix du contrôleur.
			this.CreateBand(out leftContainer, Res.Strings.Viewers.Types.Controller.Title, 0.3);

			this.fieldController = new TextFieldCombo(leftContainer.Container);
			this.fieldController.IsReadOnly = true;
			this.fieldController.PreferredWidth = 200;
			this.fieldController.HorizontalAlignment = HorizontalAlignment.Left;
			this.fieldController.Dock = DockStyle.StackBegin;
			this.fieldController.TextChanged += new EventHandler(this.HandleControllerTextChanged);
			this.fieldController.TabIndex = this.tabIndex++;
			this.fieldController.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			//	Zone 'nullable'.
			this.CreateBand(out leftContainer, Res.Strings.Viewers.Types.Nullable.Title, 0.1);

			this.primaryNullable = new CheckButton(leftContainer.Container);
			this.primaryNullable.Text = Res.Strings.Viewers.Types.Nullable.CheckButton;
			this.primaryNullable.Dock = DockStyle.StackBegin;
			this.primaryNullable.Pressed += new MessageEventHandler(this.HandleNullablePressed);
			this.primaryNullable.TabIndex = this.tabIndex++;
			this.primaryNullable.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			//	Editeur du type.
			this.CreateBand(out this.container, "", 0.6);

			this.UpdateEdit();
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


		public override void Update()
		{
			//	Met à jour le contenu du Viewer.
			base.Update();
		}

		protected override void UpdateEdit()
		{
			//	Met à jour les lignes éditables en fonction de la sélection dans le tableau.
			if (this.container == null)
			{
				return;
			}

			bool iic = this.ignoreChange;
			this.ignoreChange = true;

			int sel = this.access.AccessIndex;

			if (sel >= this.access.AccessCount)
			{
				sel = -1;
			}

			ResourceAccess.TypeType typeType = ResourceAccess.TypeType.None;
			AbstractType type = null;

			if (sel != -1)
			{
				ResourceAccess.Field field = this.access.GetField(sel, null, ResourceAccess.FieldType.AbstractType);
				if (field != null)
				{
					type = field.AbstractType;
					typeType = ResourceAccess.AbstractTypeToTypeType(type);
				}
			}

			string typeName = ResourceAccess.TypeTypeToDisplay(typeType);
			if (type is EnumType)
			{
				EnumType enumType = type as EnumType;
				if (enumType.IsNativeEnum)
				{
					typeName = string.Concat(typeName, " (", Res.Strings.Viewers.Types.Editor.Native, ")");
				}
			}

			this.container.Title = string.Format(Res.Strings.Viewers.Types.Editor.Title, typeName);

			if (this.typeType != typeType)  // autre type ?
			{
				this.typeType = typeType;

				if (this.editor != null)
				{
					//	Supprime l'éditeur actuel.
					this.container.Container.Children.Clear();

					this.editor.ContentChanged -= new EventHandler(this.HandleEditorContentChanged);
					this.editor = null;
				}

				this.editor = MyWidgets.AbstractTypeEditor.Create(this.typeType);

				if (this.editor != null)
				{
					//	Crée le nouvel éditeur.
					this.editor.SetParent(this.container.Container);
					this.editor.Module = this.module;
					this.editor.MainWindow = this.mainWindow;
					this.editor.ResourceAccess = this.access;
					this.editor.Dock = DockStyle.StackBegin;
					this.editor.ContentChanged += new EventHandler(this.HandleEditorContentChanged);
				}
			}

			if (this.editor != null)
			{
				this.editor.ResourceSelected = sel;
			}

			this.UpdateController(typeType);

			if (sel == -1)
			{
				this.SetTextField(this.fieldController, 0, null, ResourceAccess.FieldType.None);

				this.primaryNullable.Enable = false;
				this.primaryNullable.ActiveState = ActiveState.No;
			}
			else
			{
				this.SetTextField(this.fieldController, sel, null, ResourceAccess.FieldType.Controller);

				this.primaryNullable.Enable = true;
				this.primaryNullable.ActiveState = (type != null && type.IsNullable) ? ActiveState.Yes : ActiveState.No;
			}

			this.ignoreChange = iic;

			base.UpdateEdit();  // met à jour le reste
		}

		protected void UpdateController(ResourceAccess.TypeType typeType)
		{
			this.fieldController.Items.Clear();
			this.fieldController.Items.Add("Normal");

			if (typeType == ResourceAccess.TypeType.Decimal    ||
				typeType == ResourceAccess.TypeType.Double     ||
				typeType == ResourceAccess.TypeType.Integer    ||
				typeType == ResourceAccess.TypeType.LongInteger)
			{
				this.fieldController.Items.Add("Simply");
				this.fieldController.Items.Add("Slider");
				this.fieldController.Items.Add("UpDown");
			}

			if (typeType == ResourceAccess.TypeType.Enum)
			{
				this.fieldController.Items.Add("Icons");
				this.fieldController.Items.Add("RadioButtons");
				this.fieldController.Items.Add("ScrollList");
				this.fieldController.Items.Add("Combo");
			}
		}


		protected override void TextFieldToIndex(AbstractTextField textField, out int field, out int subfield)
		{
			//	Cherche les index correspondant à un texte éditable.
			base.TextFieldToIndex(textField, out field, out subfield);
		}

		protected override AbstractTextField IndexToTextField(int field, int subfield)
		{
			//	Cherche le TextField permettant d'éditer des index.
			return base.IndexToTextField(field, subfield);
		}


		private void HandleEditorContentChanged(object sender)
		{
			//	Le contenu de l'éditeur de type a changé.
			int sel = this.access.AccessIndex;

			//	[Note1] Il n'est pas nécessaire de donner le type AbstractType, car il est déjà
			//	dans le cache de ResourceAccess. Lorsqu'on utilise ResourceAccess.GetField
			//	avec FieldType.AbstractType, on obtient un type qu'on peut directement
			//	modifier !
			this.access.SetField(sel, null, ResourceAccess.FieldType.AbstractType, null);
		}

		private void HandleControllerTextChanged(object sender)
		{
			//	Le texte éditable pour le contrôleur a changé.
			if (this.ignoreChange)
			{
				return;
			}

			int sel = this.access.AccessIndex;
			string controller = this.fieldController.Text;
			this.access.SetField(sel, null, ResourceAccess.FieldType.Controller, new ResourceAccess.Field(controller));
		}

		private void HandleNullablePressed(object sender, MessageEventArgs e)
		{
			//	Bouton à cocher 'Nullable' pressé.
			if (this.ignoreChange)
			{
				return;
			}

			bool nullable = (this.primaryNullable.ActiveState == ActiveState.No);
			int sel = this.access.AccessIndex;
			ResourceAccess.Field field = this.access.GetField(sel, null, ResourceAccess.FieldType.AbstractType);
			AbstractType type = field.AbstractType;
			type.DefineIsNullable(nullable);
			this.access.SetField(sel, null, ResourceAccess.FieldType.AbstractType, null);

			this.UpdateColor();
		}


		protected MyWidgets.StackedPanel		container;
		protected ResourceAccess.TypeType		typeType = ResourceAccess.TypeType.None;
		protected CheckButton					primaryNullable;
		protected MyWidgets.AbstractTypeEditor	editor;
		protected TextFieldCombo				fieldController;
	}
}
