//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// Le widget ShortcutEditor permet de choisir un raccourci clavier (Shortcut).
	/// </summary>
	public class ShortcutEditor : Widget
	{
		public ShortcutEditor()
		{
			this.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;

			this.label = new StaticText(this);
			this.label.PreferredWidth = 90;
			this.label.ContentAlignment = ContentAlignment.MiddleRight;
			this.label.Margins = new Margins(0, 4, 0, 0);
			this.label.Dock = DockStyle.Left;

			this.fieldModifier = new TextFieldCombo(this);
			this.fieldModifier.IsReadOnly = true;
			this.fieldModifier.TextChanged += this.HandleFieldModifierTextChanged;
			this.fieldModifier.IsLiveUpdateEnabled = false;
			this.fieldModifier.PreferredWidth = 100;
			this.fieldModifier.Dock = DockStyle.Left;
			this.fieldModifier.TabIndex = 0;
			this.fieldModifier.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldModifier, Res.Strings.ShortcutEditor.Modifier.Tooltip);
			this.UpdateFieldModifier();

			StaticText link = new StaticText(this);
			link.Text = "+";
			link.PreferredWidth = 16;
			link.ContentAlignment = ContentAlignment.MiddleCenter;
			link.Dock = DockStyle.Left;

			this.fieldCode = new TextFieldCombo(this);
			this.fieldCode.IsReadOnly = true;
			this.fieldCode.TextChanged += this.HandleFieldCodeTextChanged;
			this.fieldCode.IsLiveUpdateEnabled = false;
			this.fieldCode.PreferredWidth = 100;
			this.fieldCode.Dock = DockStyle.Left;
			this.fieldCode.TabIndex = 1;
			this.fieldCode.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip (this.fieldCode, Res.Strings.ShortcutEditor.Code.Tooltip);
			this.UpdateFieldCode();
		}

		public ShortcutEditor(Widget embedder) : this ()
		{
			this.SetEmbedder(embedder);
		}


		public string Title
		{
			//	Texte du label placé à gauche.
			get
			{
				return this.label.Text;
			}

			set
			{
				this.label.Text = value;
			}
		}

		public Shortcut Shortcut
		{
			get
			{
				return this.shortcut;
			}

			set
			{
				if (this.shortcut != value)
				{
					this.shortcut = value;
					this.UpdateFields();
					this.OnEditedShortcutChanged();
				}
			}
		}


		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.label.Dispose();
				this.label = null;

				this.fieldModifier.TextChanged -= this.HandleFieldModifierTextChanged;
				this.fieldModifier.Dispose();
				this.fieldModifier = null;

				this.fieldCode.TextChanged -= this.HandleFieldCodeTextChanged;
				this.fieldCode.Dispose();
				this.fieldCode = null;
			}
			
			base.Dispose(disposing);
		}


		protected void UpdateFields()
		{
			//	Met à jour les TextFieldCombo en fonction du raccourci courant.
			this.isIgnoreChanging = true;

			this.fieldModifier.Text = ShortcutEditor.GetModifierText(this.shortcut.KeyCode);
			this.fieldCode.Text = ShortcutEditor.GetCodeText(this.shortcut.KeyCode);

			this.isIgnoreChanging = false;
		}

		protected void UpdateFieldModifier()
		{
			//	Met à jour le combo pour les touches modificatrices.
			this.fieldModifier.Items.Add(Res.Strings.ShortcutEditor.Modifier.None);

			this.listModifier = new List<KeyCode>();

			this.ComboAddModifier(KeyCode.ModifierShift);
			this.ComboAddModifier(KeyCode.ModifierControl);
			this.ComboAddModifier(KeyCode.ModifierAlt);

			this.ComboAddModifier(KeyCode.ModifierShift | KeyCode.ModifierControl);
			this.ComboAddModifier(KeyCode.ModifierShift | KeyCode.ModifierAlt);
			this.ComboAddModifier(KeyCode.ModifierAlt | KeyCode.ModifierControl);

			this.ComboAddModifier(KeyCode.ModifierShift | KeyCode.ModifierControl | KeyCode.ModifierAlt);
		}

		protected void UpdateFieldCode()
		{
			//	Met à jour le combo pour les touches principales.
			this.fieldCode.Items.Add(Res.Strings.ShortcutEditor.Code.None);

			this.listCode = new List<KeyCode>();

			this.ComboAddCode(KeyCode.AlphaA);
			this.ComboAddCode(KeyCode.AlphaB);
			this.ComboAddCode(KeyCode.AlphaC);
			this.ComboAddCode(KeyCode.AlphaD);
			this.ComboAddCode(KeyCode.AlphaE);
			this.ComboAddCode(KeyCode.AlphaF);
			this.ComboAddCode(KeyCode.AlphaG);
			this.ComboAddCode(KeyCode.AlphaH);
			this.ComboAddCode(KeyCode.AlphaI);
			this.ComboAddCode(KeyCode.AlphaJ);
			this.ComboAddCode(KeyCode.AlphaK);
			this.ComboAddCode(KeyCode.AlphaL);
			this.ComboAddCode(KeyCode.AlphaM);
			this.ComboAddCode(KeyCode.AlphaN);
			this.ComboAddCode(KeyCode.AlphaO);
			this.ComboAddCode(KeyCode.AlphaP);
			this.ComboAddCode(KeyCode.AlphaQ);
			this.ComboAddCode(KeyCode.AlphaR);
			this.ComboAddCode(KeyCode.AlphaS);
			this.ComboAddCode(KeyCode.AlphaT);
			this.ComboAddCode(KeyCode.AlphaU);
			this.ComboAddCode(KeyCode.AlphaV);
			this.ComboAddCode(KeyCode.AlphaW);
			this.ComboAddCode(KeyCode.AlphaX);
			this.ComboAddCode(KeyCode.AlphaY);
			this.ComboAddCode(KeyCode.AlphaZ);

			this.ComboAddCode(KeyCode.Digit0);
			this.ComboAddCode(KeyCode.Digit1);
			this.ComboAddCode(KeyCode.Digit2);
			this.ComboAddCode(KeyCode.Digit3);
			this.ComboAddCode(KeyCode.Digit4);
			this.ComboAddCode(KeyCode.Digit5);
			this.ComboAddCode(KeyCode.Digit6);
			this.ComboAddCode(KeyCode.Digit7);
			this.ComboAddCode(KeyCode.Digit8);
			this.ComboAddCode(KeyCode.Digit9);

			this.ComboAddCode(KeyCode.FuncF1);
			this.ComboAddCode(KeyCode.FuncF2);
			this.ComboAddCode(KeyCode.FuncF3);
			this.ComboAddCode(KeyCode.FuncF4);
			this.ComboAddCode(KeyCode.FuncF5);
			this.ComboAddCode(KeyCode.FuncF6);
			this.ComboAddCode(KeyCode.FuncF7);
			this.ComboAddCode(KeyCode.FuncF8);
			this.ComboAddCode(KeyCode.FuncF9);
			this.ComboAddCode(KeyCode.FuncF10);
			this.ComboAddCode(KeyCode.FuncF11);
			this.ComboAddCode(KeyCode.FuncF12);

			this.ComboAddCode(KeyCode.Space);
			this.ComboAddCode(KeyCode.Escape);
			this.ComboAddCode(KeyCode.Tab);
			this.ComboAddCode(KeyCode.Back);
			this.ComboAddCode(KeyCode.Return);

			this.ComboAddCode(KeyCode.Insert);
			this.ComboAddCode(KeyCode.Home);
			this.ComboAddCode(KeyCode.Delete);
			this.ComboAddCode(KeyCode.End);
			this.ComboAddCode(KeyCode.PageUp);
			this.ComboAddCode(KeyCode.PageDown);

			this.ComboAddCode(KeyCode.ArrowLeft);
			this.ComboAddCode(KeyCode.ArrowRight);
			this.ComboAddCode(KeyCode.ArrowUp);
			this.ComboAddCode(KeyCode.ArrowDown);

			this.ComboAddCode(KeyCode.Divide);
			this.ComboAddCode(KeyCode.Multiply);
			this.ComboAddCode(KeyCode.Substract);
			this.ComboAddCode(KeyCode.Add);
			this.ComboAddCode(KeyCode.Decimal);
		}

		protected void ComboAddModifier(KeyCode code)
		{
			//	Ajoute une touche modificatrice dans le menu-combo.
			this.fieldModifier.Items.Add(ShortcutEditor.GetModifierText(code));
			this.listModifier.Add(code);
		}

		protected void ComboAddCode(KeyCode code)
		{
			//	Ajoute une touche principale dans le menu-combo.
			this.fieldCode.Items.Add(ShortcutEditor.GetCodeText(code));
			this.listCode.Add(code);
		}

		static protected string GetModifierText(KeyCode code)
		{
			//	Retourne le texte pour une touche modificatrice.
			code &= KeyCode.ModifierMask;

			if (code == KeyCode.None)
			{
				return Res.Strings.ShortcutEditor.Modifier.None.ToString ();
			}
			else
			{
				code |= KeyCode.AlphaX;  // pour ne pas avoir "+None" à la fin
				string text = Message.GetKeyName(code);
				return text.Substring(0, text.Length-2);  // supprime le "+X" à la fin !
			}
		}

		static protected string GetCodeText(KeyCode code)
		{
			//	Retourne le texte pour une touche principale.
			code &= KeyCode.KeyCodeMask;

			if (code == KeyCode.None)
			{
				return Res.Strings.ShortcutEditor.Code.None.ToString ();
			}
			else
			{
				return Message.GetKeyName(code);
			}
		}

		protected KeyCode GetModifierKey()
		{
			//	Retourne la touche modificatrice choisie par le combo.
			for (int i=1; i<this.fieldModifier.Items.Count; i++)
			{
				string text = this.fieldModifier.Items[i];

				if (text == this.fieldModifier.Text)
				{
					return this.listModifier[i-1];  // -1 pour sauter "Aucun" en tête de liste
				}
			}

			return KeyCode.None;
		}

		protected KeyCode GetCodeKey()
		{
			//	Retourne la touche principale choisie par le combo.
			for (int i=1; i<this.fieldCode.Items.Count; i++)
			{
				string text = this.fieldCode.Items[i];

				if (text == this.fieldCode.Text)
				{
					return this.listCode[i-1];  // -1 pour sauver "Aucun" en tête de liste
				}
			}

			return KeyCode.None;
		}


		void HandleFieldModifierTextChanged(object sender)
		{
			//	Changement des touches modificatrices.
			if (this.isIgnoreChanging)
			{
				return;
			}

			KeyCode code = this.GetModifierKey();
			KeyCode full = this.Shortcut.KeyCode;
			full &= KeyCode.KeyCodeMask;
			full |= code;
			this.Shortcut = full;
		}

		void HandleFieldCodeTextChanged(object sender)
		{
			//	Changement des touches principales.
			if (this.isIgnoreChanging)
			{
				return;
			}

			KeyCode code = this.GetCodeKey();
			if (code == KeyCode.None)
			{
				this.Shortcut = KeyCode.None;
			}
			else
			{
				KeyCode full = this.Shortcut.KeyCode;
				full &= KeyCode.ModifierMask;
				full |= code;
				this.Shortcut = full;
			}
		}


		#region Event handlers
		protected void OnEditedShortcutChanged()
		{
			var handler = this.GetUserEventHandler("EditedShortcutChanged");

			if (handler != null)
			{
				handler(this);
			}
		}

		public event EventHandler EditedShortcutChanged
		{
			add
			{
				this.AddUserEventHandler("EditedShortcutChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("EditedShortcutChanged", value);
			}
		}
		#endregion


		protected Shortcut						shortcut;
		protected StaticText					label;
		protected TextFieldCombo				fieldModifier;
		protected TextFieldCombo				fieldCode;

		protected bool							isIgnoreChanging = false;
		protected List<KeyCode>					listModifier;
		protected List<KeyCode>					listCode;
	}
}
