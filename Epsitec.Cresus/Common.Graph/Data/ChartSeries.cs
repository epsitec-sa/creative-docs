//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Graph.Data
{
	public class ChartSeries
	{
		public ChartSeries()
		{
			this.values = new List<ChartValue> ();
		}

		
		public string Label
		{
			get;
			set;
		}
		
		public IList<ChartValue> Values
		{
			get
			{
				return this.values;
			}
		}


		private readonly List<ChartValue> values;
	}
}
