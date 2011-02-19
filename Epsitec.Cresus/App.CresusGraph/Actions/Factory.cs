//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.Actions
{
	/// <summary>
	/// The <c>Factory</c> class implements an action factory, which is used
	/// to map between C# action delegates and <see cref="Action"/> instances.
	/// </summary>
	public static class Factory
	{
		/// <summary>
		/// Registers a new action.
		/// </summary>
		/// <param name="action">The action.</param>
		/// <returns>The <see cref="Action"/> instance.</returns>
		public static GenericAction<IEnumerable<int>> New(System.Action<IEnumerable<int>> action)
		{
			var x = new GenericAction<IEnumerable<int>> (action);
			Factory.RegisterUserAction (x);
			return x;
		}

		public static GenericAction<string> New(System.Action<string> action)
		{
			var x = new GenericAction<string> (action);
			Factory.RegisterUserAction (x);
			return x;
		}

		public static GenericAction<T1, T2> New<T1, T2>(System.Action<T1, T2> action)
		{
			var x = new GenericAction<T1, T2> (action);
			Factory.RegisterUserAction (x);
			return x;
		}

		/// <summary>
		/// Registers a new action.
		/// </summary>
		/// <param name="action">The action.</param>
		/// <returns>The <see cref="Action"/> instance.</returns>
		public static GenericAction New(System.Action action)
		{
			var x = new GenericAction (action);
			Factory.RegisterUserAction (x);
			return x;
		}

		/// <summary>
		/// Finds the specified action based on its tag.
		/// </summary>
		/// <param name="tag">The action tag.</param>
		/// <returns>The <see cref="Action"/> instance or <c>null</c>.</returns>
		public static Action Find(string tag)
		{
			Action action;

			if (Factory.actions.TryGetValue (tag, out action))
			{
				return action;
			}
			else
			{
				return null;
			}
		}


		/// <summary>
		/// Registers the specified action.
		/// </summary>
		/// <param name="action">The action.</param>
		private static void RegisterUserAction(Action action)
		{
			System.Diagnostics.Debug.Assert (Factory.actions.ContainsKey (action.Tag) == false);

			Factory.actions[action.Tag] = action;
		}


		private static readonly Dictionary<string, Action> actions = new Dictionary<string, Action> ();
	}
}
