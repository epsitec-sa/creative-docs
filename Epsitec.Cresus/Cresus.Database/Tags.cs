//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	public class Tags : Epsitec.Common.Support.Tags
	{
		//	Noms des types "Crésus" fondamentaux :
		
		public const string	TypeKeyId			= "CR_KeyIdType";
//		public const string	TypeKeyRevision		= "CR_KeyRevisionType";
		public const string	TypeKeyStatus		= "CR_KeyStatusType";
		public const string	TypeName			= "CR_NameType";
		public const string	TypeCaption			= "CR_CaptionType";
		public const string	TypeDescription		= "CR_DescriptionType";
		public const string	TypeInfoXml			= "CR_InfoXmlType";
		
		//	Noms des colonnes "Crésus" fondamentales :
		
		public const string	ColumnId			= "CR_ID";
//		public const string	ColumnRevision		= "CR_REV";
		public const string	ColumnStatus		= "CR_STAT";
		public const string	ColumnName			= "CR_NAME";
		public const string	ColumnCaption		= "CR_CAPTION";
		public const string	ColumnDescription	= "CR_DESCRIPTION";
		public const string	ColumnInfoXml		= "CR_INFO";
		public const string	ColumnNextId		= "CR_NEXT_ID";
		
		public const string	ColumnRefTable		= "CREF_TABLE";
		public const string	ColumnRefType		= "CREF_TYPE";
		public const string	ColumnRefColumn		= "CREF_COLUMN";
		public const string	ColumnRefParent		= "CREF_PARENT_TABLE";
		
		//	Noms des tables "Crésus" fondamentales :
		
		public const string	TableTableDef		= "CR_TABLE_DEF";
		public const string	TableColumnDef		= "CR_COLUMN_DEF";
		public const string	TableTypeDef		= "CR_TYPE_DEF";
		public const string	TableEnumValDef		= "CR_ENUMVAL_DEF";
	}
}
