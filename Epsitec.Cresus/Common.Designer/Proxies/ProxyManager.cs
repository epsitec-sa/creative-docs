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
			IEnumerable<string> proxyNames = this.proxy.ProxyNames;
			foreach (string proxyName in proxyNames)
			{
				PossibleProxy proxy = new PossibleProxy();
				proxy.ProxyName = proxyName;
				proxy.ValueNames = this.proxy.ValueNames(proxyName);

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
					string proxyName = ProxyManager.SearchProxyName(possibleProxies, value.Name);
					System.Diagnostics.Debug.Assert(proxyName != null);

					if (!ProxyManager.IsExisting(proxiesToCreate, values, proxyName))
					{
						ProxyToCreate newProxy = new ProxyToCreate();
						newProxy.ProxyName = proxyName;
						newProxy.Values = values;

						proxiesToCreate.Add(newProxy);
					}
				}
			}

			//	Construit tous les panneaux pour les proxies.
			foreach (ProxyToCreate proxyToCreate in proxiesToCreate)
			{
				Widget panel = this.proxy.CreateInterface(box, proxyToCreate.ProxyName, proxyToCreate.Values);
				panel.Margins = new Margins(0, 0, 0, 5);
				panel.Dock = DockStyle.Top;
			}

			return box;
		}

		static protected string SearchProxyName(List<PossibleProxy> possibleProxies, string valueName)
		{
			//	Cherche le nom du proxy permettant de représenter une valeur.
			foreach (PossibleProxy proxy in possibleProxies)
			{
				foreach (string proxyValueName in proxy.ValueNames)
				{
					if (proxyValueName == valueName)
					{
						return proxy.ProxyName;
					}
				}
			}

			return null;
		}

		static protected bool IsExisting(List<ProxyToCreate> proxiesToCreate, List<AbstractValue> values, string proxyName)
		{
			//	Cherche s'il existe déjà un proxy avec exactement les mêmes valeurs.
			foreach (ProxyToCreate proxyToCreate in proxiesToCreate)
			{
				if (proxyToCreate.ProxyName == proxyName)
				{
					if (ProxyManager.IsEqual(proxyToCreate, values))
					{
						return true;
					}
				}
			}

			return false;
		}

		static protected bool IsEqual(ProxyToCreate proxyToCreate, List<AbstractValue> values)
		{
			//	Cherche si un proxy contient exactement les mêmes valeurs. Le proxy peut contenir plus de valeurs
			//	la liste cherchée, mais toutes les valeurs de la liste cherchée doivent exister et être identiques
			//	dans le proxy.
			foreach (AbstractValue value in values)
			{
				bool equal = false;
				foreach (AbstractValue valueToCreate in proxyToCreate.Values)
				{
					if (valueToCreate.Name == value.Name && valueToCreate.Value.Equals(value.Value))
					{
						equal = true;
					}
				}

				if (!equal)
				{
					return false;
				}
			}

			return true;
		}


		protected class PossibleProxy
		{
			public string ProxyName;
			public IEnumerable<string> ValueNames;
		}

		protected class ProxyToCreate
		{
			public string ProxyName;
			public List<AbstractValue> Values;
		}


		protected AbstractObjectManager objectManager;
		protected AbstractProxy proxy;
	}
}
