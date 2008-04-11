using System.Collections.Generic;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Designer.Proxies
{
	/// <summary>
	/// Cette classe gère les proxies.
	/// </summary>
	public class ProxyManager
	{
		public ProxyManager(AbstractObjectManager objectManager, AbstractProxy proxy)
		{
			this.objectManager = objectManager;
			this.proxy = proxy;
		}

		public Widget CreateInterface(Widget parent, List<Widget> selectedObjects)
		{
			//	Crée tous les panneaux permettant de modifier les propriétés d'une liste d'objets sélectionnés.
			FrameBox box = new FrameBox(parent);
			box.Dock = DockStyle.Fill;

			//	Construit la liste de tous les proxies possibles.
			List<PossibleProxy> possibleProxies = new List<PossibleProxy>();
			IEnumerable<AbstractProxy.Panel> proxyPanels = this.proxy.ProxyPanels;
			foreach (AbstractProxy.Panel proxyPanel in proxyPanels)
			{
				PossibleProxy proxy = new PossibleProxy();
				proxy.ProxyPanel = proxyPanel;
				proxy.ValueTypes = this.proxy.ValueTypes(proxyPanel);

				possibleProxies.Add(proxy);
			}

			List<ProxyToCreate> proxiesToCreate = new List<ProxyToCreate>();

			//	Passe en revue tous les objets sélectionnés.
			foreach (Widget selectedObject in selectedObjects)
			{
				List<AbstractValue> values = this.objectManager.GetValues(selectedObject);

				//	Passe en revue les valeurs de chaque objet sélectionné.
				foreach (AbstractValue value in values)
				{
					AbstractProxy.Panel proxyPanel = ProxyManager.SearchProxyPanel(possibleProxies, value.Type);
					if (proxyPanel == AbstractProxy.Panel.None)
					{
						//?throw new System.NotImplementedException();
					}
					else
					{
						int index = ProxyManager.IndexOf(possibleProxies, proxiesToCreate, values, proxyPanel);
						if (index == -1)
						{
							ProxyToCreate newProxy = new ProxyToCreate();
							newProxy.ProxyPanel = proxyPanel;
							newProxy.Values = values;

							proxiesToCreate.Add(newProxy);
						}
						else
						{
							ProxyManager.Merge(possibleProxies, proxiesToCreate[index], values, proxyPanel);
						}
					}
				}
			}

			//	Construit tous les panneaux pour les proxies.
			proxiesToCreate.Sort();  // trie les panneaux à créer
			this.proxies = proxiesToCreate;

			AbstractProxy.Panel lastPanel = AbstractProxy.Panel.None;

			foreach (ProxyToCreate proxyToCreate in proxiesToCreate)
			{
				double space = (lastPanel == AbstractProxy.Panel.None) ? 0 : 5;

				if (proxyToCreate.ProxyPanel == lastPanel)
				{
					space = -1;  // collé au panneau précédent
				}

				Widget panel = this.proxy.CreateInterface(box, proxyToCreate.ProxyPanel, proxyToCreate.Values);
				panel.Margins = new Margins(0, 0, space, 0);
				panel.Dock = DockStyle.Top;

				lastPanel = proxyToCreate.ProxyPanel;
			}

			this.UpdateInterface();

			return box;
		}

		public void UpdateInterface()
		{
			//	Met à jour toutes les valeurs dans les panneaux des proxies, ainsi que l'état enable/disable.
			foreach (ProxyToCreate proxy in this.proxies)  // passe en revue tous les panneaux
			{
				foreach (AbstractValue value in proxy.Values)  // passe en revue toutes les valeurs d'un panneau
				{
					this.objectManager.SendObjectToValue(value);  // met à jour la valeur

					if (value.WidgetInterface != null)
					{
						value.WidgetInterface.Enable = this.objectManager.IsEnable(value);  // met à jour l'état enable/disable
					}
				}
			}
		}


		static protected int IndexOf(List<PossibleProxy> possibleProxies, List<ProxyToCreate> proxiesToCreate, List<AbstractValue> values, AbstractProxy.Panel proxyPanel)
		{
			//	Cherche l'index d'un proxy ayant exactement les mêmes valeurs.
			for (int i=0; i<proxiesToCreate.Count; i++)
			{
				ProxyToCreate proxyToCreate = proxiesToCreate[i];

				if (proxyToCreate.ProxyPanel == proxyPanel)
				{
					if (ProxyManager.IsEqual(possibleProxies, proxyToCreate, values, proxyPanel))
					{
						return i;
					}
				}
			}

			return -1;
		}

		static protected bool IsEqual(List<PossibleProxy> possibleProxies, ProxyToCreate proxyToCreate, List<AbstractValue> values, AbstractProxy.Panel proxyPanel)
		{
			//	Cherche si un proxy contient exactement les mêmes valeurs. Le proxy peut contenir plus de valeurs
			//	que la liste cherchée, mais toutes les valeurs de la liste cherchée doivent exister et être identiques
			//	dans le proxy.
			foreach (AbstractValue value in values)
			{
				AbstractProxy.Panel valueProxyPanel = ProxyManager.SearchProxyPanel(possibleProxies, value.Type);
				if (valueProxyPanel == proxyPanel)
				{
					bool equal = false;
					foreach (AbstractValue valueToCreate in proxyToCreate.Values)
					{
						if (valueToCreate.Type == value.Type && valueToCreate.Value.Equals(value.Value))
						{
							equal = true;
						}
					}

					if (!equal)
					{
						return false;
					}
				}
			}

			return true;
		}

		static protected void Merge(List<PossibleProxy> possibleProxies, ProxyToCreate proxyToCreate, List<AbstractValue> values, AbstractProxy.Panel proxyPanel)
		{
			//	Fusionne les AbstractValue.SelectedObjects contenus dans 'values' aux listes de 'proxyToCreate'.
			foreach (AbstractValue value in values)
			{
				AbstractProxy.Panel valueProxyPanel = ProxyManager.SearchProxyPanel(possibleProxies, value.Type);
				if (valueProxyPanel == proxyPanel)
				{
					foreach (AbstractValue valueToCreate in proxyToCreate.Values)
					{
						if (valueToCreate.Type == value.Type && valueToCreate.Value.Equals(value.Value))
						{
							foreach (Widget widget in value.SelectedObjects)
							{
								if (!valueToCreate.SelectedObjects.Contains(widget))
								{
									valueToCreate.SelectedObjects.Add(widget);
								}
							}
						}
					}
				}
			}
		}

		static protected AbstractProxy.Panel SearchProxyPanel(List<PossibleProxy> possibleProxies, AbstractProxy.Type valueType)
		{
			//	Cherche le nom du proxy permettant de représenter une valeur.
			foreach (PossibleProxy proxy in possibleProxies)
			{
				foreach (AbstractProxy.Type proxyValueType in proxy.ValueTypes)
				{
					if (proxyValueType == valueType)
					{
						return proxy.ProxyPanel;
					}
				}
			}

			return AbstractProxy.Panel.None;
		}


		protected class PossibleProxy
		{
			public AbstractProxy.Panel ProxyPanel;
			public IEnumerable<AbstractProxy.Type> ValueTypes;
		}

		protected class ProxyToCreate : System.IComparable
		{
			public AbstractProxy.Panel ProxyPanel;
			public List<AbstractValue> Values;

			#region IComparable Members
			public int CompareTo(object obj)
			{
				//	Compare les types. Utilisé lors d'un tri.
				ProxyToCreate that = obj as ProxyToCreate;
				return this.ProxyPanel.CompareTo(that.ProxyPanel);
			}
			#endregion
		}


		protected AbstractObjectManager objectManager;
		protected AbstractProxy proxy;
		protected List<ProxyToCreate> proxies;
	}
}
