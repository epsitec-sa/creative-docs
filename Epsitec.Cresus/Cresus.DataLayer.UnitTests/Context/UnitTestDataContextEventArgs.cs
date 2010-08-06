using Epsitec.Cresus.DataLayer.Context;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Epsitec.Cresus.DataLayer.UnitTests
{


	[TestClass]
	public sealed class UnitTestDataContextEventArgs
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();

			DatabaseHelper.CreateAndConnectToDatabase ();

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				DatabaseCreator2.PupulateDatabase (dataContext);
			}
		}


		[ClassCleanup]
		public static void ClassCleanup()
		{
			DatabaseHelper.DisconnectFromDatabase ();
		}


		[TestMethod]
		public void DataContextEventArgsConstructorTest()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				new DataContextEventArgs (dataContext);
			}
		}


		[TestMethod]
		public void DataContextTest()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				DataContextEventArgs eventArg = new DataContextEventArgs (dataContext);

				Assert.AreSame (dataContext, eventArg.DataContext);
			}
		}


	}


}
