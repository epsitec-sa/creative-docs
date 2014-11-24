//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.FieldControllers
{
	public class StringFieldController : AbstractFieldController
	{
		public StringFieldController(DataAccessor accessor)
			: base (accessor)
		{
			this.maxLength = 500;
		}


		public int								LineCount = 1;

		public string							Value
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
								this.textField.Text = this.value;
								this.textField.SelectAll ();

								this.UpdateError ();
							}
						}
					}
				}
			}
		}

		public int								MaxLength
		{
			get
			{
				return this.maxLength;
			}
			set
			{
				this.maxLength = value;

				if (this.textField != null)
				{
					this.textField.MaxLength = this.maxLength;
				}
			}
		}

		private void UpdateValue()
		{
			using (this.ignoreChanges.Enter ())
			{
				this.textField.Text = this.value;
				this.textField.SelectAll ();

				this.UpdateError ();
			}
		}

		private void UpdateError()
		{
			if (this.Required)
			{
				bool error = string.IsNullOrEmpty (this.value);
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

			var type = AbstractFieldController.GetFieldColorType (this.propertyState, isLocked: this.isReadOnly, isError: this.hasError);
			AbstractFieldController.UpdateTextField (this.textField, type, this.isReadOnly);
		}


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			if (this.LineCount == 1)
			{
				this.textField = new TextField
				{
					Parent          = this.frameBox,
					Dock            = DockStyle.Left,
					PreferredWidth  = this.EditWidth,
					PreferredHeight = AbstractFieldController.lineHeight,
					TabIndex        = ++this.TabIndex,
					Text            = this.value,
					MaxLength       = this.maxLength,
				};
			}
			else
			{
				this.textField = new TextFieldMulti
				{
					Parent          = this.frameBox,
					Dock            = DockStyle.Left,
					PreferredWidth  = this.EditWidth,
					PreferredHeight = StringFieldController.GetMultiHeight (this.LineCount),
					TabIndex        = ++this.TabIndex,
					Text            = this.value,
					MaxLength       = this.maxLength,
				};
			}

			this.UpdateError ();
			this.UpdatePropertyState ();

			this.textField.TextChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					using (this.ignoreChanges.Enter ())
					{
						this.Value = this.textField.Text;
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


		private static int GetMultiHeight(int lineCount)
		{
			//	Calcul empyrique de la hauteur requise pour le widget TextFieldMulti.
			return 7 + lineCount*15;
		}


		private AbstractTextField				textField;
		private string							value;
		private int								maxLength;
	}
}
