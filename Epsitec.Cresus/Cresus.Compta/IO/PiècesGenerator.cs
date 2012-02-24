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


		public FormattedText GetRemoveError(ComptaPiècesGeneratorEntity generator)
		{
			//	Retourne le message d'erreur éventuel, si on souhaite supprimer ce générateur.
			int utilisateurCount, périodeCount, journalCount;
			int total = this.GetStatistics (generator, out utilisateurCount, out périodeCount, out journalCount);

			if (total == 0)
			{
				return FormattedText.Empty;
			}
			else
			{
				return string.Format ("Ce générateur ne peut pas être supprimé,<br/>car il est utilisé {0} fois.", total.ToString ());
			}
		}

		public FormattedText GetSample(ComptaPiècesGeneratorEntity generator)
		{
			var list = new List<string> ();

			int n = generator.Numéro;
			for (int i = 0; i < 10; i++)
			{
				list.Add (this.GetFormattedPièce (generator, n).ToString ());
				n += generator.Incrément;
			}
			list.Add ("...");

			return string.Join (" ", list);
		}

		public FormattedText GetSummary(ComptaPiècesGeneratorEntity generator)
		{
			//	Retourne un résumé de l'utilisation du générateur.
			int utilisateurCount, périodeCount, journalCount;
			int total = this.GetStatistics (generator, out utilisateurCount, out périodeCount, out journalCount);

			if (total == 0)
			{
				return "Inutilisé";
			}
			else
			{
				var list = new List<string> ();

				if (utilisateurCount != 0)
				{
					list.Add (string.Format ("{0} utilisateur{1}", utilisateurCount.ToString (), utilisateurCount <= 1 ? "":"s"));
				}

				if (périodeCount != 0)
				{
					list.Add (string.Format ("{0} période{1}", périodeCount.ToString (), périodeCount <= 1 ? "":"s"));
				}

				if (journalCount != 0)
				{
					list.Add (string.Format ("{0} journa{1}", journalCount.ToString (), journalCount <= 1 ? "l":"ux"));
				}

				return "Utilisé par " + Strings.SentenceConcat (list);
			}
		}

		private int GetStatistics(ComptaPiècesGeneratorEntity generator, out int utilisateurCount, out int périodeCount, out int journalCount)
		{
			utilisateurCount = this.mainWindowController.Compta.Utilisateurs.Where (x => x.PiècesGenerator == generator).Count ();
			périodeCount     = this.mainWindowController.Compta.Périodes    .Where (x => x.PiècesGenerator == generator).Count ();
			journalCount     = this.mainWindowController.Compta.Journaux    .Where (x => x.PiècesGenerator == generator).Count ();

			return utilisateurCount + périodeCount + journalCount;
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
			if (generator == null)
			{
				return FormattedText.Null;
			}

			var pièce = this.SearchInsideFreezer (generator, rank);  // cherche dans le congélateur

			if (pièce.IsNullOrEmpty)  // pas trouvé ?
			{
				//	Si on a mis les pièces de rangs 0, 1 et 2 dans le congélateur et qu'on demande la pièce de rang 7,
				//	on va générer et mettre dans le congélateur les pièces 3 à 7.
				int max = this.GetMaxRankInsideFreezer (generator);

				for (int i = max+1; i <= rank; i++)
				{
					pièce = this.GetProchainePièce (generator);  // génère un nouveau numéro de pièce
					this.AddInsideFreezer (generator, i, pièce);
				}

				//	On effectue à nouveau la recherche dans le congélateur, qui doit forcément aboutir.
				pièce = this.SearchInsideFreezer (generator, rank);
			}

			return pièce;
		}


		private ComptaPiècesGeneratorEntity GetGenerator(ComptaUtilisateurEntity utilisateur, ComptaPériodeEntity période, ComptaJournalEntity journal)
		{
			//	Retourne le générateur de numéros de pièces à utiliser.
			//	TODO: Priorités à revoir éventuellement ?
			if (journal != null && journal.PiècesGenerator != null)
			{
				return journal.PiècesGenerator;
			}

			if (période != null && période.PiècesGenerator != null)
			{
				return période.PiècesGenerator;
			}

			if (utilisateur != null && utilisateur.PiècesGenerator != null)
			{
				return utilisateur.PiècesGenerator;
			}

			return this.mainWindowController.Compta.PiècesGenerator.FirstOrDefault ();
		}

		private FormattedText GetProchainePièce(ComptaPiècesGeneratorEntity generator)
		{
			//	Retourne le prochain numéro de pièce à utiliser.
			if (generator == null)
			{
				return FormattedText.Null;
			}
			else
			{
				int n = this.GetPièceProchainNuméro (generator);
				return this.GetFormattedPièce (generator, n);
			}
		}

		private FormattedText GetFormattedPièce(ComptaPiècesGeneratorEntity generator, int n)
		{
			string s = n.ToString (System.Globalization.CultureInfo.InvariantCulture);

			if (generator.Digits != 0 && generator.Digits > s.Length)
			{
				s = new string ('0', generator.Digits - s.Length) + s;  // complète avec des zéros
			}

			if (!generator.SépMilliers.IsNullOrEmpty)
			{
				s = Strings.AddThousandSeparators (s, generator.SépMilliers.ToSimpleText ());
			}

			s = generator.Préfixe + s + generator.Suffixe;

			return s;
		}

		private int GetPièceProchainNuméro(ComptaPiècesGeneratorEntity generator)
		{
			//	Retourne le prochain numéro de pièce à utiliser.
			//	TODO: Il faudra vérifier que cette procédure fonctionne en multi-utilisateur !
			int n = generator.Numéro;
			generator.Numéro += generator.Incrément;
			return n;
		}



		private void ClearFreezer(ComptaPiècesGeneratorEntity generator)
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

		private int GetMaxRankInsideFreezer(ComptaPiècesGeneratorEntity generator)
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

		private FormattedText SearchInsideFreezer(ComptaPiècesGeneratorEntity generator, int rank)
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

		private void AddInsideFreezer(ComptaPiècesGeneratorEntity generator, int rank, FormattedText pièce)
		{
			//	Ajoute un numéro de pièce dans le congélateur.
			var data = new FreezerData (generator, rank, pièce);
			this.freezer.Add (data);
		}


		private class FreezerData
		{
			//	Donnée du congélateur.
			public FreezerData(ComptaPiècesGeneratorEntity generator, int rank, FormattedText pièce)
			{
				this.Generator = generator;
				this.Rank      = rank;
				this.Pièce     = pièce;
			}

			public ComptaPiècesGeneratorEntity Generator
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
