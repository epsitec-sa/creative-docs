using NUnit.Framework;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class SerialisationTest
	{
		[Test] public void CheckDbExceptionSerialisation()
		{
			DbAccess    dba  = new DbAccess ("p", "d", "s", "ln", "lp", true);
			DbException dbx1 = new DbException (dba, "xyz");
			DbException dbx2;
			
			using (System.IO.Stream stream = System.IO.File.Open ("test.bin", System.IO.FileMode.Create))
			{
				BinaryFormatter formatter = new BinaryFormatter ();
				formatter.Serialize (stream, dbx1);
			}
			
			using (System.IO.Stream stream = System.IO.File.Open ("test.bin", System.IO.FileMode.Open))
			{
				BinaryFormatter formatter = new BinaryFormatter ();
				dbx2 = (DbException) formatter.Deserialize (stream);
			}
			
			System.IO.File.Delete ("test.bin");
			
			Assertion.AssertNotNull (dbx2);
			Assertion.AssertEquals (typeof (DbException), dbx2.GetType ());
			Assertion.AssertEquals (dbx1.Message, dbx2.Message);
			
			using (System.IO.Stream stream = System.IO.File.Open ("test.soap", System.IO.FileMode.Create))
			{
				SoapFormatter formatter = new SoapFormatter ();
				formatter.Serialize (stream, dbx1);
			}
			
			using (System.IO.Stream stream = System.IO.File.Open ("test.soap", System.IO.FileMode.Open))
			{
				SoapFormatter formatter = new SoapFormatter ();
				dbx2 = (DbException) formatter.Deserialize (stream);
			}
			
			using (System.IO.Stream stream = System.IO.File.Open ("test.soap", System.IO.FileMode.Open))
			{
				System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
				byte[] buffer = new byte[stream.Length];
				stream.Read (buffer, 0, (int) stream.Length);
				System.Console.Out.WriteLine ("{0}", encoding.GetString (buffer));
			}
			
			System.IO.File.Delete ("test.soap");
			
			Assertion.AssertNotNull (dbx2);
			Assertion.AssertEquals (typeof (DbException), dbx2.GetType ());
			Assertion.AssertEquals (dbx1.Message, dbx2.Message);
		}
	}
}
