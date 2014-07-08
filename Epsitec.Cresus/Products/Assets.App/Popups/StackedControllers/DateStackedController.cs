//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups.StackedControllers
{
	public class DateStackedController : AbstractStackedController
	{
		public DateStackedController(DataAccessor accessor, StackedControllerDescription description)
			: base (accessor, description)
		{
		}


		public System.DateTime?					Value
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
						this.controller.Date = this.Value;
					}
				}
			}
		}

		public override bool					HasError
		{
			get
			{
				return this.controller.HasError;
			}
		}


		public override int						RequiredHeight
		{
			get
			{
				return DateStackedController.height;
			}
		}

		public override int						RequiredControllerWidth
		{
			get
			{
				return DateStackedController.width;
			}
		}


		public override void CreateUI(Widget parent, int labelWidth, ref int tabIndex)
		{
			this.controller = new DateController (this.accessor)
			{
				DateRangeCategory = this.description.DateRangeCategory,
				Date              = this.Value,
				DateLabelWidth    = labelWidth,
				DateDescription   = this.description.Label,
				TabIndex          = ++tabIndex,
			};

			this.controller.CreateUI (parent);

			this.controller.DateChanged += delegate
			{
				this.Value = this.controller.Date;
				this.OnValueChanged ();
			};
		}

		public override void SetFocus()
		{
			this.controller.SetFocus ();
		}


		private const int width  = DateController.controllerWidth;
		private const int height = DateController.controllerHeight;

		private System.DateTime?				value;
		private DateController					controller;
	}
}