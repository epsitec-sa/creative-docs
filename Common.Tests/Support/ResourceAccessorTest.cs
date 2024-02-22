using System.Collections.Generic;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Adorners;
using NUnit.Framework;

namespace Epsitec.Common.Tests.Support
{
    [TestFixture]
    public class ResourceAccessorTest
    {
        [SetUp]
        public void Initialize()
        {
            Epsitec.Common.Widgets.Widget.Initialize();

            this.manager = new ResourceManager(typeof(ResourceAccessorTest));
            this.manager.DefineDefaultModuleName("Test");

            Globals.Properties.SetProperty(
                Epsitec
                    .Common
                    .Support
                    .ResourceAccessors
                    .AbstractResourceAccessor
                    .DeveloperIdPropertyName,
                0
            );
            Epsitec.Common.Widgets.Window.RunningInAutomatedTestEnvironment = true;
        }

        [Test]
        public void CheckBasicTypes()
        {
            Assert.IsNotNull(Epsitec.Common.Types.DruidType.Default);

            StructuredType typeStruct = Epsitec.Common.Support.Res.Types.ResourceStructuredType;
            StructuredType typeField = Epsitec.Common.Support.Res.Types.Field;
            //-			Types.CollectionType typeFields = Epsitec.Common.Support.Res.Types.FieldCollection;

            EnumType typeFieldRelation = Epsitec.Common.Types.Res.Types.FieldRelation;
            EnumType typeFieldMembership = Epsitec.Common.Types.Res.Types.FieldMembership;

            Assert.AreEqual(
                typeField,
                typeStruct
                    .GetField(
                        Epsitec.Common.Support.Res.Fields.ResourceStructuredType.Fields.ToString()
                    )
                    .Type
            );
            Assert.AreEqual(
                FieldRelation.Collection,
                typeStruct
                    .GetField(
                        Epsitec.Common.Support.Res.Fields.ResourceStructuredType.Fields.ToString()
                    )
                    .Relation
            );
            //-			Assert.AreEqual (typeField, typeFields.ItemType);
            Assert.AreEqual(typeof(FieldRelation), typeFieldRelation.SystemType);
            Assert.AreEqual(typeof(FieldMembership), typeFieldMembership.SystemType);
            Assert.AreEqual(
                typeFieldRelation.SystemType,
                typeField
                    .GetField(Epsitec.Common.Support.Res.Fields.Field.Relation.ToString())
                    .Type.SystemType
            );
            Assert.AreEqual(
                typeFieldMembership.SystemType,
                typeField
                    .GetField(Epsitec.Common.Support.Res.Fields.Field.Membership.ToString())
                    .Type.SystemType
            );
        }

        [Test]
        public void CheckCaptionAccessor()
        {
            Epsitec.Common.Support.ResourceAccessors.CaptionResourceAccessor accessor =
                new Epsitec.Common.Support.ResourceAccessors.CaptionResourceAccessor();

            Assert.IsFalse(accessor.ContainsChanges);

            accessor.Load(this.manager);

            Assert.AreEqual(2, accessor.Collection.Count);

            Assert.AreEqual(Druid.Parse("[4002]"), accessor.Collection[Druid.Parse("[4002]")].Id);
            Assert.AreEqual("PatternAngle", accessor.Collection[Druid.Parse("[4002]")].Name);
            Assert.AreEqual(
                "Pattern angle expressed in degrees.",
                accessor
                    .Collection[Druid.Parse("[4002]")]
                    .GetCultureData("00")
                    .GetValue(Epsitec.Common.Support.Res.Fields.ResourceCaption.Description)
            );

            StructuredData data1 = accessor.Collection["PatternAngle"].GetCultureData("fr");
            StructuredData data2 = accessor.Collection["PatternAngle"].GetCultureData("fr");

            Assert.AreSame(data1, data2);
            Assert.AreEqual(
                "Angle de rotation de la trame, exprim꧃ 攀渀 搀攀最爀쌀©s.",
                data1.GetValue(Epsitec.Common.Support.Res.Fields.ResourceCaption.Description)
            );
            Assert.AreEqual(
                3,
                (
                    data1.GetValue(Epsitec.Common.Support.Res.Fields.ResourceCaption.Labels)
                    as IList<string>
                ).Count
            );
            Assert.AreEqual(
                "A",
                (
                    data1.GetValue(Epsitec.Common.Support.Res.Fields.ResourceCaption.Labels)
                    as IList<string>
                )[0]
            );
            Assert.AreEqual(
                "Angle",
                (
                    data1.GetValue(Epsitec.Common.Support.Res.Fields.ResourceCaption.Labels)
                    as IList<string>
                )[1]
            );
            Assert.AreEqual(
                "Angle de la trame",
                (
                    data1.GetValue(Epsitec.Common.Support.Res.Fields.ResourceCaption.Labels)
                    as IList<string>
                )[2]
            );
            Assert.IsTrue(
                data1.IsValueLocked(Epsitec.Common.Support.Res.Fields.ResourceCaption.Labels)
            );
            Assert.IsFalse(accessor.ContainsChanges);

            data1 = accessor.Collection["PatternAngle"].GetCultureData("de");
            data2 = accessor.Collection["PatternAngle"].GetCultureData("de");

            Assert.IsNotNull(data1);
            Assert.AreSame(data1, data2);
            Assert.AreEqual(
                0,
                (
                    (IList<string>)
                        data1.GetValue(Epsitec.Common.Support.Res.Fields.ResourceCaption.Labels)
                ).Count
            );
            Assert.AreEqual(
                UndefinedValue.Value,
                data1.GetValue(Epsitec.Common.Support.Res.Fields.ResourceCaption.Description)
            );
            Assert.IsFalse(accessor.ContainsChanges);

            data1 = accessor.Collection["PatternAngle"].GetCultureData("fr");
            data1.SetValue(
                Epsitec.Common.Support.Res.Fields.ResourceCaption.Description,
                "Angle de la hachure"
            );
            data2.SetValue(
                Epsitec.Common.Support.Res.Fields.ResourceCaption.Description,
                "Schraffurwinkel"
            );

            Assert.IsTrue(accessor.ContainsChanges);
            Assert.AreEqual(1, accessor.PersistChanges());
            Assert.IsFalse(accessor.ContainsChanges);

            Assert.AreEqual(
                "Angle de la hachure",
                this.manager.GetCaption(
                    Druid.Parse("[4002]"),
                    ResourceLevel.Merged,
                    Epsitec.Common.Support.Resources.FindCultureInfo("fr")
                ).Description
            );
            Assert.AreEqual(
                "Schraffurwinkel",
                this.manager.GetCaption(
                    Druid.Parse("[4002]"),
                    ResourceLevel.Merged,
                    Epsitec.Common.Support.Resources.FindCultureInfo("de")
                ).Description
            );

            IList<string> labels =
                data1.GetValue(Epsitec.Common.Support.Res.Fields.ResourceCaption.Labels)
                as IList<string>;

            labels.RemoveAt(2);

            Assert.IsTrue(accessor.ContainsChanges);
            Assert.AreEqual(1, accessor.PersistChanges());
            Assert.IsFalse(accessor.ContainsChanges);

            labels[0] = "A.";

            Assert.IsTrue(accessor.ContainsChanges);
            Assert.AreEqual(1, accessor.PersistChanges());
            Assert.IsFalse(accessor.ContainsChanges);

            CultureMap map = accessor.CreateItem();

            Assert.IsNotNull(map);
            Assert.AreEqual(Druid.Parse("[400C]"), map.Id);
            Assert.IsNull(accessor.Collection[map.Id]);

            accessor.Collection.Add(map);
            Assert.IsTrue(accessor.ContainsChanges);

            map.Name = "NewItem";
            map.GetCultureData("00")
                .SetValue(
                    Epsitec.Common.Support.Res.Fields.ResourceCaption.Description,
                    "New value"
                );
            map.GetCultureData("fr")
                .SetValue(
                    Epsitec.Common.Support.Res.Fields.ResourceCaption.Description,
                    "Nouvelle valeur"
                );

            Assert.AreEqual(1, accessor.PersistChanges());
            Assert.IsFalse(accessor.ContainsChanges);

            Assert.AreEqual(
                "New value",
                this.manager.GetCaption(Druid.Parse("[400C]"), ResourceLevel.Default).Description
            );
            Assert.AreEqual(
                "Nouvelle valeur",
                this.manager.GetCaption(
                    Druid.Parse("[400C]"),
                    ResourceLevel.Merged,
                    Epsitec.Common.Support.Resources.FindCultureInfo("fr")
                ).Description
            );
            Assert.AreEqual(
                "NewItem",
                this.manager.GetCaption(Druid.Parse("[400C]"), ResourceLevel.Default).Name
            );
            Assert.AreEqual(
                "NewItem",
                this.manager.GetCaption(
                    Druid.Parse("[400C]"),
                    ResourceLevel.Merged,
                    Epsitec.Common.Support.Resources.FindCultureInfo("fr")
                ).Name
            );
            Assert.AreEqual(
                "Cap.NewItem",
                this.manager.GetBundle(
                    Epsitec.Common.Support.Resources.CaptionsBundleName,
                    ResourceLevel.Default
                )[Druid.Parse("[400C]")].Name
            );
            Assert.IsTrue(
                string.IsNullOrEmpty(
                    this.manager.GetBundle(
                        Epsitec.Common.Support.Resources.CaptionsBundleName,
                        ResourceLevel.Localized,
                        Epsitec.Common.Support.Resources.FindCultureInfo("fr")
                    )[Druid.Parse("[400C]")].Name
                )
            );

            map.GetCultureData("fr")
                .SetValue(
                    Epsitec.Common.Support.Res.Fields.ResourceCaption.Description,
                    UndefinedValue.Value
                );

            Assert.AreEqual(1, accessor.PersistChanges());
            Assert.IsFalse(accessor.ContainsChanges);

            Assert.AreEqual(
                "New value",
                this.manager.GetCaption(Druid.Parse("[400C]"), ResourceLevel.Default).Description
            );
            Assert.AreEqual(
                "New value",
                this.manager.GetCaption(
                    Druid.Parse("[400C]"),
                    ResourceLevel.Merged,
                    Epsitec.Common.Support.Resources.FindCultureInfo("fr")
                ).Description
            );

            accessor.Collection.Remove(map);
            Assert.IsTrue(accessor.ContainsChanges);
            Assert.AreEqual(1, accessor.PersistChanges());
            Assert.IsFalse(accessor.ContainsChanges);

            this.manager.ClearCaptionCache(
                Druid.Parse("[400C]"),
                ResourceLevel.All,
                Epsitec.Common.Support.Resources.FindCultureInfo("fr")
            );

            Assert.IsNull(this.manager.GetCaption(Druid.Parse("[400C]"), ResourceLevel.Default));
            Assert.IsNull(
                this.manager.GetCaption(
                    Druid.Parse("[400C]"),
                    ResourceLevel.Merged,
                    Epsitec.Common.Support.Resources.FindCultureInfo("fr")
                )
            );
        }

