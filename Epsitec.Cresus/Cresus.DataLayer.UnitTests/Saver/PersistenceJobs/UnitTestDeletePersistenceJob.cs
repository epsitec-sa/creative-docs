using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.DataLayer.Saver.PersistenceJobs;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.DataLayer.UnitTests
{


	[TestClass]
	public sealed class UnitTestDeletePersistenceJob
	{


		[TestMethod]
		public void DeletePersistenceJobConstructorTest1()
		{
			AbstractEntity entity = new NaturalPersonEntity ();

			var job = new DeletePersistenceJob (entity);

			Assert.AreSame (entity, job.Entity);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void DeletePersistenceJobConstructorTest2()
		{
			new DeletePersistenceJob (null);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void ConvertTest()
		{
			AbstractEntity entity = new NaturalPersonEntity ();

			new DeletePersistenceJob (entity).Convert (null);
		}


	}


}
