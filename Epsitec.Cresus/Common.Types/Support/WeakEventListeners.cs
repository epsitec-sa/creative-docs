//	Copyright © 2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	public struct WeakEventListeners
	{
		public void Add(System.Delegate listener)
		{
			if (this.listeners == null)
			{
				this.listeners = new List<WeakEventListener> ();
			}

			this.listeners.Add (new WeakEventListener (listener));
		}

		public void Remove(System.Delegate listener)
		{
			if (this.listeners != null)
			{
				this.listeners.RemoveAll (e => e.IsDead || e.Equals (listener));
			}
		}

		public void Invoke(params object[] parameters)
		{
			WeakEventListener[] temp = this.listeners.ToArray ();

			foreach (WeakEventListener listener in temp)
			{
				if (listener.Invoke (parameters) == false)
				{
					this.listeners.Remove (listener);
				}
			}
		}


		private List<WeakEventListener> listeners;
	}
}
