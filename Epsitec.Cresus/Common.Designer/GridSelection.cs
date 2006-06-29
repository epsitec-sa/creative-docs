using System.Collections.Generic;

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// Contient les informations sur la s�lection des lignes/colonnes d'un AbstractGroup
	/// en mode ChildrenPlacement.Grid.
	/// </summary>
	public class GridSelection : DependencyObject
	{
		public enum SelectionUnit
		{
			None,
			Column,
			Row,
		}


		public GridSelection(Widget obj)
		{
			this.obj = obj;
			this.unit = SelectionUnit.None;
		}


		public void Clear()
		{
			this.unit = SelectionUnit.None;
		}

		public SelectionUnit Unit
		{
			//	Unit� de la s�lection.
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
			//	Pour une cellule, il s'agit du rang depuis la cellule sup�rieure gauche.
			get
			{
				return this.index;
			}
			set
			{
				this.index = value;
			}
		}


		#region Static methods
		public static void Attach(Widget obj)
		{
			//	Attache un GridSelection au widget, si n�cessaire.
			GridSelection gs = obj.GetValue(GridSelection.GridSelectionProperty) as GridSelection;
			if (gs == null)
			{
				gs = new GridSelection(obj);
				obj.SetValue(GridSelection.GridSelectionProperty, gs);
			}
		}

		public static void Detach(Widget obj)
		{
			//	D�tache l'�ventuel GridSelection du widget.
			obj.ClearValue(GridSelection.GridSelectionProperty);
		}

		public static GridSelection Get(Widget obj)
		{
			//	Retourne le GridSelection attach� au widget.
			return obj.GetValue(GridSelection.GridSelectionProperty) as GridSelection;
		}
		#endregion


		protected static readonly DependencyProperty GridSelectionProperty = DependencyProperty.RegisterAttached("GridSelection", typeof(GridSelection), typeof(GridSelection), new DependencyPropertyMetadata().MakeNotSerializable());

		protected Widget					obj;
		protected SelectionUnit				unit;
		protected int						index;
	}
}
