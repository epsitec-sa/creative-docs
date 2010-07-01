//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD


namespace Epsitec.Cresus.Requests
{
	
	
	/// <summary>
	/// The <c>UpdateMode</c> enumeration defines the update modes for the
	/// update requests such as <see cref="UpdateStaticDataRequest"/>.
	/// </summary>
	public enum UpdateMode
	{
		
		
		/// <summary>
		/// Updates all columns in a given row.
		/// </summary>
		Full,

		
		/// <summary>
		/// Updates only columns which have changed in a given row.
		/// </summary>
		Changed


	}


}
