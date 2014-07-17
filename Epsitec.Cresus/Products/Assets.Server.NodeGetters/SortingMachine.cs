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
			System.Func<T, int>				getGroupData,
			System.Func<T, ComparableData>	getPrimaryData,
			System.Func<T, ComparableData>	getSecondaryData
		)
		{
			var result = nodes;

			//	1) On trie en premier selon le crit�re secondaire.
			if (instructions.SecondaryField != ObjectField.Unknown)
			{
				if (instructions.SecondaryType == SortedType.Ascending)
				{
					result = result.OrderBy (x => getSecondaryData (x));
				}
				else
				{
					result = result.OrderByDescending (x => getSecondaryData (x));
				}
			}

			//	2) On trie en deuxi�me selon le crit�re principal.
			if (instructions.PrimaryField != ObjectField.Unknown)
			{
				if (instructions.PrimaryType == SortedType.Ascending)
				{
					result = result.OrderBy (x => getPrimaryData (x));
				}
				else
				{
					result = result.OrderByDescending (x => getPrimaryData (x));
				}
			}

			//	3) On trie finalement ensuite selon les groupes, ce qui prime
			//	   sur tous les autres tris.
			if (getGroupData != null)
			{
				result = result.OrderBy (x => getGroupData (x));
			}

			return result;
		}
	}
}
