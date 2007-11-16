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
			//	Les Proxies de PanelEditor sont numérotés à partir de 0.
			//	Ceux de FormEditor sont numérotés à partir de 100.
			//	Ceci évite les collisions dans PanelsContext.IsExtendedProxies !
			get
			{
				return 100+2;
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


		protected override void InitializePropertyValues()
		{
			//	Cette méthode est appelée par Proxies.Abstract quand on connecte
			//	le premier widget avec le proxy.
			if (this.ObjectModifier.IsField(this.DefaultWidget) ||
				this.ObjectModifier.IsBox(this.DefaultWidget)   )
			{
				this.ColumnsRequired = this.ObjectModifier.GetColumnsRequired(this.DefaultWidget);
				this.RowsRequired = this.ObjectModifier.GetRowsRequired(this.DefaultWidget);
			}
		}

		static Geometry()
		{
			Geometry.ColumnsRequiredProperty.DefaultMetadata.DefineNamedType(ProxyManager.ColumnsRequiredNumericType);
			Geometry.ColumnsRequiredProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldGeometry.ColumnsRequired.Id);

			Geometry.RowsRequiredProperty.DefaultMetadata.DefineNamedType(ProxyManager.RowsRequiredNumericType);
			Geometry.RowsRequiredProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldGeometry.RowsRequired.Id);
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


		public static readonly DependencyProperty ColumnsRequiredProperty = DependencyProperty.Register("ColumnsRequired", typeof(int), typeof(Geometry), new DependencyPropertyMetadata(0, Geometry.NotifyColumnsRequiredChanged));
		public static readonly DependencyProperty RowsRequiredProperty    = DependencyProperty.Register("RowsRequired",    typeof(int), typeof(Geometry), new DependencyPropertyMetadata(0, Geometry.NotifyRowsRequiredChanged));
	}
}
