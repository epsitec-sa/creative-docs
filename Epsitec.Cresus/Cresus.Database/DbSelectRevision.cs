//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	public enum DbSelectRevision
	{
		All,
		LiveActive,					//	s�l. DbRowStatus.Live, Copied
		LiveAll,					//	s�l. DbRowStatus.Live, Copied, Archive
	}
}
