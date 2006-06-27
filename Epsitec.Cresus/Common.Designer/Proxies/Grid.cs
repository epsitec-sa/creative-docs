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

		public double GridColumnsCount
		{
			get
			{
				return (double) this.GetValue(Grid.GridColumnsCountProperty);
			}
			set
			{
				this.SetValue(Grid.GridColumnsCountProperty, value);
			}
		}

		public double GridRowsCount
		{
			get
			{
				return (double) this.GetValue(Grid.GridRowsCountProperty);
			}
			set
			{
				this.SetValue(Grid.GridRowsCountProperty, value);
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

				this.GridColumnsCount = (double) columns;
				this.GridRowsCount    = (double) rows;
			}
		}

		private void NotifyColumnsCountChanged(double columns)
		{
			//	Cette méthode est appelée à la suite de la modification d'une de nos propriétés
			//	de définition pour permettre de mettre à jour les widgets connectés.
			if (this.IsNotSuspended)
			{
				this.SuspendChanges();

				try
				{
					foreach (Widget obj in this.Widgets)
					{
						this.ObjectModifier.SetGridColumnsCount(obj, (int) columns);
					}
				}
				finally
				{
					this.ResumeChanges();
				}
			}
		}

		private void NotifyRowsCountChanged(double rows)
		{
			//	Cette méthode est appelée à la suite de la modification d'une de nos propriétés
			//	de définition pour permettre de mettre à jour les widgets connectés.
			if (this.IsNotSuspended)
			{
				this.SuspendChanges();

				try
				{
					foreach (Widget obj in this.Widgets)
					{
						this.ObjectModifier.SetGridRowsCount(obj, (int) rows);
					}
				}
				finally
				{
					this.ResumeChanges();
				}
			}
		}

		private static void NotifyGridColumnsCountChanged(DependencyObject o, object oldValue, object newValue)
		{
			double value = (double) newValue;
			Grid that = (Grid) o;
			that.NotifyColumnsCountChanged(value);
		}

		private static void NotifyGridRowsCountChanged(DependencyObject o, object oldValue, object newValue)
		{
			double value = (double) newValue;
			Grid that = (Grid) o;
			that.NotifyRowsCountChanged(value);
		}


		static Grid()
		{
			Grid.GridColumnsCountProperty.DefaultMetadata.DefineNamedType(ProxyManager.GridNumericType);
			Grid.GridRowsCountProperty.DefaultMetadata.DefineNamedType(ProxyManager.GridNumericType);

			Grid.GridColumnsCountProperty.DefaultMetadata.DefineCaptionId(new Support.Druid("[100J]").ToLong());
			Grid.GridRowsCountProperty.DefaultMetadata.DefineCaptionId(new Support.Druid("[100K]").ToLong());
		}


		//	TODO: remplacer un jour le type 'double' par 'int' !
		public static readonly DependencyProperty GridColumnsCountProperty = DependencyProperty.Register("GridColumnsCount", typeof(double), typeof(Grid), new DependencyPropertyMetadata(2.0, Grid.NotifyGridColumnsCountChanged));
		public static readonly DependencyProperty GridRowsCountProperty	   = DependencyProperty.Register("GridRowsCount",    typeof(double), typeof(Grid), new DependencyPropertyMetadata(2.0, Grid.NotifyGridRowsCountChanged));
	}
}