        [Test]
        public void CheckCaptionAccessorRevert()
        {
            Epsitec.Common.Support.ResourceAccessors.CaptionResourceAccessor accessor =
                new Epsitec.Common.Support.ResourceAccessors.CaptionResourceAccessor();

            accessor.Load(this.manager);

            Assert.AreEqual(2, accessor.Collection.Count);

            Assert.AreEqual(Druid.Parse("[4002]"), accessor.Collection[Druid.Parse("[4002]")].Id);
            Assert.AreEqual("PatternAngle", accessor.Collection[Druid.Parse("[4002]")].Name);
            Assert.AreEqual(
                "Pattern angle expressed in degrees.",
                accessor
                    .Collection[Druid.Parse("[4002]")]
                    .GetCultureData("00")
                    .GetValue(Epsitec.Common.Support.Res.Fields.ResourceCaption.Description)
            );

            StructuredData data1;
            StructuredData data2;

            data1 = accessor.Collection["PatternAngle"].GetCultureData("fr");
            data2 = accessor.Collection["PatternAngle"].GetCultureData("de");

            data1.SetValue(
                Epsitec.Common.Support.Res.Fields.ResourceCaption.Description,
                "Angle de la hachure"
            );
            data2.SetValue(
                Epsitec.Common.Support.Res.Fields.ResourceCaption.Description,
                "Schraffurwinkel"
            );

            Assert.IsTrue(accessor.ContainsChanges);
            Assert.AreEqual(1, accessor.RevertChanges());
            Assert.IsFalse(accessor.ContainsChanges);

            Assert.AreEqual(
                "Angle de rotation de la trame, exprim꧃ 攀渀 搀攀最爀쌀©s.",
                this.manager.GetCaption(
                    Druid.Parse("[4002]"),
                    ResourceLevel.Merged,
                    Epsitec.Common.Support.Resources.FindCultureInfo("fr")
                ).Description
            );
            Assert.AreEqual(
                "Pattern angle expressed in degrees.",
                this.manager.GetCaption(
                    Druid.Parse("[4002]"),
                    ResourceLevel.Merged,
                    Epsitec.Common.Support.Resources.FindCultureInfo("de")
                ).Description
            );

            IList<string> labels;

            labels =
                data1.GetValue(Epsitec.Common.Support.Res.Fields.ResourceCaption.Labels)
                as IList<string>;
            labels.RemoveAt(2);

            Assert.IsTrue(accessor.ContainsChanges);
            Assert.AreEqual(1, accessor.RevertChanges());
            Assert.IsFalse(accessor.ContainsChanges);
            data1 = accessor.Collection["PatternAngle"].GetCultureData("fr");
            labels =
                data1.GetValue(Epsitec.Common.Support.Res.Fields.ResourceCaption.Labels)
                as IList<string>;
            Assert.AreEqual(
                3,
                (
                    data1.GetValue(Epsitec.Common.Support.Res.Fields.ResourceCaption.Labels)
                    as IList<string>
                ).Count
            );

            labels[0] = "A.";

            Assert.IsTrue(accessor.ContainsChanges);
            Assert.AreEqual(1, accessor.RevertChanges());
            Assert.IsFalse(accessor.ContainsChanges);
            data1 = accessor.Collection["PatternAngle"].GetCultureData("fr");
            labels =
                data1.GetValue(Epsitec.Common.Support.Res.Fields.ResourceCaption.Labels)
                as IList<string>;
            Assert.AreEqual(
                "A",
                (
                    data1.GetValue(Epsitec.Common.Support.Res.Fields.ResourceCaption.Labels)
                    as IList<string>
                )[0]
            );

            CultureMap map = accessor.CreateItem();

            Assert.IsNotNull(map);
            Assert.AreEqual(Druid.Parse("[400C]"), map.Id);
            Assert.IsNull(accessor.Collection[map.Id]);

            accessor.Collection.Add(map);
            Assert.IsTrue(accessor.ContainsChanges);

            map.Name = "NewItem";
            map.GetCultureData("00")
                .SetValue(
                    Epsitec.Common.Support.Res.Fields.ResourceCaption.Description,
                    "New value"
                );
            map.GetCultureData("fr")
                .SetValue(
                    Epsitec.Common.Support.Res.Fields.ResourceCaption.Description,
                    "Nouvelle valeur"
                );

            Assert.AreEqual(1, accessor.RevertChanges());
            Assert.IsFalse(accessor.ContainsChanges);
            Assert.IsNull(accessor.Collection[map.Id]);
        }

