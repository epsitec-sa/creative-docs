//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

namespace Epsitec.Cresus.ServerManager
{
	public static class Res
	{
		//	Code mapping for 'Caption' resources
		
		public static class Commands
		{
			//	designer:cap/AVA2
			public static readonly global::Epsitec.Common.Widgets.Command StartService = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 2));
			//	designer:cap/AVA3
			public static readonly global::Epsitec.Common.Widgets.Command StopService = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 3));
			
			internal static void _Initialize()
			{
			}
		}
		
		public static class CommandIds
		{
			//	designer:cap/AVA2
			public const long StartService = 0x3EA0000A000002L;
			//	designer:cap/AVA3
			public const long StopService = 0x3EA0000A000003L;
		}
		
		
		
		//	Code mapping for 'String' resources
		
		public static class Strings
		{
			
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
		
		//	Code mapping for 'Form' resources
		
		static Res()
		{
			Res._manager = new global::Epsitec.Common.Support.ResourceManager (typeof (Res));
			Res._manager.DefineDefaultModuleName ("App.Server.Manager");
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
		private const int _moduleId = 1002;
	}
}
