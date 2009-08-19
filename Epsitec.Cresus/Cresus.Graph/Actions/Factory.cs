//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.Actions
{
	public static class Factory
	{
		public static GenericAction<IEnumerable<int>> New(System.Action<IEnumerable<int>> action)
		{
			return new GenericAction<IEnumerable<int>> (action);
		}

		public static GenericAction New(System.Action action)
		{
			return new GenericAction (action);
		}

		public static void RegisterUserAction(Action action)
		{
			System.Diagnostics.Debug.Assert (Factory.actions.ContainsKey (action.Tag) == false);

			Factory.actions[action.Tag] = action;
		}

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


		private static readonly Dictionary<string, Action> actions = new Dictionary<string, Action> ();
	}
}
