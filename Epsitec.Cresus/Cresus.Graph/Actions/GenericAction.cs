//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.Actions
{
	public class GenericAction<T> : Action
	{
		public GenericAction(System.Action<T> action)
			: base (action)
		{
		}

		public void Invoke(T arg)
		{
			Recorder.Push (this, arg).PlayBack ();
		}

		public static implicit operator System.Action<T>(GenericAction<T> action)
		{
			return action.Invoke;
		}
	}

	public class GenericAction : Action
	{
		public GenericAction(System.Action action)
			: base (action)
		{
		}

		public void Invoke()
		{
			Recorder.Push (this).PlayBack ();
		}

		public static implicit operator System.Action(GenericAction action)
		{
			return action.Invoke;
		}
	}
}
