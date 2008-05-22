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
				//	designer:cap/9VAE
				public static readonly global::Epsitec.Common.Widgets.Command Edit = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 14));
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
				//	designer:cap/9VAE
				public const long Edit = 0x3E90000A00000EL;
			}
			
			public static class History
			{
				
				//	designer:cap/9VAC
				public const long NavigateNext = 0x3E90000A00000CL;
				//	designer:cap/9VAD
				public const long NavigatePrev = 0x3E90000A00000DL;
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
