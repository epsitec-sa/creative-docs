//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Settings.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Settings.Controllers
{
	public class SampleSettingController : AbstractSettingController
	{
		public SampleSettingController(AbstractSettingData data, System.Action actionChanged)
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

			switch (this.data.Type)
			{
				case SettingsType.PriceSample:
					this.CreatePriceUI (frame);
					break;

				case SettingsType.PercentSample:
					this.CreatePercentUI (frame);
					break;

				case SettingsType.DateSample:
					this.CreateDateUI (frame);
					break;
			}
		}

		public override void Update()
		{
			switch (this.data.Type)
			{
				case SettingsType.PriceSample:
					this.UpdatePrice ();
					break;

				case SettingsType.PercentSample:
					this.UpdatePercent ();
					break;

				case SettingsType.DateSample:
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
			if (this.Data.SettingsList.HasError (SettingsType.PriceDecimalSeparator, SettingsType.PriceGroupSeparator, SettingsType.PriceNegativeFormat, SettingsType.PriceNullParts))
			{
				this.frame.BackColor = UIBuilder.ErrorColor;

				this.sample1.Text = null;
				this.sample2.Text = null;
				this.sample3.Text = null;
			}
			else
			{
				this.frame.BackColor = Color.Empty;

				this.sample1.Text = Converters.MontantToString (12500.0m,   null);
				this.sample2.Text = Converters.MontantToString (-12500.75m, null);
				this.sample3.Text = Converters.MontantToString (0.61m,      null);
			}
		}
		#endregion


		#region Percent
		private void CreatePercentUI(Widget parent)
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

		private void UpdatePercent()
		{
			this.frame.BackColor = Color.Empty;

			this.sample1.Text = Converters.PercentToString (0.06789m);
			this.sample2.Text = Converters.PercentToString (0.024m);
			this.sample3.Text = Converters.PercentToString (0.08m);
		}
		#endregion


		#region Date
		private void CreateDateUI(Widget parent)
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
				ContentAlignment = ContentAlignment.MiddleLeft,
				Dock             = DockStyle.Top,
			};

			this.sample2 = new StaticText
			{
				Parent           = this.frame,
				ContentAlignment = ContentAlignment.MiddleLeft,
				Dock             = DockStyle.Top,
			};

			this.sample3 = new StaticText
			{
				Parent           = this.frame,
				ContentAlignment = ContentAlignment.MiddleLeft,
				Dock             = DockStyle.Top,
			};
		}

		private void UpdateDate()
		{
			if (this.Data.SettingsList.HasError (SettingsType.DateSeparator, SettingsType.DateOrder, SettingsType.DateYear))
			{
				this.frame.BackColor = UIBuilder.ErrorColor;

				this.sample1.Text = null;
				this.sample2.Text = null;
				this.sample3.Text = null;
			}
			else
			{
				this.frame.BackColor = Color.Empty;

				int year = Date.Today.Year;
				this.sample1.Text = Converters.DateToString (new Date (year, 1, 1));
				this.sample2.Text = Converters.DateToString (new Date (year, 3, 31));
				this.sample3.Text = Converters.DateToString (new Date (year, 12, 25));
			}
		}
		#endregion


		private SampleSettingData Data
		{
			get
			{
				return this.data as SampleSettingData;
			}
		}


		private FrameBox		frame;
		private StaticText		sample1;
		private StaticText		sample2;
		private StaticText		sample3;
	}
}
