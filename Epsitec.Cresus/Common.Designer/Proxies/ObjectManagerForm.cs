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
				ValueNumeric columnsRequired = new ValueNumeric(1, 10, 1, 1);
				columnsRequired.Name = "ColumnsRequired";
				columnsRequired.Label = "Nb de colonnes";
				columnsRequired.Value = this.ObjectModifier.GetColumnsRequired(selectedObject);
				list.Add(columnsRequired);
			}

			if (this.ObjectModifier.IsField(selectedObject) ||
				this.ObjectModifier.IsBox(selectedObject))
			{
				ValueNumeric rowsRequired = new ValueNumeric(1, 10, 1, 1);
				rowsRequired.Name = "RowsRequired";
				rowsRequired.Label = "Nb de lignes";
				rowsRequired.Value = this.ObjectModifier.GetRowsRequired(selectedObject);
				list.Add(rowsRequired);
			}


			if (this.ObjectModifier.IsField(selectedObject) ||
				this.ObjectModifier.IsCommand(selectedObject) ||
				this.ObjectModifier.IsBox(selectedObject) ||
				this.ObjectModifier.IsGlue(selectedObject) ||
				this.ObjectModifier.IsTitle(selectedObject) ||
				this.ObjectModifier.IsLine(selectedObject))
			{
				ValueNumeric preferredWidth = new ValueNumeric(1, 1000, 1, 1);
				preferredWidth.Name = "PreferredWidth";
				preferredWidth.Label = "Largeur";
				preferredWidth.Value = this.ObjectModifier.GetPreferredWidth(selectedObject);
				list.Add(preferredWidth);
			}

			if (this.ObjectModifier.IsField(selectedObject) ||
				this.ObjectModifier.IsCommand(selectedObject) ||
				this.ObjectModifier.IsBox(selectedObject))
			{
				ValueEnum separatorBottom = new ValueEnum(Res.Types.FieldDescription.SeparatorType);
				separatorBottom.Name = "SeparatorBottom";
				separatorBottom.Label = "Séparateur";
				separatorBottom.Value = this.ObjectModifier.GetSeparatorBottom(selectedObject);
				list.Add(separatorBottom);
			}

			if (this.ObjectModifier.IsField(selectedObject) ||
				this.ObjectModifier.IsBox(selectedObject) ||
				this.ObjectModifier.IsGlue(selectedObject))
			{
				ValueEnum backColor = new ValueEnum(Res.Types.FieldDescription.BackColorType);
				backColor.Name = "BackColor";
				backColor.Label = "Couleur fond";
				backColor.Value = this.ObjectModifier.GetBackColor(selectedObject);
				list.Add(backColor);
			}

			if (this.ObjectModifier.IsField(selectedObject) ||
				this.ObjectModifier.IsBox(selectedObject))
			{
				ValueEnum labelFontColor = new ValueEnum(Res.Types.FieldDescription.FontColorType);
				labelFontColor.Name = "LabelFontColor";
				labelFontColor.Label = "Couleur étiquette";
				labelFontColor.Value = this.ObjectModifier.GetLabelFontColor(selectedObject);
				list.Add(labelFontColor);

				ValueEnum fieldFontColor = new ValueEnum(Res.Types.FieldDescription.FontColorType);
				fieldFontColor.Name = "FieldFontColor";
				fieldFontColor.Label = "Couleur champ";
				fieldFontColor.Value = this.ObjectModifier.GetFieldFontColor(selectedObject);
				list.Add(fieldFontColor);
			}

			return list;
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
