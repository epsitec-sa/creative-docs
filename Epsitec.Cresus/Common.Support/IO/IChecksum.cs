//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 15/04/2004

namespace Epsitec.Common.Support.IO
{
	/// <summary>
	/// L'interface IChecksum permet d'accéder aux fonctions de calcul
	/// du checksum, indépendamment de l'algorithme.
	/// </summary>
	public interface IChecksum
	{
		long		Value		{ get; }
		
		void Reset();
		
		void Update(int byte_value);
		void Update(byte[] buffer);
		void Update(byte[] buffer, int offset, int length);
	}
}
