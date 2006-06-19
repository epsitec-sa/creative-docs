//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	public class Weak<T> : System.WeakReference
		where T : class
	{
		public Weak(T target)
			: base (target)
		{
		}

		public new T Target
		{
			get
			{
				return base.Target as T;
			}
		}
	}
}
