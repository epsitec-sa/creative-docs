//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// The <c>RequestType</c> enumeration defines all possible request types.
	/// </summary>
	public enum RequestType
	{
		/// <summary>
		/// 
		/// </summary>
		Unknown = 0,

		/// <summary>
		/// 
		/// </summary>
		Group,										//	groupe de requêtes

		/// <summary>
		/// 
		/// </summary>
		InsertStaticData,							//	requête INSERT, données statiques

		/// <summary>
		/// 
		/// </summary>
		UpdateStaticData,							//	requête UPDATE, données statiques
		/// <summary>
		/// 
		/// </summary>
		UpdateDynamicData,							//	requête UPDATE, données dynamiques (calculées)
	}
}
