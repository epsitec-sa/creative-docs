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
			Assert.IsNull (TypeRosetta.GetSystemTypeFromTypeObject (null));
			Assert.IsNull (TypeRosetta.GetSystemTypeFromTypeObject (15.0));

			Assert.AreEqual (typeof (double), TypeRosetta.GetSystemTypeFromTypeObject (typeof (double)));
			
			Assert.AreEqual (typeof (decimal), TypeRosetta.GetSystemTypeFromTypeObject (new DecimalType (0, 10, 0.1M)));
			Assert.AreEqual (typeof (int), TypeRosetta.GetSystemTypeFromTypeObject (new IntegerType (0, 1000)));
			Assert.AreEqual (typeof (string), TypeRosetta.GetSystemTypeFromTypeObject (new StringType ()));
			Assert.AreEqual (typeof (BindingMode), TypeRosetta.GetSystemTypeFromTypeObject (new EnumType (typeof (BindingMode))));
			Assert.AreEqual (typeof (BindingUpdateMode), TypeRosetta.GetSystemTypeFromTypeObject (new EnumType (typeof (BindingUpdateMode))));

			Assert.AreEqual (typeof (string), TypeRosetta.GetSystemTypeFromTypeObject (DependencyObjectTree.NameProperty));

			Assert.AreEqual (typeof (DataObject), TypeRosetta.GetSystemTypeFromTypeObject (DependencyObjectType.FromSystemType (typeof (DataObject))));
		}

		[Test]
		public void CheckObjectTypeToNamedType()
		{
			INamedType type;

			type = TypeRosetta.GetNamedTypeFromTypeObject (typeof (double));

			Assert.IsNotNull (type);
			Assert.AreEqual ("System.Double", type.Name);
			Assert.AreEqual (typeof (double), type.SystemType);

			type = TypeRosetta.GetNamedTypeFromTypeObject (new IntegerType (0, 100));

			Assert.IsNotNull (type);
			Assert.IsTrue (type is IntegerType);
			Assert.AreEqual ("Integer", type.Name);
			Assert.AreEqual (typeof (int), type.SystemType);

			type = TypeRosetta.GetNamedTypeFromTypeObject (DependencyObjectTree.NameProperty);

			Assert.IsNotNull (type);
			Assert.AreEqual ("System.String", type.Name);
			Assert.AreEqual (typeof (string), type.SystemType);
		}
		
		[Test]
		[ExpectedException (typeof (Exceptions.InvalidTypeObjectException))]
		public void CheckObjectTypeToNamedTypeEx1()
		{
			INamedType type;

			type = TypeRosetta.GetNamedTypeFromTypeObject (15.0);
		}
	}
}
