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
		public ValueEnum(DesignerApplication application) : base(application)
		{
			throw new System.NotImplementedException();
		}

		public ValueEnum(DesignerApplication application, Types.EnumType enumType) : base(application)
		{
			this.enumType = enumType;
			this.enumValues = new List<Types.EnumValue>(this.enumType.Values);
		}


		public bool IsChoiceByMenu
		{
			//	Détermine si le choix dans l'énumération s'effectue avec un sous-menu.
			//	Ceci est nécessaire lorsque le nombre de valeurs de l'énumération dépasse 4 ou 5.
			get
			{
				return this.isChoiceByMenu;
			}
			set
			{
				this.isChoiceByMenu = value;
			}
		}


		public override Widget CreateInterface(Widget parent)
		{
			//	Crée les widgets permettant d'éditer la valeur.
			FrameBox box = new FrameBox(parent);
			ToolTip.Default.SetToolTip(box, this.caption.Description);

			if (!this.hasHiddenLabel)
			{
				StaticText label = new StaticText(box);
				label.Text = this.label;
				label.CaptionId = this.caption.Id;
				label.ContentAlignment = ContentAlignment.MiddleRight;
				label.Margins = new Margins(0, 5, 0, 0);
				label.Dock = DockStyle.Fill;
			}

			if (this.isChoiceByMenu)  // choix par un menu ?
			{
				this.buttonUnique = new IconButton(box);
				this.buttonUnique.ButtonStyle = ButtonStyle.Icon;
				this.buttonUnique.PreferredSize = new Size(22, 22);
				this.buttonUnique.Dock = DockStyle.Fill;
				this.buttonUnique.Pressed += this.HandleButtonMenuPressed;
				this.buttonUnique.Entered += this.HandleButtonUniqueEntered;
				this.buttonUnique.Exited += this.HandleButtonUniqueExited;

				this.buttonMenu = new GlyphButton(box);
				this.buttonMenu.GlyphShape = GlyphShape.Menu;
				this.buttonMenu.ButtonStyle = ButtonStyle.ToolItem;
				this.buttonMenu.PreferredSize = new Size(22, 8);
				this.buttonMenu.Dock = DockStyle.Bottom;
				this.buttonMenu.Margins = new Margins(0, 0, -1, 0);
				this.buttonMenu.Pressed += this.HandleButtonMenuPressed;

				if (this.hasHiddenLabel)
				{
					box.PreferredWidth = 22;
				}

				box.PreferredHeight = 22+8-1;
			}
			else  // choix direct ?
			{
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
				this.buttons.SelectionChanged += this.HandleButtonsSelectionChanged;
			}

			this.UpdateInterface();

			this.widgetInterface = box;
			return box;
		}


		#region Menu
		protected VMenu CreateMenu()
		{
			//	Crée le menu permettant de choisir une valeur de l'énumération.
			VMenu menu = new VMenu();

			System.Enum e = (System.Enum) this.value;
			Types.EnumValue selectedEnumValue = this.enumType[e];

			foreach (Types.EnumValue enumValue in this.enumType.Values)
			{
				if (enumValue.IsHidden)
				{
					continue;
				}

				Types.Caption caption = enumValue.Caption;
				bool active = (enumValue.Rank == selectedEnumValue.Rank);
	
				string icon = caption.Icon;
				string text = caption.Description;
				MenuItem item = new MenuItem("", icon, text, "", enumValue.Rank.ToString(System.Globalization.CultureInfo.InvariantCulture));
				item.ActiveState = active ? ActiveState.Yes : ActiveState.No;
				item.Pressed += this.HandleMenuItemPressed;

				menu.Items.Add(item);
			}

			menu.AdjustSize();
			return menu;
		}

		private void HandleButtonMenuPressed(object sender, MessageEventArgs e)
		{
			//	Le bouton pour choisir la valeur de l'énumération dans un menu a été pressé.
			VMenu menu = this.CreateMenu();
			if (menu != null)
			{
				menu.Host = this;
				menu.MinWidth = this.buttonMenu.ActualWidth;
				TextFieldCombo.AdjustComboSize(this.buttonMenu, menu, false);
				menu.ShowAsComboList(this.buttonMenu, Point.Zero, this.buttonMenu);
			}
		}

		private void HandleMenuItemPressed(object sender, MessageEventArgs ea)
		{
			//	Une case du menu a été pressée.
			MenuItem item = sender as MenuItem;
			int rank = int.Parse(item.Name, System.Globalization.CultureInfo.InvariantCulture);

			foreach (Types.EnumValue enumValue in this.enumType.Values)
			{
				if (enumValue.IsHidden)
				{
					continue;
				}

				if (enumValue.Rank == rank)
				{
					System.Enum e = enumValue.Value;

					if (this.value != e)
					{
						this.value = e;
						this.OnValueChanged();
						this.UpdateInterface();
					}

					break;
				}
			}
		}

		private void HandleButtonUniqueEntered(object sender, MessageEventArgs e)
		{
			this.buttonMenu.ButtonStyle = ButtonStyle.Icon;
		}

		private void HandleButtonUniqueExited(object sender, MessageEventArgs e)
		{
			this.buttonMenu.ButtonStyle = ButtonStyle.ToolItem;
		}
		#endregion


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
				System.Enum e;
				if (valueText == null)
				{
					e = System.Enum.ToObject(this.enumType.SystemType, 0) as System.Enum;
				}
				else
				{
					e = System.Enum.Parse(this.enumType.SystemType, valueText) as System.Enum;
				}

				if (this.value != e)
				{
					this.value = e;
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
			if (this.isChoiceByMenu)  // choix par un menu ?
			{
				if (this.buttonUnique != null)
				{
					if (this.enumType.IsDefinedAsFlags)
					{
						throw new System.NotImplementedException();
					}
					else
					{
						System.Enum e = (System.Enum) this.value;
						Types.EnumValue enumValue = this.enumType[e];
						Druid id = (enumValue == null) ? Druid.Empty : enumValue.CaptionId;

						this.buttonUnique.CaptionId = id;
					}
				}
			}
			else  // choix direct ?
			{
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
						int rank = (enumValue == null) ? -1 : enumValue.Rank;

						this.ignoreChange = true;
						this.buttons.SelectedValue = rank;
						this.ignoreChange = false;
					}
				}
			}
		}


		protected bool isChoiceByMenu;
		protected Types.EnumType enumType;
		protected List<Types.EnumValue> enumValues;
		protected RadioIconGrid buttons;
		protected IconButton buttonUnique;
		protected GlyphButton buttonMenu;
	}
}
