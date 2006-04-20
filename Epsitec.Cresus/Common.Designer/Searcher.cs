using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer
{
	public class Searcher
	{
		[System.Flags] public enum SearchingMode
		{
			None                   = 0x00000000,
			CaseSensitive          = 0x00000001,
			WholeWord              = 0x00000002,
			Reverse                = 0x00000004,

			SearchInLabel          = 0x00000010,
			SearchInPrimaryText    = 0x00000020,
			SearchInSecondaryText  = 0x00000040,
			SearchInPrimaryAbout   = 0x00000080,
			SearchInSecondaryAbout = 0x00000100,
		}

		public Searcher(List<string> labelsIndex, ResourceBundle primaryBundle, ResourceBundle secondaryBundle)
		{
			this.labelsIndex = labelsIndex;
			this.primaryBundle = primaryBundle;
			this.secondaryBundle = secondaryBundle;
			this.mode = SearchingMode.SearchInPrimaryText | SearchingMode.SearchInSecondaryText;
		}

		public void FixStarting(SearchingMode mode, int row, AbstractTextField edit)
		{
			//	Fixe la position de départ de la recherche.
			this.mode = mode;
			this.starting = new Cursor();
			this.current  = new Cursor();

			if (row == -1)
			{
				if ((this.mode&SearchingMode.Reverse) == 0)  // en avant ?
				{
					this.starting.Row   = this.labelsIndex.Count-1;  // à la fin
					this.starting.Field = 4;
					this.starting.Index = 1000000;
				}
				else  // en arrière ?
				{
					this.starting.Row   = 0;  // au début
					this.starting.Field = 0;
					this.starting.Index = 0;
				}
			}
			else
			{
				this.starting.Row = row;

				this.starting.Field = 0;
				this.starting.Index = 0;

				if (edit != null)
				{
					switch (edit.Name)
					{
						case "LabelEdit":       this.starting.Field = 0;  break;
						case "PrimaryEdit":     this.starting.Field = 1;  break;
						case "SecondaryEdit":   this.starting.Field = 2;  break;
						case "PrimaryAbout":    this.starting.Field = 3;  break;
						case "SecondaryAbout":  this.starting.Field = 4;  break;
					}

					if ((this.mode&SearchingMode.Reverse) == 0)  // en avant ?
					{
						this.starting.Index = edit.TextLayout.FindOffsetFromIndex(System.Math.Min(edit.CursorFrom, edit.CursorTo), true);
					}
					else  // en arrière ?
					{
						this.starting.Index = edit.TextLayout.FindOffsetFromIndex(System.Math.Max(edit.CursorFrom, edit.CursorTo), false);
					}
				}
			}

			this.starting.CopyTo(this.current);
		}

		public bool Search(string searching)
		{
			//	Effectue la recherche.
			this.searching = searching;

			if ((this.mode&SearchingMode.CaseSensitive) == 0)
			{
				searching = searching.ToLower();
			}

			do
			{
				string text = this.GetText;
				if (text != null)
				{
					if ((this.mode&SearchingMode.Reverse) == 0)  // en avant ?
					{
						this.current.Index ++;  // commence par avancer d'un caractère
						if (this.current.Index <= text.Length-searching.Length)
						{
							int index = Searcher.IndexOf(text, searching, this.current.Index, this.mode);
							if (index != -1)
							{
								this.current.Index = index;
								return true;  // trouvé dans le même champ
							}
						}
					}
					else  // en arrière ?
					{
						this.current.Index --;  // commence par reculer d'un caractère
						this.current.Index = System.Math.Min(this.current.Index, text.Length);

						if (this.current.Index >= searching.Length)
						{
							this.current.Index -= searching.Length;
							int index = Searcher.IndexOf(text, searching, this.current.Index, this.mode);
							if (index != -1)
							{
								this.current.Index = index;
								return true;  // trouvé dans le même champ
							}
						}
					}
				}
			}
			while (this.MoveCurrentCursor());

			return false;
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
			//	Champ éditable atteint (0..4).
			get
			{
				return this.current.Field;
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


		protected bool MoveCurrentCursor()
		{
			//	Avance le curseur sur le row/field/index suivant.
			//	Retourne false si on atteint de nouveau le début (et donc que la recherche est terminée).
			if ((this.mode&SearchingMode.Reverse) == 0)  // en avant ?
			{
				this.current.Index = -1;
				this.current.Field++;
				if (this.current.Field >= 5)
				{
					this.current.Field = 0;
					this.current.Row ++;
					if (this.current.Row >= this.labelsIndex.Count)
					{
						this.current.Row = 0;
					}
				}
			}
			else  // en arrière ?
			{
				this.current.Index = 1000000;
				this.current.Field--;
				if (this.current.Field < 0)
				{
					this.current.Field = 4;
					this.current.Row--;
					if (this.current.Row < 0)
					{
						this.current.Row = this.labelsIndex.Count-1;
					}
				}
			}

			return (Cursor.Compare(this.current, this.starting) != 0);
		}

		protected string GetText
		{
			//	Retourne le texte à la position du curseur courant (en fonction de row/field).
			get
			{
				string label = this.labelsIndex[this.current.Row];
				string text = null;

				if (this.current.Field == 0 && (this.mode&SearchingMode.SearchInLabel) != 0)
				{
					text = label;
				}

				if (this.current.Field == 1 && (this.mode&SearchingMode.SearchInPrimaryText) != 0)
				{
					text = this.primaryBundle[label].AsString;
				}

				if (this.current.Field == 2 && (this.mode&SearchingMode.SearchInSecondaryText) != 0)
				{
					text = this.secondaryBundle[label].AsString;
				}

				if (this.current.Field == 3 && (this.mode&SearchingMode.SearchInPrimaryAbout) != 0)
				{
					text = this.primaryBundle[label].About;
				}

				if (this.current.Field == 4 && (this.mode&SearchingMode.SearchInSecondaryAbout) != 0)
				{
					text = this.secondaryBundle[label].About;
				}

				if ((this.mode&SearchingMode.CaseSensitive) == 0 && text != null)
				{
					text = text.ToLower();
				}

				return text;
			}
		}


		#region InfexOf
		static public int IndexOf(string text, string value, int startIndex, SearchingMode mode)
		{
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

		static public int IndexOf(string text, string value, int startIndex, int count, SearchingMode mode)
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

			if ((mode&SearchingMode.CaseSensitive) == 0)  // é = e ?
			{
				text = Searcher.RemoveAccent(text);
				value = Searcher.RemoveAccent(value);
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

		static protected bool IsWholeWord(string text, int index, int count)
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
		static protected string RemoveAccent(string s)
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

		static protected char RemoveAccent(char c)
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
		protected class Cursor
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
				dest.row   = this.row;
				dest.field = this.field;
				dest.index = this.index;
			}

			static public int Compare(Cursor c1, Cursor c2)
			{
				if (c1.row < c2.row)  return -1;
				if (c1.row > c2.row)  return 1;

				if (c1.field < c2.field)  return -1;
				if (c1.field > c2.field)  return 1;

				if (c1.index < c2.index)  return -1;
				if (c1.index > c2.index)  return 1;

				return 0;
			}

			protected int					row;
			protected int					field;
			protected int					index;
		}
		#endregion


		protected List<string>			labelsIndex;
		protected ResourceBundle		primaryBundle;
		protected ResourceBundle		secondaryBundle;
		protected SearchingMode			mode;
		protected string				searching;
		protected Cursor				starting;
		protected Cursor				current;
	}
}
