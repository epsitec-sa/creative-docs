//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Graph.Data
{
	public class Accumulator
	{
		public Accumulator()
		{
			this.values = new Dictionary<string, double> ();
		}


		public IEnumerable<ChartValue> Values
		{
			get
			{
				return this.values.Select (x => new ChartValue (x.Key, x.Value)).OrderBy (x => x.Label);
			}
		}

		
		public void Accumulate(string key, double value)
		{
			double total;

			this.values.TryGetValue (key, out total);
			this.values[key] = total + value;
		}


		private readonly Dictionary<string, double> values;
	}
}
