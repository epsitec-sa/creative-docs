//	Copyright © 2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Support.CodeGeneration
{
	public static class CodeHelper
	{
		public static void EmitHeader(CodeFormatter formatter)
		{
			string   html = Epsitec.Common.Support.Res.Strings.CodeGenerator.SourceFileHeader;
			string   text = Epsitec.Common.Types.Converters.TextConverter.ConvertToSimpleText (html);

			string[] lines = text.Split (new string[] { "<br/>", "\r\n", "\r", "\n" }, System.StringSplitOptions.None);

			foreach (string line in lines)
			{
				formatter.WriteCodeLine (line);
			}
		}
		
		#region CodeAttributes Constants

		public static readonly CodeAttributes PublicAttributes = new CodeAttributes (CodeVisibility.Public);
		public static readonly CodeAttributes StaticMethodAttributes = new CodeAttributes (CodeVisibility.Public, CodeAccessibility.Static);
		public static readonly CodeAttributes OverrideMethodAttributes = new CodeAttributes (CodeVisibility.Public, CodeAccessibility.Override);
		public static readonly CodeAttributes EntityClassAttributes = new CodeAttributes (CodeVisibility.Public, CodeAccessibility.Default, CodeAttributes.PartialAttribute);
		public static readonly CodeAttributes StaticClassAttributes = new CodeAttributes (CodeVisibility.Public, CodeAccessibility.Static);
		public static readonly CodeAttributes StaticPartialClassAttributes = new CodeAttributes (CodeVisibility.Public, CodeAccessibility.Static, CodeAttributes.PartialAttribute);
		public static readonly CodeAttributes InterfaceAttributes = new CodeAttributes (CodeVisibility.Public, CodeAccessibility.Default);
		public static readonly CodeAttributes PropertyAttributes = new CodeAttributes (CodeVisibility.Public);
		public static readonly CodeAttributes PartialMethodAttributes = new CodeAttributes (CodeVisibility.None, CodeAccessibility.Default, CodeAttributes.PartialDefinitionAttribute);
		public static readonly CodeAttributes StaticPartialMethodAttributes = new CodeAttributes (CodeVisibility.None, CodeAccessibility.Static, CodeAttributes.PartialDefinitionAttribute);
		public static readonly CodeAttributes StaticReadOnlyFieldAttributes = new CodeAttributes (CodeVisibility.Public, CodeAccessibility.Static, CodeAttributes.ReadOnlyAttribute);
		public static readonly CodeAttributes StaticInternalReadOnlyFieldAttributes = new CodeAttributes (CodeVisibility.Internal, CodeAccessibility.Static, CodeAttributes.ReadOnlyAttribute);

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

			public const string Void   = "void";
			public const string String = "string";
			public const string Druid  = "global::Epsitec.Common.Support.Druid";
			public const string Func   = "global::System.Func";
			public const string LinqExpression = "global::System.Linq.Expressions.Expression";
			public const string NotSupportedException = "global::System.NotSupportedException";

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

			public const string SetFieldMethod = "SetField";
			public const string GetFieldMethod = "GetField";
			public const string GetCalculationMethod = "GetCalculation";
			public const string SetCalculationMethod = "SetCalculation";
			public const string GetFieldCollectionMethod = "GetFieldCollection";
			public const string GetStructuredTypeIdMethod = "GetEntityStructuredTypeId";
			public const string OnPrefix = "On";
			public const string ChangedSuffix = "Changed";
			public const string ChangingSuffix = "Changing";
		}

		#endregion
	}
}