        [Test]
        public void CheckCommandAccessor()
        {
            Epsitec.Common.Support.ResourceAccessors.CommandResourceAccessor accessor =
                new Epsitec.Common.Support.ResourceAccessors.CommandResourceAccessor();

            Assert.IsFalse(accessor.ContainsChanges);

            accessor.Load(this.manager);

            Assert.AreEqual(1, accessor.Collection.Count);
        }

        [Test]
        public void CheckStructuredTypeAccessor()
        {
            Epsitec.Common.Support.ResourceAccessors.StructuredTypeResourceAccessor accessor =
                new Epsitec.Common.Support.ResourceAccessors.StructuredTypeResourceAccessor();

            Assert.IsFalse(accessor.ContainsChanges);

            accessor.Load(Epsitec.Common.Support.Res.Manager);

            Assert.AreEqual(22, accessor.Collection.Count);

            CultureMap map = accessor.Collection[
                Epsitec.Common.Support.Res.Types.ResourceStructuredType.CaptionId
            ];

            Assert.AreEqual("ResourceStructuredType", map.Name);
            Assert.AreEqual(
                "Typ.StructuredType.ResourceStructuredType",
                Epsitec
                    .Common.Support.Res.Manager.GetBundle(
                        Epsitec.Common.Support.Resources.CaptionsBundleName,
                        ResourceLevel.Default
                    )[Epsitec.Common.Support.Res.Types.ResourceStructuredType.CaptionId]
                    .Name
            );
            Assert.AreEqual(
                "Fld.ResourceStructuredType.Fields",
                Epsitec
                    .Common.Support.Res.Manager.GetBundle(
                        Epsitec.Common.Support.Resources.CaptionsBundleName,
                        ResourceLevel.Default
                    )[Epsitec.Common.Support.Res.Fields.ResourceStructuredType.Fields]
                    .Name
            );

            StructuredData data = map.GetCultureData(
                Epsitec.Common.Support.Resources.DefaultTwoLetterISOLanguageName
            );
            StructuredTypeClass typeClass = (StructuredTypeClass)
                data.GetValue(Epsitec.Common.Support.Res.Fields.ResourceStructuredType.Class);
            Druid baseTypeId = (Druid)
                data.GetValue(Epsitec.Common.Support.Res.Fields.ResourceStructuredType.BaseType);
            IList<StructuredData> fields =
                data.GetValue(Epsitec.Common.Support.Res.Fields.ResourceStructuredType.Fields)
                as IList<StructuredData>;

            Assert.AreEqual(15, fields.Count);

            Assert.AreEqual(
                Epsitec.Common.Support.Res.Fields.ResourceBase.ModificationId,
                fields[0].GetValue(Epsitec.Common.Support.Res.Fields.Field.CaptionId)
            );
            Assert.AreEqual(
                Epsitec.Common.Support.Res.Fields.ResourceBase.Comment,
                fields[1].GetValue(Epsitec.Common.Support.Res.Fields.Field.CaptionId)
            );

            Assert.AreEqual(
                FieldSource.Value,
                fields[0].GetValue(Epsitec.Common.Support.Res.Fields.Field.Source)
            );
            Assert.AreEqual(
                "",
                fields[0].GetValue(Epsitec.Common.Support.Res.Fields.Field.Expression)
            );

            //	Check that defining type id properly defined (!) based on where the
            //	field originates :

            Assert.AreEqual(
                Druid.Parse("[7005]"),
                fields[0].GetValue(Epsitec.Common.Support.Res.Fields.Field.DefiningTypeId)
            );
            Assert.AreEqual(
                Druid.Parse("[7005]"),
                fields[2].GetValue(Epsitec.Common.Support.Res.Fields.Field.DefiningTypeId)
            );
            Assert.AreEqual(
                Druid.Parse("[7005]"),
                fields[7].GetValue(Epsitec.Common.Support.Res.Fields.Field.DefiningTypeId)
            );
            Assert.AreEqual(
                Druid.Empty,
                fields[13].GetValue(Epsitec.Common.Support.Res.Fields.Field.DefiningTypeId)
            );

            Assert.AreEqual(
                Druid.Parse("[700B1]"),
                fields[0].GetValue(Epsitec.Common.Support.Res.Fields.Field.DeepDefiningTypeId)
            );
            Assert.AreEqual(
                Druid.Parse("[7006]"),
                fields[2].GetValue(Epsitec.Common.Support.Res.Fields.Field.DeepDefiningTypeId)
            );
            Assert.AreEqual(
                Druid.Parse("[7005]"),
                fields[7].GetValue(Epsitec.Common.Support.Res.Fields.Field.DeepDefiningTypeId)
            );
            Assert.AreEqual(
                Druid.Empty,
                fields[13].GetValue(Epsitec.Common.Support.Res.Fields.Field.DeepDefiningTypeId)
            );

            map.Name = "ResourceEntityType";
            fields[11]
                .SetValue(Epsitec.Common.Support.Res.Fields.Field.Source, FieldSource.Expression);
            fields[11].SetValue(Epsitec.Common.Support.Res.Fields.Field.Expression, "foo");

            accessor.PersistChanges();

            Assert.AreEqual("ResourceEntityType", map.Name);
            Assert.AreEqual(
                "Typ.StructuredType.ResourceEntityType",
                Epsitec
                    .Common.Support.Res.Manager.GetBundle(
                        Epsitec.Common.Support.Resources.CaptionsBundleName,
                        ResourceLevel.Default
                    )[Epsitec.Common.Support.Res.Types.ResourceStructuredType.CaptionId]
                    .Name
            );
            Assert.AreEqual(
                "Fld.ResourceEntityType.Fields",
                Epsitec
                    .Common.Support.Res.Manager.GetBundle(
                        Epsitec.Common.Support.Resources.CaptionsBundleName,
                        ResourceLevel.Default
                    )[Epsitec.Common.Support.Res.Fields.ResourceStructuredType.Fields]
                    .Name
            );
            Assert.AreEqual(
                "ResourceEntityType.Fields",
                accessor
                    .FieldAccessor.Collection[
                        Epsitec.Common.Support.Res.Fields.ResourceStructuredType.Fields
                    ]
                    .ToString()
            );

            Caption caption = Epsitec.Common.Support.Res.Manager.GetCaption(
                Epsitec.Common.Support.Res.Types.ResourceStructuredType.CaptionId,
                ResourceLevel.Default
            );
            StructuredType type = TypeRosetta.CreateTypeObject(caption, false) as StructuredType;

            Assert.AreEqual("ResourceEntityType", caption.Name);
            Assert.AreEqual(
                FieldSource.Expression,
                type.Fields[
                    Epsitec.Common.Support.Res.Fields.ResourceStructuredType.Class.ToString()
                ].Source
            );
            Assert.AreEqual(
                "foo",
                type.Fields[
                    Epsitec.Common.Support.Res.Fields.ResourceStructuredType.Class.ToString()
                ].Expression
            );

            map.Name = "ResourceStructuredType";
            accessor.PersistChanges();

            Assert.AreEqual("ResourceStructuredType", map.Name);
            Assert.AreEqual(
                "Typ.StructuredType.ResourceStructuredType",
                Epsitec
                    .Common.Support.Res.Manager.GetBundle(
                        Epsitec.Common.Support.Resources.CaptionsBundleName,
                        ResourceLevel.Default
                    )[Epsitec.Common.Support.Res.Types.ResourceStructuredType.CaptionId]
                    .Name
            );
            Assert.AreEqual(
                "Fld.ResourceStructuredType.Fields",
                Epsitec
                    .Common.Support.Res.Manager.GetBundle(
                        Epsitec.Common.Support.Resources.CaptionsBundleName,
                        ResourceLevel.Default
                    )[Epsitec.Common.Support.Res.Fields.ResourceStructuredType.Fields]
                    .Name
            );
            Assert.AreEqual(
                "ResourceStructuredType.Fields",
                accessor
                    .FieldAccessor.Collection[
                        Epsitec.Common.Support.Res.Fields.ResourceStructuredType.Fields
                    ]
                    .ToString()
            );

            CultureMap fieldsItem;

            fieldsItem = accessor.FieldAccessor.Collection[
                Epsitec.Common.Support.Res.Fields.ResourceStructuredType.Fields
            ];

            Assert.AreEqual("Fields", fieldsItem.Name);
            Assert.AreEqual("ResourceStructuredType.Fields", fieldsItem.ToString());

            fieldsItem = accessor.CreateFieldItem(map);

            fieldsItem.Name = "X";

            accessor.FieldAccessor.Collection.Add(fieldsItem);
            accessor.FieldAccessor.PersistChanges();

            Assert.AreEqual(
                "Fld.ResourceStructuredType.X",
                Epsitec
                    .Common.Support.Res.Manager.GetBundle(
                        Epsitec.Common.Support.Resources.CaptionsBundleName,
                        ResourceLevel.Default
                    )[fieldsItem.Id]
                    .Name
            );
            Assert.AreEqual(
                "ResourceStructuredType.X",
                accessor.FieldAccessor.Collection[fieldsItem.Id].ToString()
            );

            accessor.FieldAccessor.Collection.Remove(fieldsItem);
            accessor.FieldAccessor.PersistChanges();

            Assert.IsTrue(
                Epsitec
                    .Common.Support.Res.Manager.GetBundle(
                        Epsitec.Common.Support.Resources.CaptionsBundleName,
                        ResourceLevel.Default
                    )[fieldsItem.Id]
                    .IsEmpty
            );

            IList<StructuredData> interfaceIds =
                data.GetValue(Epsitec.Common.Support.Res.Fields.ResourceStructuredType.InterfaceIds)
                as IList<StructuredData>;

            Assert.IsNotNull(interfaceIds);

            IDataBroker broker = accessor.GetDataBroker(
                data,
                Epsitec.Common.Support.Res.Fields.ResourceStructuredType.InterfaceIds.ToString()
            );
            StructuredData interfaceIdData = broker.CreateData(map);

            interfaceIdData.SetValue(
                Epsitec.Common.Support.Res.Fields.InterfaceId.CaptionId,
                Druid.Parse("[700I2]")
            );

            interfaceIds.Add(interfaceIdData);

            Assert.AreEqual(17, fields.Count);
        }

