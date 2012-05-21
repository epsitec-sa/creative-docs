using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Tests.Vs.Entities;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Context
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
		public void EntityEventArgsConstructorTest()
		{
			foreach (AbstractEntity entity in this.GetSampleEntities ())
			{
				foreach (EntityChangedEventType eventType in this.GetSampleTypes ())
				{
					foreach (EntityChangedEventSource eventSource in this.GetSampleSources ())
					{
						var eventArgs = new DataLayer.Context.EntityChangedEventArgs (entity, eventType, eventSource);

						Assert.AreSame (entity, eventArgs.Entity);
						Assert.AreEqual (eventType, eventArgs.EventType);
						Assert.AreEqual (eventSource, eventArgs.EventSource);
					}
				}
			}
		}


		[TestMethod]
		public void EntityEventArgsConstructorArgumentCheck()
		{
			AbstractEntity entity = null;
			EntityChangedEventType eventType = this.GetSampleTypes ().First ();
			EntityChangedEventSource eventSource = this.GetSampleSources ().First ();

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new DataLayer.Context.EntityChangedEventArgs (entity, eventType, eventSource)
			);
		}


		private IEnumerable<AbstractEntity> GetSampleEntities()
		{
			yield return new NaturalPersonEntity ();
			yield return new LocationEntity ();
			yield return new LanguageEntity ();
		}


		private IEnumerable<EntityChangedEventType> GetSampleTypes()
		{
			yield return EntityChangedEventType.Created;
			yield return EntityChangedEventType.Deleted;
			yield return EntityChangedEventType.Updated;
		}


		private IEnumerable<EntityChangedEventSource> GetSampleSources()
		{
			yield return EntityChangedEventSource.External;
			yield return EntityChangedEventSource.Internal;
			yield return EntityChangedEventSource.Synchronization;
		}


	}


}
