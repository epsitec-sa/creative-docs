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
			IEnumerable<AbstractObjectManager.Type> proxyTypes = this.proxy.ProxyTypes;
			foreach (AbstractObjectManager.Type proxyType in proxyTypes)
			{
				PossibleProxy proxy = new PossibleProxy();
				proxy.ProxyType = proxyType;
				proxy.ValueTypes = this.proxy.ValueTypes(proxyType);

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
					AbstractObjectManager.Type proxyType = ProxyManager.SearchProxyType(possibleProxies, value.Type);
					System.Diagnostics.Debug.Assert(proxyType == AbstractObjectManager.Type.None);

					if (ProxyManager.IsExisting(possibleProxies, proxiesToCreate, values, proxyType))
					{
						//	TODO: fusionne les AbstractValue.SelectedObjects !
					}
					else
					{
						ProxyToCreate newProxy = new ProxyToCreate();
						newProxy.ProxyType = proxyType;
						newProxy.Values = values;

						proxiesToCreate.Add(newProxy);
					}
				}
			}

			//	Construit tous les panneaux pour les proxies.
			foreach (ProxyToCreate proxyToCreate in proxiesToCreate)
			{
				Widget panel = this.proxy.CreateInterface(box, proxyToCreate.ProxyType, proxyToCreate.Values);
				panel.Margins = new Margins(0, 0, 0, 5);
				panel.Dock = DockStyle.Top;
			}

			return box;
		}

		static protected bool IsExisting(List<PossibleProxy> possibleProxies, List<ProxyToCreate> proxiesToCreate, List<AbstractValue> values, AbstractObjectManager.Type proxyType)
		{
			//	Cherche s'il existe déjà un proxy avec exactement les mêmes valeurs.
			foreach (ProxyToCreate proxyToCreate in proxiesToCreate)
			{
				if (proxyToCreate.ProxyType == proxyType)
				{
					if (ProxyManager.IsEqual(possibleProxies, proxyToCreate, values, proxyType))
					{
						return true;
					}
				}
			}

			return false;
		}

		static protected bool IsEqual(List<PossibleProxy> possibleProxies, ProxyToCreate proxyToCreate, List<AbstractValue> values, AbstractObjectManager.Type proxyType)
		{
			//	Cherche si un proxy contient exactement les mêmes valeurs. Le proxy peut contenir plus de valeurs
			//	que la liste cherchée, mais toutes les valeurs de la liste cherchée doivent exister et être identiques
			//	dans le proxy.
			foreach (AbstractValue value in values)
			{
				AbstractObjectManager.Type valueProxyType = ProxyManager.SearchProxyType(possibleProxies, value.Type);
				if (valueProxyType == proxyType)
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

		static protected AbstractObjectManager.Type SearchProxyType(List<PossibleProxy> possibleProxies, AbstractObjectManager.Type valueType)
		{
			//	Cherche le nom du proxy permettant de représenter une valeur.
			foreach (PossibleProxy proxy in possibleProxies)
			{
				foreach (AbstractObjectManager.Type proxyValueType in proxy.ValueTypes)
				{
					if (proxyValueType == valueType)
					{
						return proxy.ProxyType;
					}
				}
			}

			return AbstractObjectManager.Type.None;
		}


		protected class PossibleProxy
		{
			public AbstractObjectManager.Type ProxyType;
			public IEnumerable<AbstractObjectManager.Type> ValueTypes;
		}

		protected class ProxyToCreate
		{
			public AbstractObjectManager.Type ProxyType;
			public List<AbstractValue> Values;
		}


		protected AbstractObjectManager objectManager;
		protected AbstractProxy proxy;
	}
}
