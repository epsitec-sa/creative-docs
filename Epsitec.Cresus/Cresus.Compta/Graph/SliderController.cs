//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Widgets;
using Epsitec.Cresus.Compta;

using System.Collections.Generic;

namespace Epsitec.Cresus.Compta.Graph
{
	public class SliderController
	{
		public SliderController()
		{
			this.ignoreChanges = new SafeCounter ();
		}


		public FrameBox CreateUI(Widget parent, double min, double max, double resolution, double def, FormattedText label, System.Action valueChangedAction)
		{
			this.def = def;

			this.frame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 20-4,
				PreferredWidth  = 200,
				Dock            = DockStyle.Top,
				Margins         = new Margins (0, 0, 2, 2),
			};

			new StaticText
			{
				Parent           = this.frame,
				ContentAlignment = ContentAlignment.MiddleRight,
				TextBreakMode    = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				FormattedText    = label,
				PreferredWidth   = 80,
				Dock             = DockStyle.Left,
				Margins          = new Margins (0, 10, 0, 0),
			};

			this.slider = new HSlider
			{
				Parent          = this.frame,
				MinValue        = (decimal) min,
				MaxValue        = (decimal) max,
				Resolution      = (decimal) resolution,
				SmallChange     = (decimal) resolution,
				LargeChange     = (decimal) resolution*2,
				PreferredHeight = 20-4,
				Dock            = DockStyle.Fill,
			};

			this.defButton = new GlyphButton
			{
				Parent          = this.frame,
				GlyphShape      = GlyphShape.Close,
				ButtonStyle     = ButtonStyle.ToolItem,
				PreferredHeight = 20-4,
				PreferredWidth  = 20-4,
				Dock            = DockStyle.Right,
				Margins         = new Margins (1, 0, 0, 0),
			};

			ToolTip.Default.SetToolTip (this.defButton, "Remet la valeur standard");

			this.slider.ValueChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					this.UpdateWidgets ();
					valueChangedAction ();
				}
			};

			this.defButton.Clicked += delegate
			{
				this.slider.Value = (decimal) def;
			};

			this.UpdateWidgets ();

			return this.frame;
		}


		public bool Enable
		{
			get
			{
				return this.frame.Enable;
			}
			set
			{
				this.frame.Enable = value;
			}
		}

		public bool Visibility
		{
			get
			{
				return this.frame.Visibility;
			}
			set
			{
				this.frame.Visibility = value;
			}
		}

		public double Value
		{
			get
			{
				return (double) this.slider.Value;
			}
			set
			{
				using (this.ignoreChanges.Enter ())
				{
					this.slider.Value = (decimal) value;
					this.UpdateWidgets ();
				}
			}
		}


		private void UpdateWidgets()
		{
			this.defButton.Enable = this.slider.Value != (decimal) this.def;
		}


		private readonly SafeCounter	ignoreChanges;

		private double					def;
		private FrameBox				frame;
		private HSlider					slider;
		private GlyphButton				defButton;
	}
}
