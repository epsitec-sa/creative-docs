using System.Collections.Generic;
using System.Text.RegularExpressions;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.Viewers
{
	/// <summary>
	/// Permet de repr�senter les ressources d'un module.
	/// </summary>
	public class Types : AbstractCaptions
	{
		public Types(Module module, PanelsContext context, ResourceAccess access, MainWindow mainWindow) : base(module, context, access, mainWindow)
		{
			//	Editeur contenant toutes les d�finitions.
			this.CreateBand(out this.container, "", 0.3);

			MyWidgets.StackedPanel leftContainer;
			this.CreateBand(out leftContainer, "Contr�leur", 0.7);

			this.fieldController = new TextFieldCombo(leftContainer.Container);
			this.fieldController.IsReadOnly = true;
			this.fieldController.PreferredWidth = 200;
			this.fieldController.HorizontalAlignment = HorizontalAlignment.Left;
			this.fieldController.Dock = DockStyle.StackBegin;
			this.fieldController.TextChanged += new EventHandler(this.HandleControllerTextChanged);
			this.fieldController.TabIndex = this.tabIndex++;
			this.fieldController.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.fieldController.Items.Add("Normal");
			this.fieldController.Items.Add("Icons");

			this.UpdateEdit();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.fieldController.TextChanged -= new EventHandler(this.HandleControllerTextChanged);
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
			//	Met � jour le contenu du Viewer.
			base.Update();
		}

		protected override void UpdateEdit()
		{
			//	Met � jour les lignes �ditables en fonction de la s�lection dans le tableau.
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
					typeType = ResourceAccess.CaptionTypeType(type);
				}
			}

			string typeName = typeType.ToString();
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
					//	Supprime l'�diteur actuel.
					this.container.Container.Children.Clear();

					this.editor.ContentChanged -= new EventHandler(this.HandleEditorContentChanged);
					this.editor = null;
				}

				this.editor = MyWidgets.AbstractTypeEditor.Create(this.typeType);

				if (this.editor != null)
				{
					//	Cr�e le nouvel �diteur.
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

			if (sel == -1)
			{
				this.SetTextField(this.fieldController, 0, null, ResourceAccess.FieldType.None);
			}
			else
			{
				this.SetTextField(this.fieldController, sel, null, ResourceAccess.FieldType.Controller);
			}

			this.ignoreChange = iic;

			base.UpdateEdit();  // met � jour le reste
		}


		protected override void TextFieldToIndex(AbstractTextField textField, out int field, out int subfield)
		{
			//	Cherche les index correspondant � un texte �ditable.
			base.TextFieldToIndex(textField, out field, out subfield);
		}

		protected override AbstractTextField IndexToTextField(int field, int subfield)
		{
			//	Cherche le TextField permettant d'�diter des index.
			return base.IndexToTextField(field, subfield);
		}


		private void HandleEditorContentChanged(object sender)
		{
			//	Le contenu de l'�diteur de type a chang�.
			int sel = this.access.AccessIndex;

			//	[Note1] Il n'est pas n�cessaire de donner le type AbstractType, car il est d�j�
			//	dans le cache de ResourceAccess. Lorsqu'on utilise ResourceAccess.GetField
			//	avec FieldType.AbstractType, on obtient un type qu'on peut directement
			//	modifier !
			this.access.SetField(sel, null, ResourceAccess.FieldType.AbstractType, null);
		}

		private void HandleControllerTextChanged(object sender)
		{
			//	Le texte �ditable pour le contr�leur a chang�.
			if (this.ignoreChange)
			{
				return;
			}

			int sel = this.access.AccessIndex;
			string controller = this.fieldController.Text;
			this.access.SetField(sel, null, ResourceAccess.FieldType.Controller, new ResourceAccess.Field(controller));
		}


		protected MyWidgets.StackedPanel		container;
		protected ResourceAccess.TypeType		typeType = ResourceAccess.TypeType.None;
		protected MyWidgets.AbstractTypeEditor	editor;
		protected TextFieldCombo				fieldController;
	}
}
