//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support.Serialization
{
	/// <summary>
	/// La classe Helper permet d'accéder à des informations normalement
	/// cachées par SerializationInfo.
	/// </summary>
	public class Helper
	{
		public static bool FindElement(System.Runtime.Serialization.SerializationInfo info, string name)
		{
			int index = (int) info.GetType ().InvokeMember ("FindElement",
				/**/										System.Reflection.BindingFlags.NonPublic |
				/**/										System.Reflection.BindingFlags.Instance |
				/**/										System.Reflection.BindingFlags.InvokeMethod,
				/**/										null, info, new object[] { name });
			return (index >= 0);
		}
	}
}
