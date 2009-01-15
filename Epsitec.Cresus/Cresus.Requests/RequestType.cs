//	Copyright � 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
		Group,										//	groupe de requ�tes

		/// <summary>
		/// 
		/// </summary>
		InsertStaticData,							//	requ�te INSERT, donn�es statiques

		/// <summary>
		/// 
		/// </summary>
		UpdateStaticData,							//	requ�te UPDATE, donn�es statiques
		/// <summary>
		/// 
		/// </summary>
		UpdateDynamicData,							//	requ�te UPDATE, donn�es dynamiques (calcul�es)
	}
}
