//	Copyright � 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 07/10/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'interface IRawTypeConverter permet de convertir entre un type brut
	/// non support� par la base de donn�es, et son type correspondant.
	/// </summary>
	public interface IRawTypeConverter
	{
		DbRawType	ExternalType		{ get; }
		DbRawType	InternalType		{ get; }
		int			Length				{ get; }
		bool		IsFixedLength		{ get; }

		object ConvertToInternalType(object value);
		object ConvertFromInternalType(object value);
	}
}
