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

		public void Add(OneItem item)
		{
			this.list.Add(item);
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


		#region OneItem
		public enum Unit
		{
			Column,
			Row,
		}

		public class OneItem
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
				//	Compare si deux instances de GridSelection sont identiques.
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


			protected Unit				unit;
			protected int				index;
		}
		#endregion


		#region Static methods
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


		protected static readonly DependencyProperty GridSelectionProperty = DependencyProperty.RegisterAttached("GridSelection", typeof(GridSelection), typeof(GridSelection), new DependencyPropertyMetadata().MakeNotSerializable());

		protected Widget					obj;
		protected List<OneItem>				list;
	}
}
