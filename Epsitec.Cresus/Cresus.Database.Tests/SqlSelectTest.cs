using NUnit.Framework;
using System.Data;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class SqlSelectTest
	{
		[Test] public void CheckNewSqlSelect()
		{
			//	il n'existe qu'un seul constructeur, sans paramètre
			SqlSelect sql_select1 = new SqlSelect ();
			Assertion.AssertEquals (SqlSelectPredicate.All, sql_select1.Predicate);

			sql_select1.Predicate = SqlSelectPredicate.Distinct;
			Assertion.AssertEquals (SqlSelectPredicate.Distinct, sql_select1.Predicate);

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
			db_access.Create		= false;
			
			return db_access;
		}

		[Test] public void CheckSqlSelectExecute()
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
			command.Transaction = db_abstraction.BeginTransaction ();
			sql_engine.Execute (command, sql_builder.CommandType, sql_builder.CommandCount, out data_set);

			int n = this.DumpDataSet (data_set); 
		}

		int DumpDataSet(DataSet data_set)
		{
			int		nb = 0;
			// For each table in the DataSet, print the row values.
			foreach (DataTable myTable in data_set.Tables)
			{
				System.Console.Out.WriteLine ("TableName = " + myTable.TableName.ToString ());
				foreach (DataRow myRow in myTable.Rows)
				{
					nb++;
					foreach (DataColumn myColumn in myTable.Columns)
					{
						System.Console.Out.Write (myRow[myColumn]);
						System.Console.Out.Write (" ");
					}
					System.Console.Out.WriteLine ();
				}
			}
			System.Console.Out.WriteLine ("{0} fiches listées", nb);
			return nb;
		}

		[Test] public void CheckSqlSelectExecute2()
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
			command.Transaction = db_abstraction.BeginTransaction ();
			sql_engine.Execute (command, sql_builder.CommandType, sql_builder.CommandCount, out data_set);

			int n = this.DumpDataSet (data_set);
		}

		[Test] public void CheckSqlSelectExecute3()
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
			command.Transaction = db_abstraction.BeginTransaction ();
			sql_engine.Execute (command, sql_builder.CommandType, sql_builder.CommandCount, out data_set);

			int n = this.DumpDataSet (data_set);
		}

		[Test] public void CheckSqlSelectExecute4()
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
			command.Transaction = db_abstraction.BeginTransaction ();
			sql_engine.Execute (command, sql_builder.CommandType, sql_builder.CommandCount, out data_set);

			int n = this.DumpDataSet (data_set);
		}	
	}
}
