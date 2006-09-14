using System.Collections.Generic;

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Designer.Proxies
{
	public abstract class Abstract : DependencyObject, IProxy
	{
		protected Abstract(ProxyManager manager)
		{
			this.manager = manager;
		}


		#region IProxy Members
		public void AddWidget(Widget widget)
		{
			//	Ajoute un widget à notre liste de widgets connectés. Tous les
			//	widgets de cette liste doivent partager exactement les mêmes
			//	propriétés en ce qui concerne notre proxy.
			System.Diagnostics.Debug.Assert(this.widgets.Contains(widget) == false);
			this.widgets.Add(widget);

			if (this.widgets.Count == 1)
			{
				//	Quand on ajoute le premier widget, on lit les propriétés
				//	du widget et on initialise le proxy.
				this.ReadFromWidget();
			}
		}

		public void ClearWidgets()
		{
			this.widgets.Clear();
		}

		public IEnumerable<Widget> Widgets
		{
			get
			{
				return this.widgets;
			}
		}

		public void Update()
		{
			this.ReadFromWidget();
		}

		public abstract int Rank
		{
			get;
		}

		public abstract string IconName
		{
			get;
		}

		public virtual double DataColumnWidth
		{
			get
			{
				return 50;
			}
		}

		public virtual double RowsSpacing
		{
			get
			{
				return -1;
			}
		}

		#endregion

		protected ObjectModifier ObjectModifier
		{
			get
			{
				return this.manager.ObjectModifier;
			}
		}

		protected Widget DefaultWidget
		{
			get
			{
				return this.widgets[0];
			}
		}

		protected bool IsSuspended
		{
			get
			{
				return this.suspendChanges > 0;
			}
		}

		protected bool IsNotSuspended
		{
			get
			{
				return this.suspendChanges == 0;
			}
		}

		protected void ReadFromWidget()
		{
			//	Synchronise le proxy avec le widget en lisant les propriétés du
			//	widget connecté.
			if (this.widgets.Count > 0)
			{
				//	Evite des appels récursifs de SetWidgetProperty (on ne veut
				//	pas que l'initialisation des propriétés soit perçue comme
				//	un changement qui provoque une mise à jour du widget, car
				//	pendant l'initialisation, certaines valeurs seront forcément
				//	encore invalides).
				this.SuspendChanges();
				
				try
				{
					this.InitializePropertyValues ();
				}
				finally
				{
					this.ResumeChanges();
				}
			}
		}

		protected abstract void InitializePropertyValues();

		protected void SuspendChanges()
		{
			this.suspendChanges++;
		}

		protected void ResumeChanges()
		{
			this.suspendChanges--;
		}


		protected void RegenerateProxies()
		{
			//	Régénère la liste des proxies et met à jour les panneaux de l'interface
			//	utilisateur s'il y a eu un changement dans le nombre de propriétés visibles
			//	par panneau.
			Application.QueueAsyncCallback(this.manager.Panel.RegenerateProxies);
		}
		

#if false
		protected void SetWidgetProperty(DependencyProperty property, object value)
		{
			//	Met à jour la propriété du (ou des) widget(s) connecté(s), pour
			//	autant que cela soit permis.
			if (this.suspendChanges == 0)
			{
				this.SuspendChanges();

				try
				{
					foreach (Widget widget in this.widgets)
					{
						widget.SetValue(property, value);
					}
				}
				finally
				{
					this.ResumeChanges();
				}
			}
		}

		protected object GetWidgetProperty(DependencyProperty property)
		{
			//	Lit une propriété du widget connecté. S'il y a plusieurs widgets,
			//	on lit la propriété du premier; mais ça ne doit pas changer grand
			//	chose, vu que tous les widgets ont des propriétés identiques.
			if (this.widgets.Count > 0)
			{
				return this.widgets[0].GetValue(property);
			}
			else
			{
				return UndefinedValue.Instance;
			}
		}
#endif


		private ProxyManager			manager;
		private List<Widget>			widgets = new List<Widget>();
		private int						suspendChanges;
	}
}
