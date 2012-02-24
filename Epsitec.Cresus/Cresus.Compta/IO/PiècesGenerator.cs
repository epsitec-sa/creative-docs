//	Copyright � 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	/// Cette classe s'occupe de g�n�rer les num�ros de pi�ces.
	/// </summary>
	public class Pi�cesGenerator
	{
		public Pi�cesGenerator(MainWindowController mainWindowController)
		{
			this.mainWindowController = mainWindowController;

			this.freezer = new List<FreezerData> ();
		}


		public FormattedText GetRemoveError(ComptaPi�cesGeneratorEntity generator)
		{
			//	Retourne le message d'erreur �ventuel, si on souhaite supprimer ce g�n�rateur.
			int utilisateurCount, p�riodeCount, journalCount;
			int total = this.GetStatistics (generator, out utilisateurCount, out p�riodeCount, out journalCount);

			if (total == 0)
			{
				return FormattedText.Empty;
			}
			else
			{
				return string.Format ("Ce g�n�rateur ne peut pas �tre supprim�,<br/>car il est utilis� {0} fois.", total.ToString ());
			}
		}

		public FormattedText GetSample(ComptaPi�cesGeneratorEntity generator)
		{
			var list = new List<string> ();

			int n = generator.Num�ro;
			for (int i = 0; i < 10; i++)
			{
				list.Add (this.GetFormattedPi�ce (generator, n).ToString ());
				n += generator.Incr�ment;
			}
			list.Add ("...");

			return string.Join (" ", list);
		}

		public FormattedText GetSummary(ComptaPi�cesGeneratorEntity generator)
		{
			//	Retourne un r�sum� de l'utilisation du g�n�rateur.
			int utilisateurCount, p�riodeCount, journalCount;
			int total = this.GetStatistics (generator, out utilisateurCount, out p�riodeCount, out journalCount);

			if (total == 0)
			{
				return "Inutilis�";
			}
			else
			{
				var list = new List<string> ();

				if (utilisateurCount != 0)
				{
					list.Add (string.Format ("{0} utilisateur{1}", utilisateurCount.ToString (), utilisateurCount <= 1 ? "":"s"));
				}

				if (p�riodeCount != 0)
				{
					list.Add (string.Format ("{0} p�riode{1}", p�riodeCount.ToString (), p�riodeCount <= 1 ? "":"s"));
				}

				if (journalCount != 0)
				{
					list.Add (string.Format ("{0} journa{1}", journalCount.ToString (), journalCount <= 1 ? "l":"ux"));
				}

				return "Utilis� par " + Strings.SentenceConcat (list);
			}
		}

		private int GetStatistics(ComptaPi�cesGeneratorEntity generator, out int utilisateurCount, out int p�riodeCount, out int journalCount)
		{
			utilisateurCount = this.mainWindowController.Compta.Utilisateurs.Where (x => x.Pi�cesGenerator == generator).Count ();
			p�riodeCount     = this.mainWindowController.Compta.P�riodes    .Where (x => x.Pi�cesGenerator == generator).Count ();
			journalCount     = this.mainWindowController.Compta.Journaux    .Where (x => x.Pi�cesGenerator == generator).Count ();

			return utilisateurCount + p�riodeCount + journalCount;
		}


		public void Burn(ComptaJournalEntity journal)
		{
			//	Br�le les num�ros utilis�s, ce qui revient � vider le cong�lateur.
			var generator = this.GetGenerator (this.mainWindowController.CurrentUser, this.mainWindowController.P�riode, journal);
			this.ClearFreezer (generator);
		}

		public FormattedText GetProchainePi�ce(ComptaJournalEntity journal, int rank = 0)
		{
			//	Retourne le prochain num�ro de pi�ce � utiliser. Tant que les num�ros n'ont pas �t� br�l�s,
			//	des appels successifs � cette m�thode retournent toujours les m�mes num�ros de pi�ce.
			var generator = this.GetGenerator (this.mainWindowController.CurrentUser, this.mainWindowController.P�riode, journal);
			if (generator == null)
			{
				return FormattedText.Null;
			}

			var pi�ce = this.SearchInsideFreezer (generator, rank);  // cherche dans le cong�lateur

			if (pi�ce.IsNullOrEmpty)  // pas trouv� ?
			{
				//	Si on a mis les pi�ces de rangs 0, 1 et 2 dans le cong�lateur et qu'on demande la pi�ce de rang 7,
				//	on va g�n�rer et mettre dans le cong�lateur les pi�ces 3 � 7.
				int max = this.GetMaxRankInsideFreezer (generator);

				for (int i = max+1; i <= rank; i++)
				{
					pi�ce = this.GetProchainePi�ce (generator);  // g�n�re un nouveau num�ro de pi�ce
					this.AddInsideFreezer (generator, i, pi�ce);
				}

				//	On effectue � nouveau la recherche dans le cong�lateur, qui doit forc�ment aboutir.
				pi�ce = this.SearchInsideFreezer (generator, rank);
			}

			return pi�ce;
		}


		private ComptaPi�cesGeneratorEntity GetGenerator(ComptaUtilisateurEntity utilisateur, ComptaP�riodeEntity p�riode, ComptaJournalEntity journal)
		{
			//	Retourne le g�n�rateur de num�ros de pi�ces � utiliser.
			//	TODO: Priorit�s � revoir �ventuellement ?
			if (journal != null && journal.Pi�cesGenerator != null)
			{
				return journal.Pi�cesGenerator;
			}

			if (p�riode != null && p�riode.Pi�cesGenerator != null)
			{
				return p�riode.Pi�cesGenerator;
			}

			if (utilisateur != null && utilisateur.Pi�cesGenerator != null)
			{
				return utilisateur.Pi�cesGenerator;
			}

			return this.mainWindowController.Compta.Pi�cesGenerator.FirstOrDefault ();
		}

		private FormattedText GetProchainePi�ce(ComptaPi�cesGeneratorEntity generator)
		{
			//	Retourne le prochain num�ro de pi�ce � utiliser.
			if (generator == null)
			{
				return FormattedText.Null;
			}
			else
			{
				int n = this.GetPi�ceProchainNum�ro (generator);
				return this.GetFormattedPi�ce (generator, n);
			}
		}

		private FormattedText GetFormattedPi�ce(ComptaPi�cesGeneratorEntity generator, int n)
		{
			string s = n.ToString (System.Globalization.CultureInfo.InvariantCulture);

			if (generator.Digits != 0 && generator.Digits > s.Length)
			{
				s = new string ('0', generator.Digits - s.Length) + s;  // compl�te avec des z�ros
			}

			if (!generator.S�pMilliers.IsNullOrEmpty)
			{
				s = Strings.AddThousandSeparators (s, generator.S�pMilliers.ToSimpleText ());
			}

			s = generator.Pr�fixe + s + generator.Suffixe;

			return s;
		}

		private int GetPi�ceProchainNum�ro(ComptaPi�cesGeneratorEntity generator)
		{
			//	Retourne le prochain num�ro de pi�ce � utiliser.
			//	TODO: Il faudra v�rifier que cette proc�dure fonctionne en multi-utilisateur !
			int n = generator.Num�ro;
			generator.Num�ro += generator.Incr�ment;
			return n;
		}



		private void ClearFreezer(ComptaPi�cesGeneratorEntity generator)
		{
			//	Vide le cong�lateur de tous les num�ros d'un g�n�rateur donn�.
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

		private int GetMaxRankInsideFreezer(ComptaPi�cesGeneratorEntity generator)
		{
			//	Retourne le rang le plus grand contenu dans le cong�lateur.
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

		private FormattedText SearchInsideFreezer(ComptaPi�cesGeneratorEntity generator, int rank)
		{
			//	Cherche un num�ro de pi�ce contenu dans le cong�lateur.
			foreach (var data in this.freezer)
			{
				if (data.Generator == generator &&
					data.Rank      == rank      )
				{
					return data.Pi�ce;
				}
			}

			return FormattedText.Null;
		}

		private void AddInsideFreezer(ComptaPi�cesGeneratorEntity generator, int rank, FormattedText pi�ce)
		{
			//	Ajoute un num�ro de pi�ce dans le cong�lateur.
			var data = new FreezerData (generator, rank, pi�ce);
			this.freezer.Add (data);
		}


		private class FreezerData
		{
			//	Donn�e du cong�lateur.
			public FreezerData(ComptaPi�cesGeneratorEntity generator, int rank, FormattedText pi�ce)
			{
				this.Generator = generator;
				this.Rank      = rank;
				this.Pi�ce     = pi�ce;
			}

			public ComptaPi�cesGeneratorEntity Generator
			{
				get;
				private set;
			}

			public int Rank
			{
				get;
				private set;
			}

			public FormattedText Pi�ce
			{
				get;
				private set;
			}
		}


		private readonly MainWindowController		mainWindowController;
		private readonly List<FreezerData>			freezer;
	}
}
