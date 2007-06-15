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
			MyWidgets.StackedPanel leftContainer;

			//	Choix du contr�leur.
			this.buttonSuiteCompact = this.CreateBand(out leftContainer, Res.Strings.Viewers.Types.Controller.Title, BandMode.SuiteView, GlyphShape.ArrowUp, true, 0.6);
			this.buttonSuiteCompact.Clicked += new MessageEventHandler(this.HandleButtonCompactOrExtendClicked);

			StaticText label = new StaticText(leftContainer.Container);
			label.Text = Res.Strings.Viewers.Types.Controller.Title;
			label.MinHeight = 20;  // attention, tr�s important !
			label.PreferredHeight = 20;
			label.PreferredWidth = 60;
			label.ContentAlignment = ContentAlignment.MiddleRight;
			label.Margins = new Margins(0, 5, 0, 0);
			label.Dock = DockStyle.Left;

			this.fieldController = new TextFieldCombo(leftContainer.Container);
			this.fieldController.IsReadOnly = true;
			this.fieldController.MinHeight = 20;  // attention, tr�s important !
			this.fieldController.PreferredWidth = 200;
			this.fieldController.HorizontalAlignment = HorizontalAlignment.Left;
			this.fieldController.Dock = DockStyle.Left;
			this.fieldController.TextChanged += new EventHandler(this.HandleControllerTextChanged);
			this.fieldController.TabIndex = this.tabIndex++;
			this.fieldController.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			//	Zone 'nullable'.
			this.CreateBand(out leftContainer, Res.Strings.Viewers.Types.Nullable.Title, BandMode.SuiteView, GlyphShape.None, false, 0.6);

			this.primaryNullable = new CheckButton(leftContainer.Container);
			this.primaryNullable.Text = Res.Strings.Viewers.Types.Nullable.CheckButton;
			this.primaryNullable.Dock = DockStyle.StackBegin;
			this.primaryNullable.Pressed += new MessageEventHandler(this.HandleNullablePressed);
			this.primaryNullable.TabIndex = this.tabIndex++;
			this.primaryNullable.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			//	Editeur du type.
			this.CreateBand(out this.container, Res.Strings.Viewers.Types.Editor.Title, BandMode.SuiteView, GlyphShape.None, false, 0.6);

			//	R�sum� des param�tres.
			this.buttonSuiteExtend = this.CreateBand(out leftContainer, "R�sum�", BandMode.SuiteSummary, GlyphShape.ArrowDown, true, 0.6);
			this.buttonSuiteExtend.Clicked += new MessageEventHandler(this.HandleButtonCompactOrExtendClicked);

			this.primarySuiteSummary = new StaticText(leftContainer.Container);
			this.primarySuiteSummary.MinHeight = 30;
			this.primarySuiteSummary.Dock = DockStyle.Fill;

			this.UpdateDisplayMode();
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
			//	Met � jour le contenu du Viewer.
			base.Update();
		}

		protected override string RetTitle
		{
			//	Retourne le texte � utiliser pour le titre en dessus de la zone scrollable.
			get
			{
				int sel = this.access.AccessIndex;

				if (sel == -1)
				{
					return "";
				}
				else
				{
					//	Cherche le nom du type du type.
					string typeName = null;
					ResourceAccess.Field field = this.access.GetField(sel, null, ResourceAccess.FieldType.AbstractType);
					if (field != null)
					{
						AbstractType type = field.AbstractType;
						TypeCode typeCode = ResourceAccess.AbstractTypeToTypeCode(type);
						typeName = ResourceAccess.TypeCodeToDisplay(typeCode);

						if (type is EnumType)
						{
							EnumType enumType = type as EnumType;
							if (enumType.IsNativeEnum)
							{
								typeName = string.Concat(typeName, " ", Res.Strings.Viewers.Types.Editor.Native);
							}
						}
					}

					field = this.access.GetField(sel, null, ResourceAccess.FieldType.Name);
					if (typeName == null)
					{
						return field.String;
					}
					else
					{
						//	Ajoute le type du type, en gras et entre parenth�ses.
						return string.Concat(field.String, " (<b>", typeName, "</b>)");
					}
				}
			}
		}

		protected override void ClearCache()
		{
			//	Force une nouvelle mise � jour lors du prochain Update.
			if (this.editor != null)
			{
				this.editor.ClearCache();
			}
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

			TypeCode typeCode = TypeCode.Invalid;
			AbstractType type = null;

			if (sel != -1)
			{
				ResourceAccess.Field field = this.access.GetField(sel, null, ResourceAccess.FieldType.AbstractType);
				if (field != null)
				{
					type = field.AbstractType;
					typeCode = ResourceAccess.AbstractTypeToTypeCode(type);
				}
			}

			if (this.typeCode != typeCode)  // autre type ?
			{
				this.typeCode = typeCode;

				if (this.editor != null)
				{
					//	Supprime l'�diteur actuel.
					this.container.Container.Children.Clear();

					this.editor.ContentChanged -= new EventHandler(this.HandleEditorContentChanged);
					this.editor = null;
				}

				this.editor = MyWidgets.AbstractTypeEditor.Create(this.typeCode);

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

			this.UpdateController(typeCode);

			if (sel == -1)
			{
				this.SetTextField(this.fieldController, 0, null, ResourceAccess.FieldType.None);

				this.primaryNullable.Enable = false;
				this.primaryNullable.ActiveState = ActiveState.No;

				this.primarySuiteSummary.Text = "";
			}
			else
			{
				this.SetTextField(this.fieldController, sel, null, ResourceAccess.FieldType.Controller);

				this.primaryNullable.Enable = true;
				this.primaryNullable.ActiveState = (type != null && type.IsNullable) ? ActiveState.Yes : ActiveState.No;

				if (this.editor == null)
				{
					this.primarySuiteSummary.Text = "";
				}
				else
				{
					this.primarySuiteSummary.Text = this.editor.GetSummary();
				}
			}

			this.ignoreChange = iic;

			base.UpdateEdit();  // met � jour le reste
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

		private void HandleNullablePressed(object sender, MessageEventArgs e)
		{
			//	Bouton � cocher 'Nullable' press�.
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
		protected TypeCode						typeCode = TypeCode.Invalid;
		protected CheckButton					primaryNullable;
		protected MyWidgets.AbstractTypeEditor	editor;
		protected TextFieldCombo				fieldController;
		protected StaticText					primarySuiteSummary;
	}
}
