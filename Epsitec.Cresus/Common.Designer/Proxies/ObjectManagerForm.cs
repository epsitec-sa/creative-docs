using System.Collections.Generic;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Designer.Proxies
{
	/// <summary>
	/// Cette classe gère les objets associés à un proxy de type Form.
	/// </summary>
	public class ObjectManagerForm : AbstractObjectManager
	{
		public ObjectManagerForm(object objectModifier) : base(objectModifier)
		{
		}

		public override List<AbstractValue> GetValues(Widget selectedObject)
		{
			//	Retourne la liste des valeurs nécessaires pour représenter un objet.
			List<AbstractValue> list = new List<AbstractValue>();

			if (this.ObjectModifier.IsField(selectedObject) ||
				this.ObjectModifier.IsCommand(selectedObject) ||
				this.ObjectModifier.IsBox(selectedObject) ||
				this.ObjectModifier.IsGlue(selectedObject))
			{
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormColumnsRequired, "Nb de colonnes", 1, 10, 1, 1);
			}

			if (this.ObjectModifier.IsField(selectedObject) ||
				this.ObjectModifier.IsBox(selectedObject))
			{
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormRowsRequired, "Nb de lignes", 1, 10, 1, 1);
			}

			if (this.ObjectModifier.IsField(selectedObject) ||
				this.ObjectModifier.IsCommand(selectedObject) ||
				this.ObjectModifier.IsBox(selectedObject) ||
				this.ObjectModifier.IsGlue(selectedObject) ||
				this.ObjectModifier.IsTitle(selectedObject) ||
				this.ObjectModifier.IsLine(selectedObject))
			{
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormPreferredWidth, "Largeur", 1, 1000, 1, 1);
			}

			if (this.ObjectModifier.IsField(selectedObject) ||
				this.ObjectModifier.IsCommand(selectedObject) ||
				this.ObjectModifier.IsBox(selectedObject))
			{
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormSeparatorBottom, "Lien suivant", Res.Types.FieldDescription.SeparatorType);
			}

			if (this.ObjectModifier.IsField(selectedObject) ||
				this.ObjectModifier.IsBox(selectedObject) ||
				this.ObjectModifier.IsGlue(selectedObject))
			{
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormBackColor, "Couleur fond", Res.Types.FieldDescription.BackColorType);
			}

			if (this.ObjectModifier.IsField(selectedObject) ||
				this.ObjectModifier.IsBox(selectedObject))
			{
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormLabelFontColor, "Couleur étiquette", Res.Types.FieldDescription.FontColorType);
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormFieldFontColor, "Couleur champ", Res.Types.FieldDescription.FontColorType);
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormLabelFontFace, "Police étiquette", Res.Types.FieldDescription.FontFaceType);
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormFieldFontFace, "Police champ", Res.Types.FieldDescription.FontFaceType);
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormLabelFontStyle, "Style étiquette", Res.Types.FieldDescription.FontStyleType);
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormFieldFontStyle, "Style champ", Res.Types.FieldDescription.FontStyleType);
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormLabelFontSize, "Taille étiquette", Res.Types.FieldDescription.FontSizeType);
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormFieldFontSize, "Taille champ", Res.Types.FieldDescription.FontSizeType);
			}

			if (this.ObjectModifier.IsTitle(selectedObject) ||
				this.ObjectModifier.IsCommand(selectedObject))
			{
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormLabelFontColor, "Couleur texte", Res.Types.FieldDescription.FontColorType);
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormLabelFontFace, "Police texte", Res.Types.FieldDescription.FontFaceType);
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormLabelFontStyle, "Style texte", Res.Types.FieldDescription.FontStyleType);
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormLabelFontSize, "Taille texte", Res.Types.FieldDescription.FontSizeType);
			}

			if (this.ObjectModifier.IsBox(selectedObject))
			{
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormBoxLayout, "Contenu", Res.Types.FieldDescription.BoxLayoutType);
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormBoxPadding, "Marges int", Res.Types.FieldDescription.BoxPaddingType);
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormBoxFrameState, "Cadre", Res.Types.FieldDescription.FrameState);
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormBoxFrameWidth, "Epaisseur", 0, 5, 1, 1);
			}

			if (this.ObjectModifier.IsCommand(selectedObject))
			{
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormButtonClass, "Type commande", Res.Types.FieldDescription.CommandButtonClass);
			}

			return list;
		}

		protected void AddValue(List<AbstractValue> list, Widget selectedObject, AbstractProxy.Type type, string label, double min, double max, double step, double resolution)
		{
			this.SuspendChanges();

			try
			{
				ValueNumeric value = new ValueNumeric(min, max, step, resolution);
				value.SelectedObjects.Add(selectedObject);
				value.Type = type;
				value.Label = label;
				value.ValueChanged += new EventHandler(this.HandleValueChanged);
				this.SendObjectToValue(value);

				list.Add(value);
			}
			finally
			{
				this.ResumeChanges();
			}
		}

		protected void AddValue(List<AbstractValue> list, Widget selectedObject, AbstractProxy.Type type, string label, Types.EnumType enumType)
		{
			this.SuspendChanges();

			try
			{
				ValueEnum value = new ValueEnum(enumType);
				value.SelectedObjects.Add(selectedObject);
				value.Type = type;
				value.Label = label;
				value.ValueChanged += new EventHandler(this.HandleValueChanged);
				this.SendObjectToValue(value);

				list.Add(value);
			}
			finally
			{
				this.ResumeChanges();
			}
		}

		private void HandleValueChanged(object sender)
		{
			if (this.IsNotSuspended)
			{
				AbstractValue value = sender as AbstractValue;
				this.SendValueToObject(value);
			}
		}


		protected void SendObjectToValue(AbstractValue value)
		{
			//	Tous les objets ont la même valeur. Il suffit donc de s'occuper du premier objet.
			Widget selectedObject = value.SelectedObjects[0];

			switch (value.Type)
			{
				case AbstractProxy.Type.FormColumnsRequired:
					value.Value = this.ObjectModifier.GetColumnsRequired(selectedObject);
					break;

				case AbstractProxy.Type.FormRowsRequired:
					value.Value = this.ObjectModifier.GetRowsRequired(selectedObject);
					break;

				case AbstractProxy.Type.FormPreferredWidth:
					value.Value = this.ObjectModifier.GetPreferredWidth(selectedObject);
					break;

				case AbstractProxy.Type.FormSeparatorBottom:
					value.Value = this.ObjectModifier.GetSeparatorBottom(selectedObject);
					break;

				case AbstractProxy.Type.FormBackColor:
					value.Value = this.ObjectModifier.GetBackColor(selectedObject);
					break;

				case AbstractProxy.Type.FormLabelFontColor:
					value.Value = this.ObjectModifier.GetLabelFontColor(selectedObject);
					break;

				case AbstractProxy.Type.FormFieldFontColor:
					value.Value = this.ObjectModifier.GetFieldFontColor(selectedObject);
					break;

				case AbstractProxy.Type.FormLabelFontFace:
					value.Value = this.ObjectModifier.GetLabelFontFace(selectedObject);
					break;

				case AbstractProxy.Type.FormFieldFontFace:
					value.Value = this.ObjectModifier.GetFieldFontFace(selectedObject);
					break;

				case AbstractProxy.Type.FormLabelFontStyle:
					value.Value = this.ObjectModifier.GetLabelFontStyle(selectedObject);
					break;

				case AbstractProxy.Type.FormFieldFontStyle:
					value.Value = this.ObjectModifier.GetFieldFontStyle(selectedObject);
					break;

				case AbstractProxy.Type.FormLabelFontSize:
					value.Value = this.ObjectModifier.GetLabelFontSize(selectedObject);
					break;

				case AbstractProxy.Type.FormFieldFontSize:
					value.Value = this.ObjectModifier.GetFieldFontSize(selectedObject);
					break;

				case AbstractProxy.Type.FormBoxLayout:
					value.Value = this.ObjectModifier.GetBoxLayout(selectedObject);
					break;

				case AbstractProxy.Type.FormBoxPadding:
					value.Value = this.ObjectModifier.GetBoxPadding(selectedObject);
					break;

				case AbstractProxy.Type.FormBoxFrameState:
					value.Value = this.ObjectModifier.GetBoxFrameState(selectedObject);
					break;

				case AbstractProxy.Type.FormBoxFrameWidth:
					value.Value = this.ObjectModifier.GetBoxFrameWidth(selectedObject);
					break;

				case AbstractProxy.Type.FormButtonClass:
					value.Value = this.ObjectModifier.GetCommandButtonClass(selectedObject);
					break;
			}
		}

		protected void SendValueToObject(AbstractValue value)
		{
			foreach (Widget selectedObject in value.SelectedObjects)
			{
				switch (value.Type)
				{
					case AbstractProxy.Type.FormColumnsRequired:
						this.ObjectModifier.SetColumnsRequired(selectedObject, (int) value.Value);
						break;

					case AbstractProxy.Type.FormRowsRequired:
						this.ObjectModifier.SetRowsRequired(selectedObject, (int) value.Value);
						break;

					case AbstractProxy.Type.FormPreferredWidth:
						this.ObjectModifier.SetPreferredWidth(selectedObject, (double) value.Value);
						break;

					case AbstractProxy.Type.FormSeparatorBottom:
						this.ObjectModifier.SetSeparatorBottom(selectedObject, (FormEngine.FieldDescription.SeparatorType) value.Value);
						break;

					case AbstractProxy.Type.FormBackColor:
						this.ObjectModifier.SetBackColor(selectedObject, (FormEngine.FieldDescription.BackColorType) value.Value);
						break;

					case AbstractProxy.Type.FormLabelFontColor:
						this.ObjectModifier.SetLabelFontColor(selectedObject, (FormEngine.FieldDescription.FontColorType) value.Value);
						break;

					case AbstractProxy.Type.FormFieldFontColor:
						this.ObjectModifier.SetFieldFontColor(selectedObject, (FormEngine.FieldDescription.FontColorType) value.Value);
						break;

					case AbstractProxy.Type.FormLabelFontFace:
						this.ObjectModifier.SetLabelFontFace(selectedObject, (FormEngine.FieldDescription.FontFaceType) value.Value);
						break;

					case AbstractProxy.Type.FormFieldFontFace:
						this.ObjectModifier.SetFieldFontFace(selectedObject, (FormEngine.FieldDescription.FontFaceType) value.Value);
						break;

					case AbstractProxy.Type.FormLabelFontStyle:
						this.ObjectModifier.SetLabelFontStyle(selectedObject, (FormEngine.FieldDescription.FontStyleType) value.Value);
						break;

					case AbstractProxy.Type.FormFieldFontStyle:
						this.ObjectModifier.SetFieldFontStyle(selectedObject, (FormEngine.FieldDescription.FontStyleType) value.Value);
						break;

					case AbstractProxy.Type.FormLabelFontSize:
						this.ObjectModifier.SetLabelFontSize(selectedObject, (FormEngine.FieldDescription.FontSizeType) value.Value);
						break;

					case AbstractProxy.Type.FormFieldFontSize:
						this.ObjectModifier.SetFieldFontSize(selectedObject, (FormEngine.FieldDescription.FontSizeType) value.Value);
						break;

					case AbstractProxy.Type.FormBoxLayout:
						this.ObjectModifier.SetBoxLayout(selectedObject, (FormEngine.FieldDescription.BoxLayoutType) value.Value);
						break;

					case AbstractProxy.Type.FormBoxPadding:
						this.ObjectModifier.SetBoxPadding(selectedObject, (FormEngine.FieldDescription.BoxPaddingType) value.Value);
						break;

					case AbstractProxy.Type.FormBoxFrameState:
						this.ObjectModifier.SetBoxFrameState(selectedObject, (FrameState) value.Value);
						break;

					case AbstractProxy.Type.FormBoxFrameWidth:
						this.ObjectModifier.SetBoxFrameWidth(selectedObject, (double) value.Value);
						break;

					case AbstractProxy.Type.FormButtonClass:
						this.ObjectModifier.SetCommandButtonClass(selectedObject, (FormEngine.FieldDescription.CommandButtonClass) value.Value);
						break;
				}
			}

			Application.QueueAsyncCallback(this.ObjectModifier.FormEditor.RegenerateForm);
			this.ObjectModifier.FormEditor.Module.AccessForms.SetLocalDirty();
		}


		protected FormEditor.ObjectModifier ObjectModifier
		{
			get
			{
				return this.objectModifier as FormEditor.ObjectModifier;
			}
		}
	}
}
