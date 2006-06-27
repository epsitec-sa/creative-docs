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
			this.list = new List<GridSelectionItem>();
		}

		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public GridSelectionItem Get(int rank)
		{
			return this.list[rank];
		}

		public void Clear()
		{
			this.list.Clear();
		}

		public void Add(GridSelectionItem item)
		{
			this.list.Add(item);
		}

		public void Remove(GridSelectionItem item)
		{
			this.list.Remove(item);
		}

		public void RemoveAt(int rank)
		{
			this.list.RemoveAt(rank);
		}


		#region GridSelectionItem
		public enum GridSelectionType
		{
			Cell,
			Column,
			Row,
		}

		public class GridSelectionItem
		{
			public GridSelectionItem(GridSelectionType type, int rank, bool selected)
			{
				this.type = type;
				this.rank = rank;
				this.selected = selected;
			}

			public GridSelectionType Type
			{
				get
				{
					return this.type;
				}
				set
				{
					this.type = value;
				}
			}

			public int Rank
			{
				get
				{
					return this.rank;
				}
				set
				{
					this.rank = value;
				}
			}

			public bool Selected
			{
				get
				{
					return this.selected;
				}
				set
				{
					this.selected = value;
				}
			}

			protected GridSelectionType		type;
			protected int					rank;
			protected bool					selected;
		}
		#endregion


		#region Static methods
		public static void Attach(Widget obj)
		{
			GridSelection gs = obj.GetValue(GridSelection.GridSelectionProperty) as GridSelection;
			if (gs == null)
			{
				gs = new GridSelection(obj);
				obj.SetValue(GridSelection.GridSelectionProperty, gs);
			}
		}

		public static void Detach(Widget obj)
		{
			obj.ClearValue(GridSelection.GridSelectionProperty);
		}

		public static GridSelection Get(Widget obj)
		{
			return obj.GetValue(GridSelection.GridSelectionProperty) as GridSelection;
		}
		#endregion


		protected static readonly DependencyProperty GridSelectionProperty = DependencyProperty.RegisterAttached("GridSelection", typeof(GridSelection), typeof(GridSelection));

		protected Widget					obj;
		protected List<GridSelectionItem>	list;
	}
}
