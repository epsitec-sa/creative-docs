//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Widgets.Layouts
{
	/// <summary>
	/// The <c>ColumnLayoutList</c> class is a specialized list of <see cref="ColumnLayoutInfo"/>
	/// records. It is responsible for the global layout of the individual columns, based on the
	/// available size.
	/// </summary>
	public sealed class ColumnLayoutList : List<ColumnLayoutInfo>
	{
		public ColumnLayoutList()
		{
		}


		public void Add(ColumnDefinition definition)
		{
			this.Add (new ColumnLayoutInfo (definition));
		}

		public void AddRange(IEnumerable<ColumnDefinition> collection)
		{
			this.AddRange (collection.Select (x => new ColumnLayoutInfo (x)));
		}

		public double Fit(double availableSpace)
		{
			this.ResetMeasures ();
			this.ConstrainColumns ();
			return this.GenerateOffsets (availableSpace);
		}


		public static double Fit(IEnumerable<ColumnLayoutInfo> collection, double availableSpace)
		{
			var list = new ColumnLayoutList ();
			list.AddRange (collection);
			return list.Fit (availableSpace);
		}


		private void ResetMeasures()
		{
			this.passId = 1;

			foreach (var item in this)
			{
				item.Measure.Reset (0);
			}
		}

		private void ConstrainColumns()
		{
			for (int index = 0; index < this.Count; index++)
			{
				var def     = this[index].Definition;
				var measure = this[index].Measure;
				var length  = def.Width;

				measure.UpdateMin (this.passId, def.MinWidth);
				measure.UpdateMax (this.passId, def.MaxWidth);
				measure.UpdateDesired (length.IsAbsolute ? length.Value : 0);
				measure.UpdatePassId (this.passId);
			}
		}

		private double GenerateOffsets(double availableSpace)
		{
			double total = 0;
			double flex  = 0;

			foreach (var item in this)
			{
				var def = item.Definition;
				var length = def.Width;
				var width = length.IsAbsolute ? length.Value : 0;
				var pos = total + def.LeftBorder;

				item.Measure.UpdateDesired (width);

				width = item.Measure.Desired;
				total = pos + width + def.RightBorder;
				
				def.DefineActualOffset (pos);
				def.DefineActualWidth (width);

				if (def.Width.IsProportional)
				{
					flex += def.Width.Value;
				}
			}

			double space = availableSpace - total;

			if ((space > 0) &&
				(flex > 0))
			{
				double move = 0;

				foreach (var item in this)
				{
					var def = item.Definition;
					var pos = def.ActualOffset + move;

					def.DefineActualOffset (pos);

					if (def.Width.IsProportional)
					{
						double d = def.Width.Value * space / flex;
						double w = def.ActualWidth + d;

						item.Measure.UpdateDesired (w);
						def.DefineActualWidth (w);

						move += d;
					}
				}

				total += move;
			}

			return total;
		}

		
		private int								passId;
	}
}
