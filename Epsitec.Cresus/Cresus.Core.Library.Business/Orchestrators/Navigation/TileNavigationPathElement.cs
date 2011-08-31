//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Orchestrators.Navigation;

using System.Collections.Generic;
using System.Linq;

[assembly:NavigationPathElementClass ("Tile", typeof (TileNavigationPathElement))]

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

		private TileNavigationPathElement()
		{
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

		protected override NavigationPathElement Deserialize(string data)
		{
			if (data.StartsWith (TileNavigationPathElement.ClassIdPrefix))
			{
				return new TileNavigationPathElement (data.Substring (TileNavigationPathElement.ClassIdPrefix.Length));
			}

			throw new System.FormatException (string.Format ("Invalid format; expected prefix '{0}'", TileNavigationPathElement.ClassIdPrefix));			
		}

		protected override string Serialize()
		{
			return string.Concat (TileNavigationPathElement.ClassIdPrefix, this.name);
		}


		private const string					ClassIdPrefix = "Tile:";
		
		private readonly string					name;
	}
}