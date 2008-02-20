//	Copyright � 2002-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Daniel ROUX

using System.Collections.Generic;

namespace Epsitec.Common.Text.BreakEngines
{
	/// <summary>
	/// Algorithme de c�sure pour les mots fran�ais.
	/// Ce syst�me a �t� invent� en 1985 pour le logiciel Text sur Smaky.
	/// Il �tait initialement programm� en assembleur Calm 2 pour le processeur
	/// Mototola 68000. Ceci explique son approche tr�s compacte et efficace,
	/// avec 4 dictionnaires extr�mement courts.
	/// Ce syst�me permet de g�rer tous les mots, m�me s'ils n'existent pas ou
	/// s'ils sont mal orthographi�s, selon la logique utilis�e dans la langue
	/// fran�aise :
	///   antichenit    -> an/ti/che/nit
	///   bisouillage   -> bi/souil/la/ge
	///   fradorn�re    -> fra/dor/n�/re
	///   franssaize    -> frans/sai/ze
	///   fumm�e        -> fum/m�e
	///   ortografe     -> or/to/gra/fe
	///   ponturer      -> pon/tu/rer
	///   tralal�rement -> tra/la/l�/re/ment
	/// </summary>
	public class FrenchWordBreakEngine
	{
		static public IEnumerable<int> Break(string word)
		{
			// Coupe un mot selon un certain nombre de r�gles bas�es sur
			// les voyelles et les consonnes. Le mot donn� peut contenir des
			// lettres accentu�es. En revanche, il ne doit pas contenir de
			// tiret. Rend une liste des c�sures possibles.
			List<int> list = new List<int>();

			TextPointer tp = new TextPointer(word);
			if (tp.Length >= 5)
			{
				// Cherche dans le dictionnaire des exceptions si le mot en fait
				// partie ou pas.
				tp.SearchException(list);

				// Avance dans le mot syllabes par syllabes ...
				bool firstSyllable = true;
				while (tp.Position < tp.Length)
				{
					if (!tp.IsSyllable(firstSyllable))
					{
						tp.Advance();
						continue;
					}

					// Teste si la c�sure est une c�sure interdite � cause d'une
					// exception dans dico_nc.
					if (tp.Position == tp.Banned)
					{
						continue;
					}

					// Teste si la c�sure est trop proche du d�but du mot (par
					// exemple: �-cole) ou trop proche de la fin du mot (par
					// exemple: �cole-s).
					if (tp.Position < tp.Root+2)
					{
						continue;
					}

					if (tp.Position > tp.Length-2)
					{
						continue;
					}

					firstSyllable = false;

					// Teste s'il ne reste que des consonnes depuis cette �ventuelle
					// c�sure (par exemple: rangeme-nts)
					if (tp.IsRestConsonant())
					{
						break;
					}

					// Si on est juste apr�s un apostrophe, il faut ignorer
					// la c�sure. Par exemple dans:  aujourd'hui
					if (tp.GetChar(-1) == '\'')
					{
						continue;
					}
					if (tp.GetChar(-2) == '\'')
					{
						continue;
					}

					list.Add(tp.Start+tp.Position);
				}
			}

			return list;
		}


		// Dictionnaire de tous les d�buts possibles de syllabes (cette
		// liste ne doit contenir que des consonnes) :
		static protected string[] syllableStartTable =
		{
			"BL",		// re-blochon
			"BR",		// cham-bre
			"CH",		// al-chimie
			"CL",		// in-cliner
			"CR",		// convain-cre
			"DR",		// suspen-dre
			"FL",		// in-fluence
			"FR",		// b�-froid
			"GL",		// jon-gleur
			"GN",		// ma-gn�tique
			"GR",		// bi-gre
			"PH",		// phos-phore
			"PL",		// exem-plaire
			"PR",		// com-pr�hension
			"PS",		// rha-psodie
			"SCH",		// (pour l'allemand)
			"TH",		// or-thographe
			"TR",		// cons-truction
			"VR",		// fi�-vre
		};

		// Dictionnaire de tous les mots (ou d�buts de mots) � couper
		// normalement (cette liste est parcourue AVANT la liste des
		// divisions �tymologiques tableExceptions) :
		static protected string[] tableNoExceptions =
		{
			"DESERT",	// d�-sert
			"DESID",	// d�-sid�ratif
			"DESIGN",	// d�-signer
			"DESIR",	// d�-sirer
			"DESIS",	// d�-sister
			"DESOL",	// d�-sol�
			"DESORM",	// d�-sormais

			"INIQ",		// ini-que
			"INIT",		// ini-tiative

			"NONA",		// no-nante

			"PREN",		// pren-dre
			"PRESB",	// pres-byte
			"PRESCR",	// pres-crire
			"PRESQ",	// pres-que
			"PRESS",	// pres-sion
			"PREST",	// pres-tence
			"PREU",		// preu-ve

			"REINE",	// rei-ne
			"RETROU",	// retrou-ver

			"SURAN",	// su-rann�
			"SUREA",	// su-reau
			"SUREMEN",	// su-rement
			"SURET",	// su-ret�
			"SURICA",	// su-ricate
			"SUROI",	// su-ro�t
			"SUROS",	// su-ros
			"SURIR",	// su-rir
			"SUPERI",	// su-p�-rio-ri-t�

			"TRANSI",	// tran-sistor
		};

