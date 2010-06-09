//	Copyright � 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// L'�num�ration <c>TextFieldMultiExPreserveMode</c> d�fini la position de l'ascenseur et des
	/// boutons accept/cancel lorsqu'ils sont visibles simultan�ments.
	/// </summary>
	public enum TextFieldMultiExPreferredLayout
	{
		/// <summary>
		/// Ascenseur et boutons accept/cancel les uns � c�t� des autres.
		/// </summary>
		PreserveScrollerHeight,

		/// <summary>
		/// Ascenseur et boutons accept/cancel les uns sur les autres.
		/// </summary>
		PreserveTextWidth,
	}
}
