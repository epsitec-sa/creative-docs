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

		public List<UI.ItemTableColumn> TableColumns
		{
			get
			{
				return (List<UI.ItemTableColumn>) this.GetValue (Content.TableProperty);
			}
			set
			{
				this.SetValue(Content.TableProperty, value);
			}
		}

		public StructuredType StructuredType
		{
			get
			{
				return (StructuredType) this.GetValue(Content.StructuredTypeProperty);
			}
			set
			{
				this.SetValue(Content.StructuredTypeProperty, value);
			}
		}


		protected override void InitializePropertyValues()
		{
			//	Cette m�thode est appel�e par Proxies.Abstract quand on connecte
			//	le premier widget avec le proxy.
			
			//	Recopie localement les diverses propri�t�s du widget s�lectionn�
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

			if (ObjectModifier.GetObjectType(this.DefaultWidget) == ObjectModifier.ObjectType.Table)
			{
				UI.TablePlaceholder table = this.DefaultWidget as UI.TablePlaceholder;
				this.TableColumns = new List<UI.ItemTableColumn> (table.Columns);
			}

			if (ObjectModifier.HasStructuredType(this.DefaultWidget))
			{
				this.StructuredType = ObjectModifier.GetStructuredType(this.DefaultWidget);
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
			//	Cette m�thode est appel�e � la suite de la modification d'une de
			//	nos propri�t�s de d�finition pour permettre de mettre � jour les widgets connect�s :
			if (this.IsNotSuspended)
			{
				this.SuspendChanges();

				try
				{
					StructuredType type = this.StructuredType;
					
					foreach (Widget obj in this.Widgets)
					{
						ObjectModifier.SetBinding(obj, binding, type);
					}
				}
				finally
				{
					this.ResumeChanges();
				}
			}
		}

		private void NotifyTableChanged(List<UI.ItemTableColumn> columns)
		{
			//	Cette m�thode est appel�e � la suite de la modification d'une de
			//	nos propri�t�s de d�finition pour permettre de mettre � jour les widgets connect�s :
			if (this.IsNotSuspended)
			{
				this.SuspendChanges();

				try
				{
					StructuredType type = this.StructuredType;

					foreach (Widget obj in this.Widgets)
					{
						UI.TablePlaceholder table = obj as UI.TablePlaceholder;
						if (table != null)
						{
							table.Columns.Clear ();
							table.Columns.AddRange (columns);
						}
					}
				}
				finally
				{
					this.ResumeChanges();
				}
			}
		}

		private void NotifyStructuredTypeChanged(StructuredType type)
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
						ObjectModifier.SetStructuredType(obj, type);
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
			Content.TableProperty.DefaultMetadata.DefineNamedType(ProxyManager.TableType);
			Content.StructuredTypeProperty.DefaultMetadata.DefineNamedType(ProxyManager.StructuredType);
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

		private static void NotifyTableChanged(DependencyObject o, object oldValue, object newValue)
		{
			List<UI.ItemTableColumn> value = (List<UI.ItemTableColumn>) newValue;
			Content that = (Content) o;
			that.NotifyTableChanged(value);
		}

		private static void NotifyStructuredTypeChanged(DependencyObject o, object oldValue, object newValue)
		{
			StructuredType value = (StructuredType) newValue;
			Content that = (Content) o;
			that.NotifyStructuredTypeChanged(value);
		}


		public static readonly DependencyProperty DruidCaptionProperty   = DependencyProperty.Register("DruidCaption",   typeof(string),         typeof(Content), new DependencyPropertyMetadata(Content.NotifyDruidChanged));
		public static readonly DependencyProperty DruidPanelProperty     = DependencyProperty.Register("DruidPanel",     typeof(string),         typeof(Content), new DependencyPropertyMetadata(Content.NotifyDruidChanged));
		public static readonly DependencyProperty BindingProperty        = DependencyProperty.Register("Binding",        typeof(Binding),        typeof(Content), new DependencyPropertyMetadata(Content.NotifyBindingChanged));
		public static readonly DependencyProperty TableProperty          = DependencyProperty.Register("Table", typeof(List<UI.ItemTableColumn>), typeof(Content), new DependencyPropertyMetadata(Content.NotifyTableChanged));
		public static readonly DependencyProperty StructuredTypeProperty = DependencyProperty.Register("StructuredType", typeof(StructuredType), typeof(Content), new DependencyPropertyMetadata(Content.NotifyStructuredTypeChanged));
	}
}