		// Dictionnaire de tous les mots (ou d�buts de mots) � couper
		// sp�cialement (divisions �tymologiques) :
		static protected string[] tableExceptions =
		{
			"AERO 4",		// a�ro-spatial
			"ANTIA 24",		// anti-alcoolique
			"ANTISP 24",	// anti-spasmodique
			"ATMO 24",		// atmo-sph�re

			"BAY 2",		// ba-yer

			"CAOU 4",		// caou-tchouc
			"COO 2",		// co-op�ration
			"CONSCI 3",		// con-science
			"CIS 3",		// cis-alpin

			"DESTA 2",		// d�-stabiliser
			"DES 3",		// d�s-aveu
			"DIAGN 4",		// diag-nostique

			"EXTRAO 25",	// extra-ordinaire

			"HEMI 24",		// hemi-sph�re

			"INA 2",		// in-actif
			"INE 2",		// in-�gal
			"INI 2",		// in-imaginable
			"INO 2",		// in-oubliable
			"INSTAB 2",		// in-stable
			"INU 2",		// in-utiliser

			"KILO 24",		// kilo-octets

			"MALADR 3",		// mal-adroit
			"MALAP 3",		// mal-appris
			"MALENT 3",		// mal-entendu
			"MALINT 3",		// mal-intensionn�
			"MALODO 3",		// mal-odorant
			"MESAL 3",		// mes-alliance
			"MESAV 3",		// mes-aventure
			"MESE 3",		// mes-estimer
			"MESI 3",		// mes-intelligence
			"MICRO 25",		// micro-ordinateur

			"NON 3",		// non-obstant

			"OTORH 3",		// oto-rhinolaryngologiste

			"POE 2",		// po-�te
			"PRE 3",		// pr�-occuper
			"PROE 3",		// pro-�minent

			"REAB 2",		// r�-abonner
			"REAC 2",		// r�-action
			"REAF 2",		// r�-affirmer
			"REAG 2",		// r�-agir
			"REAR 2",		// r�-armer
			"REAS 2",		// r�-assigner
			"REAT 2",		// r�-atteler
			"REE 2",		// r�-�lection
			"REI 2",		// r�-int�grer
			"REO 2",		// r�-organiser
			"RESTR 2",		// re-structurer
			"RETRO 25",		// r�tro-spectif

			"STAG 4",		// stag-nant
			"SUBLU 3",		// sub-lunaire
			"SUBO 3",		// sub-ordonner
			"SUBRO 3",		// sub-rogateur
			"SUBUR 3",		// sub-urbain
			"SUR 3",		// sur-�lever
			"SUPER 25",		// su-per-struc-tu-re

			"TRANS 5",		// trans-atlantique
			"TECH 4",		// tech-nique
			"TELE 24",		// t�l�-scope
		};

		// Dictionnaire de tous les d�buts de mots � ne pas couper
		// (exceptions tr�s sp�ciales) :
		static protected string[] tableSpecialBreak =
		{
			"HYMNE 3",		// hym-ne
		};


		static protected string RemoveAccent(string s)
		{
			//	Retourne la m�me cha�ne sans accent (� -> e).
			System.Text.StringBuilder builder;

			builder = new System.Text.StringBuilder(s.Length);
			for (int i=0; i<s.Length; i++)
			{
				builder.Append(FrenchWordBreakEngine.RemoveAccent(s[i]));
			}
			return builder.ToString();
		}

		static protected char RemoveAccent(char c)
		{
			//	Retourne le m�me caract�re sans accent (� -> e).
			//	TODO: traiter tous les accents unicode ?
			char lower = System.Char.ToLowerInvariant(c);
			char cc = lower;

			switch (cc)
			{
				case '�':
				case '�':
				case '�':
				case '�':
				case '�':
					cc = 'a';
					break;

				case '�':
					cc = 'c';
					break;

				case '�':
				case '�':
				case '�':
				case '�':
					cc = 'e';
					break;

				case '�':
				case '�':
				case '�':
				case '�':
					cc = 'i';
					break;

				case '�':
					cc = 'n';
					break;

				case '�':
				case '�':
				case '�':
				case '�':
				case '�':
					cc = 'o';
					break;

				case '�':
				case '�':
				case '�':
				case '�':
					cc = 'u';
					break;
			}

			if (lower != c)  // a-t-on utilis� une majuscule transform�e en minuscule ?
			{
				cc = System.Char.ToUpperInvariant(cc);  // remet en majuscule
			}

			return cc;
		}

