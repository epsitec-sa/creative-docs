//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

[assembly: global::Epsitec.Common.Support.EntityClass ("[700B1]", typeof (Epsitec.Common.Support.Entities.ResourceBaseEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[7007]", typeof (Epsitec.Common.Support.Entities.ResourceStringEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[7006]", typeof (Epsitec.Common.Support.Entities.ResourceCaptionEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[700G]", typeof (Epsitec.Common.Support.Entities.ResourceCommandEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[7005]", typeof (Epsitec.Common.Support.Entities.ResourceBaseTypeEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[700H]", typeof (Epsitec.Common.Support.Entities.ResourceStructuredTypeEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[700Q]", typeof (Epsitec.Common.Support.Entities.ShortcutEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[70041]", typeof (Epsitec.Common.Support.Entities.FieldEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[700F1]", typeof (Epsitec.Common.Support.Entities.ResourceNumericTypeEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[700G1]", typeof (Epsitec.Common.Support.Entities.ResourceDateTimeTypeEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[700H1]", typeof (Epsitec.Common.Support.Entities.ResourceStringTypeEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[70002]", typeof (Epsitec.Common.Support.Entities.ResourceBinaryTypeEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[70032]", typeof (Epsitec.Common.Support.Entities.ResourceCollectionTypeEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[70042]", typeof (Epsitec.Common.Support.Entities.ResourceOtherTypeEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[70052]", typeof (Epsitec.Common.Support.Entities.ResourceEnumTypeEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[70062]", typeof (Epsitec.Common.Support.Entities.EnumValueEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[7013]", typeof (Epsitec.Common.Support.Entities.TestInterfaceUserEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[7019]", typeof (Epsitec.Common.Support.Entities.InterfaceIdEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[701P]", typeof (Epsitec.Common.Support.Entities.ResourceBaseFileEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[701R]", typeof (Epsitec.Common.Support.Entities.ResourcePanelEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[701U]", typeof (Epsitec.Common.Support.Entities.ResourceFormEntity))]
#region Epsitec.Common.Support.ResourceBase Entity
namespace Epsitec.Common.Support.Entities
{
	///	<summary>
	///	The <c>ResourceBase</c> entity.
	///	designer:cap/700B1
	///	</summary>
	public partial class ResourceBaseEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>ModificationId</c> field.
		///	designer:fld/700B1/70091
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[70091]")]
		public int ModificationId
		{
			get
			{
				return this.GetField<int> ("[70091]");
			}
			set
			{
				int oldValue = this.ModificationId;
				if (oldValue != value || !this.IsFieldDefined("[70091]"))
				{
					this.OnModificationIdChanging (oldValue, value);
					this.SetField<int> ("[70091]", oldValue, value);
					this.OnModificationIdChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Comment</c> field.
		///	designer:fld/700B1/700A
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[700A]")]
		public string Comment
		{
			get
			{
				return this.GetField<string> ("[700A]");
			}
			set
			{
				string oldValue = this.Comment;
				if (oldValue != value || !this.IsFieldDefined("[700A]"))
				{
					this.OnCommentChanging (oldValue, value);
					this.SetField<string> ("[700A]", oldValue, value);
					this.OnCommentChanged (oldValue, value);
				}
			}
		}
		
		partial void OnModificationIdChanging(int oldValue, int newValue);
		partial void OnModificationIdChanged(int oldValue, int newValue);
		partial void OnCommentChanging(string oldValue, string newValue);
		partial void OnCommentChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Common.Support.Entities.ResourceBaseEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Common.Support.Entities.ResourceBaseEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (7, 0, 43);	// [700B1]
		public static readonly new string EntityStructuredTypeKey = "[700B1]";
	}
}
#endregion

#region Epsitec.Common.Support.ResourceString Entity
namespace Epsitec.Common.Support.Entities
{
	///	<summary>
	///	The <c>ResourceString</c> entity.
	///	designer:cap/7007
	///	</summary>
	public partial class ResourceStringEntity : global::Epsitec.Common.Support.Entities.ResourceBaseEntity
	{
		///	<summary>
		///	The <c>Text</c> field.
		///	designer:fld/7007/7008
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[7008]")]
		public string Text
		{
			get
			{
				return this.GetField<string> ("[7008]");
			}
			set
			{
				string oldValue = this.Text;
				if (oldValue != value || !this.IsFieldDefined("[7008]"))
				{
					this.OnTextChanging (oldValue, value);
					this.SetField<string> ("[7008]", oldValue, value);
					this.OnTextChanged (oldValue, value);
				}
			}
		}
		
		partial void OnTextChanging(string oldValue, string newValue);
		partial void OnTextChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Common.Support.Entities.ResourceStringEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Common.Support.Entities.ResourceStringEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (7, 0, 7);	// [7007]
		public static readonly new string EntityStructuredTypeKey = "[7007]";
	}
}
#endregion

#region Epsitec.Common.Support.ResourceCaption Entity
namespace Epsitec.Common.Support.Entities
{
	///	<summary>
	///	The <c>ResourceCaption</c> entity.
	///	designer:cap/7006
	///	</summary>
	public partial class ResourceCaptionEntity : global::Epsitec.Common.Support.Entities.ResourceBaseEntity
	{
		///	<summary>
		///	The <c>Labels</c> field.
		///	designer:fld/7006/700B
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[700B]")]
		public global::System.Collections.Generic.IList<string> Labels
		{
			get
			{
				throw new global::System.NotSupportedException ("Collection of type string not supported");
			}
		}
		///	<summary>
		///	The <c>Description</c> field.
		///	designer:fld/7006/700C
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[700C]")]
		public string Description
		{
			get
			{
				return this.GetField<string> ("[700C]");
			}
			set
			{
				string oldValue = this.Description;
				if (oldValue != value || !this.IsFieldDefined("[700C]"))
				{
					this.OnDescriptionChanging (oldValue, value);
					this.SetField<string> ("[700C]", oldValue, value);
					this.OnDescriptionChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Icon</c> field.
		///	designer:fld/7006/700D
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[700D]")]
		public string Icon
		{
			get
			{
				return this.GetField<string> ("[700D]");
			}
			set
			{
				string oldValue = this.Icon;
				if (oldValue != value || !this.IsFieldDefined("[700D]"))
				{
					this.OnIconChanging (oldValue, value);
					this.SetField<string> ("[700D]", oldValue, value);
					this.OnIconChanged (oldValue, value);
				}
			}
		}
		
		partial void OnDescriptionChanging(string oldValue, string newValue);
		partial void OnDescriptionChanged(string oldValue, string newValue);
		partial void OnIconChanging(string oldValue, string newValue);
		partial void OnIconChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Common.Support.Entities.ResourceCaptionEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Common.Support.Entities.ResourceCaptionEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (7, 0, 6);	// [7006]
		public static readonly new string EntityStructuredTypeKey = "[7006]";
	}
}
#endregion

#region Epsitec.Common.Support.ResourceCommand Entity
namespace Epsitec.Common.Support.Entities
{
	///	<summary>
	///	The <c>ResourceCommand</c> entity.
	///	designer:cap/700G
	///	</summary>
	public partial class ResourceCommandEntity : global::Epsitec.Common.Support.Entities.ResourceCaptionEntity
	{
		///	<summary>
		///	The <c>DefaultParameter</c> field.
		///	designer:fld/700G/700M
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[700M]")]
		public string DefaultParameter
		{
			get
			{
				return this.GetField<string> ("[700M]");
			}
			set
			{
				string oldValue = this.DefaultParameter;
				if (oldValue != value || !this.IsFieldDefined("[700M]"))
				{
					this.OnDefaultParameterChanging (oldValue, value);
					this.SetField<string> ("[700M]", oldValue, value);
					this.OnDefaultParameterChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Statefull</c> field.
		///	designer:fld/700G/700N
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[700N]")]
		public bool Statefull
		{
			get
			{
				return this.GetField<bool> ("[700N]");
			}
			set
			{
				bool oldValue = this.Statefull;
				if (oldValue != value || !this.IsFieldDefined("[700N]"))
				{
					this.OnStatefullChanging (oldValue, value);
					this.SetField<bool> ("[700N]", oldValue, value);
					this.OnStatefullChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Shortcuts</c> field.
		///	designer:fld/700G/700O
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[700O]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Common.Support.Entities.ShortcutEntity> Shortcuts
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Common.Support.Entities.ShortcutEntity> ("[700O]");
			}
		}
		///	<summary>
		///	The <c>Group</c> field.
		///	designer:fld/700G/700A1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[700A1]")]
		public string Group
		{
			get
			{
				return this.GetField<string> ("[700A1]");
			}
			set
			{
				string oldValue = this.Group;
				if (oldValue != value || !this.IsFieldDefined("[700A1]"))
				{
					this.OnGroupChanging (oldValue, value);
					this.SetField<string> ("[700A1]", oldValue, value);
					this.OnGroupChanged (oldValue, value);
				}
			}
		}
		
		partial void OnDefaultParameterChanging(string oldValue, string newValue);
		partial void OnDefaultParameterChanged(string oldValue, string newValue);
		partial void OnStatefullChanging(bool oldValue, bool newValue);
		partial void OnStatefullChanged(bool oldValue, bool newValue);
		partial void OnGroupChanging(string oldValue, string newValue);
		partial void OnGroupChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Common.Support.Entities.ResourceCommandEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Common.Support.Entities.ResourceCommandEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (7, 0, 16);	// [700G]
		public static readonly new string EntityStructuredTypeKey = "[700G]";
	}
}
#endregion

#region Epsitec.Common.Support.ResourceBaseType Entity
namespace Epsitec.Common.Support.Entities
{
	///	<summary>
	///	The <c>ResourceBaseType</c> entity.
	///	designer:cap/7005
	///	</summary>
	public partial class ResourceBaseTypeEntity : global::Epsitec.Common.Support.Entities.ResourceCaptionEntity
	{
		///	<summary>
		///	The <c>TypeCode</c> field.
		///	designer:fld/7005/70022
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[70022]")]
		public global::Epsitec.Common.Types.TypeCode TypeCode
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.TypeCode> ("[70022]");
			}
			set
			{
				global::Epsitec.Common.Types.TypeCode oldValue = this.TypeCode;
				if (oldValue != value || !this.IsFieldDefined("[70022]"))
				{
					this.OnTypeCodeChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.TypeCode> ("[70022]", oldValue, value);
					this.OnTypeCodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DefaultController</c> field.
		///	designer:fld/7005/70001
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[70001]")]
		public string DefaultController
		{
			get
			{
				return this.GetField<string> ("[70001]");
			}
			set
			{
				string oldValue = this.DefaultController;
				if (oldValue != value || !this.IsFieldDefined("[70001]"))
				{
					this.OnDefaultControllerChanging (oldValue, value);
					this.SetField<string> ("[70001]", oldValue, value);
					this.OnDefaultControllerChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DefaultControllerParameters</c> field.
		///	designer:fld/7005/700B2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[700B2]")]
		public string DefaultControllerParameters
		{
			get
			{
				return this.GetField<string> ("[700B2]");
			}
			set
			{
				string oldValue = this.DefaultControllerParameters;
				if (oldValue != value || !this.IsFieldDefined("[700B2]"))
				{
					this.OnDefaultControllerParametersChanging (oldValue, value);
					this.SetField<string> ("[700B2]", oldValue, value);
					this.OnDefaultControllerParametersChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Nullable</c> field.
		///	designer:fld/7005/70011
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[70011]")]
		public bool Nullable
		{
			get
			{
				return this.GetField<bool> ("[70011]");
			}
			set
			{
				bool oldValue = this.Nullable;
				if (oldValue != value || !this.IsFieldDefined("[70011]"))
				{
					this.OnNullableChanging (oldValue, value);
					this.SetField<bool> ("[70011]", oldValue, value);
					this.OnNullableChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DefaultValue</c> field.
		///	designer:fld/7005/701O
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[701O]")]
		public global::System.Object DefaultValue
		{
			get
			{
				return this.GetField<global::System.Object> ("[701O]");
			}
			set
			{
				global::System.Object oldValue = this.DefaultValue;
				if (oldValue != value || !this.IsFieldDefined("[701O]"))
				{
					this.OnDefaultValueChanging (oldValue, value);
					this.SetField<global::System.Object> ("[701O]", oldValue, value);
					this.OnDefaultValueChanged (oldValue, value);
				}
			}
		}
		
		partial void OnTypeCodeChanging(global::Epsitec.Common.Types.TypeCode oldValue, global::Epsitec.Common.Types.TypeCode newValue);
		partial void OnTypeCodeChanged(global::Epsitec.Common.Types.TypeCode oldValue, global::Epsitec.Common.Types.TypeCode newValue);
		partial void OnDefaultControllerChanging(string oldValue, string newValue);
		partial void OnDefaultControllerChanged(string oldValue, string newValue);
		partial void OnDefaultControllerParametersChanging(string oldValue, string newValue);
		partial void OnDefaultControllerParametersChanged(string oldValue, string newValue);
		partial void OnNullableChanging(bool oldValue, bool newValue);
		partial void OnNullableChanged(bool oldValue, bool newValue);
		partial void OnDefaultValueChanging(global::System.Object oldValue, global::System.Object newValue);
		partial void OnDefaultValueChanged(global::System.Object oldValue, global::System.Object newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Common.Support.Entities.ResourceBaseTypeEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Common.Support.Entities.ResourceBaseTypeEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (7, 0, 5);	// [7005]
		public static readonly new string EntityStructuredTypeKey = "[7005]";
	}
}
#endregion

#region Epsitec.Common.Support.ResourceStructuredType Entity
namespace Epsitec.Common.Support.Entities
{
	///	<summary>
	///	The <c>ResourceStructuredType</c> entity.
	///	designer:cap/700H
	///	</summary>
	public partial class ResourceStructuredTypeEntity : global::Epsitec.Common.Support.Entities.ResourceBaseTypeEntity
	{
		///	<summary>
		///	The <c>Fields</c> field.
		///	designer:fld/700H/70021
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[70021]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Common.Support.Entities.FieldEntity> Fields
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Common.Support.Entities.FieldEntity> ("[70021]");
			}
		}
		///	<summary>
		///	The <c>Class</c> field.
		///	designer:fld/700H/700C1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[700C1]")]
		public global::Epsitec.Common.Types.StructuredTypeClass Class
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.StructuredTypeClass> ("[700C1]");
			}
			set
			{
				global::Epsitec.Common.Types.StructuredTypeClass oldValue = this.Class;
				if (oldValue != value || !this.IsFieldDefined("[700C1]"))
				{
					this.OnClassChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.StructuredTypeClass> ("[700C1]", oldValue, value);
					this.OnClassChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>BaseType</c> field.
		///	designer:fld/700H/700D1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[700D1]")]
		public global::Epsitec.Common.Support.Druid BaseType
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Support.Druid> ("[700D1]");
			}
			set
			{
				global::Epsitec.Common.Support.Druid oldValue = this.BaseType;
				if (oldValue != value || !this.IsFieldDefined("[700D1]"))
				{
					this.OnBaseTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Support.Druid> ("[700D1]", oldValue, value);
					this.OnBaseTypeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>InterfaceIds</c> field.
		///	designer:fld/700H/700H2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[700H2]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Common.Support.Entities.InterfaceIdEntity> InterfaceIds
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Common.Support.Entities.InterfaceIdEntity> ("[700H2]");
			}
		}
		///	<summary>
		///	The <c>DesignerLayouts</c> field.
		///	designer:fld/700H/700F2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[700F2]")]
		public string DesignerLayouts
		{
			get
			{
				return this.GetField<string> ("[700F2]");
			}
			set
			{
				string oldValue = this.DesignerLayouts;
				if (oldValue != value || !this.IsFieldDefined("[700F2]"))
				{
					this.OnDesignerLayoutsChanging (oldValue, value);
					this.SetField<string> ("[700F2]", oldValue, value);
					this.OnDesignerLayoutsChanged (oldValue, value);
				}
			}
		}
		
		partial void OnClassChanging(global::Epsitec.Common.Types.StructuredTypeClass oldValue, global::Epsitec.Common.Types.StructuredTypeClass newValue);
		partial void OnClassChanged(global::Epsitec.Common.Types.StructuredTypeClass oldValue, global::Epsitec.Common.Types.StructuredTypeClass newValue);
		partial void OnBaseTypeChanging(global::Epsitec.Common.Support.Druid oldValue, global::Epsitec.Common.Support.Druid newValue);
		partial void OnBaseTypeChanged(global::Epsitec.Common.Support.Druid oldValue, global::Epsitec.Common.Support.Druid newValue);
		partial void OnDesignerLayoutsChanging(string oldValue, string newValue);
		partial void OnDesignerLayoutsChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Common.Support.Entities.ResourceStructuredTypeEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Common.Support.Entities.ResourceStructuredTypeEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (7, 0, 17);	// [700H]
		public static readonly new string EntityStructuredTypeKey = "[700H]";
	}
}
#endregion

#region Epsitec.Common.Support.Shortcut Entity
namespace Epsitec.Common.Support.Entities
{
	///	<summary>
	///	The <c>Shortcut</c> entity.
	///	designer:cap/700Q
	///	</summary>
	public partial class ShortcutEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>KeyCode</c> field.
		///	designer:fld/700Q/700R
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[700R]")]
		public string KeyCode
		{
			get
			{
				return this.GetField<string> ("[700R]");
			}
			set
			{
				string oldValue = this.KeyCode;
				if (oldValue != value || !this.IsFieldDefined("[700R]"))
				{
					this.OnKeyCodeChanging (oldValue, value);
					this.SetField<string> ("[700R]", oldValue, value);
					this.OnKeyCodeChanged (oldValue, value);
				}
			}
		}
		
		partial void OnKeyCodeChanging(string oldValue, string newValue);
		partial void OnKeyCodeChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Common.Support.Entities.ShortcutEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Common.Support.Entities.ShortcutEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (7, 0, 26);	// [700Q]
		public static readonly new string EntityStructuredTypeKey = "[700Q]";
	}
}
#endregion

#region Epsitec.Common.Support.Field Entity
namespace Epsitec.Common.Support.Entities
{
	///	<summary>
	///	The <c>Field</c> entity.
	///	designer:cap/70041
	///	</summary>
	public partial class FieldEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Type</c> field.
		///	designer:fld/70041/70051
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[70051]")]
		public global::Epsitec.Common.Support.Druid Type
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Support.Druid> ("[70051]");
			}
			set
			{
				global::Epsitec.Common.Support.Druid oldValue = this.Type;
				if (oldValue != value || !this.IsFieldDefined("[70051]"))
				{
					this.OnTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Support.Druid> ("[70051]", oldValue, value);
					this.OnTypeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>CaptionId</c> field.
		///	designer:fld/70041/70061
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[70061]")]
		public global::Epsitec.Common.Support.Druid CaptionId
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Support.Druid> ("[70061]");
			}
			set
			{
				global::Epsitec.Common.Support.Druid oldValue = this.CaptionId;
				if (oldValue != value || !this.IsFieldDefined("[70061]"))
				{
					this.OnCaptionIdChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Support.Druid> ("[70061]", oldValue, value);
					this.OnCaptionIdChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Relation</c> field.
		///	designer:fld/70041/70071
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[70071]")]
		public global::Epsitec.Common.Types.FieldRelation Relation
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FieldRelation> ("[70071]");
			}
			set
			{
				global::Epsitec.Common.Types.FieldRelation oldValue = this.Relation;
				if (oldValue != value || !this.IsFieldDefined("[70071]"))
				{
					this.OnRelationChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FieldRelation> ("[70071]", oldValue, value);
					this.OnRelationChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Membership</c> field.
		///	designer:fld/70041/7003
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[7003]")]
		public global::Epsitec.Common.Types.FieldMembership Membership
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FieldMembership> ("[7003]");
			}
			set
			{
				global::Epsitec.Common.Types.FieldMembership oldValue = this.Membership;
				if (oldValue != value || !this.IsFieldDefined("[7003]"))
				{
					this.OnMembershipChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FieldMembership> ("[7003]", oldValue, value);
					this.OnMembershipChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Source</c> field.
		///	designer:fld/70041/700D2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[700D2]")]
		public global::Epsitec.Common.Types.FieldSource Source
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FieldSource> ("[700D2]");
			}
			set
			{
				global::Epsitec.Common.Types.FieldSource oldValue = this.Source;
				if (oldValue != value || !this.IsFieldDefined("[700D2]"))
				{
					this.OnSourceChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FieldSource> ("[700D2]", oldValue, value);
					this.OnSourceChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Options</c> field.
		///	designer:fld/70041/7011
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[7011]")]
		public global::Epsitec.Common.Types.FieldOptions Options
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FieldOptions> ("[7011]");
			}
			set
			{
				global::Epsitec.Common.Types.FieldOptions oldValue = this.Options;
				if (oldValue != value || !this.IsFieldDefined("[7011]"))
				{
					this.OnOptionsChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FieldOptions> ("[7011]", oldValue, value);
					this.OnOptionsChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Expression</c> field.
		///	designer:fld/70041/700E2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[700E2]")]
		public string Expression
		{
			get
			{
				return this.GetField<string> ("[700E2]");
			}
			set
			{
				string oldValue = this.Expression;
				if (oldValue != value || !this.IsFieldDefined("[700E2]"))
				{
					this.OnExpressionChanging (oldValue, value);
					this.SetField<string> ("[700E2]", oldValue, value);
					this.OnExpressionChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DefiningTypeId</c> field.
		///	designer:fld/70041/700G2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[700G2]")]
		public global::Epsitec.Common.Support.Druid DefiningTypeId
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Support.Druid> ("[700G2]");
			}
			set
			{
				global::Epsitec.Common.Support.Druid oldValue = this.DefiningTypeId;
				if (oldValue != value || !this.IsFieldDefined("[700G2]"))
				{
					this.OnDefiningTypeIdChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Support.Druid> ("[700G2]", oldValue, value);
					this.OnDefiningTypeIdChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DeepDefiningTypeId</c> field.
		///	designer:fld/70041/701I
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[701I]")]
		public global::Epsitec.Common.Support.Druid DeepDefiningTypeId
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Support.Druid> ("[701I]");
			}
			set
			{
				global::Epsitec.Common.Support.Druid oldValue = this.DeepDefiningTypeId;
				if (oldValue != value || !this.IsFieldDefined("[701I]"))
				{
					this.OnDeepDefiningTypeIdChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Support.Druid> ("[701I]", oldValue, value);
					this.OnDeepDefiningTypeIdChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>CultureMapSource</c> field.
		///	designer:fld/70041/701B
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[701B]")]
		public global::Epsitec.Common.Support.CultureMapSource CultureMapSource
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Support.CultureMapSource> ("[701B]");
			}
			set
			{
				global::Epsitec.Common.Support.CultureMapSource oldValue = this.CultureMapSource;
				if (oldValue != value || !this.IsFieldDefined("[701B]"))
				{
					this.OnCultureMapSourceChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Support.CultureMapSource> ("[701B]", oldValue, value);
					this.OnCultureMapSourceChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>IsInterfaceDefinition</c> field.
		///	designer:fld/70041/701J
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[701J]")]
		public bool IsInterfaceDefinition
		{
			get
			{
				return this.GetField<bool> ("[701J]");
			}
			set
			{
				bool oldValue = this.IsInterfaceDefinition;
				if (oldValue != value || !this.IsFieldDefined("[701J]"))
				{
					this.OnIsInterfaceDefinitionChanging (oldValue, value);
					this.SetField<bool> ("[701J]", oldValue, value);
					this.OnIsInterfaceDefinitionChanged (oldValue, value);
				}
			}
		}
		
		partial void OnTypeChanging(global::Epsitec.Common.Support.Druid oldValue, global::Epsitec.Common.Support.Druid newValue);
		partial void OnTypeChanged(global::Epsitec.Common.Support.Druid oldValue, global::Epsitec.Common.Support.Druid newValue);
		partial void OnCaptionIdChanging(global::Epsitec.Common.Support.Druid oldValue, global::Epsitec.Common.Support.Druid newValue);
		partial void OnCaptionIdChanged(global::Epsitec.Common.Support.Druid oldValue, global::Epsitec.Common.Support.Druid newValue);
		partial void OnRelationChanging(global::Epsitec.Common.Types.FieldRelation oldValue, global::Epsitec.Common.Types.FieldRelation newValue);
		partial void OnRelationChanged(global::Epsitec.Common.Types.FieldRelation oldValue, global::Epsitec.Common.Types.FieldRelation newValue);
		partial void OnMembershipChanging(global::Epsitec.Common.Types.FieldMembership oldValue, global::Epsitec.Common.Types.FieldMembership newValue);
		partial void OnMembershipChanged(global::Epsitec.Common.Types.FieldMembership oldValue, global::Epsitec.Common.Types.FieldMembership newValue);
		partial void OnSourceChanging(global::Epsitec.Common.Types.FieldSource oldValue, global::Epsitec.Common.Types.FieldSource newValue);
		partial void OnSourceChanged(global::Epsitec.Common.Types.FieldSource oldValue, global::Epsitec.Common.Types.FieldSource newValue);
		partial void OnOptionsChanging(global::Epsitec.Common.Types.FieldOptions oldValue, global::Epsitec.Common.Types.FieldOptions newValue);
		partial void OnOptionsChanged(global::Epsitec.Common.Types.FieldOptions oldValue, global::Epsitec.Common.Types.FieldOptions newValue);
		partial void OnExpressionChanging(string oldValue, string newValue);
		partial void OnExpressionChanged(string oldValue, string newValue);
		partial void OnDefiningTypeIdChanging(global::Epsitec.Common.Support.Druid oldValue, global::Epsitec.Common.Support.Druid newValue);
		partial void OnDefiningTypeIdChanged(global::Epsitec.Common.Support.Druid oldValue, global::Epsitec.Common.Support.Druid newValue);
		partial void OnDeepDefiningTypeIdChanging(global::Epsitec.Common.Support.Druid oldValue, global::Epsitec.Common.Support.Druid newValue);
		partial void OnDeepDefiningTypeIdChanged(global::Epsitec.Common.Support.Druid oldValue, global::Epsitec.Common.Support.Druid newValue);
		partial void OnCultureMapSourceChanging(global::Epsitec.Common.Support.CultureMapSource oldValue, global::Epsitec.Common.Support.CultureMapSource newValue);
		partial void OnCultureMapSourceChanged(global::Epsitec.Common.Support.CultureMapSource oldValue, global::Epsitec.Common.Support.CultureMapSource newValue);
		partial void OnIsInterfaceDefinitionChanging(bool oldValue, bool newValue);
		partial void OnIsInterfaceDefinitionChanged(bool oldValue, bool newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Common.Support.Entities.FieldEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Common.Support.Entities.FieldEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (7, 0, 36);	// [70041]
		public static readonly new string EntityStructuredTypeKey = "[70041]";
	}
}
#endregion

#region Epsitec.Common.Support.ResourceNumericType Entity
namespace Epsitec.Common.Support.Entities
{
	///	<summary>
	///	The <c>ResourceNumericType</c> entity.
	///	designer:cap/700F1
	///	</summary>
	public partial class ResourceNumericTypeEntity : global::Epsitec.Common.Support.Entities.ResourceBaseTypeEntity
	{
		///	<summary>
		///	The <c>Range</c> field.
		///	designer:fld/700F1/700I1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[700I1]")]
		public global::Epsitec.Common.Types.DecimalRange Range
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.DecimalRange> ("[700I1]");
			}
			set
			{
				global::Epsitec.Common.Types.DecimalRange oldValue = this.Range;
				if (oldValue != value || !this.IsFieldDefined("[700I1]"))
				{
					this.OnRangeChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.DecimalRange> ("[700I1]", oldValue, value);
					this.OnRangeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PreferredRange</c> field.
		///	designer:fld/700F1/700J1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[700J1]")]
		public global::Epsitec.Common.Types.DecimalRange PreferredRange
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.DecimalRange> ("[700J1]");
			}
			set
			{
				global::Epsitec.Common.Types.DecimalRange oldValue = this.PreferredRange;
				if (oldValue != value || !this.IsFieldDefined("[700J1]"))
				{
					this.OnPreferredRangeChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.DecimalRange> ("[700J1]", oldValue, value);
					this.OnPreferredRangeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>SmallStep</c> field.
		///	designer:fld/700F1/700K1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[700K1]")]
		public global::System.Decimal SmallStep
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[700K1]");
			}
			set
			{
				global::System.Decimal oldValue = this.SmallStep;
				if (oldValue != value || !this.IsFieldDefined("[700K1]"))
				{
					this.OnSmallStepChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[700K1]", oldValue, value);
					this.OnSmallStepChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>LargeStep</c> field.
		///	designer:fld/700F1/700L1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[700L1]")]
		public global::System.Decimal LargeStep
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[700L1]");
			}
			set
			{
				global::System.Decimal oldValue = this.LargeStep;
				if (oldValue != value || !this.IsFieldDefined("[700L1]"))
				{
					this.OnLargeStepChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[700L1]", oldValue, value);
					this.OnLargeStepChanged (oldValue, value);
				}
			}
		}
		
		partial void OnRangeChanging(global::Epsitec.Common.Types.DecimalRange oldValue, global::Epsitec.Common.Types.DecimalRange newValue);
		partial void OnRangeChanged(global::Epsitec.Common.Types.DecimalRange oldValue, global::Epsitec.Common.Types.DecimalRange newValue);
		partial void OnPreferredRangeChanging(global::Epsitec.Common.Types.DecimalRange oldValue, global::Epsitec.Common.Types.DecimalRange newValue);
		partial void OnPreferredRangeChanged(global::Epsitec.Common.Types.DecimalRange oldValue, global::Epsitec.Common.Types.DecimalRange newValue);
		partial void OnSmallStepChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnSmallStepChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnLargeStepChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnLargeStepChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Common.Support.Entities.ResourceNumericTypeEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Common.Support.Entities.ResourceNumericTypeEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (7, 0, 47);	// [700F1]
		public static readonly new string EntityStructuredTypeKey = "[700F1]";
	}
}
#endregion

#region Epsitec.Common.Support.ResourceDateTimeType Entity
namespace Epsitec.Common.Support.Entities
{
	///	<summary>
	///	The <c>ResourceDateTimeType</c> entity.
	///	designer:cap/700G1
	///	</summary>
	public partial class ResourceDateTimeTypeEntity : global::Epsitec.Common.Support.Entities.ResourceBaseTypeEntity
	{
		///	<summary>
		///	The <c>Resolution</c> field.
		///	designer:fld/700G1/700M1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[700M1]")]
		public global::Epsitec.Common.Types.TimeResolution Resolution
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.TimeResolution> ("[700M1]");
			}
			set
			{
				global::Epsitec.Common.Types.TimeResolution oldValue = this.Resolution;
				if (oldValue != value || !this.IsFieldDefined("[700M1]"))
				{
					this.OnResolutionChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.TimeResolution> ("[700M1]", oldValue, value);
					this.OnResolutionChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>MinimumDate</c> field.
		///	designer:fld/700G1/700N1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[700N1]")]
		public global::Epsitec.Common.Types.Date MinimumDate
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Date> ("[700N1]");
			}
			set
			{
				global::Epsitec.Common.Types.Date oldValue = this.MinimumDate;
				if (oldValue != value || !this.IsFieldDefined("[700N1]"))
				{
					this.OnMinimumDateChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Date> ("[700N1]", oldValue, value);
					this.OnMinimumDateChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>MaximumDate</c> field.
		///	designer:fld/700G1/700O1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[700O1]")]
		public global::Epsitec.Common.Types.Date MaximumDate
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Date> ("[700O1]");
			}
			set
			{
				global::Epsitec.Common.Types.Date oldValue = this.MaximumDate;
				if (oldValue != value || !this.IsFieldDefined("[700O1]"))
				{
					this.OnMaximumDateChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Date> ("[700O1]", oldValue, value);
					this.OnMaximumDateChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>MinimumTime</c> field.
		///	designer:fld/700G1/700P1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[700P1]")]
		public global::Epsitec.Common.Types.Time MinimumTime
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Time> ("[700P1]");
			}
			set
			{
				global::Epsitec.Common.Types.Time oldValue = this.MinimumTime;
				if (oldValue != value || !this.IsFieldDefined("[700P1]"))
				{
					this.OnMinimumTimeChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Time> ("[700P1]", oldValue, value);
					this.OnMinimumTimeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>MaximumTime</c> field.
		///	designer:fld/700G1/700Q1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[700Q1]")]
		public global::Epsitec.Common.Types.Time MaximumTime
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Time> ("[700Q1]");
			}
			set
			{
				global::Epsitec.Common.Types.Time oldValue = this.MaximumTime;
				if (oldValue != value || !this.IsFieldDefined("[700Q1]"))
				{
					this.OnMaximumTimeChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Time> ("[700Q1]", oldValue, value);
					this.OnMaximumTimeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DateStep</c> field.
		///	designer:fld/700G1/700S1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[700S1]")]
		public global::Epsitec.Common.Types.DateSpan DateStep
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.DateSpan> ("[700S1]");
			}
			set
			{
				global::Epsitec.Common.Types.DateSpan oldValue = this.DateStep;
				if (oldValue != value || !this.IsFieldDefined("[700S1]"))
				{
					this.OnDateStepChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.DateSpan> ("[700S1]", oldValue, value);
					this.OnDateStepChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>TimeStep</c> field.
		///	designer:fld/700G1/700R1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[700R1]")]
		public global::System.TimeSpan TimeStep
		{
			get
			{
				return this.GetField<global::System.TimeSpan> ("[700R1]");
			}
			set
			{
				global::System.TimeSpan oldValue = this.TimeStep;
				if (oldValue != value || !this.IsFieldDefined("[700R1]"))
				{
					this.OnTimeStepChanging (oldValue, value);
					this.SetField<global::System.TimeSpan> ("[700R1]", oldValue, value);
					this.OnTimeStepChanged (oldValue, value);
				}
			}
		}
		
		partial void OnResolutionChanging(global::Epsitec.Common.Types.TimeResolution oldValue, global::Epsitec.Common.Types.TimeResolution newValue);
		partial void OnResolutionChanged(global::Epsitec.Common.Types.TimeResolution oldValue, global::Epsitec.Common.Types.TimeResolution newValue);
		partial void OnMinimumDateChanging(global::Epsitec.Common.Types.Date oldValue, global::Epsitec.Common.Types.Date newValue);
		partial void OnMinimumDateChanged(global::Epsitec.Common.Types.Date oldValue, global::Epsitec.Common.Types.Date newValue);
		partial void OnMaximumDateChanging(global::Epsitec.Common.Types.Date oldValue, global::Epsitec.Common.Types.Date newValue);
		partial void OnMaximumDateChanged(global::Epsitec.Common.Types.Date oldValue, global::Epsitec.Common.Types.Date newValue);
		partial void OnMinimumTimeChanging(global::Epsitec.Common.Types.Time oldValue, global::Epsitec.Common.Types.Time newValue);
		partial void OnMinimumTimeChanged(global::Epsitec.Common.Types.Time oldValue, global::Epsitec.Common.Types.Time newValue);
		partial void OnMaximumTimeChanging(global::Epsitec.Common.Types.Time oldValue, global::Epsitec.Common.Types.Time newValue);
		partial void OnMaximumTimeChanged(global::Epsitec.Common.Types.Time oldValue, global::Epsitec.Common.Types.Time newValue);
		partial void OnDateStepChanging(global::Epsitec.Common.Types.DateSpan oldValue, global::Epsitec.Common.Types.DateSpan newValue);
		partial void OnDateStepChanged(global::Epsitec.Common.Types.DateSpan oldValue, global::Epsitec.Common.Types.DateSpan newValue);
		partial void OnTimeStepChanging(global::System.TimeSpan oldValue, global::System.TimeSpan newValue);
		partial void OnTimeStepChanged(global::System.TimeSpan oldValue, global::System.TimeSpan newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Common.Support.Entities.ResourceDateTimeTypeEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Common.Support.Entities.ResourceDateTimeTypeEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (7, 0, 48);	// [700G1]
		public static readonly new string EntityStructuredTypeKey = "[700G1]";
	}
}
#endregion

#region Epsitec.Common.Support.ResourceStringType Entity
namespace Epsitec.Common.Support.Entities
{
	///	<summary>
	///	The <c>ResourceStringType</c> entity.
	///	designer:cap/700H1
	///	</summary>
	public partial class ResourceStringTypeEntity : global::Epsitec.Common.Support.Entities.ResourceBaseTypeEntity
	{
		///	<summary>
		///	The <c>MinimumLength</c> field.
		///	designer:fld/700H1/700T1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[700T1]")]
		public int MinimumLength
		{
			get
			{
				return this.GetField<int> ("[700T1]");
			}
			set
			{
				int oldValue = this.MinimumLength;
				if (oldValue != value || !this.IsFieldDefined("[700T1]"))
				{
					this.OnMinimumLengthChanging (oldValue, value);
					this.SetField<int> ("[700T1]", oldValue, value);
					this.OnMinimumLengthChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>MaximumLength</c> field.
		///	designer:fld/700H1/700U1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[700U1]")]
		public int MaximumLength
		{
			get
			{
				return this.GetField<int> ("[700U1]");
			}
			set
			{
				int oldValue = this.MaximumLength;
				if (oldValue != value || !this.IsFieldDefined("[700U1]"))
				{
					this.OnMaximumLengthChanging (oldValue, value);
					this.SetField<int> ("[700U1]", oldValue, value);
					this.OnMaximumLengthChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>UseMultilingualStorage</c> field.
		///	designer:fld/700H1/700V1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[700V1]")]
		public bool UseMultilingualStorage
		{
			get
			{
				return this.GetField<bool> ("[700V1]");
			}
			set
			{
				bool oldValue = this.UseMultilingualStorage;
				if (oldValue != value || !this.IsFieldDefined("[700V1]"))
				{
					this.OnUseMultilingualStorageChanging (oldValue, value);
					this.SetField<bool> ("[700V1]", oldValue, value);
					this.OnUseMultilingualStorageChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>UseFormattedText</c> field.
		///	designer:fld/700H1/70A2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[70A2]")]
		public bool UseFormattedText
		{
			get
			{
				return this.GetField<bool> ("[70A2]");
			}
			set
			{
				bool oldValue = this.UseFormattedText;
				if (oldValue != value || !this.IsFieldDefined("[70A2]"))
				{
					this.OnUseFormattedTextChanging (oldValue, value);
					this.SetField<bool> ("[70A2]", oldValue, value);
					this.OnUseFormattedTextChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DefaultSearchBehavior</c> field.
		///	designer:fld/700H1/70A
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[70A]")]
		public global::Epsitec.Common.Types.StringSearchBehavior DefaultSearchBehavior
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.StringSearchBehavior> ("[70A]");
			}
			set
			{
				global::Epsitec.Common.Types.StringSearchBehavior oldValue = this.DefaultSearchBehavior;
				if (oldValue != value || !this.IsFieldDefined("[70A]"))
				{
					this.OnDefaultSearchBehaviorChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.StringSearchBehavior> ("[70A]", oldValue, value);
					this.OnDefaultSearchBehaviorChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DefaultComparisonBehavior</c> field.
		///	designer:fld/700H1/70A1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[70A1]")]
		public global::Epsitec.Common.Types.StringComparisonBehavior DefaultComparisonBehavior
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.StringComparisonBehavior> ("[70A1]");
			}
			set
			{
				global::Epsitec.Common.Types.StringComparisonBehavior oldValue = this.DefaultComparisonBehavior;
				if (oldValue != value || !this.IsFieldDefined("[70A1]"))
				{
					this.OnDefaultComparisonBehaviorChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.StringComparisonBehavior> ("[70A1]", oldValue, value);
					this.OnDefaultComparisonBehaviorChanged (oldValue, value);
				}
			}
		}
		
		partial void OnMinimumLengthChanging(int oldValue, int newValue);
		partial void OnMinimumLengthChanged(int oldValue, int newValue);
		partial void OnMaximumLengthChanging(int oldValue, int newValue);
		partial void OnMaximumLengthChanged(int oldValue, int newValue);
		partial void OnUseMultilingualStorageChanging(bool oldValue, bool newValue);
		partial void OnUseMultilingualStorageChanged(bool oldValue, bool newValue);
		partial void OnUseFormattedTextChanging(bool oldValue, bool newValue);
		partial void OnUseFormattedTextChanged(bool oldValue, bool newValue);
		partial void OnDefaultSearchBehaviorChanging(global::Epsitec.Common.Types.StringSearchBehavior oldValue, global::Epsitec.Common.Types.StringSearchBehavior newValue);
		partial void OnDefaultSearchBehaviorChanged(global::Epsitec.Common.Types.StringSearchBehavior oldValue, global::Epsitec.Common.Types.StringSearchBehavior newValue);
		partial void OnDefaultComparisonBehaviorChanging(global::Epsitec.Common.Types.StringComparisonBehavior oldValue, global::Epsitec.Common.Types.StringComparisonBehavior newValue);
		partial void OnDefaultComparisonBehaviorChanged(global::Epsitec.Common.Types.StringComparisonBehavior oldValue, global::Epsitec.Common.Types.StringComparisonBehavior newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Common.Support.Entities.ResourceStringTypeEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Common.Support.Entities.ResourceStringTypeEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (7, 0, 49);	// [700H1]
		public static readonly new string EntityStructuredTypeKey = "[700H1]";
	}
}
#endregion

#region Epsitec.Common.Support.ResourceBinaryType Entity
namespace Epsitec.Common.Support.Entities
{
	///	<summary>
	///	The <c>ResourceBinaryType</c> entity.
	///	designer:cap/70002
	///	</summary>
	public partial class ResourceBinaryTypeEntity : global::Epsitec.Common.Support.Entities.ResourceBaseTypeEntity
	{
		///	<summary>
		///	The <c>MimeType</c> field.
		///	designer:fld/70002/70012
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[70012]")]
		public string MimeType
		{
			get
			{
				return this.GetField<string> ("[70012]");
			}
			set
			{
				string oldValue = this.MimeType;
				if (oldValue != value || !this.IsFieldDefined("[70012]"))
				{
					this.OnMimeTypeChanging (oldValue, value);
					this.SetField<string> ("[70012]", oldValue, value);
					this.OnMimeTypeChanged (oldValue, value);
				}
			}
		}
		
		partial void OnMimeTypeChanging(string oldValue, string newValue);
		partial void OnMimeTypeChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Common.Support.Entities.ResourceBinaryTypeEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Common.Support.Entities.ResourceBinaryTypeEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (7, 0, 64);	// [70002]
		public static readonly new string EntityStructuredTypeKey = "[70002]";
	}
}
#endregion

#region Epsitec.Common.Support.ResourceCollectionType Entity
namespace Epsitec.Common.Support.Entities
{
	///	<summary>
	///	The <c>ResourceCollectionType</c> entity.
	///	designer:cap/70032
	///	</summary>
	public partial class ResourceCollectionTypeEntity : global::Epsitec.Common.Support.Entities.ResourceBaseTypeEntity
	{
		///	<summary>
		///	The <c>ItemType</c> field.
		///	designer:fld/70032/70082
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[70082]")]
		public global::System.Type ItemType
		{
			get
			{
				return this.GetField<global::System.Type> ("[70082]");
			}
			set
			{
				global::System.Type oldValue = this.ItemType;
				if (oldValue != value || !this.IsFieldDefined("[70082]"))
				{
					this.OnItemTypeChanging (oldValue, value);
					this.SetField<global::System.Type> ("[70082]", oldValue, value);
					this.OnItemTypeChanged (oldValue, value);
				}
			}
		}
		
		partial void OnItemTypeChanging(global::System.Type oldValue, global::System.Type newValue);
		partial void OnItemTypeChanged(global::System.Type oldValue, global::System.Type newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Common.Support.Entities.ResourceCollectionTypeEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Common.Support.Entities.ResourceCollectionTypeEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (7, 0, 67);	// [70032]
		public static readonly new string EntityStructuredTypeKey = "[70032]";
	}
}
#endregion

#region Epsitec.Common.Support.ResourceOtherType Entity
namespace Epsitec.Common.Support.Entities
{
	///	<summary>
	///	The <c>ResourceOtherType</c> entity.
	///	designer:cap/70042
	///	</summary>
	public partial class ResourceOtherTypeEntity : global::Epsitec.Common.Support.Entities.ResourceBaseTypeEntity
	{
		///	<summary>
		///	The <c>SystemType</c> field.
		///	designer:fld/70042/70072
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[70072]")]
		public global::System.Type SystemType
		{
			get
			{
				return this.GetField<global::System.Type> ("[70072]");
			}
			set
			{
				global::System.Type oldValue = this.SystemType;
				if (oldValue != value || !this.IsFieldDefined("[70072]"))
				{
					this.OnSystemTypeChanging (oldValue, value);
					this.SetField<global::System.Type> ("[70072]", oldValue, value);
					this.OnSystemTypeChanged (oldValue, value);
				}
			}
		}
		
		partial void OnSystemTypeChanging(global::System.Type oldValue, global::System.Type newValue);
		partial void OnSystemTypeChanged(global::System.Type oldValue, global::System.Type newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Common.Support.Entities.ResourceOtherTypeEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Common.Support.Entities.ResourceOtherTypeEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (7, 0, 68);	// [70042]
		public static readonly new string EntityStructuredTypeKey = "[70042]";
	}
}
#endregion

#region Epsitec.Common.Support.ResourceEnumType Entity
namespace Epsitec.Common.Support.Entities
{
	///	<summary>
	///	The <c>ResourceEnumType</c> entity.
	///	designer:cap/70052
	///	</summary>
	public partial class ResourceEnumTypeEntity : global::Epsitec.Common.Support.Entities.ResourceBaseTypeEntity
	{
		///	<summary>
		///	The <c>SystemType</c> field.
		///	designer:fld/70052/700C2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[700C2]")]
		public global::System.Type SystemType
		{
			get
			{
				return this.GetField<global::System.Type> ("[700C2]");
			}
			set
			{
				global::System.Type oldValue = this.SystemType;
				if (oldValue != value || !this.IsFieldDefined("[700C2]"))
				{
					this.OnSystemTypeChanging (oldValue, value);
					this.SetField<global::System.Type> ("[700C2]", oldValue, value);
					this.OnSystemTypeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Values</c> field.
		///	designer:fld/70052/70092
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[70092]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Common.Support.Entities.EnumValueEntity> Values
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Common.Support.Entities.EnumValueEntity> ("[70092]");
			}
		}
		
		partial void OnSystemTypeChanging(global::System.Type oldValue, global::System.Type newValue);
		partial void OnSystemTypeChanged(global::System.Type oldValue, global::System.Type newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Common.Support.Entities.ResourceEnumTypeEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Common.Support.Entities.ResourceEnumTypeEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (7, 0, 69);	// [70052]
		public static readonly new string EntityStructuredTypeKey = "[70052]";
	}
}
#endregion

#region Epsitec.Common.Support.EnumValue Entity
namespace Epsitec.Common.Support.Entities
{
	///	<summary>
	///	The <c>EnumValue</c> entity.
	///	designer:cap/70062
	///	</summary>
	public partial class EnumValueEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>CaptionId</c> field.
		///	designer:fld/70062/700A2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[700A2]")]
		public global::Epsitec.Common.Support.Druid CaptionId
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Support.Druid> ("[700A2]");
			}
			set
			{
				global::Epsitec.Common.Support.Druid oldValue = this.CaptionId;
				if (oldValue != value || !this.IsFieldDefined("[700A2]"))
				{
					this.OnCaptionIdChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Support.Druid> ("[700A2]", oldValue, value);
					this.OnCaptionIdChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>CultureMapSource</c> field.
		///	designer:fld/70062/701D
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[701D]")]
		public global::Epsitec.Common.Support.CultureMapSource CultureMapSource
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Support.CultureMapSource> ("[701D]");
			}
			set
			{
				global::Epsitec.Common.Support.CultureMapSource oldValue = this.CultureMapSource;
				if (oldValue != value || !this.IsFieldDefined("[701D]"))
				{
					this.OnCultureMapSourceChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Support.CultureMapSource> ("[701D]", oldValue, value);
					this.OnCultureMapSourceChanged (oldValue, value);
				}
			}
		}
		
		partial void OnCaptionIdChanging(global::Epsitec.Common.Support.Druid oldValue, global::Epsitec.Common.Support.Druid newValue);
		partial void OnCaptionIdChanged(global::Epsitec.Common.Support.Druid oldValue, global::Epsitec.Common.Support.Druid newValue);
		partial void OnCultureMapSourceChanging(global::Epsitec.Common.Support.CultureMapSource oldValue, global::Epsitec.Common.Support.CultureMapSource newValue);
		partial void OnCultureMapSourceChanged(global::Epsitec.Common.Support.CultureMapSource oldValue, global::Epsitec.Common.Support.CultureMapSource newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Common.Support.Entities.EnumValueEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Common.Support.Entities.EnumValueEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (7, 0, 70);	// [70062]
		public static readonly new string EntityStructuredTypeKey = "[70062]";
	}
}
#endregion

#region Epsitec.Common.Support.TestInterface Interface
namespace Epsitec.Common.Support.Entities
{
	///	<summary>
	///	Interface bidon utilisée pour des tests..
	///	designer:cap/700I2
	///	</summary>
	public interface TestInterface
	{
		///	<summary>
		///	Nom fictif pour une interface bidon.
		///	designer:fld/700I2/700J2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[700J2]")]
		string Name
		{
			get;
			set;
		}
		///	<summary>
		///	The <c>Resource</c> field.
		///	designer:fld/700I2/7012
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[7012]")]
		global::Epsitec.Common.Support.Entities.ResourceStringEntity Resource
		{
			get;
			set;
		}
	}
	public static partial class TestInterfaceInterfaceImplementation
	{
		public static string GetName(global::Epsitec.Common.Support.Entities.TestInterface obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<string> ("[700J2]");
		}
		public static void SetName(global::Epsitec.Common.Support.Entities.TestInterface obj, string value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			string oldValue = obj.Name;
			if (oldValue != value || !entity.IsFieldDefined("[700J2]"))
			{
				TestInterfaceInterfaceImplementation.OnNameChanging (obj, oldValue, value);
				entity.SetField<string> ("[700J2]", oldValue, value);
				TestInterfaceInterfaceImplementation.OnNameChanged (obj, oldValue, value);
			}
		}
		static partial void OnNameChanged(global::Epsitec.Common.Support.Entities.TestInterface obj, string oldValue, string newValue);
		static partial void OnNameChanging(global::Epsitec.Common.Support.Entities.TestInterface obj, string oldValue, string newValue);
		public static global::Epsitec.Common.Support.Entities.ResourceStringEntity GetResource(global::Epsitec.Common.Support.Entities.TestInterface obj)
		{
			return global::Epsitec.Common.Support.EntityEngine.AbstractEntity.GetCalculation<global::Epsitec.Common.Support.Entities.TestInterface, global::Epsitec.Common.Support.Entities.ResourceStringEntity> (obj, "[7012]", TestInterfaceInterfaceImplementation.FuncResource, TestInterfaceInterfaceImplementation.ExprResource);
		}
		public static void SetResource(global::Epsitec.Common.Support.Entities.TestInterface obj, global::Epsitec.Common.Support.Entities.ResourceStringEntity value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity.SetCalculation<global::Epsitec.Common.Support.Entities.TestInterface, global::Epsitec.Common.Support.Entities.ResourceStringEntity> (obj, "[7012]", value);
		}
		internal static readonly global::System.Func<global::Epsitec.Common.Support.Entities.TestInterface, global::Epsitec.Common.Support.Entities.ResourceStringEntity> FuncResource = x => null; // λ [700I2] [7012]
		internal static readonly global::System.Linq.Expressions.Expression<global::System.Func<global::Epsitec.Common.Support.Entities.TestInterface, global::Epsitec.Common.Support.Entities.ResourceStringEntity>> ExprResource = x => null; // λ [700I2] [7012]
	}
}
#endregion

#region Epsitec.Common.Support.TestInterfaceUser Entity
namespace Epsitec.Common.Support.Entities
{
	///	<summary>
	///	The <c>TestInterfaceUser</c> entity.
	///	designer:cap/7013
	///	</summary>
	public partial class TestInterfaceUserEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Common.Support.Entities.TestInterface
	{
		#region TestInterface Members
		///	<summary>
		///	Nom fictif pour une interface bidon.
		///	designer:fld/7013/700J2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[700J2]")]
		public virtual string Name
		{
			get
			{
				return global::Epsitec.Common.Support.EntityEngine.AbstractEntity.GetCalculation<global::Epsitec.Common.Support.Entities.TestInterfaceUserEntity, string> (this, "[700J2]", global::Epsitec.Common.Support.Entities.TestInterfaceUserEntity.FuncName, global::Epsitec.Common.Support.Entities.TestInterfaceUserEntity.ExprName);
			}
			set
			{
				global::Epsitec.Common.Support.EntityEngine.AbstractEntity.SetCalculation<global::Epsitec.Common.Support.Entities.TestInterfaceUserEntity, string> (this, "[700J2]", value);
			}
		}
		private static readonly global::System.Func<global::Epsitec.Common.Support.Entities.TestInterfaceUserEntity, string> FuncName = x => string.Empty; // λ [7013] [700J2]
		private static readonly global::System.Linq.Expressions.Expression<global::System.Func<global::Epsitec.Common.Support.Entities.TestInterfaceUserEntity, string>> ExprName = x => string.Empty; // λ [7013] [700J2]
		///	<summary>
		///	The <c>Resource</c> field.
		///	designer:fld/7013/7012
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[7012]")]
		public virtual global::Epsitec.Common.Support.Entities.ResourceStringEntity Resource
		{
			get
			{
				return global::Epsitec.Common.Support.Entities.TestInterfaceInterfaceImplementation.GetResource (this);
			}
			set
			{
				global::Epsitec.Common.Support.Entities.TestInterfaceInterfaceImplementation.SetResource (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>Extension1</c> field.
		///	designer:fld/7013/7014
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[7014]")]
		public virtual string Extension1
		{
			get
			{
				return global::Epsitec.Common.Support.EntityEngine.AbstractEntity.GetCalculation<global::Epsitec.Common.Support.Entities.TestInterfaceUserEntity, string> (this, "[7014]", global::Epsitec.Common.Support.Entities.TestInterfaceUserEntity.FuncExtension1, global::Epsitec.Common.Support.Entities.TestInterfaceUserEntity.ExprExtension1);
			}
			set
			{
				global::Epsitec.Common.Support.EntityEngine.AbstractEntity.SetCalculation<global::Epsitec.Common.Support.Entities.TestInterfaceUserEntity, string> (this, "[7014]", value);
			}
		}
		private static readonly global::System.Func<global::Epsitec.Common.Support.Entities.TestInterfaceUserEntity, string> FuncExtension1 = x => x.Name.ToUpper (); // λ [7013] [7014]
		private static readonly global::System.Linq.Expressions.Expression<global::System.Func<global::Epsitec.Common.Support.Entities.TestInterfaceUserEntity, string>> ExprExtension1 = x => x.Name.ToUpper (); // λ [7013] [7014]
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Common.Support.Entities.TestInterfaceUserEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Common.Support.Entities.TestInterfaceUserEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (7, 1, 3);	// [7013]
		public static readonly new string EntityStructuredTypeKey = "[7013]";
	}
}
#endregion

#region Epsitec.Common.Support.InterfaceId Entity
namespace Epsitec.Common.Support.Entities
{
	///	<summary>
	///	The <c>InterfaceId</c> entity.
	///	designer:cap/7019
	///	</summary>
	public partial class InterfaceIdEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>CaptionId</c> field.
		///	designer:fld/7019/701A
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[701A]")]
		public global::Epsitec.Common.Support.Druid CaptionId
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Support.Druid> ("[701A]");
			}
			set
			{
				global::Epsitec.Common.Support.Druid oldValue = this.CaptionId;
				if (oldValue != value || !this.IsFieldDefined("[701A]"))
				{
					this.OnCaptionIdChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Support.Druid> ("[701A]", oldValue, value);
					this.OnCaptionIdChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>CultureMapSource</c> field.
		///	designer:fld/7019/701C
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[701C]")]
		public global::Epsitec.Common.Support.CultureMapSource CultureMapSource
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Support.CultureMapSource> ("[701C]");
			}
			set
			{
				global::Epsitec.Common.Support.CultureMapSource oldValue = this.CultureMapSource;
				if (oldValue != value || !this.IsFieldDefined("[701C]"))
				{
					this.OnCultureMapSourceChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Support.CultureMapSource> ("[701C]", oldValue, value);
					this.OnCultureMapSourceChanged (oldValue, value);
				}
			}
		}
		
		partial void OnCaptionIdChanging(global::Epsitec.Common.Support.Druid oldValue, global::Epsitec.Common.Support.Druid newValue);
		partial void OnCaptionIdChanged(global::Epsitec.Common.Support.Druid oldValue, global::Epsitec.Common.Support.Druid newValue);
		partial void OnCultureMapSourceChanging(global::Epsitec.Common.Support.CultureMapSource oldValue, global::Epsitec.Common.Support.CultureMapSource newValue);
		partial void OnCultureMapSourceChanged(global::Epsitec.Common.Support.CultureMapSource oldValue, global::Epsitec.Common.Support.CultureMapSource newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Common.Support.Entities.InterfaceIdEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Common.Support.Entities.InterfaceIdEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (7, 1, 9);	// [7019]
		public static readonly new string EntityStructuredTypeKey = "[7019]";
	}
}
#endregion

#region Epsitec.Common.Support.ResourceBaseFile Entity
namespace Epsitec.Common.Support.Entities
{
	///	<summary>
	///	The <c>ResourceBaseFile</c> entity.
	///	designer:cap/701P
	///	</summary>
	public partial class ResourceBaseFileEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Bundle</c> field.
		///	designer:fld/701P/701Q
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[701Q]")]
		public global::System.Object Bundle
		{
			get
			{
				return this.GetField<global::System.Object> ("[701Q]");
			}
			set
			{
				global::System.Object oldValue = this.Bundle;
				if (oldValue != value || !this.IsFieldDefined("[701Q]"))
				{
					this.OnBundleChanging (oldValue, value);
					this.SetField<global::System.Object> ("[701Q]", oldValue, value);
					this.OnBundleChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>BundleAux</c> field.
		///	designer:fld/701P/70141
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[70141]")]
		public global::System.Object BundleAux
		{
			get
			{
				return this.GetField<global::System.Object> ("[70141]");
			}
			set
			{
				global::System.Object oldValue = this.BundleAux;
				if (oldValue != value || !this.IsFieldDefined("[70141]"))
				{
					this.OnBundleAuxChanging (oldValue, value);
					this.SetField<global::System.Object> ("[70141]", oldValue, value);
					this.OnBundleAuxChanged (oldValue, value);
				}
			}
		}
		
		partial void OnBundleChanging(global::System.Object oldValue, global::System.Object newValue);
		partial void OnBundleChanged(global::System.Object oldValue, global::System.Object newValue);
		partial void OnBundleAuxChanging(global::System.Object oldValue, global::System.Object newValue);
		partial void OnBundleAuxChanged(global::System.Object oldValue, global::System.Object newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Common.Support.Entities.ResourceBaseFileEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Common.Support.Entities.ResourceBaseFileEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (7, 1, 25);	// [701P]
		public static readonly new string EntityStructuredTypeKey = "[701P]";
	}
}
#endregion

#region Epsitec.Common.Support.ResourcePanel Entity
namespace Epsitec.Common.Support.Entities
{
	///	<summary>
	///	The <c>ResourcePanel</c> entity.
	///	designer:cap/701R
	///	</summary>
	public partial class ResourcePanelEntity : global::Epsitec.Common.Support.Entities.ResourceBaseFileEntity
	{
		///	<summary>
		///	The <c>XmlSource</c> field.
		///	designer:fld/701R/701S
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[701S]")]
		public string XmlSource
		{
			get
			{
				return this.GetField<string> ("[701S]");
			}
			set
			{
				string oldValue = this.XmlSource;
				if (oldValue != value || !this.IsFieldDefined("[701S]"))
				{
					this.OnXmlSourceChanging (oldValue, value);
					this.SetField<string> ("[701S]", oldValue, value);
					this.OnXmlSourceChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DefaultSize</c> field.
		///	designer:fld/701R/701T
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[701T]")]
		public string DefaultSize
		{
			get
			{
				return this.GetField<string> ("[701T]");
			}
			set
			{
				string oldValue = this.DefaultSize;
				if (oldValue != value || !this.IsFieldDefined("[701T]"))
				{
					this.OnDefaultSizeChanging (oldValue, value);
					this.SetField<string> ("[701T]", oldValue, value);
					this.OnDefaultSizeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>RootEntityId</c> field.
		///	designer:fld/701R/70121
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[70121]")]
		public global::Epsitec.Common.Support.Druid RootEntityId
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Support.Druid> ("[70121]");
			}
			set
			{
				global::Epsitec.Common.Support.Druid oldValue = this.RootEntityId;
				if (oldValue != value || !this.IsFieldDefined("[70121]"))
				{
					this.OnRootEntityIdChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Support.Druid> ("[70121]", oldValue, value);
					this.OnRootEntityIdChanged (oldValue, value);
				}
			}
		}
		
		partial void OnXmlSourceChanging(string oldValue, string newValue);
		partial void OnXmlSourceChanged(string oldValue, string newValue);
		partial void OnDefaultSizeChanging(string oldValue, string newValue);
		partial void OnDefaultSizeChanged(string oldValue, string newValue);
		partial void OnRootEntityIdChanging(global::Epsitec.Common.Support.Druid oldValue, global::Epsitec.Common.Support.Druid newValue);
		partial void OnRootEntityIdChanged(global::Epsitec.Common.Support.Druid oldValue, global::Epsitec.Common.Support.Druid newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Common.Support.Entities.ResourcePanelEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Common.Support.Entities.ResourcePanelEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (7, 1, 27);	// [701R]
		public static readonly new string EntityStructuredTypeKey = "[701R]";
	}
}
#endregion

#region Epsitec.Common.Support.ResourceForm Entity
namespace Epsitec.Common.Support.Entities
{
	///	<summary>
	///	The <c>ResourceForm</c> entity.
	///	designer:cap/701U
	///	</summary>
	public partial class ResourceFormEntity : global::Epsitec.Common.Support.Entities.ResourceBaseFileEntity
	{
		///	<summary>
		///	The <c>XmlSource</c> field.
		///	designer:fld/701U/701V
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[701V]")]
		public string XmlSource
		{
			get
			{
				return this.GetField<string> ("[701V]");
			}
			set
			{
				string oldValue = this.XmlSource;
				if (oldValue != value || !this.IsFieldDefined("[701V]"))
				{
					this.OnXmlSourceChanging (oldValue, value);
					this.SetField<string> ("[701V]", oldValue, value);
					this.OnXmlSourceChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>XmlSourceAux</c> field.
		///	designer:fld/701U/70131
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[70131]")]
		public string XmlSourceAux
		{
			get
			{
				return this.GetField<string> ("[70131]");
			}
			set
			{
				string oldValue = this.XmlSourceAux;
				if (oldValue != value || !this.IsFieldDefined("[70131]"))
				{
					this.OnXmlSourceAuxChanging (oldValue, value);
					this.SetField<string> ("[70131]", oldValue, value);
					this.OnXmlSourceAuxChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>XmlSourceMerge</c> field.
		///	designer:fld/701U/70151
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[70151]")]
		public string XmlSourceMerge
		{
			get
			{
				return this.GetField<string> ("[70151]");
			}
			set
			{
				string oldValue = this.XmlSourceMerge;
				if (oldValue != value || !this.IsFieldDefined("[70151]"))
				{
					this.OnXmlSourceMergeChanging (oldValue, value);
					this.SetField<string> ("[70151]", oldValue, value);
					this.OnXmlSourceMergeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DefaultSize</c> field.
		///	designer:fld/701U/70101
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[70101]")]
		public string DefaultSize
		{
			get
			{
				return this.GetField<string> ("[70101]");
			}
			set
			{
				string oldValue = this.DefaultSize;
				if (oldValue != value || !this.IsFieldDefined("[70101]"))
				{
					this.OnDefaultSizeChanging (oldValue, value);
					this.SetField<string> ("[70101]", oldValue, value);
					this.OnDefaultSizeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>RootEntityId</c> field.
		///	designer:fld/701U/70111
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[70111]")]
		public global::Epsitec.Common.Support.Druid RootEntityId
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Support.Druid> ("[70111]");
			}
			set
			{
				global::Epsitec.Common.Support.Druid oldValue = this.RootEntityId;
				if (oldValue != value || !this.IsFieldDefined("[70111]"))
				{
					this.OnRootEntityIdChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Support.Druid> ("[70111]", oldValue, value);
					this.OnRootEntityIdChanged (oldValue, value);
				}
			}
		}
		
		partial void OnXmlSourceChanging(string oldValue, string newValue);
		partial void OnXmlSourceChanged(string oldValue, string newValue);
		partial void OnXmlSourceAuxChanging(string oldValue, string newValue);
		partial void OnXmlSourceAuxChanged(string oldValue, string newValue);
		partial void OnXmlSourceMergeChanging(string oldValue, string newValue);
		partial void OnXmlSourceMergeChanged(string oldValue, string newValue);
		partial void OnDefaultSizeChanging(string oldValue, string newValue);
		partial void OnDefaultSizeChanged(string oldValue, string newValue);
		partial void OnRootEntityIdChanging(global::Epsitec.Common.Support.Druid oldValue, global::Epsitec.Common.Support.Druid newValue);
		partial void OnRootEntityIdChanged(global::Epsitec.Common.Support.Druid oldValue, global::Epsitec.Common.Support.Druid newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Common.Support.Entities.ResourceFormEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Common.Support.Entities.ResourceFormEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (7, 1, 30);	// [701U]
		public static readonly new string EntityStructuredTypeKey = "[701U]";
	}
}
#endregion

