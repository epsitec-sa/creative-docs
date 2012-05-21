//	Copyright © 2006-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Layouts
{
	public sealed class RowMeasure : LayoutMeasure
	{
		public RowMeasure(int passId)
			: base (passId)
		{
		}

		public double MinH1
		{
			get
			{
				return this.minH1;
			}
		}

		public double MinH2
		{
			get
			{
				return this.minH2;
			}
		}

		public void UpdateMinH1H2(int passId, double h1, double h2)
		{
			double oldH1 = this.minH1;
			double oldH2 = this.minH2;

			if (this.PassId == passId)
			{
				this.minH1 = System.Math.Max (oldH1, h1);
				this.minH2 = System.Math.Max (oldH2, h2);
			}
			else
			{
				this.minH1 = h1;
				this.minH2 = h2;
			}

			if ((this.minH1 != oldH1) ||
						(this.minH2 != oldH2))
			{
				this.UpdateMin (passId, this.minH1 + this.minH2, forceChange: true);
			}
		}

		private double minH1;
		private double minH2;
	}
}