        [Test]
        public void CheckStructuredTypeAccessor2()
        {
            Epsitec.Common.Support.ResourceAccessors.StructuredTypeResourceAccessor accessor =
                new Epsitec.Common.Support.ResourceAccessors.StructuredTypeResourceAccessor();
            accessor.Load(Epsitec.Common.Support.Res.Manager);

            CultureMap map = accessor.Collection[
                null /*Epsitec.Common.Support.Res.Types.TestInterfaceUser.CaptionId*/
            ];

            Assert.AreEqual("TestInterfaceUser", map.Name);

            StructuredData data = map.GetCultureData(
                Epsitec.Common.Support.Resources.DefaultTwoLetterISOLanguageName
            );
            StructuredTypeClass typeClass = (StructuredTypeClass)
                data.GetValue(Epsitec.Common.Support.Res.Fields.ResourceStructuredType.Class);
            Druid baseTypeId = (Druid)
                data.GetValue(Epsitec.Common.Support.Res.Fields.ResourceStructuredType.BaseType);
            IList<StructuredData> fields =
                data.GetValue(Epsitec.Common.Support.Res.Fields.ResourceStructuredType.Fields)
                as IList<StructuredData>;
            IList<StructuredData> interfaceIds =
                data.GetValue(Epsitec.Common.Support.Res.Fields.ResourceStructuredType.InterfaceIds)
                as IList<StructuredData>;

            Assert.AreEqual(3, fields.Count);
            Assert.AreEqual(1, interfaceIds.Count);

            Assert.AreEqual(Druid.Empty, baseTypeId);
            //Assert.AreEqual (Epsitec.Common.Support.Res.Types.TestInterface.CaptionId, (Druid) interfaceIds[0].GetValue (Epsitec.Common.Support.Res.Fields.InterfaceId.CaptionId));

            Assert.AreEqual(
                "[700J2]",
                fields[0].GetValue(Epsitec.Common.Support.Res.Fields.Field.CaptionId).ToString()
            ); //	from interface, "Name" -- field redefined by TestInterfaceUser
            Assert.AreEqual(
                "[7012]",
                fields[1].GetValue(Epsitec.Common.Support.Res.Fields.Field.CaptionId).ToString()
            ); //	from interface, "Resource"
            Assert.AreEqual(
                "[7014]",
                fields[2].GetValue(Epsitec.Common.Support.Res.Fields.Field.CaptionId).ToString()
            ); //	locally defined, "Extension1"

            Assert.AreEqual(
                FieldSource.Expression,
                (FieldSource)fields[0].GetValue(Epsitec.Common.Support.Res.Fields.Field.Source)
            );
            Assert.AreEqual(
                FieldSource.Expression,
                (FieldSource)fields[1].GetValue(Epsitec.Common.Support.Res.Fields.Field.Source)
            );
            Assert.AreEqual(
                FieldSource.Expression,
                (FieldSource)fields[2].GetValue(Epsitec.Common.Support.Res.Fields.Field.Source)
            );

            //Assert.AreEqual (Epsitec.Common.Support.Res.Types.TestInterface.CaptionId, (Druid) fields[0].GetValue (Epsitec.Common.Support.Res.Fields.Field.DeepDefiningTypeId));
            //Assert.AreEqual (Epsitec.Common.Support.Res.Types.TestInterface.CaptionId, (Druid) fields[1].GetValue (Epsitec.Common.Support.Res.Fields.Field.DeepDefiningTypeId));
            Assert.AreEqual(
                Druid.Empty,
                (Druid)
                    fields[2].GetValue(Epsitec.Common.Support.Res.Fields.Field.DeepDefiningTypeId)
            );

            Assert.AreEqual(
                FieldMembership.Local,
                (FieldMembership)
                    fields[0].GetValue(Epsitec.Common.Support.Res.Fields.Field.Membership)
            );
            Assert.AreEqual(
                FieldMembership.Local,
                (FieldMembership)
                    fields[1].GetValue(Epsitec.Common.Support.Res.Fields.Field.Membership)
            );
            Assert.AreEqual(
                FieldMembership.Local,
                (FieldMembership)
                    fields[2].GetValue(Epsitec.Common.Support.Res.Fields.Field.Membership)
            );

            //Assert.AreEqual (FieldMembership.LocalOverride, Epsitec.Common.Support.Res.Types.TestInterfaceUser.Fields["[700J2]"].Membership);
            //Assert.AreEqual (FieldMembership.Local,         Epsitec.Common.Support.Res.Types.TestInterfaceUser.Fields["[7012]"].Membership);
            //Assert.AreEqual (FieldMembership.Local,         Epsitec.Common.Support.Res.Types.TestInterfaceUser.Fields["[7014]"].Membership);

            Assert.AreEqual(
                "믂⼃挀⌀尀爀尀渀砀 㴀㸀 猀琀爀椀渀最⸀䔀洀瀀琀礀∀Ⰰ      ⠀猀琀爀椀渀最⤀ 昀椀攀氀搀猀嬀　崀⸀䜀攀琀嘀愀氀甀攀 ⠀䔀瀀猀椀琀攀挀⸀䌀漀洀洀漀渀⸀匀甀瀀瀀漀爀琀⸀刀攀猀⸀䘀椀攀氀搀猀⸀䘀椀攀氀搀⸀䔀砀瀀爀攀猀猀椀漀渀⤀⤀㬀ഀ਀ऀऀऀ䄀猀猀攀爀琀⸀䄀爀攀䔀焀甀愀氀 ⠀∀숀λ/c#\r\nx => x.Name.ToUpper ()",
                (string)fields[2].GetValue(Epsitec.Common.Support.Res.Fields.Field.Expression)
            );

            Assert.AreEqual(
                false,
                fields[0].GetValue(Epsitec.Common.Support.Res.Fields.Field.IsInterfaceDefinition)
            );
            Assert.AreEqual(
                true,
                fields[1].GetValue(Epsitec.Common.Support.Res.Fields.Field.IsInterfaceDefinition)
            );
            Assert.AreEqual(
                UndefinedValue.Value,
                fields[2].GetValue(Epsitec.Common.Support.Res.Fields.Field.IsInterfaceDefinition)
            );
        }

