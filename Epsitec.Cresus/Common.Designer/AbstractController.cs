using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using System.Globalization;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// La classe AbstractController sert de base pour tous les contrôleurs.
	/// </summary>
	public abstract class AbstractController
	{
		protected AbstractController()
		{
		}
		
		protected virtual void ThrowInvalidOperationException(CommandEventArgs e, int num_expected)
		{
			string command   = e.CommandName;
			int    num_found = e.CommandArgs.Length;
			
			throw new System.InvalidOperationException (string.Format ("Command {0} requires {1} argument(s), got {2}.", command, num_expected, num_found));
		}
	}
}
