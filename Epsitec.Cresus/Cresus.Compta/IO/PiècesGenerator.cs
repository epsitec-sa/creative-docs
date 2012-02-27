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

			int num�ro = generator.Num�ro;
			for (int i = 0; i < 10; i++)
			{
				list.Add (this.GetFormattedPi�ce (generator, num�ro).ToString ());
				num�ro += generator.Incr�ment;
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

		private bool IsPerso(ComptaPi�cesGeneratorEntity generator)
		{
			//	On consid�re qu'un g�n�rateur est personnel s'il n'a qu'un seul utilisateur.
			int utilisateurCount, p�riodeCount, journalCount;
			this.GetStatistics (generator, out utilisateurCount, out p�riodeCount, out journalCount);

			if (utilisateurCount == 1)
			{
				return true;
			}
			else if (utilisateurCount > 1)
			{
				return false;
			}

			return this.mainWindowController.Compta.Utilisateurs.Count <= 1;
		}

		private int GetStatistics(ComptaPi�cesGeneratorEntity generator, out int utilisateurCount, out int p�riodeCount, out int journalCount)
		{
			utilisateurCount = this.mainWindowController.Compta.Utilisateurs.Where (x => x.Pi�cesGenerator == generator).Count ();
			p�riodeCount     = this.mainWindowController.Compta.P�riodes    .Where (x => x.Pi�cesGenerator == generator).Count ();
			journalCount     = this.mainWindowController.Compta.Journaux    .Where (x => x.Pi�cesGenerator == generator).Count ();

			return utilisateurCount + p�riodeCount + journalCount;
		}


		public void Burn(ComptaJournalEntity journal, List<FormattedText> pi�ces)
		{
			//	Br�le les num�ros utilis�s, ce qui revient � vider le cong�lateur.
			var generator = this.GetGenerator (this.mainWindowController.CurrentUser, this.mainWindowController.P�riode, journal);

			if (generator != null)
			{
				this.Adjust (generator, pi�ces);
			}
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
					int num�ro = Pi�cesGenerator.GetPi�ceProchainNum�ro (generator);
					pi�ce = this.GetFormattedPi�ce (generator, num�ro);  // g�n�re un nouveau num�ro de pi�ce
					this.AddInsideFreezer (generator, num�ro, i, pi�ce);
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

		private FormattedText GetFormattedPi�ce(ComptaPi�cesGeneratorEntity generator, int num�ro)
		{
			//	Retourne un num�ro de pi�ce format�, avec pr�fixe, suffixe, etc.
			if (generator == null || string.IsNullOrEmpty (generator.Format))
			{
				return num�ro.ToString (System.Globalization.CultureInfo.InvariantCulture);
			}
			else
			{
				return Pi�cesGenerator.FormatPi�ce (generator.Format, num�ro);
			}
		}

		private static FormattedText FormatPi�ce(string format, int num�ro)
		{
			//	Retourne un num�ro de pi�ce format�, avec pr�fixe, suffixe, etc.
			string numString = num�ro.ToString (System.Globalization.CultureInfo.InvariantCulture);

			//	Construit la cha�ne du num�ro avec le bon nombre de digits.
			int digits = format.Where (x => x == '#').Count ();
			if (digits != 0)
			{
				if (digits > numString.Length)  // num�ro trop court ?
				{
					numString = new string ('0', digits - numString.Length) + numString;  // compl�te avec des z�ros
				}

				if (digits < numString.Length)  // num�ro trop long ?
				{
					numString = numString.Substring (numString.Length-digits);  // tronque
				}
			}

			//	Construit le num�ro final d'apr�s le format.
			var builder = new System.Text.StringBuilder ();

			int i = 0;
			foreach (var c in format)
			{
				if (c == '#')  // un digit ?
				{
					builder.Append (numString[i++]);
				}
				else  // un caract�re fixe ?
				{
					builder.Append (c);
				}
			}

			return builder.ToString ();
		}

		private static int? ParsePi�ce(string format, FormattedText pi�ce)
		{
			string p = pi�ce.ToSimpleText ();

			if (format.Length != p.Length)
			{
				return null;
			}

			int n = 0;

			for (int i = 0; i < p.Length; i++)
			{
				if (format[i] == '#')
				{
					var c = p[i];
					if (c < '0' || c > '9')
					{
						return null;
					}

					n *= 10;
					n += c-'0';
				}
				else
				{
					if (format[i] != p[i])
					{
						return null;
					}
				}
			}

			return n;
		}


		private static int GetPi�ceProchainNum�ro(ComptaPi�cesGeneratorEntity generator)
		{
			//	Retourne le prochain num�ro de pi�ce � utiliser.
			//	TODO: Il faudra v�rifier que cette proc�dure fonctionne en multi-utilisateur !
			int n = generator.Num�ro;
			generator.Num�ro += generator.Incr�ment;
			return n;
		}

		private static void SetPi�ceProchainNum�ro(ComptaPi�cesGeneratorEntity generator, int num�ro)
		{
			//	Modifie le prochain num�ro de pi�ce � utiliser.
			generator.Num�ro = num�ro;
		}


		private void Adjust(ComptaPi�cesGeneratorEntity generator, List<FormattedText> pi�ces)
		{
			//	Si un g�n�rateur n'est utilis� que par un seul utilisateur, la modification par l'utilisateur
			//	de la pi�ce propos�e modifie le prochain num�ro g�n�r�.
			if (this.IsPerso (generator))
			{
				//	Cherche tous les num�ros qui ont �t� entr�s manuellement et qui ne correspondent pas
				//	� un num�ro automatique.
				var outsiders = new List<FormattedText> ();

				foreach (var pi�ce in pi�ces)
				{
					if (!this.freezer.Where (x => x.Generator == generator && x.Pi�ce == pi�ce).Any ())
					{
						outsiders.Add (pi�ce);
					}
				}

				if (outsiders.Any ())
				{
					outsiders.Sort ();

					for (int i = outsiders.Count-1; i >= 0; i--)
					{
						int? num�ro = Pi�cesGenerator.ParsePi�ce (generator.Format, outsiders[i]);
						if (num�ro.HasValue)
						{
							Pi�cesGenerator.SetPi�ceProchainNum�ro (generator, num�ro.Value+generator.Incr�ment);
							this.ClearFreezer (generator);
							return;
						}
					}
				}
			}

			this.ClearFreezer (generator, pi�ces);
		}


		private void ClearFreezer(ComptaPi�cesGeneratorEntity generator, List<FormattedText> pi�ces)
		{
			//	Vide le cong�lateur de tous les num�ros utilis�s avec un g�n�rateur donn�.
			//	Contenu initial:
			//		Num�ro	Rank
			//		15		0
			//		16		1
			//		17		2
			//		18		3 <-- last=3
			//		19		4
			//		20		5
			//	Num�ros utilis�s:
			//		16 et 18
			//	Contenu final:
			//		Num�ro	Rank
			//		19		0
			//		20		1
			//	Dans cet exemple, les num�ros 15 et 17 n'ont pas �t� utilis�s, mais ils doivent tout
			//	de m�me �tre br�l�s, afin de garantir une num�rotation sans trous.

			//	Cherche le rang du dernier num�ro utilis�.
			int i = this.freezer.Count-1;
			int last = -1;
			while (i >= 0)
			{
				var data = this.freezer[i];

				if (data.Generator == generator)
				{
					if (pi�ces.Contains (data.Pi�ce))
					{
						last = data.Rank;
						break;
					}
				}

				i--;
			}

			//	Supprime tous les num�ros <= last.
			i = 0;
			while (i < this.freezer.Count)
			{
				var data = this.freezer[i];

				if (data.Generator == generator && data.Rank <= last)
				{
					this.freezer.RemoveAt (i);
				}
				else
				{
					i++;
				}
			}

			//	Renum�rote les rangs des num�ros restants de 0 � n.
			foreach (var data in this.freezer)
			{
				if (data.Generator == generator)
				{
					data.Rank -= last+1;
				}
			}
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

		private void AddInsideFreezer(ComptaPi�cesGeneratorEntity generator, int num�ro, int rank, FormattedText pi�ce)
		{
			//	Ajoute un num�ro de pi�ce dans le cong�lateur.
			var data = new FreezerData (generator, num�ro, rank, pi�ce);
			this.freezer.Add (data);
		}


		private class FreezerData
		{
			//	Donn�e du cong�lateur.
			public FreezerData(ComptaPi�cesGeneratorEntity generator, int num�ro, int rank, FormattedText pi�ce)
			{
				this.Generator = generator;
				this.Num�ro    = num�ro;
				this.Rank      = rank;
				this.Pi�ce     = pi�ce;
			}

			public ComptaPi�cesGeneratorEntity Generator
			{
				get;
				private set;
			}

			public int Num�ro
			{
				get;
				private set;
			}

			public int Rank
			{
				get;
				set;
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
