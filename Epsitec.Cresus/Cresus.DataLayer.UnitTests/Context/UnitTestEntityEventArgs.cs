using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.UnitTests
{


	[TestClass]
	public sealed class UnitTestEntityEventArgs
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}

		
		[TestMethod]
		public void EntityEventArgsConstructorTest1()
		{
			foreach (AbstractEntity entity in this.GetSampleEntities ())
			{
				foreach (EntityEventType eventType in this.GetSampleTypes ())
				{
					foreach (EntityEventSource eventSource in this.GetSampleSources ())
					{
						var eventArgs = new DataLayer.Context.EntityEventArgs (entity, eventType, eventSource);

						Assert.AreSame (entity, eventArgs.Entity);
						Assert.AreEqual (eventType, eventArgs.EventType);
						Assert.AreEqual (eventSource, eventArgs.EventSource);
					}
				}
			}
		}


		[TestMethod]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void EntityEventArgsConstructorTest2()
		{
			AbstractEntity entity = null;
			EntityEventType eventType = this.GetSampleTypes ().First ();
			EntityEventSource eventSource = this.GetSampleSources ().First ();

			new DataLayer.Context.EntityEventArgs (entity, eventType, eventSource);
		}


		private IEnumerable<AbstractEntity> GetSampleEntities()
		{
			yield return new NaturalPersonEntity ();
			yield return new LocationEntity ();
			yield return new LanguageEntity ();
		}


		private IEnumerable<EntityEventType> GetSampleTypes()
		{
			yield return EntityEventType.Created;
			yield return EntityEventType.Deleted;
			yield return EntityEventType.Updated;
		}


		private IEnumerable<EntityEventSource> GetSampleSources()
		{
			yield return EntityEventSource.External;
			yield return EntityEventSource.Internal;
			yield return EntityEventSource.Synchronization;
		}


	}


}
