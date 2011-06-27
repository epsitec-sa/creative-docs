//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Orchestrators.Navigation
{
	/// <summary>
	/// The <c>TileNavigationPathElement</c> class implements a navigation path element associated
	/// to a clickable tile.
	/// </summary>
	public class TileNavigationPathElement : NavigationPathElement
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TileNavigationPathElement"/> class.
		/// </summary>
		/// <param name="name">The name of the UI element.</param>
		public TileNavigationPathElement(string name)
		{
			this.name = name;
		}


		/// <summary>
		/// Navigates to this element using the specified navigator.
		/// </summary>
		/// <param name="navigator">The navigator.</param>
		/// <returns>
		/// 	<c>true</c> if navigation succeeded; otherwise, <c>false</c>.
		/// </returns>
		public override bool Navigate(Orchestrators.NavigationOrchestrator navigator)
		{
			return navigator.GetLeafClickSimulator ().SimulateClick (this.name);
		}

		public override string ToString()
		{
			return string.Concat ("<Tile:", this.name, ">");
		}

		
		private readonly string name;
	}
}