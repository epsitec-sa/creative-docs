//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	public class Tags : Epsitec.Common.Support.Tags
	{
		//	Noms des types "Cr�sus" fondamentaux :
		
		public const string	TypeKeyId			= "CR_KeyIdType";
		public const string	TypeKeyStatus		= "CR_KeyStatusType";
		public const string	TypeName			= "CR_NameType";
		public const string	TypeCaption			= "CR_CaptionType";
		public const string	TypeDescription		= "CR_DescriptionType";
		public const string	TypeInfoXml			= "CR_InfoXmlType";
		public const string	TypeDateTime		= "CR_DateTimeType";
		public const string TypeRawData			= "CR_RawDataType";
		public const string TypeDictKey			= "CR_DictKeyType";
		public const string TypeDictValue		= "CR_DictKeyValue";
		
		//	Noms des colonnes "Cr�sus" fondamentales :
		
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
		
		public const string ColumnQueueData		= "CR_QUEUE_DATA";
		
		public const string	ColumnRefTable		= "CREF_TABLE";
		public const string	ColumnRefType		= "CREF_TYPE";
		public const string	ColumnRefColumn		= "CREF_COLUMN";
		public const string	ColumnRefParent		= "CREF_PARENT_TABLE";
		public const string ColumnRefLog		= "CREF_LOG";
		
		//	Noms des tables "Cr�sus" fondamentales :
		
		public const string	TableTableDef		= "CR_TABLE_DEF";
		public const string	TableColumnDef		= "CR_COLUMN_DEF";
		public const string	TableTypeDef		= "CR_TYPE_DEF";
		public const string	TableEnumValDef		= "CR_ENUMVAL_DEF";
		public const string TableLog			= "CR_LOG";
		public const string TableRequestQueue	= "CR_RQ_QUEUE";
	}
}
