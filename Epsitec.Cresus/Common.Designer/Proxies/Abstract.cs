using System.Collections.Generic;

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Designer.Proxies
{
	public abstract class Abstract : DependencyObject
	{
		protected Abstract(Widget widget)
		{
			this.AddWidget (widget);
		}

		public void AddWidget(Widget widget)
		{
			System.Diagnostics.Debug.Assert (this.widgets.Contains (widget) == false);
			this.widgets.Add (widget);

			if (this.widgets.Count == 1)
			{
				this.ReadFromWidget ();
			}
		}

		public void ReadFromWidget()
		{
			if (this.widgets.Count > 0)
			{
				this.suspendSetWidgetProperty++;
				this.InitialisePropertyValues ();
				this.suspendSetWidgetProperty--;
			}
		}

		protected abstract void InitialisePropertyValues();
		
		protected void SetWidgetProperty(DependencyProperty property, object value)
		{
			if (this.suspendSetWidgetProperty == 0)
			{
				foreach (Widget widget in this.widgets)
				{
					widget.SetValue (property, value);
				}
			}
		}

		protected object GetWidgetProperty(DependencyProperty property)
		{
			if (this.widgets.Count > 0)
			{
				return this.widgets[0].GetValue (property);
			}
			else
			{
				return UndefinedValue.Instance;
			}
		}

		List<Widget> widgets = new List<Widget> ();
		private int suspendSetWidgetProperty = 0;
	}
}
