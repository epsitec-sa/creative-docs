/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


using Epsitec.Common.Support;
using Epsitec.Common.Types;
using NUnit.Framework;
using System.Collections.Generic;

namespace Epsitec.Common.Tests.Types
{
    [TestFixture]
    public class StructuredTest
    {
        [SetUp]
        public void SetUp()
        {
            this.buffer = new System.Text.StringBuilder();

            this.manager = new ResourceManager(@"S:\Epsitec.Cresus\Common.Types.Tests");
            this.manager.DefineDefaultModuleName("Test");
            this.manager.ActivePrefix = "file";
            this.manager.ActiveCulture = Epsitec.Common.Support.Resources.FindCultureInfo("en");
        }

        [Test]
        [Ignore("Crashes the tests execution and prevents other tests from running")]
        public void CheckStructuredData()
        {
            StructuredData data = new StructuredData();

            Assert.AreEqual(0, Collection.Count(data.GetValueIds()));
            Assert.AreEqual(0, Collection.Count(data.StructuredType.GetFieldIds()));

            Assert.IsTrue(data.IsEmpty);

            data.SetValue("A", 10);
            data.SetValue("B", 20);

            Assert.AreEqual(2, Collection.Count(data.GetValueIds()));
            Assert.AreEqual(2, Collection.Count(data.StructuredType.GetFieldIds()));

            Assert.AreEqual("A", Collection.Extract(data.StructuredType.GetFieldIds(), 0));
            Assert.AreEqual("B", Collection.Extract(data.StructuredType.GetFieldIds(), 1));

            Assert.AreEqual(typeof(int), data.StructuredType.GetField("A").Type.SystemType);
            Assert.AreEqual(10, data.GetValue("A"));
            Assert.AreEqual(20, data.GetValue("B"));
            Assert.AreEqual(UndefinedValue.Value, data.GetValue("X"));

            data.SetValue("A", UndefinedValue.Value);

            Assert.AreEqual(1, Collection.Count(data.GetValueIds()));
            Assert.AreEqual(1, Collection.Count(data.StructuredType.GetFieldIds()));

            Assert.AreEqual("B", Collection.Extract(data.StructuredType.GetFieldIds(), 0));

            Assert.IsFalse(data.IsValueLocked("A"));
            Assert.IsFalse(data.IsValueLocked("B"));

            Assert.IsFalse(data.LockValue("A"));
            Assert.IsTrue(data.LockValue("B"));

            Assert.IsFalse(data.IsValueLocked("A"));
            Assert.IsTrue(data.IsValueLocked("B"));

            data.SetValue("A", 10);
            data.SetValue("B", 20);

            data = new StructuredData();

            data.SetValue("U", UndefinedValue.Value);
            Assert.IsTrue(data.IsEmpty);
            data.SetValue("L", new List<string>());

            Assert.IsFalse(data.IsEmpty);
            data.LockValue("L");
            Assert.IsTrue(data.IsEmpty);
            ((List<string>)data.GetValue("L")).Add("x");
            Assert.IsFalse(data.IsEmpty);
            ((List<string>)data.GetValue("L")).Clear();
            Assert.IsTrue(data.IsEmpty);
        }

        [Test]
        public void CheckStructuredDataEquality()
        {
            StructuredData a = new StructuredData();
            StructuredData b = new StructuredData();
            StructuredData c = null;

            Assert.IsTrue(a.Equals(b));
            Assert.IsFalse(a.Equals(c));

            a.SetValue("x", 123);
            a.SetValue("y", 456);

            Assert.IsFalse(a.Equals(b));

            b.SetValue("y", 456);
            b.SetValue("x", 123);

            Assert.IsTrue(a.Equals(b));

            StructuredData x1 = new StructuredData();
            StructuredData x2 = new StructuredData();

            x1.SetValue("A", 1);
            x2.SetValue("A", 1);

            a.SetValue("list", new List<StructuredData>(new StructuredData[] { x1 }));
            b.SetValue("list", new List<StructuredData>(new StructuredData[] { x2 }));

            Assert.IsTrue(a.Equals(b));

            a.SetValue("u", UndefinedValue.Value);

            Assert.IsTrue(a.Equals(b));
        }

        [Test]
        public void CheckStrurctureDataEx1()
        {
            StructuredData data = new StructuredData();

            data.SetValue("A", 10);
            data.LockValue("A");
            Assert.Throws<System.InvalidOperationException>(() => data.SetValue("A", 11));
        }

        [Test]
        public void CheckStrurctureDataEx2()
        {
            StructuredData data = new StructuredData();

            data.SetValue("A", 10);
            data.LockValue("A");
            Assert.Throws<System.InvalidOperationException>(
                () => data.SetValue("A", UndefinedValue.Value)
            );
        }