		static protected bool IsVowel(char letter)
		{
			// Test si le caract�re est une voyelle, c'est-�-dire :
			//  "A" , "E" , "I" , "O" , "U"  ou  "Y"
			// La lettre "X" suit les m�mes r�gles que la lettre "Y".
			// De plus, une voyelle peut �tre un apostrophe pour r�soudre
			// les cas du genre  "l'�cran"  "qu'il"  "d'apprendre"  etc.
			// (on suppose qu'un apostrophe est toujours suivit d'une
			// voyelle).
			switch (letter)
			{
				case 'A':
				case 'E':
				case 'I':
				case 'O':
				case 'U':
				case 'Y':
				case 'X':
				case '\'':
					return true;
			}

			return false;
		}


		protected class TextPointer
		{
			public TextPointer(string word)
			{
				//	Constructeur avec un mot initial.
				this.text = FrenchWordBreakEngine.RemoveAccent(word).ToUpper();
				this.length = word.Length;
				this.position = 0;
				this.start = 0;
				this.banned = 0;
				this.root = 0;

				// Ignore les caract�res � la fin qui ne sont pas des lettres,
				// pour r�soudre les cas du type:  (commenc�-e)
				for (int i=this.length-1; i>=0; i--)
				{
					char letter = this.GetChar(i);

					if (letter >= 'A' && letter <= 'Z')
					{
						break;
					}

					this.length--;  // ignore un caract�re � la fin
				}

				// Avance dans le mot jusqu'� ce qu'une lettre soit rencontr�e.
				int len = 0;
				for (int i=0; i<this.length; i++)
				{
					char letter = this.GetChar(i);

					if (letter >= 'A' && letter <= 'Z')
					{
						break;
					}

					len++;
				}
				this.length -= len;
				this.start  += len;

				// Test si la 2�me ou 3�me lettre du mot est un apostrophe,
				// pour r�soudre les cas du genre:  l'extra-ordinaire  qu'elles
				if (this.GetChar(2) == '\'')
				{
					this.length -= 3;
					this.start  += 3;
				}
				else if (this.GetChar(1) == '\'')
				{
					this.length -= 2;
					this.start  += 2;
				}
			}

			public TextPointer(TextPointer src)
			{
				//	Constructeur qui copie une instance existante.
				this.text = src.text;
				src.CopyCursor(this);
			}

			protected void CopyCursor(TextPointer dst)
			{
				//	Copie la position du curseur.
				dst.start = this.start;
				dst.length = this.length;
				dst.position = this.position;
				dst.banned = this.banned;
				dst.root = this.root;
			}

			public int Start
			{
				//	Offset du d�but du mot (par exemple 2 avec "l'a�rospatial").
				get
				{
					return this.start;
				}
			}

			public int Length
			{
				//	Longueur du mot, apr�s Start (par exemple 11 avec "l'a�rospatial").
				get
				{
					return this.length;
				}
			}

			public int Position
			{
				//	Position dans le mot, apr�s Start (par exemple 1 avec "l'a|�rospatial).
				get
				{
					return this.position;
				}
			}

			public int Banned
			{
				get
				{
					return this.banned;
				}
			}

			public int Root
			{
				//	Longueur de la racine, apr�s Start (par exemple 4 avec "l'a�rospatial").
				get
				{
					return this.root;
				}
			}

			public char GetChar(int index)
			{
				// Donne un caract�re. Si index=0, c'est le caract�re sous
				// le pointeur qui est rendu. La valeur d'index peut �tre
				// positive ou n�gative.
				if (this.position+index < 0 || this.position+index >= this.length)
				{
					return (char) 0;
				}

				return this.text[this.start+this.position+index];
			}

			public bool Advance()
			{
				//	Avance le pointeur.
				if (this.position >= this.length)
				{
					return false;
				}

				this.position++;
				return true;
			}

			protected bool IsDouble()
			{
				// Test s'il y a deux m�mes lettres qui se suivent.
				// Reste moins de 2 lettres ?
				if (this.length-this.position < 2)
				{
					return false;
				}

				// Deux lettres diff�rentes ?
				if (this.GetChar(0) != this.GetChar(1))
				{
					return false;
				}

				// V�rifie si la c�sure est dans une terminaison "f�minin pluriel",
				// c'est-�-dire "�es", pour �viter de couper "commenc�-es" !

				if (this.length-this.position == 3 &&
					this.GetChar(0) == 'E' &&
					this.GetChar(1) == 'E' &&
					this.GetChar(2) == 'S')
				{
					return false;
				}

				this.Advance();
				return true;
			}

