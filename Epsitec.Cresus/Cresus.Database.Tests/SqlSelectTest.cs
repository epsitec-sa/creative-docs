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
			sql_select1.Fields.Add(SqlField.CreateName ("Table1", "Colonne1"));
			sql_select1.Fields.Add(SqlField.CreateName ("Table1", "Colonne2"));
			sql_select1.Fields.Add(SqlField.CreateName ("Table2", "Colonne3"));
			//	DD?	il faudrait que les champs créés aient le nom qualifié comme Alias, automatiquement ?
			//	PA: non, parce que le nom d'alias ne peut en principe pas contenir de ".", d'après ce que
			//	j'ai compris de la norme SQL... A voir. A moins de transformer les noms d'alias avec "_"
			//	à la place de ".".

			//	crée une seconde instance, par exemple pour faire un UNION
			SqlSelect sql_select2 = new SqlSelect ();

			//	ajoute quelques champs
			sql_select2.Fields.Add(SqlField.CreateName ("Table1", "Colonne3"));
			sql_select2.Fields.Add(SqlField.CreateName ("Table2", "Colonne2"));

			//	combine les 2 clauses
			sql_select1.Add(sql_select2, SqlSelectSetOp.Union);
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
			sql_select.Fields.Add(SqlField.CreateAll ());
			sql_select.Tables.Add(SqlField.CreateName ("EMPLOYEE"));

			//	construit la commande d'extraction
			sql_builder.SelectData(sql_select);

			System.Data.IDbCommand command = sql_builder.Command;
			System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
			
			//	lecture des résultats
			DataSet data_set = new DataSet();
			command.Transaction = db_abstraction.BeginTransaction ();
			sql_engine.Execute (command, sql_builder.CommandType, out data_set);

			// For each table in the DataSet, print the row values.
			foreach(DataTable myTable in data_set.Tables)
			{
				System.Console.Out.WriteLine("TableName = " + myTable.TableName.ToString());
				foreach(DataRow myRow in myTable.Rows)
				{
					foreach (DataColumn myColumn in myTable.Columns)
					{
						System.Console.Out.Write(myRow[myColumn]);
						System.Console.Out.Write(" ");
					}
					System.Console.Out.WriteLine();
				}
			}

		}
	}
}
