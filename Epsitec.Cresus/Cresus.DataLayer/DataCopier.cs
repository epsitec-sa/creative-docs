//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Cresus.DataLayer
{
	/// <summary>
	/// La classe DataCopier permet de réaliser des copies de données manipulées
	/// par le DataLayer.
	/// </summary>
	public class DataCopier
	{
		private DataCopier()
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
