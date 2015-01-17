namespace Epsitec.Cresus.WebCore.Server.Layout
{


	/// <summary>
	/// This class represents a warning that will be displayed to the user, letting him know that
	/// the edition of the current entity might have side effects, as it is referenced by many
	/// other. It is usefull when the user is editing the name of a city that is shared by several
	/// addresses for instance.
	/// </summary>
	internal class GlobalWarning : AbstractEditionTilePart
	{


		protected override string GetEditionTilePartType()
		{
			return "globalWarning";
		}


	}


}
