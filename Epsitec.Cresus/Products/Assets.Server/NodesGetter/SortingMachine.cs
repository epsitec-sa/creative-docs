//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.NodesGetter
{
	public static class SortingMachine<T>
	{
		public static IEnumerable<T> Sort
		(
			SortingInstructions				instructions,
			IEnumerable<T>					nodes,
			System.Func<T, ComparableData>	getPrimaryData,
			System.Func<T, ComparableData>	getSecondaryData
		)
		{
			if (instructions.PrimaryField   != SimpleEngine.ObjectField.Unknown &&
				instructions.SecondaryField == SimpleEngine.ObjectField.Unknown)
			{
				if (instructions.PrimaryType == SortedType.Ascending)
				{
					return nodes.OrderBy (x => getPrimaryData (x));
				}
				else
				{
					return nodes.OrderByDescending (x => getPrimaryData (x));
				}
			}
			else if (instructions.PrimaryField   != SimpleEngine.ObjectField.Unknown &&
					 instructions.SecondaryField != SimpleEngine.ObjectField.Unknown)
			{
				if (instructions.PrimaryType   == SortedType.Ascending &&
					instructions.SecondaryType == SortedType.Descending)
				{
					return nodes
						.OrderByDescending (x => getSecondaryData (x))
						.OrderBy           (x => getPrimaryData (x));
				}
				else if (instructions.PrimaryType   == SortedType.Descending &&
						 instructions.SecondaryType == SortedType.Ascending)
				{
					return nodes
						.OrderBy           (x => getSecondaryData (x))
						.OrderByDescending (x => getPrimaryData (x));
				}
				else if (instructions.PrimaryType   == SortedType.Descending &&
						 instructions.SecondaryType == SortedType.Descending)
				{
					return nodes
						.OrderByDescending (x => getSecondaryData (x))
						.OrderByDescending (x => getPrimaryData (x));
				}
				else
				{
					return nodes
						.OrderBy (x => getSecondaryData (x))
						.OrderBy (x => getPrimaryData (x));
				}
			}
			else
			{
				return nodes;
			}
		}
	}
}
