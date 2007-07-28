using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Dolphin
{
	/// <summary>
	/// Permet d'éditer une valeur hexadécimale.
	/// </summary>
	public class TextFieldHexa : AbstractGroup
	{
		public TextFieldHexa() : base()
		{
			this.label = new StaticText(this);
			this.label.ContentAlignment = ContentAlignment.MiddleLeft;
			this.label.PreferredWidth = 30;
			this.label.Margins = new Margins(5, 5, 0, 0);
			this.label.Dock = DockStyle.Left;

			this.textField = new TextField(this);
			this.textField.Dock = DockStyle.Left;
			this.textField.TextChanged += new EventHandler(this.HandleFieldTextChanged);
			this.textField.IsFocusedChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleFieldIsFocusedChanged);

			Panel buttons = new Panel(this);
			buttons.PreferredWidth = 170;
			buttons.Dock = DockStyle.Left;

			this.buttons = new List<Switch>();
			for (int i=0; i<12; i++)
			{
				Switch button = new Switch(buttons);
				button.Index = i;
				button.PreferredWidth = 10;
				button.PreferredHeight = 20;
				button.Margins = new Margins((i+1)%4 == 0 ? 10:1, 1, 1, 1);
				button.Dock = DockStyle.Right;
				button.Clicked += new MessageEventHandler(this.HandleButtonClicked);

				this.buttons.Add(button);
			}
		}

		public TextFieldHexa(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.label.Dispose();

				this.textField.TextChanged -= new EventHandler(this.HandleFieldTextChanged);
				this.textField.IsFocusedChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleFieldIsFocusedChanged);
				this.textField.Dispose();

				for (int i=0; i<12; i++)
				{
					this.buttons[i].Clicked -= new MessageEventHandler(this.HandleButtonClicked);
					this.buttons[i].Dispose();
				}
			}

			base.Dispose(disposing);
		}


		public void SetTabIndex(int index)
		{
			//	Spécifie l'ordre pour la navigation avec Tab.
			this.textField.TabIndex = index;
			this.textField.TabNavigationMode = TabNavigationMode.ActivateOnTab;
		}

		public int BitCount
		{
			//	Nombre de bits de la valeur (à priori 8 ou 12).
			get
			{
				return this.bitCount;
			}
			set
			{
				if (this.bitCount != value)
				{
					this.bitCount = value;

					double max = TextFieldHexa.GetHexaWidth(12);
					double width = TextFieldHexa.GetHexaWidth(this.bitCount);

					this.textField.PreferredWidth = width;
					this.textField.Margins = new Margins(max-width, 20, 0, 0);
					this.textField.MaxChar = (this.bitCount+3)/4;

					for (int i=0; i<this.buttons.Count; i++)
					{
						this.buttons[i].Visibility = (i < this.bitCount);
					}
				}
			}
		}

		public string Label
		{
			//	Label affiché à gauche.
			get
			{
				return this.baseName;
			}
			set
			{
				if (this.baseName != value)
				{
					this.baseName = value;
					this.label.Text = string.Concat("<b>", this.baseName, "</b>");;
				}
			}
		}

		public int HexaValue
		{
			//	Valeur courante.
			get
			{
				return TextFieldHexa.ParseHexa(this.textField.Text);
			}
			set
			{
				string text = this.GetHexaText(value);
				if (this.textField.Text != text)
				{
					this.textField.Text = text;
					this.UpdateButtons();
					this.OnHexaValueChanged();
				}
			}
		}


		protected string GetHexaText(int value)
		{
			//	Retourne le texte hexa de la valeur 
			string format = string.Format("X{0}", (this.bitCount+3)/4);  // "X2" si 8 bits, "X3" si 12 bits, etc.
			return value.ToString(format);
		}

		protected void UpdateButtons()
		{
			//	Met à jour les boutons binaires en fonction de la valeur actuelle.
			int value = this.HexaValue;

			for (int i=0; i<this.buttons.Count; i++)
			{
				this.buttons[i].ActiveState = (value & (1 << i)) == 0 ? ActiveState.No : ActiveState.Yes;
			}
		}

		protected static double GetHexaWidth(int bitCount)
		{
			//	Retourne la largeur nécessaire pour représenter un certain nombre de bits en hexa.
			return 10 + ((bitCount+3)/4)*10;
		}

		protected static int ParseHexa(string hexa)
		{
			int result;
			if (System.Int32.TryParse(hexa, System.Globalization.NumberStyles.AllowHexSpecifier, System.Globalization.CultureInfo.CurrentCulture, out result))
			{
				return result;
			}
			else
			{
				return 0;
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			Color color = this.BackColor;
			if (!color.IsEmpty)
			{
				graphics.AddFilledRectangle(this.Client.Bounds);
				graphics.RenderSolid(color);  // dessine un fond rouge si MarkPC
			}
		}


		private void HandleFieldTextChanged(object sender)
		{
			//	La valeur hexa éditée a changé.
			this.UpdateButtons();
			this.OnHexaValueChanged();
		}

		private void HandleFieldIsFocusedChanged(object sender, Types.DependencyPropertyChangedEventArgs e)
		{
			//	La ligne éditable a perdu le focus.
			bool focused = (bool) e.NewValue;
			if (!focused)
			{
				this.textField.Text = this.GetHexaText(this.HexaValue);  // remet un nombre propre ("a" devient "0A" par exemple)
			}
		}

		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			//	Switch binaire basculé.
			Switch button = sender as Switch;

			if (button.ActiveState == ActiveState.No)
			{
				button.ActiveState = ActiveState.Yes;
			}
			else
			{
				button.ActiveState = ActiveState.No;
			}

			int value = 0;
			for (int i=0; i<buttons.Count; i++)
			{
				if (buttons[i].ActiveState == ActiveState.Yes)
				{
					value |= (1 << i);
				}
			}
			this.HexaValue = value;
		}


		#region EventHandler
		protected virtual void OnHexaValueChanged()
		{
			//	Génère un événement pour dire qu'une cellule a été sélectionnée.
			EventHandler handler = (EventHandler) this.GetUserEventHandler("HexaValueChanged");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event Support.EventHandler HexaValueChanged
		{
			add
			{
				this.AddUserEventHandler("HexaValueChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("HexaValueChanged", value);
			}
		}
		#endregion


		protected int						bitCount = -1;
		protected string					baseName;

		protected StaticText				label;
		protected TextField					textField;
		protected List<Switch>				buttons;
	}
}