        [Test]
        public void CheckStructuredDataWithType()
        {
            StructuredType type = new StructuredType();
            StructuredData data = new StructuredData(type);

            type.Fields.Add("A", new IntegerType(0, 100));
            type.Fields.Add("B", new IntegerType(0, 100));

            Assert.AreEqual(2, Collection.Count(data.GetValueIds()));
            Assert.AreEqual(2, Collection.Count(data.StructuredType.GetFieldIds()));

            Assert.AreEqual("A", Collection.Extract(data.StructuredType.GetFieldIds(), 0));
            Assert.AreEqual("B", Collection.Extract(data.StructuredType.GetFieldIds(), 1));

            Assert.AreEqual(UndefinedValue.Value, data.GetValue("A"));
            Assert.AreEqual(UndefinedValue.Value, data.GetValue("B"));

            Assert.AreEqual(-1, data.InternalGetValueCount());

            data.SetValue("A", 10);
            data.SetValue("B", 20);

            Assert.AreEqual(2, data.InternalGetValueCount());
            Assert.AreEqual(typeof(IntegerType), data.StructuredType.GetField("A").Type.GetType());
            Assert.AreEqual(10, data.GetValue("A"));
            Assert.AreEqual(20, data.GetValue("B"));

            this.buffer.Length = 0;

            data.AttachListener("A", this.HandleDataPropertyChanged);
            data.SetValue("A", 15);
            data.SetValue("A", UndefinedValue.Value);
            Assert.AreEqual(2, data.InternalGetValueCount());
            data.DetachListener("A", this.HandleDataPropertyChanged);
            Assert.AreEqual(1, data.InternalGetValueCount());
            data.SetValue("A", 10);
            Assert.AreEqual(2, data.InternalGetValueCount());

            Assert.AreEqual("[A:10->15][A:15-><UndefinedValue>]", this.buffer.ToString());

            data.SetValue("A", UndefinedValue.Value);
            data.SetValue("B", UndefinedValue.Value);
            Assert.AreEqual(0, data.InternalGetValueCount());

            bool original;

            Assert.AreEqual(UndefinedValue.Value, data.GetValue("A", out original));
            Assert.IsTrue(original);

            data.ResetToOriginalValue("A");

            Assert.AreEqual(UndefinedValue.Value, data.GetValue("A", out original));
            Assert.IsTrue(original);

            data.SetValue("A", 10);
            data.SetValue("B", 20);

            data.PromoteToOriginalValue("A");

            Assert.AreEqual(10, data.GetValue("A", out original));
            Assert.IsTrue(original);
            Assert.AreEqual(20, data.GetValue("B", out original));
            Assert.IsFalse(original);

            data.SetValue("A", 15);

            Assert.AreEqual(15, data.GetValue("A", out original));
            Assert.IsFalse(original);

            data.ResetToOriginalValue("A");
            data.ResetToOriginalValue("B");

            Assert.AreEqual(10, data.GetValue("A", out original));
            Assert.IsTrue(original);
            Assert.AreEqual(UndefinedValue.Value, data.GetValue("B", out original));
            Assert.IsTrue(original);

            data.CopyOriginalToCurrentValue("A");

            Assert.AreEqual(10, data.GetValue("A", out original));
            Assert.IsFalse(original);

            data.SetValue("A", UndefinedValue.Value);

            Assert.AreEqual(UndefinedValue.Value, data.GetValue("A", out original));
            Assert.IsFalse(original);

            data.PromoteToOriginalValue("A");
            data.ResetToOriginalValue("A");

            Assert.AreEqual(UndefinedValue.Value, data.GetValue("A", out original));
            Assert.IsTrue(original);
        }

        [Test]
        public void CheckStructuredDataWithTypeAndRelation()
        {
            StructuredType type1 = new StructuredType();
            StructuredType type2 = new StructuredType();

            StructuredData data1 = new StructuredData(type1);
            StructuredData data2 = new StructuredData(type2);

            data1.UndefinedValueMode = UndefinedValueMode.Undefined;
            data2.UndefinedValueMode = UndefinedValueMode.Undefined;

            type1.Fields.Add(
                new StructuredTypeField("A", type2, Druid.Empty, 0, FieldRelation.Reference)
            );
            type1.Fields.Add(
                new StructuredTypeField("B", type2, Druid.Empty, 1, FieldRelation.Collection)
            );

            type2.Fields.Add("X", new IntegerType(0, 100));
            type2.DefineIsNullable(true);

            List<StructuredData> list = new List<StructuredData>();

            Assert.AreEqual(UndefinedValue.Value, data1.GetValue("A"));
            Assert.AreEqual(UndefinedValue.Value, data1.GetValue("B"));
            Assert.AreEqual(UnknownValue.Value, data1.GetValue("C"));
            Assert.AreEqual(UndefinedValue.Value, data2.GetValue("X"));
            Assert.AreEqual(UnknownValue.Value, data2.GetValue("Y"));

            data1.SetValue("A", data2);
            data1.SetValue("B", list);

            data2.SetValue("X", 10);

            //	We can use an empty list of StructuredData (there is no possible
            //	verification) or a list which contains a first item of the proper
            //	structured type :

            Assert.IsTrue(TypeRosetta.IsValidValue(list, type1.GetField("B")));
            list.Add(data2);
            Assert.IsTrue(TypeRosetta.IsValidValue(list, type1.GetField("B")));

            //	...but we cannot use a list of StructuredData of the wrong type.

            list.Clear();
            list.Add(data1);
            Assert.IsFalse(TypeRosetta.IsValidValue(list, type1.GetField("B")));
        }

        [Test]
        public void CheckStructuredDataWithTypeEx1()
        {
            StructuredType type = new StructuredType();
            StructuredData data = new StructuredData(type);

            type.Fields.Add("A", IntegerType.Default);
            type.Fields.Add("B", IntegerType.Default);

            Assert.Throws<System.Collections.Generic.KeyNotFoundException>(
                () => data.SetValue("X", 100)
            );
        }

        [Test]
        [Ignore("Crashes the tests execution and prevents other tests from running")]
        public void CheckStructuredDataWithTypeEx2()
        {
            StructuredType type = new StructuredType();
            StructuredData data = new StructuredData(type);

            type.Fields.Add("A", IntegerType.Default);
            type.Fields.Add("B", IntegerType.Default);

            Assert.IsTrue(UnknownValue.IsUnknownValue(data.GetValue("X")));
        }

        [Test]
        [Ignore("Crashes the tests execution and prevents other tests from running")]
        public void CheckStructuredDataWithTypeEx3()
        {
            StructuredType type = new StructuredType();
            StructuredData data = new StructuredData(type);

            type.Fields.Add("A", new IntegerType(0, 100));

            Assert.Throws<System.ArgumentException>(() => data.SetValue("A", 200));
        }

