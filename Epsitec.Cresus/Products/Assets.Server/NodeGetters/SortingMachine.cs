//	Copyright � 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	/// <summary>
	/// Trie une �num�ration g�n�rique selon des instructions contenant un tri
	/// primaire et �ventuellement un tri secondaire.
	/// </summary>
	public static class SortingMachine<T>
		where T : struct
	{
		public static IEnumerable<T> Sorts
		(
			//	Retourne une �num�ration de noeuds tri�e.
			//	T est une structure ParentNode ou OrderNode.
			SortingInstructions				instructions,
			IEnumerable<T>					nodes,
			System.Func<T, ComparableData>	getPrimaryData,
			System.Func<T, ComparableData>	getSecondaryData
		)
		{
			if (instructions.PrimaryField   != ObjectField.Unknown &&
				instructions.SecondaryField == ObjectField.Unknown)
			{
				//	Seulement un crit�re de tri principal.

				if (instructions.PrimaryType == SortedType.Ascending)
				{
					return nodes.OrderBy (x => getPrimaryData (x));
				}
				else if (instructions.PrimaryType == SortedType.Descending)
				{
					return nodes.OrderByDescending (x => getPrimaryData (x));
				}
			}
			else if (instructions.PrimaryField   != ObjectField.Unknown &&
					 instructions.SecondaryField != ObjectField.Unknown)
			{
				//	Un crit�re de tri principal et un secondaire.

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

			//	Si les instructions sont incoh�rentes, on retourne l'�num�ration non tri�e.
			return nodes;
		}
	}
}