        [Test]
        public void CheckIntegerTypeAccessor()
        {
            Epsitec.Common.Support.ResourceAccessors.AnyTypeResourceAccessor accessor =
                new Epsitec.Common.Support.ResourceAccessors.AnyTypeResourceAccessor();

            Assert.IsFalse(accessor.ContainsChanges);

            accessor.Load(Epsitec.Common.Support.Res.Manager);

            CultureMap newItem = accessor.CreateItem();
            StructuredData newData = newItem.GetCultureData(
                Epsitec.Common.Support.Resources.DefaultTwoLetterISOLanguageName
            );
            newItem.Name = "AnyTypeAccessorInteger1";

            newData.SetValue(
                Epsitec.Common.Support.Res.Fields.ResourceBaseType.TypeCode,
                TypeCode.Integer
            );
            newData.SetValue(Epsitec.Common.Support.Res.Fields.ResourceNumericType.SmallStep, 1M);
            newData.SetValue(Epsitec.Common.Support.Res.Fields.ResourceNumericType.LargeStep, 10M);
            newData.SetValue(
                Epsitec.Common.Support.Res.Fields.ResourceNumericType.Range,
                new DecimalRange(0, 999, 1)
            );

            accessor.Collection.Add(newItem);
            accessor.PersistChanges();

            Caption caption = accessor.ResourceManager.GetCaption(
                newItem.Id,
                ResourceLevel.Default
            );
            IntegerType intType = TypeRosetta.CreateTypeObject(caption, false) as IntegerType;

            Assert.IsNotNull(intType);
            Assert.AreEqual(1M, intType.SmallStep);
            Assert.AreEqual(10M, intType.LargeStep);
            Assert.AreEqual(999M, intType.Range.Maximum);
        }

        [Test]
        public void CheckDateTimeTypeAccessor()
        {
            Epsitec.Common.Support.ResourceAccessors.AnyTypeResourceAccessor accessor =
                new Epsitec.Common.Support.ResourceAccessors.AnyTypeResourceAccessor();

            Assert.IsFalse(accessor.ContainsChanges);

            accessor.Load(Epsitec.Common.Support.Res.Manager);

            CultureMap newItem = accessor.CreateItem();
            StructuredData newData = newItem.GetCultureData(
                Epsitec.Common.Support.Resources.DefaultTwoLetterISOLanguageName
            );
            newItem.Name = "AnyTypeAccessorDateTime1";

            newData.SetValue(
                Epsitec.Common.Support.Res.Fields.ResourceBaseType.TypeCode,
                TypeCode.DateTime
            );
            newData.SetValue(
                Epsitec.Common.Support.Res.Fields.ResourceDateTimeType.Resolution,
                TimeResolution.Minutes
            );
            newData.SetValue(
                Epsitec.Common.Support.Res.Fields.ResourceDateTimeType.MinimumDate,
                new Date(2000, 06, 10)
            );
            newData.SetValue(
                Epsitec.Common.Support.Res.Fields.ResourceDateTimeType.TimeStep,
                new System.TimeSpan(0, 15, 0)
            );

            accessor.Collection.Add(newItem);
            accessor.PersistChanges();

            Caption caption = accessor.ResourceManager.GetCaption(newItem.Id, ResourceLevel.Default);
            DateTimeType dtType = TypeRosetta.CreateTypeObject(caption, false) as DateTimeType;

            Assert.IsNotNull(dtType);
            Assert.AreEqual(TimeResolution.Minutes, dtType.Resolution);
            Assert.AreEqual(2000, dtType.MinimumDate.Year);
            Assert.AreEqual(6, dtType.MinimumDate.Month);
            Assert.AreEqual(10, dtType.MinimumDate.Day);
            Assert.IsTrue(dtType.MaximumDate.IsNull);
            Assert.IsTrue(dtType.MinimumTime.IsNull);
            Assert.IsTrue(dtType.MaximumTime.IsNull);
            Assert.AreEqual(15, dtType.TimeStep.TotalMinutes);
        }

        [Test]
        public void CheckOtherTypeAccessor()
        {
            Epsitec.Common.Support.ResourceAccessors.AnyTypeResourceAccessor accessor =
                new Epsitec.Common.Support.ResourceAccessors.AnyTypeResourceAccessor();

            Assert.IsFalse(accessor.ContainsChanges);

            accessor.Load(Epsitec.Common.Support.Res.Manager);

            CultureMap newItem = accessor.CreateItem();
            StructuredData newData = newItem.GetCultureData(
                Epsitec.Common.Support.Resources.DefaultTwoLetterISOLanguageName
            );
            newItem.Name = "AnyTypeAccessorOther1";

            newData.SetValue(
                Epsitec.Common.Support.Res.Fields.ResourceBaseType.TypeCode,
                TypeCode.Other
            );
            newData.SetValue(
                Epsitec.Common.Support.Res.Fields.ResourceOtherType.SystemType,
                typeof(char)
            );

            accessor.Collection.Add(newItem);
            accessor.PersistChanges();

            Caption caption = accessor.ResourceManager.GetCaption(newItem.Id, ResourceLevel.Default);
            OtherType otherType = TypeRosetta.CreateTypeObject(caption, false) as OtherType;

            Assert.IsNotNull(otherType);
            Assert.AreEqual(typeof(char).Name, otherType.SystemType.Name);
        }

        [Test]
        public void CheckStringTypeAccessor()
        {
            Epsitec.Common.Support.ResourceAccessors.AnyTypeResourceAccessor accessor =
                new Epsitec.Common.Support.ResourceAccessors.AnyTypeResourceAccessor();

            Assert.IsFalse(accessor.ContainsChanges);

            accessor.Load(Epsitec.Common.Support.Res.Manager);

            CultureMap newItem = accessor.CreateItem();
            StructuredData newData = newItem.GetCultureData(
                Epsitec.Common.Support.Resources.DefaultTwoLetterISOLanguageName
            );
            newItem.Name = "AnyTypeAccessorString1";

            newData.SetValue(
                Epsitec.Common.Support.Res.Fields.ResourceBaseType.TypeCode,
                TypeCode.String
            );
            newData.SetValue(
                Epsitec.Common.Support.Res.Fields.ResourceStringType.UseMultilingualStorage,
                true
            );
            newData.SetValue(Epsitec.Common.Support.Res.Fields.ResourceStringType.MinimumLength, 1);

            accessor.Collection.Add(newItem);
            accessor.PersistChanges();

            Caption caption = accessor.ResourceManager.GetCaption(newItem.Id, ResourceLevel.Default);
            StringType stringType = TypeRosetta.CreateTypeObject(caption, false) as StringType;

            Assert.IsNotNull(stringType);
            Assert.AreEqual(true, stringType.UseMultilingualStorage);
            Assert.AreEqual(1, stringType.MinimumLength);
            Assert.AreEqual(1000000, stringType.MaximumLength);
        }

