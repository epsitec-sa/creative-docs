using NUnit.Framework;

namespace Epsitec.Cresus.DataLayer
{
	[TestFixture]
	public class RequestFactoryTest
	{
		[Test] public void Check01NewAndDispose()
		{
			RequestFactory factory = new RequestFactory ();
			
			factory.Dispose ();
		}
		
		[Test] public void Check02GenerateRequests()
		{
			RequestFactory factory = new RequestFactory ();
			
			System.Data.DataTable table = new System.Data.DataTable ("Test Table");
			
			System.Data.DataColumn col_a = new System.Data.DataColumn ("Column A", typeof (long));
			System.Data.DataColumn col_b = new System.Data.DataColumn ("Column B", typeof (string));
			System.Data.DataColumn col_c = new System.Data.DataColumn ("Column C", typeof (decimal)); 
			
			table.Columns.Add (col_a);
			table.Columns.Add (col_b);
			table.Columns.Add (col_c);
			
			table.Rows.Add (new object[] { 1L, "Item X", 10.50M });
			
			factory.GenerateRequests (table);
		}
	}
}
