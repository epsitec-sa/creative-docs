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
			if (string.IsNullOrEmpty (key))
			{
				throw new System.ArgumentException ("Invalid key", "key");
			}

			if (double.IsNaN (value))
			{
				return;
			}

			double total;

			this.values.TryGetValue (key, out total);
			this.values[key] = total + value;
		}

		public void Accumulate(string[] key, double value)
		{
			if (key.Length == 1)
			{
				this.Accumulate (key[0], value);
			}
			else if (key.Length > 1)
			{
				this.Accumulate (string.Join (DataCube.LabelSeparator.ToString (), key), value);
			}
			else
			{
				throw new System.ArgumentException ("Invalid key", "key");
			}
		}

		public void Accumulate(IEnumerable<ChartValue> collection)
		{
			foreach (var item in collection)
			{
				this.Accumulate (item.Label, item.Value);
			}
		}


		private readonly Dictionary<string, double> values;
	}
}
