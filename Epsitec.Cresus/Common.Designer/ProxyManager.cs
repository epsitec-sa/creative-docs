using System.Collections.Generic;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer
{
	public sealed class ProxyManager
	{
		public ProxyManager(ObjectModifier objectModifier)
		{
			this.objectModifier = objectModifier;
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
			foreach (IProxy proxy in this.proxies)
			{
				this.CreateUserInterface(container, proxy);
			}
		}

		public void UpdateUserInterface()
		{
			if (this.Proxies != null)
			{
				foreach (IProxy proxy in this.Proxies)
				{
					proxy.Update();
				}
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
			//	si leur rang est diff�rent, car cela implique qu'ils ne vont pas
			//	�tre repr�sent�s par des panneaux identiques :
			if (a.Rank != b.Rank)
			{
				return false;
			}
			
			return DependencyObject.EqualValues(a as DependencyObject, b as DependencyObject);
		}
		
		private void GenerateProxies()
		{
			//	G�n�re une liste (tri�e) de tous les proxies. Il se peut qu'il
			//	y ait plusieurs proxies de type identique si plusieurs widgets
			//	utilisent des r�glages diff�rents.
			List<IProxy> proxies = new List<IProxy>();

			foreach (Widget widget in this.widgets)
			{
				foreach (IProxy proxy in this.GenerateProxies(widget))
				{
					//	Evite les doublons pour des proxies qui seraient � 100%
					//	identiques :
					bool insert = true;

					foreach (IProxy item in proxies)
					{
						if (ProxyManager.EqualValues(item, proxy))
						{
							//	Trouv� un doublon. On ajoute simplement le widget
							//	courant au proxy qui existe d�j� avec les m�mes
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
			yield return new Proxies.Geometry(widget, this.objectModifier);
			yield return new Proxies.Layout(widget, this.objectModifier);
			yield return new Proxies.Padding(widget, this.objectModifier);
		}

		private void CreateUserInterface(Widget container, IProxy proxy)
		{
			//	Cr�e un panneau pour repr�senter le proxy sp�cifi�.
			DependencyObject source = proxy as DependencyObject;

			MyWidgets.PropertyPanel panel = new MyWidgets.PropertyPanel(container);
			panel.Dock = DockStyle.Top;
			panel.Icon = proxy.IconName;
			panel.DataColumnWidth = proxy.DataColumnWidth;
			panel.Title = source.GetType().Name;

			foreach (DependencyProperty property in source.DefinedProperties)
			{
				Placeholder placeholder = new Placeholder();
				Binding binding = new Binding(BindingMode.TwoWay, source, property.Name);
				placeholder.SetBinding(Placeholder.ValueProperty, binding);
				placeholder.Controller = "*";
				panel.AddPlaceHolder(placeholder);
			}
		}

		static ProxyManager()
		{
			Types.DoubleType locationNumericType = new Types.DoubleType (-9999, 9999, 0.1M);
			Types.DoubleType sizeNumericType     = new Types.DoubleType (0, 9999, 0.1M);
			Types.DoubleType marginNumericType   = new Types.DoubleType (-1, 9999, 0.1M);

			locationNumericType.DefinePreferredRange (new Types.DecimalRange (0, 1000, 2));
			sizeNumericType.DefinePreferredRange (new Types.DecimalRange (0, 1000, 1));
			marginNumericType.DefinePreferredRange (new Types.DecimalRange (0, 200, 1));

			ProxyManager.LocationNumericType = locationNumericType;
			ProxyManager.SizeNumericType     = sizeNumericType;
			ProxyManager.MarginNumericType   = marginNumericType;
		}
		
		public static readonly Types.INumericType LocationNumericType;
		public static readonly Types.INumericType SizeNumericType;
		public static readonly Types.INumericType MarginNumericType;

		private ObjectModifier			objectModifier;
		private List<Widget>			widgets;
		private List<IProxy>			proxies;
	}
}
