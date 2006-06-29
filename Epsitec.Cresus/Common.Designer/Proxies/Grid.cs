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

		public override double DataColumnWidth
		{
			get
			{
				return 50;
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

		public ObjectModifier.GridMode GridColumnMode
		{
			get
			{
				return (ObjectModifier.GridMode) this.GetValue(Grid.GridColumnModeProperty);
			}
			set
			{
				this.SetValue(Grid.GridColumnModeProperty, value);
			}
		}

		public ObjectModifier.GridMode GridRowMode
		{
			get
			{
				return (ObjectModifier.GridMode) this.GetValue(Grid.GridRowModeProperty);
			}
			set
			{
				this.SetValue(Grid.GridRowModeProperty, value);
			}
		}

		public double GridColumnValue
		{
			get
			{
				return (double) this.GetValue(Grid.GridColumnValueProperty);
			}
			set
			{
				this.SetValue(Grid.GridColumnValueProperty, value);
			}
		}

		public double GridRowValue
		{
			get
			{
				return (double) this.GetValue(Grid.GridRowValueProperty);
			}
			set
			{
				this.SetValue(Grid.GridRowValueProperty, value);
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

			if (this.ObjectModifier.AreChildrenGrid(this.DefaultWidget))
			{
				GridSelection gs = GridSelection.Get(this.DefaultWidget);
				if (gs != null)
				{
					if (gs.Unit == GridSelection.SelectionUnit.Column)
					{
						ObjectModifier.GridMode mode = this.ObjectModifier.GetGridColumnMode(this.DefaultWidget, gs.Index);
						double value = this.ObjectModifier.GetGridColumnWidth(this.DefaultWidget, gs.Index);

						this.GridColumnMode = mode;
						this.GridColumnValue = value;
					}

					if (gs.Unit == GridSelection.SelectionUnit.Row)
					{
						ObjectModifier.GridMode mode = this.ObjectModifier.GetGridRowMode(this.DefaultWidget, gs.Index);
						double value = this.ObjectModifier.GetGridRowHeight(this.DefaultWidget, gs.Index);

						this.GridRowMode = mode;
						this.GridRowValue = value;
					}
				}
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

		private static void NotifyGridColumnModeChanged(DependencyObject o, object oldValue, object newValue)
		{
			//	Cette méthode est appelée à la suite de la modification d'une de nos propriétés
			//	de définition pour permettre de mettre à jour les widgets connectés.
			ObjectModifier.GridMode value = (ObjectModifier.GridMode) newValue;
			Grid that = (Grid) o;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						GridSelection gs = GridSelection.Get(obj);
						if (gs != null)
						{
							if (gs.Unit == GridSelection.SelectionUnit.Column)
							{
								that.ObjectModifier.SetGridColumnMode(obj, gs.Index, value);
							}
						}
					}
				}
				finally
				{
					that.ResumeChanges();
				}
			}
		}

		private static void NotifyGridRowModeChanged(DependencyObject o, object oldValue, object newValue)
		{
			//	Cette méthode est appelée à la suite de la modification d'une de nos propriétés
			//	de définition pour permettre de mettre à jour les widgets connectés.
			ObjectModifier.GridMode value = (ObjectModifier.GridMode) newValue;
			Grid that = (Grid) o;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						GridSelection gs = GridSelection.Get(obj);
						if (gs != null)
						{
							if (gs.Unit == GridSelection.SelectionUnit.Row)
							{
								that.ObjectModifier.SetGridRowMode(obj, gs.Index, value);
							}
						}
					}
				}
				finally
				{
					that.ResumeChanges();
				}
			}
		}

		private static void NotifyGridColumnValueChanged(DependencyObject o, object oldValue, object newValue)
		{
			//	Cette méthode est appelée à la suite de la modification d'une de nos propriétés
			//	de définition pour permettre de mettre à jour les widgets connectés.
			double value = (double) newValue;
			Grid that = (Grid) o;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						GridSelection gs = GridSelection.Get(obj);
						if (gs != null)
						{
							if (gs.Unit == GridSelection.SelectionUnit.Column)
							{
								that.ObjectModifier.SetGridColumnWidth(obj, gs.Index, value);
							}
						}
					}
				}
				finally
				{
					that.ResumeChanges();
				}
			}
		}

		private static void NotifyGridRowValueChanged(DependencyObject o, object oldValue, object newValue)
		{
			//	Cette méthode est appelée à la suite de la modification d'une de nos propriétés
			//	de définition pour permettre de mettre à jour les widgets connectés.
			double value = (double) newValue;
			Grid that = (Grid) o;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						GridSelection gs = GridSelection.Get(obj);
						if (gs != null)
						{
							if (gs.Unit == GridSelection.SelectionUnit.Row)
							{
								that.ObjectModifier.SetGridRowHeight(obj, gs.Index, value);
							}
						}
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

			Grid.GridColumnModeProperty.DefaultMetadata.DefineCaptionId(new Support.Druid("[100N]").ToLong());
			Grid.GridRowModeProperty.DefaultMetadata.DefineCaptionId(new Support.Druid("[100O]").ToLong());

			Grid.GridColumnValueProperty.DefaultMetadata.DefineNamedType(ProxyManager.SizeNumericType);
			Grid.GridRowValueProperty.DefaultMetadata.DefineNamedType(ProxyManager.SizeNumericType);
			Grid.GridColumnValueProperty.DefaultMetadata.DefineCaptionId(new Support.Druid("[100P]").ToLong());
			Grid.GridRowValueProperty.DefaultMetadata.DefineCaptionId(new Support.Druid("[100Q]").ToLong());
		}


		public static readonly DependencyProperty GridColumnsCountProperty = DependencyProperty.Register("GridColumnsCount", typeof(int), typeof(Grid), new DependencyPropertyMetadata(2, Grid.NotifyGridColumnsCountChanged));
		public static readonly DependencyProperty GridRowsCountProperty	   = DependencyProperty.Register("GridRowsCount",    typeof(int), typeof(Grid), new DependencyPropertyMetadata(2, Grid.NotifyGridRowsCountChanged));

		public static readonly DependencyProperty GridColumnSpanProperty   = DependencyProperty.Register("GridColumnSpan", typeof(int), typeof(Grid), new DependencyPropertyMetadata(1, Grid.NotifyGridColumnSpanChanged));
		public static readonly DependencyProperty GridRowSpanProperty	   = DependencyProperty.Register("GridRowSpan",    typeof(int), typeof(Grid), new DependencyPropertyMetadata(1, Grid.NotifyGridRowSpanChanged));

		public static readonly DependencyProperty GridColumnModeProperty   = DependencyProperty.Register("GridColumnMode", typeof(ObjectModifier.GridMode), typeof(Grid), new DependencyPropertyMetadata(ObjectModifier.GridMode.Proportional, Grid.NotifyGridColumnModeChanged));
		public static readonly DependencyProperty GridRowModeProperty	   = DependencyProperty.Register("GridRowMode",    typeof(ObjectModifier.GridMode), typeof(Grid), new DependencyPropertyMetadata(ObjectModifier.GridMode.Proportional, Grid.NotifyGridRowModeChanged));

		public static readonly DependencyProperty GridColumnValueProperty  = DependencyProperty.Register("GridColumnValue", typeof(double), typeof(Grid), new DependencyPropertyMetadata(1.0, Grid.NotifyGridColumnValueChanged));
		public static readonly DependencyProperty GridRowValueProperty	   = DependencyProperty.Register("GridRowValue",    typeof(double), typeof(Grid), new DependencyPropertyMetadata(1.0, Grid.NotifyGridRowValueChanged));
	}
}
