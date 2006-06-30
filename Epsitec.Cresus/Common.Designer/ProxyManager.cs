using System.Collections.Generic;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer
{
	public sealed class ProxyManager
	{
		public ProxyManager(Viewers.Panels panel)
		{
			this.panel = panel;
			this.objectModifier = this.panel.PanelEditor.ObjectModifier;
			this.extendedProxies = new Dictionary<int, bool>();
		}

		public IEnumerable<Widget> Widgets
		{
			get
			{
				return this.widgets;
			}
		}

		public Viewers.Panels Panel
		{
			get
			{
				return this.panel;
			}
		}

		public ObjectModifier ObjectModifier
		{
			get
			{
				return this.objectModifier;
			}
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
			//	Spécifie l'objet sélectionné et construit la liste des proxies nécessaires.
			this.widgets = new List<Widget>();
			this.widgets.Add(widget);
			this.GenerateProxies();
		}
		
		public void SetSelection(IEnumerable<Widget> collection)
		{
			//	Spécifie les objets sélectionnés et construit la liste des proxies nécessaires.
			this.widgets = new List<Widget>();
			this.widgets.AddRange(collection);
			this.GenerateProxies();
		}

		public void CreateUserInterface(Widget container)
		{
			//	Crée l'interface utilisateur (panneaux) pour la liste des proxies.
			foreach (IProxy proxy in this.proxies)
			{
				this.CreateUserInterface(container, proxy);
			}
		}

		public void UpdateUserInterface()
		{
			//	Met à jour l'interface utilisateur (panneaux), sans changer le nombre de
			//	propriétés visibles par panneau.
			if (this.Proxies != null)
			{
				foreach (IProxy proxy in this.Proxies)
				{
					proxy.Update();
				}
			}
		}

		public bool RegenerateProxies()
		{
			//	Régénère la liste des proxies.
			//	Retourne true si la liste a changé.
			return this.GenerateProxies();
		}

		public void ClearUserInterface(Widget container)
		{
			//	Supprime l'interface utilisateur (panneaux) pour la liste des proxies.
			foreach (Widget obj in container.Children)
			{
				if (obj is MyWidgets.PropertyPanel)
				{
					MyWidgets.PropertyPanel panel = obj as MyWidgets.PropertyPanel;
					panel.ExtendedSize -= new Epsitec.Common.Support.EventHandler(this.HandlePanelExtendedSize);
				}
			}

			container.Children.Clear();
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

		private bool GenerateProxies()
		{
			//	Génère une liste (triée) de tous les proxies. Il se peut qu'il
			//	y ait plusieurs proxies de type identique si plusieurs widgets
			//	utilisent des réglages différents.
			//	Retourne true si la liste a changé.
			List<IProxy> proxies = new List<IProxy>();

			foreach (Widget widget in this.widgets)
			{
				foreach (IProxy proxy in this.GenerateWidgetProxies(widget))
				{
					//	Evite les doublons pour des proxies qui seraient à 100%
					//	identiques :
					bool insert = true;

					proxy.AddWidget(widget);
					
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

			if (ProxyManager.EqualLists(this.proxies, proxies))
			{
				for (int i=0; i<proxies.Count; i++)
				{
					this.proxies[i].ClearWidgets();

					foreach (Widget widget in proxies[i].Widgets)
					{
						this.proxies[i].AddWidget(widget);
					}
				}
				return false;
			}
			else
			{
				this.proxies = proxies;
				return true;
			}
		}

		private IEnumerable<IProxy> GenerateWidgetProxies(Widget widget)
		{
			yield return new Proxies.Geometry(this);
			yield return new Proxies.Layout(this);
			yield return new Proxies.Padding(this);
			yield return new Proxies.Grid(this);
		}

		static private bool EqualLists(List<IProxy> list1, List<IProxy> list2)
		{
			//	Compare si deux listes contiennent des proxies identiques.
			if (list1 != null && list1.Count == list2.Count)
			{
				for (int i=0; i<list1.Count; i++)
				{
					IProxy item1 = list1[i];
					IProxy item2 = list2[i];
					if (!ProxyManager.EqualValues(item1, item2))
					{
						return false;
					}
				}
				return true;
			}

			return false;
		}

		private void CreateUserInterface(Widget container, IProxy proxy)
		{
			//	Crée un panneau pour représenter le proxy spécifié.
			DependencyObject source = proxy as DependencyObject;

			MyWidgets.PropertyPanel panel = new MyWidgets.PropertyPanel(container);
			panel.Dock = DockStyle.Top;
			panel.Icon = proxy.IconName;
			panel.DataColumnWidth = proxy.DataColumnWidth;
			panel.RowsSpacing = proxy.RowsSpacing;
			panel.Title = source.GetType().Name;
			panel.Rank = proxy.Rank;
			panel.IsExtendedSize = this.IsExtendedProxies(proxy.Rank);
			panel.ExtendedSize += new Epsitec.Common.Support.EventHandler(this.HandlePanelExtendedSize);

			foreach (DependencyProperty property in source.DefinedProperties)
			{
				Placeholder placeholder = new Placeholder();
				Binding binding = new Binding(BindingMode.TwoWay, source, property.Name);
				placeholder.SetBinding(Placeholder.ValueProperty, binding);
				placeholder.Controller = "*";
				panel.AddPlaceHolder(placeholder);
			}
		}

		void HandlePanelExtendedSize(object sender)
		{
			MyWidgets.PropertyPanel panel = sender as MyWidgets.PropertyPanel;
			System.Diagnostics.Debug.Assert(panel != null);

			this.SetExtendedProxies(panel.Rank, panel.IsExtendedSize);
		}

		private bool IsExtendedProxies(int rank)
		{
			//	Indique si un panneau pour un proxy est étendu ou non.
			if (!this.extendedProxies.ContainsKey(rank))
			{
				this.extendedProxies.Add(rank, false);
			}

			return this.extendedProxies[rank];
		}

		private void SetExtendedProxies(int rank, bool extended)
		{
			//	Modifie l'état étendu ou non d'un panneau pour un proxy.
			if (!this.extendedProxies.ContainsKey(rank))
			{
				this.extendedProxies.Add(rank, false);
			}

			this.extendedProxies[rank] = extended;
		}


		static ProxyManager()
		{
			Types.DoubleType locationNumericType = new Types.DoubleType(-9999, 9999, 1.0M);
			Types.DoubleType sizeNumericType     = new Types.DoubleType(0, 9999, 1.0M);
			Types.DoubleType marginNumericType   = new Types.DoubleType(-1, 9999, 1.0M);
			Types.IntegerType gridNumericType    = new Types.IntegerType(1, 100);

			locationNumericType.DefinePreferredRange(new Types.DecimalRange(0, 1000, 2));
			sizeNumericType.DefinePreferredRange(new Types.DecimalRange(0, 1000, 1));
			marginNumericType.DefinePreferredRange(new Types.DecimalRange(0, 200, 1));
			gridNumericType.DefinePreferredRange(new Types.DecimalRange(1, 10, 1));

			ProxyManager.LocationNumericType = locationNumericType;
			ProxyManager.SizeNumericType     = sizeNumericType;
			ProxyManager.MarginNumericType   = marginNumericType;
			ProxyManager.GridNumericType     = gridNumericType;
		}
		
		public static readonly Types.INumericType LocationNumericType;
		public static readonly Types.INumericType SizeNumericType;
		public static readonly Types.INumericType MarginNumericType;
		public static readonly Types.INumericType GridNumericType;

		private Viewers.Panels				panel;
		private ObjectModifier				objectModifier;
		private List<Widget>				widgets;
		private List<IProxy>				proxies;
		private Dictionary<int, bool>		extendedProxies;	
	}
}
