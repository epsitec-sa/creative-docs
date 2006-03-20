//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	public interface ITypeConverter
	{
		string ConvertToString(object value, IContextResolver context);
		object ConvertFromString(string value, IContextResolver context);
	}
}
