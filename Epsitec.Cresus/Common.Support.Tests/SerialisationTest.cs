using NUnit.Framework;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;

namespace Epsitec.Common.Support
{
	[TestFixture]
	public class SerialisationTest
	{
		[Test] public void CheckResourceExceptionSerialisation()
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
			
			Assertion.AssertNotNull (exc_2);
			Assertion.AssertEquals (typeof (ResourceException), exc_2.GetType ());
			Assertion.AssertEquals (exc_1.Message, exc_2.Message);
			
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
			
			Assertion.AssertNotNull (exc_2);
			Assertion.AssertEquals (typeof (ResourceException), exc_2.GetType ());
			Assertion.AssertEquals (exc_1.Message, exc_2.Message);
		}
		
		[Test] public void CheckBundleAttributeSerialisation()
		{
			BundleAttribute   bat_1 = new BundleAttribute ("my bundle attribute");
			BundleAttribute   bat_2;
			
			bat_1.DefaultValue = "xyz";
			
			using (System.IO.Stream stream = System.IO.File.Open ("test.bin", System.IO.FileMode.Create))
			{
				BinaryFormatter formatter = new BinaryFormatter ();
				formatter.Serialize (stream, bat_1);
			}
			
			using (System.IO.Stream stream = System.IO.File.Open ("test.bin", System.IO.FileMode.Open))
			{
				BinaryFormatter formatter = new BinaryFormatter ();
				bat_2 = (BundleAttribute)   formatter.Deserialize (stream);
			}
			
			System.IO.File.Delete ("test.bin");
			
			Assertion.AssertNotNull (bat_2);
			Assertion.AssertEquals (typeof (BundleAttribute), bat_2.GetType ());
			Assertion.AssertEquals (bat_1.PropertyName, bat_2.PropertyName);
			Assertion.AssertEquals (bat_1.DefaultValue, bat_2.DefaultValue);
			
			using (System.IO.Stream stream = System.IO.File.Open ("test.soap", System.IO.FileMode.Create))
			{
				SoapFormatter formatter = new SoapFormatter ();
				formatter.Serialize (stream, bat_1);
			}
			
			using (System.IO.Stream stream = System.IO.File.Open ("test.soap", System.IO.FileMode.Open))
			{
				SoapFormatter formatter = new SoapFormatter ();
				bat_2 = (BundleAttribute)   formatter.Deserialize (stream);
			}
			
			using (System.IO.Stream stream = System.IO.File.Open ("test.soap", System.IO.FileMode.Open))
			{
				System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
				byte[] buffer = new byte[stream.Length];
				stream.Read (buffer, 0, (int) stream.Length);
				System.Console.Out.WriteLine ("{0}", encoding.GetString (buffer));
			}
			
			System.IO.File.Delete ("test.soap");
			
			Assertion.AssertNotNull (bat_2);
			Assertion.AssertEquals (typeof (BundleAttribute), bat_2.GetType ());
			Assertion.AssertEquals (bat_1.PropertyName, bat_2.PropertyName);
			Assertion.AssertEquals (bat_1.DefaultValue, bat_2.DefaultValue);
		}
	}
}
