namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'interface IRawTypeConverter permet de convertir entre un type brut
	/// non supporté par la base de données, et son type correspondant.
	/// </summary>
	public interface IRawTypeConverter
	{
		DbRawType	MatchingType		{ get; }
		int			Length				{ get; }

		object ConvertToInternalType(object value);
		object ConvertFromInternalType(object value);
	}
}
