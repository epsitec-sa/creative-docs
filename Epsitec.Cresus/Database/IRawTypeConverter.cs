namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'interface IRawTypeConverter permet de convertir entre un type brut
	/// non support� par la base de donn�es, et son type correspondant.
	/// </summary>
	public interface IRawTypeConverter
	{
		DbRawType	MatchingType		{ get; }
		int			Length				{ get; }

		object ConvertToInternalType(object value);
		object ConvertFromInternalType(object value);
	}
}
