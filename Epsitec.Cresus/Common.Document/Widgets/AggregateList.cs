using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// AggregateList repr�sente la liste des styles graphiques.
	/// </summary>
	public class AggregateList : AbstractStyleList
	{
		public AggregateList() : base()
		{
		}


		public UndoableList List
		{
			//	Liste des aggr�gats repr�sent�s dans la liste.
			get
			{
				return this.list;
			}

			set
			{
				this.list = value;
			}
		}


		protected override int ListCount
		{
			//	Nombre le lignes de la liste.
			get
			{
				int count = 0;

				if ( this.list != null )
				{
					count = this.list.Count;;
				}

				if ( this.excludeRank != -1 )
				{
					count --;
				}

				if ( this.isNoneLine )
				{
					count ++;
				}

				return count;
			}
		}

		protected override int ListSelected
		{
			//	Ligne s�lectionn�e dans la liste.
			get
			{
				if ( this.list == null )
				{
					return -1;
				}
				else
				{
					return this.list.Selected;
				}
			}
		}

		protected override string ListName(int rank)
		{
			//	Nom d'une ligne de la liste.
			if ( rank == -1 || this.list == null )
			{
				return Res.Strings.Aggregates.NoneLine;
			}
			else
			{
				Properties.Aggregate agg = this.list[rank] as Properties.Aggregate;
				return agg.AggregateName;
			}
		}

		protected override string ListChildrensCount(int rank)
		{
			//	Nombre d'enfants d'une ligne de la liste.
			if ( rank != -1 && this.list != null )
			{
				Properties.Aggregate agg = this.list[rank] as Properties.Aggregate;
				int count = agg.Childrens.Count;
				if ( count != 0 )
				{
					return count.ToString();
				}
			}
			return "";
		}

		protected override AbstractSample CreateSample()
		{
			//	Cr�e un �chantillon.
			return new Sample();
		}

		protected override void ListSample(AbstractSample sample, int rank)
		{
			//	 Met � jour l'�chantillon d'une ligne de la liste.
			Sample sm = sample as Sample;

			if ( rank == -1 || this.list == null )
			{
				sm.Aggregate = null;
			}
			else
			{
				sm.Aggregate = this.list[rank] as Properties.Aggregate;
			}

			sm.Invalidate();
		}


		protected UndoableList					list;
	}
}
