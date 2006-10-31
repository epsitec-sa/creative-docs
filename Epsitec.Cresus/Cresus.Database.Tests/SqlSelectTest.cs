using NUnit.Framework;
using System.Data;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class SqlSelectTest
	{
		[Test] public void Check00NewSqlSelect()
		{
			//	il n'existe qu'un seul constructeur, sans paramètre
			SqlSelect sql_select1 = new SqlSelect ();
			Assert.AreEqual (SqlSelectPredicate.All, sql_select1.Predicate);

			sql_select1.Predicate = SqlSelectPredicate.Distinct;
			Assert.AreEqual (SqlSelectPredicate.Distinct, sql_select1.Predicate);

			//	ajoute quelques champs
			sql_select1.Fields.Add (SqlField.CreateName ("Table1", "Colonne1"));
			sql_select1.Fields.Add (SqlField.CreateName ("Table1", "Colonne2"));
			sql_select1.Fields.Add (SqlField.CreateName ("Table2", "Colonne3"));

			//	crée une seconde instance, par exemple pour faire un UNION
			SqlSelect sql_select2 = new SqlSelect ();

			//	ajoute quelques champs
			sql_select2.Fields.Add (SqlField.CreateName ("Table1", "Colonne3"));
			sql_select2.Fields.Add (SqlField.CreateName ("Table2", "Colonne2"));

			//	combine les 2 clauses
			sql_select1.Add (sql_select2, SqlSelectSetOp.Union);
		}

		public static DbAccess CreateDbAccess()
		{
			//	défini un accès pour la base de donnée "employee"
			//	qui doit déjà exister
			DbAccess db_access = new DbAccess ();
			
			db_access.Provider		= "Firebird";
			db_access.LoginName		= "sysdba";
			db_access.LoginPassword = "masterkey";
			db_access.Database		= "employee";
			db_access.Server		= "localhost";
			db_access.CreateDatabase		= false;
			
			return db_access;
		}

		[Test] public void Check01SqlSelectAll()
		{
			//	fait un test de sélection de données.
			//	utilise une base de données existante
			//	C:\Program Files\firebird15\Data\Epsitec\Employee.Firebird

			DbAccess db_access = SqlSelectTest.CreateDbAccess ();

			IDbAbstraction db_abstraction = null;
			db_abstraction = DbFactory.FindDbAbstraction (db_access);

			ISqlEngine     sql_engine    = db_abstraction.SqlEngine;
			ISqlBuilder    sql_builder   = db_abstraction.SqlBuilder;

			//	fait la liste de tous les champs de la table EMPLOYEE
			SqlSelect sql_select = new SqlSelect ();
			sql_select.Fields.Add (SqlField.CreateAll ());
			sql_select.Tables.Add (SqlField.CreateName ("EMPLOYEE"));

			//	construit la commande d'extraction
			sql_builder.SelectData(sql_select);

			System.Data.IDbCommand command = sql_builder.Command;
			System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
			
			//	lecture des résultats
			DataSet data_set = new DataSet();
			command.Transaction = db_abstraction.BeginReadOnlyTransaction ();
			sql_engine.Execute (command, sql_builder.CommandType, sql_builder.CommandCount, out data_set);

			int n = this.DumpDataSet (data_set); 
		}

		int DumpDataSet(DataSet data_set)
		{
			int		nb = 0;

			//	Pour chaque table dans DataSet, imprime les valeurs.
			foreach (DataTable myTable in data_set.Tables)
			{
				System.Console.Out.WriteLine ("TableName = " + myTable.TableName.ToString ());
				
				//	Affiche le nom des colonnes
				foreach (DataColumn myColumn in data_set.Tables[0].Columns)
				{
					System.Console.Out.Write (myColumn.ColumnName);
					System.Console.Out.Write (", ");
				}
				System.Console.Out.WriteLine ();

				foreach (DataRow myRow in myTable.Rows)
				{
					nb++;
					foreach (DataColumn myColumn in myTable.Columns)
					{
						System.Console.Out.Write (myRow[myColumn]);
						System.Console.Out.Write (", ");
					}
					System.Console.Out.WriteLine ();
				}
			}
			System.Console.Out.WriteLine ("{0} fiches listées", nb);
			return nb;
		}

		[Test] public void Check02SqlSelectExecute2()
		{
			//	Test pour la clause ORDER BY
			//	C:\Program Files\firebird15\Data\Epsitec\Employee.Firebird

			DbAccess db_access = SqlSelectTest.CreateDbAccess ();

			IDbAbstraction db_abstraction = null;
			db_abstraction = DbFactory.FindDbAbstraction (db_access);

			ISqlEngine     sql_engine    = db_abstraction.SqlEngine;
			ISqlBuilder    sql_builder   = db_abstraction.SqlBuilder;

			//	fait la liste des noms de personne distincts de la table EMPLOYEE
			SqlSelect sql_select = new SqlSelect ();
			sql_select.Predicate = SqlSelectPredicate.Distinct;

			sql_select.Fields.Add (SqlField.CreateName ("LAST_NAME"), SqlFieldOrder.Inverse);
			sql_select.Tables.Add (SqlField.CreateName ("EMPLOYEE"));

			//	construit la commande d'extraction
			sql_builder.SelectData (sql_select);

			System.Data.IDbCommand command = sql_builder.Command;
			System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
			
			//	lecture des résultats
			DataSet data_set = new DataSet();
			command.Transaction = db_abstraction.BeginReadOnlyTransaction ();
			sql_engine.Execute (command, sql_builder.CommandType, sql_builder.CommandCount, out data_set);

			int n = this.DumpDataSet (data_set);
		}

		[Test] public void Check03SqlSelectWhere()
		{
			//	Test pour la clause WHERE
			//	C:\Program Files\firebird15\Data\Epsitec\Employee.Firebird

			DbAccess db_access = SqlSelectTest.CreateDbAccess ();

			IDbAbstraction db_abstraction = null;
			db_abstraction = DbFactory.FindDbAbstraction (db_access);

			ISqlEngine     sql_engine    = db_abstraction.SqlEngine;
			ISqlBuilder    sql_builder   = db_abstraction.SqlBuilder;

			//	fait la liste des noms de personne distincts de la table EMPLOYEE
			SqlSelect sql_select = new SqlSelect ();

			sql_select.Fields.Add (SqlField.CreateName ("LAST_NAME"));
			sql_select.Fields.Add (SqlField.CreateName ("FIRST_NAME"));
			sql_select.Fields.Add (SqlField.CreateName ("JOB_GRADE"));
			sql_select.Fields.Add (SqlField.CreateName ("JOB_COUNTRY"));
			sql_select.Tables.Add (SqlField.CreateName ("EMPLOYEE"));

			//	défini la fonction JOB_COUNTRY <> 'England'
			SqlFunction sql_func = new SqlFunction (SqlFunctionType.CompareNotEqual, 
													SqlField.CreateName("JOB_COUNTRY"),
													SqlField.CreateConstant("England", DbRawType.String));

			sql_select.Conditions.Add (SqlField.CreateFunction(sql_func));

			//	et JOB_GRADE > 3
			sql_func = new SqlFunction (SqlFunctionType.CompareGreaterThan, 
				SqlField.CreateName ("JOB_GRADE"),
				SqlField.CreateConstant (3, DbRawType.Int16));

			sql_select.Conditions.Add (SqlField.CreateFunction(sql_func));

			//	construit la commande d'extraction
			sql_builder.SelectData (sql_select);

			System.Data.IDbCommand command = sql_builder.Command;
			System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
			
			//	lecture des résultats
			DataSet data_set = new DataSet();
			command.Transaction = db_abstraction.BeginReadOnlyTransaction ();
			sql_engine.Execute (command, sql_builder.CommandType, sql_builder.CommandCount, out data_set);

			int n = this.DumpDataSet (data_set);
		}

		[Test] public void Check04SqlSelectWhereMax()
		{
			//	Test avec une sous-requête, dans
			//	C:\Program Files\firebird15\Data\Epsitec\Employee.Firebird
			//	pour chercher les fiches avec le JOB_GRADE le plus élevé
			//	select * from employee where job_grade = (select max(job_grade) from employee) 

			DbAccess db_access = SqlSelectTest.CreateDbAccess ();

			IDbAbstraction db_abstraction = null;
			db_abstraction = DbFactory.FindDbAbstraction (db_access);

			ISqlEngine     sql_engine    = db_abstraction.SqlEngine;
			ISqlBuilder    sql_builder   = db_abstraction.SqlBuilder;

			SqlSelect sql_select = new SqlSelect ();

			sql_select.Fields.Add (SqlField.CreateAll ());
			sql_select.Tables.Add (SqlField.CreateName ("EMPLOYEE"));

			SqlSelect sub_query = new SqlSelect ();
			sub_query.Fields.Add (SqlField.CreateAggregate (SqlAggregateType.Max, SqlField.CreateName ("JOB_GRADE")));
			sub_query.Tables.Add (SqlField.CreateName ("EMPLOYEE"));

			//	défini la fonction JOB_GRADE == max(JOB_GRADE)
			SqlFunction sql_func = new SqlFunction (SqlFunctionType.CompareEqual, 
				SqlField.CreateName ("JOB_GRADE"),
				SqlField.CreateSubQuery (sub_query));

			sql_select.Conditions.Add (SqlField.CreateFunction(sql_func));

			//	construit la commande d'extraction
			sql_builder.SelectData (sql_select);

			System.Data.IDbCommand command = sql_builder.Command;
			System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
			
			//	lecture des résultats
			DataSet data_set = new DataSet();
			command.Transaction = db_abstraction.BeginReadOnlyTransaction ();
			sql_engine.Execute (command, sql_builder.CommandType, sql_builder.CommandCount, out data_set);

			int n = this.DumpDataSet (data_set);
		}	

		[Test] public void Check05SqlSelectInnerJoin()
		{
			//	Test avec jointure, dans
			//	C:\Program Files\firebird15\Data\Epsitec\Employee.Firebird
			//	pour lister les employés et leurs projets
			//	SELECT LAST_NAME , FIRST_NAME , PROJ_ID FROM EMPLOYEE INNER JOIN employee_project ON EMPLOYEE.emp_no = employee_project.emp_no;

			DbAccess db_access = SqlSelectTest.CreateDbAccess ();

			IDbAbstraction db_abstraction = null;
			db_abstraction = DbFactory.FindDbAbstraction (db_access);

			ISqlEngine     sql_engine    = db_abstraction.SqlEngine;
			ISqlBuilder    sql_builder   = db_abstraction.SqlBuilder;

			SqlSelect sql_select = new SqlSelect ();

			sql_select.Fields.Add (SqlField.CreateName ("LAST_NAME"));
			sql_select.Fields.Add (SqlField.CreateName ("FIRST_NAME"));
			sql_select.Fields.Add (SqlField.CreateName ("PROJ_NAME"));

			SqlField table1 = SqlField.CreateName ("EMPLOYEE_PROJECT");
			table1.Alias = "A1";
			sql_select.Tables.Add (table1);
			SqlField table2 = SqlField.CreateName ("EMPLOYEE");
			table2.Alias = "A2";
			sql_select.Tables.Add (table2);
			SqlField table3 = SqlField.CreateName ("PROJECT");
			table3.Alias = "A3";
			sql_select.Tables.Add (table3);

			//	défini la fonction INNER JOIN ...
			SqlJoin sql_join = new SqlJoin (SqlJoinType.Inner, 
				SqlField.CreateName ("A1", "EMP_NO"), 
				SqlField.CreateName ("A2", "EMP_NO"));
			sql_select.Joins.Add (SqlField.CreateJoin(sql_join));

			//	cumule un second INNER JOIN ...
			sql_join = new SqlJoin (SqlJoinType.Inner, 
				SqlField.CreateName ("A1", "PROJ_ID"), 
				SqlField.CreateName ("A3", "PROJ_ID"));
			sql_select.Joins.Add (sql_join);

			//	construit la commande d'extraction
			sql_builder.SelectData (sql_select);

			System.Data.IDbCommand command = sql_builder.Command;
			System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
			
			//	lecture des résultats
			DataSet data_set = new DataSet();
			command.Transaction = db_abstraction.BeginReadOnlyTransaction ();
			sql_engine.Execute (command, sql_builder.CommandType, sql_builder.CommandCount, out data_set);

			int n = this.DumpDataSet (data_set);
		}

		[Test] public void Check06SqlSelectOrderBy()
		{
			//	Test avec aggrégat nécessitant un ORDER BY automatique, dans
			//	C:\Program Files\firebird15\Data\Epsitec\Employee.Firebird
			//	pour lister les employés et leurs projets
			//	SELECT job_country, sum( salary ), last_name FROM EMPLOYEE group by job_country, last_name;

			DbAccess db_access = SqlSelectTest.CreateDbAccess ();

			IDbAbstraction db_abstraction = null;
			db_abstraction = DbFactory.FindDbAbstraction (db_access);

			ISqlEngine     sql_engine    = db_abstraction.SqlEngine;
			ISqlBuilder    sql_builder   = db_abstraction.SqlBuilder;

			SqlSelect sql_select = new SqlSelect ();

			sql_select.Fields.Add (SqlField.CreateName ("JOB_COUNTRY"));
			sql_select.Fields.Add (SqlField.CreateAggregate (SqlAggregateType.Sum, SqlField.CreateName ("SALARY")));
			sql_select.Fields.Add (SqlField.CreateName ("LAST_NAME"));

			SqlField table = SqlField.CreateName ("EMPLOYEE");
			table.Alias = "EMPL";
			sql_select.Tables.Add (table);

			//	ajoute une condition HAVING... sur la somme
			SqlFunction sql_func = new SqlFunction (SqlFunctionType.CompareGreaterThan, 
				SqlField.CreateAggregate (SqlAggregateType.Sum, SqlField.CreateName ("SALARY")),
				SqlField.CreateConstant (50000, DbRawType.Int16));
			sql_select.Conditions.Add (SqlField.CreateFunction(sql_func));

			//	ainsi qu'une condition sur le pays
			sql_func = new SqlFunction (SqlFunctionType.CompareNotEqual, 
				SqlField.CreateName("JOB_COUNTRY"),
				SqlField.CreateConstant("England", DbRawType.String));
			sql_select.Conditions.Add (SqlField.CreateFunction(sql_func));

			//	construit la commande d'extraction
			sql_builder.SelectData (sql_select);

			System.Data.IDbCommand command = sql_builder.Command;
			System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
			
			//	lecture des résultats
			DataSet data_set = new DataSet();
			command.Transaction = db_abstraction.BeginReadOnlyTransaction ();
			sql_engine.Execute (command, sql_builder.CommandType, sql_builder.CommandCount, out data_set);

			int n = this.DumpDataSet (data_set);
		}

		[Test] public void Check07SqlSelectUnion()
		{
			//	Test avec UNION, dans
			//	C:\Program Files\firebird15\Data\Epsitec\Employee.Firebird
			
			DbAccess db_access = SqlSelectTest.CreateDbAccess ();

			IDbAbstraction db_abstraction = null;
			db_abstraction = DbFactory.FindDbAbstraction (db_access);

			ISqlEngine     sql_engine    = db_abstraction.SqlEngine;
			ISqlBuilder    sql_builder   = db_abstraction.SqlBuilder;

			SqlSelect sql_select1 = new SqlSelect ();

			sql_select1.Fields.Add (SqlField.CreateName ("LAST_NAME"));
			sql_select1.Fields.Add (SqlField.CreateName ("JOB_COUNTRY"));
			sql_select1.Fields.Add (SqlField.CreateName ("JOB_CODE"));
			sql_select1.Tables.Add (SqlField.CreateName ("EMPLOYEE"));

			//	défini la fonction JOB_COUNTRY == 'England'
			SqlFunction sql_func = new SqlFunction (SqlFunctionType.CompareEqual, 
				SqlField.CreateName("JOB_COUNTRY"),
				SqlField.CreateConstant("England", DbRawType.String));

			sql_select1.Conditions.Add (SqlField.CreateFunction(sql_func));

			//	crée une seconde instance, pour faire un UNION
			SqlSelect sql_select2 = new SqlSelect ();
			sql_select2.Predicate = SqlSelectPredicate.All;

			sql_select2.Fields.Add (SqlField.CreateName ("LAST_NAME"));
			sql_select2.Fields.Add (SqlField.CreateName ("JOB_COUNTRY"));
			sql_select2.Fields.Add (SqlField.CreateName ("JOB_CODE"));
			sql_select2.Tables.Add (SqlField.CreateName ("EMPLOYEE"));

			//	défini la fonction JOB_COUNTRY == 'Canada'
			sql_func = new SqlFunction (SqlFunctionType.CompareEqual, 
				SqlField.CreateName("JOB_CODE"),
				SqlField.CreateConstant("Admin", DbRawType.String));

			sql_select2.Conditions.Add (SqlField.CreateFunction(sql_func));

			//	combine les 2 clauses
			sql_select1.Add (sql_select2, SqlSelectSetOp.Union);

			//	construit la commande d'extraction
			sql_builder.SelectData (sql_select1);

			System.Data.IDbCommand command = sql_builder.Command;
			System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
			
			//	lecture des résultats
			DataSet data_set = new DataSet();
			command.Transaction = db_abstraction.BeginReadOnlyTransaction ();
			sql_engine.Execute (command, sql_builder.CommandType, sql_builder.CommandCount, out data_set);

			int n = this.DumpDataSet (data_set);

			//	note: je n'ai pas trouvé comment mettre des ORDER BY dans cette commande UNION,
			//		  me semble que ce n'est pas possible du tout ??
		}

		/* la commande INTERSECT n'est pas connue par Firebird,
		 * pas trouvé autre chose d'équivalent
		 
		[Test] public void Check08SqlSelectIntersect()
		{
			//	Test avec INTERSECT, dans
			//	C:\Program Files\firebird15\Data\Epsitec\Employee.Firebird
			
			DbAccess db_access = SqlSelectTest.CreateDbAccess ();

			IDbAbstraction db_abstraction = null;
			db_abstraction = DbFactory.FindDbAbstraction (db_access);

			ISqlEngine     sql_engine    = db_abstraction.SqlEngine;
			ISqlBuilder    sql_builder   = db_abstraction.SqlBuilder;

			SqlSelect sql_select1 = new SqlSelect ();

			sql_select1.Fields.Add (SqlField.CreateName ("LAST_NAME"));
			sql_select1.Fields.Add (SqlField.CreateName ("JOB_COUNTRY"));
			sql_select1.Fields.Add (SqlField.CreateName ("JOB_CODE"));
			sql_select1.Tables.Add (SqlField.CreateName ("EMPLOYEE"));

			//	défini la fonction JOB_COUNTRY == 'England'
			SqlFunction sql_func = new SqlFunction (SqlFunctionType.CompareEqual, 
				SqlField.CreateName("JOB_COUNTRY"),
				SqlField.CreateConstant("England", DbRawType.String));

			sql_select1.Conditions.Add (SqlField.CreateFunction(sql_func));

			//	crée une seconde instance, pour faire un UNION
			SqlSelect sql_select2 = new SqlSelect ();
			sql_select2.Predicate = SqlSelectPredicate.All;

			sql_select2.Fields.Add (SqlField.CreateName ("LAST_NAME"));
			sql_select2.Fields.Add (SqlField.CreateName ("JOB_COUNTRY"));
			sql_select2.Fields.Add (SqlField.CreateName ("JOB_CODE"));
			sql_select2.Tables.Add (SqlField.CreateName ("EMPLOYEE"));

			//	défini la fonction JOB_COUNTRY == 'Canada'
			sql_func = new SqlFunction (SqlFunctionType.CompareEqual, 
				SqlField.CreateName("JOB_CODE"),
				SqlField.CreateConstant("Admin", DbRawType.String));

			sql_select2.Conditions.Add (SqlField.CreateFunction(sql_func));

			//	combine les 2 clauses
			sql_select1.Add (sql_select2, SqlSelectSetOp.Intersect);

			//	construit la commande d'extraction
			sql_builder.SelectData (sql_select1);

			System.Data.IDbCommand command = sql_builder.Command;
			System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
			
			//	lecture des résultats
			DataSet data_set = new DataSet();
			command.Transaction = db_abstraction.BeginTransaction ();
			sql_engine.Execute (command, sql_builder.CommandType, sql_builder.CommandCount, out data_set);

			int n = this.DumpDataSet (data_set);
		}*/
	}
}
