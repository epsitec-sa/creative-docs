using System.Collections.Generic;

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Proxies
{
	public class Grid : Abstract
	{
		public Grid(ProxyManager manager) : base (manager)
		{
		}

		public override int Rank
		{
			//	Retourne le rang de ce proxy parmi la liste de tous les proxies.
			//	Plus le numéro est petit, plus le proxy apparaîtra haut dans la
			//	liste.
			get
			{
				return 4;
			}
		}

		public override string IconName
		{
			get
			{
				return "PropertyGrid";
			}
		}

		public int GridColumnsCount
		{
			get
			{
				return (int) this.GetValue(Grid.GridColumnsCountProperty);
			}
			set
			{
				this.SetValue(Grid.GridColumnsCountProperty, value);
			}
		}

		public int GridRowsCount
		{
			get
			{
				return (int) this.GetValue (Grid.GridRowsCountProperty);
			}
			set
			{
				this.SetValue(Grid.GridRowsCountProperty, value);
			}
		}

		public int GridColumnSpan
		{
			get
			{
				return (int) this.GetValue(Grid.GridColumnSpanProperty);
			}
			set
			{
				this.SetValue(Grid.GridColumnSpanProperty, value);
			}
		}

		public int GridRowSpan
		{
			get
			{
				return (int) this.GetValue(Grid.GridRowSpanProperty);
			}
			set
			{
				this.SetValue(Grid.GridRowSpanProperty, value);
			}
		}


		protected override void InitialisePropertyValues()
		{
			//	Cette méthode est appelée par Proxies.Abstract quand on connecte
			//	le premier widget avec le proxy.
			
			//	Recopie localement les diverses propriétés du widget sélectionné
			//	pour pouvoir ensuite travailler dessus :
			if (this.ObjectModifier.AreChildrenGrid(this.DefaultWidget))
			{
				int columns = this.ObjectModifier.GetGridColumnsCount(this.DefaultWidget);
				int rows    = this.ObjectModifier.GetGridRowsCount(this.DefaultWidget);

				this.GridColumnsCount = columns;
				this.GridRowsCount    = rows;
			}

			if (this.ObjectModifier.AreChildrenGrid(this.DefaultWidget.Parent))
			{
				int columnSpan = this.ObjectModifier.GetGridColumnSpan(this.DefaultWidget);
				int rowSpan    = this.ObjectModifier.GetGridRowSpan(this.DefaultWidget);

				this.GridColumnSpan = columnSpan;
				this.GridRowSpan    = rowSpan;
			}
		}

		private static void NotifyGridColumnsCountChanged(DependencyObject o, object oldValue, object newValue)
		{
			//	Cette méthode est appelée à la suite de la modification d'une de nos propriétés
			//	de définition pour permettre de mettre à jour les widgets connectés.
			int value = (int) newValue;
			Grid that = (Grid) o;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						that.ObjectModifier.SetGridColumnsCount(obj, value);
					}
				}
				finally
				{
					that.ResumeChanges();
				}
			}
		}

		private static void NotifyGridRowsCountChanged(DependencyObject o, object oldValue, object newValue)
		{
			//	Cette méthode est appelée à la suite de la modification d'une de nos propriétés
			//	de définition pour permettre de mettre à jour les widgets connectés.
			int value = (int) newValue;
			Grid that = (Grid) o;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						that.ObjectModifier.SetGridRowsCount(obj, value);
					}
				}
				finally
				{
					that.ResumeChanges();
				}
			}
		}

		private static void NotifyGridColumnSpanChanged(DependencyObject o, object oldValue, object newValue)
		{
			//	Cette méthode est appelée à la suite de la modification d'une de nos propriétés
			//	de définition pour permettre de mettre à jour les widgets connectés.
			int value = (int) newValue;
			Grid that = (Grid) o;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						that.ObjectModifier.SetGridColumnSpan(obj, value);
					}
				}
				finally
				{
					that.ResumeChanges();
				}
			}
		}

		private static void NotifyGridRowSpanChanged(DependencyObject o, object oldValue, object newValue)
		{
			//	Cette méthode est appelée à la suite de la modification d'une de nos propriétés
			//	de définition pour permettre de mettre à jour les widgets connectés.
			int value = (int) newValue;
			Grid that = (Grid) o;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						that.ObjectModifier.SetGridRowSpan(obj, value);
					}
				}
				finally
				{
					that.ResumeChanges();
				}
			}
		}


		static Grid()
		{
			Grid.GridColumnsCountProperty.DefaultMetadata.DefineNamedType(ProxyManager.GridNumericType);
			Grid.GridRowsCountProperty.DefaultMetadata.DefineNamedType(ProxyManager.GridNumericType);
			Grid.GridColumnsCountProperty.DefaultMetadata.DefineCaptionId(new Support.Druid("[100J]").ToLong());
			Grid.GridRowsCountProperty.DefaultMetadata.DefineCaptionId(new Support.Druid("[100K]").ToLong());

			Grid.GridColumnSpanProperty.DefaultMetadata.DefineNamedType(ProxyManager.GridNumericType);
			Grid.GridRowSpanProperty.DefaultMetadata.DefineNamedType(ProxyManager.GridNumericType);
			Grid.GridColumnSpanProperty.DefaultMetadata.DefineCaptionId(new Support.Druid("[100L]").ToLong());
			Grid.GridRowSpanProperty.DefaultMetadata.DefineCaptionId(new Support.Druid("[100M]").ToLong());
		}


		public static readonly DependencyProperty GridColumnsCountProperty = DependencyProperty.Register("GridColumnsCount", typeof(int), typeof(Grid), new DependencyPropertyMetadata(2, Grid.NotifyGridColumnsCountChanged));
		public static readonly DependencyProperty GridRowsCountProperty	   = DependencyProperty.Register("GridRowsCount",    typeof(int), typeof(Grid), new DependencyPropertyMetadata(2, Grid.NotifyGridRowsCountChanged));

		public static readonly DependencyProperty GridColumnSpanProperty = DependencyProperty.Register("GridColumnSpan", typeof(int), typeof(Grid), new DependencyPropertyMetadata(1, Grid.NotifyGridColumnSpanChanged));
		public static readonly DependencyProperty GridRowSpanProperty	 = DependencyProperty.Register("GridRowSpan",    typeof(int), typeof(Grid), new DependencyPropertyMetadata(1, Grid.NotifyGridRowSpanChanged));
	}
}
