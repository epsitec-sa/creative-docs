//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>DockStyle</c> enumeration defines how widgets get docked
	/// relative to their parent.
	/// </summary>
	public enum DockStyle
	{
		/// <summary>
		/// No docking.
		/// </summary>
		None	= 0,

		/// <summary>
		/// The widget is docked at the top of its parent.
		/// </summary>
		Top		= 1,				//	colle en haut
		
		/// <summary>
		/// The widget is docked at the bottom of its parent.
		/// </summary>
		Bottom	= 2,				//	colle en bas
		
		/// <summary>
		/// The widget is docked at the left of its parent.
		/// </summary>
		Left	= 3,				//	colle à gauche
		
		/// <summary>
		/// The widget is docked at the right of its parent.
		/// </summary>
		Right	= 4,				//	colle à droite
		
		/// <summary>
		/// The widget fills its parent.
		/// </summary>
		Fill	= 5,				//	remplit tout
		
		
		/// <summary>
		/// The widget is stacked (either from left to right or from top to bottom,
		/// depending on the parent's <see cref="T:ContainerLayoutMode"/>.
		/// </summary>
		Stacked	= 6,				//	organisé en pile
		
		/// <summary>
		/// The widget is stacked at the stack beginning (usually left or top).
		/// </summary>
		StackBegin	= 7,
		
		/// <summary>
		/// The widget is stacked at the stack end (usually right or bottom).
		/// </summary>
		StackEnd	= 8,
		
		/// <summary>
		/// The widget is stacked in order to fill the stack center.
		/// </summary>
		StackFill	= 9,
	}
}
