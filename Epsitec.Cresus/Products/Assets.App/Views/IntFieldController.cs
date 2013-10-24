﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class IntFieldController : AbstractFieldController
	{
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
						using (this.ignoreChanges.Enter ())
						{
							this.textField.Text = IntFieldController.ConvIntToString (this.value);
						}
					}

					this.OnValueChanged ();
				}
			}
		}

		protected override void ClearValue()
		{
			this.Value = null;
		}


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.textField = new TextField
			{
				Parent          = this.frameBox,
				PreferredWidth  = 50,
				PreferredHeight = AbstractFieldController.lineHeight,
				Dock            = DockStyle.Left,
				TabIndex        = this.TabIndex,
				Text            = IntFieldController.ConvIntToString (this.value),
				IsReadOnly      = this.PropertyState == PropertyState.Readonly,
			};

			this.textField.TextChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					this.Value = IntFieldController.ConvStringToInt (this.textField.Text);
				}
			};
		}

		public override void SetFocus()
		{
			this.textField.SelectAll ();
			this.textField.Focus ();
		}


		private static string ConvIntToString(int? value)
		{
			return Helpers.Converters.IntToString (value);
		}

		private static int? ConvStringToInt(string text)
		{
			return Helpers.Converters.ParseInt (text);
		}


		private TextField						textField;
		private int?								value;
	}
}