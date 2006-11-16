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
			//	Plus le num�ro est petit, plus le proxy appara�tra haut dans la
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


		protected override void InitializePropertyValues()
		{
			//	Cette m�thode est appel�e par Proxies.Abstract quand on connecte
			//	le premier widget avec le proxy.
			
			//	Recopie localement les diverses propri�t�s du widget s�lectionn�
			//	pour pouvoir ensuite travailler dessus :
			if (this.ObjectModifier.HasDruid(this.DefaultWidget))
			{
				ObjectModifier.ObjectType type = ObjectModifier.GetObjectType(this.DefaultWidget);
				if (type == ObjectModifier.ObjectType.Panel)
				{
					this.DruidPanel = this.ObjectModifier.GetDruid(this.DefaultWidget);
				}
				else
				{
					this.DruidCaption = this.ObjectModifier.GetDruid(this.DefaultWidget);
				}
			}
		}

		private void NotifyDruidChanged(string druid)
		{
			//	Cette m�thode est appel�e � la suite de la modification d'une de
			//	nos propri�t�s de d�finition pour permettre de mettre � jour les widgets connect�s :
			if (this.IsNotSuspended)
			{
				this.SuspendChanges();

				try
				{
					foreach (Widget obj in this.Widgets)
					{
						this.ObjectModifier.SetDruid(obj, druid);
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
		}


		private static void NotifyDruidChanged(DependencyObject o, object oldValue, object newValue)
		{
			string value = (string) newValue;
			Content that = (Content) o;
			that.NotifyDruidChanged(value);
		}


		public static readonly DependencyProperty DruidCaptionProperty = DependencyProperty.Register("DruidCaption", typeof(string), typeof(Content), new DependencyPropertyMetadata(0.0, Content.NotifyDruidChanged));
		public static readonly DependencyProperty DruidPanelProperty   = DependencyProperty.Register("DruidPanel",   typeof(string), typeof(Content), new DependencyPropertyMetadata(0.0, Content.NotifyDruidChanged));
	}
}
