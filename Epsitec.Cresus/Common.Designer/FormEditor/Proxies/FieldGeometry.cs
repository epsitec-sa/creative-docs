using System.Collections.Generic;

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.FormEngine;

namespace Epsitec.Common.Designer.FormEditor.Proxies
{
	public class FieldGeometry : Abstract
	{
		public FieldGeometry(ProxyManager manager) : base (manager)
		{
		}

		public override int Rank
		{
			//	Retourne le rang de ce proxy parmi la liste de tous les proxies.
			//	Plus le numéro est petit, plus le proxy apparaîtra haut dans la
			//	liste.
			get
			{
				return 100+1;
			}
		}

		public override string IconName
		{
			get
			{
				return "PropertyContent";
			}
		}


		public int ColumnsRequired
		{
			get
			{
				return (int) this.GetValue(FieldGeometry.ColumnsRequiredProperty);
			}
			set
			{
				this.SetValue(FieldGeometry.ColumnsRequiredProperty, value);
			}
		}

		public int RowsRequired
		{
			get
			{
				return (int) this.GetValue(FieldGeometry.RowsRequiredProperty);
			}
			set
			{
				this.SetValue(FieldGeometry.RowsRequiredProperty, value);
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
		}

		static FieldGeometry()
		{
			FieldGeometry.ColumnsRequiredProperty.DefaultMetadata.DefineNamedType(ProxyManager.ColumnsRequiredNumericType);
			FieldGeometry.ColumnsRequiredProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldGeometry.ColumnsRequired.Id);

			FieldGeometry.RowsRequiredProperty.DefaultMetadata.DefineNamedType(ProxyManager.RowsRequiredNumericType);
			FieldGeometry.RowsRequiredProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldGeometry.RowsRequired.Id);
		}


		private static void NotifyColumnsRequiredChanged(DependencyObject o, object oldValue, object newValue)
		{
			int value = (int) newValue;
			FieldGeometry that = (FieldGeometry) o;

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
			FieldGeometry that = (FieldGeometry) o;

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


		public static readonly DependencyProperty ColumnsRequiredProperty = DependencyProperty.Register("ColumnsRequired", typeof(int), typeof(FieldGeometry), new DependencyPropertyMetadata(0, FieldGeometry.NotifyColumnsRequiredChanged));
		public static readonly DependencyProperty RowsRequiredProperty    = DependencyProperty.Register("RowsRequired",    typeof(int), typeof(FieldGeometry), new DependencyPropertyMetadata(0, FieldGeometry.NotifyRowsRequiredChanged));
	}
}
