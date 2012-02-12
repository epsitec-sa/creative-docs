//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Settings.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Settings.Controllers
{
	public class SampleSettingsController : AbstractSettingsController
	{
		public SampleSettingsController(AbstractSettingsData data, System.Action actionChanged)
			: base (data, actionChanged)
		{
		}

		public override void CreateUI(Widget parent)
		{
			var frame = new FrameBox
			{
				Parent   = parent,
				Dock     = DockStyle.Top,
				Margins  = new Margins (0, 0, 10, 0),
			};

			this.CreateLabel (frame);

			switch (this.data.Name)
			{
				case "Price.Sample":
					this.CreatePriceUI (frame);
					break;

				case "Date.Sample":
					this.CreateDateUI (frame);
					break;
			}
		}

		public override void Update()
		{
			switch (this.data.Name)
			{
				case "Price.Sample":
					this.UpdatePrice ();
					break;

				case "Date.Sample":
					this.UpdateDate ();
					break;
			}
		}


		#region Price
		private void CreatePriceUI(Widget parent)
		{
			this.frame = new FrameBox
			{
				Parent         = parent,
				PreferredWidth = 100,
				DrawFullFrame  = true,
				Dock           = DockStyle.Left,
				Padding        = new Margins (10),
			};

			this.sample1 = new StaticText
			{
				Parent           = this.frame,
				ContentAlignment = ContentAlignment.MiddleRight,
				Dock             = DockStyle.Top,
			};

			this.sample2 = new StaticText
			{
				Parent           = this.frame,
				ContentAlignment = ContentAlignment.MiddleRight,
				Dock             = DockStyle.Top,
			};

			this.sample3 = new StaticText
			{
				Parent           = this.frame,
				ContentAlignment = ContentAlignment.MiddleRight,
				Dock             = DockStyle.Top,
			};
		}

		private void UpdatePrice()
		{
			if (this.Data.SettingsList.HasError)
			{
				this.frame.BackColor = Color.FromHexa ("ffb1b1");  // rouge pâle

				this.sample1.Text = null;
				this.sample2.Text = null;
				this.sample3.Text = null;
			}
			else
			{
				this.frame.BackColor = Color.Empty;

				this.sample1.Text = Converters.MontantToString (-12500.75m);
				this.sample2.Text = Converters.MontantToString (12500.0m);
				this.sample3.Text = Converters.MontantToString (0.61m);
			}
		}
		#endregion


		#region Date
		private void CreateDateUI(Widget parent)
		{
		}

		private void UpdateDate()
		{
		}
		#endregion


		private SampleSettingsData Data
		{
			get
			{
				return this.data as SampleSettingsData;
			}
		}


		private FrameBox		frame;
		private StaticText		sample1;
		private StaticText		sample2;
		private StaticText		sample3;
	}
}
