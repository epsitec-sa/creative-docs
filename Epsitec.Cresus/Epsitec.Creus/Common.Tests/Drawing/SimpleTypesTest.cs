using NUnit.Framework;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Tests.Drawing
{
	[TestFixture] public class SimpleTypesTest
	{
		[Test] public void CheckPointSerialization()
		{
			Point  point_1 = new Point (10, 20.5);
			object point_2;
			
			using (System.IO.Stream stream = System.IO.File.Open ("test.bin", System.IO.FileMode.Create))
			{
				BinaryFormatter formatter = new BinaryFormatter ();
				formatter.Serialize (stream, point_1);
			}
			
			using (System.IO.Stream stream = System.IO.File.Open ("test.bin", System.IO.FileMode.Open))
			{
				BinaryFormatter formatter = new BinaryFormatter ();
				point_2 = formatter.Deserialize (stream);
			}
			
			System.IO.File.Delete ("test.bin");
			
			Assert.IsNotNull (point_2);
			Assert.AreEqual (typeof (Point), point_2.GetType ());
			Assert.AreEqual (point_1, point_2);
			
			using (System.IO.Stream stream = System.IO.File.Open ("test.soap", System.IO.FileMode.Create))
			{
				SoapFormatter formatter = new SoapFormatter ();
				formatter.Serialize (stream, point_1);
			}
			
			using (System.IO.Stream stream = System.IO.File.Open ("test.soap", System.IO.FileMode.Open))
			{
				SoapFormatter formatter = new SoapFormatter ();
				point_2 = formatter.Deserialize (stream);
			}
			
			using (System.IO.Stream stream = System.IO.File.Open ("test.soap", System.IO.FileMode.Open))
			{
				System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
				byte[] buffer = new byte[stream.Length];
				stream.Read (buffer, 0, (int) stream.Length);
				System.Console.Out.WriteLine ("{0}", encoding.GetString (buffer));
			}
			
			System.IO.File.Delete ("test.soap");
			
			Assert.IsNotNull (point_2);
			Assert.AreEqual (typeof (Point), point_2.GetType ());
			Assert.AreEqual (point_1, point_2);
		}
		
		[Test] public void CheckSizeSerialization()
		{
			Size   size_1 = new Size (10, 20.5);
			object size_2;
			
			using (System.IO.Stream stream = System.IO.File.Open ("test.bin", System.IO.FileMode.Create))
			{
				BinaryFormatter formatter = new BinaryFormatter ();
				formatter.Serialize (stream, size_1);
			}
			
			using (System.IO.Stream stream = System.IO.File.Open ("test.bin", System.IO.FileMode.Open))
			{
				BinaryFormatter formatter = new BinaryFormatter ();
				size_2 = formatter.Deserialize (stream);
			}
			
			System.IO.File.Delete ("test.bin");
			
			Assert.IsNotNull (size_2);
			Assert.AreEqual (typeof (Size), size_2.GetType ());
			Assert.AreEqual (size_1, size_2);
		}
		
		[Test] public void CheckColorSerialization()
		{
			Color  color_1 = new Color (0.1, 0.2, 0.3, 0.4);
			object color_2;
			
			using (System.IO.Stream stream = System.IO.File.Open ("test.bin", System.IO.FileMode.Create))
			{
				BinaryFormatter formatter = new BinaryFormatter ();
				formatter.Serialize (stream, color_1);
			}
			
			using (System.IO.Stream stream = System.IO.File.Open ("test.bin", System.IO.FileMode.Open))
			{
				BinaryFormatter formatter = new BinaryFormatter ();
				color_2 = formatter.Deserialize (stream);
			}
			
			System.IO.File.Delete ("test.bin");
			
			Assert.IsNotNull (color_2);
			Assert.AreEqual (typeof (Color), color_2.GetType ());
			Assert.AreEqual (color_1, color_2);
		}
		
		[Test] public void CheckRectangleSerialization()
		{
			Rectangle rect_1 = new Rectangle (5, 6, 10, 20.5);
			object    rect_2;
			
			using (System.IO.Stream stream = System.IO.File.Open ("test.bin", System.IO.FileMode.Create))
			{
				BinaryFormatter formatter = new BinaryFormatter ();
				formatter.Serialize (stream, rect_1);
			}
			
			using (System.IO.Stream stream = System.IO.File.Open ("test.bin", System.IO.FileMode.Open))
			{
				BinaryFormatter formatter = new BinaryFormatter ();
				rect_2 = formatter.Deserialize (stream);
			}
			
			System.IO.File.Delete ("test.bin");
			
			Assert.IsNotNull (rect_2);
			Assert.AreEqual (typeof (Rectangle), rect_2.GetType ());
			Assert.AreEqual (rect_1, rect_2);
		}
	}
}
