//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.FieldControllers
{
	public class BoolFieldController : AbstractFieldController
	{
		public BoolFieldController(DataAccessor accessor)
			: base (accessor)
		{
		}


		public bool								Value
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

					if (this.button != null)
					{
						this.button.ActiveState = this.value ? ActiveState.Yes : ActiveState.No;
					}
				}
			}
		}

		protected override void ClearValue()
		{
			this.Value = false;
			this.OnValueEdited (this.Field);
		}

		protected override void UpdatePropertyState()
		{
			base.UpdatePropertyState ();

			if (this.button != null)
			{
				this.button.Enable = !this.isReadOnly;
			}
		}


		public override void CreateUI(Widget parent)
		{
			var l = this.Label;
			this.Label = null;
			base.CreateUI (parent);
			this.Label = l;

			this.button = new CheckButton
			{
				Parent          = this.frameBox,
				Text            = this.Label,
				PreferredWidth  = AbstractFieldController.maxWidth,
				PreferredHeight = AbstractFieldController.lineHeight,
				Dock            = DockStyle.Left,
				TabIndex        = ++this.TabIndex,
				AutoToggle      = false,
				ActiveState     = this.value ? ActiveState.Yes : ActiveState.No,
			};

			this.UpdatePropertyState ();

			this.button.Clicked += delegate
			{
				this.Value = !this.Value;
				this.OnValueEdited (this.Field);
			};

			this.button.IsFocusedChanged += delegate (object sender, DependencyPropertyChangedEventArgs e)
			{
				bool focused = (bool) e.NewValue;

				if (focused)  // pris le focus ?
				{
					this.SetFocus ();
				}
				else  // perdu le focus ?
				{
				}
			};
		}

		public override void SetFocus()
		{
			this.button.Focus ();

			base.SetFocus ();
		}


		private CheckButton						button;
		private bool							value;
	}
}
