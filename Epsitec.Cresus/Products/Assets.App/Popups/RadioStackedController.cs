//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class RadioStackedController : AbstractStackedController
	{
		public RadioStackedController(DataAccessor accessor)
			: base (accessor)
		{
			this.radios = new List<RadioButton> ();
		}


		public int?								Value;


		public override void CreateUI(Widget parent, int labelWidth, int tabIndex, StackedControllerDescription description)
		{
			this.radios.Clear ();

			int rank = 0;
			foreach (var label in description.Labels)
			{
				var radio = new RadioButton
				{
					Parent          = parent,
					Text            = label,
					Name            = rank.ToString (System.Globalization.CultureInfo.InvariantCulture),
					AutoToggle      = false,
					PreferredHeight = RadioStackedController.radioHeight,
					Dock            = DockStyle.Top,
					Margins         = new Margins (labelWidth, 0, 0, 0),
				};

				radio.Clicked += delegate
				{
					this.Value = int.Parse (radio.Name);
					this.UpdateRadios ();
					this.UpdateWidgets ();
					this.OnValueChanged (description);
				};

				this.radios.Add (radio);
				rank++;
			}

			if (description.BottomMargin > 0)
			{
				new FrameBox
				{
					Parent  = parent,
					Dock    = DockStyle.Top,
					Margins = new Margins (0, 0, 0, description.BottomMargin),
				};
			}

			this.UpdateRadios ();
		}

		private void UpdateRadios()
		{
			int rank = 0;
			foreach (var radio in this.radios)
			{
				radio.ActiveState = (rank == this.Value) ? ActiveState.Yes : ActiveState.No;
				rank++;
			}
		}


		public const int radioHeight = 21;

		private readonly List<RadioButton>		radios;
	}
}