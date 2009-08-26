//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

namespace Epsitec.Cresus
{
	public static class Res
	{
		//	Code mapping for 'Caption' resources
		
		public static class Commands
		{
			public static class GraphType
			{
				internal static void _Initialize()
				{
				}
				
				//	designer:cap/BVA1
				public static readonly global::Epsitec.Common.Widgets.Command UseBarChartHorizontal = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 1));
				//	designer:cap/BVA2
				public static readonly global::Epsitec.Common.Widgets.Command UseBarChartVertical = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 2));
				//	designer:cap/BVA
				public static readonly global::Epsitec.Common.Widgets.Command UseLineChart = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 0));
			}
			
			internal static void _Initialize()
			{
				GraphType._Initialize ();
			}
		}
		
		public static class CommandIds
		{
			public static class GraphType
			{
				
				//	designer:cap/BVA1
				public const long UseBarChartHorizontal = 0x3EB0000A000001L;
				//	designer:cap/BVA2
				public const long UseBarChartVertical = 0x3EB0000A000002L;
				//	designer:cap/BVA
				public const long UseLineChart = 0x3EB0000A000000L;
			}
		}
		
		//	Code mapping for 'String' resources
		
		public static class Strings
		{
			public static class Application
			{
				//	designer:str/BVA
				public static global::Epsitec.Common.Types.FormattedText Name
				{
					get
					{
						return global::Epsitec.Cresus.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (167772160));
					}
				}
			}
			
			public static class DataPicker
			{
				//	designer:str/BVA1
				public static global::Epsitec.Common.Types.FormattedText Title
				{
					get
					{
						return global::Epsitec.Cresus.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (167772161));
					}
				}
			}
			
			public static global::Epsitec.Common.Types.FormattedText GetText(params string[] path)
			{
				string field = string.Join (".", path);
				return new global::Epsitec.Common.Types.FormattedText (_stringsBundle[field].AsString);
			}
			
			#region Internal Support Code
			
			private static global::Epsitec.Common.Types.FormattedText GetText(string bundle, params string[] path)
			{
				string field = string.Join (".", path);
				return new global::Epsitec.Common.Types.FormattedText (_stringsBundle[field].AsString);
			}
			
			private static global::Epsitec.Common.Types.FormattedText GetText(global::Epsitec.Common.Support.Druid druid)
			{
				return new global::Epsitec.Common.Types.FormattedText (_stringsBundle[druid].AsString);
			}
			
			private static readonly global::Epsitec.Common.Support.ResourceBundle _stringsBundle = Res._manager.GetBundle ("Strings");
			
			#endregion
		}
		
		static Res()
		{
			Res._manager = new global::Epsitec.Common.Support.ResourceManager (typeof (Res));
			Res._manager.DefineDefaultModuleName ("Cresus.Graph");
			Commands._Initialize ();
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
		private const int _moduleId = 1003;
	}
}
