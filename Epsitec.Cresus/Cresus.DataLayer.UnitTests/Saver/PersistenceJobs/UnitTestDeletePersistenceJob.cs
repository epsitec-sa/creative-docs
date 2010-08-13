﻿using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.DataLayer.Saver.PersistenceJobs;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.DataLayer.UnitTests.Saver.PersistenceJobs
{


	[TestClass]
	public sealed class UnitTestDeletePersistenceJob
	{


		[TestMethod]
		public void DeletePersistenceJobConstructorTest()
		{
			AbstractEntity entity = new NaturalPersonEntity ();

			var job = new DeletePersistenceJob (entity);

			Assert.AreSame (entity, job.Entity);
		}


		[TestMethod]
		public void DeletePersistenceJobConstructorArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new DeletePersistenceJob (null)
			);
		}


		[TestMethod]
		public void ConvertArgumentCheck()
		{
			AbstractEntity entity = new NaturalPersonEntity ();

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new DeletePersistenceJob (entity).Convert (null)
			);
		}


	}


}
