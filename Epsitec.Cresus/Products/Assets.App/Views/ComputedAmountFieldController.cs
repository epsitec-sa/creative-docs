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
						if (this.ignoreChanges.IsZero)
						{
							using (this.ignoreChanges.Enter ())
							{
								this.controller.ComputedAmount = this.value;
							}
						}
						else
						{
							using (this.ignoreChanges.Enter ())
							{
								this.controller.ComputedAmountNoEditing = this.value;
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
			this.OnValueEdited ();
		}

		protected override void UpdatePropertyState()
		{
			base.UpdatePropertyState ();

			if (this.controller != null)
			{
				this.controller.BackgroundColor = this.BackgroundColor;
			}
		}


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.controller = new ComputedAmountController ();
			this.controller.CreateUI (this.frameBox);
			this.controller.IsReadOnly = this.PropertyState == PropertyState.Readonly;
			this.controller.ComputedAmount = this.value.HasValue ? this.value : new ComputedAmount ();

			this.controller.ValueEdited += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					using (this.ignoreChanges.Enter ())
					{
						this.Value = this.controller.ComputedAmount;
						this.OnValueEdited ();
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
		}


		private ComputedAmountController		controller;
		private ComputedAmount?					value;
	}
}