        [Test]
        [Ignore("Crashes the tests execution and prevents other tests from running")]
        public void CheckStructuredDataWithTypeEx4()
        {
            StructuredType type = new StructuredType();
            StructuredData data = new StructuredData(type);

            type.Fields.Add("A", new IntegerType(0, 100));

            Assert.Throws<System.ArgumentException>(() => data.SetValue("A", "-"));
        }

        [Test]
        public void CheckStructuredTreeIsPathValid()
        {
            StructuredType record = new StructuredType();

            StructuredTest.Fill(record);

            Assert.IsTrue(StructuredTree.IsPathValid(record, "Number1"));
            Assert.IsTrue(StructuredTree.IsPathValid(record, "Number2"));
            Assert.IsTrue(StructuredTree.IsPathValid(record, "Text1"));
            Assert.IsFalse(StructuredTree.IsPathValid(record, "Text2"));
            Assert.IsFalse(StructuredTree.IsPathValid(record, null));
            Assert.IsFalse(StructuredTree.IsPathValid(record, ""));
            Assert.IsTrue(StructuredTree.IsPathValid(record, "Personne"));
            Assert.IsTrue(StructuredTree.IsPathValid(record, "Personne.Nom"));
            Assert.IsTrue(StructuredTree.IsPathValid(record, "Personne.Prénom"));
            Assert.IsTrue(StructuredTree.IsPathValid(record, "Personne.Adresse"));
            Assert.IsTrue(StructuredTree.IsPathValid(record, "Personne.Adresse.NPA"));
            Assert.IsTrue(StructuredTree.IsPathValid(record, "Personne.Adresse.Ville"));
        }

        [Test]
        public void CheckStructuredTreeGetField()
        {
            StructuredType record = new StructuredType();

            StructuredTest.Fill(record);

            Assert.IsTrue(StructuredTree.GetField(record, "Number1").Type is DecimalType);
            Assert.IsTrue(StructuredTree.GetField(record, "Text1").Type is StringType);

            Assert.IsTrue(StructuredTree.GetField(record, "Personne").Type is StructuredType);
            Assert.IsTrue(
                StructuredTree.GetField(record, "Personne.Adresse").Type is StructuredType
            );
            Assert.IsTrue(
                StructuredTree.GetField(record, "Personne.Adresse.NPA").Type is IntegerType
            );
        }

        [Test]
        public void CheckStructuredTreeGetSampleValue()
        {
            StringType strType = new StringType();
            DateType dateType = new DateType();

            strType.DefineSampleValue("Abc");
            dateType.DefineSampleValue(Date.Today);

            StructuredType rec1 = new StructuredType();
            StructuredType rec2 = new StructuredType();
            StructuredType rec3 = new StructuredType();

            rec1.Fields.Add("Employee", rec2);
            rec1.Fields.Add("Comment", strType);

            rec2.Fields.Add("FirstName", strType);
            rec2.Fields.Add("LastName", strType);
            rec2.Fields.Add("History", rec3);

            rec3.Fields.Add("HireDate", dateType);
            rec3.Fields.Add("FireDate", dateType);

            StructuredData data = new StructuredData(rec1);

            Assert.IsNotNull(StructuredTree.GetSampleValue(data, "Employee"));
            Assert.AreEqual(
                typeof(StructuredData),
                StructuredTree.GetSampleValue(data, "Employee").GetType()
            );
            Assert.AreEqual("Abc", StructuredTree.GetSampleValue(data, "Comment"));

            Assert.IsNotNull(StructuredTree.GetSampleValue(data, "Employee.FirstName"));
            Assert.IsNotNull(StructuredTree.GetSampleValue(data, "Employee.LastName"));
            Assert.IsNotNull(StructuredTree.GetSampleValue(data, "Employee.History"));

            Assert.AreEqual("Abc", StructuredTree.GetSampleValue(data, "Employee.FirstName"));
            Assert.AreEqual("Abc", StructuredTree.GetSampleValue(data, "Employee.LastName"));

            Assert.AreEqual(
                Date.Today,
                StructuredTree.GetSampleValue(data, "Employee.History.HireDate")
            );
            Assert.AreEqual(
                Date.Today,
                StructuredTree.GetSampleValue(data, "Employee.History.FireDate")
            );
        }

        [Test]
        public void CheckStructuredTreeGetValue()
        {
            StructuredType record = new StructuredType();
            StructuredData data = new StructuredData(record);

            record.Fields.Add("X", StringType.NativeDefault);
            record.Fields.Add("Z", StringType.NativeDefault);

            data.SetValue("Z", "z");

            Assert.AreEqual(UndefinedValue.Value, StructuredTree.GetValue(data, "X"));
            Assert.AreEqual(UnknownValue.Value, StructuredTree.GetValue(data, "Y"));
            Assert.AreEqual("z", StructuredTree.GetValue(data, "Z"));
        }

