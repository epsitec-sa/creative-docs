//	Copyright © 2002-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Daniel ROUX

using System.Collections.Generic;

namespace Epsitec.Common.Text.BreakEngines
{
	/// <summary>
	/// Algorithme de césure pour les mots français.
	/// Ce système a été inventé en 1985 pour le logiciel Text sur Smaky.
	/// Il était initialement programmé en assembleur Calm 2 pour le processeur
	/// Mototola 68000. Ceci explique son approche très compacte et efficace,
	/// avec 4 dictionnaires extrèmement courts.
	/// Ce système permet de gérer tous les mots, même s'ils n'existent pas ou
	/// s'ils sont mal orthographiés, selon la logique utilisée dans la langue
	/// française :
	///   antichenit    -> an/ti/che/nit
	///   bisouillage   -> bi/souil/la/ge
	///   fradornère    -> fra/dor/nè/re
	///   franssaize    -> frans/sai/ze
	///   fummée        -> fum/mée
	///   ortografe     -> or/to/gra/fe
	///   ponturer      -> pon/tu/rer
	///   tralalèrement -> tra/la/lè/re/ment
	/// </summary>
	public class FrenchWordBreakEngine
	{
		static public IEnumerable<int> Break(string word)
		{
			// Coupe un mot selon un certain nombre de règles basées sur
			// les voyelles et les consonnes. Le mot donné peut contenir des
			// lettres accentuées. En revanche, il ne doit pas contenir de
			// tiret. Rend une liste des césures possibles.
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

					// Teste si la césure est une césure interdite à cause d'une
					// exception dans dicoNc.
					if (tp.Position == tp.Banned)
					{
						continue;
					}

					// Teste si la césure est trop proche du début du mot (par
					// exemple: é-cole) ou trop proche de la fin du mot (par
					// exemple: école-s).
					if (tp.Position < tp.Root+2)
					{
						continue;
					}

					if (tp.Position > tp.Length-2)
					{
						continue;
					}

					firstSyllable = false;

					// Teste s'il ne reste que des consonnes depuis cette éventuelle
					// césure (par exemple: rangeme-nts)
					if (tp.IsRestConsonant())
					{
						break;
					}

					// Si on est juste après un apostrophe, il faut ignorer
					// la césure. Par exemple dans:  aujourd'hui
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


		// Dictionnaire de tous les débuts possibles de syllabes (cette
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
			"FR",		// bé-froid
			"GL",		// jon-gleur
			"GN",		// ma-gnétique
			"GR",		// bi-gre
			"PH",		// phos-phore
			"PL",		// exem-plaire
			"PR",		// com-préhension
			"PS",		// rha-psodie
			"SCH",		// (pour l'allemand)
			"TH",		// or-thographe
			"TR",		// cons-truction
			"VR",		// fiè-vre
		};

		// Dictionnaire de tous les mots (ou débuts de mots) à couper
		// normalement (cette liste est parcourue AVANT la liste des
		// divisions étymologiques tableExceptions) :
		static protected string[] tableNoExceptions =
		{
			"DESERT",	// dé-sert
			"DESID",	// dé-sidératif
			"DESIGN",	// dé-signer
			"DESIR",	// dé-sirer
			"DESIS",	// dé-sister
			"DESOL",	// dé-solé
			"DESORM",	// dé-sormais

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

			"SURAN",	// su-ranné
			"SUREA",	// su-reau
			"SUREMEN",	// su-rement
			"SURET",	// su-reté
			"SURICA",	// su-ricate
			"SUROI",	// su-roît
			"SUROS",	// su-ros
			"SURIR",	// su-rir
			"SUPERI",	// su-pé-rio-ri-té

			"TRANSI",	// tran-sistor
		};

		// Dictionnaire de tous les mots (ou débuts de mots) à couper
		// spécialement (divisions étymologiques) :
		static protected string[] tableExceptions =
		{
			"AERO 4",		// aéro-spatial
			"ANTIA 24",		// anti-alcoolique
			"ANTISP 24",	// anti-spasmodique
			"ATMO 24",		// atmo-sphère

			"BAY 2",		// ba-yer

			"CAOU 4",		// caou-tchouc
			"COO 2",		// co-opération
			"CONSCI 3",		// con-science
			"CIS 3",		// cis-alpin

			"DESTA 2",		// dé-stabiliser
			"DES 3",		// dés-aveu
			"DIAGN 4",		// diag-nostique

			"EXTRAO 25",	// extra-ordinaire

			"HEMI 24",		// hemi-sphère

			"INA 2",		// in-actif
			"INE 2",		// in-égal
			"INI 2",		// in-imaginable
			"INO 2",		// in-oubliable
			"INSTAB 2",		// in-stable
			"INU 2",		// in-utiliser

			"KILO 24",		// kilo-octets

			"MALADR 3",		// mal-adroit
			"MALAP 3",		// mal-appris
			"MALENT 3",		// mal-entendu
			"MALINT 3",		// mal-intensionné
			"MALODO 3",		// mal-odorant
			"MESAL 3",		// mes-alliance
			"MESAV 3",		// mes-aventure
			"MESE 3",		// mes-estimer
			"MESI 3",		// mes-intelligence
			"MICRO 25",		// micro-ordinateur

			"NON 3",		// non-obstant

			"OTORH 3",		// oto-rhinolaryngologiste

			"POE 2",		// po-ète
			"PRE 3",		// pré-occuper
			"PROE 3",		// pro-éminent

			"REAB 2",		// ré-abonner
			"REAC 2",		// ré-action
			"REAF 2",		// ré-affirmer
			"REAG 2",		// ré-agir
			"REAR 2",		// ré-armer
			"REAS 2",		// ré-assigner
			"REAT 2",		// ré-atteler
			"REE 2",		// ré-élection
			"REI 2",		// ré-intégrer
			"REO 2",		// ré-organiser
			"RESTR 2",		// re-structurer
			"RETRO 25",		// rétro-spectif

			"STAG 4",		// stag-nant
			"SUBLU 3",		// sub-lunaire
			"SUBO 3",		// sub-ordonner
			"SUBRO 3",		// sub-rogateur
			"SUBUR 3",		// sub-urbain
			"SUR 3",		// sur-élever
			"SUPER 25",		// su-per-struc-tu-re

			"TRANS 5",		// trans-atlantique
			"TECH 4",		// tech-nique
			"TELE 24",		// télé-scope
		};

