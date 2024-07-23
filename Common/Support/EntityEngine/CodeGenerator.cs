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

﻿//	Copyright © 2007-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;
using Epsitec.Common.Support.CodeGeneration;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

namespace Epsitec.Common.Support.EntityEngine
{
    using Keywords = CodeHelper.Keywords;

    /// <summary>
    /// The <c>CodeGenerator</c> class produces a C# source code wrapper for
    /// the entities defined in the resource files.
    /// </summary>
    public class CodeGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeGenerator"/> class.
        /// </summary>
        /// <param name="resourceManager">The resource manager.</param>
        public CodeGenerator(ResourceManager resourceManager)
            : this(new CodeFormatter(), resourceManager) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeGenerator"/> class.
        /// </summary>
        /// <param name="formatter">The formatter.</param>
        /// <param name="resourceManager">The resource manager.</param>
        public CodeGenerator(CodeFormatter formatter, ResourceManager resourceManager)
        {
            this.formatter = formatter;
            this.resourceManager = resourceManager;
            this.resourceManagerPool = this.resourceManager.Pool;
            this.resourceModuleInfo = this.resourceManagerPool.GetModuleInfo(
                this.resourceManager.DefaultModulePath
            );
            this.sourceNamespaceRes = this.resourceModuleInfo.SourceNamespaceRes;
            this.sourceNamespaceEntities = this.resourceModuleInfo.SourceNamespaceEntities;
        }

        /// <summary>
        /// Gets the formatter used when generating code.
        /// </summary>
        /// <value>The formatter.</value>
        public CodeFormatter Formatter
        {
            get { return this.formatter; }
        }

        /// <summary>
        /// Gets the resource manager.
        /// </summary>
        /// <value>The resource manager.</value>
        public ResourceManager ResourceManager
        {
            get { return this.resourceManager; }
        }

        /// <summary>
        /// Gets the resource manager pool.
        /// </summary>
        /// <value>The resource manager pool.</value>
        public ResourceManagerPool ResourceManagerPool
        {
            get { return this.resourceManagerPool; }
        }

        /// <summary>
        /// Gets the resource module information.
        /// </summary>
        /// <value>The resource module information.</value>
        public ResourceModuleInfo ResourceModuleInfo
        {
            get { return this.resourceModuleInfo; }
        }

        /// <summary>
        /// Gets the source namespace.
        /// </summary>
        /// <value>The source namespace.</value>
        public string SourceNamespaceRes
        {
            get { return this.sourceNamespaceRes ?? this.sourceNamespaceEntities; }
        }

        /// <summary>
        /// Gets the source namespace.
        /// </summary>
        /// <value>The source namespace.</value>
        public string SourceNamespaceEntities
        {
            get { return this.sourceNamespaceEntities ?? this.sourceNamespaceRes; }
        }

        /// <summary>
        /// Emits the code for all entities and interfaces defined in the
        /// current module.
        /// </summary>
        public void Emit()
        {
            CodeHelper.EmitHeader(this.formatter);

            ResourceAccessors.StructuredTypeResourceAccessor accessor =
                new ResourceAccessors.StructuredTypeResourceAccessor();
            accessor.Load(this.resourceManager);

            foreach (CultureMap item in accessor.Collection)
            {
                Caption caption = this.resourceManager.GetCaption(item.Id);
                StructuredType type =
                    TypeRosetta.CreateTypeObject(caption, false) as StructuredType;

                if (
                    (type != null)
                    && (
                        (type.Class == StructuredTypeClass.Entity)
                        || (type.Class == StructuredTypeClass.Interface)
                    )
                )
                {
                    this.Emit(type);
                }
            }

            this.Formatter.Flush();
        }

