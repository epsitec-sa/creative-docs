//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Orchestrators.Navigation
{
	/// <summary>
	/// The <c>NavigationPathElement</c> is the base class used to record items into the
	/// navigation  history, building a list of elements leading to the active controller.
	/// </summary>
	public abstract class NavigationPathElement : System.IEquatable<NavigationPathElement>
	{
		protected NavigationPathElement ()
		{
		}

		/// <summary>
		/// Navigates to this element using the specified navigator.
		/// </summary>
		/// <param name="navigator">The navigator.</param>
		/// <returns><c>true</c> if navigation succeeded; otherwise, <c>false</c>.</returns>
		public virtual bool Navigate(NavigationOrchestrator navigator)
		{
			return false;
		}

		#region IEquatable<NavigationPathElement> Members

		public bool Equals(NavigationPathElement other)
		{
			if (other == null)
			{
				return false;
			}
			else
			{
				return this.ToString () == other.ToString ();
			}
		}

		#endregion
	}
}
