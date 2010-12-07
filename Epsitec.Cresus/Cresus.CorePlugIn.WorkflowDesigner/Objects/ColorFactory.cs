//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.CorePlugIn.WorkflowDesigner.Objects
{
	public class ColorFactory
	{
		public ColorFactory(ColorItem colorItem)
		{
			this.colorItem = colorItem;
		}


		public ColorItem ColorItem
		{
			get
			{
				return this.colorItem;
			}
			set
			{
				this.colorItem = value;
			}
		}

		public double DimmedIntensity
		{
			get
			{
				return this.dimmedIntensity;
			}
			set
			{
				this.dimmedIntensity = value;
			}
		}


		public Color GetColorMain()
		{
			//	Retourne la couleur pour les mises en évidence.
			return this.GetColorMain (1.0);
		}

		public Color GetColorMain(double alpha)
		{
			//	Retourne la couleur pour les mises en évidence.
			return this.GetColorMain (this.colorItem, alpha);
		}

		public Color GetColorMain(ColorItem colorItem, double alpha)
		{
			//	Retourne la couleur pour les mises en évidence.
			Color color = Color.FromAlphaRgb (alpha, 128.0/255.0, 128.0/255.0, 128.0/255.0);

			switch (colorItem)
			{
				case ColorItem.Yellow:
					color = Color.FromAlphaRgb (alpha, 200.0/255.0, 200.0/255.0, 0.0/255.0);
					break;

				case ColorItem.Orange:
					color = Color.FromAlphaRgb (alpha, 200.0/255.0, 150.0/255.0, 0.0/255.0);
					break;

				case ColorItem.Red:
					color = Color.FromAlphaRgb (alpha, 140.0/255.0, 30.0/255.0, 0.0/255.0);
					break;

				case ColorItem.Lilac:
					color = Color.FromAlphaRgb (alpha, 100.0/255.0, 0.0/255.0, 150.0/255.0);
					break;

				case ColorItem.Purple:
					color = Color.FromAlphaRgb (alpha, 30.0/255.0, 0.0/255.0, 200.0/255.0);
					break;

				case ColorItem.Blue:
					color = Color.FromAlphaRgb (alpha, 0.0/255.0, 90.0/255.0, 160.0/255.0);
					break;

				case ColorItem.Green:
					color = Color.FromAlphaRgb (alpha, 0.0/255.0, 130.0/255.0, 20.0/255.0);
					break;

				case ColorItem.Grey:
					color = Color.FromAlphaRgb (alpha, 100.0/255.0, 100.0/255.0, 100.0/255.0);
					break;
			}

			if (this.dimmedIntensity != 0)
			{
				color = this.GetColorLighter (color, 0.2+0.8*(1-this.dimmedIntensity));
			}

			return color;
		}

		public Color GetColor(double brightness, double alpha = 1)
		{
			//	Retourne un niveau de gris.
			Color color = Color.FromBrightness (brightness);
			color = Color.FromAlphaRgb (alpha, color.R, color.G, color.B);

			if (this.dimmedIntensity != 0)
			{
				color = this.GetColorLighter (color, 0.2+0.8*(1-this.dimmedIntensity));
			}

			return color;
		}

		public Color GetColorAdjusted(Color color, double factor)
		{
			//	Retourne une couleur ajustée, sans changer la transparence.
			if (this.IsDarkColorMain)
			{
				return this.GetColorDarker (color, factor);
			}
			else
			{
				return this.GetColorLighter (color, factor);
			}
		}

		private bool IsDarkColorMain
		{
			//	Indique si la couleur pour les mises en évidence est foncée.
			get
			{
				return false;
			}
		}

		private Color GetColorLighter(Color color, double factor)
		{
			//	Retourne une couleur éclaircie, sans changer la transparence.
			return Color.FromAlphaRgb (color.A, 1-(1-color.R)*factor, 1-(1-color.G)*factor, 1-(1-color.B)*factor);
		}

		private Color GetColorDarker(Color color, double factor)
		{
			//	Retourne une couleur assombrie, sans changer la transparence.
			factor = 0.5+(factor*0.5);
			return Color.FromAlphaRgb (color.A, color.R*factor, color.G*factor, color.B*factor);
		}


		private ColorItem		colorItem;
		private double			dimmedIntensity;
	}
}
