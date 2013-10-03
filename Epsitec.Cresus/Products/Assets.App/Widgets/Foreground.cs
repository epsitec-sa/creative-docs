//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Ce widget permet de dessiner une zone rectangulaire servant de feedback
	/// visuel pour un hilite, par exemple lors du déplacement d'une colonne.
	/// </summary>
	public class Foreground : Widget
	{
		public Rectangle HilitedZone
		{
			get
			{
				return this.hilitedZone;
			}
			set
			{
				if (this.hilitedZone != value)
				{
					this.hilitedZone = value;
					this.Invalidate ();
				}
			}
		}

		public Color HilitedColor
		{
			get
			{
				return this.hilitedColor;
			}
			set
			{
				if (this.hilitedColor != value)
				{
					this.hilitedColor = value;
					this.Invalidate ();
				}
			}
		}


		protected override void OnPaintBackground(PaintEventArgs e)
		{
			if (this.hilitedZone.IsValid && this.hilitedColor.IsValid)
			{
				e.Graphics.AddFilledRectangle (this.hilitedZone);
				e.Graphics.RenderSolid (this.hilitedColor);
			}
		}


		private Rectangle hilitedZone;
		private Color hilitedColor;
	}
}