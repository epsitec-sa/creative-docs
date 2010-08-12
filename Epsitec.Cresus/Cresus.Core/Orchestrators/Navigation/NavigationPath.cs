//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types.Collections;
using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Orchestrators.Navigation
{
	public class NavigationPath
	{
		public NavigationPath()
		{
			this.elements = new List<NavigationPathElement> ();
		}

		
		public void Add(NavigationPathElement element)
		{
			this.elements.Add (element);
		}

		public void AddRange(IEnumerable<NavigationPathElement> collection)
		{
			this.elements.AddRange (collection);
		}


		public bool Navigate(NavigationOrchestrator navigator)
		{
			return this.elements.All (x => x.Navigate (navigator));
		}

		public override string ToString()
		{
			return string.Join (" / ", this.elements.Select (x => x.ToString ()).ToArray ());
		}

		
		private readonly List<NavigationPathElement> elements;
	}
}
