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
			AtBeginning            = 0x00000004,
			Jocker                 = 0x00000008,
			Reverse                = 0x00000010,

			SearchInLabel          = 0x00000100,
			SearchInPrimaryText    = 0x00000200,
			SearchInSecondaryText  = 0x00000400,
			SearchInPrimaryAbout   = 0x00000800,
			SearchInSecondaryAbout = 0x00001000,
		}


		public Searcher(List<Druid> druidsIndex, ResourceBundle primaryBundle, ResourceBundle secondaryBundle)
		{
			this.druidsIndex = druidsIndex;
			this.primaryBundle = primaryBundle;
			this.secondaryBundle = secondaryBundle;
			this.mode = SearchingMode.SearchInPrimaryText | SearchingMode.SearchInSecondaryText;
		}

		public void FixStarting(SearchingMode mode, int row, AbstractTextField edit, bool lastActionIsReplace)
		{
			//	Fixe la position de d�part de la recherche.
			this.mode = mode;
			this.starting = new Cursor();
			this.current  = new Cursor();

			if (row == -1)
			{
				if ((this.mode&SearchingMode.Reverse) == 0)  // en avant ?
				{
					this.starting.Row   = 0;  // au d�but
					this.starting.Field = 0;
					this.starting.Index = -1;
				}
				else  // en arri�re ?
				{
					this.starting.Row   = this.druidsIndex.Count-1;  // � la fin
					this.starting.Field = 4;
					this.starting.Index = 1000000;
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
						if (lastActionIsReplace)  // saute la s�lection ?
						{
							this.starting.Index = edit.TextLayout.FindOffsetFromIndex(System.Math.Max(edit.CursorFrom, edit.CursorTo), false);
						}
						else
						{
							this.starting.Index = edit.TextLayout.FindOffsetFromIndex(System.Math.Min(edit.CursorFrom, edit.CursorTo), true);
						}
					}
					else  // en arri�re ?
					{
						if (lastActionIsReplace)  // saute la s�leciton ?
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
					this.current.Index++;  // commence par avancer d'un caract�re
				}
				else  // en arri�re ?
				{
					this.current.Index--;  // commence par reculer d'un caract�re
				}

				if (this.Find())  return true;
			}
			while (this.MoveCurrentCursor());

			return false;
		}

		public int Count(string searching)
		{
			//	Effectue le d�compte.
			this.InitSearching(searching);

			this.mode &= ~SearchingMode.Reverse;
			this.starting.Row   = 0;  // au d�but
			this.starting.Field = 0;
			this.starting.Index = -1;
			this.starting.CopyTo(this.current);

			int count = 0;
			do
			{
				while (true)
				{
					this.current.Index++;  // commence par avancer d'un caract�re
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
			//	Effectue la recherche pour la substitution, mais pas la substitution elle-m�me.
			this.InitSearching(searching);

			if (fromBeginning)
			{
				this.mode &= ~SearchingMode.Reverse;
				this.starting.Row   = 0;  // au d�but
				this.starting.Field = 0;
				this.starting.Index = -1;
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
					this.current.Index++;  // commence par avancer d'un caract�re
				}
				else  // en arri�re ?
				{
					this.current.Index--;  // commence par reculer d'un caract�re
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
			//	Saute les caract�res s�lectionn�s, apr�s un Replace, pendant le ReplaceAll.
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
			//	Champ �ditable atteint (0..4).
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
			//	Longueur de la cha�ne trouv�e.
			get
			{
				return this.searching.Length;
			}
		}


		protected void InitSearching(string searching)
		{
			this.searching = searching;

			if ((this.mode&SearchingMode.CaseSensitive) == 0)
			{
				this.searching = Searcher.RemoveAccent(this.searching.ToLower());
			}
		}

		protected bool Find()
		{
			//	Cherche si la cha�ne est trouv� � la position du curseur.
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
							return true;  // trouv�
						}
					}
				}
				else  // en arri�re ?
				{
					this.current.Index = System.Math.Min(this.current.Index, text.Length);

					if (this.current.Index >= this.searching.Length)
					{
						int index = Searcher.IndexOf(text, this.searching, this.current.Index-1, this.mode);
						if (index != -1)
						{
							this.current.Index = index;
							return true;  // trouv�
						}
					}
				}
			}
			return false;
		}

		protected bool MoveCurrentCursor()
		{
			//	Avance le curseur sur le row/field/index suivant.
			//	Retourne false si on atteint de nouveau le d�but (et donc que la recherche est termin�e).
			if ((this.mode&SearchingMode.Reverse) == 0)  // en avant ?
			{
				this.current.Index = -1;
				this.current.Field++;
				if (this.current.Field >= 5)
				{
					this.current.Field = 0;
					this.current.Row ++;
					if (this.current.Row >= this.druidsIndex.Count)
					{
						this.current.Row = 0;
						this.limitOverflow = true;
					}
				}
				return (!this.limitOverflow || Cursor.Compare(this.current, this.starting) < 0);
			}
			else  // en arri�re ?
			{
				this.current.Index = 1000000;
				this.current.Field--;
				if (this.current.Field < 0)
				{
					this.current.Field = 4;
					this.current.Row--;
					if (this.current.Row < 0)
					{
						this.current.Row = this.druidsIndex.Count-1;
						this.limitOverflow = true;
					}
				}
				return (!this.limitOverflow || Cursor.Compare(this.current, this.starting) > 0);
			}
		}

		protected string ResourceText
		{
			//	Retourne le texte � la position du curseur courant (en fonction de row/field).
			get
			{
				Druid druid = this.druidsIndex[this.current.Row];

				if (this.current.Field == 0 && (this.mode&SearchingMode.SearchInLabel) != 0)
				{
					return this.primaryBundle[druid].Name;
				}

				if (this.current.Field == 1 && (this.mode&SearchingMode.SearchInPrimaryText) != 0)
				{
					return this.primaryBundle[druid].AsString;
				}

				if (this.current.Field == 2 && (this.mode&SearchingMode.SearchInSecondaryText) != 0 && this.secondaryBundle != null)
				{
					return this.secondaryBundle[druid].AsString;
				}

				if (this.current.Field == 3 && (this.mode&SearchingMode.SearchInPrimaryAbout) != 0)
				{
					return this.primaryBundle[druid].About;
				}

				if (this.current.Field == 4 && (this.mode&SearchingMode.SearchInSecondaryAbout) != 0 && this.secondaryBundle != null)
				{
					return this.secondaryBundle[druid].About;
				}

				return null;
			}
		}


		#region IndexOf
		static public int IndexOf(string text, string value, int startIndex, SearchingMode mode)
		{
			//	Cherche l'index de 'value' dans 'text'.
			int count;
			if ((mode&SearchingMode.Reverse) != 0)  // en arri�re ?
			{
				count = startIndex+1;
			}
			else	// en avant ?
			{
				count = text.Length-startIndex;
			}

			return Searcher.IndexOf(text, value, startIndex, count, mode);
		}

		static protected int IndexOf(string text, string value, int startIndex, int count, SearchingMode mode)
		{
			//	Cherche l'index de 'value' dans 'text' (un peu comme string.IndexOf), mais avec quelques
			//	options suppl�mentaires.
			//	Lorsqu'on recule (SearchingMode.Reverse), 'startIndex' est � la fin (sur le premier
			//	caract�re cherch�) et 'count' est positif (mais compte de droite � gauche).
			//	Cette fa�on absurde de proc�der est celle de string.LastIndexOf !
			if ((mode&SearchingMode.Reverse) != 0)  // en arri�re ?
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
				if ((mode&SearchingMode.Reverse) != 0)  // en arri�re ?
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
				if ((mode&SearchingMode.Reverse) != 0)  // en arri�re ?
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
			//	V�rifie si un mot et pr�c�d� et suivi d'un caract�re s�parateur de mots.
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
			//	Retourne la m�me cha�ne sans accent (� -> e).
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
			//	Retourne le m�me caract�re sans accent (� -> e).
			//	TODO: traiter tous les accents unicode ?
			char lower = System.Char.ToLower(c);
			char cc = lower;

			switch ( cc )
			{
				case '�':
				case '�':
				case '�':
				case '�':
				case '�':  cc = 'a';  break;

				case '�':  cc = 'c';  break;

				case '�':
				case '�':
				case '�':
				case '�':  cc = 'e';  break;

				case '�':
				case '�':
				case '�':
				case '�':  cc = 'i';  break;

				case '�':  cc = 'n';  break;

				case '�':
				case '�':
				case '�':
				case '�':
				case '�':  cc = 'o';  break;

				case '�':
				case '�':
				case '�':
				case '�':  cc = 'u';  break;
			}

			if ( lower != c )  // a-t-on utilis� une majuscule transform�e en minuscule ?
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


		protected List<Druid>			druidsIndex;
		protected ResourceBundle		primaryBundle;
		protected ResourceBundle		secondaryBundle;
		protected SearchingMode			mode;
		protected string				searching;
		protected Cursor				starting;
		protected Cursor				current;
		protected bool					limitOverflow;
	}
}
