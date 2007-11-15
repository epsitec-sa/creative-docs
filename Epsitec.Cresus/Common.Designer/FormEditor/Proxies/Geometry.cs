using System.Collections.Generic;

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.FormEngine;

namespace Epsitec.Common.Designer.FormEditor.Proxies
{
	public class Geometry : Abstract
	{
		public Geometry(ProxyManager manager) : base (manager)
		{
		}

		public override int Rank
		{
			//	Retourne le rang de ce proxy parmi la liste de tous les proxies.
			//	Plus le numéro est petit, plus le proxy apparaîtra haut dans la
			//	liste.
			get
			{
				return 1;
			}
		}

		public override string IconName
		{
			get
			{
				return "PropertyGeometry";
			}
		}

		public override double DataColumnWidth
		{
			get
			{
				return 22*4+1;
			}
		}

		public override double RowsSpacing
		{
			get
			{
				return 3;
			}
		}


		public int ColumnsRequired
		{
			get
			{
				return (int) this.GetValue(Geometry.ColumnsRequiredProperty);
			}
			set
			{
				this.SetValue(Geometry.ColumnsRequiredProperty, value);
			}
		}

		public int RowsRequired
		{
			get
			{
				return (int) this.GetValue(Geometry.RowsRequiredProperty);
			}
			set
			{
				this.SetValue(Geometry.RowsRequiredProperty, value);
			}
		}

		public FieldDescription.SeparatorType Separator
		{
			get
			{
				return (FieldDescription.SeparatorType) this.GetValue(Geometry.SeparatorProperty);
			}
			set
			{
				this.SetValue(Geometry.SeparatorProperty, value);
			}
		}


		protected override void InitializePropertyValues()
		{
			//	Cette méthode est appelée par Proxies.Abstract quand on connecte
			//	le premier widget avec le proxy.
			
			//	Recopie localement les diverses propriétés du widget sélectionné
			//	pour pouvoir ensuite travailler dessus :
			this.ColumnsRequired = this.ObjectModifier.GetColumnsRequired(this.DefaultWidget);
			this.RowsRequired = this.ObjectModifier.GetRowsRequired(this.DefaultWidget);
			this.Separator = this.ObjectModifier.GetSeparator(this.DefaultWidget);
		}

		static Geometry()
		{
			Geometry.ColumnsRequiredProperty.DefaultMetadata.DefineNamedType(ProxyManager.ColumnsRequiredNumericType);
			Geometry.ColumnsRequiredProperty.DefaultMetadata.DefineCaptionId(Res.Captions.Geometry.Width.Id);

			Geometry.RowsRequiredProperty.DefaultMetadata.DefineNamedType(ProxyManager.RowsRequiredNumericType);
			Geometry.RowsRequiredProperty.DefaultMetadata.DefineCaptionId(Res.Captions.Geometry.Height.Id);

			EnumType separatorEnumType = Res.Types.FieldDescription.SeparatorType;
			Geometry.SeparatorProperty.DefaultMetadata.DefineNamedType(separatorEnumType);
			Geometry.SeparatorProperty.DefaultMetadata.DefineCaptionId(Res.Captions.Grid.ColumnMode.Id);
		}


		private static void NotifyColumnsRequiredChanged(DependencyObject o, object oldValue, object newValue)
		{
			int value = (int) newValue;
			Geometry that = (Geometry) o;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						that.ObjectModifier.SetColumnsRequired(obj, value);
					}
				}
				finally
				{
					that.ResumeChanges();
					that.RegenerateProxiesAndForm();
				}
			}
		}

		private static void NotifyRowsRequiredChanged(DependencyObject o, object oldValue, object newValue)
		{
			int value = (int) newValue;
			Geometry that = (Geometry) o;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						that.ObjectModifier.SetRowsRequired(obj, value);
					}
				}
				finally
				{
					that.ResumeChanges();
					that.RegenerateProxiesAndForm();
				}
			}
		}

		private static void NotifySeparatorChanged(DependencyObject o, object oldValue, object newValue)
		{
			FieldDescription.SeparatorType value = (FieldDescription.SeparatorType) newValue;
			Geometry that = (Geometry) o;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						that.ObjectModifier.SetSeparator(obj, value);
					}
				}
				finally
				{
					that.ResumeChanges();
					that.RegenerateProxiesAndForm();
				}
			}
		}


		public static readonly DependencyProperty ColumnsRequiredProperty = DependencyProperty.Register("ColumnsRequired", typeof(int), typeof(Geometry), new DependencyPropertyMetadata(0, Geometry.NotifyColumnsRequiredChanged));
		public static readonly DependencyProperty RowsRequiredProperty    = DependencyProperty.Register("RowsRequired",    typeof(int), typeof(Geometry), new DependencyPropertyMetadata(0, Geometry.NotifyRowsRequiredChanged));

		public static readonly DependencyProperty SeparatorProperty = DependencyProperty.Register("Separator", typeof(FieldDescription.SeparatorType), typeof(Geometry), new DependencyPropertyMetadata(FieldDescription.SeparatorType.Normal, Geometry.NotifySeparatorChanged));
	}
}
