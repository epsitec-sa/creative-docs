using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Tests.Vs.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;



namespace Epsitec.Common.Tests.Vs.Support.EntityEngine
{


	[TestClass]
	public sealed class UnitTestEntityNullReferenceVirtualizer
	{


		[TestMethod]
		public void FrozenEntityTest()
		{
			ReferenceDataEntity entity = new ReferenceDataEntity ();

			EntityNullReferenceVirtualizer.PatchNullReferences (entity);

			entity.Freeze ();

			Assert.IsNotNull (entity.Reference);
			Assert.IsTrue (entity.Reference.IsReadOnly);
		}


	}


}
