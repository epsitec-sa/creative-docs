//	Copyright © 2003-2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD


using Epsitec.Cresus.Database.Exceptions;
using Epsitec.Cresus.Database.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.IO;

using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;


namespace Epsitec.Cresus.Database.Tests.Vs
{


	[TestClass]
	public sealed class UnitTestGenericException
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestMethod]
		public void BinarySerializationTest()
		{
			DbAccess dbAccess  = new DbAccess ("p", "d", "s", "ln", "lp", true);

			GenericException genericException1 = new GenericException (dbAccess, "xyz");
			GenericException genericException2;

			using (Stream stream = File.Open ("test.bin", FileMode.Create))
			{
				BinaryFormatter formatter = new BinaryFormatter ();

				formatter.Serialize (stream, genericException1);
			}

			using (Stream stream = File.Open ("test.bin", FileMode.Open))
			{
				BinaryFormatter formatter = new BinaryFormatter ();

				genericException2 = (GenericException) formatter.Deserialize (stream);
			}

			File.Delete ("test.bin");

			Assert.IsNotNull (genericException2);
			Assert.AreEqual (typeof (GenericException), genericException2.GetType ());
			Assert.AreEqual (genericException1.Message, genericException2.Message);
		}


		[TestMethod]
		public void SoapSerializationTest()
		{
			DbAccess dbAccess  = new DbAccess ("p", "d", "s", "ln", "lp", true);

			GenericException genericException1 = new GenericException (dbAccess, "xyz");
			GenericException genericException2;

			using (Stream stream = File.Open ("test.soap", FileMode.Create))
			{
				SoapFormatter formatter = new SoapFormatter ();

				formatter.Serialize (stream, genericException1);
			}

			using (Stream stream = File.Open ("test.soap", FileMode.Open))
			{
				SoapFormatter formatter = new SoapFormatter ();

				genericException2 = (GenericException) formatter.Deserialize (stream);
			}

			using (Stream stream = File.Open ("test.soap", FileMode.Open))
			{
				byte[] buffer = new byte[stream.Length];

				stream.Read (buffer, 0, (int) stream.Length);
			}

			File.Delete ("test.soap");

			Assert.IsNotNull (genericException2);
			Assert.AreEqual (typeof (GenericException), genericException2.GetType ());
			Assert.AreEqual (genericException1.Message, genericException2.Message);
		}


	}


}
