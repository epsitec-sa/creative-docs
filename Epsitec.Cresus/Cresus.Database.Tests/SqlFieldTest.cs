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
			
			sql_field = SqlField.CreateName ("TestNotQualified");
			Assert.AreEqual (SqlFieldType.Name, sql_field.FieldType);
			Assert.AreEqual ("TestNotQualified", sql_field.AsName);
			Assert.AreEqual (null, sql_field.AsQualifiedName);
			Assert.AreEqual (null, sql_field.Alias);
			Assert.AreEqual (SqlSortOrder.None, sql_field.SortOrder);
			Assert.AreEqual (DbRawType.Unknown, sql_field.RawType);
			
			sql_field = SqlField.CreateName ("Test", "Qualified");
			Assert.AreEqual (SqlFieldType.QualifiedName, sql_field.FieldType);
			Assert.AreEqual ("Test.Qualified", sql_field.AsQualifiedName);
			Assert.AreEqual ("Qualified", sql_field.AsName);
			Assert.AreEqual (null, sql_field.Alias);
			Assert.AreEqual (SqlSortOrder.None, sql_field.SortOrder);
			Assert.AreEqual (DbRawType.Unknown, sql_field.RawType);
			
			sql_field.Alias = "SetAlias";
			Assert.AreEqual ("SetAlias", sql_field.Alias);
			sql_field.SortOrder = SqlSortOrder.Ascending;
			Assert.AreEqual (SqlSortOrder.Ascending, sql_field.SortOrder);
			
			//	constructeurs, avec un alias
			
			sql_field = SqlField.CreateName ("TestNotQualified");
			sql_field.Alias = "NotQ";
			Assert.AreEqual (SqlFieldType.Name, sql_field.FieldType);
			Assert.AreEqual ("TestNotQualified", sql_field.AsName);
			Assert.AreEqual (null, sql_field.AsQualifiedName);
			Assert.AreEqual ("NotQ", sql_field.Alias);
			Assert.AreEqual (SqlSortOrder.None, sql_field.SortOrder);
			Assert.AreEqual (DbRawType.Unknown, sql_field.RawType);
			
			//	constructeurs, avec un alias et un ordre de tri
			
			sql_field = SqlField.CreateName ("Test", "Qualified");
			sql_field.Alias = "TQ";
			sql_field.SortOrder = SqlSortOrder.Descending;
			Assert.AreEqual (SqlFieldType.QualifiedName, sql_field.FieldType);
			Assert.AreEqual ("Test.Qualified", sql_field.AsQualifiedName);
			Assert.AreEqual ("Qualified", sql_field.AsName);
			Assert.AreEqual ("TQ", sql_field.Alias);
			Assert.AreEqual (SqlSortOrder.Descending, sql_field.SortOrder);
			Assert.AreEqual (DbRawType.Unknown, sql_field.RawType);
		}
	
		[Test] public void CheckValidate()
		{
			//	la primitive Validate demande un ISqlValidator
			//	ce qui est implémenté dans SqlBuilder

			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			ISqlBuilder     sql_builder    = db_abstraction.SqlBuilder;

			SqlField sql_field;

			sql_field = SqlField.CreateName ("TestNotQualifiedButValid");
			Assert.AreEqual (SqlFieldType.Name, sql_field.FieldType);
			Assert.AreEqual ("TestNotQualifiedButValid", sql_field.AsName);
			Assert.AreEqual (true, sql_field.Validate(sql_builder));

			sql_field = SqlField.CreateName ("Test", "QualifiedValid");
			sql_field.Alias = "TQ";
			sql_field.SortOrder = SqlSortOrder.Descending;
			Assert.AreEqual (SqlFieldType.QualifiedName, sql_field.FieldType);
			Assert.AreEqual ("Test.QualifiedValid", sql_field.AsQualifiedName);
			Assert.AreEqual (true, sql_field.Validate(sql_builder));
		}
		
		[Test] [ExpectedException (typeof (Exceptions.FormatException))] public void CheckValidateEx1()
		{
			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			ISqlBuilder     sql_builder    = db_abstraction.SqlBuilder;
			
			SqlField sql_field = SqlField.CreateName ("TestNotQualifiedBut&Invalid");
		}
		
		[Test] [ExpectedException (typeof (Exceptions.FormatException))] public void CheckValidateEx2()
		{
			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			ISqlBuilder     sql_builder    = db_abstraction.SqlBuilder;
			
			SqlField sql_field = SqlField.CreateName ("Test", "Qualified-Invalid");
		}
		
		
		[Test] public void CheckCreate()
		{
			SqlField sql_field, sql_field1, sql_field2;

			//	crée un champ nul
			sql_field = SqlField.CreateNull ();
			Assert.AreEqual (SqlFieldType.Null, sql_field.FieldType);
			
			//	crée un champ défaut
			sql_field = SqlField.CreateDefault ();
			Assert.AreEqual (SqlFieldType.Default, sql_field.FieldType);

			//	crée un champ pour une constante
			sql_field = SqlField.CreateConstant (123.45M, DbRawType.LargeDecimal);
			Assert.AreEqual (SqlFieldType.Constant, sql_field.FieldType);
			Assert.AreEqual (DbRawType.LargeDecimal, sql_field.RawType);
			Assert.AreEqual (123.45M, sql_field.AsConstant);

			//	crée un champ pour un paramètre d'entrée (== constante)
			object val = System.Guid.NewGuid ();
			sql_field = SqlField.CreateParameterIn (val, DbRawType.Guid);
			Assert.AreEqual (SqlFieldType.ParameterIn, sql_field.FieldType);
			Assert.AreEqual (DbRawType.Guid, sql_field.RawType);
			Assert.AreEqual (val, sql_field.AsParameter);

			//	crée un champ pour un paramètre de sortie
			sql_field = SqlField.CreateParameterOut (DbRawType.String);
			Assert.AreEqual (SqlFieldType.ParameterOut, sql_field.FieldType);
			Assert.AreEqual (DbRawType.String, sql_field.RawType);
			Assert.AreEqual (null, sql_field.AsParameter);

			//	crée un champ pour un paramètre en entrée/sortie
			sql_field = SqlField.CreateParameterInOut ("abc", DbRawType.String);
			Assert.AreEqual (SqlFieldType.ParameterInOut, sql_field.FieldType);
			Assert.AreEqual (DbRawType.String, sql_field.RawType);
			Assert.AreEqual ("abc", sql_field.AsParameter);

			//	crée un champ pour un résultat de fonction
			sql_field = SqlField.CreateParameterResult (DbRawType.Time);
			Assert.AreEqual (SqlFieldType.ParameterResult, sql_field.FieldType);
			Assert.AreEqual (DbRawType.Time, sql_field.RawType);
			Assert.AreEqual (null, sql_field.AsParameter);

			//	place la valeur du résultat (il n'y a pas de contrôle de type ici
			sql_field.SetParameterOutResult("abc");
			Assert.AreEqual ("abc", sql_field.AsParameter);

			//	crée un champ pour une extraction sur tout ( * ) 
			sql_field = SqlField.CreateAll();
			Assert.AreEqual (SqlFieldType.All, sql_field.FieldType);
			Assert.AreEqual (null, sql_field.AsName);
			Assert.AreEqual (null, sql_field.AsQualifiedName);
			Assert.AreEqual (null, sql_field.Alias);
			Assert.AreEqual (SqlSortOrder.None, sql_field.SortOrder);
			Assert.AreEqual (DbRawType.Unknown, sql_field.RawType);

			//	crée un champ pour un nom (qualifié ou non)
			sql_field = SqlField.CreateName ("Test", "Qualified");
			Assert.AreEqual (SqlFieldType.QualifiedName, sql_field.FieldType);
			Assert.AreEqual ("Test.Qualified", sql_field.AsQualifiedName);
			Assert.AreEqual ("Qualified", sql_field.AsName);
			Assert.AreEqual (null, sql_field.Alias);
			Assert.AreEqual (SqlSortOrder.None, sql_field.SortOrder);
			Assert.AreEqual (DbRawType.Unknown, sql_field.RawType);

			//	crée un champ pour un nom de table / colonne
			sql_field1 = SqlField.CreateName ("Table", "Colonne");
			Assert.AreEqual (SqlFieldType.QualifiedName, sql_field1.FieldType);
			Assert.AreEqual ("Table.Colonne", sql_field1.AsQualifiedName);
			Assert.AreEqual ("Colonne", sql_field1.AsName);
			Assert.AreEqual (null, sql_field1.Alias);
			Assert.AreEqual (SqlSortOrder.None, sql_field1.SortOrder);
			Assert.AreEqual (DbRawType.Unknown, sql_field1.RawType);

			//	crée un champ pour un aggrégat concernant la colonne ci-dessus
			sql_field2 = SqlField.CreateAggregate (new SqlAggregate (SqlAggregateFunction.Sum, sql_field1));
			Assert.AreEqual (SqlFieldType.Aggregate, sql_field2.FieldType);
			Assert.AreEqual (SqlAggregateFunction.Sum, sql_field2.AsAggregate.Function);
			Assert.AreEqual (null, sql_field2.AsQualifiedName);
			Assert.AreEqual (null, sql_field2.AsName);
			Assert.AreEqual (null, sql_field2.Alias);
			Assert.AreEqual (SqlSortOrder.None, sql_field2.SortOrder);
			Assert.AreEqual (DbRawType.Unknown, sql_field2.RawType);

			//	idem en donnant juste le type d'aggrégat souhaité
			sql_field2 = SqlField.CreateAggregate (SqlAggregateFunction.Count, sql_field1);
			Assert.AreEqual (SqlFieldType.Aggregate, sql_field2.FieldType);
			Assert.AreEqual (SqlAggregateFunction.Count, sql_field2.AsAggregate.Function);
			Assert.AreEqual (null, sql_field2.AsQualifiedName);
			Assert.AreEqual (null, sql_field2.AsName);
			Assert.AreEqual (null, sql_field2.Alias);
			Assert.AreEqual (SqlSortOrder.None, sql_field2.SortOrder);
			Assert.AreEqual (DbRawType.Unknown, sql_field2.RawType);

			//	crée un champ pour une varialble
			sql_field = SqlField.CreateVariable ();
			Assert.AreEqual (SqlFieldType.Variable, sql_field.FieldType);

			//	crée un champ pour une fonction
			sql_field = SqlField.CreateFunction (null);
			Assert.AreEqual (SqlFieldType.Function, sql_field.FieldType);

			//	crée un champ pour une procédure
			sql_field = SqlField.CreateProcedure ("ProcedureName");
			Assert.AreEqual (SqlFieldType.Procedure, sql_field.FieldType);
			Assert.AreEqual ("ProcedureName", sql_field.AsProcedure);

			//	crée un champ pour une sous-requête
			SqlSelect select = new SqlSelect ();
			sql_field = SqlField.CreateSubQuery (select);
			Assert.AreEqual (SqlFieldType.SubQuery, sql_field.FieldType);
			Assert.AreEqual (select, sql_field.AsSubQuery);
		}
	}
}
