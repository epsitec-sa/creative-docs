//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using NUnit.Framework;
using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	[TestFixture] public class TypeRosettaTest
	{
		[Test]
		public void CheckObjectTypeToSytemType()
		{
			Assert.AreEqual (null, TypeRosetta.GetSystemTypeFromTypeObject (null));
			
			Assert.AreEqual (typeof (decimal), TypeRosetta.GetSystemTypeFromTypeObject (new DecimalType (0, 10, 0.1M)));
			Assert.AreEqual (typeof (int), TypeRosetta.GetSystemTypeFromTypeObject (new IntegerType (0, 1000)));
			Assert.AreEqual (typeof (string), TypeRosetta.GetSystemTypeFromTypeObject (new StringType ()));
			Assert.AreEqual (typeof (BindingMode), TypeRosetta.GetSystemTypeFromTypeObject (new EnumType (typeof (BindingMode))));
			Assert.AreEqual (typeof (BindingUpdateMode), TypeRosetta.GetSystemTypeFromTypeObject (new EnumType (typeof (BindingUpdateMode))));

			Assert.AreEqual (typeof (string), TypeRosetta.GetSystemTypeFromTypeObject (DependencyObjectTree.NameProperty));

			Assert.AreEqual (typeof (DataObject), TypeRosetta.GetSystemTypeFromTypeObject (DependencyObjectType.FromSystemType (typeof (DataObject))));
		}
	}
}