        [Test]
        public void CheckCollectionTypeAccessor()
        {
            Epsitec.Common.Support.ResourceAccessors.AnyTypeResourceAccessor accessor =
                new Epsitec.Common.Support.ResourceAccessors.AnyTypeResourceAccessor();

            Assert.IsFalse(accessor.ContainsChanges);

            accessor.Load(Epsitec.Common.Support.Res.Manager);

            CultureMap newItem = accessor.CreateItem();
            StructuredData newData = newItem.GetCultureData(
                Epsitec.Common.Support.Resources.DefaultTwoLetterISOLanguageName
            );
            newItem.Name = "AnyTypeAccessorCollection1";

            newData.SetValue(
                Epsitec.Common.Support.Res.Fields.ResourceBaseType.TypeCode,
                TypeCode.Collection
            );
            newData.SetValue(
                Epsitec.Common.Support.Res.Fields.ResourceCollectionType.ItemType,
                IntegerType.Default.CaptionId
            );

            accessor.Collection.Add(newItem);
            accessor.PersistChanges();

            Caption caption = accessor.ResourceManager.GetCaption(newItem.Id, ResourceLevel.Default);
            CollectionType colType = TypeRosetta.CreateTypeObject(caption, false) as CollectionType;

            Assert.IsNotNull(colType);
            Assert.AreEqual(IntegerType.Default.CaptionId, colType.ItemType.CaptionId);
        }

        [Test]
        public void CheckEnumerationTypeAccessor()
        {
            Epsitec.Common.Support.ResourceAccessors.AnyTypeResourceAccessor accessor =
                new Epsitec.Common.Support.ResourceAccessors.AnyTypeResourceAccessor();

            Assert.IsFalse(accessor.ContainsChanges);

            accessor.Load(Epsitec.Common.Support.Res.Manager);

            CultureMap map = accessor.Collection[
                Epsitec.Common.Types.Res.Types.BindingMode.CaptionId
            ];

            Assert.AreEqual("BindingMode", map.Name);
            Assert.AreEqual(
                "Typ.BindingMode",
                accessor
                    .ResourceManager.GetBundle(
                        Epsitec.Common.Support.Resources.CaptionsBundleName,
                        ResourceLevel.Default
                    )[Epsitec.Common.Types.Res.Types.BindingMode.CaptionId]
                    .Name
            );
            Assert.AreEqual(
                "Val.BindingMode.None",
                accessor
                    .ResourceManager.GetBundle(
                        Epsitec.Common.Support.Resources.CaptionsBundleName,
                        ResourceLevel.Default
                    )[Epsitec.Common.Types.Res.Values.BindingMode.None.Id]
                    .Name
            );

            Assert.IsFalse(accessor.CreateMissingValueItems(map));

            StructuredData enumData = map.GetCultureData(
                Epsitec.Common.Support.Resources.DefaultTwoLetterISOLanguageName
            );
            IList<StructuredData> enumValues =
                enumData.GetValue(Epsitec.Common.Support.Res.Fields.ResourceEnumType.Values)
                as IList<StructuredData>;

            Assert.AreEqual(5, enumValues.Count);
            Assert.AreEqual(
                typeof(BindingMode),
                enumData.GetValue(Epsitec.Common.Support.Res.Fields.ResourceEnumType.SystemType)
                    as System.Type
            );

            Assert.AreEqual(
                Epsitec.Common.Types.Res.Values.BindingMode.None.Id,
                enumValues[0].GetValue(Epsitec.Common.Support.Res.Fields.EnumValue.CaptionId)
            );
            Assert.AreEqual(
                Epsitec.Common.Types.Res.Values.BindingMode.TwoWay.Id,
                enumValues[4].GetValue(Epsitec.Common.Support.Res.Fields.EnumValue.CaptionId)
            );

            map.Name = "Foo";
            accessor.PersistChanges();

            Assert.AreEqual("Foo", map.Name);
            Assert.AreEqual(
                "Typ.Foo",
                accessor
                    .ResourceManager.GetBundle(
                        Epsitec.Common.Support.Resources.CaptionsBundleName,
                        ResourceLevel.Default
                    )[Epsitec.Common.Types.Res.Types.BindingMode.CaptionId]
                    .Name
            );
            Assert.AreEqual(
                "Val.Foo.None",
                accessor
                    .ResourceManager.GetBundle(
                        Epsitec.Common.Support.Resources.CaptionsBundleName,
                        ResourceLevel.Default
                    )[Epsitec.Common.Types.Res.Values.BindingMode.None.Id]
                    .Name
            );
            Assert.AreEqual(
                "Foo.None",
                accessor
                    .ValueAccessor.Collection[Epsitec.Common.Types.Res.Values.BindingMode.None.Id]
                    .ToString()
            );

            map.Name = "BindingMode";
            accessor.PersistChanges();

            Assert.AreEqual("BindingMode", map.Name);
            Assert.AreEqual(
                "Typ.BindingMode",
                accessor
                    .ResourceManager.GetBundle(
                        Epsitec.Common.Support.Resources.CaptionsBundleName,
                        ResourceLevel.Default
                    )[Epsitec.Common.Types.Res.Types.BindingMode.CaptionId]
                    .Name
            );
            Assert.AreEqual(
                "Val.BindingMode.None",
                accessor
                    .ResourceManager.GetBundle(
                        Epsitec.Common.Support.Resources.CaptionsBundleName,
                        ResourceLevel.Default
                    )[Epsitec.Common.Types.Res.Values.BindingMode.None.Id]
                    .Name
            );
            Assert.AreEqual(
                "BindingMode.None",
                accessor
                    .ValueAccessor.Collection[Epsitec.Common.Types.Res.Values.BindingMode.None.Id]
                    .ToString()
            );

            map = accessor.CreateItem();
            map.Name = "Test.MyTestEnum";
            enumData = map.GetCultureData(
                Epsitec.Common.Support.Resources.DefaultTwoLetterISOLanguageName
            );
            enumData.SetValue(
                Epsitec.Common.Support.Res.Fields.ResourceBaseType.TypeCode,
                TypeCode.Enum
            );
            enumData.SetValue(
                Epsitec.Common.Support.Res.Fields.ResourceEnumType.SystemType,
                typeof(MyTestEnum)
            );

            accessor.Collection.Add(map);

            int count = accessor.ValueAccessor.Collection.Count;

            Assert.IsTrue(accessor.CreateMissingValueItems(map));
            Assert.AreEqual(count + 3, accessor.ValueAccessor.Collection.Count);

            accessor.PersistChanges();
        }

        [Test]
        public void CheckPanelAccessor()
        {
            Epsitec.Common.Support.ResourceAccessors.PanelResourceAccessor accessor =
                new Epsitec.Common.Support.ResourceAccessors.PanelResourceAccessor();
            ResourceModuleId module = new ResourceModuleId(
                "Cresus.Tests",
                @"S:\Epsitec.Cresus\App.CresusDocuments\Resources\Cresus.Tests",
                500,
                ResourceModuleLayer.System
            );
            ResourceManager manager = new ResourceManager(new ResourceManagerPool(), module);
            manager.DefineDefaultModuleName("Cresus.Tests");

            accessor.Load(manager);

            Assert.AreEqual(8, accessor.Collection.Count);
            Assert.AreEqual("TestAvecHeritage", accessor.Collection[5].Name);

            foreach (CultureMap item in accessor.Collection)
            {
                System.Console.Out.WriteLine("{0}: {1}", item.Id, item.Name);
            }

            StructuredData data = accessor
                .Collection[5]
                .GetCultureData(Epsitec.Common.Support.Resources.DefaultTwoLetterISOLanguageName);
            string xml =
                data.GetValue(Epsitec.Common.Support.Res.Fields.ResourcePanel.XmlSource) as string;

            Assert.IsNotNull(xml);

            System.Console.Out.WriteLine(xml);

            Assert.AreEqual(
                "200;200",
                data.GetValue(Epsitec.Common.Support.Res.Fields.ResourcePanel.DefaultSize)
            );
            Assert.IsTrue(xml.StartsWith("<panel"));
            Assert.IsTrue(xml.EndsWith("</panel>"));

            CultureMap item1 = accessor.Collection[1];
            CultureMap item2 = accessor.Collection[2];
            CultureMap newItem = accessor.CreateItem();

            newItem.Name = "FooBar";

            ResourceBundleBatchSaver saver = new ResourceBundleBatchSaver();

            accessor.Collection.RemoveAt(2);
            accessor.Collection.RemoveAt(1);
            accessor.Collection.Insert(1, item1);
            accessor.Collection.Insert(1, item2);
            accessor.Collection.Add(newItem);

            Assert.IsTrue(accessor.ContainsChanges);
            accessor.PersistChanges();
            Assert.IsFalse(accessor.ContainsChanges);

            accessor.Save(saver.DelaySave);
            saver.Execute();

            accessor.Collection.RemoveAt(2);
            accessor.Collection.RemoveAt(1);
            accessor.Collection.Insert(1, item1);
            accessor.Collection.Insert(2, item2);

            accessor.Collection.Remove(newItem);
            accessor.PersistChanges();

            accessor.Save(saver.DelaySave);
            saver.Execute();
        }

