using System.Collections.Generic;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Designer.Proxies
{
	/// <summary>
	/// Cette classe permet...
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
					System.Diagnostics.Debug.Assert(proxyPanel != AbstractProxy.Panel.None);

					if (ProxyManager.IsExisting(possibleProxies, proxiesToCreate, values, proxyPanel))
					{
						//	TODO: fusionne les AbstractValue.SelectedObjects !
					}
					else
					{
						ProxyToCreate newProxy = new ProxyToCreate();
						newProxy.ProxyPanel = proxyPanel;
						newProxy.Values = values;

						proxiesToCreate.Add(newProxy);
					}
				}
			}

			//	Construit tous les panneaux pour les proxies.
			foreach (ProxyToCreate proxyToCreate in proxiesToCreate)
			{
				Widget panel = this.proxy.CreateInterface(box, proxyToCreate.ProxyPanel, proxyToCreate.Values);
				panel.Margins = new Margins(0, 0, 0, 5);
				panel.Dock = DockStyle.Top;
			}

			return box;
		}

		static protected bool IsExisting(List<PossibleProxy> possibleProxies, List<ProxyToCreate> proxiesToCreate, List<AbstractValue> values, AbstractProxy.Panel proxyPanel)
		{
			//	Cherche s'il existe déjà un proxy avec exactement les mêmes valeurs.
			foreach (ProxyToCreate proxyToCreate in proxiesToCreate)
			{
				if (proxyToCreate.ProxyPanel == proxyPanel)
				{
					if (ProxyManager.IsEqual(possibleProxies, proxyToCreate, values, proxyPanel))
					{
						return true;
					}
				}
			}

			return false;
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

		protected class ProxyToCreate
		{
			public AbstractProxy.Panel ProxyPanel;
			public List<AbstractValue> Values;
		}


		protected AbstractObjectManager objectManager;
		protected AbstractProxy proxy;
	}
}
