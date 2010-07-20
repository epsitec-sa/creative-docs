using Epsitec.Cresus.DataLayer.Context;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Epsitec.Cresus.DataLayer.UnitTests
{


	[TestClass]
	public class UnitTestDataContextEventArgs
	{


		[ClassInitialize]
		public void Initialize(TestContext testContext)
		{
			TestHelper.Initialize ();

			Database2.CreateAndConnectToDatabase ();

			using (DataContext dataContext = new DataContext (Database1.DbInfrastructure))
			{
				Database2.PupulateDatabase (dataContext);
			}
		}


		[ClassCleanup]
		public void Cleanup()
		{
			Database.DisconnectFromDatabase ();
		}


		[TestMethod]
		public void DataContextEventArgsConstructorTest()
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				new DataContextEventArgs (dataContext);
			}
		}


		[TestMethod]
		public void DataContextTest()
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				DataContextEventArgs eventArg = new DataContextEventArgs (dataContext);

				Assert.AreSame (dataContext, eventArg.DataContext);
			}
		}


	}


}