        [Test]
        public void CheckStructuredTreeMisc()
        {
            Assert.AreEqual("a*b*c", string.Join("*", StructuredTree.SplitPath("a.b.c")));
            Assert.AreEqual("a.b.c.d", StructuredTree.CreatePath("a", "b", "c.d"));
            Assert.AreEqual("", StructuredTree.CreatePath());
            Assert.AreEqual("", StructuredTree.CreatePath(""));
            Assert.AreEqual(0, StructuredTree.SplitPath(null).Length);
            Assert.AreEqual(0, StructuredTree.SplitPath("").Length);
            Assert.AreEqual("a.b.c.d", StructuredTree.GetSubPath("a.b.c.d", 0));
            Assert.AreEqual("b.c.d", StructuredTree.GetSubPath("a.b.c.d", 1));
            Assert.AreEqual("c.d", StructuredTree.GetSubPath("a.b.c.d", 2));
            Assert.AreEqual("", StructuredTree.GetSubPath("a.b.c.d", 10));
            Assert.AreEqual("a", StructuredTree.GetRootName("a.b.c.d"));
            Assert.AreEqual("abc", StructuredTree.GetRootName("abc"));
            Assert.AreEqual("", StructuredTree.GetRootName(""));
            Assert.IsNull(StructuredTree.GetRootName(null));

            string leafPath;
            string leafName;

            leafPath = StructuredTree.GetLeafPath("a.b.c.d", out leafName);

            Assert.AreEqual("a.b.c", leafPath);
            Assert.AreEqual("d", leafName);

            leafPath = StructuredTree.GetLeafPath("a", out leafName);

            Assert.IsNull(leafPath);
            Assert.AreEqual("a", leafName);

            leafPath = StructuredTree.GetLeafPath("", out leafName);

            Assert.IsNull(leafPath);
            Assert.IsNull(leafName);
        }

        [Test]
        public void CheckStructuredTree()
        {
            StructuredType type = new StructuredType();
            StructuredData record = new StructuredData(null);

            Assert.AreEqual(0, Collection.Count(type.GetFieldIds()));

            StructuredTest.Fill(type);

            record = new StructuredData(type);

            Assert.AreEqual(
                "Number1/Number2/Personne/Text1",
                string.Join("/", Collection.ToSortedArray(type.GetFieldIds()))
            );
            Assert.AreEqual(
                "Personne.Adresse/Personne.Nom/Personne.Prénom",
                string.Join(
                    "/",
                    Collection.ToSortedArray(StructuredTree.GetFieldPaths(type, "Personne"))
                )
            );
            Assert.AreEqual(
                "Personne.Adresse.NPA/Personne.Adresse.Ville",
                string.Join(
                    "/",
                    Collection.ToSortedArray(StructuredTree.GetFieldPaths(type, "Personne.Adresse"))
                )
            );

            Assert.AreEqual(0, Collection.Count(StructuredTree.GetFieldPaths(type, "Number1")));
            Assert.AreEqual(
                0,
                Collection.Count(StructuredTree.GetFieldPaths(type, "Personne.Adresse.Ville"))
            );
            Assert.AreEqual(0, Collection.Count(StructuredTree.GetFieldPaths(type, "X")));

            Assert.AreEqual(
                "Number1/Number2/Personne/Text1",
                string.Join("/", Collection.ToSortedArray(record.StructuredType.GetFieldIds()))
            );
            Assert.AreEqual(
                "Personne.Adresse/Personne.Nom/Personne.Prénom",
                string.Join(
                    "/",
                    Collection.ToSortedArray(
                        StructuredTree.GetFieldPaths(record.StructuredType, "Personne")
                    )
                )
            );
            Assert.AreEqual(
                "Personne.Adresse.NPA/Personne.Adresse.Ville",
                string.Join(
                    "/",
                    Collection.ToSortedArray(
                        StructuredTree.GetFieldPaths(record.StructuredType, "Personne.Adresse")
                    )
                )
            );

            Assert.AreEqual(
                0,
                Collection.Count(StructuredTree.GetFieldPaths(record.StructuredType, "Number1"))
            );
            Assert.AreEqual(
                0,
                Collection.Count(
                    StructuredTree.GetFieldPaths(record.StructuredType, "Personne.Adresse.Ville")
                )
            );
            Assert.AreEqual(
                0,
                Collection.Count(StructuredTree.GetFieldPaths(record.StructuredType, "X"))
            );

            Assert.AreEqual(
                typeof(DecimalType),
                record.StructuredType.GetField("Number1").Type.GetType()
            );
            Assert.AreEqual(
                typeof(StringType),
                record.StructuredType.GetField("Text1").Type.GetType()
            );
            Assert.AreEqual(
                typeof(StructuredType),
                record.StructuredType.GetField("Personne").Type.GetType()
            );

            Assert.IsTrue(StructuredTree.IsPathValid(type, "Number1"));
            Assert.IsTrue(StructuredTree.IsPathValid(type, "Personne.Nom"));
            Assert.IsTrue(StructuredTree.IsPathValid(type, "Personne.Adresse.Ville"));
            Assert.IsFalse(StructuredTree.IsPathValid(type, "Personne.Adresse.Pays"));
            Assert.IsFalse(StructuredTree.IsPathValid(type, "Client.Adresse.Pays"));
        }

        [Test]
        public void CheckStructuredTypeEx1()
        {
            StructuredType type = new StructuredType(StructuredTypeClass.Entity);
            type.SetValue(StructuredType.DebugDisableChecksProperty, true);

            Assert.Throws<System.ArgumentException>(
                () =>
                    type.Fields.Add(
                        new StructuredTypeField(
                            "Name",
                            StringType.NativeDefault,
                            Druid.Empty,
                            0,
                            FieldRelation.Reference
                        )
                    )
            );
        }

        [Test]
        [Ignore("Broken test")]
        public void CheckStructuredTypeEx2()
        {
            StructuredType type = new StructuredType(StructuredTypeClass.Entity);
            type.SetValue(StructuredType.DebugDisableChecksProperty, true);
            Druid typeId = Druid.Parse("[123456780]");

            TypeRosetta.RecordType(typeId, type);

            type.DefineCaptionId(typeId);

            type.Fields.Add(
                new StructuredTypeField(
                    "Name",
                    StringType.NativeDefault,
                    Druid.Empty,
                    0,
                    FieldRelation.None
                )
            );
            //-         Assert.Throws<System.InvalidOperationException>(() => type.Fields.Add (new StructuredTypeField ("SelfName", type, Druid.Empty, 1, FieldRelation.Inclusion)));
        }

