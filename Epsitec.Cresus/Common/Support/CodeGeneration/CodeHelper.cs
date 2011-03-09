//	Copyright © 2008-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Support.CodeGeneration
{
	/// <summary>
	/// The <c>CodeHelper</c> class provides useful methods and constants which
	/// ease the implementation of a code generator (see <see cref="Epsitec.Common.Support.EntityEngine.CodeGenerator"/>
	/// for an example).
	/// </summary>
	public static class CodeHelper
	{
		/// <summary>
		/// Emits the header of a C# file, which contains a "do not edit" warning.
		/// </summary>
		/// <param name="formatter">The code formatter.</param>
		public static void EmitHeader(CodeFormatter formatter)
		{
			string text = Epsitec.Common.Support.Res.Strings.CodeGenerator.SourceFileHeader.ToSimpleText ();

			string[] lines = text.Split (new string[] { "<br/>", "\r\n", "\r", "\n" }, System.StringSplitOptions.None);

			foreach (string line in lines)
			{
				formatter.WriteCodeLine (line);
			}
		}
		
		#region CodeAttributes Constants

		public static readonly CodeAttributes PublicAttributes = new CodeAttributes (CodeVisibility.Public);

		public static readonly CodeAttributes PublicStaticMethodAttributes = new CodeAttributes (CodeVisibility.Public, CodeAccessibility.Static);
		public static readonly CodeAttributes PrivateStaticMethodAttributes = new CodeAttributes (CodeVisibility.Private, CodeAccessibility.Static);
		public static readonly CodeAttributes PublicOverrideMethodAttributes = new CodeAttributes (CodeVisibility.Public, CodeAccessibility.Override);
		public static readonly CodeAttributes PublicStaticPartialClassAttributes = new CodeAttributes (CodeVisibility.Public, CodeAccessibility.Static, CodeAttributes.PartialAttribute);
		public static readonly CodeAttributes PublicStaticReadOnlyFieldAttributes = new CodeAttributes (CodeVisibility.Public, CodeAccessibility.Static, CodeAttributes.ReadOnlyAttribute);
		public static readonly CodeAttributes PublicConstFieldAttributes = new CodeAttributes (CodeVisibility.Public, CodeAttributes.ConstAttribute);
		public static readonly CodeAttributes PublicStaticNewReadOnlyFieldAttributes = new CodeAttributes (CodeVisibility.Public, CodeAccessibility.Static, CodeAttributes.ReadOnlyAttribute, CodeAttributes.NewAttribute);
		public static readonly CodeAttributes PublicInterfaceAttributes = new CodeAttributes (CodeVisibility.Public, CodeAccessibility.Default);
		public static readonly CodeAttributes PublicStaticPropertyAttributes = new CodeAttributes (CodeVisibility.Public, CodeAccessibility.Static);
		public static readonly CodeAttributes PublicPropertyAttributes = new CodeAttributes (CodeVisibility.Public);
		public static readonly CodeAttributes PublicVirtualPropertyAttributes = new CodeAttributes (CodeVisibility.Public, CodeAccessibility.Virtual);
		public static readonly CodeAttributes PublicOverridePropertyAttributes = new CodeAttributes (CodeVisibility.Public, CodeAccessibility.Override);

		public static readonly CodeAttributes PublicStaticClassAttributes = new CodeAttributes (CodeVisibility.Public, CodeAccessibility.Static);
		public static readonly CodeAttributes EntityClassAttributes = new CodeAttributes (CodeVisibility.Public, CodeAccessibility.Default, CodeAttributes.PartialAttribute);
		public static readonly CodeAttributes RepositoryClassAttributes = new CodeAttributes (CodeVisibility.Public, CodeAccessibility.Default, CodeAttributes.PartialAttribute);
		public static readonly CodeAttributes FormIdsClassAttributes = new CodeAttributes (CodeVisibility.Public, CodeAccessibility.Static);

		public static readonly CodeAttributes PrivateStaticReadOnlyFieldAttributes = new CodeAttributes (CodeVisibility.Private, CodeAccessibility.Static, CodeAttributes.ReadOnlyAttribute);
		public static readonly CodeAttributes PrivateConstFieldAttributes = new CodeAttributes (CodeVisibility.Private, CodeAttributes.ConstAttribute);
		public static readonly CodeAttributes InternalStaticReadOnlyFieldAttributes = new CodeAttributes (CodeVisibility.Internal, CodeAccessibility.Static, CodeAttributes.ReadOnlyAttribute);
		public static readonly CodeAttributes InternalStaticMethodAttributes = new CodeAttributes (CodeVisibility.Internal, CodeAccessibility.Static);

		public static readonly CodeAttributes PartialMethodAttributes = new CodeAttributes (CodeVisibility.None, CodeAccessibility.Default, CodeAttributes.PartialDefinitionAttribute);
		public static readonly CodeAttributes PartialStaticMethodAttributes = new CodeAttributes (CodeVisibility.None, CodeAccessibility.Static, CodeAttributes.PartialDefinitionAttribute);

		public static readonly CodeAttributes StaticClassConstructorAttributes = new CodeAttributes (CodeVisibility.None, CodeAccessibility.Static);

		#endregion

		#region Keywords Class

		public static class Keywords
		{
			public const string Global = "global";
			public const string Return = "return";
			public const string New    = "new";
			public const string This   = "this";
			public const string Throw  = "throw";
			public const string If     = "if";
			public const string As     = "as";
			public const string Quote  = "\"";
			public const string Override = "override";
			public const string Base   = "base";

			public const string Void   = "void";
			public const string String = "string";
			public const string Druid  = "global::Epsitec.Common.Support.Druid";
			public const string Func   = "global::System.Func";
			public const string LinqExpression = "global::System.Linq.Expressions.Expression";
			public const string NotSupportedException = "global::System.NotSupportedException";
			public const string QualifiedGenericRepositoryBase = "global::Epsitec.Cresus.Core.Repositories.Repository";
			public const string QualifiedCoreData = "global::Epsitec.Cresus.Core.CoreData";
			public const string QualifiedDataContext = "global::Epsitec.Cresus.DataLayer.Context.DataContext";
			public const string QualifiedDataLifetime = "global::Epsitec.Common.Types.DataLifetimeExpectancy";

			public const string EntityFieldAttribute = "global::Epsitec.Common.Support.EntityField";
			public const string EntityClassAttribute = "global::Epsitec.Common.Support.EntityClass";

			public const string SimpleComment = "//";
			public const string XmlComment = "///";
			public const string XmlBeginSummary = "<summary>";
			public const string XmlEndSummary = "</summary>";


			public const string DesignerCaptionProtocol = "designer:cap/";
			public const string DesignerEntityFieldProtocol = "designer:fld/";

			public const string BeginRegion = "#region";
			public const string EndRegion = "#endregion";

			public const string ValueVariable = "value";
			public const string OldValueVariable = "oldValue";
			public const string NewValueVariable = "newValue";
			public const string FieldNameVariable = "fieldName";
			public const string ObjVariable = "obj";
			public const string EntityVariable = "entity";
			public const string DataVariable = "data";
			public const string DataContextVariable = "dataContext";

			public const string GenericIList = "System.Collections.Generic.IList";
			public const string Entities = "Entities";
			public const string Forms = "Forms";
			public const string EntitySuffix = "Entity";
			public const string ExprPrefix = "Expr";
			public const string FuncPrefix = "Func";
			public const string InterfaceImplementationSuffix = "InterfaceImplementation";
			public const string InterfaceImplementationGetterMethodPrefix = "Get";
			public const string InterfaceImplementationSetterMethodPrefix = "Set";
			public const string AbstractEntity = "Epsitec.Common.Support.EntityEngine.AbstractEntity";
			public const string Repository = "Repository";

			public const string IsFieldDefinedMethod = "IsFieldDefined";
			public const string SetFieldMethod = "SetField";
			public const string GetFieldMethod = "GetField";
			public const string GetCalculationMethod = "GetCalculation";
			public const string SetCalculationMethod = "SetCalculation";
			public const string GetFieldCollectionMethod = "GetFieldCollection";
			public const string GetEntityStructuredTypeIdMethod = "GetEntityStructuredTypeId";
			public const string GetEntityStructuredTypeKeyMethod = "GetEntityStructuredTypeKey";
			public const string EntityStructuredTypeIdProperty = "EntityStructuredTypeId";
			public const string EntityStructuredTypeKeyProperty = "EntityStructuredTypeKey";
			public const string OnPrefix = "On";
			public const string ChangedSuffix = "Changed";
			public const string ChangingSuffix = "Changing";
		}

		#endregion
	}
}
