//	Copyright � 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using System.Collections.Generic;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Widgets.Collections.ShortcutCollection))]

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
	}
}
