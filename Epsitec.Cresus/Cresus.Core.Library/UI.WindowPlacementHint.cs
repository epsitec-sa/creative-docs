//	Copyright © 2008-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Core.Library
{
	public static partial class UI
	{
		/// <summary>
		/// The <c>WindowPlacementHint</c> structure records the placement
		/// information for a window identified by its name and title.
		/// </summary>
		private struct WindowPlacementHint : System.IEquatable<WindowPlacementHint>
		{
			public WindowPlacementHint(string name, string title, WindowPlacement placement)
			{
				this.name = name ?? "";
				this.title = title ?? "";
				this.placement = placement;
			}

			public string						Name
			{
				get
				{
					return this.name;
				}
			}

			public string						Title
			{
				get
				{
					return this.title;
				}
			}

			public WindowPlacement				Placement
			{
				get
				{
					return this.placement;
				}
			}

			public bool							IsEmpty
			{
				get
				{
					return this.name == null && this.title == null;
				}
			}

			public static readonly WindowPlacementHint Empty = new WindowPlacementHint ();

			#region IEquatable<WindowPlacementHint> Members

			public bool Equals(WindowPlacementHint other)
			{
				return (this.name == other.name) && (this.title == other.title) && (this.placement.Equals (other.placement));
			}

			#endregion

			public override bool Equals(object obj)
			{
				if (obj is WindowPlacementHint)
				{
					return this.Equals ((WindowPlacementHint) obj);
				}
				else
				{
					return false;
				}
			}

			public override int GetHashCode()
			{
				return (this.name == null ? 0 : this.name.GetHashCode ())
					^ (this.title == null ? 0 : this.title.GetHashCode ());
			}

			private readonly string				name;
			private readonly string				title;
			private readonly WindowPlacement	placement;
		}
	}
}
