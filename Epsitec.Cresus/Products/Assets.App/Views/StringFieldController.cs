//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class StringFieldController : AbstractFieldController
	{
		public string Value
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
						this.textField.Text = this.value;
					}
				}
			}
		}

		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.textField = new TextField
			{
				Parent         = this.frameBox,
				Dock           = DockStyle.Left,
				PreferredWidth = 300,
				Margins        = new Margins (0, 10, 0, 0),
				Text           = this.value,
			};

			this.textField.TextChanged += delegate
			{
				this.value = this.textField.Text;
				this.OnValueChanged ();
			};
		}


		private TextField						textField;
		private string							value;
	}
}
