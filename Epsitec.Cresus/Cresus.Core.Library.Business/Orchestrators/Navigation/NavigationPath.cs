//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Orchestrators.Navigation
{
	/// <summary>
	/// The <c>NavigationPath</c> class represents a path leading to a given
	/// UI element. It is used by the navigation history to record which UI
	/// elements were previously active.
	/// </summary>
	public sealed class NavigationPath : System.IEquatable<NavigationPath>, IReadOnly, IEnumerable<NavigationPathElement>
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

		public NavigationPath(IEnumerable<NavigationPathElement> collection)
			: this ()
		{
			this.AddRange (collection);
		}


		public NavigationPathElement			Root
		{
			get
			{
				if (this.elements.Count > 0)
				{
					return this.elements[0];
				}
				else
				{
					return null;
				}
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

		#region IEnumerable<NavigationPathElement> Members

		public IEnumerator<NavigationPathElement> GetEnumerator()
		{
			return this.elements.GetEnumerator ();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.elements.GetEnumerator ();
		}

		#endregion

		public static NavigationPath CreateTileNavigationPath(NavigationPathElement root, params AbstractEntity[] entities)
		{
			NavigationPath path = new NavigationPath ();

			path.Add (root);

			foreach (var entity in entities)
			{
				path.Add (NavigationPath.CreateTileNavigationPathElement (entity));
			}

			return path;
		}

		/// <summary>
		/// Creates a tile navigation path element based on an entity. This will use a special name
		/// (such as <c>entity:123</c>) to describe the target tile.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns></returns>
		public static NavigationPathElement CreateTileNavigationPathElement(AbstractEntity entity)
		{
			return NavigationPath.CreateTileNavigationPathElement ("entity:" + InvariantConverter.ToString (entity.GetEntitySerialId ()));
		}

		/// <summary>
		/// Gets the entity serial ID from the name of a tile navigation path element.
		/// </summary>
		/// <param name="name">The name (like <c>entity:123</c>).</param>
		/// <returns>The entity serial ID or <c>null</c> if the name does not map to an entity.</returns>
		public static long? GetEntitySerialIdFromTileNavigationPathElementName(string name)
		{
			if (name.StartsWith ("entity:"))
			{
				return InvariantConverter.ParseLong (name.RemoveFirstToken ("entity:"));
			}
			else
			{
				return null;
			}
		}
		
		public static NavigationPathElement CreateTileNavigationPathElement(string name)
		{
			return NavigationPathElementClassFactory.Parse (string.Concat ("<", "Tile:", name, ">"));
		}
		
		public static NavigationPath Parse(string value)
		{
			string[] tokens = value.Split (" / ");
			return new NavigationPath (tokens.Select (x => NavigationPathElementClassFactory.Parse (x)));
		}

		private readonly List<NavigationPathElement> elements;
		private string frozen;
	}
}