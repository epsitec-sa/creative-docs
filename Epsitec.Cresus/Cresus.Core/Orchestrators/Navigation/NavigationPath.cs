//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types.Collections;
using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Orchestrators.Navigation
{
	/// <summary>
	/// The <c>NavigationPath</c> class represents a path leading to a given
	/// UI element. It is used by the navigation history to record which UI
	/// elements were previously active.
	/// </summary>
	public sealed class NavigationPath
	{
		public NavigationPath()
		{
			this.elements = new List<NavigationPathElement> ();
		}

		
		public void Add(NavigationPathElement element)
		{
			if (element == null)
            {
				throw new System.ArgumentNullException ("element");
            }

			this.elements.Add (element);
		}

		public void AddRange(IEnumerable<NavigationPathElement> collection)
		{
			if (collection != null)
			{
				collection.ForEach (item => this.Add (item));
			}
		}


		/// <summary>
		/// Navigates to the recorded UI element, using the specified navigator.
		/// </summary>
		/// <param name="navigator">The navigator.</param>
		/// <returns><c>true</c> if the navigation was successful; otherwise, <c>false</c>.</returns>
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