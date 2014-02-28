﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class AmortizedAmountFieldController : AbstractFieldController
	{
		public AmortizedAmount?					Value
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
						if (this.ignoreChanges.IsZero)
						{
							using (this.ignoreChanges.Enter ())
							{
								this.controller.AmortizedAmount = this.value;
							}
						}
						else
						{
							using (this.ignoreChanges.Enter ())
							{
								this.controller.AmortizedAmountNoEditing = this.value;
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
				this.controller.UpdateValue ();
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

			if (this.controller != null)
			{
				this.controller.BackgroundColor = this.BackgroundColor;
				this.controller.PropertyState = this.PropertyState;
			}
		}


		public override void CreateUI(Widget parent)
		{
			this.LabelWidth = 0;
			base.CreateUI (parent);

			this.controller = new AmortizedAmountController ();
			this.controller.CreateUI (this.frameBox);
			this.controller.IsReadOnly = this.PropertyState == PropertyState.Readonly;
			this.controller.AmortizedAmount = this.value;

			this.controller.ValueEdited += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					using (this.ignoreChanges.Enter ())
					{
						this.Value = this.controller.AmortizedAmount;
						this.OnValueEdited (this.Field);
					}
				}
			};

			this.controller.FocusLost += delegate
			{
				this.UpdateValue ();
			};
		}

		public override void SetFocus()
		{
			this.controller.SetFocus ();

			base.SetFocus ();
		}


		private AmortizedAmountController		controller;
		private AmortizedAmount?				value;
	}
}