using System.Collections.Generic;

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Proxies
{
	public class Content : Abstract
	{
		public Content(ProxyManager manager) : base (manager)
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
				return "PropertyContent";
			}
		}

		public string DruidCaption
		{
			get
			{
				return (string) this.GetValue(Content.DruidCaptionProperty);
			}
			set
			{
				this.SetValue(Content.DruidCaptionProperty, value);
			}
		}

		public string DruidPanel
		{
			get
			{
				return (string) this.GetValue(Content.DruidPanelProperty);
			}
			set
			{
				this.SetValue(Content.DruidPanelProperty, value);
			}
		}

		public Binding Binding
		{
			get
			{
				return (Binding) this.GetValue(Content.BindingProperty);
			}
			set
			{
				this.SetValue(Content.BindingProperty, value);
			}
		}


		protected override void InitializePropertyValues()
		{
			//	Cette méthode est appelée par Proxies.Abstract quand on connecte
			//	le premier widget avec le proxy.
			
			//	Recopie localement les diverses propriétés du widget sélectionné
			//	pour pouvoir ensuite travailler dessus :
			if (ObjectModifier.HasDruid(this.DefaultWidget))
			{
				ObjectModifier.ObjectType type = ObjectModifier.GetObjectType(this.DefaultWidget);
				if (type == ObjectModifier.ObjectType.SubPanel)
				{
					this.DruidPanel = ObjectModifier.GetDruid(this.DefaultWidget);
				}
				else
				{
					this.DruidCaption = ObjectModifier.GetDruid(this.DefaultWidget);
				}
			}

			if (ObjectModifier.HasBinding(this.DefaultWidget))
			{
				this.Binding = ObjectModifier.GetBinding(this.DefaultWidget);
			}
		}

		private void NotifyDruidChanged(string druid)
		{
			//	Cette méthode est appelée à la suite de la modification d'une de
			//	nos propriétés de définition pour permettre de mettre à jour les widgets connectés :
			if (this.IsNotSuspended)
			{
				this.SuspendChanges();

				try
				{
					foreach (Widget obj in this.Widgets)
					{
						ObjectModifier.SetDruid(obj, druid);
					}
				}
				finally
				{
					this.ResumeChanges();
				}
			}
		}

		private void NotifyBindingChanged(Binding binding)
		{
			//	Cette méthode est appelée à la suite de la modification d'une de
			//	nos propriétés de définition pour permettre de mettre à jour les widgets connectés :
			if (this.IsNotSuspended)
			{
				this.SuspendChanges();

				try
				{
					foreach (Widget obj in this.Widgets)
					{
						ObjectModifier.SetBinding(obj, binding);
					}
				}
				finally
				{
					this.ResumeChanges();
				}
			}
		}


		static Content()
		{
			Content.DruidCaptionProperty.DefaultMetadata.DefineNamedType(ProxyManager.DruidCaptionStringType);
			Content.DruidPanelProperty.DefaultMetadata.DefineNamedType(ProxyManager.DruidPanelStringType);
			Content.BindingProperty.DefaultMetadata.DefineNamedType(ProxyManager.BindingType);
		}


		private static void NotifyDruidChanged(DependencyObject o, object oldValue, object newValue)
		{
			string value = (string) newValue;
			Content that = (Content) o;
			that.NotifyDruidChanged(value);
		}

		private static void NotifyBindingChanged(DependencyObject o, object oldValue, object newValue)
		{
			Binding value = (Binding) newValue;
			Content that = (Content) o;
			that.NotifyBindingChanged(value);
		}


		public static readonly DependencyProperty DruidCaptionProperty = DependencyProperty.Register("DruidCaption", typeof(string),  typeof(Content), new DependencyPropertyMetadata(Content.NotifyDruidChanged));
		public static readonly DependencyProperty DruidPanelProperty   = DependencyProperty.Register("DruidPanel",   typeof(string),  typeof(Content), new DependencyPropertyMetadata(Content.NotifyDruidChanged));
		public static readonly DependencyProperty BindingProperty      = DependencyProperty.Register("Binding",      typeof(Binding), typeof(Content), new DependencyPropertyMetadata(Content.NotifyBindingChanged));
	}
}
