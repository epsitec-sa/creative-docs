using NUnit.Framework;

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
	}
}
