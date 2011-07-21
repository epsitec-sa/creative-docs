//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Business;

namespace Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers
{
	public class IncrementalNumberGenerator
	{
		public IncrementalNumberGenerator()
		{
			this.seeds = new Dictionary<int, int> ();
			this.groupNumberAlreadyGenerated = new List<int> ();
		}

		public void Reset()
		{
			this.seeds.Clear ();
			this.groupNumberAlreadyGenerated.Clear ();

			this.groupIndex = 0;
			this.seed = 0;
			this.rank = 0;
		}

		public void PutNext(int groupIndex)
		{
			this.groupIndex = groupIndex;

			if (!this.seeds.ContainsKey (this.groupIndex))
			{
				this.seeds.Add (this.groupIndex, 0);
			}

			this.seeds[this.groupIndex]++;
			this.seed = this.seeds[this.groupIndex];

			this.rank++;
			this.GenerateGroupNumber ();
			this.GenerateFullNumber ();
		}

		public string SimpleNumber
		{
			//	Retourne une numérotation incrémentale simple ("1", "2", "3", etc.).
			get
			{
				return this.rank.ToString ();
			}
		}

		public string GroupNumber
		{
			//	Retourne une numérotation de type paragraphe (par exemple "2.4.1") unique.
			//	Les lignes suivantes qui ont le même numéro retournent null.
			get
			{
				return this.groupNumber;
			}
		}

		public string FullNumber
		{
			//	Retourne une numérotation de type paragraphe (par exemple "2.4.1.1") pour
			//	chaque ligne. Chaque ligne aura un numéro unique.
			get
			{
				return this.fullNumber;
			}
		}


		private void GenerateGroupNumber()
		{
			if (this.groupNumberAlreadyGenerated.Contains (this.groupIndex))  // numéro déjà donné ?
			{
				this.groupNumber = null;
			}
			else
			{
				this.groupNumberAlreadyGenerated.Add (this.groupIndex);
				this.groupNumber = IncrementalNumberGenerator.GetFormattedGroupIndex (this.groupIndex);
			}
		}

		private void GenerateFullNumber()
		{
			var n1 = IncrementalNumberGenerator.GetFormattedGroupIndex (this.groupIndex);
			var n2 = this.seed.ToString ();

			if (string.IsNullOrEmpty (n1))
			{
				this.fullNumber = n2;
			}
			else
			{
				this.fullNumber = string.Concat (n1, ".", n2);
			}
		}


		private static string GetFormattedGroupIndex(int groupIndex)
		{
			//	Formate un GroupIndex pour l'homme.
			//	    0 -> rien
			//	    1 -> 1
			//	  201 -> 1.2
			//	30201 -> 1.2.3
			if (groupIndex == 0)
			{
				return "0";
			}

			var builder = new System.Text.StringBuilder ();

			bool first = true;

			while (groupIndex != 0)
			{
				if (!first)
				{
					builder.Append (".");
				}

				builder.Append ((groupIndex%100).ToString ());
				groupIndex /= 100;
				first = false;
			}

			return builder.ToString ();
		}


		private readonly Dictionary<int, int>	seeds;
		private readonly List<int>				groupNumberAlreadyGenerated;

		private int								groupIndex;
		private int								seed;
		private int								rank;
		private string							groupNumber;
		private string							fullNumber;
	}
}
