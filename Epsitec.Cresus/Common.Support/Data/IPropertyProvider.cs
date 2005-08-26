//	Copyright © 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support.Data
{
	public interface IPropertyProvider
	{
		string[] GetPropertyNames();
		void SetProperty(string key, object value);
		object GetProperty(string key);
		bool IsPropertyDefined(string key);
		void ClearProperty(string key);
	}
}
