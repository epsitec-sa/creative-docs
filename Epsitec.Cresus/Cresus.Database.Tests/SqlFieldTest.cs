using NUnit.Framework;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class SqlFieldTest
	{
		[Test] public void CheckNewSqlField()
		{
			SqlField sql_field;

			//	constructeurs, avec nom qualifié ou non

			sql_field = new SqlField ("TestNotQualified");
			Assertion.AssertEquals (SqlFieldType.Name, sql_field.Type);
			Assertion.AssertEquals ("TestNotQualified", sql_field.AsName);
			Assertion.AssertEquals (null, sql_field.AsQualifiedName);
			Assertion.AssertEquals (null, sql_field.Alias);
			Assertion.AssertEquals (SqlFieldOrder.None, sql_field.Order);
			Assertion.AssertEquals (DbRawType.Unknown, sql_field.RawType);

			sql_field = new SqlField ("Test.Qualified");
			Assertion.AssertEquals (SqlFieldType.QualifiedName, sql_field.Type);
			Assertion.AssertEquals ("Test.Qualified", sql_field.AsQualifiedName);
			Assertion.AssertEquals ("Qualified", sql_field.AsName);
			Assertion.AssertEquals (null, sql_field.Alias);
			Assertion.AssertEquals (SqlFieldOrder.None, sql_field.Order);
			Assertion.AssertEquals (DbRawType.Unknown, sql_field.RawType);

			sql_field.Alias = "SetAlias";
			Assertion.AssertEquals ("SetAlias", sql_field.Alias);
			sql_field.Order = SqlFieldOrder.Normal;
			Assertion.AssertEquals (SqlFieldOrder.Normal, sql_field.Order);

			//	constructeurs, avec un alias

			sql_field = new SqlField ("TestNotQualified", "NotQ");
			Assertion.AssertEquals (SqlFieldType.Name, sql_field.Type);
			Assertion.AssertEquals ("TestNotQualified", sql_field.AsName);
			Assertion.AssertEquals (null, sql_field.AsQualifiedName);
			Assertion.AssertEquals ("NotQ", sql_field.Alias);
			Assertion.AssertEquals (SqlFieldOrder.None, sql_field.Order);
			Assertion.AssertEquals (DbRawType.Unknown, sql_field.RawType);

			//	constructeurs, avec un alias et un ordre de tri

			sql_field = new SqlField ("Test.Qualified", "TQ", SqlFieldOrder.Inverse);
			Assertion.AssertEquals (SqlFieldType.QualifiedName, sql_field.Type);
			Assertion.AssertEquals ("Test.Qualified", sql_field.AsQualifiedName);
			Assertion.AssertEquals ("Qualified", sql_field.AsName);
			Assertion.AssertEquals ("TQ", sql_field.Alias);
			Assertion.AssertEquals (SqlFieldOrder.Inverse, sql_field.Order);
			Assertion.AssertEquals (DbRawType.Unknown, sql_field.RawType);
		}
	
		[Test] public void CheckValidate()
		{
			//	la primitive Validate demande un ISqlValidator
			//	ce qui est implémenté dans SqlBuilder

			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			ISqlBuilder     sql_builder    = db_abstraction.SqlBuilder;

			SqlField sql_field;

			sql_field = new SqlField ("TestNotQualifiedButValide");
			Assertion.AssertEquals (SqlFieldType.Name, sql_field.Type);
			Assertion.AssertEquals ("TestNotQualifiedButValide", sql_field.AsName);
			Assertion.AssertEquals (true, sql_field.Validate(sql_builder));

			sql_field = new SqlField ("TestNotQualifiedBut&Invalide");
			Assertion.AssertEquals (SqlFieldType.Name, sql_field.Type);
			Assertion.AssertEquals ("TestNotQualifiedBut&Invalide", sql_field.AsName);
			Assertion.AssertEquals (false, sql_field.Validate(sql_builder));

			sql_field = new SqlField ("Test.QualifiedValide", "TQ", SqlFieldOrder.Inverse);
			Assertion.AssertEquals (SqlFieldType.QualifiedName, sql_field.Type);
			Assertion.AssertEquals ("Test.QualifiedValide", sql_field.AsQualifiedName);
			Assertion.AssertEquals (true, sql_field.Validate(sql_builder));

			sql_field = new SqlField ("Test.Qualified-Invalide", "TQ", SqlFieldOrder.Inverse);
/*			il n'est pas possible de créer un Field avec un nom qualifié non valide
 *			le texte est considéré alors comme non-qualifié */
			Assertion.AssertEquals (SqlFieldType.Name, sql_field.Type);
			Assertion.AssertEquals ("Test.Qualified-Invalide", sql_field.AsName);
			Assertion.AssertEquals (false, sql_field.Validate(sql_builder));
		}

		[Test] public void CheckCreate()
		{
			SqlField sql_field, sql_field1, sql_field2;

			//	crée un champ nul
			sql_field = SqlField.CreateNull ();
			Assertion.AssertEquals (SqlFieldType.Null, sql_field.Type);
			
			//	crée un champ défaut
			sql_field = SqlField.CreateDefault ();
			Assertion.AssertEquals (SqlFieldType.Default, sql_field.Type);

			//	crée un champ pour une constante
			sql_field = SqlField.CreateConstant (123.45M, DbRawType.LargeDecimal);
			Assertion.AssertEquals (SqlFieldType.Constant, sql_field.Type);
			Assertion.AssertEquals (DbRawType.LargeDecimal, sql_field.RawType);
			Assertion.AssertEquals (123.45M, sql_field.AsConstant);

			//	crée un champ pour un paramètre d'entrée (== constante)
			object val = System.Guid.NewGuid ();
			sql_field = SqlField.CreateParameterIn (val, DbRawType.Guid);
			Assertion.AssertEquals (SqlFieldType.ParameterIn, sql_field.Type);
			Assertion.AssertEquals (DbRawType.Guid, sql_field.RawType);
			Assertion.AssertEquals (val, sql_field.AsParameter);

			//	crée un champ pour un paramètre de sortie
			sql_field = SqlField.CreateParameterOut (DbRawType.String);
			Assertion.AssertEquals (SqlFieldType.ParameterOut, sql_field.Type);
			Assertion.AssertEquals (DbRawType.String, sql_field.RawType);
			Assertion.AssertEquals (null, sql_field.AsParameter);

			//	crée un champ pour un paramètre en entrée/sortie
			sql_field = SqlField.CreateParameterInOut ("abc", DbRawType.String);
			Assertion.AssertEquals (SqlFieldType.ParameterInOut, sql_field.Type);
			Assertion.AssertEquals (DbRawType.String, sql_field.RawType);
			Assertion.AssertEquals ("abc", sql_field.AsParameter);

			//	crée un champ pour un résultat de fonction
			sql_field = SqlField.CreateParameterResult (DbRawType.Time);
			Assertion.AssertEquals (SqlFieldType.ParameterResult, sql_field.Type);
			Assertion.AssertEquals (DbRawType.Time, sql_field.RawType);
			Assertion.AssertEquals (null, sql_field.AsParameter);

			// place la valeur du résultat (il n'y a pas de contrôle de type ici
			sql_field.SetParameterOutResult("abc");
			Assertion.AssertEquals ("abc", sql_field.AsParameter);

			//	crée un champ à tout usage
			sql_field = SqlField.CreateAll();
			Assertion.AssertEquals (SqlFieldType.Unsupported, sql_field.Type);
			Assertion.AssertEquals (null, sql_field.AsName);
			Assertion.AssertEquals (null, sql_field.AsQualifiedName);
			Assertion.AssertEquals (null, sql_field.Alias);
			Assertion.AssertEquals (SqlFieldOrder.None, sql_field.Order);
			Assertion.AssertEquals (DbRawType.Unknown, sql_field.RawType);

			//	crée un champ pour un nom (qualifié ou non)
			sql_field = SqlField.CreateName ("Test.Qualified");
			Assertion.AssertEquals (SqlFieldType.QualifiedName, sql_field.Type);
			Assertion.AssertEquals ("Test.Qualified", sql_field.AsQualifiedName);
			Assertion.AssertEquals ("Qualified", sql_field.AsName);
			Assertion.AssertEquals (null, sql_field.Alias);
			Assertion.AssertEquals (SqlFieldOrder.None, sql_field.Order);
			Assertion.AssertEquals (DbRawType.Unknown, sql_field.RawType);

			//	crée un champ pour un nom de table / colonne
			sql_field1 = SqlField.CreateName ("Table", "Colonne");
			Assertion.AssertEquals (SqlFieldType.QualifiedName, sql_field1.Type);
			Assertion.AssertEquals ("Table.Colonne", sql_field1.AsQualifiedName);
			Assertion.AssertEquals ("Colonne", sql_field1.AsName);
			Assertion.AssertEquals (null, sql_field1.Alias);
			Assertion.AssertEquals (SqlFieldOrder.None, sql_field1.Order);
			Assertion.AssertEquals (DbRawType.Unknown, sql_field1.RawType);

			//	crée un champ pour un aggrégat concernant la colonne ci-dessus
			sql_field2 = SqlField.CreateAggregate (new SqlAggregate (SqlAggregateType.Sum, sql_field1));
			Assertion.AssertEquals (SqlFieldType.Aggregate, sql_field2.Type);
			Assertion.AssertEquals (SqlAggregateType.Sum, sql_field2.AsAggregate.Type);
			Assertion.AssertEquals (null, sql_field2.AsQualifiedName);
			Assertion.AssertEquals (null, sql_field2.AsName);
			Assertion.AssertEquals (null, sql_field2.Alias);
			Assertion.AssertEquals (SqlFieldOrder.None, sql_field2.Order);
			Assertion.AssertEquals (DbRawType.Unknown, sql_field2.RawType);

			//	idem en donnant juste le type d'aggrégat souhaité
			sql_field2 = SqlField.CreateAggregate (SqlAggregateType.Count, sql_field1);
			Assertion.AssertEquals (SqlFieldType.Aggregate, sql_field2.Type);
			Assertion.AssertEquals (SqlAggregateType.Count, sql_field2.AsAggregate.Type);
			Assertion.AssertEquals (null, sql_field2.AsQualifiedName);
			Assertion.AssertEquals (null, sql_field2.AsName);
			Assertion.AssertEquals (null, sql_field2.Alias);
			Assertion.AssertEquals (SqlFieldOrder.None, sql_field2.Order);
			Assertion.AssertEquals (DbRawType.Unknown, sql_field2.RawType);

			//	crée un champ pour une varialble
			sql_field = SqlField.CreateVariable ();
			Assertion.AssertEquals (SqlFieldType.Variable, sql_field.Type);

			//	crée un champ pour une fonction
			sql_field = SqlField.CreateFunction ();
			Assertion.AssertEquals (SqlFieldType.Function, sql_field.Type);

			//	crée un champ pour une procédure
			sql_field = SqlField.CreateProcedure ("ProcedureName");
			Assertion.AssertEquals (SqlFieldType.Procedure, sql_field.Type);
			Assertion.AssertEquals ("ProcedureName", sql_field.AsProcedure);

			//	crée un champ pour une sous-requête
			SqlSelect select = new SqlSelect ();
			sql_field = SqlField.CreateSubQuery (select);
			Assertion.AssertEquals (SqlFieldType.SubQuery, sql_field.Type);
			Assertion.AssertEquals (select, sql_field.AsSubQuery);
		}
	}
}
