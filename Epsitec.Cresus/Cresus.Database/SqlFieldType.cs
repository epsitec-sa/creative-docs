//	Copyright � 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	public enum SqlFieldType
	{
		Unsupported,							//	champ non support� (ou non d�fini)
										
		Null,									//	constante NULL
		All,									//	constante sp�ciale pour aggr�gats: *
		Default,								//	constante sp�ciale pour INSERT INTO...
		Constant,								//	constante (donn�e compatible DbRawType)
										
		ParameterIn = Constant,					//	param�tre en entr�e = comme constante
		ParameterOut,							//	param�tre en sortie
		ParameterInOut,							//	param�tre en entr�e et en sortie
		ParameterResult,						//	param�tre en sortie (r�sultat de proc�dure)
										
		Name,									//	nom simple (nom de colonne, nom de table, nom de type, ...)
		QualifiedName,							//	nom qualifi� (nom de table + nom de colonne)
										
		Aggregate,						
		Variable,								//	variable SQL (?)
		Function,								//	fonction SQL (?)
		Procedure,								//	proc�dure SQL (?)
										
		SubQuery,								//	sous-requ�te
		Join									//	jointure
	}									
}
