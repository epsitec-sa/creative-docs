//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.NodesGetter
{
	/// <summary>
	/// Trie une énumération générique selon des instructions contenant un tri
	/// primaire et éventuellement un tri secondaire.
	/// </summary>
	public static class SortingMachine<T>
		where T : struct
	{
		public static IEnumerable<T> Sorts
		(
			//	Retourne une énumération de noeuds triée.
			//	T est une structure ParentNode ou OrderNode.
			SortingInstructions				instructions,
			IEnumerable<T>					nodes,
			System.Func<T, ComparableData>	getPrimaryData,
			System.Func<T, ComparableData>	getSecondaryData
		)
		{
			if (instructions.PrimaryField   != SimpleEngine.ObjectField.Unknown &&
				instructions.SecondaryField == SimpleEngine.ObjectField.Unknown)
			{
				//	Seulement un critère de tri principal.

				if (instructions.PrimaryType == SortedType.Ascending)
				{
					return nodes.OrderBy (x => getPrimaryData (x));
				}
				else if (instructions.PrimaryType == SortedType.Descending)
				{
					return nodes.OrderByDescending (x => getPrimaryData (x));
				}
			}
			else if (instructions.PrimaryField   != SimpleEngine.ObjectField.Unknown &&
					 instructions.SecondaryField != SimpleEngine.ObjectField.Unknown)
			{
				//	Un critère de tri principal et un secondaire.

				if (instructions.PrimaryType   == SortedType.Ascending &&
					instructions.SecondaryType == SortedType.Ascending)
				{
					return nodes.OrderBy (x => getSecondaryData (x))
								.OrderBy (x => getPrimaryData   (x));
				}
				else if (instructions.PrimaryType   == SortedType.Ascending &&
						 instructions.SecondaryType == SortedType.Descending)
				{
					return nodes.OrderByDescending (x => getSecondaryData (x))
								.OrderBy           (x => getPrimaryData   (x));
				}
				else if (instructions.PrimaryType   == SortedType.Descending &&
						 instructions.SecondaryType == SortedType.Ascending)
				{
					return nodes.OrderBy           (x => getSecondaryData (x))
								.OrderByDescending (x => getPrimaryData   (x));
				}
				else if (instructions.PrimaryType   == SortedType.Descending &&
						 instructions.SecondaryType == SortedType.Descending)
				{
					return nodes.OrderByDescending (x => getSecondaryData (x))
								.OrderByDescending (x => getPrimaryData   (x));
				}
			}

			//	Si les instructions sont incohérentes, on retourne l'énumération non triée.
			return nodes;
		}
	}
}
