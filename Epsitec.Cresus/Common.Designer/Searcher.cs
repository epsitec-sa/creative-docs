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
			Reverse                = 0x00000002,
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
				if ((this.mode & SearchingMode.Reverse) == 0)  // en avant ?
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

					this.starting.Index = edit.Cursor;
				}
			}

			this.starting.CopyTo(this.current);
		}

		public bool Search(string searching)
		{
			//	Effectue la recherche.
			if ((this.mode & SearchingMode.CaseSensitive) == 0)
			{
				searching = searching.ToLower();
			}

			while (true)
			{
				if (!this.MoveCurrentCursor())
				{
					return false;
				}

				string text = this.GetText;
				if (text == null)
				{
					continue;
				}

				int index = text.IndexOf(searching);
				if (index != -1)
				{
					this.current.Index = index;
					return true;
				}
			}
		}

		public int Row
		{
			get
			{
				return this.current.Row;
			}
		}

		public int Field
		{
			get
			{
				return this.current.Field;
			}
		}

		public int Index
		{
			get
			{
				return this.current.Index;
			}
		}


		protected bool MoveCurrentCursor()
		{
			//	Avance le curseur sur le row/field suivant.
			if ((this.mode & SearchingMode.Reverse) == 0)  // en avant ?
			{
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

			if (Cursor.Compare(this.current, this.starting) == 0)
			{
				return false;
			}

			return true;
		}

		protected string GetText
		{
			//	Retourne le texte à la position du curseur courant.
			get
			{
				string label = this.labelsIndex[this.current.Row];
				string text = null;

				if (this.current.Field == 0 && (this.mode & SearchingMode.SearchInLabel) != 0)
				{
					text = label;
				}

				if (this.current.Field == 1 && (this.mode & SearchingMode.SearchInPrimaryText) != 0)
				{
					text = this.primaryBundle[label].AsString;
				}

				if (this.current.Field == 2 && (this.mode & SearchingMode.SearchInSecondaryText) != 0)
				{
					text = this.secondaryBundle[label].AsString;
				}

				if (this.current.Field == 3 && (this.mode & SearchingMode.SearchInPrimaryAbout) != 0)
				{
					text = this.primaryBundle[label].About;
				}

				if (this.current.Field == 4 && (this.mode & SearchingMode.SearchInSecondaryAbout) != 0)
				{
					text = this.secondaryBundle[label].About;
				}

				if ((this.mode & SearchingMode.CaseSensitive) == 0 && text != null)
				{
					text = text.ToLower();
				}

				return text;
			}
		}


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
		protected Cursor				starting;
		protected Cursor				current;
	}
}