		// Dictionnaire de tous les débuts de mots à ne pas couper
		// (exceptions très spéciales) :
		static protected string[] tableSpecialBreak =
		{
			"HYMNE 3",		// hym-ne
		};


		static protected string RemoveAccent(string s)
		{
			//	Retourne la même chaîne sans accent (é -> e).
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
			//	Retourne le même caractère sans accent (é -> e).
			//	TODO: traiter tous les accents unicode ?
			char lower = System.Char.ToLowerInvariant(c);
			char cc = lower;

			switch (cc)
			{
				case 'á':
				case 'à':
				case 'â':
				case 'ä':
				case 'ã':
					cc = 'a';
					break;

				case 'ç':
					cc = 'c';
					break;

				case 'é':
				case 'è':
				case 'ê':
				case 'ë':
					cc = 'e';
					break;

				case 'í':
				case 'ì':
				case 'î':
				case 'ï':
					cc = 'i';
					break;

				case 'ñ':
					cc = 'n';
					break;

				case 'ó':
				case 'ò':
				case 'ô':
				case 'ö':
				case 'õ':
					cc = 'o';
					break;

				case 'ú':
				case 'ù':
				case 'û':
				case 'ü':
					cc = 'u';
					break;
			}

			if (lower != c)  // a-t-on utilisé une majuscule transformée en minuscule ?
			{
				cc = System.Char.ToUpperInvariant(cc);  // remet en majuscule
			}

			return cc;
		}

		static protected bool IsVowel(char letter)
		{
			// Test si le caractère est une voyelle, c'est-à-dire :
			//  "A" , "E" , "I" , "O" , "U"  ou  "Y"
			// La lettre "X" suit les mêmes règles que la lettre "Y".
			// De plus, une voyelle peut être un apostrophe pour résoudre
			// les cas du genre  "l'écran"  "qu'il"  "d'apprendre"  etc.
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

				// Ignore les caractères à la fin qui ne sont pas des lettres,
				// pour résoudre les cas du type:  (commencé-e)
				for (int i=this.length-1; i>=0; i--)
				{
					char letter = this.GetChar(i);

					if (letter >= 'A' && letter <= 'Z')
					{
						break;
					}

					this.length--;  // ignore un caractère à la fin
				}

				// Avance dans le mot jusqu'à ce qu'une lettre soit rencontrée.
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

				// Test si la 2ème ou 3ème lettre du mot est un apostrophe,
				// pour résoudre les cas du genre:  l'extra-ordinaire  qu'elles
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
				//	Offset du début du mot (par exemple 2 avec "l'aérospatial").
				get
				{
					return this.start;
				}
			}

			public int Length
			{
				//	Longueur du mot, après Start (par exemple 11 avec "l'aérospatial").
				get
				{
					return this.length;
				}
			}

			public int Position
			{
				//	Position dans le mot, après Start (par exemple 1 avec "l'a|érospatial).
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
				//	Longueur de la racine, après Start (par exemple 4 avec "l'aérospatial").
				get
				{
					return this.root;
				}
			}

			public char GetChar(int index)
			{
				// Donne un caractère. Si index=0, c'est le caractère sous
				// le pointeur qui est rendu. La valeur d'index peut être
				// positive ou négative.
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
				// Test s'il y a deux mêmes lettres qui se suivent.
				// Reste moins de 2 lettres ?
				if (this.length-this.position < 2)
				{
					return false;
				}

				// Deux lettres différentes ?
				if (this.GetChar(0) != this.GetChar(1))
				{
					return false;
				}

				// Vérifie si la césure est dans une terminaison "féminin pluriel",
				// c'est-à-dire "ées", pour éviter de couper "commencé-es" !

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
				// Cherche s'il s'agit d'une syllabe normale, c'est-à-dire:
				//  - une ou plusieurs consonnes quelconques (voir *)
				//  - une ou plusieurs voyelles quelconques (voir *)
				//  - éventuellement quelques consonnes (jusqu'au début de la
				//    syllabe suivante)
				// (*) Considère qu'il s'agit d'une syllabe lorsque deux mêmes
				// lettres sont trouvées.

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
					// Début de syllabe ?
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
					this.root = list[list.Count-1]-this.start;  // offset préposition
					this.position = this.root;
					return;
				}
			}

			protected string				text;
			protected int					start;  // début effectif du mot
			protected int					length;  // longueur du texte
			protected int					position;  // position du pointeur
			protected int					banned;  // position césure interdite
			protected int					root;  // longueur préposition
		}

	}
}
