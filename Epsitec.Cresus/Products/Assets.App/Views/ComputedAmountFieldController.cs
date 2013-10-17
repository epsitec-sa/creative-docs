//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ComputedAmountFieldController : AbstractFieldController
	{
		public ComputedAmount?					Value
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

					if (this.controller != null)
					{
						this.controller.ComputedAmount = this.value;
					}
				}
			}
		}


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.controller = new ComputedAmountController ();
			this.controller.CreateUI (this.frameBox);
			this.controller.IsReadOnly = this.PropertyState == PropertyState.Readonly;
			this.controller.ComputedAmount = this.value == null ? new ComputedAmount () : this.value;

			this.controller.ValueChanged += delegate
			{
				this.value = this.controller.ComputedAmount;
				this.OnValueChanged ();
			};
		}

		public override void SetFocus()
		{
			this.controller.SetFocus ();
		}


		private ComputedAmountController		controller;
		private ComputedAmount?					value;
	}
}
