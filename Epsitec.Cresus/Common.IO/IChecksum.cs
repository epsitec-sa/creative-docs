//	Copyright © 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.IO
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
		
		void UpdateValue(string value);
		void UpdateValue(int value);
		void UpdateValue(short value);
		void UpdateValue(double value);
		void UpdateValue(bool value);
	}
}
