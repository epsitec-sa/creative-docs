//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.NaiveEngine
{
	public static class ObjectCalculator
	{
		public static void UpdateComputedAmounts(DataObject obj)
		{
			if (obj != null)
			{
				for (int i=0; i<3; i++)  // Valeur1..3
				{
					decimal? last = null;

					foreach (var e in obj.Events)
					{
						var current = ObjectCalculator.GetComputedAmount (e, i);

						if (current.HasValue)
						{
							if (last.HasValue == false)
							{
								last = current.Value.FinalAmount;
								current = new ComputedAmount (last);
								ObjectCalculator.SetComputedAmount (e, i, current);
							}
							else
							{
								current = new ComputedAmount (last.Value, current.Value);
								last = current.Value.FinalAmount;
								ObjectCalculator.SetComputedAmount (e, i, current);
							}
						}
					}
				}
			}
		}

		private static ComputedAmount? GetComputedAmount(DataEvent e, int rank)
		{
			int id = ObjectCalculator.RankToField (rank);
			if (id != -1)
			{
				var p = e.GetProperty (id) as DataComputedAmountProperty;
				if (p != null)
				{
					return p.Value;
				}
			}

			return null;
		}

		private static void SetComputedAmount(DataEvent e, int rank, ComputedAmount? value)
		{
			int id = ObjectCalculator.RankToField (rank);
			if (id != -1)
			{
				if (value.HasValue)
				{
					var newProperty = new DataComputedAmountProperty (id, value.Value);
					e.AddProperty (newProperty);
				}
				else
				{
					e.RemoveProperty (id);
				}
			}
		}

		private static int RankToField(int rank)
		{
			switch (rank)
			{
				case 0:
					return (int) ObjectField.Valeur1;

				case 1:
					return (int) ObjectField.Valeur2;

				case 2:
					return (int) ObjectField.Valeur3;

				default:
					return -1;
			}
		}
	}
}
