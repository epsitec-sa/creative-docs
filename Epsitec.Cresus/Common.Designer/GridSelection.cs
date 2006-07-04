using System.Collections.Generic;

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// Contient les informations sur la sélection des lignes/colonnes d'un AbstractGroup
	/// en mode ChildrenPlacement.Grid.
	/// </summary>
	public class GridSelection : DependencyObject, IEnumerable<GridSelection.OneItem>
	{
		public GridSelection(Widget obj)
		{
			this.obj = obj;
			this.list = new List<OneItem>();
		}


		#region IEnumerable
		public IEnumerator<GridSelection.OneItem> GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.list.GetEnumerator();
		}
		#endregion


		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public OneItem this[int index]
		{
			get
			{
				return this.list[index];
			}
		}

		public void Clear()
		{
			this.list.Clear();
		}

		public void Add(Unit unit, int index)
		{
			this.list.Add(new OneItem(unit, index));
			this.list.Sort();
		}

		public void Add(OneItem item)
		{
			this.list.Add(item);
			this.list.Sort();
		}

		public void Remove(OneItem item)
		{
			this.list.Remove(item);
		}

		public void RemoveAt(int index)
		{
			this.list.RemoveAt(index);
		}

		public static bool EqualValues(GridSelection a, GridSelection b)
		{
			//	Compare si deux instances de GridSelection sont identiques.
			if (a == null && b == null)
			{
				return true;
			}

			if (a == null || b == null)
			{
				return false;
			}

			if (a.Count != b.Count)
			{
				return false;
			}

			for (int i=0; i<a.Count; i++)
			{
				if (!OneItem.EqualValues(a[i], b[i]))
				{
					return false;
				}
			}

			return true;
		}

		public bool AreOnlyColumns
		{
			//	Indique s'il n'y a que des sélections de colonnes.
			get
			{
				foreach (OneItem item in this)
				{
					if (item.Unit == Unit.Row)
					{
						return false;
					}
				}
				return true;
			}
		}

		public bool AreOnlyRows
		{
			//	Indique s'il n'y a que des sélections de lignes.
			get
			{
				foreach (OneItem item in this)
				{
					if (item.Unit == Unit.Column)
					{
						return false;
					}
				}
				return true;
			}
		}

		public bool AreMix
		{
			//	Indique s'il y a au moins une ligne et une colonne.
			get
			{
				bool c = false;
				bool r = false;

				foreach (OneItem item in this)
				{
					if (item.Unit == Unit.Column)
					{
						c = true;
					}

					if (item.Unit == Unit.Row)
					{
						r = true;
					}
				}

				return (c && r);
			}
		}

		public void InvertColumnSelection(int column)
		{
			//	Inverse une sélection de colonne.
			for (int i=0; i<this.Count; i++)
			{
				OneItem item = this[i];

				if (item.Unit == Unit.Column && item.Index == column)
				{
					this.RemoveAt(i);
					return;
				}
			}

			this.Add(Unit.Column, column);
		}

		public void InvertRowSelection(int row)
		{
			//	Inverse une sélection de ligne.
			for (int i=0; i<this.Count; i++)
			{
				OneItem item = this[i];

				if (item.Unit == Unit.Row && item.Index == row)
				{
					this.RemoveAt(i);
					return;
				}
			}

			this.Add(Unit.Row, row);
		}

		public void SelectPart(int column1, int column2, int row1, int row2)
		{
			//	Sélectionne quelques lignes et colonnes.
			this.Clear();

			int c1 = System.Math.Min(column1, column2);
			int c2 = System.Math.Max(column1, column2);

			int r1 = System.Math.Min(row1, row2);
			int r2 = System.Math.Max(row1, row2);

			for (int c=c1; c<=c2; c++)
			{
				this.Add(Unit.Column, c);
			}

			for (int r=r1; r<=r2; r++)
			{
				this.Add(Unit.Row, r);
			}
		}


		#region OneItem
		public enum Unit
		{
			Column,
			Row,
		}

		public class OneItem : System.IComparable
		{
			public OneItem(Unit unit, int index)
			{
				this.unit = unit;
				this.index = index;
			}

			public Unit Unit
			{
				//	Unité de la sélection.
				get
				{
					return this.unit;
				}
				set
				{
					this.unit = value;
				}
			}

			public int Index
			{
				//	Index de la cellule, ligne ou colonne.
				//	Pour une cellule, il s'agit du rang depuis la cellule supérieure gauche.
				get
				{
					return this.index;
				}
				set
				{
					this.index = value;
				}
			}

			public static bool EqualValues(OneItem a, OneItem b)
			{
				//	Compare si deux instances de OneItem sont identiques.
				if (a == null && b == null)
				{
					return true;
				}

				if (a == null || b == null)
				{
					return false;
				}

				return (a.unit == b.unit && a.index == b.index);
			}


			#region IComparable Members
			public int CompareTo(object obj)
			{
				//	D'abord les colonnes par index croissants, puis les lignes.
				OneItem that = obj as OneItem;

				if (this.unit != that.unit)
				{
					return (this.unit == Unit.Column) ? -1 : 1;
				}

				return this.index.CompareTo(that.index);
			}
			#endregion

			
			protected Unit				unit;
			protected int				index;
		}
		#endregion


		#region Static methods
		public static void Attach(Widget obj, GridSelection gs)
		{
			//	Attache un GridSelection au widget.
			obj.SetValue(GridSelection.GridSelectionProperty, gs);
		}

		public static void Attach(Widget obj)
		{
			//	Attache un GridSelection au widget, si nécessaire.
			GridSelection gs = obj.GetValue(GridSelection.GridSelectionProperty) as GridSelection;
			if (gs == null)
			{
				gs = new GridSelection(obj);
				obj.SetValue(GridSelection.GridSelectionProperty, gs);
			}
		}

		public static void Detach(Widget obj)
		{
			//	Détache l'éventuel GridSelection du widget.
			obj.ClearValue(GridSelection.GridSelectionProperty);
		}

		public static GridSelection Get(Widget obj)
		{
			//	Retourne le GridSelection attaché au widget.
			return obj.GetValue(GridSelection.GridSelectionProperty) as GridSelection;
		}
		#endregion


		public static readonly int Invalid = int.MinValue;

		protected static readonly DependencyProperty GridSelectionProperty = DependencyProperty.RegisterAttached("GridSelection", typeof(GridSelection), typeof(GridSelection), new DependencyPropertyMetadata().MakeNotSerializable());

		protected Widget					obj;
		protected List<OneItem>				list;
	}
}
