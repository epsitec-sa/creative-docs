//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'interface IRawTypeConverter permet de convertir entre un type brut
	/// non supporté par la base de données, et son type correspondant.
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
