//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

namespace Epsitec.Cresus.Core
{
	public static class Res
	{
		//	Code mapping for 'Caption' resources
		
		public static class Types
		{
			//	designer:cap/DVAU
			public static readonly Epsitec.Common.Types.EnumType WorkflowTransitionType = (global::Epsitec.Common.Types.EnumType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 30));
			//	designer:cap/DVA
			public static readonly Epsitec.Common.Types.StructuredType Workflow = (global::Epsitec.Common.Types.StructuredType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 0));
			//	designer:cap/DVA5
			public static readonly Epsitec.Common.Types.StructuredType WorkflowCall = (global::Epsitec.Common.Types.StructuredType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 5));
			//	designer:cap/DVA6
			public static readonly Epsitec.Common.Types.StructuredType WorkflowDefinition = (global::Epsitec.Common.Types.StructuredType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 6));
			//	designer:cap/DVA3
			public static readonly Epsitec.Common.Types.StructuredType WorkflowEdge = (global::Epsitec.Common.Types.StructuredType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 3));
			//	designer:cap/DVA2
			public static readonly Epsitec.Common.Types.StructuredType WorkflowNode = (global::Epsitec.Common.Types.StructuredType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 2));
			//	designer:cap/DVA4
			public static readonly Epsitec.Common.Types.StructuredType WorkflowStep = (global::Epsitec.Common.Types.StructuredType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 4));
			//	designer:cap/DVA1
			public static readonly Epsitec.Common.Types.StructuredType WorkflowThread = (global::Epsitec.Common.Types.StructuredType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 1));
		}
		
		public static class Values
		{
			public static class WorkflowTransitionType
			{
				//	designer:cap/DVA11
				public static global::Epsitec.Common.Types.Caption Call
				{
					get
					{
						return global::Epsitec.Cresus.Core.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 33));
					}
				}
				//	designer:cap/DVAV
				public static global::Epsitec.Common.Types.Caption Default
				{
					get
					{
						return global::Epsitec.Cresus.Core.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 31));
					}
				}
				//	designer:cap/DVA01
				public static global::Epsitec.Common.Types.Caption Fork
				{
					get
					{
						return global::Epsitec.Cresus.Core.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 32));
					}
				}
			}
			
		}
		
		public static class Fields
		{
			public static class Workflow
			{
				//	designer:cap/DVAO
				public static readonly global::Epsitec.Common.Support.Druid Threads = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 24);
			}
			
			public static class WorkflowCall
			{
				//	designer:cap/DVAF
				public static readonly global::Epsitec.Common.Support.Druid Continuation = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 15);
			}
			
			public static class WorkflowDefinition
			{
				//	designer:cap/DVAN
				public static readonly global::Epsitec.Common.Support.Druid SerializedDesign = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 23);
				//	designer:cap/DVAL
				public static readonly global::Epsitec.Common.Support.Druid WorkflowDescription = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 21);
				//	designer:cap/DVAK
				public static readonly global::Epsitec.Common.Support.Druid WorkflowName = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 20);
				//	designer:cap/DVAM
				public static readonly global::Epsitec.Common.Support.Druid WorkflowNodes = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 22);
			}
			
			public static class WorkflowEdge
			{
				//	designer:cap/DVAC
				public static readonly global::Epsitec.Common.Support.Druid Continuation = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 12);
				//	designer:cap/DVAB
				public static readonly global::Epsitec.Common.Support.Druid NextNode = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 11);
				//	designer:cap/DVAE
				public static readonly global::Epsitec.Common.Support.Druid TransitionAction = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 14);
				//	designer:cap/DVAD
				public static readonly global::Epsitec.Common.Support.Druid TransitionType = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 13);
			}
			
			public static class WorkflowNode
			{
				//	designer:cap/DVAA
				public static readonly global::Epsitec.Common.Support.Druid Edges = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 10);
				//	designer:cap/DVA7
				public static readonly global::Epsitec.Common.Support.Druid IsAuto = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 7);
				//	designer:cap/DVA9
				public static readonly global::Epsitec.Common.Support.Druid IsForeign = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 9);
				//	designer:cap/DVA8
				public static readonly global::Epsitec.Common.Support.Druid IsPublic = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 8);
			}
			
			public static class WorkflowStep
			{
				//	designer:cap/DVAR
				public static readonly global::Epsitec.Common.Support.Druid Date = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 27);
				//	designer:cap/DVAP
				public static readonly global::Epsitec.Common.Support.Druid Edge = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 25);
				//	designer:cap/DVAQ
				public static readonly global::Epsitec.Common.Support.Druid Node = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 26);
				//	designer:cap/DVAT
				public static readonly global::Epsitec.Common.Support.Druid OwnerCode = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 29);
				//	designer:cap/DVAS
				public static readonly global::Epsitec.Common.Support.Druid UserCode = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 28);
			}
			
			public static class WorkflowThread
			{
				//	designer:cap/DVAJ
				public static readonly global::Epsitec.Common.Support.Druid CallGraph = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 19);
				//	designer:cap/DVAH
				public static readonly global::Epsitec.Common.Support.Druid Definition = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 17);
				//	designer:cap/DVAI
				public static readonly global::Epsitec.Common.Support.Druid History = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 18);
				//	designer:cap/DVAG
				public static readonly global::Epsitec.Common.Support.Druid Status = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 16);
			}
		}
		
		//	Code mapping for 'String' resources
		
		public static class Strings
		{
			
			public static global::Epsitec.Common.Types.FormattedText GetText(params string[] path)
			{
				string field = string.Join (".", path);
				return new global::Epsitec.Common.Types.FormattedText (_stringsBundle[field].AsString);
			}
			
			public static global::System.String GetString(params string[] path)
			{
				string field = string.Join (".", path);
				return _stringsBundle[field].AsString;
			}
			
			#region Internal Support Code
			
			private static global::Epsitec.Common.Types.FormattedText GetText(string bundle, params string[] path)
			{
				return new global::Epsitec.Common.Types.FormattedText (global::Epsitec.Cresus.Core.Res.Strings.GetString (bundle, path));
			}
			
			private static global::Epsitec.Common.Types.FormattedText GetText(global::Epsitec.Common.Support.Druid druid)
			{
				return new global::Epsitec.Common.Types.FormattedText (global::Epsitec.Cresus.Core.Res.Strings.GetString (druid));
			}
			
			private static global::System.String GetString(string bundle, params string[] path)
			{
				string field = string.Join (".", path);
				return _stringsBundle[field].AsString;
			}
			
			private static global::System.String GetString(global::Epsitec.Common.Support.Druid druid)
			{
				return _stringsBundle[druid].AsString;
			}
			
			private static readonly global::Epsitec.Common.Support.ResourceBundle _stringsBundle = Res._manager.GetBundle ("Strings");
			
			#endregion
		}
		
		static Res()
		{
			Res._manager = new global::Epsitec.Common.Support.ResourceManager (typeof (Res));
			Res._manager.DefineDefaultModuleName ("Cresus.Core.Workflows");
		}
		
		public static void Initialize()
		{
		}
		
		public static global::Epsitec.Common.Support.ResourceManager Manager
		{
			get
			{
				return Res._manager;
			}
		}
		public static int ModuleId
		{
			get
			{
				return Res._moduleId;
			}
		}
		private static readonly global::Epsitec.Common.Support.ResourceManager _manager;
		private const int _moduleId = 1005;
	}
}
