//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.WorkflowDesigner.Objects
{
	public class ColorEngine
	{
		public ColorEngine(MainColor color)
		{
			this.color = color;
		}


		public MainColor MainColor
		{
			get
			{
				return this.color;
			}
			set
			{
				this.color = value;
			}
		}


		public bool IsDarkColorMain
		{
			//	Indique si la couleur pour les mises en évidence est foncée.
			get
			{
				return this.color == MainColor.DarkGrey;
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
			return this.GetColorMain (this.color, alpha);
		}

		public Color GetColorMain(MainColor boxColor)
		{
			//	Retourne la couleur pour les mises en évidence.
			return this.GetColorMain (boxColor, 1.0);
		}

		public Color GetColorMain(MainColor boxColor, double alpha)
		{
			//	Retourne la couleur pour les mises en évidence.
			Color color = Color.FromAlphaRgb (alpha, 128.0/255.0, 128.0/255.0, 128.0/255.0);

			switch (boxColor)
			{
				case MainColor.Blue:
					color = Color.FromAlphaRgb (alpha, 0.0/255.0, 90.0/255.0, 160.0/255.0);
					break;

				case MainColor.Green:
					color = Color.FromAlphaRgb (alpha, 0.0/255.0, 130.0/255.0, 20.0/255.0);
					break;

				case MainColor.Red:
					color = Color.FromAlphaRgb (alpha, 140.0/255.0, 30.0/255.0, 0.0/255.0);
					break;

				case MainColor.Grey:
					color = Color.FromAlphaRgb (alpha, 100.0/255.0, 100.0/255.0, 100.0/255.0);
					break;

				case MainColor.DarkGrey:
					color = Color.FromAlphaRgb (alpha, 100.0/255.0, 100.0/255.0, 100.0/255.0);
					break;

				case MainColor.Yellow:
					color = Color.FromAlphaRgb (alpha, 200.0/255.0, 200.0/255.0, 0.0/255.0);
					break;

				case MainColor.Orange:
					color = Color.FromAlphaRgb (alpha, 200.0/255.0, 150.0/255.0, 0.0/255.0);
					break;

				case MainColor.Lilac:
					color = Color.FromAlphaRgb (alpha, 100.0/255.0, 0.0/255.0, 150.0/255.0);
					break;

				case MainColor.Purple:
					color = Color.FromAlphaRgb (alpha, 30.0/255.0, 0.0/255.0, 200.0/255.0);
					break;
			}

			return color;
		}

		public Color GetColor(double brightness)
		{
			//	Retourne un niveau de gris.
			return Color.FromBrightness (brightness);
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

		public Color GetColorLighter(Color color, double factor)
		{
			//	Retourne une couleur éclaircie, sans changer la transparence.
			return Color.FromAlphaRgb (color.A, 1-(1-color.R)*factor, 1-(1-color.G)*factor, 1-(1-color.B)*factor);
		}

		public Color GetColorDarker(Color color, double factor)
		{
			//	Retourne une couleur assombrie, sans changer la transparence.
			factor = 0.5+(factor*0.5);
			return Color.FromAlphaRgb (color.A, color.R*factor, color.G*factor, color.B*factor);
		}


		private MainColor color;
	}
}
