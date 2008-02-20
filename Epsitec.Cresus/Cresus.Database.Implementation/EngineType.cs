//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Implementation
{
	/// <summary>
	/// The <c>EngineType</c> enumeration defines which database engine should
	/// be used (either the real server or an embedded server).
	/// </summary>
	public enum EngineType
	{
		Server		= 0,
		Embedded	= 1,
	}
}
