//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Lorsque plusieurs popups sont empilés, il faut en garder la trace, essentiellement
	/// pour savoir celui qui est actif à l'avant-plan.
	/// </summary>
	public static class PopupStack
	{
		public static bool						HasPopup
		{
			get
			{
				return PopupStack.stack.Any ();
			}
		}

		public static void Push(AbstractPopup popup)
		{
			PopupStack.stack.Push (popup);
		}

		public static AbstractPopup Pop()
		{
			return PopupStack.stack.Pop ();
		}

		public static AbstractPopup Top()
		{
			//	Retourne le popup à l'avant-plan.
			if (PopupStack.stack.Any ())
			{
				return PopupStack.stack.Peek ();
			}
			else
			{
				return null;
			}
		}

		public static bool IsOnTop(AbstractPopup popup)
		{
			//	Indique si un popup donné est à l'avant-plan.
			return PopupStack.stack.Any () && popup == PopupStack.stack.Peek ();
		}


		private static Stack<AbstractPopup> stack = new Stack<AbstractPopup> ();
	}
}