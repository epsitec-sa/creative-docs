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
			//	Ajoute un widget � notre liste de widgets connect�s. Tous les
			//	widgets de cette liste doivent partager exactement les m�mes
			//	propri�t�s en ce qui concerne notre proxy.
			
			System.Diagnostics.Debug.Assert (this.widgets.Contains (widget) == false);
			
			this.widgets.Add (widget);

			if (this.widgets.Count == 1)
			{
				//	Quand on ajoute le premier widget, on lit les propri�t�s
				//	du widget et on initialise le proxy.
				
				this.ReadFromWidget ();
			}
		}

		public void ReadFromWidget()
		{
			//	Synchronise le proxy avec le widget en lisant les propri�t�s du
			//	widget connect�.
			
			if (this.widgets.Count > 0)
			{
				//	Evite des appels r�cursifs de SetWidgetProperty (on ne veut
				//	pas que l'initialisation des propri�t�s soit per�ue comme
				//	un changement qui provoque une mise � jour du widget, car
				//	pendant l'initialisation, certaines valeurs seront forc�ment
				//	encore invalides).
				
				this.SuspendChanges ();
				
				try
				{
					this.InitialisePropertyValues ();
				}
				finally
				{
					this.ResumeChanges ();
				}
			}
		}

		protected abstract void InitialisePropertyValues();

		protected void SuspendChanges()
		{
			this.suspendChanges++;
		}

		protected void ResumeChanges()
		{
			this.suspendChanges--;
		}
		
		protected void SetWidgetProperty(DependencyProperty property, object value)
		{
			//	Met � jour la propri�t� du (ou des) widget(s) connect�(s), pour
			//	autant que cela soit permis.
			
			if (this.suspendChanges == 0)
			{
				this.SuspendChanges ();

				try
				{
					foreach (Widget widget in this.widgets)
					{
						widget.SetValue (property, value);
					}
				}
				finally
				{
					this.ResumeChanges ();
				}
			}
		}

		protected object GetWidgetProperty(DependencyProperty property)
		{
			//	Lit une propri�t� du widget connect�. S'il y a plusieurs widgets,
			//	on lit la propri�t� du premier; mais �a ne doit pas changer grand
			//	chose, vu que tous les widgets ont des propri�t�s identiques.
			
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
		private int suspendChanges = 0;
	}
}
