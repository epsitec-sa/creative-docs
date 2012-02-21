//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.IO
{
	/// <summary>
	/// Cette classe s'occupe de générer les numéros de pièces.
	/// </summary>
	public class PiècesGenerator
	{
		public PiècesGenerator(MainWindowController mainWindowController)
		{
			this.mainWindowController = mainWindowController;

			this.freezer = new List<FreezerData> ();
		}


		public void Burn(ComptaJournalEntity journal)
		{
			//	Brûle les numéros utilisés, ce qui revient à vider le congélateur.
			var generator = this.GetGenerator (this.mainWindowController.CurrentUser, this.mainWindowController.Période, journal);
			this.ClearFreezer (generator);
		}

		public FormattedText GetProchainePièce(ComptaJournalEntity journal, int rank = 0)
		{
			//	Retourne le prochain numéro de pièce à utiliser. Tant que les numéros n'ont pas été brûlés,
			//	des appels successifs à cette méthode retournent toujours les mêmes numéros de pièce.
			var generator = this.GetGenerator (this.mainWindowController.CurrentUser, this.mainWindowController.Période, journal);
			var pièce = this.SearchInsideFreezer (generator, rank);  // cherche dans le congélateur

			if (pièce.IsNullOrEmpty)  // pas trouvé ?
			{
				//	Si on a mis les pièces de rangs 0, 1 et 2 dans le congélateur et qu'on demande la pièce de rang 7,
				//	on va demander et mettre dans le congélateur les pièces 3 à 7.
				int max = this.GetMaxRankInsideFreezer (generator);

				for (int i = 0; i < rank-max; i++)
				{
					pièce = this.GetProchainePièce (generator);
					this.AddInsideFreezer (generator, rank, pièce);
				}

				//	On effectue à nouveau la recherche dans le congélateur, qui doit forcément aboutir.
				pièce = this.SearchInsideFreezer (generator, rank);
			}

			return pièce;
		}


		private ComptaPièceEntity GetGenerator(ComptaUtilisateurEntity utilisateur, ComptaPériodeEntity période, ComptaJournalEntity journal)
		{
			//	Retourne le générateur de numéros de pièces à utiliser.
			//	TODO: Priorités à revoir éventuellement ?
			if (journal != null && journal.GénérateurDePièces != null)
			{
				return journal.GénérateurDePièces;
			}

			if (période != null && période.GénérateurDePièces != null)
			{
				return période.GénérateurDePièces;
			}

			if (utilisateur != null && utilisateur.GénérateurDePièces != null)
			{
				return utilisateur.GénérateurDePièces;
			}

			return this.mainWindowController.Compta.Pièces.FirstOrDefault ();
		}

		private FormattedText GetProchainePièce(ComptaPièceEntity generator)
		{
			//	Retourne le prochain numéro de pièce à utiliser.
			int n = this.GetPièceProchainNuméro (generator);
			string s = n.ToString (System.Globalization.CultureInfo.InvariantCulture);

			if (generator.Digits != 0 && generator.Digits > s.Length)
			{
				s = new string ('0', generator.Digits - s.Length) + s;
			}

			if (!generator.SépMilliers.IsNullOrEmpty)
			{
				s = Strings.AddThousandSeparators (s, generator.SépMilliers.ToSimpleText ());
			}

			s = generator.Préfixe + s + generator.Suffixe;

			return s;
		}

		private int GetPièceProchainNuméro(ComptaPièceEntity generator)
		{
			//	Retourne le prochain numéro de pièce à utiliser.
			//	TODO: Il faudra vérifier que cette procédure fonctionne en multi-utilisateur !
			int n = generator.Numéro;
			generator.Numéro += generator.Incrément;
			return n;
		}



		private void ClearFreezer(ComptaPièceEntity generator)
		{
			//	Vide le congélateur de tous les numéros d'un générateur donné.
			int i = 0;

			while (i < this.freezer.Count)
			{
				var data = this.freezer[i];

				if (data.Generator == generator)
				{
					this.freezer.RemoveAt (i);
				}
				else
				{
					i++;
				}
			}
		}

		private int GetMaxRankInsideFreezer(ComptaPièceEntity generator)
		{
			//	Retourne le rang le plus grand contenu dans le congélateur.
			int max = -1;

			foreach (var data in this.freezer)
			{
				if (data.Generator == generator)
				{
					max = System.Math.Max (max, data.Rank);
				}
			}

			return max;
		}

		private FormattedText SearchInsideFreezer(ComptaPièceEntity generator, int rank)
		{
			//	Cherche un numéro de pièce contenu dans le congélateur.
			foreach (var data in this.freezer)
			{
				if (data.Generator == generator &&
					data.Rank      == rank      )
				{
					return data.Pièce;
				}
			}

			return FormattedText.Null;
		}

		private void AddInsideFreezer(ComptaPièceEntity generator, int rank, FormattedText pièce)
		{
			//	Ajoute un numéro de pièce dans le congélateur.
			var data = new FreezerData (generator, rank, pièce);
			this.freezer.Add (data);
		}


		private class FreezerData
		{
			//	Donnée du congélateur.
			public FreezerData(ComptaPièceEntity generator, int rank, FormattedText pièce)
			{
				this.Generator = generator;
				this.Rank      = rank;
				this.Pièce     = pièce;
			}

			public ComptaPièceEntity Generator
			{
				get;
				private set;
			}

			public int Rank
			{
				get;
				private set;
			}

			public FormattedText Pièce
			{
				get;
				private set;
			}
		}


		private readonly MainWindowController		mainWindowController;
		private readonly List<FreezerData>			freezer;
	}
}
