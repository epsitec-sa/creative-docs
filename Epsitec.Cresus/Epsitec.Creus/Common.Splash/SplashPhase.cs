//	Copyright © 2009-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Splash
{
	/// <summary>
	/// The <c>SplashPhase</c> enumeration defines the possible animation
	/// states of a splash screen.
	/// </summary>
	public enum SplashPhase
	{
		/// <summary>
		/// The splash screen is in the fade-in state (becoming visible).
		/// </summary>
		FadeIn,

		/// <summary>
		/// The splash screen is in the show state (visible).
		/// </summary>
		Show,

		/// <summary>
		/// The splash screen is in the fade-out state (becoming invisible).
		/// </summary>
		FadeOut,
	};
}
