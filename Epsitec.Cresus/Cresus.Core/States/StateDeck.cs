//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Core.States
{
	/// <summary>
	/// The <c>StateDeck</c> enumeration defines how states get grouped
	/// together.
	/// </summary>
	public enum StateDeck
	{
		/// <summary>
		/// The state does not belong to any deck.
		/// </summary>
		None,

		/// <summary>
		/// The state belongs to the history deck, which is in fact just a pile
		/// of cards.
		/// </summary>
		History,

		/// <summary>
		/// The state is a stand-alone state which does not belong to a pile,
		/// but rather to a spread set of cards.
		/// </summary>
		StandAlone
	}
}
