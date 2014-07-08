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
	public class RadioStackedController : AbstractStackedController
	{
		public RadioStackedController(DataAccessor accessor, StackedControllerDescription description)
			: base (accessor, description)
		{
			this.radios = new List<RadioButton> ();
		}


		public int?								Value
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

					this.UpdateRadios ();
				}
			}
		}


		public override int						RequiredHeight
		{
			get
			{
				return this.description.Labels.Count * RadioStackedController.radioHeight;
			}
		}

		public override int						RequiredControllerWidth
		{
			get
			{
				return 30 + this.description.Labels.Select (x => x.GetTextWidth ()).Max ();
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
			this.radios.Clear ();

			if (labelWidth > 0)
			{
				labelWidth += 10;
			}

			int rank = 0;
			foreach (var label in this.description.Labels)
			{
				var radio = new RadioButton
				{
					Parent          = parent,
					Text            = label,
					Name            = rank.ToString (System.Globalization.CultureInfo.InvariantCulture),
					AutoToggle      = false,
					TabIndex        = ++tabIndex,
					Group           = tabIndex.ToString (System.Globalization.CultureInfo.InvariantCulture),
					PreferredHeight = RadioStackedController.radioHeight,
					Dock            = DockStyle.Top,
					Margins         = new Margins (labelWidth, 0, 0, 0),
				};

				radio.Clicked += delegate
				{
					this.Value = int.Parse (radio.Name);
					this.UpdateRadios ();
					this.OnValueChanged ();
				};

				this.radios.Add (radio);
				rank++;
			}

			this.UpdateRadios ();
		}

		public void SetRadioEnable(int index, bool enable)
		{
			System.Diagnostics.Debug.Assert (index >= 0 && index < this.radios.Count);
			this.radios[index].Enable = enable;
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


		private const int radioHeight = 17;

		private int?							value;
		private readonly List<RadioButton>		radios;
	}
}