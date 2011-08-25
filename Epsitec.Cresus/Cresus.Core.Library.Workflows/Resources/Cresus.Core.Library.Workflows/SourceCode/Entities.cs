//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

[assembly: global::Epsitec.Common.Support.EntityClass ("[DVA]", typeof (Epsitec.Cresus.Core.Entities.WorkflowEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[DVA1]", typeof (Epsitec.Cresus.Core.Entities.WorkflowThreadEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[DVA2]", typeof (Epsitec.Cresus.Core.Entities.WorkflowNodeEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[DVA3]", typeof (Epsitec.Cresus.Core.Entities.WorkflowEdgeEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[DVA4]", typeof (Epsitec.Cresus.Core.Entities.WorkflowStepEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[DVA5]", typeof (Epsitec.Cresus.Core.Entities.WorkflowCallEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[DVA6]", typeof (Epsitec.Cresus.Core.Entities.WorkflowDefinitionEntity))]
#region Epsitec.Cresus.Core.Workflow Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>Workflow</c> entity.
	///	designer:cap/DVA
	///	</summary>
	public partial class WorkflowEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.IItemCode
	{
		#region IItemCode Members
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/DVA/8VA5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA5]")]
		public string Code
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IItemCodeInterfaceImplementation.GetCode (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IItemCodeInterfaceImplementation.SetCode (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>Threads</c> field.
		///	designer:fld/DVA/DVAO
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[DVAO]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.WorkflowThreadEntity> Threads
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.WorkflowThreadEntity> ("[DVAO]");
			}
		}
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.WorkflowEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.WorkflowEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1005, 10, 0);	// [DVA]
		public static readonly string EntityStructuredTypeKey = "[DVA]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<WorkflowEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Volatile)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.WorkflowThread Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>WorkflowThread</c> entity.
	///	designer:cap/DVA1
	///	</summary>
	public partial class WorkflowThreadEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.IItemCode
	{
		#region IItemCode Members
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/DVA1/8VA5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA5]")]
		public string Code
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IItemCodeInterfaceImplementation.GetCode (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IItemCodeInterfaceImplementation.SetCode (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>Status</c> field.
		///	designer:fld/DVA1/DVAG
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[DVAG]")]
		public global::Epsitec.Cresus.Core.Business.WorkflowState Status
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.WorkflowState> ("[DVAG]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.WorkflowState oldValue = this.Status;
				if (oldValue != value || !this.IsFieldDefined("[DVAG]"))
				{
					this.OnStatusChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.WorkflowState> ("[DVAG]", oldValue, value);
					this.OnStatusChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Definition</c> field.
		///	designer:fld/DVA1/DVAH
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[DVAH]")]
		public global::Epsitec.Cresus.Core.Entities.WorkflowDefinitionEntity Definition
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.WorkflowDefinitionEntity> ("[DVAH]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.WorkflowDefinitionEntity oldValue = this.Definition;
				if (oldValue != value || !this.IsFieldDefined("[DVAH]"))
				{
					this.OnDefinitionChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.WorkflowDefinitionEntity> ("[DVAH]", oldValue, value);
					this.OnDefinitionChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>History</c> field.
		///	designer:fld/DVA1/DVAI
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[DVAI]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.WorkflowStepEntity> History
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.WorkflowStepEntity> ("[DVAI]");
			}
		}
		///	<summary>
		///	The <c>CallGraph</c> field.
		///	designer:fld/DVA1/DVAJ
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[DVAJ]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.WorkflowCallEntity> CallGraph
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.WorkflowCallEntity> ("[DVAJ]");
			}
		}
		///	<summary>
		///	The <c>RestrictedUserCode</c> field.
		///	designer:fld/DVA1/DVAD1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[DVAD1]")]
		public string RestrictedUserCode
		{
			get
			{
				return this.GetField<string> ("[DVAD1]");
			}
			set
			{
				string oldValue = this.RestrictedUserCode;
				if (oldValue != value || !this.IsFieldDefined("[DVAD1]"))
				{
					this.OnRestrictedUserCodeChanging (oldValue, value);
					this.SetField<string> ("[DVAD1]", oldValue, value);
					this.OnRestrictedUserCodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>SerializedArgs</c> field.
		///	designer:fld/DVA1/DVAB1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[DVAB1]")]
		public global::Epsitec.Cresus.Core.Entities.XmlBlobEntity SerializedArgs
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.XmlBlobEntity> ("[DVAB1]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.XmlBlobEntity oldValue = this.SerializedArgs;
				if (oldValue != value || !this.IsFieldDefined("[DVAB1]"))
				{
					this.OnSerializedArgsChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.XmlBlobEntity> ("[DVAB1]", oldValue, value);
					this.OnSerializedArgsChanged (oldValue, value);
				}
			}
		}
		
		partial void OnStatusChanging(global::Epsitec.Cresus.Core.Business.WorkflowState oldValue, global::Epsitec.Cresus.Core.Business.WorkflowState newValue);
		partial void OnStatusChanged(global::Epsitec.Cresus.Core.Business.WorkflowState oldValue, global::Epsitec.Cresus.Core.Business.WorkflowState newValue);
		partial void OnDefinitionChanging(global::Epsitec.Cresus.Core.Entities.WorkflowDefinitionEntity oldValue, global::Epsitec.Cresus.Core.Entities.WorkflowDefinitionEntity newValue);
		partial void OnDefinitionChanged(global::Epsitec.Cresus.Core.Entities.WorkflowDefinitionEntity oldValue, global::Epsitec.Cresus.Core.Entities.WorkflowDefinitionEntity newValue);
		partial void OnRestrictedUserCodeChanging(string oldValue, string newValue);
		partial void OnRestrictedUserCodeChanged(string oldValue, string newValue);
		partial void OnSerializedArgsChanging(global::Epsitec.Cresus.Core.Entities.XmlBlobEntity oldValue, global::Epsitec.Cresus.Core.Entities.XmlBlobEntity newValue);
		partial void OnSerializedArgsChanged(global::Epsitec.Cresus.Core.Entities.XmlBlobEntity oldValue, global::Epsitec.Cresus.Core.Entities.XmlBlobEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.WorkflowThreadEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.WorkflowThreadEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1005, 10, 1);	// [DVA1]
		public static readonly string EntityStructuredTypeKey = "[DVA1]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<WorkflowThreadEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Volatile)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.WorkflowNode Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>WorkflowNode</c> entity.
	///	designer:cap/DVA2
	///	</summary>
	public partial class WorkflowNodeEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ICategory
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/DVA2/8VA3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA3]")]
		public bool IsArchive
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.ILifetimeInterfaceImplementation.GetIsArchive (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.ILifetimeInterfaceImplementation.SetIsArchive (this, value);
			}
		}
		#endregion
		#region IItemCode Members
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/DVA2/8VA5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA5]")]
		public string Code
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IItemCodeInterfaceImplementation.GetCode (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IItemCodeInterfaceImplementation.SetCode (this, value);
			}
		}
		#endregion
		#region INameDescription Members
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/DVA2/8VA7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA7]")]
		public global::Epsitec.Common.Types.FormattedText Name
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.INameDescriptionInterfaceImplementation.GetName (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.INameDescriptionInterfaceImplementation.SetName (this, value);
			}
		}
		///	<summary>
		///	The <c>Description</c> field.
		///	designer:fld/DVA2/8VA8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA8]")]
		public global::Epsitec.Common.Types.FormattedText Description
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.INameDescriptionInterfaceImplementation.GetDescription (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.INameDescriptionInterfaceImplementation.SetDescription (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>IsAuto</c> field.
		///	designer:fld/DVA2/DVA7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[DVA7]")]
		public bool IsAuto
		{
			get
			{
				return this.GetField<bool> ("[DVA7]");
			}
			set
			{
				bool oldValue = this.IsAuto;
				if (oldValue != value || !this.IsFieldDefined("[DVA7]"))
				{
					this.OnIsAutoChanging (oldValue, value);
					this.SetField<bool> ("[DVA7]", oldValue, value);
					this.OnIsAutoChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>IsPublic</c> field.
		///	designer:fld/DVA2/DVA8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[DVA8]")]
		public bool IsPublic
		{
			get
			{
				return this.GetField<bool> ("[DVA8]");
			}
			set
			{
				bool oldValue = this.IsPublic;
				if (oldValue != value || !this.IsFieldDefined("[DVA8]"))
				{
					this.OnIsPublicChanging (oldValue, value);
					this.SetField<bool> ("[DVA8]", oldValue, value);
					this.OnIsPublicChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>IsForeign</c> field.
		///	designer:fld/DVA2/DVA9
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[DVA9]")]
		public bool IsForeign
		{
			get
			{
				return this.GetField<bool> ("[DVA9]");
			}
			set
			{
				bool oldValue = this.IsForeign;
				if (oldValue != value || !this.IsFieldDefined("[DVA9]"))
				{
					this.OnIsForeignChanging (oldValue, value);
					this.SetField<bool> ("[DVA9]", oldValue, value);
					this.OnIsForeignChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Edges</c> field.
		///	designer:fld/DVA2/DVAA
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[DVAA]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.WorkflowEdgeEntity> Edges
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.WorkflowEdgeEntity> ("[DVAA]");
			}
		}
		
		partial void OnIsAutoChanging(bool oldValue, bool newValue);
		partial void OnIsAutoChanged(bool oldValue, bool newValue);
		partial void OnIsPublicChanging(bool oldValue, bool newValue);
		partial void OnIsPublicChanged(bool oldValue, bool newValue);
		partial void OnIsForeignChanging(bool oldValue, bool newValue);
		partial void OnIsForeignChanged(bool oldValue, bool newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.WorkflowNodeEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.WorkflowNodeEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1005, 10, 2);	// [DVA2]
		public static readonly string EntityStructuredTypeKey = "[DVA2]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<WorkflowNodeEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.WorkflowEdge Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>WorkflowEdge</c> entity.
	///	designer:cap/DVA3
	///	</summary>
	public partial class WorkflowEdgeEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ICategory
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/DVA3/8VA3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA3]")]
		public bool IsArchive
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.ILifetimeInterfaceImplementation.GetIsArchive (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.ILifetimeInterfaceImplementation.SetIsArchive (this, value);
			}
		}
		#endregion
		#region IItemCode Members
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/DVA3/8VA5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA5]")]
		public string Code
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IItemCodeInterfaceImplementation.GetCode (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IItemCodeInterfaceImplementation.SetCode (this, value);
			}
		}
		#endregion
		#region INameDescription Members
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/DVA3/8VA7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA7]")]
		public global::Epsitec.Common.Types.FormattedText Name
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.INameDescriptionInterfaceImplementation.GetName (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.INameDescriptionInterfaceImplementation.SetName (this, value);
			}
		}
		///	<summary>
		///	The <c>Description</c> field.
		///	designer:fld/DVA3/8VA8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA8]")]
		public global::Epsitec.Common.Types.FormattedText Description
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.INameDescriptionInterfaceImplementation.GetDescription (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.INameDescriptionInterfaceImplementation.SetDescription (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>NextNode</c> field.
		///	designer:fld/DVA3/DVAB
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[DVAB]")]
		public global::Epsitec.Cresus.Core.Entities.WorkflowNodeEntity NextNode
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.WorkflowNodeEntity> ("[DVAB]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.WorkflowNodeEntity oldValue = this.NextNode;
				if (oldValue != value || !this.IsFieldDefined("[DVAB]"))
				{
					this.OnNextNodeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.WorkflowNodeEntity> ("[DVAB]", oldValue, value);
					this.OnNextNodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Continuation</c> field.
		///	designer:fld/DVA3/DVAC
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[DVAC]")]
		public global::Epsitec.Cresus.Core.Entities.WorkflowNodeEntity Continuation
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.WorkflowNodeEntity> ("[DVAC]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.WorkflowNodeEntity oldValue = this.Continuation;
				if (oldValue != value || !this.IsFieldDefined("[DVAC]"))
				{
					this.OnContinuationChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.WorkflowNodeEntity> ("[DVAC]", oldValue, value);
					this.OnContinuationChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>TransitionType</c> field.
		///	designer:fld/DVA3/DVAD
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[DVAD]")]
		public global::Epsitec.Cresus.Core.Business.WorkflowTransitionType TransitionType
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.WorkflowTransitionType> ("[DVAD]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.WorkflowTransitionType oldValue = this.TransitionType;
				if (oldValue != value || !this.IsFieldDefined("[DVAD]"))
				{
					this.OnTransitionTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.WorkflowTransitionType> ("[DVAD]", oldValue, value);
					this.OnTransitionTypeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>TransitionAction</c> field.
		///	designer:fld/DVA3/DVAE
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[DVAE]")]
		public string TransitionAction
		{
			get
			{
				return this.GetField<string> ("[DVAE]");
			}
			set
			{
				string oldValue = this.TransitionAction;
				if (oldValue != value || !this.IsFieldDefined("[DVAE]"))
				{
					this.OnTransitionActionChanging (oldValue, value);
					this.SetField<string> ("[DVAE]", oldValue, value);
					this.OnTransitionActionChanged (oldValue, value);
				}
			}
		}
		
		partial void OnNextNodeChanging(global::Epsitec.Cresus.Core.Entities.WorkflowNodeEntity oldValue, global::Epsitec.Cresus.Core.Entities.WorkflowNodeEntity newValue);
		partial void OnNextNodeChanged(global::Epsitec.Cresus.Core.Entities.WorkflowNodeEntity oldValue, global::Epsitec.Cresus.Core.Entities.WorkflowNodeEntity newValue);
		partial void OnContinuationChanging(global::Epsitec.Cresus.Core.Entities.WorkflowNodeEntity oldValue, global::Epsitec.Cresus.Core.Entities.WorkflowNodeEntity newValue);
		partial void OnContinuationChanged(global::Epsitec.Cresus.Core.Entities.WorkflowNodeEntity oldValue, global::Epsitec.Cresus.Core.Entities.WorkflowNodeEntity newValue);
		partial void OnTransitionTypeChanging(global::Epsitec.Cresus.Core.Business.WorkflowTransitionType oldValue, global::Epsitec.Cresus.Core.Business.WorkflowTransitionType newValue);
		partial void OnTransitionTypeChanged(global::Epsitec.Cresus.Core.Business.WorkflowTransitionType oldValue, global::Epsitec.Cresus.Core.Business.WorkflowTransitionType newValue);
		partial void OnTransitionActionChanging(string oldValue, string newValue);
		partial void OnTransitionActionChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.WorkflowEdgeEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.WorkflowEdgeEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1005, 10, 3);	// [DVA3]
		public static readonly string EntityStructuredTypeKey = "[DVA3]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<WorkflowEdgeEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.WorkflowStep Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>WorkflowStep</c> entity.
	///	designer:cap/DVA4
	///	</summary>
	public partial class WorkflowStepEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Edge</c> field.
		///	designer:fld/DVA4/DVAP
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[DVAP]")]
		public global::Epsitec.Cresus.Core.Entities.WorkflowEdgeEntity Edge
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.WorkflowEdgeEntity> ("[DVAP]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.WorkflowEdgeEntity oldValue = this.Edge;
				if (oldValue != value || !this.IsFieldDefined("[DVAP]"))
				{
					this.OnEdgeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.WorkflowEdgeEntity> ("[DVAP]", oldValue, value);
					this.OnEdgeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Node</c> field.
		///	designer:fld/DVA4/DVAQ
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[DVAQ]")]
		public global::Epsitec.Cresus.Core.Entities.WorkflowNodeEntity Node
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.WorkflowNodeEntity> ("[DVAQ]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.WorkflowNodeEntity oldValue = this.Node;
				if (oldValue != value || !this.IsFieldDefined("[DVAQ]"))
				{
					this.OnNodeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.WorkflowNodeEntity> ("[DVAQ]", oldValue, value);
					this.OnNodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Date</c> field.
		///	designer:fld/DVA4/DVAR
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[DVAR]")]
		public global::System.DateTime Date
		{
			get
			{
				return this.GetField<global::System.DateTime> ("[DVAR]");
			}
			set
			{
				global::System.DateTime oldValue = this.Date;
				if (oldValue != value || !this.IsFieldDefined("[DVAR]"))
				{
					this.OnDateChanging (oldValue, value);
					this.SetField<global::System.DateTime> ("[DVAR]", oldValue, value);
					this.OnDateChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ExecutingUserCode</c> field.
		///	designer:fld/DVA4/DVAS
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[DVAS]")]
		public string ExecutingUserCode
		{
			get
			{
				return this.GetField<string> ("[DVAS]");
			}
			set
			{
				string oldValue = this.ExecutingUserCode;
				if (oldValue != value || !this.IsFieldDefined("[DVAS]"))
				{
					this.OnExecutingUserCodeChanging (oldValue, value);
					this.SetField<string> ("[DVAS]", oldValue, value);
					this.OnExecutingUserCodeChanged (oldValue, value);
				}
			}
		}
		
		partial void OnEdgeChanging(global::Epsitec.Cresus.Core.Entities.WorkflowEdgeEntity oldValue, global::Epsitec.Cresus.Core.Entities.WorkflowEdgeEntity newValue);
		partial void OnEdgeChanged(global::Epsitec.Cresus.Core.Entities.WorkflowEdgeEntity oldValue, global::Epsitec.Cresus.Core.Entities.WorkflowEdgeEntity newValue);
		partial void OnNodeChanging(global::Epsitec.Cresus.Core.Entities.WorkflowNodeEntity oldValue, global::Epsitec.Cresus.Core.Entities.WorkflowNodeEntity newValue);
		partial void OnNodeChanged(global::Epsitec.Cresus.Core.Entities.WorkflowNodeEntity oldValue, global::Epsitec.Cresus.Core.Entities.WorkflowNodeEntity newValue);
		partial void OnDateChanging(global::System.DateTime oldValue, global::System.DateTime newValue);
		partial void OnDateChanged(global::System.DateTime oldValue, global::System.DateTime newValue);
		partial void OnExecutingUserCodeChanging(string oldValue, string newValue);
		partial void OnExecutingUserCodeChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.WorkflowStepEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.WorkflowStepEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1005, 10, 4);	// [DVA4]
		public static readonly string EntityStructuredTypeKey = "[DVA4]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<WorkflowStepEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.WorkflowCall Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>WorkflowCall</c> entity.
	///	designer:cap/DVA5
	///	</summary>
	public partial class WorkflowCallEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Continuation</c> field.
		///	designer:fld/DVA5/DVAF
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[DVAF]")]
		public global::Epsitec.Cresus.Core.Entities.WorkflowNodeEntity Continuation
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.WorkflowNodeEntity> ("[DVAF]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.WorkflowNodeEntity oldValue = this.Continuation;
				if (oldValue != value || !this.IsFieldDefined("[DVAF]"))
				{
					this.OnContinuationChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.WorkflowNodeEntity> ("[DVAF]", oldValue, value);
					this.OnContinuationChanged (oldValue, value);
				}
			}
		}
		
		partial void OnContinuationChanging(global::Epsitec.Cresus.Core.Entities.WorkflowNodeEntity oldValue, global::Epsitec.Cresus.Core.Entities.WorkflowNodeEntity newValue);
		partial void OnContinuationChanged(global::Epsitec.Cresus.Core.Entities.WorkflowNodeEntity oldValue, global::Epsitec.Cresus.Core.Entities.WorkflowNodeEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.WorkflowCallEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.WorkflowCallEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1005, 10, 5);	// [DVA5]
		public static readonly string EntityStructuredTypeKey = "[DVA5]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<WorkflowCallEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.WorkflowDefinition Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>WorkflowDefinition</c> entity.
	///	designer:cap/DVA6
	///	</summary>
	public partial class WorkflowDefinitionEntity : global::Epsitec.Cresus.Core.Entities.WorkflowNodeEntity
	{
		///	<summary>
		///	The <c>WorkflowName</c> field.
		///	designer:fld/DVA6/DVAK
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[DVAK]")]
		public global::Epsitec.Common.Types.FormattedText WorkflowName
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[DVAK]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.WorkflowName;
				if (oldValue != value || !this.IsFieldDefined("[DVAK]"))
				{
					this.OnWorkflowNameChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[DVAK]", oldValue, value);
					this.OnWorkflowNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>WorkflowDescription</c> field.
		///	designer:fld/DVA6/DVAL
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[DVAL]")]
		public global::Epsitec.Common.Types.FormattedText WorkflowDescription
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[DVAL]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.WorkflowDescription;
				if (oldValue != value || !this.IsFieldDefined("[DVAL]"))
				{
					this.OnWorkflowDescriptionChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[DVAL]", oldValue, value);
					this.OnWorkflowDescriptionChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>WorkflowNodes</c> field.
		///	designer:fld/DVA6/DVAM
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[DVAM]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.WorkflowNodeEntity> WorkflowNodes
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.WorkflowNodeEntity> ("[DVAM]");
			}
		}
		///	<summary>
		///	The <c>SerializedDesign</c> field.
		///	designer:fld/DVA6/DVAN
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[DVAN]")]
		public global::Epsitec.Cresus.Core.Entities.XmlBlobEntity SerializedDesign
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.XmlBlobEntity> ("[DVAN]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.XmlBlobEntity oldValue = this.SerializedDesign;
				if (oldValue != value || !this.IsFieldDefined("[DVAN]"))
				{
					this.OnSerializedDesignChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.XmlBlobEntity> ("[DVAN]", oldValue, value);
					this.OnSerializedDesignChanged (oldValue, value);
				}
			}
		}
		
		partial void OnWorkflowNameChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnWorkflowNameChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnWorkflowDescriptionChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnWorkflowDescriptionChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnSerializedDesignChanging(global::Epsitec.Cresus.Core.Entities.XmlBlobEntity oldValue, global::Epsitec.Cresus.Core.Entities.XmlBlobEntity newValue);
		partial void OnSerializedDesignChanged(global::Epsitec.Cresus.Core.Entities.XmlBlobEntity oldValue, global::Epsitec.Cresus.Core.Entities.XmlBlobEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.WorkflowDefinitionEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.WorkflowDefinitionEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1005, 10, 6);	// [DVA6]
		public static readonly new string EntityStructuredTypeKey = "[DVA6]";
		
		#region Repository Class
		public new partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<WorkflowDefinitionEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.IWorkflowHost Interface
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>IWorkflowHost</c> entity.
	///	designer:cap/DVA21
	///	</summary>
	public interface IWorkflowHost
	{
		///	<summary>
		///	The <c>Workflow</c> field.
		///	designer:fld/DVA21/DVA31
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[DVA31]")]
		global::Epsitec.Cresus.Core.Entities.WorkflowEntity Workflow
		{
			get;
			set;
		}
	}
	public static partial class IWorkflowHostInterfaceImplementation
	{
		public static global::Epsitec.Cresus.Core.Entities.WorkflowEntity GetWorkflow(global::Epsitec.Cresus.Core.Entities.IWorkflowHost obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<global::Epsitec.Cresus.Core.Entities.WorkflowEntity> ("[DVA31]");
		}
		public static void SetWorkflow(global::Epsitec.Cresus.Core.Entities.IWorkflowHost obj, global::Epsitec.Cresus.Core.Entities.WorkflowEntity value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			global::Epsitec.Cresus.Core.Entities.WorkflowEntity oldValue = obj.Workflow;
			if (oldValue != value || !entity.IsFieldDefined("[DVA31]"))
			{
				IWorkflowHostInterfaceImplementation.OnWorkflowChanging (obj, oldValue, value);
				entity.SetField<global::Epsitec.Cresus.Core.Entities.WorkflowEntity> ("[DVA31]", oldValue, value);
				IWorkflowHostInterfaceImplementation.OnWorkflowChanged (obj, oldValue, value);
			}
		}
		static partial void OnWorkflowChanged(global::Epsitec.Cresus.Core.Entities.IWorkflowHost obj, global::Epsitec.Cresus.Core.Entities.WorkflowEntity oldValue, global::Epsitec.Cresus.Core.Entities.WorkflowEntity newValue);
		static partial void OnWorkflowChanging(global::Epsitec.Cresus.Core.Entities.IWorkflowHost obj, global::Epsitec.Cresus.Core.Entities.WorkflowEntity oldValue, global::Epsitec.Cresus.Core.Entities.WorkflowEntity newValue);
	}
}
#endregion

