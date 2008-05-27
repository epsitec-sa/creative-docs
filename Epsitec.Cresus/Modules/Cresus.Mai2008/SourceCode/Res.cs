//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

namespace Epsitec.Cresus.Mai2008
{
	public static class Res
	{
		//	Code mapping for 'Caption' resources
		
		public static class Commands
		{
			public static class Edition
			{
				internal static void _Initialize()
				{
				}
				
				//	designer:cap/9VAB
				public static readonly global::Epsitec.Common.Widgets.Command Accept = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 11));
				//	designer:cap/9VAA
				public static readonly global::Epsitec.Common.Widgets.Command Cancel = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 10));
				//	designer:cap/9VAR
				public static readonly global::Epsitec.Common.Widgets.Command Delete = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 27));
				//	designer:cap/9VAE
				public static readonly global::Epsitec.Common.Widgets.Command Edit = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 14));
				//	designer:cap/9VAQ
				public static readonly global::Epsitec.Common.Widgets.Command New = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 26));
			}
			
			public static class History
			{
				internal static void _Initialize()
				{
				}
				
				//	designer:cap/9VAC
				public static readonly global::Epsitec.Common.Widgets.Command NavigateNext = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 12));
				//	designer:cap/9VAD
				public static readonly global::Epsitec.Common.Widgets.Command NavigatePrev = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 13));
			}
			
			public static class Quick
			{
				internal static void _Initialize()
				{
				}
				
				//	designer:cap/9VAS
				public static readonly global::Epsitec.Common.Widgets.Command CreateBillForCustomer = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 28));
			}
			
			public static class SwitchToBase
			{
				internal static void _Initialize()
				{
				}
				
				//	designer:cap/9VA5
				public static readonly global::Epsitec.Common.Widgets.Command BillIn = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 5));
				//	designer:cap/9VA6
				public static readonly global::Epsitec.Common.Widgets.Command BillOut = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 6));
				//	designer:cap/9VA7
				public static readonly global::Epsitec.Common.Widgets.Command Customers = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 7));
				//	designer:cap/9VA8
				public static readonly global::Epsitec.Common.Widgets.Command Items = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 8));
				//	designer:cap/9VA9
				public static readonly global::Epsitec.Common.Widgets.Command Suppliers = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 9));
			}
			
			internal static void _Initialize()
			{
				Edition._Initialize ();
				History._Initialize ();
				Quick._Initialize ();
				SwitchToBase._Initialize ();
			}
		}
		
		public static class CommandIds
		{
			public static class Edition
			{
				
				//	designer:cap/9VAB
				public const long Accept = 0x3E90000A00000BL;
				//	designer:cap/9VAA
				public const long Cancel = 0x3E90000A00000AL;
				//	designer:cap/9VAR
				public const long Delete = 0x3E90000A00001BL;
				//	designer:cap/9VAE
				public const long Edit = 0x3E90000A00000EL;
				//	designer:cap/9VAQ
				public const long New = 0x3E90000A00001AL;
			}
			
			public static class History
			{
				
				//	designer:cap/9VAC
				public const long NavigateNext = 0x3E90000A00000CL;
				//	designer:cap/9VAD
				public const long NavigatePrev = 0x3E90000A00000DL;
			}
			
			public static class Quick
			{
				
				//	designer:cap/9VAS
				public const long CreateBillForCustomer = 0x3E90000A00001CL;
			}
			
			public static class SwitchToBase
			{
				
				//	designer:cap/9VA5
				public const long BillIn = 0x3E90000A000005L;
				//	designer:cap/9VA6
				public const long BillOut = 0x3E90000A000006L;
				//	designer:cap/9VA7
				public const long Customers = 0x3E90000A000007L;
				//	designer:cap/9VA8
				public const long Items = 0x3E90000A000008L;
				//	designer:cap/9VA9
				public const long Suppliers = 0x3E90000A000009L;
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
		
		//	Code mapping for 'Form' resources
		
		//	Code mapping for 'Form' resources
		
		//	Code mapping for 'Form' resources
		
		static Res()
		{
			Res._manager = new global::Epsitec.Common.Support.ResourceManager (typeof (Res));
			Res._manager.DefineDefaultModuleName ("Cresus.Mai2008");
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
		private const int _moduleId = 1001;
	}
}
