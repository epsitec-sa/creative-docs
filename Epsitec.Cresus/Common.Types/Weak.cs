//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
