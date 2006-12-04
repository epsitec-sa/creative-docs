using System.Collections.Generic;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Designer
{
	public sealed class ProxyManager
	{
		public ProxyManager(Viewers.Panels panel)
		{
			this.panel = panel;
			this.objectModifier = this.panel.PanelEditor.ObjectModifier;
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
			//	Sp�cifie l'objet s�lectionn� et construit la liste des proxies n�cessaires.
			this.widgets = new List<Widget>();
			this.widgets.Add(widget);
			this.GenerateProxies();
		}
		
		public void SetSelection(IEnumerable<Widget> collection)
		{
			//	Sp�cifie les objets s�lectionn�s et construit la liste des proxies n�cessaires.
			this.widgets = new List<Widget>();
			this.widgets.AddRange(collection);
			this.GenerateProxies();
		}

		public void CreateUserInterface(Widget container)
		{
			//	Cr�e l'interface utilisateur (panneaux) pour la liste des proxies.
			foreach (IProxy proxy in this.proxies)
			{
				this.CreateUserInterface(container, proxy);
			}
		}

		public void UpdateUserInterface()
		{
			//	Met � jour l'interface utilisateur (panneaux), sans changer le nombre de
			//	propri�t�s visibles par panneau.
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
			//	R�g�n�re la liste des proxies.
			//	Retourne true si la liste a chang�.
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
			//	si leur rang est diff�rent, car cela implique qu'ils ne vont pas
			//	�tre repr�sent�s par des panneaux identiques :
			if (a.Rank != b.Rank)
			{
				return false;
			}
			
			return DependencyObject.EqualValues(a as DependencyObject, b as DependencyObject);
		}

		private bool GenerateProxies()
		{
			//	G�n�re une liste (tri�e) de tous les proxies. Il se peut qu'il
			//	y ait plusieurs proxies de type identique si plusieurs widgets
			//	utilisent des r�glages diff�rents.
			//	Retourne true si la liste a chang�.
			List<IProxy> proxies = new List<IProxy>();

			foreach (Widget widget in this.widgets)
			{
				foreach (IProxy proxy in this.GenerateWidgetProxies(widget))
				{
					//	Evite les doublons pour des proxies qui seraient � 100%
					//	identiques :
					bool insert = true;

					proxy.AddWidget(widget);
					
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
			yield return new Proxies.Content(this);
			yield return new Proxies.Aspect(this);
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
			//	Cr�e un panneau pour repr�senter le proxy sp�cifi�.
			DependencyObject source = proxy as DependencyObject;

			MyWidgets.PropertyPanel panel = new MyWidgets.PropertyPanel(container);
			panel.Dock = DockStyle.Top;
			panel.Icon = proxy.IconName;
			panel.DataColumnWidth = proxy.DataColumnWidth;
			panel.RowsSpacing = proxy.RowsSpacing;
			panel.Title = source.GetType().Name;
			panel.Rank = proxy.Rank;
			panel.IsExtendedSize = this.panel.PanelsContext.IsExtendedProxies(proxy.Rank);
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

		private void HandlePanelExtendedSize(object sender)
		{
			MyWidgets.PropertyPanel panel = sender as MyWidgets.PropertyPanel;
			System.Diagnostics.Debug.Assert(panel != null);

			this.panel.PanelsContext.SetExtendedProxies(panel.Rank, panel.IsExtendedSize);
		}


		static ProxyManager()
		{
			StringType druidCaptionStringType = new StringType();
			druidCaptionStringType.DefineDefaultController("Druid", "Caption");  // utilise DruidController
			ProxyManager.DruidCaptionStringType = druidCaptionStringType;

			StringType druidPanelStringType = new StringType();
			druidPanelStringType.DefineDefaultController("Druid", "Panel");  // utilise DruidController
			ProxyManager.DruidPanelStringType = druidPanelStringType;

			InternalBindingType bindingType = new InternalBindingType();
			bindingType.DefineDefaultController("Binding", "");  // utilise BindingController
			ProxyManager.BindingType = bindingType;

			InternalTableType tableType = new InternalTableType();
			tableType.DefineDefaultController("Table", "");  // utilise TableController
			ProxyManager.TableType = tableType;

			InternalStructuredType structuredType = new InternalStructuredType();
			structuredType.DefineDefaultController("Structured", "");  // utilise StructuredController
			ProxyManager.StructuredType = structuredType;

			DoubleType locationNumericType = new DoubleType(-9999, 9999, 1.0M);
			locationNumericType.DefinePreferredRange(new DecimalRange(0, 1000, 2));
			ProxyManager.LocationNumericType = locationNumericType;
			
			DoubleType sizeNumericType = new DoubleType(0, 9999, 1.0M);
			sizeNumericType.DefinePreferredRange(new DecimalRange(0, 1000, 1));
			ProxyManager.SizeNumericType = sizeNumericType;
			
			DoubleType marginNumericType = new DoubleType(-1, 9999, 1.0M);
			marginNumericType.DefinePreferredRange(new DecimalRange(0, 200, 1));
			ProxyManager.MarginNumericType = marginNumericType;
			
			IntegerType gridNumericType = new IntegerType(1, 100);
			gridNumericType.DefinePreferredRange(new DecimalRange(1, 10, 1));
			ProxyManager.GridNumericType = gridNumericType;
		}

		private class InternalBindingType : AbstractType
		{
			public override System.Type SystemType
			{
				get
				{
					return typeof(Binding);
				}
			}

			public override bool IsValidValue(object value)
			{
				if (value == null)
				{
					return true;
				}
				else
				{
					return value is Binding;
				}
			}
		}

//		--- erreur // UI.Collections.ItemTableColumnCollection n'est pas un AbstractType; comment faire ???
		private class InternalTableType : AbstractType
		{
			public override System.Type SystemType
			{
				get
				{
					return typeof (List<UI.ItemTableColumn>);
				}
			}

			public override bool IsValidValue(object value)
			{
				if (value == null)
				{
					return true;
				}
				else
				{
					return value is List<UI.ItemTableColumn>;
				}
			}
		}

		private class InternalStructuredType : AbstractType
		{
			public override System.Type SystemType
			{
				get
				{
					return typeof(StructuredType);
				}
			}

			public override bool IsValidValue(object value)
			{
				if (value == null)
				{
					return true;
				}
				else
				{
					return value is StructuredType;
				}
			}
		}

		public static readonly IStringType  DruidCaptionStringType;
		public static readonly IStringType  DruidPanelStringType;
		public static readonly INamedType	BindingType;
		public static readonly INamedType	TableType;
		public static readonly INamedType	StructuredType;
		public static readonly INumericType LocationNumericType;
		public static readonly INumericType SizeNumericType;
		public static readonly INumericType MarginNumericType;
		public static readonly INumericType GridNumericType;

		private Viewers.Panels				panel;
		private ObjectModifier				objectModifier;
		private List<Widget>				widgets;
		private List<IProxy>				proxies;
	}
}
