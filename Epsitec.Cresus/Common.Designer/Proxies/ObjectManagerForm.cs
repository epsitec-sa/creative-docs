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
				this.AddValue(list, selectedObject, "ColumnsRequired", "Nb de colonnes", 1, 10, 1, 1);
			}

			if (this.ObjectModifier.IsField(selectedObject) ||
				this.ObjectModifier.IsBox(selectedObject))
			{
				this.AddValue(list, selectedObject, "RowsRequired", "Nb de lignes", 1, 10, 1, 1);
			}

			if (this.ObjectModifier.IsField(selectedObject) ||
				this.ObjectModifier.IsCommand(selectedObject) ||
				this.ObjectModifier.IsBox(selectedObject) ||
				this.ObjectModifier.IsGlue(selectedObject) ||
				this.ObjectModifier.IsTitle(selectedObject) ||
				this.ObjectModifier.IsLine(selectedObject))
			{
				this.AddValue(list, selectedObject, "PreferredWidth", "Largeur", 1, 1000, 1, 1);
			}

			if (this.ObjectModifier.IsField(selectedObject) ||
				this.ObjectModifier.IsCommand(selectedObject) ||
				this.ObjectModifier.IsBox(selectedObject))
			{
				this.AddValue(list, selectedObject, "SeparatorBottom", "Séparateur", Res.Types.FieldDescription.SeparatorType);
			}

			if (this.ObjectModifier.IsField(selectedObject) ||
				this.ObjectModifier.IsBox(selectedObject) ||
				this.ObjectModifier.IsGlue(selectedObject))
			{
				this.AddValue(list, selectedObject, "BackColor", "Couleur fond", Res.Types.FieldDescription.BackColorType);
			}

			if (this.ObjectModifier.IsField(selectedObject) ||
				this.ObjectModifier.IsBox(selectedObject))
			{
				this.AddValue(list, selectedObject, "LabelFontColor", "Couleur étiquette", Res.Types.FieldDescription.FontColorType);
				this.AddValue(list, selectedObject, "FieldFontColor", "Couleur champ", Res.Types.FieldDescription.FontColorType);
			}

			return list;
		}

		protected void AddValue(List<AbstractValue> list, Widget selectedObject, string name, string label, double min, double max, double step, double resolution)
		{
			ValueNumeric value = new ValueNumeric(min, max, step, resolution);
			value.SelectedObjects.Add(selectedObject);
			value.Name = name;
			value.Label = label;
			value.ValueChanged += new EventHandler(this.HandleValueChanged);
			this.SendObjectToValue(value);

			list.Add(value);
		}

		protected void AddValue(List<AbstractValue> list, Widget selectedObject, string name, string label, Types.EnumType enumType)
		{
			ValueEnum value = new ValueEnum(enumType);
			value.SelectedObjects.Add(selectedObject);
			value.Name = name;
			value.Label = label;
			value.ValueChanged += new EventHandler(this.HandleValueChanged);
			this.SendObjectToValue(value);

			list.Add(value);
		}

		protected void SendObjectToValue(AbstractValue value)
		{
			//	Tous les objets ont la même valeur. Il suffit donc de s'occuper du premier objet.
			Widget selectedObject = value.SelectedObjects[0];

			switch (value.Name)
			{
				case "ColumnsRequired":
					value.Value = this.ObjectModifier.GetColumnsRequired(selectedObject);
					break;

				case "RowsRequired":
					value.Value = this.ObjectModifier.GetRowsRequired(selectedObject);
					break;

				case "PreferredWidth":
					value.Value = this.ObjectModifier.GetPreferredWidth(selectedObject);
					break;

				case "SeparatorBottom":
					value.Value = this.ObjectModifier.GetSeparatorBottom(selectedObject);
					break;

				case "BackColor":
					value.Value = this.ObjectModifier.GetBackColor(selectedObject);
					break;

				case "LabelFontColor":
					value.Value = this.ObjectModifier.GetLabelFontColor(selectedObject);
					break;

				case "FieldFontColor":
					value.Value = this.ObjectModifier.GetFieldFontColor(selectedObject);
					break;
			}
		}

		protected void SendValueToObject(AbstractValue value)
		{
			foreach (Widget selectedObject in value.SelectedObjects)
			{
				switch (value.Name)
				{
					case "ColumnsRequired":
						this.ObjectModifier.SetColumnsRequired(selectedObject, (int) value.Value);
						break;

					case "RowsRequired":
						this.ObjectModifier.SetRowsRequired(selectedObject, (int) value.Value);
						break;

					case "PreferredWidth":
						this.ObjectModifier.SetPreferredWidth(selectedObject, (double) value.Value);
						break;

					case "SeparatorBottom":
						this.ObjectModifier.SetSeparatorBottom(selectedObject, (FormEngine.FieldDescription.SeparatorType) value.Value);
						break;

					case "BackColor":
						this.ObjectModifier.SetBackColor(selectedObject, (FormEngine.FieldDescription.BackColorType) value.Value);
						break;

					case "LabelFontColor":
						this.ObjectModifier.SetLabelFontColor(selectedObject, (FormEngine.FieldDescription.FontColorType) value.Value);
						break;

					case "FieldFontColor":
						this.ObjectModifier.SetFieldFontColor(selectedObject, (FormEngine.FieldDescription.FontColorType) value.Value);
						break;
				}
			}
		}

		private void HandleValueChanged(object sender)
		{
			AbstractValue value = sender as AbstractValue;
			this.SendValueToObject(value);
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
