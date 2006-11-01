//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	public enum SqlFieldType
	{
		Unsupported,							//	champ non supporté (ou non défini)
										
		Null,									//	constante NULL
		All,									//	constante spéciale pour aggrégats: *
		Default,								//	constante spéciale pour INSERT INTO...
		Constant,								//	constante (donnée compatible DbRawType)
										
		ParameterIn = Constant,					//	paramètre en entrée = comme constante
		ParameterOut,							//	paramètre en sortie
		ParameterInOut,							//	paramètre en entrée et en sortie
		ParameterResult,						//	paramètre en sortie (résultat de procédure)
										
		Name,									//	nom simple (nom de colonne, nom de table, nom de type, ...)
		QualifiedName,							//	nom qualifié (nom de table + nom de colonne)
										
		Aggregate,						
		Variable,								//	variable SQL (?)
		Function,								//	fonction SQL (?)
		Procedure,								//	procédure SQL (?)
										
		SubQuery,								//	sous-requête
		Join									//	jointure
	}									
}
