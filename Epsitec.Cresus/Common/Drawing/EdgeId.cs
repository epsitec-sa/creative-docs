//	Copyright © 2003-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// Définition des côtés d'un rectangle. Plusieurs côtés peuvent être
	/// combinés, c'est donc un bitset.
	/// </summary>
	[System.Flags]
	public enum EdgeId : byte
	{
		None = 0,

		Bottom	= 1,
		Top		= 2,
		Left	= 4,
		Right	= 8,
	}
}
