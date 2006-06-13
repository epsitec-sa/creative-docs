using System.Collections.Generic;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer
{
	public sealed class ProxyManager
	{
		public ProxyManager()
		{
		}

		public IEnumerable<IProxy> Proxies
		{
			get
			{
				return this.proxies;
			}
		}
		
		public void SetSelection(Widget widget)
		{
			this.widgets = new List<Widget>();
			this.widgets.Add(widget);
			this.GenerateProxies();
		}
		
		public void SetSelection(IEnumerable<Widget> collection)
		{
			this.widgets = new List<Widget>();
			this.widgets.AddRange(collection);
			this.GenerateProxies();
		}

		public void CreateUserInterface(Widget container)
		{
			foreach (IProxy proxy in this.Proxies)
			{
				this.CreateUserInterface(container, proxy);
			}
		}

		public static bool EqualValues(IProxy a, IProxy b)
		{
			if (a == b)
			{
				return true;
			}
			
			if (a == null || b == null)
			{
				return false;
			}
			
			//	Ca ne vaut pas la peine de comparer les valeurs des deux proxies
			//	si leur rang est différent, car cela implique qu'ils ne vont pas
			//	être représentés par des panneaux identiques :
			if (a.Rank != b.Rank)
			{
				return false;
			}
			
			return DependencyObject.EqualValues(a as DependencyObject, b as DependencyObject);
		}
		
		private void GenerateProxies()
		{
			//	Génère une liste (triée) de tous les proxies. Il se peut qu'il
			//	y ait plusieurs proxies de type identique si plusieurs widgets
			//	utilisent des réglages différents.
			List<IProxy> proxies = new List<IProxy>();

			foreach (Widget widget in this.widgets)
			{
				foreach (IProxy proxy in this.GenerateProxies(widget))
				{
					//	Evite les doublons pour des proxies qui seraient à 100%
					//	identiques :
					bool insert = true;

					foreach (IProxy item in proxies)
					{
						if (ProxyManager.EqualValues(item, proxy))
						{
							//	Trouvé un doublon. On ajoute simplement le widget
							//	courant au proxy qui existe déjà avec les mêmes
							//	valeurs :
							item.AddWidget(widget);
							insert = false;
							break;
						}
					}

					if (insert)
					{
						proxies.Add(proxy);
					}
				}
			}

			//	Trie les proxies selon leur rang :
			proxies.Sort(new Comparers.ProxyRank());

			this.proxies = proxies;
		}

		private IEnumerable<IProxy> GenerateProxies(Widget widget)
		{
			//	TODO: créer les divers Proxies pour le widget; on peut simplement
			//	ajouter ici des 'yield return new ...'
			yield return new Proxies.Geometry(widget);
		}

		private void CreateUserInterface(Widget container, IProxy proxy)
		{
			//	Crée un panneau pour représenter le proxy spécifié.
			DependencyObject source = proxy as DependencyObject;

			MyWidgets.PropertyPanel panel = new MyWidgets.PropertyPanel(container);
			panel.Dock = DockStyle.Top;
			panel.Icon = proxy.IconName;
			panel.Title = source.GetType().Name;

			foreach (DependencyProperty property in source.LocalProperties)
			{
				Placeholder placeholder = new Placeholder();
				Binding binding = new Binding(BindingMode.TwoWay, source, property.Name);
				placeholder.SetBinding(Placeholder.ValueProperty, binding);
				placeholder.Controller = "String";
				panel.AddPlaceHolder(placeholder);
			}
		}


		private List<Widget>			widgets;
		private List<IProxy>			proxies;
	}
}
