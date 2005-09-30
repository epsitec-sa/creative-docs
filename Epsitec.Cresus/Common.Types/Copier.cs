//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe Copier permet de réaliser des copies de données simples.
	/// </summary>
	public sealed class Copier
	{
		private Copier()
		{
		}
		
		public static object Copy(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			if (obj is System.String)
			{
				return obj;
			}
			if (obj is System.ValueType)
			{
				return obj;
			}
			
			//	TODO: compléter...

			throw new System.NotSupportedException (string.Format ("Cannot copy type {0}, not supported", obj.GetType ().Name));
		}
	}
}
