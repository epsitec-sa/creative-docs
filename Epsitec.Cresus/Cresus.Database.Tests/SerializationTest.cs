using NUnit.Framework;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class SerializationTest
	{
		[Test] public void CheckGenericExceptionSerialization()
		{
			DbAccess    dba  = new DbAccess ("p", "d", "s", "ln", "lp", true);
			Exceptions.GenericException dbx1 = new Exceptions.GenericException (dba, "xyz");
			Exceptions.GenericException dbx2;
			
			using (System.IO.Stream stream = System.IO.File.Open ("test.bin", System.IO.FileMode.Create))
			{
				BinaryFormatter formatter = new BinaryFormatter ();
				formatter.Serialize (stream, dbx1);
			}
			
			using (System.IO.Stream stream = System.IO.File.Open ("test.bin", System.IO.FileMode.Open))
			{
				BinaryFormatter formatter = new BinaryFormatter ();
				dbx2 = (Exceptions.GenericException) formatter.Deserialize (stream);
			}
			
			System.IO.File.Delete ("test.bin");
			
			Assert.IsNotNull (dbx2);
			Assert.AreEqual (typeof (Exceptions.GenericException), dbx2.GetType ());
			Assert.AreEqual (dbx1.Message, dbx2.Message);
			
			using (System.IO.Stream stream = System.IO.File.Open ("test.soap", System.IO.FileMode.Create))
			{
				SoapFormatter formatter = new SoapFormatter ();
				formatter.Serialize (stream, dbx1);
			}
			
			using (System.IO.Stream stream = System.IO.File.Open ("test.soap", System.IO.FileMode.Open))
			{
				SoapFormatter formatter = new SoapFormatter ();
				dbx2 = (Exceptions.GenericException) formatter.Deserialize (stream);
			}
			
			using (System.IO.Stream stream = System.IO.File.Open ("test.soap", System.IO.FileMode.Open))
			{
				System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
				byte[] buffer = new byte[stream.Length];
				stream.Read (buffer, 0, (int) stream.Length);
				System.Console.Out.WriteLine ("{0}", encoding.GetString (buffer));
			}
			
			System.IO.File.Delete ("test.soap");
			
			Assert.IsNotNull (dbx2);
			Assert.AreEqual (typeof (Exceptions.GenericException), dbx2.GetType ());
			Assert.AreEqual (dbx1.Message, dbx2.Message);
		}
	}
}
