//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe Tags exporte des fonctions qui ne sont pas accessibles
	/// dans Support (problème de références circulaires).
	/// </summary>
	internal sealed class Tags
	{
		private Tags()
		{
		}
		
		
		public const string	Caption		= "capt";
		public const string	Description	= "desc";
	}
}