        [Test]
        [Ignore("Broken test")]
        public void CheckStructuredTypeEx3()
        {
            StructuredType interf = new StructuredType(StructuredTypeClass.Interface);
            StructuredType entity = new StructuredType(StructuredTypeClass.Entity);
            interf.SetValue(StructuredType.DebugDisableChecksProperty, true);
            entity.SetValue(StructuredType.DebugDisableChecksProperty, true);

            Druid typeId1 = Druid.Parse("[123456781]");
            Druid typeId2 = Druid.Parse("[123456782]");

            TypeRosetta.RecordType(typeId1, interf);
            TypeRosetta.RecordType(typeId2, entity);

            interf.DefineCaptionId(typeId1);
            entity.DefineCaptionId(typeId2);

            interf.Fields.Add(
                new StructuredTypeField(
                    "Name",
                    StringType.NativeDefault,
                    Druid.Empty,
                    0,
                    FieldRelation.None
                )
            );

            //-			entity.Fields.Add (new StructuredTypeField ("Name", interf, Druid.Empty, 1, FieldRelation.Inclusion));
            //-			entity.Fields.Add (new StructuredTypeField ("Name", interf, Druid.Empty, 2, FieldRelation.Inclusion));
            //-         assert.Throws<System.ArgumentException>(() => );
        }

        [Test]
        public void CheckStructuredTypeInclusion()
        {
            //Assert.IsNull (Epsitec.Common.Support.Res.Types.TestInterfaceUser.BaseType);
            //Assert.AreEqual (1, Collection.Count (Epsitec.Common.Support.Res.Types.TestInterfaceUser.GetInterfaceIds (false)));
            //Assert.AreEqual (Epsitec.Common.Support.Res.Types.TestInterface.CaptionId, Collection.GetFirst (Epsitec.Common.Support.Res.Types.TestInterfaceUser.GetInterfaceIds (false)));

            //string[] ids = Collection.ToArray (Epsitec.Common.Support.Res.Types.TestInterfaceUser.GetFieldIds ());

            //Assert.AreEqual (3, ids.Length);
            //Assert.AreEqual ("[700J2]", ids[0]);	//	from interface, "Name" -- field redefined by TestInterfaceUser
            //Assert.AreEqual ("[7012]", ids[1]);		//	from interface, "Resource"
            //Assert.AreEqual ("[7014]", ids[2]);		//	locally defined, "Extension1"

            //Assert.AreEqual (FieldSource.Expression, Epsitec.Common.Support.Res.Types.TestInterfaceUser.GetField ("[700J2]").Source);
            //Assert.AreEqual (FieldSource.Expression, Epsitec.Common.Support.Res.Types.TestInterfaceUser.GetField ("[7012]").Source);
            //Assert.AreEqual (FieldSource.Expression, Epsitec.Common.Support.Res.Types.TestInterfaceUser.GetField ("[7014]").Source);

            //Assert.AreEqual (Epsitec.Common.Support.Res.Types.TestInterface.CaptionId, Epsitec.Common.Support.Res.Types.TestInterfaceUser.GetField ("[700J2]").DefiningTypeId);
            //Assert.AreEqual (Epsitec.Common.Support.Res.Types.TestInterface.CaptionId, Epsitec.Common.Support.Res.Types.TestInterfaceUser.GetField ("[7012]").DefiningTypeId);
            //Assert.AreEqual (Druid.Empty, Epsitec.Common.Support.Res.Types.TestInterfaceUser.GetField ("[7014]").DefiningTypeId);

            //Assert.AreEqual (FieldMembership.LocalOverride, Epsitec.Common.Support.Res.Types.TestInterfaceUser.GetField ("[700J2]").Membership);
            //Assert.AreEqual (FieldMembership.Local, Epsitec.Common.Support.Res.Types.TestInterfaceUser.GetField ("[7012]").Membership);
            //Assert.AreEqual (FieldMembership.Local, Epsitec.Common.Support.Res.Types.TestInterfaceUser.GetField ("[7014]").Membership);

            //Assert.AreEqual ("»/c#\r\nx => string.Empty", Epsitec.Common.Support.Res.Types.TestInterfaceUser.GetField ("[700J2]").Expression);
            //Assert.AreEqual ("»/c#\r\nx => x.Name.ToUpper ()", Epsitec.Common.Support.Res.Types.TestInterfaceUser.GetField ("[7014]").Expression);
        }

        [Test]
        public void CheckStructuredTypeRelations()
        {
            StructuredType interf = new StructuredType(StructuredTypeClass.Interface);
            StructuredType entity = new StructuredType(StructuredTypeClass.Entity);
            interf.SetValue(StructuredType.DebugDisableChecksProperty, true);
            entity.SetValue(StructuredType.DebugDisableChecksProperty, true);

            Druid typeId = Druid.Parse("[12345678A]");

            TypeRosetta.RecordType(typeId, interf);

            interf.DefineCaptionId(typeId);

            interf.Fields.Add(
                new StructuredTypeField(
                    "Name",
                    StringType.NativeDefault,
                    Druid.Empty,
                    0,
                    FieldRelation.None
                )
            );
            interf.Fields.Add(
                new StructuredTypeField("SelfRef", interf, Druid.Empty, 1, FieldRelation.Reference)
            );
            interf.Fields.Add(
                new StructuredTypeField(
                    "SelfCollection",
                    interf,
                    Druid.Empty,
                    2,
                    FieldRelation.Collection
                )
            );

            //-			entity.Fields.Add (new StructuredTypeField ("Name", interf, Druid.Empty, 0, FieldRelation.Inclusion));
            //-			entity.Fields.Add (new StructuredTypeField ("SelfRef", interf, Druid.Empty, 0, FieldRelation.Inclusion));
        }

