//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public abstract class AbstractStackedController
	{
		public AbstractStackedController(DataAccessor accessor)
		{
			this.accessor = accessor;
		}


		public virtual void CreateUI(Widget parent, int labelWidth, int tabIndex, StackedControllerDescription description)
		{
		}

		public virtual void SetFocus()
		{
		}


		protected void CreateLabel(Widget parent, int labelWidth, StackedControllerDescription description)
		{
			new StaticText
			{
				Parent           = parent,
				Text             = description.Label,
				ContentAlignment = ContentAlignment.MiddleRight,
				TextBreakMode    = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				Dock             = DockStyle.Left,
				PreferredWidth   = labelWidth,
				PreferredHeight  = AbstractFieldController.lineHeight - 1,
				Margins          = new Margins (0, 10, 0, 1),
			};
		}

		protected FrameBox CreateControllerFrame(Widget parent)
		{
			return new FrameBox
			{
				Parent    = parent,
				Dock      = DockStyle.Fill,
				BackColor = ColorManager.WindowBackgroundColor,
			};
		}


		#region Events handler
		protected void OnValueChanged(StackedControllerDescription description)
		{
			this.ValueChanged.Raise (this, description);
		}

		public event EventHandler<StackedControllerDescription> ValueChanged;
		#endregion


		protected DataAccessor					accessor;
	}
}