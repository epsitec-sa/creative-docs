using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer
{
	public class Searcher
	{
		[System.Flags] public enum SearchingMode
		{
			None          = 0x00000000,
			CaseSensitive = 0x00000001,
			WholeWord     = 0x00000002,
			AtBeginning   = 0x00000004,
			AtEnding      = 0x00000008,
			Joker         = 0x00000010,
			Reverse       = 0x00000020,
		}


		public Searcher(ResourceAccess access)
		{
			this.access = access;
			this.mode = SearchingMode.None;
			this.filter = new List<int>();
		}

		public void FixStarting(SearchingMode mode, List<int> filter, int row, int field, int subfield, AbstractTextField edit, string secondaryCulture, bool lastActionIsReplace)
		{
			//	Fixe la position de départ de la recherche.
			this.mode = mode;
			this.filter = filter;
			this.secondaryCulture = secondaryCulture;
			this.starting = new Cursor();
			this.current  = new Cursor();

			if (row == -1)
			{
				if ((this.mode&SearchingMode.Reverse) == 0)  // en avant ?
				{
					this.starting.Row      = 0;  // au début
					this.starting.Field    = 0;
					this.starting.Subfield = 0;
					this.starting.Index    = -1;
				}
				else  // en arrière ?
				{
					this.starting.Row      = this.access.AccessCount-1;  // à la fin
					this.starting.Field    = 10-1;
					this.starting.Subfield = 10-1;
					this.starting.Index    = 1000000;
				}
			}
			else
			{
				this.starting.Row      = row;
				this.starting.Field    = field;
				this.starting.Subfield = subfield;
				this.starting.Index    = 0;

				if (edit != null)
				{
					if ((this.mode&SearchingMode.Reverse) == 0)  // en avant ?
					{
						if (lastActionIsReplace)  // saute la sélection ?
						{
							this.starting.Index = edit.TextLayout.FindOffsetFromIndex(System.Math.Max(edit.CursorFrom, edit.CursorTo), false);
						}
						else
						{
							this.starting.Index = edit.TextLayout.FindOffsetFromIndex(System.Math.Min(edit.CursorFrom, edit.CursorTo), true);
						}
					}
					else  // en arrière ?
					{
						if (lastActionIsReplace)  // saute la séleciton ?
						{
							this.starting.Index = edit.TextLayout.FindOffsetFromIndex(System.Math.Min(edit.CursorFrom, edit.CursorTo), true);
						}
						else
						{
							this.starting.Index = edit.TextLayout.FindOffsetFromIndex(System.Math.Max(edit.CursorFrom, edit.CursorTo), false);
						}
					}
				}
			}

			this.starting.CopyTo(this.current);
			this.limitOverflow = false;
		}

		public bool Search(string searching)
		{
			//	Effectue la recherche.
			this.InitSearching(searching);

			do
			{
				if ((this.mode&SearchingMode.Reverse) == 0)  // en avant ?
				{
					this.current.Index++;  // commence par avancer d'un caractère
				}
				else  // en arrière ?
				{
					this.current.Index--;  // commence par reculer d'un caractère
				}

				if (this.Find())  return true;
			}
			while (this.MoveCurrentCursor());

			return false;
		}

		public int Count(string searching)
		{
			//	Effectue le comptage.
			this.InitSearching(searching);

			this.mode &= ~SearchingMode.Reverse;
			this.starting.Row      = 0;  // au début
			this.starting.Field    = 0;
			this.starting.Subfield = 0;
			this.starting.Index    = -1;
			this.starting.CopyTo(this.current);

			int count = 0;
			do
			{
				while (true)
				{
					this.current.Index++;  // commence par avancer d'un caractère
					if (this.Find())
					{
						count++;
					}
					else
					{
						break;
					}
				}
			}
			while (this.MoveCurrentCursor());

			return count;
		}

		public bool Replace(string searching, bool fromBeginning)
		{
			//	Effectue la recherche pour la substitution, mais pas la substitution elle-même.
			this.InitSearching(searching);

			if (fromBeginning)
			{
				this.mode &= ~SearchingMode.Reverse;
				this.starting.Row      = 0;  // au début
				this.starting.Field    = 0;
				this.starting.Subfield = 0;
				this.starting.Index    = -1;
				this.starting.CopyTo(this.current);
			}

			if (this.Find())
			{
				return true;
			}

			do
			{
				if ((this.mode&SearchingMode.Reverse) == 0)  // en avant ?
				{
					this.current.Index++;  // commence par avancer d'un caractère
				}
				else  // en arrière ?
				{
					this.current.Index--;  // commence par reculer d'un caractère
				}

				if (this.Find())
				{
					return true;
				}
			}
			while (this.MoveCurrentCursor());

			return false;
		}

		public void Skip(int length)
		{
			//	Saute les caractères sélectionnés, après un Replace, pendant le ReplaceAll.
			this.current.Index += length;
		}

		public int Row
		{
			//	Ligne atteinte.
			get
			{
				return this.current.Row;
			}
		}

		public int Field
		{
			//	Champ éditable atteint.
			get
			{
				return this.current.Field;
			}
		}

		public int Subfield
		{
			//	Sous-champ éditable atteint.
			get
			{
				return this.current.Subfield;
			}
		}

		public int Index
		{
			//	Index atteint dans le texte.
			get
			{
				return this.current.Index;
			}
		}

		public int Length
		{
			//	Longueur de la chaîne trouvée.
			get
			{
				return this.searching.Length;
			}
		}


		private void InitSearching(string searching)
		{
			this.searching = searching;

			if ((this.mode&SearchingMode.CaseSensitive) == 0)
			{
				this.searching = Searcher.RemoveAccent(this.searching.ToLower());
			}
		}

		private bool Find()
		{
			//	Cherche si la chaîne est trouvé à la position du curseur.
			string text = this.ResourceText;
			if (text != null)
			{
				if ((this.mode&SearchingMode.Reverse) == 0)  // en avant ?
				{
					if (this.current.Index <= text.Length-this.searching.Length)
					{
						int index = Searcher.IndexOf(text, this.searching, this.current.Index, this.mode);
						if (index != -1)
						{
							this.current.Index = index;
							return true;  // trouvé
						}
					}
				}
				else  // en arrière ?
				{
					this.current.Index = System.Math.Min(this.current.Index, text.Length);

					if (this.current.Index >= this.searching.Length)
					{
						int index = Searcher.IndexOf(text, this.searching, this.current.Index-1, this.mode);
						if (index != -1)
						{
							this.current.Index = index;
							return true;  // trouvé
						}
					}
				}
			}
			return false;
		}

		private bool MoveCurrentCursor()
		{
			//	Avance le curseur sur le row/field/index suivant.
			//	Retourne false si on atteint de nouveau le début (et donc que la recherche est terminée).
			if ((this.mode&SearchingMode.Reverse) == 0)  // en avant ?
			{
				this.current.Index = -1;
				this.current.Subfield++;
				if (this.current.Subfield >= 10)
				{
					this.current.Subfield = 0;
					this.current.Field++;
					if (this.current.Field >= 10)
					{
						this.current.Field = 0;
						this.current.Row++;
						if (this.current.Row >= this.access.AccessCount)
						{
							this.current.Row = 0;
							this.limitOverflow = true;
						}
					}
				}
				return (!this.limitOverflow || Cursor.Compare(this.current, this.starting) < 0);
			}
			else  // en arrière ?
			{
				this.current.Index = 1000000;
				this.current.Subfield--;
				if (this.current.Subfield < 0)
				{
					this.current.Subfield = 10-1;
					this.current.Field--;
					if (this.current.Field < 0)
					{
						this.current.Field = 10-1;
						this.current.Row--;
						if (this.current.Row < 0)
						{
							this.current.Row = this.access.AccessCount-1;
							this.limitOverflow = true;
						}
					}
				}
				return (!this.limitOverflow || Cursor.Compare(this.current, this.starting) > 0);
			}
		}

		private string ResourceText
		{
			//	Retourne le texte à la position du curseur courant (en fonction de row/field/subfield).
			get
			{
				if (!this.filter.Contains(this.current.Field))
				{
					return null;
				}

				string cultureName;
				ResourceAccess.FieldType fieldType;
				this.access.SearcherIndexToAccess(this.current.Field, this.secondaryCulture, out cultureName, out fieldType);

				ResourceAccess.Field field = this.access.GetField(this.current.Row, cultureName, fieldType);
				if (field == null)
				{
					return null;
				}

				if (field.FieldType == ResourceAccess.Field.Type.String)
				{
					if (this.current.Subfield == 0)
					{
						return field.String;
					}
				}

				if (field.FieldType == ResourceAccess.Field.Type.StringCollection)
				{
					if (this.current.Subfield < field.StringCollection.Count)
					{
						string[] array = new string[field.StringCollection.Count];
						field.StringCollection.CopyTo(array, 0);
						return array[this.current.Subfield];
					}
				}

				return null;
			}
		}


		#region IndexOf
		static public int IndexOf(string text, string value, int startIndex, SearchingMode mode)
		{
			//	Cherche l'index de 'value' dans 'text'.
			int count;
			if ((mode&SearchingMode.Reverse) != 0)  // en arrière ?
			{
				count = startIndex+1;
			}
			else	// en avant ?
			{
				count = text.Length-startIndex;
			}

			return Searcher.IndexOf(text, value, startIndex, count, mode);
		}

		static private int IndexOf(string text, string value, int startIndex, int count, SearchingMode mode)
		{
			//	Cherche l'index de 'value' dans 'text' (un peu comme string.IndexOf), mais avec quelques
			//	options supplémentaires.
			//	Lorsqu'on recule (SearchingMode.Reverse), 'startIndex' est à la fin (sur le premier
			//	caractère cherché) et 'count' est positif (mais compte de droite à gauche).
			//	Cette façon absurde de procéder est celle de string.LastIndexOf !
			if ((mode&SearchingMode.Reverse) != 0)  // en arrière ?
			{
				startIndex = System.Math.Min(startIndex, text.Length);
				count = System.Math.Min(count, startIndex+1);
			}
			else	// en avant ?
			{
				startIndex = System.Math.Max(startIndex, 0);
				count = System.Math.Min(count, text.Length-startIndex);
			}

			if (count <= 0 || text.Length < value.Length)
			{
				return -1;
			}

			if ((mode&SearchingMode.CaseSensitive) == 0)
			{
				text = Searcher.RemoveAccent(text.ToLower());
			}

			if ((mode&SearchingMode.WholeWord) != 0)  // mot entier ?
			{
				if ((mode&SearchingMode.Reverse) != 0)  // en arrière ?
				{
					int begin = startIndex-count+1;
					while (true)
					{
						startIndex = text.LastIndexOf(value, startIndex, count);
						if (startIndex == -1)  return -1;
						if (Searcher.IsWholeWord(text, startIndex, value.Length))  return startIndex;
						startIndex--;
						count = startIndex-begin+1;
						if (startIndex < 0)  return -1;
					}
				}
				else	// en avant ?
				{
					int length = startIndex+count;
					while (true)
					{
						startIndex = text.IndexOf(value, startIndex, count);
						if (startIndex == -1)  return -1;
						if (Searcher.IsWholeWord(text, startIndex, value.Length))  return startIndex;
						startIndex++;
						count = length-startIndex;
						if (count <= 0)  return -1;
					}
				}
			}
			else
			{
				if ((mode&SearchingMode.Reverse) != 0)  // en arrière ?
				{
					return text.LastIndexOf(value, startIndex, count);
				}
				else	// en avant ?
				{
					return text.IndexOf(value, startIndex, count);
				}
			}
		}

		static private bool IsWholeWord(string text, int index, int count)
		{
			//	Vérifie si un mot et précédé et suivi d'un caractère séparateur de mots.
			if (index > 0)
			{
				char c1 = text[index-1];
				char c2 = text[index];
				if (!Text.Unicode.IsWordStart(c2, c1))  return false;
			}

			if (index+count < text.Length)
			{
				char c1 = text[index+count-1];
				char c2 = text[index+count];
				if (!Text.Unicode.IsWordEnd(c2, c1))  return false;
			}

			return true;
		}
		#endregion


		#region Accents
		static public string RemoveAccent(string s)
		{
			//	Retourne la même chaîne sans accent (é -> e).
			System.Text.StringBuilder builder;
				
			builder = new System.Text.StringBuilder(s.Length);
			for ( int i=0 ; i<s.Length ; i++ )
			{
				builder.Append(Searcher.RemoveAccent(s[i]));
			}
			return builder.ToString();
		}

		static private char RemoveAccent(char c)
		{
			//	Retourne le même caractère sans accent (é -> e).
			//	TODO: traiter tous les accents unicode ?
			char lower = System.Char.ToLower(c);
			char cc = lower;

			switch ( cc )
			{
				case 'á':
				case 'à':
				case 'â':
				case 'ä':
				case 'ã':  cc = 'a';  break;

				case 'ç':  cc = 'c';  break;

				case 'é':
				case 'è':
				case 'ê':
				case 'ë':  cc = 'e';  break;

				case 'í':
				case 'ì':
				case 'î':
				case 'ï':  cc = 'i';  break;

				case 'ñ':  cc = 'n';  break;

				case 'ó':
				case 'ò':
				case 'ô':
				case 'ö':
				case 'õ':  cc = 'o';  break;

				case 'ú':
				case 'ù':
				case 'û':
				case 'ü':  cc = 'u';  break;
			}

			if ( lower != c )  // a-t-on utilisé une majuscule transformée en minuscule ?
			{
				cc = System.Char.ToUpper(cc);  // remet en majuscule
			}

			return cc;
		}
		#endregion


		#region Cursor
		private class Cursor
		{
			public int Row
			{
				get
				{
					return this.row;
				}
				set
				{
					this.row = value;
				}
			}

			public int Field
			{
				get
				{
					return this.field;
				}
				set
				{
					this.field = value;
				}
			}

			public int Subfield
			{
				get
				{
					return this.subfield;
				}
				set
				{
					this.subfield = value;
				}
			}

			public int Index
			{
				get
				{
					return this.index;
				}
				set
				{
					this.index = value;
				}
			}

			public void CopyTo(Cursor dest)
			{
				dest.row      = this.row;
				dest.field    = this.field;
				dest.subfield = this.subfield;
				dest.index    = this.index;
			}

			static public int Compare(Cursor c1, Cursor c2)
			{
				if (c1.row < c2.row)  return -1;
				if (c1.row > c2.row)  return 1;

				if (c1.field < c2.field)  return -1;
				if (c1.field > c2.field)  return 1;

				if (c1.subfield < c2.subfield)  return -1;
				if (c1.subfield > c2.subfield)  return 1;

				if (c1.index < c2.index)  return -1;
				if (c1.index > c2.index)  return 1;

				return 0;
			}

			private int					row;
			private int					field;
			private int					subfield;
			private int					index;
		}
		#endregion


		private ResourceAccess				access;
		private SearchingMode				mode;
		private List<int>					filter;
		private string						secondaryCulture;
		private string						searching;
		private Cursor						starting;
		private Cursor						current;
		private bool						limitOverflow;
	}
}