        [Test]
        public void CheckStructuredTypeSerialization()
        {
            StructuredType type = new StructuredType(StructuredTypeClass.Entity);
            type.SetValue(StructuredType.DebugDisableChecksProperty, true);
            Druid typeId = Druid.Parse("[12345678B]");

            type.Caption.Name = "TestStruct";
            type.DefineCaptionId(typeId);

            type.Fields.Add("Name", StringType.NativeDefault);
            type.Fields.Add("Age", IntegerType.Default);

            type.Fields["Name"] = new StructuredTypeField(
                "Name",
                StringType.NativeDefault,
                Druid.Empty,
                1
            );
            type.InterfaceIds.Add(Druid.Parse("[12345678C]"));
            type.InterfaceIds.Add(Druid.Parse("[12345678D]"));

            Assert.IsNull(type.SystemType);

            TypeRosetta.RecordType(typeId, type);

            string xml = type.Caption.SerializeToString();

            System.Console.Out.WriteLine("StructuredType: {0}", xml);

            Caption caption = new Caption();
            caption.DeserializeFromString(xml);

            StructuredType restoredType = TypeRosetta.CreateTypeObject(caption) as StructuredType;

            Assert.AreEqual(2, type.InterfaceIds.Count);
            Assert.AreEqual(Druid.Parse("[12345678C]"), type.InterfaceIds[0]);
            Assert.AreEqual(Druid.Parse("[12345678D]"), type.InterfaceIds[1]);

            //	-----------------------------------------------------------------
            //	We need to clear the interface list since they don't really exist
            //	and the access to the Fields property would crash :
            type.InterfaceIds.Clear();
            restoredType.InterfaceIds.Clear();
            //	-----------------------------------------------------------------

            Assert.AreEqual(type.Fields.Count, restoredType.Fields.Count);
            Assert.AreEqual(
                type.GetField("Name").Type.GetType().Name,
                restoredType.GetField("Name").Type.GetType().Name
            );
            Assert.AreEqual(
                type.GetField("Age").Type.GetType().Name,
                restoredType.GetField("Age").Type.GetType().Name
            );
            Assert.AreEqual("Name", restoredType.Fields["Name"].Id);
            Assert.AreEqual("Age", restoredType.Fields["Age"].Id);
            Assert.AreEqual(1, restoredType.Fields["Name"].Rank);
            Assert.AreEqual(-1, restoredType.Fields["Age"].Rank);
            Assert.AreEqual(StructuredTypeClass.Entity, restoredType.Class);

            List<StructuredTypeField> fields = new List<StructuredTypeField>(
                restoredType.Fields.Values
            );

            Assert.AreEqual("Name", fields[0].Id);
            Assert.AreEqual("Age", fields[1].Id);

            fields.Sort(StructuredType.RankComparer);

            Assert.AreEqual("Age", fields[0].Id);
            Assert.AreEqual("Name", fields[1].Id);

            string[] fieldIds = Collection.ToArray<string>(restoredType.GetFieldIds());

            Assert.AreEqual("Age", fieldIds[0]);
            Assert.AreEqual("Name", fieldIds[1]);

            restoredType.Fields.Remove("Age");
            Assert.AreEqual(1, restoredType.Fields.Count);

            restoredType.Fields.Remove("Xxx");
            Assert.AreEqual(1, restoredType.Fields.Count);
        }

        [Test]
        public void CheckStructuredTypeFieldInsertionEx1()
        {
            StructuredType type = new StructuredType();
            StructuredTypeField fieldAbc = new StructuredTypeField("Abc", StringType.NativeDefault);
            StructuredTypeField fieldXyz = new StructuredTypeField("Xyz", StringType.NativeDefault);

            type.Fields.Add(fieldXyz);
            Assert.Throws<System.ArgumentException>(() =>
            {
                type.Fields["Xyz"] = fieldAbc;
            });
        }

        [Test]
        public void CheckStructuredTypeCreateEntities()
        {
            StructuredType e1;
            StructuredType e2;
            StructuredType e3;

            this.CreateEntities(out e1, out e2, out e3);
        }

        [Test]
        public void CheckStructuredTypeCreateInterface()
        {
            StructuredType i = this.CreateInterface();

            Assert.AreEqual(2, i.Fields.Count);
            Assert.AreEqual("[400I]", Collection.ToArray(i.GetFieldIds())[0]);
            Assert.AreEqual("[400J]", Collection.ToArray(i.GetFieldIds())[1]);

            StructuredType e1;
            StructuredType e2;
            StructuredType e3;

            this.CreateEntities(out e1, out e2, out e3);

            int n = e1.Fields.Count;

            e1.InterfaceIds.Add(i.CaptionId);

            Assert.AreEqual(n + 2, e1.Fields.Count);

            Assert.AreEqual(FieldMembership.Local, e1.Fields["[400E]"].Membership);
            Assert.AreEqual(FieldMembership.Local, e1.Fields["[400F]"].Membership);
            Assert.AreEqual(FieldMembership.Local, e1.Fields["[400I]"].Membership);
            Assert.AreEqual(FieldMembership.Local, e1.Fields["[400J]"].Membership);

            Assert.AreEqual(Druid.Empty, e1.Fields["[400E]"].DefiningTypeId);
            Assert.AreEqual(Druid.Empty, e1.Fields["[400F]"].DefiningTypeId);
            Assert.AreEqual(i.CaptionId, e1.Fields["[400I]"].DefiningTypeId);
            Assert.AreEqual(i.CaptionId, e1.Fields["[400J]"].DefiningTypeId);

            Assert.AreEqual(1, e1.GetInterfaceIds(false).Count);
            Assert.AreEqual("[400H]", e1.GetInterfaceIds(false)[0].ToString());
        }

