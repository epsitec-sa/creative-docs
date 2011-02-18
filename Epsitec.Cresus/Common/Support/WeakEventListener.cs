//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	internal struct WeakEventListener : System.IEquatable<WeakEventListener>
	{
		public WeakEventListener(System.Delegate listener)
		{
			this.info = listener.Method;
			this.target = new Weak<object> (listener.Target);
		}

		public bool IsDead
		{
			get
			{
				return this.target.IsAlive == false;
			}
		}

		public bool Equals(System.Delegate listener)
		{
			return listener.Method == this.info
					&& listener.Target == this.target.Target;
		}

		public bool Invoke(params object[] parameters)
		{
			object target = this.target.Target;

			if (target == null)
			{
				return false;
			}
			else
			{
				this.info.Invoke (target, parameters);
				return true;
			}
		}

		#region IEquatable<WeakEventListener> Members

		public bool Equals(WeakEventListener other)
		{
			return this.info == other.info
				&& this.target.Target == other.target.Target;
		}

		#endregion

		public override bool Equals(object obj)
		{
			if (obj is WeakEventListener)
			{
				return this.Equals ((WeakEventListener) obj);
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return this.info.GetHashCode ();
		}

		public System.Reflection.MethodInfo info;
		public Weak<object> target;
	}
}
