//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

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
			this.label.ContentAlignment = ContentAlignment.MiddleRight;
			this.label.Margins = new Margins(0, 4, 0, 0);
			this.label.Dock = DockStyle.Fill;

			this.fieldModifier = new TextFieldCombo(this);
			this.fieldModifier.IsReadOnly = true;
			this.fieldModifier.TextChanged += new EventHandler(this.HandleFieldModifierTextChanged);
			this.fieldModifier.Margins = new Margins(0, 4, 0, 0);
			this.fieldModifier.Dock = DockStyle.Fill;
			this.UpdateFieldModifier();

			this.fieldCode = new TextFieldCombo(this);
			this.fieldCode.IsReadOnly = true;
			this.fieldCode.TextChanged += new EventHandler(this.HandleFieldCodeTextChanged);
			this.fieldCode.Dock = DockStyle.Fill;
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

				this.fieldModifier.TextChanged -= new EventHandler(this.HandleFieldModifierTextChanged);
				this.fieldModifier.Dispose();
				this.fieldModifier = null;

				this.fieldCode.TextChanged -= new EventHandler(this.HandleFieldCodeTextChanged);
				this.fieldCode.Dispose();
				this.fieldCode = null;
			}
			
			base.Dispose(disposing);
		}


		protected void UpdateFieldModifier()
		{
			this.ComboAdd(this.fieldModifier, KeyCode.ShiftKey);
			this.ComboAdd(this.fieldModifier, KeyCode.ControlKey);
			this.ComboAdd(this.fieldModifier, KeyCode.AltKey);

			this.ComboAdd(this.fieldModifier, KeyCode.ShiftKey | KeyCode.ControlKey);
			this.ComboAdd(this.fieldModifier, KeyCode.ShiftKey | KeyCode.AltKey);
			this.ComboAdd(this.fieldModifier, KeyCode.AltKey | KeyCode.ControlKey);
		}

		protected void UpdateFieldCode()
		{
			this.ComboAdd(this.fieldCode, KeyCode.AlphaA);
			this.ComboAdd(this.fieldCode, KeyCode.AlphaB);
			this.ComboAdd(this.fieldCode, KeyCode.AlphaC);
			this.ComboAdd(this.fieldCode, KeyCode.AlphaD);
			this.ComboAdd(this.fieldCode, KeyCode.AlphaE);
			this.ComboAdd(this.fieldCode, KeyCode.AlphaF);
			this.ComboAdd(this.fieldCode, KeyCode.AlphaG);
			this.ComboAdd(this.fieldCode, KeyCode.AlphaH);
			this.ComboAdd(this.fieldCode, KeyCode.AlphaI);
			this.ComboAdd(this.fieldCode, KeyCode.AlphaJ);
			this.ComboAdd(this.fieldCode, KeyCode.AlphaK);
			this.ComboAdd(this.fieldCode, KeyCode.AlphaL);
			this.ComboAdd(this.fieldCode, KeyCode.AlphaM);
			this.ComboAdd(this.fieldCode, KeyCode.AlphaN);
			this.ComboAdd(this.fieldCode, KeyCode.AlphaO);
			this.ComboAdd(this.fieldCode, KeyCode.AlphaP);
			this.ComboAdd(this.fieldCode, KeyCode.AlphaQ);
			this.ComboAdd(this.fieldCode, KeyCode.AlphaR);
			this.ComboAdd(this.fieldCode, KeyCode.AlphaS);
			this.ComboAdd(this.fieldCode, KeyCode.AlphaT);
			this.ComboAdd(this.fieldCode, KeyCode.AlphaU);
			this.ComboAdd(this.fieldCode, KeyCode.AlphaV);
			this.ComboAdd(this.fieldCode, KeyCode.AlphaW);
			this.ComboAdd(this.fieldCode, KeyCode.AlphaX);
			this.ComboAdd(this.fieldCode, KeyCode.AlphaY);
			this.ComboAdd(this.fieldCode, KeyCode.AlphaZ);

			this.ComboAdd(this.fieldCode, KeyCode.FuncF1);
			this.ComboAdd(this.fieldCode, KeyCode.FuncF2);
			this.ComboAdd(this.fieldCode, KeyCode.FuncF3);
			this.ComboAdd(this.fieldCode, KeyCode.FuncF4);
			this.ComboAdd(this.fieldCode, KeyCode.FuncF5);
			this.ComboAdd(this.fieldCode, KeyCode.FuncF6);
			this.ComboAdd(this.fieldCode, KeyCode.FuncF7);
			this.ComboAdd(this.fieldCode, KeyCode.FuncF8);
			this.ComboAdd(this.fieldCode, KeyCode.FuncF9);
			this.ComboAdd(this.fieldCode, KeyCode.FuncF10);
			this.ComboAdd(this.fieldCode, KeyCode.FuncF11);
			this.ComboAdd(this.fieldCode, KeyCode.FuncF12);

			this.ComboAdd(this.fieldCode, KeyCode.ArrowLeft);
			this.ComboAdd(this.fieldCode, KeyCode.ArrowRight);
			this.ComboAdd(this.fieldCode, KeyCode.ArrowUp);
			this.ComboAdd(this.fieldCode, KeyCode.ArrowDown);

			this.ComboAdd(this.fieldCode, KeyCode.Return);
			this.ComboAdd(this.fieldCode, KeyCode.Escape);

			this.ComboAdd(this.fieldCode, KeyCode.Insert);
			this.ComboAdd(this.fieldCode, KeyCode.Home);
			this.ComboAdd(this.fieldCode, KeyCode.Delete);
			this.ComboAdd(this.fieldCode, KeyCode.End);
			this.ComboAdd(this.fieldCode, KeyCode.PageUp);
			this.ComboAdd(this.fieldCode, KeyCode.PageDown);
		}

		protected void ComboAdd(TextFieldCombo combo, KeyCode code)
		{
			string text = Message.GetKeyName(code);
			combo.Items.Add(text);
		}

		
		void HandleFieldModifierTextChanged(object sender)
		{
		}

		void HandleFieldCodeTextChanged(object sender)
		{
		}


		#region Event handlers
		protected void OnEditedShortcutChanged()
		{
			EventHandler handler = (EventHandler) this.GetUserEventHandler("EditedShortcutChanged");

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
	}
}
