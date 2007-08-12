//	Copyright © 2003-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.App.Dolphin.MyWidgets
{
	/// <summary>
	/// Permet d'éditer une valeur hexadécimale et binaire.
	/// </summary>
	public class TextFieldHexa : AbstractGroup
	{
		public TextFieldHexa() : base()
		{
			this.PreferredHeight = 20;

			this.label = new StaticText(this);
			this.label.ContentAlignment = ContentAlignment.MiddleLeft;
			this.label.PreferredHeight = 20;
			this.label.PreferredWidth = 25;
			this.label.Margins = new Margins(5, 3, 0, 0);
			this.label.Dock = DockStyle.Left;

			this.textField = new TextField(this);
			this.textField.Text = TextFieldHexa.initValue;
			this.textField.PreferredHeight = 20;
			this.textField.Dock = DockStyle.Left;
			this.textField.TextChanged += new EventHandler(this.HandleFieldTextChanged);
			this.textField.IsFocusedChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleFieldIsFocusedChanged);

			Panel pm = new Panel(this);
			pm.PreferredSize = new Size(14, 20);
			pm.Margins = new Margins(-1, 0, 0, 0);
			pm.Dock = DockStyle.Left;

			this.buttonPlus = new GlyphButton(pm);
			this.buttonPlus.GlyphShape = GlyphShape.TriangleUp;
			this.buttonPlus.ButtonStyle = ButtonStyle.UpDown;
			this.buttonPlus.PreferredSize = new Size(14, 11);
			this.buttonPlus.Margins = new Margins(0, 0, 0, -1);
			this.buttonPlus.Dock = DockStyle.Top;
			this.buttonPlus.Clicked += new MessageEventHandler(this.HandleButtonClicked);

			this.buttonMinus = new GlyphButton(pm);
			this.buttonMinus.GlyphShape = GlyphShape.TriangleDown;
			this.buttonMinus.ButtonStyle = ButtonStyle.UpDown;
			this.buttonMinus.PreferredSize = new Size(14, 11);
			this.buttonMinus.Margins = new Margins(0, 0, -1, 0);
			this.buttonMinus.Dock = DockStyle.Bottom;
			this.buttonMinus.Clicked += new MessageEventHandler(this.HandleButtonClicked);

			this.buttonClear = new GlyphButton(this);
			this.buttonClear.GlyphShape = GlyphShape.Close;
			this.buttonClear.ButtonStyle = ButtonStyle.UpDown;
			this.buttonClear.PreferredSize = new Size(18, 20);
			this.buttonClear.Margins = new Margins(-1, 0, 0, 0);
			this.buttonClear.Dock = DockStyle.Left;
			this.buttonClear.Clicked += new MessageEventHandler(this.HandleButtonClicked);

			Panel buttons = new Panel(this);
			buttons.PreferredHeight = 20;
			buttons.PreferredWidth = 256;
			buttons.Dock = DockStyle.Left;

			this.buttons = new List<PushButton>();
			for (int i=0; i<12; i++)
			{
				PushButton button = new PushButton(buttons);
				button.Index = i;
				button.PreferredWidth = 18;
				button.PreferredHeight = 20;
				button.Margins = new Margins((i+1)%4 == 0 ? 10:1, 0, 0, 0);
				button.Dock = DockStyle.Right;
				button.Clicked += new MessageEventHandler(this.HandleSwitchClicked);

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

				this.buttonPlus.Clicked -= new MessageEventHandler(this.HandleButtonClicked);
				this.buttonMinus.Clicked -= new MessageEventHandler(this.HandleButtonClicked);
				this.buttonClear.Clicked -= new MessageEventHandler(this.HandleButtonClicked);

				for (int i=0; i<12; i++)
				{
					this.buttons[i].Clicked -= new MessageEventHandler(this.HandleSwitchClicked);
					this.buttons[i].Dispose();
				}
			}

			base.Dispose(disposing);
		}


		public void SetTabIndex(int index)
		{
			//	Spécifie l'ordre pour la navigation avec Tab.
			//	Attention, il ne doit pas y avoir 2x les mêmes numéros, même dans des widgets de parents différents !
			this.textField.TabIndex = index;
			this.textField.TabNavigationMode = TabNavigationMode.None;  // gestion maison, dans MainPanel
		}

		public MemoryAccessor MemoryAccessor
		{
			//	MemoryAccessor associé au widget, facultatif.
			get
			{
				return this.memoryAccessor;
			}
			set
			{
				this.memoryAccessor = value;
			}
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
					this.textField.Margins = new Margins(max-width, 0, 0, 0);
					this.textField.MaxChar = (this.bitCount+3)/4;

					for (int i=0; i<this.buttons.Count; i++)
					{
						this.buttons[i].Visibility = (i < this.bitCount);
					}
				}
			}
		}

		public string BitNames
		{
			//	Noms des bits représentés.
			get
			{
				return this.bitNames;
			}
			set
			{
				this.bitNames = value;

				if (string.IsNullOrEmpty(this.bitNames))
				{
					//	Numérote les bits 0..n.
					for (int i=0; i<this.buttons.Count; i++)
					{
						this.buttons[i].Text = string.Concat("<font size=\"70%\">", i.ToString(), "</font>");
					}
				}
				else
				{
					//	Met une lettre dans chaque bouton.
					for (int i=0; i<this.buttons.Count; i++)
					{
						if (i < this.bitNames.Length)
						{
							string letter = this.bitNames.Substring(i, 1);
							this.buttons[i].Text = string.Concat("<font size=\"90%\"><b>", letter, "</b></font>");
						}
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
				return Misc.ParseHexa(this.textField.Text);
			}
			set
			{
#if false
				string text = this.GetHexaText(value);
				if (this.textField.Text != text)
				{
					this.textField.Text = text;
					this.UpdateButtons();
					this.OnHexaValueChanged();
				}
#else
				value &= (1 << this.bitCount)-1;

				int current = Misc.ParseHexa(this.textField.Text);
				if (current != value || this.textField.Text == TextFieldHexa.initValue)
				{
					this.textField.Text = this.GetHexaText(value);
					this.UpdateButtons();
					this.OnHexaValueChanged();
				}
#endif
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
			//	Retourne la largeur nécessaire pour représenter un certain nombre de bits en hexa dans un TextField.
			return 8 + ((bitCount+3)/4)*6;
		}


		protected MainPanel MainPanel
		{
			//	Retourne le premier parent de type MainPanel.
			get
			{
				Widget my = this;

				while (my.Parent != null)
				{
					my = my.Parent;
					if (my is MainPanel)
					{
						return my as MainPanel;
					}
				}

				return null;
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

		private void HandleFieldIsFocusedChanged(object sender, Common.Types.DependencyPropertyChangedEventArgs e)
		{
			//	La ligne éditable a pris ou perdu le focus.
			Widget widget = sender as Widget;
			bool focused = (bool) e.NewValue;

			if (focused)  // focus pris ?
			{
				MainPanel mp = this.MainPanel;
				if (mp != null)
				{
					mp.SetDolphinFocusedWidget(widget);
				}
			}
			else  // focus perdu ?
			{
				this.textField.Text = this.GetHexaText(this.HexaValue);  // remet un nombre propre ("a" devient "0A" par exemple)
			}
		}

		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			//	Bouton "+/-/0" cliqué.
			if (sender == this.buttonPlus)
			{
				this.HexaValue = this.HexaValue+1;
			}

			if (sender == this.buttonMinus)
			{
				this.HexaValue = this.HexaValue-1;
			}

			if (sender == this.buttonClear)
			{
				this.HexaValue = 0;
			}
		}

		private void HandleSwitchClicked(object sender, MessageEventArgs e)
		{
			//	Switch binaire basculé.
			PushButton button = sender as PushButton;

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

		public event Common.Support.EventHandler HexaValueChanged
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


		// Pour forcer l'initialisation la première fois qu'une valeur est donnée.
		protected static readonly string	initValue = "??";

		protected int						bitCount = -1;
		protected string					baseName;
		protected string					bitNames;
		protected MemoryAccessor			memoryAccessor;

		protected StaticText				label;
		protected TextField					textField;
		protected GlyphButton				buttonPlus;
		protected GlyphButton				buttonMinus;
		protected GlyphButton				buttonClear;
		protected List<PushButton>			buttons;
	}
}
