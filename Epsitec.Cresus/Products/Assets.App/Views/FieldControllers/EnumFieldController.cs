//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.FieldControllers
{
	public class EnumFieldController : AbstractFieldController
	{
		public EnumFieldController(DataAccessor accessor)
			: base (accessor)
		{
		}


		public Dictionary<int, string>			Enums
		{
			get
			{
				return this.enums;
			}
			set
			{
				this.enums = value;
			}
		}


		public int?								Value
		{
			get
			{
				return this.value;
			}
			set
			{
				if (this.value != value)
				{
					this.value = value;

					if (this.textField != null)
					{
						if (this.ignoreChanges.IsZero)
						{
							using (this.ignoreChanges.Enter ())
							{
								this.textField.Text = this.IntToString (this.value);
								this.textField.SelectAll ();

								this.UpdateError ();
							}
						}
					}
				}
			}
		}

		private void UpdateValue()
		{
			using (this.ignoreChanges.Enter ())
			{
				this.textField.Text = this.IntToString (this.value);
				this.textField.SelectAll ();

				this.UpdateError ();
			}
		}

		private void UpdateError()
		{
			if (this.Required)
			{
				bool error = !this.value.HasValue;
				if (this.hasError != error)
				{
					this.hasError = error;
					this.UpdatePropertyState ();
				}
			}
		}

		protected override void ClearValue()
		{
			this.Value = null;
			this.OnValueEdited (this.Field);
		}

		protected override void UpdatePropertyState()
		{
			base.UpdatePropertyState ();

			var type = AbstractFieldController.GetFieldColorType (this.propertyState, this.hasError);
			AbstractFieldController.UpdateCombo (this.textField, type, this.isReadOnly);
		}


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.textField = new TextFieldCombo
			{
				Parent          = this.frameBox,
				Dock            = DockStyle.Left,
				PreferredWidth  = this.EditWidth,
				PreferredHeight = AbstractFieldController.lineHeight,
				TabIndex        = ++this.TabIndex,
				Text            = this.IntToString (this.value),
			};

			this.UpdateCombo ();
			this.UpdateError ();
			this.UpdatePropertyState ();

			this.textField.TextChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					using (this.ignoreChanges.Enter ())
					{
						this.Value = this.StringToInt (this.textField.Text);
						this.UpdateError ();
						this.OnValueEdited (this.Field);
					}
				}
			};

			this.textField.KeyboardFocusChanged += delegate (object sender, DependencyPropertyChangedEventArgs e)
			{
				bool focused = (bool) e.NewValue;

				if (focused)  // pris le focus ?
				{
					this.SetFocus ();
				}
				else  // perdu le focus ?
				{
					this.UpdateValue ();
				}
			};
		}

		public override void SetFocus()
		{
			this.textField.SelectAll ();
			this.textField.Focus ();

			base.SetFocus ();
		}


		private string IntToString(int? n)
		{
			if (n.HasValue)
			{
				string s;
				if (this.enums.TryGetValue (n.Value, out s))
				{
					return s;
				}
			}

			return null;
		}

		private int? StringToInt(string s)
		{
			foreach (var e in this.enums)
			{
				if (s == e.Value)
				{
					return e.Key;
				}
			}

			return null;
		}

		private void UpdateCombo()
		{
			this.textField.Items.Clear ();

			foreach (var e in this.enums)
			{
				this.textField.Items.Add (e.Value);
			}
		}


		private TextFieldCombo					textField;
		private int?							value;
		private Dictionary<int, string>			enums;
	}
}