			protected bool SkipVowels()
			{
				// Saute une ou plusieurs voyelles.
				// Retourne true si doubles lettres.
				while (true)
				{
					if (this.position == this.length)
					{
						return false;
					}

					if (this.IsDouble())
					{
						return true;
					}

					if (!FrenchWordBreakEngine.IsVowel(this.GetChar(0)))
					{
						return false;
					}

					this.Advance();
				}
			}

			protected bool SkipConsonants(bool firstSyllable)
			{
				// Saute une ou plusieurs consonnes.
				// Retourne true si doubles lettres.
				if (!firstSyllable)
				{
					while (true)
					{
						if (this.position == this.length)
						{
							return false;
						}

						if (!FrenchWordBreakEngine.IsVowel(this.GetChar(0)))
						{
							break;
						}

						this.Advance();
					}
				}

				while (true)
				{
					if (this.position == this.length)
					{
						return false;
					}

					if (this.IsDouble())
					{
						return true;
					}

					if (FrenchWordBreakEngine.IsVowel(this.GetChar(0)))
					{
						return false;
					}

					this.Advance();
				}
			}

			public bool IsRestConsonant()
			{
				// Teste s'il ne reste plus que des consonnes dans le mot.
				TextPointer tmp = new TextPointer(this);

				while (true)
				{
					char letter = tmp.GetChar(0);
					if (letter != 'X' && FrenchWordBreakEngine.IsVowel(letter))
					{
						return false;
					}

					if (!tmp.Advance())
					{
						return true;
					}
				}
			}

			protected bool SearchDictionary(string[] dico, List<int> list)
			{
				// Cherche un groupe de lettres dans un dictionnaire.
				if (this.position == this.length)
				{
					return false;
				}

				foreach (string key in dico)
				{
					for (int i=0; i<=key.Length; i++)
					{
						if (i == key.Length)
						{
							return true;
						}

						if (key[i] == ' ')
						{
							if (list != null)
							{
								i++;  // saute l'espace
								while (i < key.Length)
								{
									char num = key[i++];
									System.Diagnostics.Debug.Assert(num >= '1' && num <= '9');
									int offset = (int) (num-'0');
									list.Add(this.start+offset);
								}
							}
							return true;
						}

						if (this.GetChar(i) != key[i])
						{
							break;
						}
					}
				}

				return false;
			}

			public bool IsSyllable(bool firstSyllable)
			{
				// Cherche s'il s'agit d'une syllabe normale, c'est-�-dire:
				//  - une ou plusieurs consonnes quelconques (voir *)
				//  - une ou plusieurs voyelles quelconques (voir *)
				//  - �ventuellement quelques consonnes (jusqu'au d�but de la
				//    syllabe suivante)
				// (*) Consid�re qu'il s'agit d'une syllabe lorsque deux m�mes
				// lettres sont trouv�es.

				// Reste moins de 2 lettres ?
				if (this.length-this.position < 2)
				{
					return false;
				}

				TextPointer tp = new TextPointer(this);
				if (tp.SkipConsonants(firstSyllable))
				{
					tp.CopyCursor(this);
					return true;
				}

				if (tp.SkipVowels())
				{
					tp.CopyCursor(this);
					return true;
				}

				while (true)
				{
					// D�but de syllabe ?
					if (tp.SearchDictionary(FrenchWordBreakEngine.syllableStartTable, null))
					{
						tp.CopyCursor(this);
						return true;
					}

					// Doubles lettres ?
					if (tp.IsDouble())
					{
						tp.CopyCursor(this);
						return true;
					}

					if (tp.length-tp.position < 2)
					{
						return false;
					}

					// Consonne-voyelle ?
					if (FrenchWordBreakEngine.IsVowel(tp.GetChar(1)))
					{
						tp.CopyCursor(this);
						return true;
					}

					tp.Advance();
				}
			}

			public void SearchException(List<int> list)
			{
				// Cherche dans les dictionnaires si le mot est une exception.
				if (this.SearchDictionary(FrenchWordBreakEngine.tableNoExceptions, null))
				{
					return;
				}

				List<int> lb = new List<int>();
				if (this.SearchDictionary(FrenchWordBreakEngine.tableSpecialBreak, lb))
				{
					System.Diagnostics.Debug.Assert(lb.Count == 1);
					this.banned = lb[0]-this.start;
					return;
				}

				if (this.SearchDictionary(FrenchWordBreakEngine.tableExceptions, list))
				{
					System.Diagnostics.Debug.Assert(list.Count != 0);
					this.root = list[list.Count-1]-this.start;  // offset pr�position
					this.position = this.root;
					return;
				}
			}

			protected string				text;
			protected int					start;  // d�but effectif du mot
			protected int					length;  // longueur du texte
			protected int					position;  // position du pointeur
			protected int					banned;  // position c�sure interdite
			protected int					root;  // longueur pr�position
		}

	}
}
