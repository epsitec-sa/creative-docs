using NUnit.Framework;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using Epsitec.Common.Support;

namespace Epsitec.Common.Tests.Support
{
	[TestFixture]
	public class SerializationTest
	{
		[Test] public void CheckResourceExceptionSerialization()
		{
			ResourceException exc_1 = new ResourceException ("xyz");
			ResourceException exc_2;
			
			using (System.IO.Stream stream = System.IO.File.Open ("test.bin", System.IO.FileMode.Create))
			{
				BinaryFormatter formatter = new BinaryFormatter ();
				formatter.Serialize (stream, exc_1);
			}
			
			using (System.IO.Stream stream = System.IO.File.Open ("test.bin", System.IO.FileMode.Open))
			{
				BinaryFormatter formatter = new BinaryFormatter ();
				exc_2 = (ResourceException) formatter.Deserialize (stream);
			}
			
			System.IO.File.Delete ("test.bin");
			
			Assert.IsNotNull (exc_2);
			Assert.AreEqual (typeof (ResourceException), exc_2.GetType ());
			Assert.AreEqual (exc_1.Message, exc_2.Message);
			
			using (System.IO.Stream stream = System.IO.File.Open ("test.soap", System.IO.FileMode.Create))
			{
				SoapFormatter formatter = new SoapFormatter ();
				formatter.Serialize (stream, exc_1);
			}
			
			using (System.IO.Stream stream = System.IO.File.Open ("test.soap", System.IO.FileMode.Open))
			{
				SoapFormatter formatter = new SoapFormatter ();
				exc_2 = (ResourceException) formatter.Deserialize (stream);
			}
			
			using (System.IO.Stream stream = System.IO.File.Open ("test.soap", System.IO.FileMode.Open))
			{
				System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
				byte[] buffer = new byte[stream.Length];
				stream.Read (buffer, 0, (int) stream.Length);
				System.Console.Out.WriteLine ("{0}", encoding.GetString (buffer));
			}
			
			System.IO.File.Delete ("test.soap");
			
			Assert.IsNotNull (exc_2);
			Assert.AreEqual (typeof (ResourceException), exc_2.GetType ());
			Assert.AreEqual (exc_1.Message, exc_2.Message);
		}
	}
}
