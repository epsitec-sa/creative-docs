//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 07/02/2004

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
