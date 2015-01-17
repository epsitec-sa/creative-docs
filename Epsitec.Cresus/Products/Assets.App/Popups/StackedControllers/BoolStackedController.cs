//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups.StackedControllers
{
	public class BoolStackedController : AbstractStackedController
	{
		public BoolStackedController(DataAccessor accessor, StackedControllerDescription description)
			: base (accessor, description)
		{
		}


		public override bool					Enable
		{
			get
			{
				return this.button.Enable;
			}
			set
			{
				this.button.Enable = value;
			}
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

					this.UpdateButton ();
				}
			}
		}


		public override int						RequiredHeight
		{
			get
			{
				return BoolStackedController.checkHeight;
			}
		}

		public override int						RequiredControllerWidth
		{
			get
			{
				return 30 + this.description.Label.GetTextWidth ();
			}
		}

		public override int						RequiredLabelsWidth
		{
			get
			{
				return 0;
			}
		}


		public override void CreateUI(Widget parent, int labelWidth, ref int tabIndex)
		{
			this.button = new CheckButton
			{
				Parent          = parent,
				Text            = this.description.Label,
				AutoToggle      = false,
				ActiveState     = this.Value ? ActiveState.Yes : ActiveState.No,
				TabIndex        = ++tabIndex,
				PreferredHeight = BoolStackedController.checkHeight,
				Dock            = DockStyle.Top,
				Margins         = new Margins (labelWidth+10, 0, 0, 0),
			};

			this.button.Clicked += delegate
			{
				this.Value = !this.Value;
				this.OnValueChanged ();
			};
		}

		private void UpdateButton()
		{
			if (this.button != null)
			{
				this.button.ActiveState = this.value ? ActiveState.Yes : ActiveState.No;
			}
		}


		private const int checkHeight = 17;

		private bool							value;
		private CheckButton						button;
	}
}