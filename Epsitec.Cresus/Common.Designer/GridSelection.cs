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
	public class GridSelection : DependencyObject
	{
		public GridSelection(Widget obj)
		{
			this.obj = obj;
			this.list = new List<Item>();
		}

		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public Item Get(int index)
		{
			return this.list[index];
		}

#if false
		public Item this[int index]
		{
			get
			{
				return this.list[index];
			}
		}
#endif

		public void Clear()
		{
			this.list.Clear();
		}

		public void Add(Item item)
		{
			this.list.Add(item);
		}

		public void Remove(Item item)
		{
			this.list.Remove(item);
		}

		public void RemoveAt(int index)
		{
			this.list.RemoveAt(index);
		}


		#region Item
		public enum Unit
		{
			Cell,
			Column,
			Row,
		}

		public class Item
		{
			public Item(Unit unit, int index)
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

			protected Unit					unit;
			protected int					index;
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
		protected List<Item>				list;
	}
}
