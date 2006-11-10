//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	public class Tags
	{
		public const string Id = "id";
		public const string Parent = "prnt";
		public const string Caption = "capt";
		public const string Description = "desc";
		public const string Name = "name";
		
		//	Noms des types "Crésus" fondamentaux :
		
		public const string	TypeKeyId			= "CR_KeyIdType";
		public const string	TypeKeyStatus		= "CR_KeyStatusType";
		public const string	TypeName			= "CR_NameType";
		public const string	TypeCaption			= "CR_CaptionType";
		public const string	TypeDescription		= "CR_DescriptionType";
		public const string	TypeInfoXml			= "CR_InfoXmlType";
		public const string	TypeDateTime		= "CR_DateTimeType";
		public const string TypeDictKey			= "CR_DictKeyType";
		public const string TypeDictValue		= "CR_DictValueType";
		public const string TypeReqData			= "CR_RqDataType";
		public const string TypeReqExState		= "CR_RqExStateType";
		
		//	Noms des colonnes "Crésus" fondamentales :
		
		public const string	ColumnId			= "CR_ID";
		public const string	ColumnStatus		= "CR_STAT";
		public const string	ColumnName			= "CR_NAME";
		public const string	ColumnCaption		= "CR_CAPTION";
		public const string	ColumnDescription	= "CR_DESCRIPTION";
		public const string	ColumnInfoXml		= "CR_INFO";
		public const string	ColumnNextId		= "CR_NEXT_ID";
		public const string ColumnDateTime		= "CR_DATETIME";
		
		public const string ColumnDictKey		= "CR_DICT_KEY";
		public const string ColumnDictValue		= "CR_DICT_VALUE";
		
		public const string ColumnReqData		= "CR_RQ_DATA";
		public const string ColumnReqExState	= "CR_RQ_EX_STATE";
		
		public const string	ColumnRefTable		= "CREF_TABLE";
		public const string	ColumnRefType		= "CREF_TYPE";
		public const string	ColumnRefColumn		= "CREF_COLUMN";
		public const string	ColumnRefParent		= "CREF_PARENT_TABLE";
		public const string ColumnRefLog		= "CREF_LOG";
		
		public const string ColumnClientName	= "CR_CLIENT_NAME";
		public const string ColumnClientId		= "CR_CLIENT_ID";
		public const string ColumnClientSync	= "CR_CLIENT_SYNC";
		public const string ColumnClientCreDate	= "CR_CLIENT_CRE_DATE";
		public const string ColumnClientConDate	= "CR_CLIENT_CON_DATE";
		
		//	Noms des tables "Crésus" fondamentales :
		
		public const string	TableTableDef		= "CR_TABLE_DEF";
		public const string	TableColumnDef		= "CR_COLUMN_DEF";
		public const string	TableTypeDef		= "CR_TYPE_DEF";
		public const string	TableEnumValDef		= "CR_ENUMVAL_DEF";
		public const string TableLog			= "CR_LOG";
		public const string TableRequestQueue	= "CR_RQ_QUEUE";
		public const string TableClientDef      = "CR_CLIENT_DEF";
	}
}
