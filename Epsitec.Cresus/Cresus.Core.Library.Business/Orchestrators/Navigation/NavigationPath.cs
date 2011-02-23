//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
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
	public sealed class NavigationPath : System.IEquatable<NavigationPath>, IReadOnly
	{
		public NavigationPath()
		{
			this.elements = new List<NavigationPathElement> ();
		}

		public NavigationPath(NavigationPath path)
			: this ()
		{
			if (path != null)
			{
				this.AddRange (path.elements);
			}
		}

		
		public void Add(NavigationPathElement element)
		{
			if (element == null)
            {
				throw new System.ArgumentNullException ("element");
            }
			if (this.IsReadOnly)
            {
				throw new System.InvalidOperationException ("NavigationPath is read-only");
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

		/// <summary>
		/// Freezes this instance. It may no longer be modified.
		/// </summary>
		public void Freeze()
		{
			this.frozen = this.ToString ();
		}

		
		public override string ToString()
		{
			return this.frozen ?? string.Join (" / ", this.elements.Select (x => x.ToString ()).ToArray ());
		}

		public override bool Equals(object obj)
		{
			return this.Equals (obj as NavigationPath);
		}

		public override int GetHashCode()
		{
			return this.ToString ().GetHashCode ();
		}

		#region IEquatable<NavigationPath> Members

		public bool Equals(NavigationPath other)
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

		#region IReadOnly Members

		public bool IsReadOnly
		{
			get
			{
				return this.frozen != null;
			}
		}

		#endregion

		private readonly List<NavigationPathElement> elements;
		private string frozen;
	}
}