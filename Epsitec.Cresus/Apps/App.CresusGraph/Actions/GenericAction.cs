//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.Actions
{
	/// <summary>
	/// The <c>GenericAction{T}</c> class wraps <see cref="System.Action{T}"/>. Its
	/// instances can be used as action delegates: when they execute, they get pushed
	/// onto the undo stack.
	/// </summary>
	/// <typeparam name="T">The argument type.</typeparam>
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

	/// <summary>
	/// The <c>GenericAction{T1,T2}</c> class wraps <see cref="System.Action{T1,T2}"/>. Its
	/// instances can be used as action delegates: when they execute, they get pushed
	/// onto the undo stack.
	/// </summary>
	/// <typeparam name="T">The argument type.</typeparam>
	public class GenericAction<T1, T2> : Action
	{
		public GenericAction(System.Action<T1, T2> action)
			: base (action)
		{
		}

		public void Invoke(T1 arg1, T2 arg2)
		{
			Recorder.Push (this, arg1, arg2).PlayBack ();
		}

		public static implicit operator System.Action<T1, T2>(GenericAction<T1, T2> action)
		{
			return action.Invoke;
		}
	}

	/// <summary>
	/// The <c>GenericAction</c> class wraps <see cref="System.Action"/>. Its
	/// instances can be used as action delegates: when they execute, they get pushed
	/// onto the undo stack.
	/// </summary>
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
