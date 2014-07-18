﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
				this.textField.Text = this.value;
				this.textField.SelectAll ();
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

			AbstractFieldController.UpdateTextField (this.textField, this.propertyState, this.isReadOnly, this.hasError);
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
					TabIndex        = this.TabIndex,
					Text            = this.value,
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
					TabIndex        = this.TabIndex,
					Text            = this.value,
				};
			}

			this.UpdatePropertyState ();

			this.textField.TextChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					using (this.ignoreChanges.Enter ())
					{
						this.Value = this.textField.Text;
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
	}
}