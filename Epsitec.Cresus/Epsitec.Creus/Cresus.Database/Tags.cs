//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>Tags</c> class defines all well known tags (texts) used to name
	/// columns, tables, types, etc.
	/// </summary>
	internal static class Tags
	{
		//	Basic CRESUS type names :

		public const string TypeKeyId					= "K004"; //"Num.KeyId";
		public const string TypeNullableKeyId			= "K007"; //"Num.NullableKeyId";
		public const string TypeKeyStatus				= "K005"; //"Num.KeyStatus";
		public const string TypeReqExState				= "K006"; //"Num.ReqExecState";
		public const string TypeName					= "K008"; //"Str.Name";
		public const string	TypeInfoXml					= "K009"; //"Str.InfoXml";
		public const string TypeDictKey					= "K00A"; //"Str.Dict.Key";
		public const string TypeDictValue				= "K00B"; //"Str.Dict.Value";
		public const string TypeReqData					= "K00C"; //"Other.ReqData";
		public const string TypeDateTime				= "K00D"; //"Other.DateTime";
		public const string TypeCollectionRank			= "K01";  //"Num.CollectionRank";
		
		//	Basic CRESUS column names :
		
		public const string	ColumnId					= "CR_ID";
		public const string ColumnName					= "CR_NAME";
		public const string ColumnDisplayName			= "CR_DISPLAY_NAME";
		public const string ColumnInfoXml				= "CR_INFO";
		
		public const string	ColumnRefTable				= "CREF_TABLE";
		public const string	ColumnRefType				= "CREF_TYPE";
		public const string	ColumnRefTarget				= "CREF_TARGET_TABLE";
		public const string ColumnRefSourceId			= "CREF_SOURCE_ID";
		public const string ColumnRefTargetId			= "CREF_TARGET_ID";
		public const string ColumnRefRank				= "CREF_RANK";
		
		//	Basic CRESUS table names :

		public const string	TableTableDef				= "CR_TABLE_DEF";
		public const string	TableColumnDef				= "CR_COLUMN_DEF";
		public const string	TableTypeDef				= "CR_TYPE_DEF";
	}
}