        public enum MyTestEnum
        {
            None,

            Foo,
            Bar,

            [Hidden]
            Other
        }

        [Test]
        public void CheckMetadata()
        {
            Epsitec.Common.Support.ResourceAccessors.StringResourceAccessor stringAccessor =
                new Epsitec.Common.Support.ResourceAccessors.StringResourceAccessor();
            Epsitec.Common.Support.ResourceAccessors.CaptionResourceAccessor captionAccessor =
                new Epsitec.Common.Support.ResourceAccessors.CaptionResourceAccessor();
            Epsitec.Common.Support.ResourceAccessors.CommandResourceAccessor commandAccessor =
                new Epsitec.Common.Support.ResourceAccessors.CommandResourceAccessor();
            Epsitec.Common.Support.ResourceAccessors.StructuredTypeResourceAccessor structAccessor =
                new Epsitec.Common.Support.ResourceAccessors.StructuredTypeResourceAccessor();

            stringAccessor.Load(this.manager);
            captionAccessor.Load(this.manager);
            commandAccessor.Load(this.manager);
            structAccessor.Load(Epsitec.Common.Support.Res.Manager);

            System.Console.Out.WriteLine("Strings:");
            this.DumpCultureMap(stringAccessor.Collection[0]);
            System.Console.Out.WriteLine("Captions:");
            this.DumpCultureMap(captionAccessor.Collection[0]);
            System.Console.Out.WriteLine("Commands:");
            this.DumpCultureMap(commandAccessor.Collection[0]);
            System.Console.Out.WriteLine("Structured Types:");
            this.DumpCultureMap(structAccessor.Collection[0]);
        }

        [Test]
        public void CheckStringAccessor()
        {
            Epsitec.Common.Support.ResourceAccessors.StringResourceAccessor accessor =
                new Epsitec.Common.Support.ResourceAccessors.StringResourceAccessor();

            Assert.IsFalse(accessor.ContainsChanges);

            accessor.Load(this.manager);

            Assert.AreEqual(9, accessor.Collection.Count);

            Assert.AreEqual(Druid.Parse("[4002]"), accessor.Collection[Druid.Parse("[4002]")].Id);
            Assert.AreEqual(
                "Text A",
                accessor
                    .Collection[Druid.Parse("[4002]")]
                    .GetCultureData("00")
                    .GetValue(Epsitec.Common.Support.Res.Fields.ResourceString.Text)
            );

            Assert.AreEqual(Druid.Parse("[4006]"), accessor.Collection[Druid.Parse("[4006]")].Id);
            Assert.AreEqual("Text1", accessor.Collection[Druid.Parse("[4006]")].Name);
            Assert.AreEqual(
                "Hello, world",
                accessor
                    .Collection["Text1"]
                    .GetCultureData("00")
                    .GetValue(Epsitec.Common.Support.Res.Fields.ResourceString.Text)
            );

            Assert.AreEqual(Druid.Parse("[4008]"), accessor.Collection[Druid.Parse("[4008]")].Id);
            Assert.IsNull(
                accessor
                    .Collection[Druid.Parse("[4008]")]
                    .GetCultureData("00")
                    .GetValue(Epsitec.Common.Support.Res.Fields.ResourceString.Text)
            );

            StructuredData data1 = accessor.Collection["Text1"].GetCultureData("fr");
            StructuredData data2 = accessor.Collection["Text1"].GetCultureData("fr");

            Assert.AreSame(data1, data2);
            Assert.AreEqual(
                "Bonjour",
                data1.GetValue(Epsitec.Common.Support.Res.Fields.ResourceString.Text)
            );
            Assert.AreEqual(
                0,
                data1.GetValue(Epsitec.Common.Support.Res.Fields.ResourceBase.ModificationId)
            );
            Assert.IsFalse(accessor.ContainsChanges);

            data1 = accessor.Collection["Text1"].GetCultureData("de");
            data2 = accessor.Collection["Text1"].GetCultureData("de");

            Assert.IsNotNull(data1);
            Assert.AreSame(data1, data2);
            Assert.AreEqual(
                UndefinedValue.Value,
                data1.GetValue(Epsitec.Common.Support.Res.Fields.ResourceString.Text)
            );
            Assert.IsFalse(accessor.ContainsChanges);
            Assert.IsTrue(data1.IsEmpty);

            data1 = accessor.Collection["Text1"].GetCultureData("fr");
            data1.SetValue(
                Epsitec.Common.Support.Res.Fields.ResourceString.Text,
                "Bonjour tout le monde"
            );
            data2.SetValue(Epsitec.Common.Support.Res.Fields.ResourceString.Text, "Hallo, Welt");
            data2.SetValue(Epsitec.Common.Support.Res.Fields.ResourceBase.ModificationId, 1);

            Assert.IsTrue(accessor.ContainsChanges);
            Assert.AreEqual(1, accessor.PersistChanges());
            Assert.IsFalse(accessor.ContainsChanges);

            Assert.AreEqual(
                "Bonjour tout le monde",
                this.manager.GetText(
                    Druid.Parse("[4006]"),
                    ResourceLevel.Localized,
                    Epsitec.Common.Support.Resources.FindCultureInfo("fr")
                )
            );
            Assert.AreEqual(
                "Hallo, Welt",
                this.manager.GetText(
                    Druid.Parse("[4006]"),
                    ResourceLevel.Localized,
                    Epsitec.Common.Support.Resources.FindCultureInfo("de")
                )
            );
            Assert.AreEqual(
                1,
                this.manager.GetBundle(
                    "Strings",
                    ResourceLevel.Localized,
                    Epsitec.Common.Support.Resources.FindCultureInfo("de")
                )[Druid.Parse("[4006]")].ModificationId
            );

            CultureMap map = accessor.CreateItem();

            Assert.IsNotNull(map);
            Assert.AreEqual(Druid.Parse("[4009]"), map.Id);
            Assert.IsNull(accessor.Collection[map.Id]);

            accessor.Collection.Add(map);
            Assert.IsTrue(accessor.ContainsChanges);

            map.Name = "NewItem";
            map.GetCultureData("00")
                .SetValue(Epsitec.Common.Support.Res.Fields.ResourceString.Text, "New value");
            map.GetCultureData("fr")
                .SetValue(Epsitec.Common.Support.Res.Fields.ResourceString.Text, "Nouvelle valeur");

            Assert.AreEqual(1, accessor.PersistChanges());
            Assert.IsFalse(accessor.ContainsChanges);

            Assert.AreEqual(
                "New value",
                this.manager.GetText(Druid.Parse("[4009]"), ResourceLevel.Default)
            );
            Assert.AreEqual(
                "Nouvelle valeur",
                this.manager.GetText(
                    Druid.Parse("[4009]"),
                    ResourceLevel.Merged,
                    Epsitec.Common.Support.Resources.FindCultureInfo("fr")
                )
            );

            map.GetCultureData("fr")
                .SetValue(
                    Epsitec.Common.Support.Res.Fields.ResourceString.Text,
                    UndefinedValue.Value
                );

            Assert.AreEqual(1, accessor.PersistChanges());
            Assert.IsFalse(accessor.ContainsChanges);

            Assert.AreEqual(
                "New value",
                this.manager.GetText(Druid.Parse("[4009]"), ResourceLevel.Default)
            );
            Assert.AreEqual(
                "New value",
                this.manager.GetText(
                    Druid.Parse("[4009]"),
                    ResourceLevel.Merged,
                    Epsitec.Common.Support.Resources.FindCultureInfo("fr")
                )
            );

            accessor.Collection.Remove(map);
            Assert.IsTrue(accessor.ContainsChanges);
            Assert.AreEqual(1, accessor.PersistChanges());
            Assert.IsFalse(accessor.ContainsChanges);

            Assert.IsNull(this.manager.GetText(Druid.Parse("[4009]"), ResourceLevel.Default));
            Assert.IsNull(
                this.manager.GetText(
                    Druid.Parse("[4009]"),
                    ResourceLevel.Localized,
                    Epsitec.Common.Support.Resources.FindCultureInfo("fr")
                )
            );
        }

