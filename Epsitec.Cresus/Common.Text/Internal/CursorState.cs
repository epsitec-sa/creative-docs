//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Internal
{
	/// <summary>
	/// L'énumération CursorState définit l'état (interne) d'un curseur.
	/// </summary>
	internal enum CursorState : byte
	{
		Copied		= 0,	//	curseur standard (en général, copie d'un curseur alloué)
		
		Allocated	= 1,	//	pour CursorTable: curseur alloué
		Free		= 2,	//	pour CursorTable: curseur libre
		Invalid		= 3,	//	pour CursorTable: curseur non valide (id = 0)
	}
}
