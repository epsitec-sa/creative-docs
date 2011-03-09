//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>Tags</c> class defines all well known tags (texts) used to name
	/// columns, tables, types, etc.
	/// </summary>
	public static class Tags
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
		public const string ColumnDateTime				= "CR_DATETIME";
		public const string ColumnInstanceType			= "CR_TYPE";
		public const string ColumnConnectionId			= "CR_CONNECTION_ID";
		public const string ColumnUidSlot				= "CR_UID_SLOT";
		public const string ColumnUidMin				= "CR_UID_MIN";
		public const string ColumnUidMax				= "CR_UID_MAX";
		public const string ColumnUidNext				= "CR_UID_NEXT";
		public const string ColumnCounter				= "CR_COUNTER";
		public const string ColumnConnectionIdentity	= "CR_IDENTITY";
		public const string ColumnEstablishmentTime		= "CR_ESTB_TIME";
		public const string ColumnRefreshTime			= "CR_RFR_TIME";
		public const string ColumnConnectionStatus		= "CR_CONN_STATUS";
		public const string ColumnKey					= "CR_KEY";
		public const string ColumnValue					= "CR_VALUE";
		
		public const string	ColumnRefTable				= "CREF_TABLE";
		public const string	ColumnRefType				= "CREF_TYPE";
		public const string	ColumnRefTarget				= "CREF_TARGET_TABLE";
		public const string ColumnRefLog				= "CREF_LOG";
		public const string ColumnRefId					= "CREF_ID";
		public const string ColumnRefSourceId			= "CREF_SOURCE_ID";
		public const string ColumnRefTargetId			= "CREF_TARGET_ID";
		public const string ColumnRefModel				= "CREF_MODEL";
		public const string ColumnRefRank				= "CREF_RANK";
		
		//	Basic CRESUS table names :

		public const string	TableTableDef				= "CR_TABLE_DEF";
		public const string	TableColumnDef				= "CR_COLUMN_DEF";
		public const string	TableTypeDef				= "CR_TYPE_DEF";
		public const string TableInfo					= "CR_INFO";
		public const string TableLog					= "CR_LOG";
		public const string TableEntityDeletionLog		= "CR_EDL";
		public const string TableUid					= "CR_UID";
		public const string TableLock					= "CR_LOCK";
		public const string TableConnection				= "CR_CONNECTION";
	}
}