        [Test]
        public void CheckStructuredTypeMerge1()
        {
            StructuredType e1;
            StructuredType e2;
            StructuredType e3;

            this.CreateEntities(out e1, out e2, out e3);

            StructuredType e12 = StructuredType.Merge(e1, e2);
            StructuredType e21 = StructuredType.Merge(e2, e1);
            StructuredType e13 = StructuredType.Merge(e1, e3);
            StructuredType e31 = StructuredType.Merge(e3, e1);

            Assert.AreEqual("E2", e12.Name); //	E1 merged with E2, caption of E2 wins
            Assert.AreEqual("E1", e21.Name); //	E2 merged with E1, caption of E1 wins
            Assert.AreEqual("U1", e13.Name); //	E1 merged with U1, U1 wins as it is of a higher layer
            Assert.AreEqual("U1", e31.Name); //	U1 merged with E1, U1 wins as it is of a higher layer

            Assert.AreEqual(ResourceModuleLayer.Application, e1.Module.Layer);
            Assert.AreEqual(ResourceModuleLayer.Application, e2.Module.Layer);
            Assert.AreEqual(ResourceModuleLayer.User, e3.Module.Layer);

            Assert.AreEqual(ResourceModuleLayer.Application, e12.Module.Layer);
            Assert.AreEqual(ResourceModuleLayer.Application, e21.Module.Layer);
            Assert.AreEqual(ResourceModuleLayer.Application, e13.Module.Layer); //	App+User --> App
            Assert.AreEqual(ResourceModuleLayer.Application, e31.Module.Layer); //	User+App --> App

            Assert.AreEqual(StructuredTypeClass.Entity, e12.Class);
            Assert.AreEqual(StructuredTypeClass.Entity, e21.Class);
        }

        [Test]
        public void CheckStructuredTypeMerge2()
        {
            StructuredType e1;
            StructuredType e2;
            StructuredType e3;

            this.CreateEntities(out e1, out e2, out e3);

            StructuredType e12 = StructuredType.Merge(e1, e2);
            StructuredType e21 = StructuredType.Merge(e2, e1);
            StructuredType e13 = StructuredType.Merge(e1, e3);
            StructuredType e31 = StructuredType.Merge(e3, e1);

            string[] e12Fields = Collection.ToArray(e12.GetFieldIds());
            string[] e21Fields = Collection.ToArray(e21.GetFieldIds());
            string[] e13Fields = Collection.ToArray(e13.GetFieldIds());
            string[] e31Fields = Collection.ToArray(e31.GetFieldIds());

            Assert.AreEqual(3, e12.Fields.Count);
            Assert.AreEqual(3, e21.Fields.Count);

            //	Verify field merge and rank assignment :

            Assert.AreEqual("[400E]", e12Fields[0]); //	E1.X
            Assert.AreEqual("[400F]", e12Fields[1]); //	E1.Y
            Assert.AreEqual("[400G]", e12Fields[2]); //	E2.Z
            Assert.AreEqual(0, e12.Fields[e12Fields[0]].Rank); //	E1.X, rank = 0
            Assert.AreEqual(1, e12.Fields[e12Fields[1]].Rank); //	E1.Y, rank = 1
            Assert.AreEqual(2, e12.Fields[e12Fields[2]].Rank); //	E2.Z, rank = 2

            Assert.AreEqual("[400G]", e21Fields[0]); //	E2.Z
            Assert.AreEqual("[400E]", e21Fields[1]); //	E1.X
            Assert.AreEqual("[400F]", e21Fields[2]); //	E1.Y
            Assert.AreEqual(0, e21.Fields[e21Fields[0]].Rank); //	E2.Z, rank = 0
            Assert.AreEqual(1, e21.Fields[e21Fields[1]].Rank); //	E1.X, rank = 1
            Assert.AreEqual(2, e21.Fields[e21Fields[2]].Rank); //	E1.Y, rank = 2

            Assert.AreEqual(4, e13.Fields.Count);
            Assert.AreEqual(4, e31.Fields.Count);

            Assert.AreEqual("[400E]", e13Fields[0]); //	E1.X
            Assert.AreEqual("[400F]", e13Fields[1]); //	E1.Y
            Assert.AreEqual("[V002]", e13Fields[2]); //	U1.V
            Assert.AreEqual("[V003]", e13Fields[3]); //	U1.W

            Assert.AreEqual("[400E]", e31Fields[0]); //	E1.X
            Assert.AreEqual("[400F]", e31Fields[1]); //	E1.Y
            Assert.AreEqual("[V002]", e31Fields[2]); //	U1.V
            Assert.AreEqual("[V003]", e31Fields[3]); //	U1.W
        }

        [Test]
        public void CheckStructuredTypeMerge3()
        {
            StructuredType e1;
            StructuredType e2;
            StructuredType e3;

            this.CreateEntities(out e1, out e2, out e3);

            e2.Fields.Add(
                new StructuredTypeField(null, IntegerType.Default, Druid.Parse("[400E]"), 1)
            );

            Assert.AreEqual(StringType.NativeDefault, e1.Fields["[400E]"].Type);
            Assert.AreEqual(StringType.NativeDefault, e1.Fields["[400F]"].Type);

            Assert.AreEqual(StringType.NativeDefault, e2.Fields["[400G]"].Type);
            Assert.AreEqual(IntegerType.Default, e2.Fields["[400E]"].Type);

            //	Merging two entities where the second one redefines a field of the
            //	first one (field [400E] is redefined to be of type Integer) :

            StructuredType e12 = StructuredType.Merge(e1, e2);

            string[] e12Fields = Collection.ToArray(e12.GetFieldIds());

            Assert.AreEqual(3, e12.Fields.Count);

            Assert.AreEqual("[400E]", e12Fields[0]); //	E2.X <--
            Assert.AreEqual("[400F]", e12Fields[1]); //	E1.Y
            Assert.AreEqual("[400G]", e12Fields[2]); //	E2.Z

            Assert.AreEqual(0, e12.Fields[e12Fields[0]].Rank); //	E1.X, rank = 0
            Assert.AreEqual(1, e12.Fields[e12Fields[1]].Rank); //	E1.Y, rank = 1
            Assert.AreEqual(2, e12.Fields[e12Fields[2]].Rank); //	E2.Z, rank = 2

            Assert.AreEqual(IntegerType.Default, e12.Fields[e12Fields[0]].Type);
            Assert.AreEqual(StringType.NativeDefault, e12.Fields[e12Fields[1]].Type);
            Assert.AreEqual(StringType.NativeDefault, e12.Fields[e12Fields[2]].Type);
        }