        /// <summary>
        /// Emits the source code for the entity described by the specified type.
        /// </summary>
        /// <param name="type">The structured type.</param>
        public void Emit(StructuredType type)
        {
            StructuredTypeClass typeClass = type.Class;

            string name = type.Caption.Name;
            var specifiers = new List<string>();
            var baseInterfaceIds = new HashSet<Druid>(
                CodeGenerator.GetFlatInterfaceIds(type.BaseType)
            );

            System.Diagnostics.Debug.Assert(
                (typeClass == StructuredTypeClass.Entity)
                    || (typeClass == StructuredTypeClass.Interface)
            );

            //	Define the base type from which the entity class will inherit; it is
            //	either the defined base type entity or the root AbstractEntity class
            //	for entities. For interfaces, this does not apply.

            switch (typeClass)
            {
                case StructuredTypeClass.Entity:
                    if (type.BaseTypeId.IsValid)
                    {
                        specifiers.Add(this.CreateEntityFullName(type.BaseTypeId));
                    }
                    else
                    {
                        specifiers.Add(
                            string.Concat(Keywords.Global, "::", Keywords.AbstractEntity)
                        );
                    }
                    break;

                case StructuredTypeClass.Interface:
                    System.Diagnostics.Debug.Assert(type.BaseTypeId.IsEmpty);
                    break;

                default:
                    throw new System.ArgumentException(
                        string.Format(
                            "StructuredTypeClass.{0} not valid in this context",
                            typeClass
                        )
                    );
            }

            //	Include all locally imported interfaces, if any :

            foreach (
                Druid interfaceId in type.InterfaceIds.Where(id => !baseInterfaceIds.Contains(id))
            )
            {
                specifiers.Add(this.CreateEntityFullName(interfaceId));
            }

            Emitter emitter;
            InterfaceImplementationEmitter implementationEmitter;

            this.formatter.WriteCodeLine(
                Keywords.BeginRegion,
                " ",
                this.SourceNamespaceEntities,
                ".",
                name,
                " ",
                typeClass.ToString()
            );

            if (typeClass == StructuredTypeClass.Entity)
            {
                this.formatter.WriteAssemblyAttribute(
                    "[assembly: ",
                    Keywords.EntityClassAttribute,
                    @" (""",
                    type.CaptionId.ToString(),
                    @""", typeof (",
                    CodeGenerator.CreateEntityNamespace(this.SourceNamespaceEntities),
                    ".",
                    CodeGenerator.CreateEntityIdentifier(name),
                    "))]"
                );
            }

            this.formatter.WriteBeginNamespace(
                CodeGenerator.CreateEntityNamespace(this.SourceNamespaceEntities)
            );

            this.EmitClassComment(type);

            switch (typeClass)
            {
                case StructuredTypeClass.Entity:
                    emitter = new EntityEmitter(this, type, baseInterfaceIds);

                    CodeAttributes classAttributes =
                        type.Flags.HasFlag(StructuredTypeFlags.AbstractClass)
                        && !type.Flags.HasFlag(StructuredTypeFlags.GenerateRepository)
                            ? CodeHelper.AbstractEntityClassAttributes
                            : CodeHelper.EntityClassAttributes;

                    this.formatter.WriteBeginClass(
                        classAttributes,
                        CodeGenerator.CreateEntityIdentifier(name),
                        specifiers
                    );
                    type.ForEachField(emitter.EmitLocalProperty);
                    emitter.EmitCloseRegion();
                    this.formatter.WriteCodeLine();
                    type.ForEachField(emitter.EmitLocalPropertyHandlers);
                    emitter.EmitCloseRegion();
                    this.formatter.WriteCodeLine();
                    type.ForEachField(emitter.EmitMethodsForLocalVirtualProperties);
                    this.formatter.WriteCodeLine();
                    this.EmitMethods(type);
                    emitter.EmitClasses(type);
                    this.formatter.WriteEndClass();
                    break;

                case StructuredTypeClass.Interface:
                    emitter = new InterfaceEmitter(this, type);

                    this.formatter.WriteBeginInterface(
                        CodeHelper.PublicInterfaceAttributes,
                        CodeGenerator.CreateInterfaceIdentifier(name),
                        specifiers
                    );
                    type.ForEachField(emitter.EmitLocalProperty);
                    emitter.EmitCloseRegion();
                    this.formatter.WriteEndInterface();

                    implementationEmitter = new InterfaceImplementationEmitter(this, type);

                    this.formatter.WriteBeginClass(
                        CodeHelper.PublicStaticPartialClassAttributes,
                        CodeGenerator.CreateInterfaceImplementationIdentifier(name)
                    );
                    type.ForEachField(implementationEmitter.EmitLocalPropertyImplementation);
                    this.formatter.WriteEndClass();
                    break;
            }

            this.formatter.WriteEndNamespace();
            this.formatter.WriteCodeLine(Keywords.EndRegion);
            this.formatter.WriteCodeLine();
        }

        private static IEnumerable<Druid> GetFlatInterfaceIds(StructuredType type)
        {
            while (type != null)
            {
                foreach (var id in type.InterfaceIds)
                {
                    yield return id;
                }

                type = type.BaseType;
            }
        }

        private void EmitClassComment(StructuredType type)
        {
            string description = type.Caption.Description;

            if (string.IsNullOrEmpty(description))
            {
                description = string.Concat("The <c>", type.Caption.Name, "</c> entity.");
            }

            this.formatter.WriteCodeLine(Keywords.XmlComment, "\t", Keywords.XmlBeginSummary);

            foreach (
                string line in description.Split(
                    new char[] { '\n', '\r' },
                    System.StringSplitOptions.RemoveEmptyEntries
                )
            )
            {
                this.formatter.WriteCodeLine(Keywords.XmlComment, "\t", line);
            }

            this.formatter.WriteCodeLine(
                Keywords.XmlComment,
                "\t",
                Keywords.DesignerCaptionProtocol,
                type.CaptionId.ToString().Trim('[', ']')
            );
            this.formatter.WriteCodeLine(Keywords.XmlComment, "\t", Keywords.XmlEndSummary);
        }

        private void EmitPropertyComment(
            StructuredType type,
            StructuredTypeField field,
            string fieldName
        )
        {
            Caption caption = this.resourceManager.GetCaption(field.CaptionId);
            string description = caption == null ? null : caption.Description;

            if (string.IsNullOrEmpty(description))
            {
                description = string.Concat("The <c>", fieldName, "</c> field.");
            }

            this.formatter.WriteCodeLine(Keywords.XmlComment, "\t", Keywords.XmlBeginSummary);

            foreach (
                string line in description.Split(
                    new char[] { '\n', '\r' },
                    System.StringSplitOptions.RemoveEmptyEntries
                )
            )
            {
                this.formatter.WriteCodeLine(Keywords.XmlComment, "\t", line);
            }

            this.formatter.WriteCodeLine(
                Keywords.XmlComment,
                "\t",
                Keywords.DesignerEntityFieldProtocol,
                type.CaptionId.ToString().Trim('[', ']'),
                "/",
                field.Id.Trim('[', ']')
            );
            this.formatter.WriteCodeLine(Keywords.XmlComment, "\t", Keywords.XmlEndSummary);
        }

        private void EmitPropertyAttribute(
            StructuredType structuredType,
            StructuredTypeField field,
            string propName
        )
        {
            if (field.Options.HasFlag(FieldOptions.Virtual))
            {
                this.formatter.WriteCodeLine(
                    "[",
                    Keywords.EntityFieldAttribute,
                    " (",
                    @"""",
                    field.CaptionId.ToString(),
                    @"""",
                    ", ",
                    Keywords.EntityFieldAttributeIsVirtual,
                    ")]"
                );
            }
            else
            {
                this.formatter.WriteCodeLine(
                    "[",
                    Keywords.EntityFieldAttribute,
                    " (",
                    @"""",
                    field.CaptionId.ToString(),
                    @"""",
                    ")]"
                );
            }
        }

        private void EmitMethods(StructuredType type)
        {
            Druid id = type.CaptionId;
            string className = this.CreateEntityFullName(type.CaptionId);

            string code;

            code = string.Concat(
                Keywords.Druid,
                " ",
                Keywords.GetEntityStructuredTypeIdMethod,
                "()"
            );
            this.formatter.WriteBeginMethod(CodeHelper.PublicOverrideMethodAttributes, code);
            this.formatter.WriteCodeLine(
                Keywords.Return,
                " ",
                className,
                ".",
                Keywords.EntityStructuredTypeIdProperty,
                ";"
            );
            this.formatter.WriteEndMethod();

            code = string.Concat(
                Keywords.String,
                " ",
                Keywords.GetEntityStructuredTypeKeyMethod,
                "()"
            );
            this.formatter.WriteBeginMethod(CodeHelper.PublicOverrideMethodAttributes, code);
            this.formatter.WriteCodeLine(
                Keywords.Return,
                " ",
                className,
                ".",
                Keywords.EntityStructuredTypeKeyProperty,
                ";"
            );
            this.formatter.WriteEndMethod();

            CodeAttributes fieldAttributes =
                type.BaseType == null
                    ? CodeHelper.PublicStaticReadOnlyFieldAttributes
                    : CodeHelper.PublicStaticNewReadOnlyFieldAttributes;

            this.formatter.WriteField(
                fieldAttributes,
                Keywords.Druid,
                " ",
                Keywords.EntityStructuredTypeIdProperty,
                " = ",
                /**/Keywords.New,
                " ",
                Keywords.Druid,
                " (",
                /**/id.Module.ToString(System.Globalization.CultureInfo.InvariantCulture),
                ", ",
                /**/id.DeveloperAndPatchLevel.ToString(
                    System.Globalization.CultureInfo.InvariantCulture
                ),
                ", ",
                /**/id.Local.ToString(System.Globalization.CultureInfo.InvariantCulture),
                ");",
                /**/"\t",
                Keywords.SimpleComment,
                " ",
                id.ToString()
            );

            this.formatter.WriteField(
                fieldAttributes,
                Keywords.String,
                " ",
                Keywords.EntityStructuredTypeKeyProperty,
                " = ",
                /**/Keywords.Quote,
                id.ToString(),
                Keywords.Quote,
                ";"
            );
        }

        private static string CreateEntityIdentifier(string name)
        {
            return string.Concat(name, Keywords.EntitySuffix);
        }

        private static string CreatePropertyIdentifier(string name)
        {
            return name;
        }

        private static string CreateMethodIdentifierForVirtualPropertyGetter(string name)
        {
            return string.Concat("Get", name);
        }

        private static string CreateMethodIdentifierForVirtualPropertySetter(string name)
        {
            return string.Concat("Set", name);
        }

        private static string CreateInterfaceIdentifier(string name)
        {
            return name;
        }

        private static string CreateInterfaceImplementationIdentifier(string name)
        {
            return string.Concat(name, Keywords.InterfaceImplementationSuffix);
        }

        private static string CreateEntityNamespace(string name)
        {
            return string.Concat(name, ".", Keywords.Entities);
        }

        private string CreateEntityShortName(Druid id)
        {
            Caption caption = this.resourceManager.GetCaption(id);
            StructuredType type = TypeRosetta.GetTypeObject(caption) as StructuredType;

            System.Diagnostics.Debug.Assert(type != null);

            string identifier;

            switch (type.Class)
            {
                case StructuredTypeClass.Entity:
                    identifier = CodeGenerator.CreateEntityIdentifier(caption.Name);
                    break;

                case StructuredTypeClass.Interface:
                    identifier = CodeGenerator.CreateInterfaceIdentifier(caption.Name);
                    break;

                default:
                    throw new System.ArgumentException(
                        string.Format(
                            "StructuredTypeClass.{0} not valid in this context",
                            type.Class
                        )
                    );
            }

            return identifier;
        }

        private string CreateEntityFullName(Druid id)
        {
            Caption caption = this.resourceManager.GetCaption(id);
            StructuredType type = TypeRosetta.GetTypeObject(caption) as StructuredType;

            IList<ResourceModuleInfo> infos = this.resourceManagerPool.FindModuleInfos(id.Module);

            System.Diagnostics.Debug.Assert(infos.Count > 0);
            System.Diagnostics.Debug.Assert(caption != null);
            System.Diagnostics.Debug.Assert(
                !string.IsNullOrEmpty(infos[0].SourceNamespaceEntities)
            );
            System.Diagnostics.Debug.Assert(type != null);
            System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(caption.Name));

            string identifier;

            switch (type.Class)
            {
                case StructuredTypeClass.Entity:
                    identifier = CodeGenerator.CreateEntityIdentifier(caption.Name);
                    break;

                case StructuredTypeClass.Interface:
                    identifier = CodeGenerator.CreateInterfaceIdentifier(caption.Name);
                    break;

                default:
                    throw new System.ArgumentException(
                        string.Format(
                            "StructuredTypeClass.{0} not valid in this context",
                            type.Class
                        )
                    );
            }

            return string.Concat(
                Keywords.Global,
                "::",
                CodeGenerator.CreateEntityNamespace(infos[0].SourceNamespaceEntities),
                ".",
                identifier
            );
        }

        private string CreatePropertyName(Druid id)
        {
            Caption caption = this.resourceManager.GetCaption(id);
            return CodeGenerator.CreatePropertyIdentifier(caption.Name);
        }

        private string CreateMethodNameForVirtualPropertyGetter(Druid id)
        {
            Caption caption = this.resourceManager.GetCaption(id);
            return CodeGenerator.CreateMethodIdentifierForVirtualPropertyGetter(caption.Name);
        }

        private string CreateMethodNameForVirtualPropertySetter(Druid id)
        {
            Caption caption = this.resourceManager.GetCaption(id);
            return CodeGenerator.CreateMethodIdentifierForVirtualPropertySetter(caption.Name);
        }

        private string CreateTypeFullName(Druid typeId)
        {
            return this.CreateTypeFullName(typeId, false);
        }

        private string CreateTypeFullName(Druid typeId, bool nullable)
        {
            Caption caption = this.resourceManager.GetCaption(typeId);
            AbstractType type = TypeRosetta.GetTypeObject(caption);
            System.Type sysType = type.SystemType;

            if ((sysType == typeof(StructuredType)) || (sysType == null))
            {
                return this.CreateEntityFullName(typeId);
            }
            else if (nullable && (sysType.IsValueType) && (!TypeRosetta.IsNullable(sysType)))
            {
                return string.Concat(CodeGenerator.GetTypeName(sysType), "?");
            }
            else
            {
                return CodeGenerator.GetTypeName(sysType);
            }
        }

        private string CreateFieldTypeFullName(StructuredTypeField field)
        {
            string typeName = this.CreateTypeFullName(field.TypeId, TypeRosetta.IsNullable(field));

            if (field.Relation == FieldRelation.Collection)
            {
                typeName = string.Concat(
                    Keywords.Global,
                    "::",
                    Keywords.GenericIList,
                    "<",
                    typeName,
                    ">"
                );
            }

            return typeName;
        }

        private static string GetTypeName(System.Type systemType)
        {
            string systemTypeName = systemType.FullName;

            switch (systemTypeName)
            {
                case "System.Boolean":
                    return "bool";
                case "System.Int16":
                    return "short";
                case "System.Int32":
                    return "int";
                case "System.Int64":
                    return "long";
                case "System.String":
                    return "string";
            }

            return string.Concat(Keywords.Global, "::", systemTypeName);
        }

        #region Emitter Class

        private abstract class Emitter
        {
            public Emitter(CodeGenerator generator, StructuredType type)
            {
                this.generator = generator;
                this.type = type;
            }

            public virtual void EmitLocalProperty(StructuredTypeField field)
            {
                if (
                    (field.Membership == FieldMembership.Local)
                    || (field.Membership == FieldMembership.LocalOverride)
                )
                {
                    if (this.SuppressField(field))
                    {
                        return;
                    }

                    this.EmitInterfaceRegion(field);

                    string typeName = this.generator.CreateTypeFullName(
                        field.TypeId,
                        TypeRosetta.IsNullable(field)
                    );
                    string propName = this.generator.CreatePropertyName(field.CaptionId);

                    this.generator.EmitPropertyComment(this.type, field, propName);
                    this.generator.EmitPropertyAttribute(this.type, field, propName);

                    this.EmitLocalBeginProperty(field, typeName, propName);

                    switch (field.Source)
                    {
                        case FieldSource.Value:
                            this.EmitPropertyGetter(field, typeName, propName);
                            this.EmitPropertySetter(field, typeName, propName);
                            break;

                        case FieldSource.Expression:
                            this.EmitPropertyGetter(field, typeName, propName);
                            this.EmitPropertySetter(field, typeName, propName);
                            break;

                        default:
                            throw new System.ArgumentException(
                                string.Format(
                                    "FieldSource.{0} not valid in this context",
                                    field.Source
                                )
                            );
                    }

                    this.generator.formatter.WriteEndProperty();
                }
            }

            protected virtual bool SuppressField(StructuredTypeField field)
            {
                return false;
            }

            public void EmitLocalPropertyHandlers(StructuredTypeField field)
            {
                if ((field.Membership == FieldMembership.Local) && (field.DefiningTypeId.IsEmpty))
                {
                    string typeName = this.generator.CreateTypeFullName(
                        field.TypeId,
                        TypeRosetta.IsNullable(field)
                    );
                    string propName = this.generator.CreatePropertyName(field.CaptionId);

                    switch (field.Relation)
                    {
                        case FieldRelation.None:
                        case FieldRelation.Reference:
                            this.EmitPropertyOnChanging(field, typeName, propName);
                            this.EmitPropertyOnChanged(field, typeName, propName);
                            break;

                        case FieldRelation.Collection:
                            break;

                        default:
                            throw new System.NotSupportedException(
                                string.Format("Relation {0} not supported", field.Relation)
                            );
                    }
                }
            }

            public void EmitMethodsForLocalVirtualProperties(StructuredTypeField field)
            {
                if ((field.Membership == FieldMembership.Local) && (field.DefiningTypeId.IsEmpty))
                {
                    if (field.Options.HasFlag(FieldOptions.Virtual))
                    {
                        string fieldTypeName = this.generator.CreateFieldTypeFullName(field);

                        switch (field.Relation)
                        {
                            case FieldRelation.None:
                            case FieldRelation.Reference:
                                this.EmitMethodForVirtualPropertyGetter(field, fieldTypeName);
                                this.EmitMethodForVirtualPropertySetter(field, fieldTypeName);
                                break;

                            case FieldRelation.Collection:
                                this.EmitMethodForVirtualPropertyGetter(field, fieldTypeName);
                                break;

                            default:
                                throw new System.NotSupportedException(
                                    string.Format("Relation {0} not supported", field.Relation)
                                );
                        }
                    }
                }
            }

            public void EmitCloseRegion()
            {
                this.EmitInterfaceRegion(null);
            }

            protected virtual void EmitPropertyOnChanging(
                StructuredTypeField field,
                string typeName,
                string propName
            ) { }

            protected virtual void EmitPropertyOnChanged(
                StructuredTypeField field,
                string typeName,
                string propName
            ) { }

            protected virtual void EmitPropertyGetter(
                StructuredTypeField field,
                string typeName,
                string propName
            ) { }

            protected virtual void EmitPropertySetter(
                StructuredTypeField field,
                string typeName,
                string propName
            ) { }

            protected virtual void EmitMethodForVirtualPropertyGetter(
                StructuredTypeField field,
                string typeName
            ) { }

            protected virtual void EmitMethodForVirtualPropertySetter(
                StructuredTypeField field,
                string typeName
            ) { }

            private void EmitLocalBeginProperty(
                StructuredTypeField field,
                string typeName,
                string propName
            )
            {
                string code;

                switch (field.Relation)
                {
                    case FieldRelation.None:
                    case FieldRelation.Reference:
                        code = string.Concat(typeName, " ", propName);
                        break;

                    case FieldRelation.Collection:
                        code = string.Concat(
                            Keywords.Global,
                            "::",
                            Keywords.GenericIList,
                            "<",
                            typeName,
                            ">",
                            " ",
                            propName
                        );
                        break;

                    default:
                        throw new System.ArgumentException(
                            string.Format(
                                "FieldRelation.{0} not valid in this context",
                                field.Relation
                            )
                        );
                }

                if (field.Source == FieldSource.Expression)
                {
                    Druid definingTypeId = field.DefiningTypeId;

                    if (
                        (definingTypeId.IsEmpty)
                        || (this.type.InterfaceIds.Contains(definingTypeId))
                    )
                    {
                        this.generator.formatter.WriteBeginProperty(
                            CodeHelper.PublicVirtualPropertyAttributes,
                            code
                        );
                    }
                    else
                    {
                        this.generator.formatter.WriteBeginProperty(
                            CodeHelper.PublicOverridePropertyAttributes,
                            code
                        );
                    }
                }
                else
                {
                    this.generator.formatter.WriteBeginProperty(
                        CodeHelper.PublicPropertyAttributes,
                        code
                    );
                }
            }

            private void EmitInterfaceRegion(StructuredTypeField field)
            {
                Druid currentInterfaceId = field == null ? Druid.Empty : field.DefiningTypeId;

                if (this.interfaceId == currentInterfaceId)
                {
                    return;
                }

                if (this.interfaceId.IsValid)
                {
                    this.generator.formatter.WriteCodeLine(Keywords.EndRegion);
                }

                this.interfaceId = currentInterfaceId;

                if (this.interfaceId.IsValid)
                {
                    string interfaceName = this.generator.CreateEntityShortName(this.interfaceId);
                    this.generator.formatter.WriteCodeLine(
                        Keywords.BeginRegion,
                        " ",
                        interfaceName,
                        " Members"
                    );
                }
            }

            public void EmitClasses(StructuredType type)
            {
                if (type.Flags.HasFlag(StructuredTypeFlags.GenerateRepository))
                {
                    this.EmitRepositoryClass(type);
                }
            }

            private void EmitRepositoryClass(StructuredType type)
            {
                string entityClassName = CodeGenerator.CreateEntityIdentifier(type.Caption.Name);
                string expectancy = type.DefaultLifetimeExpectancy.ToString();

                this.generator.formatter.WriteCodeLine();

                this.EmitClassRegion(Keywords.Repository);

                bool overridesBaseRepository = this.IsBaseRepositoryOverriden(type);

                CodeAttributes repositoryAttributes = overridesBaseRepository
                    ? CodeHelper.RepositoryNewClassAttributes
                    : CodeHelper.RepositoryClassAttributes;

                this.generator.formatter.WriteBeginClass(
                    repositoryAttributes,
                    Keywords.Repository,
                    string.Concat(
                        Keywords.QualifiedGenericRepositoryBase,
                        "<",
                        entityClassName,
                        ">"
                    )
                );

                string code = string.Concat(
                    Keywords.Repository,
                    "(",
                    /**/Keywords.QualifiedCoreData,
                    " ",
                    Keywords.DataVariable,
                    ", ",
                    /**/Keywords.QualifiedDataContext,
                    " ",
                    Keywords.DataContextVariable,
                    /**/")",
                    " : ",
                    Keywords.Base,
                    "(",
                    /**/Keywords.DataVariable,
                    ", ",
                    Keywords.DataContextVariable,
                    ", ",
                    /**/Keywords.QualifiedDataLifetime,
                    ".",
                    expectancy,
                    ")"
                );
                this.generator.formatter.WriteBeginMethod(CodeHelper.PublicAttributes, code);
                this.generator.formatter.WriteEndMethod();
                this.generator.formatter.WriteEndClass();

                this.generator.formatter.WriteCodeLine(Keywords.EndRegion);
            }

            private bool IsBaseRepositoryOverriden(StructuredType type)
            {
                bool overridesBaseRepository = false;

                StructuredType baseType = type.BaseType;

                while (!overridesBaseRepository && baseType != null)
                {
                    overridesBaseRepository = type.BaseType.Flags.HasFlag(
                        StructuredTypeFlags.GenerateRepository
                    );

                    baseType = baseType.BaseType;
                }

                return overridesBaseRepository;
            }

            protected void EmitClassRegion(string className)
            {
                this.generator.formatter.WriteCodeLine(
                    Keywords.BeginRegion,
                    " ",
                    className,
                    " Class"
                );
            }

            protected CodeGenerator generator;
            protected StructuredType type;

            private Druid interfaceId;
        }

        #endregion

        #region EntityEmitter Class

        private sealed class EntityEmitter : Emitter
        {
            public EntityEmitter(
                CodeGenerator generator,
                StructuredType type,
                HashSet<Druid> baseEntityIds
            )
                : base(generator, type)
            {
                this.baseEntityIds = baseEntityIds;
            }

            protected override bool SuppressField(StructuredTypeField field)
            {
                //	Don't generate fields defined in inherited interfaces.
                if (this.baseEntityIds.Contains(field.DefiningTypeId))
                {
                    return true;
                }

                return false;
            }

            public override void EmitLocalProperty(StructuredTypeField field)
            {
                base.EmitLocalProperty(field);

                if (field.Source == FieldSource.Expression)
                {
                    if (
                        (field.DefiningTypeId.IsEmpty)
                        || (field.Membership == FieldMembership.LocalOverride)
                    )
                    {
                        //	The field is defined locally and is not the implementation of any
                        //	interface.

                        string typeName = this.generator.CreateTypeFullName(
                            field.TypeId,
                            TypeRosetta.IsNullable(field)
                        );
                        string propName = this.generator.CreatePropertyName(field.CaptionId);

                        this.EmitExpression(field, typeName, propName);
                    }
                }
            }

            protected override void EmitPropertyGetter(
                StructuredTypeField field,
                string typeName,
                string propName
            )
            {
                this.generator.formatter.WriteBeginGetter(CodeHelper.PublicAttributes);

                if (
                    (field.DefiningTypeId.IsEmpty)
                    || (field.Membership == FieldMembership.LocalOverride)
                )
                {
                    //	The field is defined locally and is not the implementation of any
                    //	interface.

                    if (field.Options.HasFlag(FieldOptions.Virtual))
                    {
                        // The field is virtual and we must fetch its value with a partial method.

                        var methodName = this.generator.CreateMethodNameForVirtualPropertyGetter(
                            field.CaptionId
                        );
                        var fieldTypeName = this.generator.CreateFieldTypeFullName(field);

                        this.generator.formatter.WriteCodeLine(
                            fieldTypeName,
                            " ",
                            Keywords.ValueVariable,
                            " = ",
                            Keywords.Default,
                            " (",
                            fieldTypeName,
                            ")",
                            ";"
                        );
                        this.generator.formatter.WriteCodeLine(
                            Keywords.This,
                            ".",
                            methodName,
                            " (",
                            Keywords.Ref,
                            " ",
                            Keywords.ValueVariable,
                            ")",
                            ";"
                        );
                        this.generator.formatter.WriteCodeLine(
                            Keywords.Return,
                            " ",
                            Keywords.ValueVariable,
                            ";"
                        );
                    }
                    else
                    {
                        // The field is not virtual so we apply the regular way of fetching its
                        // value.

                        switch (field.Source)
                        {
                            case FieldSource.Value:
                                this.EmitValueGetter(field, typeName);
                                break;

                            case FieldSource.Expression:
                                this.EmitExpressionGetter(field, typeName, propName);
                                break;
                        }
                    }
                }
                else
                {
                    //	The field is defined by an interface and this is an implementation
                    //	of the getter.

                    string interfaceName = this.generator.CreateTypeFullName(field.DefiningTypeId);
                    string interfaceImplementationName =
                        CodeGenerator.CreateInterfaceImplementationIdentifier(interfaceName);
                    string getterMethodName = string.Concat(
                        Keywords.InterfaceImplementationGetterMethodPrefix,
                        propName
                    );

                    this.generator.formatter.WriteCodeLine(
                        Keywords.Return,
                        " ",
                        interfaceImplementationName,
                        ".",
                        getterMethodName,
                        " (",
                        Keywords.This,
                        ");"
                    );
                }

                this.generator.formatter.WriteEndGetter();
            }

            private void EmitExpression(StructuredTypeField field, string typeName, string propName)
            {
                EntityExpression expression = EntityExpression.FromEncodedExpression(
                    field.Expression
                );

                if (expression.Encoding != EntityExpressionEncoding.LambdaCSharpSourceCode)
                {
                    throw new System.InvalidOperationException(
                        string.Format(
                            "Invalid expression encoding '{0}' for field {1}.{2}",
                            expression.Encoding,
                            this.type.Name,
                            propName
                        )
                    );
                }

                string className = this.generator.CreateEntityFullName(this.type.CaptionId);
                string comment = string.Concat(
                    " ",
                    Keywords.SimpleComment,
                    " λ ",
                    this.type.CaptionId.ToString(),
                    " ",
                    field.Id
                );

                this.generator.formatter.WriteField(
                    CodeHelper.PrivateStaticReadOnlyFieldAttributes,
                    Keywords.Func,
                    "<",
                    className,
                    ", ",
                    typeName,
                    "> ",
                    Keywords.FuncPrefix,
                    propName,
                    " = ",
                    expression.SourceCode,
                    ";",
                    comment
                );

                this.generator.formatter.WriteField(
                    CodeHelper.PrivateStaticReadOnlyFieldAttributes,
                    Keywords.LinqExpression,
                    "<",
                    Keywords.Func,
                    "<",
                    className,
                    ", ",
                    typeName,
                    ">> ",
                    Keywords.ExprPrefix,
                    propName,
                    " = ",
                    expression.SourceCode,
                    ";",
                    comment
                );
            }

            private void EmitValueGetter(StructuredTypeField field, string typeName)
            {
                if (field.Relation == FieldRelation.Collection)
                {
                    if (field.Type is StructuredType)
                    {
                        this.generator.formatter.WriteCodeLine(
                            Keywords.Return,
                            " ",
                            Keywords.This,
                            ".",
                            Keywords.GetFieldCollectionMethod,
                            "<",
                            typeName,
                            "> (",
                            Keywords.Quote,
                            field.Id,
                            Keywords.Quote,
                            ");"
                        );
                    }
                    else
                    {
                        this.generator.formatter.WriteCodeLine(
                            Keywords.Throw,
                            " ",
                            Keywords.New,
                            " ",
                            Keywords.NotSupportedException,
                            @" (""Collection of type ",
                            typeName,
                            @" not supported"");"
                        );
                    }
                }
                else
                {
                    this.generator.formatter.WriteCodeLine(
                        Keywords.Return,
                        " ",
                        Keywords.This,
                        ".",
                        Keywords.GetFieldMethod,
                        "<",
                        typeName,
                        "> (",
                        Keywords.Quote,
                        field.Id,
                        Keywords.Quote,
                        ");"
                    );
                }
            }

            private void EmitExpressionGetter(
                StructuredTypeField field,
                string typeName,
                string propName
            )
            {
                string className = this.generator.CreateEntityFullName(this.type.CaptionId);

                this.generator.formatter.WriteCodeLine(
                    Keywords.Return,
                    " ",
                    Keywords.Global,
                    "::",
                    Keywords.AbstractEntity,
                    ".",
                    Keywords.GetCalculationMethod,
                    "<",
                    className,
                    ", ",
                    typeName,
                    "> (",
                    Keywords.This,
                    ", ",
                    Keywords.Quote,
                    field.Id,
                    Keywords.Quote,
                    ", ",
                    className,
                    ".",
                    Keywords.FuncPrefix,
                    propName,
                    ", ",
                    className,
                    ".",
                    Keywords.ExprPrefix,
                    propName,
                    ");"
                );
            }

            protected override void EmitPropertySetter(
                StructuredTypeField field,
                string typeName,
                string propName
            )
            {
                if (field.Relation == FieldRelation.Collection) { }
                else
                {
                    this.generator.formatter.WriteBeginSetter(CodeHelper.PublicAttributes);

                    if (
                        (field.DefiningTypeId.IsEmpty)
                        || (field.Membership == FieldMembership.LocalOverride)
                    )
                    {
                        //	The field is defined locally and is not the implementation of any
                        //	interface.

                        if (field.Options.HasFlag(FieldOptions.Virtual))
                        {
                            // The field is virtual and we must set its value with a partial method.

                            var methodName =
                                this.generator.CreateMethodNameForVirtualPropertySetter(
                                    field.CaptionId
                                );

                            this.generator.formatter.WriteCodeLine(
                                typeName,
                                " ",
                                Keywords.OldValueVariable,
                                " = ",
                                Keywords.This,
                                ".",
                                propName,
                                ";"
                            );
                            this.generator.formatter.WriteCodeLine(
                                Keywords.If,
                                " (",
                                Keywords.OldValueVariable,
                                " != ",
                                Keywords.ValueVariable,
                                " || ",
                                "!",
                                Keywords.This,
                                ".",
                                Keywords.IsFieldDefinedMethod,
                                "(",
                                Keywords.Quote,
                                field.Id,
                                Keywords.Quote,
                                ")",
                                ")"
                            );
                            this.generator.formatter.WriteBeginBlock();
                            this.generator.formatter.WriteCodeLine(
                                Keywords.This,
                                ".",
                                Keywords.OnPrefix,
                                propName,
                                Keywords.ChangingSuffix,
                                " (",
                                Keywords.OldValueVariable,
                                ", ",
                                Keywords.ValueVariable,
                                ")",
                                ";"
                            );
                            this.generator.formatter.WriteCodeLine(
                                Keywords.This,
                                ".",
                                methodName,
                                " (",
                                Keywords.ValueVariable,
                                ")",
                                ";"
                            );
                            this.generator.formatter.WriteCodeLine(
                                Keywords.This,
                                ".",
                                Keywords.OnPrefix,
                                propName,
                                Keywords.ChangedSuffix,
                                " (",
                                Keywords.OldValueVariable,
                                ", ",
                                Keywords.ValueVariable,
                                ")",
                                ";"
                            );
                            this.generator.formatter.WriteEndBlock();
                        }
                        else
                        {
                            // The field is not virtual thus we must set its value the regular way.

                            switch (field.Source)
                            {
                                case FieldSource.Value:
                                    this.EmitValueSetter(field, typeName, propName);
                                    break;

                                case FieldSource.Expression:
                                    this.EmitExpressionSetter(field, typeName, propName);
                                    break;
                            }
                        }
                    }
                    else
                    {
                        //	The field is defined by an interface and this is an implementation
                        //	of the setter.

                        string interfaceName = this.generator.CreateTypeFullName(
                            field.DefiningTypeId
                        );
                        string interfaceImplementationName =
                            CodeGenerator.CreateInterfaceImplementationIdentifier(interfaceName);
                        string setterMethodName = string.Concat(
                            Keywords.InterfaceImplementationSetterMethodPrefix,
                            propName
                        );

                        this.generator.formatter.WriteCodeLine(
                            interfaceImplementationName,
                            ".",
                            setterMethodName,
                            " (",
                            Keywords.This,
                            ", ",
                            Keywords.ValueVariable,
                            ");"
                        );
                    }

                    this.generator.formatter.WriteEndSetter();
                }
            }

            private void EmitValueSetter(
                StructuredTypeField field,
                string typeName,
                string propName
            )
            {
                this.generator.formatter.WriteCodeLine(
                    typeName,
                    " ",
                    Keywords.OldValueVariable,
                    " = ",
                    Keywords.This,
                    ".",
                    propName,
                    ";"
                );
                this.generator.formatter.WriteCodeLine(
                    Keywords.If,
                    " (",
                    Keywords.OldValueVariable,
                    " != ",
                    Keywords.ValueVariable,
                    " || ",
                    "!",
                    Keywords.This,
                    ".",
                    Keywords.IsFieldDefinedMethod,
                    "(",
                    Keywords.Quote,
                    field.Id,
                    Keywords.Quote,
                    ")",
                    ")"
                );
                this.generator.formatter.WriteBeginBlock();
                this.generator.formatter.WriteCodeLine(
                    Keywords.This,
                    ".",
                    Keywords.OnPrefix,
                    propName,
                    Keywords.ChangingSuffix,
                    " (",
                    Keywords.OldValueVariable,
                    ", ",
                    Keywords.ValueVariable,
                    ");"
                );
                this.generator.formatter.WriteCodeLine(
                    Keywords.This,
                    ".",
                    Keywords.SetFieldMethod,
                    "<",
                    typeName,
                    "> (",
                    Keywords.Quote,
                    field.Id,
                    Keywords.Quote,
                    ", ",
                    Keywords.OldValueVariable,
                    ", ",
                    Keywords.ValueVariable,
                    ");"
                );
                this.generator.formatter.WriteCodeLine(
                    Keywords.This,
                    ".",
                    Keywords.OnPrefix,
                    propName,
                    Keywords.ChangedSuffix,
                    " (",
                    Keywords.OldValueVariable,
                    ", ",
                    Keywords.ValueVariable,
                    ");"
                );
                this.generator.formatter.WriteEndBlock();
            }

            private void EmitExpressionSetter(
                StructuredTypeField field,
                string typeName,
                string propName
            )
            {
                string className = this.generator.CreateEntityFullName(this.type.CaptionId);

                this.generator.formatter.WriteCodeLine(
                    Keywords.Global,
                    "::",
                    Keywords.AbstractEntity,
                    ".",
                    Keywords.SetCalculationMethod,
                    "<",
                    className,
                    ", ",
                    typeName,
                    "> (",
                    Keywords.This,
                    ", ",
                    Keywords.Quote,
                    field.Id,
                    Keywords.Quote,
                    ", ",
                    Keywords.ValueVariable,
                    ");"
                );
            }

            protected override void EmitPropertyOnChanged(
                StructuredTypeField field,
                string typeName,
                string propName
            )
            {
                if (field.Source == FieldSource.Value)
                {
                    string code = string.Concat(
                        Keywords.Void,
                        " ",
                        Keywords.OnPrefix,
                        propName,
                        Keywords.ChangedSuffix,
                        "(",
                        /**/typeName,
                        " ",
                        Keywords.OldValueVariable,
                        ", ",
                        /**/typeName,
                        " ",
                        Keywords.NewValueVariable,
                        ")"
                    );
                    this.generator.formatter.WriteBeginMethod(
                        CodeHelper.PartialMethodAttributes,
                        code
                    );
                    this.generator.formatter.WriteEndMethod();
                }
            }

            protected override void EmitPropertyOnChanging(
                StructuredTypeField field,
                string typeName,
                string propName
            )
            {
                if (field.Source == FieldSource.Value)
                {
                    string code = string.Concat(
                        Keywords.Void,
                        " ",
                        Keywords.OnPrefix,
                        propName,
                        Keywords.ChangingSuffix,
                        "(",
                        /**/typeName,
                        " ",
                        Keywords.OldValueVariable,
                        ", ",
                        /**/typeName,
                        " ",
                        Keywords.NewValueVariable,
                        ")"
                    );
                    this.generator.formatter.WriteBeginMethod(
                        CodeHelper.PartialMethodAttributes,
                        code
                    );
                    this.generator.formatter.WriteEndMethod();
                }
            }

            protected override void EmitMethodForVirtualPropertyGetter(
                StructuredTypeField field,
                string fieldTypeName
            )
            {
                string methodName = this.generator.CreateMethodNameForVirtualPropertyGetter(
                    field.CaptionId
                );

                string code =
                    Keywords.Void
                    + " "
                    + methodName
                    + "("
                    + Keywords.Ref
                    + " "
                    + fieldTypeName
                    + " "
                    + Keywords.ValueVariable
                    + ")";
                this.generator.formatter.WriteBeginMethod(CodeHelper.PartialMethodAttributes, code);
                this.generator.formatter.WriteEndMethod();
            }

            protected override void EmitMethodForVirtualPropertySetter(
                StructuredTypeField field,
                string fieldTypeName
            )
            {
                string methodName = this.generator.CreateMethodNameForVirtualPropertySetter(
                    field.CaptionId
                );

                string code =
                    Keywords.Void
                    + " "
                    + methodName
                    + "("
                    + fieldTypeName
                    + " "
                    + Keywords.ValueVariable
                    + ")";
                this.generator.formatter.WriteBeginMethod(CodeHelper.PartialMethodAttributes, code);
                this.generator.formatter.WriteEndMethod();
            }

            private readonly HashSet<Druid> baseEntityIds;
        }

        #endregion

        #region InterfaceEmitter Class

        private sealed class InterfaceEmitter : Emitter
        {
            public InterfaceEmitter(CodeGenerator generator, StructuredType type)
                : base(generator, type) { }

            protected override bool SuppressField(StructuredTypeField field)
            {
                //	Don't generate fields defined in inherited interfaces.
                Druid interfaceId = field == null ? Druid.Empty : field.DefiningTypeId;
                return interfaceId.IsValid;
            }

            protected override void EmitPropertyGetter(
                StructuredTypeField field,
                string typeName,
                string propName
            )
            {
                this.generator.formatter.WriteBeginGetter(CodeHelper.PublicAttributes);
                this.generator.formatter.WriteEndGetter();
            }

            protected override void EmitPropertySetter(
                StructuredTypeField field,
                string typeName,
                string propName
            )
            {
                if (field.Relation == FieldRelation.Collection) { }
                else
                {
                    this.generator.formatter.WriteBeginSetter(CodeHelper.PublicAttributes);
                    this.generator.formatter.WriteEndSetter();
                }
            }
        }

        #endregion

        #region InterfaceImplementationEmitter Class

        private sealed class InterfaceImplementationEmitter : Emitter
        {
            public InterfaceImplementationEmitter(CodeGenerator generator, StructuredType type)
                : base(generator, type)
            {
                this.className = CodeGenerator.CreateInterfaceImplementationIdentifier(
                    type.Caption.Name
                );
                this.interfaceName = this.generator.CreateEntityFullName(type.CaptionId);
            }

            protected override bool SuppressField(StructuredTypeField field)
            {
                //	Don't generate fields defined in inherited interfaces.
                Druid interfaceId = field == null ? Druid.Empty : field.DefiningTypeId;
                return interfaceId.IsValid;
            }

            private void EmitExpression(StructuredTypeField field, string typeName, string propName)
            {
                EntityExpression expression = EntityExpression.FromEncodedExpression(
                    field.Expression
                );

                if (expression.Encoding != EntityExpressionEncoding.LambdaCSharpSourceCode)
                {
                    throw new System.InvalidOperationException(
                        string.Format(
                            "Invalid expression encoding '{0}' for field {1}.{2}",
                            expression.Encoding,
                            this.type.Name,
                            propName
                        )
                    );
                }

                string className = this.generator.CreateEntityFullName(this.type.CaptionId);
                string comment = string.Concat(
                    " ",
                    Keywords.SimpleComment,
                    " λ ",
                    this.type.CaptionId.ToString(),
                    " ",
                    field.Id
                );

                this.generator.formatter.WriteField(
                    CodeHelper.InternalStaticReadOnlyFieldAttributes,
                    Keywords.Func,
                    "<",
                    className,
                    ", ",
                    typeName,
                    "> ",
                    Keywords.FuncPrefix,
                    propName,
                    " = ",
                    expression.SourceCode,
                    ";",
                    comment
                );

                this.generator.formatter.WriteField(
                    CodeHelper.InternalStaticReadOnlyFieldAttributes,
                    Keywords.LinqExpression,
                    "<",
                    Keywords.Func,
                    "<",
                    className,
                    ", ",
                    typeName,
                    ">> ",
                    Keywords.ExprPrefix,
                    propName,
                    " = ",
                    expression.SourceCode,
                    ";",
                    comment
                );
            }

            private void EmitValueGetter(
                string entityType,
                StructuredTypeField field,
                string typeName
            )
            {
                this.generator.formatter.WriteCodeLine(
                    entityType,
                    " ",
                    Keywords.EntityVariable,
                    " = ",
                    Keywords.ObjVariable,
                    " ",
                    Keywords.As,
                    " ",
                    entityType,
                    ";"
                );

                if (field.Relation == FieldRelation.Collection)
                {
                    this.generator.formatter.WriteCodeLine(
                        Keywords.Return,
                        " ",
                        Keywords.EntityVariable,
                        ".",
                        Keywords.GetFieldCollectionMethod,
                        "<",
                        typeName,
                        "> (",
                        Keywords.Quote,
                        field.Id,
                        Keywords.Quote,
                        ");"
                    );
                }
                else
                {
                    this.generator.formatter.WriteCodeLine(
                        Keywords.Return,
                        " ",
                        Keywords.EntityVariable,
                        ".",
                        Keywords.GetFieldMethod,
                        "<",
                        typeName,
                        "> (",
                        Keywords.Quote,
                        field.Id,
                        Keywords.Quote,
                        ");"
                    );
                }
            }

            private void EmitExpressionGetter(
                StructuredTypeField field,
                string typeName,
                string propName
            )
            {
                this.generator.formatter.WriteCodeLine(
                    Keywords.Return,
                    " ",
                    Keywords.Global,
                    "::",
                    Keywords.AbstractEntity,
                    ".",
                    Keywords.GetCalculationMethod,
                    "<",
                    this.interfaceName,
                    ", ",
                    typeName,
                    "> (",
                    Keywords.ObjVariable,
                    ", ",
                    Keywords.Quote,
                    field.Id,
                    Keywords.Quote,
                    ", ",
                    this.className,
                    ".",
                    Keywords.FuncPrefix,
                    propName,
                    ", ",
                    this.className,
                    ".",
                    Keywords.ExprPrefix,
                    propName,
                    ");"
                );
            }

            private void EmitExpressionSetter(
                StructuredTypeField field,
                string typeName,
                string propName
            )
            {
                this.generator.formatter.WriteCodeLine(
                    Keywords.Global,
                    "::",
                    Keywords.AbstractEntity,
                    ".",
                    Keywords.SetCalculationMethod,
                    "<",
                    this.interfaceName,
                    ", ",
                    typeName,
                    "> (",
                    Keywords.ObjVariable,
                    ", ",
                    Keywords.Quote,
                    field.Id,
                    Keywords.Quote,
                    ", ",
                    Keywords.ValueVariable,
                    ");"
                );
            }

            public void EmitLocalPropertyImplementation(StructuredTypeField field)
            {
                if (field.Membership == FieldMembership.Local)
                {
                    if (this.SuppressField(field))
                    {
                        return;
                    }

                    string typeName = this.generator.CreateTypeFullName(
                        field.TypeId,
                        TypeRosetta.IsNullable(field)
                    );
                    string propName = this.generator.CreatePropertyName(field.CaptionId);

                    string getterMethodName = string.Concat(
                        Keywords.InterfaceImplementationGetterMethodPrefix,
                        propName
                    );
                    string getterReturnType = typeName;

                    if (field.Relation == FieldRelation.Collection)
                    {
                        getterReturnType = string.Concat(
                            Keywords.Global,
                            "::",
                            Keywords.GenericIList,
                            "<",
                            typeName,
                            ">"
                        );
                    }

                    string getterCode = string.Concat(
                        getterReturnType,
                        " ",
                        getterMethodName,
                        "(",
                        this.interfaceName,
                        " ",
                        Keywords.ObjVariable,
                        ")"
                    );
                    string entityType = string.Concat(
                        Keywords.Global,
                        "::",
                        Keywords.AbstractEntity
                    );

                    this.generator.formatter.WriteBeginMethod(
                        CodeHelper.PublicStaticMethodAttributes,
                        getterCode
                    );

                    switch (field.Source)
                    {
                        case FieldSource.Value:
                            this.EmitValueGetter(entityType, field, typeName);
                            break;

                        case FieldSource.Expression:
                            this.EmitExpressionGetter(field, typeName, propName);
                            break;
                    }

                    this.generator.formatter.WriteEndMethod();

                    if (field.Relation == FieldRelation.Collection)
                    {
                        //	Don't generate a setter method.
                    }
                    else
                    {
                        string setterMethodName = string.Concat(
                            Keywords.InterfaceImplementationSetterMethodPrefix,
                            propName
                        );
                        string setterCode = string.Concat(
                            Keywords.Void,
                            " ",
                            setterMethodName,
                            "(",
                            this.interfaceName,
                            " ",
                            Keywords.ObjVariable,
                            ", ",
                            typeName,
                            " ",
                            Keywords.ValueVariable,
                            ")"
                        );

                        this.generator.formatter.WriteBeginMethod(
                            CodeHelper.PublicStaticMethodAttributes,
                            setterCode
                        );

                        if (field.Source == FieldSource.Value)
                        {
                            this.generator.formatter.WriteCodeLine(
                                entityType,
                                " ",
                                Keywords.EntityVariable,
                                " = ",
                                Keywords.ObjVariable,
                                " ",
                                Keywords.As,
                                " ",
                                entityType,
                                ";"
                            );
                            this.generator.formatter.WriteCodeLine(
                                typeName,
                                " ",
                                Keywords.OldValueVariable,
                                " = ",
                                Keywords.ObjVariable,
                                ".",
                                propName,
                                ";"
                            );
                            this.generator.formatter.WriteCodeLine(
                                Keywords.If,
                                " (",
                                Keywords.OldValueVariable,
                                " != ",
                                Keywords.ValueVariable,
                                " || ",
                                "!",
                                Keywords.EntityVariable,
                                ".",
                                Keywords.IsFieldDefinedMethod,
                                "(",
                                Keywords.Quote,
                                field.Id,
                                Keywords.Quote,
                                ")",
                                ")"
                            );
                            this.generator.formatter.WriteBeginBlock();
                            this.generator.formatter.WriteCodeLine(
                                this.className,
                                ".",
                                Keywords.OnPrefix,
                                propName,
                                Keywords.ChangingSuffix,
                                " (",
                                Keywords.ObjVariable,
                                ", ",
                                Keywords.OldValueVariable,
                                ", ",
                                Keywords.ValueVariable,
                                ");"
                            );
                            this.generator.formatter.WriteCodeLine(
                                Keywords.EntityVariable,
                                ".",
                                Keywords.SetFieldMethod,
                                "<",
                                typeName,
                                "> (",
                                Keywords.Quote,
                                field.Id,
                                Keywords.Quote,
                                ", ",
                                Keywords.OldValueVariable,
                                ", ",
                                Keywords.ValueVariable,
                                ");"
                            );
                            this.generator.formatter.WriteCodeLine(
                                this.className,
                                ".",
                                Keywords.OnPrefix,
                                propName,
                                Keywords.ChangedSuffix,
                                " (",
                                Keywords.ObjVariable,
                                ", ",
                                Keywords.OldValueVariable,
                                ", ",
                                Keywords.ValueVariable,
                                ");"
                            );
                            this.generator.formatter.WriteEndBlock();
                        }
                        else
                        {
                            this.EmitExpressionSetter(field, typeName, propName);
                        }

                        this.generator.formatter.WriteEndMethod();

                        if (field.Source == FieldSource.Value)
                        {
                            string code;

                            code = string.Concat(
                                Keywords.Void,
                                " ",
                                Keywords.OnPrefix,
                                propName,
                                Keywords.ChangedSuffix,
                                "(",
                                /**/this.interfaceName,
                                " ",
                                Keywords.ObjVariable,
                                ", ",
                                /**/typeName,
                                " ",
                                Keywords.OldValueVariable,
                                ", ",
                                /**/typeName,
                                " ",
                                Keywords.NewValueVariable,
                                ")"
                            );
                            this.generator.formatter.WriteBeginMethod(
                                CodeHelper.PartialStaticMethodAttributes,
                                code
                            );
                            this.generator.formatter.WriteEndMethod();

                            code = string.Concat(
                                Keywords.Void,
                                " ",
                                Keywords.OnPrefix,
                                propName,
                                Keywords.ChangingSuffix,
                                "(",
                                /**/this.interfaceName,
                                " ",
                                Keywords.ObjVariable,
                                ", ",
                                /**/typeName,
                                " ",
                                Keywords.OldValueVariable,
                                ", ",
                                /**/typeName,
                                " ",
                                Keywords.NewValueVariable,
                                ")"
                            );
                            this.generator.formatter.WriteBeginMethod(
                                CodeHelper.PartialStaticMethodAttributes,
                                code
                            );
                            this.generator.formatter.WriteEndMethod();
                        }
                    }

                    switch (field.Source)
                    {
                        case FieldSource.Value:
                            break;

                        case FieldSource.Expression:
                            this.EmitExpression(field, typeName, propName);
                            break;

                        default:
                            throw new System.ArgumentException(
                                string.Format(
                                    "FieldSource.{0} not valid in this context",
                                    field.Source
                                )
                            );
                    }
                }
            }

            private string className;
            private string interfaceName;
        }

        #endregion

        private CodeFormatter formatter;
        private ResourceManager resourceManager;
        private ResourceManagerPool resourceManagerPool;
        private ResourceModuleInfo resourceModuleInfo;
        private string sourceNamespaceRes;
        private string sourceNamespaceEntities;
    }
}
