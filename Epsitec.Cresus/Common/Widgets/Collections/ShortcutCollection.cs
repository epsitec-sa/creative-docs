//	Copyright © 2005-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Collections
{
	/// <summary>
	/// The <c>ShortcutCollection</c> class represents a list of <see cref="Shortcut"/>
	/// items.
	/// </summary>
	public class ShortcutCollection : Types.Collections.HostedDependencyObjectList<Shortcut>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ShortcutCollection"/> class.
		/// </summary>
		public ShortcutCollection()
			: base (null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ShortcutCollection"/> class.
		/// </summary>
		/// <param name="host">The host which must be notified.</param>
		public ShortcutCollection(IListHost<Shortcut> host)
			: base (host)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ShortcutCollection"/> class.
		/// </summary>
		/// <param name="insertionCallback">The insertion callback.</param>
		/// <param name="removalCallback">The removal callback.</param>
		public ShortcutCollection(Callback insertionCallback, Callback removalCallback)
			: base (insertionCallback, removalCallback)
		{
		}


		/// <summary>
		/// Defines the specified shortcut as the only active shortcut in
		/// the collection.
		/// </summary>
		/// <param name="value">The shortcut.</param>
		public void Define(Shortcut value)
		{
			if (this.Count == 1)
			{
				Shortcut old = this[0];
				
				if (old.Equals (value))
				{
					return;
				}
			}

			this.Clear ();
			this.Add (value);
		}

		/// <summary>
		/// Defines the specified shortcuts as the only active shortcuts
		/// in the collection.
		/// </summary>
		/// <param name="values">The shortcuts.</param>
		public void Define(params Shortcut[] values)
		{
			if (((values == null) || (values.Length == 0)) &&
				(this.Count == 0))
			{
				return;
			}
			
			if ((values != null) &&
				(this.Count == values.Length))
			{
				int same = 0;
				
				for (int i = 0; i < values.Length; i++)
				{
					if (this[i].Equals (values[i]))
					{
						same++;
					}
				}
				
				if (same == values.Length)
				{
					return;
				}
			}
			
			this.Clear ();
			
			if ((values != null) &&
				(values.Length > 0))
			{
				this.AddRange (values);
			}
		}

		/// <summary>
		/// Inserts the specified shortcut if it is not yet in the collection.
		/// Empty shortcuts will be ignored.
		/// </summary>
		/// <param name="shortcut">The shortcut.</param>
		public override void Insert(int index, Shortcut shortcut)
		{
			if ((this.Contains (shortcut)) &&
				(! shortcut.IsEmpty))
			{
				//	Don't add twice... unless the shortcut is empty, in which case
				//	we might be currently processing a deserialization with partially
				//	initialized shortcuts.
			}
			else
			{
				base.Insert (index, shortcut);
			}
		}
	}
}