        [Test]
        public void CheckStructuredTypeMergeEx1()
        {
            StructuredType e1;
            StructuredType e2;
            StructuredType e3;

            this.CreateEntities(out e1, out e2, out e3);

            e1.SetValue(StructuredType.ClassProperty, StructuredTypeClass.Interface);

            //	We cannot merge two entities of different classes; verify
            //	that this raises the ArgumentException exception :

            Assert.Throws<System.ArgumentException>(() => StructuredType.Merge(e1, e2));
        }

        private void CreateEntities(
            out StructuredType e1,
            out StructuredType e2,
            out StructuredType e3
        )
        {
            //	Manually create 3 entities based on captions stored in the Test
            //	and OtherModule modules :

            e1 = new StructuredType(StructuredTypeClass.Entity, Druid.Empty);
            e2 = new StructuredType(StructuredTypeClass.Entity, Druid.Empty);
            e3 = new StructuredType(StructuredTypeClass.Entity, Druid.Empty);

            e1.DefineCaption(this.manager.GetCaption(Druid.Parse("[400C]"))); //	from Test, Application layer
            e2.DefineCaption(this.manager.GetCaption(Druid.Parse("[400D]"))); //	from Test, Application layer
            e3.DefineCaption(this.manager.GetCaption(Druid.Parse("[V001]"))); //	from OtherModule, User layer

            Assert.AreEqual(ResourceModuleLayer.Application, e1.Module.Layer);
            Assert.AreEqual(ResourceModuleLayer.Application, e2.Module.Layer);
            Assert.AreEqual(ResourceModuleLayer.User, e3.Module.Layer);

            e1.Fields.Add(
                new StructuredTypeField(null, StringType.NativeDefault, Druid.Parse("[400E]"), 0)
            );
            e1.Fields.Add(
                new StructuredTypeField(null, StringType.NativeDefault, Druid.Parse("[400F]"), 1)
            );
            e2.Fields.Add(
                new StructuredTypeField(null, StringType.NativeDefault, Druid.Parse("[400G]"), 0)
            );
            e3.Fields.Add(
                new StructuredTypeField(null, StringType.NativeDefault, Druid.Parse("[V002]"), 0)
            );
            e3.Fields.Add(
                new StructuredTypeField(null, StringType.NativeDefault, Druid.Parse("[V003]"), 1)
            );

            Assert.AreEqual("E1", e1.Name);
            Assert.AreEqual("E2", e2.Name);
            Assert.AreEqual("U1", e3.Name);
            Assert.AreEqual("X", this.manager.GetCaption(e1.GetField("[400E]").CaptionId).Name);
            Assert.AreEqual("Y", this.manager.GetCaption(e1.GetField("[400F]").CaptionId).Name);
            Assert.AreEqual("Z", this.manager.GetCaption(e2.GetField("[400G]").CaptionId).Name);
            Assert.AreEqual("V", this.manager.GetCaption(e3.GetField("[V002]").CaptionId).Name);
            Assert.AreEqual("W", this.manager.GetCaption(e3.GetField("[V003]").CaptionId).Name);
        }

        private StructuredType CreateInterface()
        {
            StructuredType i = new StructuredType(StructuredTypeClass.Interface, Druid.Empty);

            i.DefineCaption(this.manager.GetCaption(Druid.Parse("[400H]"))); //	from Test, Application layer

            Assert.AreEqual(ResourceModuleLayer.Application, i.Module.Layer);

            i.Fields.Add(
                new StructuredTypeField(null, StringType.NativeDefault, Druid.Parse("[400I]"), 0)
            );
            i.Fields.Add(
                new StructuredTypeField(null, StringType.NativeDefault, Druid.Parse("[400J]"), 1)
            );

            Assert.AreEqual("I", i.Name);
            Assert.AreEqual("If1", this.manager.GetCaption(i.GetField("[400I]").CaptionId).Name);
            Assert.AreEqual("If2", this.manager.GetCaption(i.GetField("[400J]").CaptionId).Name);

            return i;
        }

        private void HandleDataPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.buffer.Append("[");
            this.buffer.Append(e.PropertyName);
            this.buffer.Append(":");
            this.buffer.Append(e.OldValue);
            this.buffer.Append("->");
            this.buffer.Append(e.NewValue);
            this.buffer.Append("]");
        }

        private System.Text.StringBuilder buffer;
        private ResourceManager manager;

        private static void Fill(StructuredType record)
        {
            StructuredType subRec1 = new StructuredType();
            StructuredType subRec2 = new StructuredType();

            record.Fields.Add("Number1", new DecimalType());
            record.Fields.Add("Text1", new StringType());
            record.Fields.Add("Number2", new DecimalType(new DecimalRange(0.0M, 999.9M, 0.1M)));

            record.Fields.Add("Personne", subRec1);

            subRec1.Fields.Add("Nom", new StringType());
            subRec1.Fields.Add("Prénom", new StringType());
            subRec1.Fields.Add("Adresse", subRec2);

            subRec2.Fields.Add("NPA", new IntegerType(1, 999999));
            subRec2.Fields.Add("Ville", new StringType());
        }
    }
}
