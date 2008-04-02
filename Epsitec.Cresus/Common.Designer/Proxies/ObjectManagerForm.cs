using System.Collections.Generic;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Designer.Proxies
{
	/// <summary>
	/// Cette classe permet...
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
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormSeparatorBottom, "Séparateur", Res.Types.FieldDescription.SeparatorType);
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
				}
			}

			Application.QueueAsyncCallback(this.ObjectModifier.FormEditor.RegenerateForm);
			this.ObjectModifier.FormEditor.Module.AccessForms.SetLocalDirty();
		}

		private void HandleValueChanged(object sender)
		{
			if (this.IsNotSuspended)
			{
				AbstractValue value = sender as AbstractValue;
				this.SendValueToObject(value);
			}
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
