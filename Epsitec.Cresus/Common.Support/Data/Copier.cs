//	Copyright � 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Common.Support.Data
{
	/// <summary>
	/// La classe Copier permet de r�aliser des copies de donn�es simples.
	/// </summary>
	public class Copier
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
			
			//	TODO: compl�ter...

			throw new System.NotSupportedException (string.Format ("Cannot copy type {0}, not supported", obj.GetType ().Name));
		}
	}
}
