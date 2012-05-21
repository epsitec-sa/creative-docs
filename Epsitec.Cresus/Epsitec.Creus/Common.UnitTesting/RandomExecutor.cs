using Epsitec.Common.Support.Extensions;

using System;

using System.Linq;


namespace Epsitec.Common.UnitTesting
{


	public static class RandomExecutor
	{


		public static void ExecuteRandomly(int nbExecutions, params Action[] actions)
		{
			nbExecutions.ThrowIf (i => i < 1, "nbExecutions cannot be smaller than one");
			actions.ThrowIfNull ("actions");
			actions.ThrowIf (a => a.Length == 0, "actions must contain at least one element");
			actions.ThrowIf (a => a.Any (e => e == null), "actions cannot contain null elements");

			var randomSeed = Guid.NewGuid ().GetHashCode ();
			var dice = new Random (randomSeed);

			for (int i = 0; i < nbExecutions; i++)
			{
				var selectionActionIndex = dice.Next (actions.Length);
				var selectedAction = actions[selectionActionIndex];

				selectedAction ();
			}
		}


		public static void ExecuteRandomly(this Action[] actions, int nbExecutions)
		{
			RandomExecutor.ExecuteRandomly (nbExecutions, actions);
		}


	}


}
