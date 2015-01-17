//	Copyright � 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Internal
{
	/// <summary>
	/// L'�num�ration CursorState d�finit l'�tat (interne) d'un curseur.
	/// </summary>
	internal enum CursorState : byte
	{
		Copied		= 0,	//	curseur standard (en g�n�ral, copie d'un curseur allou�)
		
		Allocated	= 1,	//	pour CursorTable: curseur allou�
		Free		= 2,	//	pour CursorTable: curseur libre
		Invalid		= 3,	//	pour CursorTable: curseur non valide (id = 0)
	}
}
