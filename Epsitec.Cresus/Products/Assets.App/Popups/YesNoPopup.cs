//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class YesNoPopup : AbstractPopup
	{
		public YesNoPopup()
		{
			this.radios = new List<Radio> ();
		}


		public string							Question;

		public List<Radio>						Radios
		{
			get
			{
				return this.radios;
			}
		}

		public string							RadioSelected
		{
			get
			{
				return this.radioSelected;
			}
		}

		protected override Size					DialogSize
		{
			get
			{
				int dx = YesNoPopup.dialogWidth;
				int dy = YesNoPopup.dialogHeight + YesNoPopup.radioHeight * this.radios.Count;

				return new Size (dx, dy);
			}
		}

		public override void CreateUI()
		{
			this.CreateTitle (YesNoPopup.titleHeight, this.Question);

			{
				int y = (int) this.DialogSize.Height - YesNoPopup.titleHeight - YesNoPopup.radioHeight - 12;

				foreach (var radio in this.radios)
				{
					var button = this.CreateRadio
					(
						YesNoPopup.margins,
						y,
						YesNoPopup.dialogWidth - YesNoPopup.margins*2,
						YesNoPopup.radioHeight,
						radio.Name,
						radio.Text,
						radio.Tooltip,
						radio.Activate
					);

					if (radio.Activate)
					{
						this.radioSelected = radio.Name;
					}

					button.Clicked += delegate
					{
						this.radioSelected = button.Name;
					};

					y -= YesNoPopup.radioHeight;
				}
			}

			{
				int y = (int) YesNoPopup.margins;
				int dx = (YesNoPopup.dialogWidth - YesNoPopup.margins*2 - YesNoPopup.buttonGap) /2;
				int dy = 24;

				this.CreateButton (YesNoPopup.margins,                         y, dx, dy, "yes", "Oui");
				this.CreateButton (YesNoPopup.margins+dx+YesNoPopup.buttonGap, y, dx, dy, "no",  "Non");
			}
		}


		public struct Radio
		{
			public Radio(string name, string text, string tooltip = null, bool activate = false)
			{
				this.Name     = name;
				this.Text     = text;
				this.Tooltip  = tooltip;
				this.Activate = activate;
			}

			public readonly string		Name;
			public readonly string		Text;
			public readonly string		Tooltip;
			public readonly bool		Activate;
		}


		private static readonly int margins      = 20;
		private static readonly int titleHeight  = 30;
		private static readonly int dialogWidth  = 260;
		private static readonly int dialogHeight = 100;
		private static readonly int radioHeight  = 20;
		private static readonly int buttonGap    = 10;

		private readonly List<Radio> radios;
		private string radioSelected;
	}
}