//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Collections
{
	public class ShortcutCollection : Types.Collections.HostedDependencyObjectList<Shortcut>
	{
		public ShortcutCollection()
			: base (null)
		{
		}

		public ShortcutCollection(IListHost<Shortcut> host)
			: base (host)
		{
		}

		public ShortcutCollection(Callback insertionCallback, Callback removalCallback)
			: base (insertionCallback, removalCallback)
		{
		}
		
		
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
		
		public void Define(Shortcut[] values)
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

		public override void Add(Shortcut shortcut)
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
				base.Add (shortcut);
			}
		}
	}
}
