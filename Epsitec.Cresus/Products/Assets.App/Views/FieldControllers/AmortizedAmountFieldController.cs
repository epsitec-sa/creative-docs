//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Views.EditorPages;
using Epsitec.Cresus.Assets.App.Views.ViewStates;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.FieldControllers
{
	public class AmortizedAmountFieldController : AbstractFieldController
	{
		public AmortizedAmountFieldController(DataAccessor accessor)
			: base (accessor)
		{
		}


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
								this.controller.Value = this.value;
							}
						}
						else
						{
							using (this.ignoreChanges.Enter ())
							{
								this.controller.ValueNoEditing = this.value;
							}
						}
					}
				}
			}
		}

		public override IEnumerable<CommentaryType> CommentaryTypes
		{
			get
			{
				return this.controller.CommentaryTypes;
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
				this.controller.PropertyState = this.PropertyState;
				this.controller.IsReadOnly = this.isReadOnly;
			}
		}


		public override void CreateUI(Widget parent)
		{
			this.LabelWidth = 0;
			base.CreateUI (parent);

			this.controller = new AmortizedAmountController (this.accessor);
			this.controller.CreateUI (this.frameBox);
			this.controller.IsReadOnly = this.PropertyState == PropertyState.Readonly;
			this.controller.Value = this.value;

			this.UpdatePropertyState ();

			this.controller.ValueEdited += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					using (this.ignoreChanges.Enter ())
					{
						this.Value = this.controller.Value;
						this.OnValueEdited (this.Field);
					}
				}
			};

			this.controller.FocusEngage += delegate
			{
				base.SetFocus ();
			};

			this.controller.FocusLost += delegate
			{
				this.UpdateValue ();
			};

			this.controller.Goto += delegate (object sender, AbstractViewState viewState)
			{
				this.OnGoto (viewState);
			};
		}

		public override void SetFocus()
		{
			this.controller.SetFocus ();

			base.SetFocus ();
		}


		private AmortizedAmountController			controller;
		private AmortizedAmount?					value;
	}
}
