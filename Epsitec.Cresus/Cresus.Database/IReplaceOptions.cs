//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	public interface IReplaceOptions
	{
		bool IgnoreColumn(DbColumn column);
		object GetDefaultValue(DbColumn column);
	}
}
