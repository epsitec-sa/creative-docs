using System.Collections.Generic;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Designer.Proxies
{
	/// <summary>
	/// Cette classe permet de représenter une valeur de type énumération.
	/// </summary>
	public class ValueEnum : AbstractValue
	{
		public ValueEnum()
		{
			throw new System.NotImplementedException();
		}

		public ValueEnum(Types.EnumType enumType)
		{
			this.enumType = enumType;
			this.enumValues = new List<Types.EnumValue>(this.enumType.Values);
		}


		public override Widget CreateInterface(Widget parent)
		{
			//	Crée les widgets permettant d'éditer la valeur.
			FrameBox box = new FrameBox(parent);

			StaticText label = new StaticText(box);
			label.Text = this.label;
			label.ContentAlignment = ContentAlignment.MiddleRight;
			label.Margins = new Margins(0, 5, 0, 0);
			label.Dock = DockStyle.Fill;

			this.buttons = this.enumType.IsDefinedAsFlags ? new CheckIconGrid(box) : new RadioIconGrid(box);

			int count = 0;
			foreach (Types.EnumValue enumValue in this.enumType.Values)
			{
				if (enumValue.IsHidden)
				{
					continue;
				}

				Types.Caption caption = enumValue.Caption;
				int value = this.enumType.IsDefinedAsFlags ? Types.EnumType.ConvertToInt(enumValue.Value) : enumValue.Rank;
				this.buttons.AddRadioIcon(caption.Icon, caption.Description, value, false);
				count++;
			}

			this.buttons.PreferredSize = this.buttons.GetBestFitSize();
			this.buttons.Dock = DockStyle.Right;
			this.buttons.SelectionChanged += new EventHandler(this.HandleButtonsSelectionChanged);

			this.UpdateInterface();

			return box;
		}

		private void HandleButtonsSelectionChanged(object sender)
		{
			if (this.ignoreChange)
			{
				return;
			}

			RadioIconGrid button = sender as RadioIconGrid;

			if (this.enumType.IsDefinedAsFlags)
			{
				int flags = button.SelectedValue;
				IEnumerable<Types.IEnumValue> enumValues = Types.EnumType.ConvertEnumValuesFromFlags(this.enumType, flags);
				string valueText = Types.EnumType.ConvertToString(enumValues);
				System.Enum valueEnum = System.Enum.Parse(this.enumType.SystemType, valueText) as System.Enum;

				if (this.value != valueEnum)
				{
					this.value = valueEnum;
					this.OnValueChanged();
				}
			}
			else
			{
				IEnumerable<Types.EnumValue> enumValues = this.enumType.Values;
				foreach (Types.EnumValue enumValue in enumValues)
				{
					if (enumValue.Rank == button.SelectedValue)
					{
						System.Enum e = enumValue.Value;

						if (this.value != e)
						{
							this.value = e;
							this.OnValueChanged();
						}

						break;
					}
				}
			}
		}

		protected override void UpdateInterface()
		{
			//	Met à jour la valeur dans l'interface.
			if (this.buttons != null)
			{
				if (this.enumType.IsDefinedAsFlags)
				{
					System.Enum e = (System.Enum) this.value;
					string valueText = e.ToString();
					IEnumerable<Types.IEnumValue> flagValues = Types.EnumType.ConvertFromString(this.enumType, valueText);
					int flags = Types.EnumType.ConvertEnumValuesToFlags(flagValues);

					this.ignoreChange = true;
					this.buttons.SelectedValue = flags;
					this.ignoreChange = false;
				}
				else
				{
					System.Enum e = (System.Enum) this.value;
					Types.EnumValue enumValue = this.enumType[e];

					this.ignoreChange = true;
					this.buttons.SelectedValue = enumValue.Rank;
					this.ignoreChange = false;
				}
			}
		}


		protected Types.EnumType enumType;
		protected List<Types.EnumValue> enumValues;
		protected RadioIconGrid buttons;
	}
}