        [Test]
        public void CheckUI()
        {
            ResourceManager manager = new ResourceManager(typeof(ResourceAccessorTest));
            manager.DefineDefaultModuleName("Common.Document");

            Epsitec.Common.Support.ResourceAccessors.StringResourceAccessor stringAccessor =
                new Epsitec.Common.Support.ResourceAccessors.StringResourceAccessor();
            stringAccessor.Load(manager);

            IResourceAccessor accessor = stringAccessor;

            StructuredType cultureMapType = new StructuredType();
            cultureMapType.Fields.Add("Name", StringType.NativeDefault);

            CollectionView collectionView = new CollectionView(accessor.Collection);

            Factory.SetActive("LookRoyale");
            Window window = new Window();
            window.Text = "CheckUI";
            window.ClientSize = new Size(300, 500);

            ItemTable table = new ItemTable(window.Root);
            table.Dock = DockStyle.Fill;

            table.SourceType = cultureMapType;
            table.Items = collectionView;
            table.Columns.Add(new Epsitec.Common.UI.ItemTableColumn("Name"));
            table.HorizontalScrollMode = Epsitec.Common.UI.ItemTableScrollMode.None;
            table.VerticalScrollMode = Epsitec.Common.UI.ItemTableScrollMode.ItemBased;
            table.HeaderVisibility = false;
            table.FrameVisibility = false;
            table.ItemPanel.Layout = Epsitec.Common.UI.ItemPanelLayout.VerticalList;
            table.ItemPanel.ItemSelectionMode = Epsitec.Common.UI.ItemPanelSelectionMode.ExactlyOne;
            table.Margins = new Margins(4, 1, 4, 2);

            table.SizeChanged += this.HandleTableSizeChanged;

            TextFieldMulti field = new Epsitec.Common.Widgets.TextFieldMulti(window.Root);
            field.Dock = DockStyle.Bottom;
            field.PreferredHeight = 60;
            field.Margins = new Margins(4, 0, 2, 4);

            HSplitter splitter = new Epsitec.Common.Widgets.HSplitter(window.Root);
            splitter.Dock = DockStyle.Bottom;
            splitter.PreferredHeight = 8;

            //	Critꣃ爀攀 搀攀 琀爀椀 㨀 猀攀氀漀渀 氀攀 渀漀洀 ⠀漀渀 渀✀愀 瀀愀猀 瘀爀愀椀洀攀渀琀 氀攀 挀栀漀椀砀Ⰰ 瘀甀 氀愀 搀쌀©finition
            //	de CultureMap)

            collectionView.SortDescriptions.Clear();
            collectionView.SortDescriptions.Add(new Epsitec.Common.Types.SortDescription("Name"));

            //	Filtre uniquement les items qui ont un "b" dans leur nom :

            collectionView.Filter += delegate(object obj)
            {
                CultureMap item = obj as CultureMap;

                if (item.Name.Contains("b"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            };

            table.ItemPanel.SelectionChanged += delegate
            {
                CultureMap item = collectionView.CurrentItem as CultureMap;
                System.Text.StringBuilder buffer = new System.Text.StringBuilder();
                string[] cultures = new string[] { "00", "fr", "en", "de" };

                foreach (string culture in cultures)
                {
                    StructuredData data = item.GetCultureData(culture);
                    string text =
                        data.GetValue(Epsitec.Common.Support.Res.Fields.ResourceString.Text)
                        as string;
                    if (text != null)
                    {
                        buffer.Append(culture);
                        buffer.Append(": ");
                        buffer.Append(TextLayout.ConvertToTaggedText(text));
                        buffer.Append("<br/>");
                    }
                }
                field.Text = buffer.ToString();
            };

            window.Show();
            Window.RunInTestEnvironment(window);
        }

        void HandleTableSizeChanged(
            object sender,
            Epsitec.Common.Types.DependencyPropertyChangedEventArgs e
        )
        {
            ItemTable table = (ItemTable)sender;
            Size size = (Size)e.NewValue;

            double width = size.Width - table.GetPanelPadding().Width;
            table.ColumnHeader.SetColumnWidth(0, width);

            table.ItemPanel.ItemViewDefaultSize = new Epsitec.Common.Drawing.Size(width, 20);
        }

        private void DumpCultureMap(CultureMap map)
        {
            foreach (string culture in map.GetDefinedCultures())
            {
                StructuredData data = map.GetCultureData(culture);
                IStructuredType type = data.StructuredType;

                this.DumpStructuredData("", data, type);
            }
        }

        private void DumpStructuredData(string indent, StructuredData data, IStructuredType type)
        {
            if (data == null)
            {
                return;
            }

            foreach (string fieldId in type.GetFieldIds())
            {
                StructuredTypeField field = type.GetField(fieldId);
                Caption caption = this.manager.GetCaption(field.CaptionId);

                System.Console.Out.WriteLine(
                    "{4}{0} ({1}) : type = {2}, data = {3}, relation = {5}, {6}",
                    fieldId,
                    (caption == null) ? "<?>" : caption.Name,
                    (field.Type == null) ? "<null>" : field.Type.Name,
                    data.GetValue(fieldId),
                    indent,
                    field.Relation,
                    field.Membership
                );

                if ((field.Type is IStructuredType) && (field.Relation != FieldRelation.Collection))
                {
                    this.DumpStructuredData(
                        "  " + indent,
                        data.GetValue(fieldId) as StructuredData,
                        field.Type as IStructuredType
                    );
                }
                else if (field.Type is CollectionType)
                {
                    CollectionType collectionType = field.Type as CollectionType;

                    if (collectionType.ItemType is IStructuredType)
                    {
                        System.Collections.IList collection =
                            data.GetValue(fieldId) as System.Collections.IList;
                        StructuredData item0;

                        if (collection.Count > 0)
                        {
                            item0 = collection[0] as StructuredData;
                        }
                        else
                        {
                            item0 = new StructuredData(collectionType.ItemType as IStructuredType);
                        }

                        this.DumpStructuredData(
                            "* " + indent,
                            item0,
                            collectionType.ItemType as IStructuredType
                        );
                    }
                }
                else if (field.Relation == FieldRelation.Collection)
                {
                    AbstractType collectionType = field.Type as AbstractType;

                    if (collectionType is IStructuredType)
                    {
                        System.Collections.IList collection =
                            data.GetValue(fieldId) as System.Collections.IList;
                        StructuredData item0;

                        if (collection.Count > 0)
                        {
                            item0 = collection[0] as StructuredData;
                        }
                        else
                        {
                            item0 = new StructuredData(collectionType as IStructuredType);
                        }

                        this.DumpStructuredData(
                            "* " + indent,
                            item0,
                            collectionType as IStructuredType
                        );
                    }
                }
            }
        }

        ResourceManager manager;
    }
}
