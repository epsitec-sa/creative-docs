using System.Collections.Generic;

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Designer
{
	public sealed class ProxyManager
	{
		public ProxyManager()
		{
		}

		public void SetSelection(Widget widget)
		{
			this.widgets = new List<Widget> ();
			this.widgets.Add (widget);
			this.GenerateProxies ();
		}
		
		public void SetSelection(IEnumerable<Widget> collection)
		{
			this.widgets = new List<Widget> ();
			this.widgets.AddRange (collection);
			this.GenerateProxies ();
		}

		public IEnumerable<Proxies.Abstract> GetProxies()
		{
			return this.proxies;
		}

		private void GenerateProxies()
		{
			List<Proxies.Abstract> proxies = new List<Proxies.Abstract> ();

			foreach (Widget widget in this.widgets)
			{
				foreach (Proxies.Abstract proxy in this.GenerateProxies (widget))
				{
					//	Evite les doublons pour des proxies qui seraient à 100%
					//	identiques:

					bool insert = true;

					foreach (Proxies.Abstract item in proxies)
					{
						if (DependencyObject.EqualValues (item, proxy))
						{
							//	Trouvé un doublon. On ajoute simplement le widget
							//	courant au proxy qui existe déjà avec les mêmes
							//	valeurs :

							item.AddWidget (widget);
							insert = false;
							break;
						}
					}

					if (insert)
					{
						proxies.Add (proxy);
					}
				}
			}

			this.proxies = proxies;
		}

		private IEnumerable<Proxies.Abstract> GenerateProxies(Widget widget)
		{
			//	TODO: créer les divers Proxies pour le widget; on peut simplement
			//	ajouter ici des 'yield return new ...'

			yield return new Proxies.Geometry (widget);
		}

		List<Widget> widgets;
		List<Proxies.Abstract> proxies;
	}
}
