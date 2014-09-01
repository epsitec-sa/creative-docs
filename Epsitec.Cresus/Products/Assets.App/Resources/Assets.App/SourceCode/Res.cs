//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

namespace Epsitec.Cresus.Assets.App
{
	public static class Res
	{
		//	Code mapping for 'Caption' resources
		
		public static class Commands
		{
			public static class Edit
			{
				internal static void _Initialize()
				{
					global::System.Object.Equals (Edit.Accept, null);
				}
				
				//	designer:cap/JUK6001
				public static readonly global::Epsitec.Common.Widgets.Command Accept = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 6));
				//	designer:cap/JUK7001
				public static readonly global::Epsitec.Common.Widgets.Command Cancel = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 7));
			}
			
			public static class View
			{
				internal static void _Initialize()
				{
					global::System.Object.Equals (View.Categories, null);
				}
				
				//	designer:cap/JUK1001
				public static readonly global::Epsitec.Common.Widgets.Command Categories = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 1));
				//	designer:cap/JUK3001
				public static readonly global::Epsitec.Common.Widgets.Command Events = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 3));
				//	designer:cap/JUK2001
				public static readonly global::Epsitec.Common.Widgets.Command Groups = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 2));
				//	designer:cap/JUK0001
				public static readonly global::Epsitec.Common.Widgets.Command Objects = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 0));
				//	designer:cap/JUK4001
				public static readonly global::Epsitec.Common.Widgets.Command Reports = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 4));
				//	designer:cap/JUK5001
				public static readonly global::Epsitec.Common.Widgets.Command Settings = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 5));
			}
		}
		
		public static class CommandIds
		{
			public static class Edit
			{
				//	designer:cap/JUK6001
				public const long Accept = 0x7D300014000006L;
				//	designer:cap/JUK7001
				public const long Cancel = 0x7D300014000007L;
			}
			
			public static class View
			{
				//	designer:cap/JUK1001
				public const long Categories = 0x7D300014000001L;
				//	designer:cap/JUK3001
				public const long Events = 0x7D300014000003L;
				//	designer:cap/JUK2001
				public const long Groups = 0x7D300014000002L;
				//	designer:cap/JUK0001
				public const long Objects = 0x7D300014000000L;
				//	designer:cap/JUK4001
				public const long Reports = 0x7D300014000004L;
				//	designer:cap/JUK5001
				public const long Settings = 0x7D300014000005L;
			}
			
		}
		
		public static class Types
		{
			internal static void _Initialize()
			{
				global::System.Object.Equals (DateType, null);
			}
			
			//	designer:cap/JUKS101
			public static readonly Epsitec.Common.Types.EnumType DateType = (global::Epsitec.Common.Types.EnumType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 60));
			//	designer:cap/JUKS001
			public static readonly Epsitec.Common.Types.EnumType ExportColor = (global::Epsitec.Common.Types.EnumType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 28));
			//	designer:cap/JUKC001
			public static readonly Epsitec.Common.Types.EnumType ExportFormat = (global::Epsitec.Common.Types.EnumType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 12));
			//	designer:cap/JUKK001
			public static readonly Epsitec.Common.Types.EnumType PageFormatType = (global::Epsitec.Common.Types.EnumType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 20));
			//	designer:cap/JUK9101
			public static readonly Epsitec.Common.Types.EnumType PfdPredefinedStyle = (global::Epsitec.Common.Types.EnumType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 41));
			//	designer:cap/JUKN101
			public static readonly Epsitec.Common.Types.EnumType ReportType = (global::Epsitec.Common.Types.EnumType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 55));
		}
		
		public static class Values
		{
			public static class DateType
			{
				//	designer:cap/JUK8201
				public static global::Epsitec.Common.Types.Caption BeginCurrentMonth
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 72));
					}
				}
				//	designer:cap/JUK2201
				public static global::Epsitec.Common.Types.Caption BeginCurrentYear
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 66));
					}
				}
				//	designer:cap/JUKV101
				public static global::Epsitec.Common.Types.Caption BeginMandat
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 63));
					}
				}
				//	designer:cap/JUKA201
				public static global::Epsitec.Common.Types.Caption BeginNextMonth
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 74));
					}
				}
				//	designer:cap/JUK4201
				public static global::Epsitec.Common.Types.Caption BeginNextYear
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 68));
					}
				}
				//	designer:cap/JUK6201
				public static global::Epsitec.Common.Types.Caption BeginPreviousMonth
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 70));
					}
				}
				//	designer:cap/JUK0201
				public static global::Epsitec.Common.Types.Caption BeginPreviousYear
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 64));
					}
				}
				//	designer:cap/JUK9201
				public static global::Epsitec.Common.Types.Caption EndCurrentMonth
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 73));
					}
				}
				//	designer:cap/JUK3201
				public static global::Epsitec.Common.Types.Caption EndCurrentYear
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 67));
					}
				}
				//	designer:cap/JUKB201
				public static global::Epsitec.Common.Types.Caption EndNextMonth
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 75));
					}
				}
				//	designer:cap/JUK5201
				public static global::Epsitec.Common.Types.Caption EndNextYear
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 69));
					}
				}
				//	designer:cap/JUK7201
				public static global::Epsitec.Common.Types.Caption EndPreviousMonth
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 71));
					}
				}
				//	designer:cap/JUK1201
				public static global::Epsitec.Common.Types.Caption EndPreviousYear
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 65));
					}
				}
				//	designer:cap/JUKC201
				public static global::Epsitec.Common.Types.Caption Now
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 76));
					}
				}
				//	designer:cap/JUKU101
				public static global::Epsitec.Common.Types.Caption Separator
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 62));
					}
				}
				//	designer:cap/JUKT101
				public static global::Epsitec.Common.Types.Caption Unknown
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 61));
					}
				}
			}
			
			public static class ExportColor
			{
				//	designer:cap/JUK3101
				public static global::Epsitec.Common.Types.Caption Black
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 35));
					}
				}
				//	designer:cap/JUK2101
				public static global::Epsitec.Common.Types.Caption DarkGrey
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 34));
					}
				}
				//	designer:cap/JUK1101
				public static global::Epsitec.Common.Types.Caption Grey
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 33));
					}
				}
				//	designer:cap/JUK6101
				public static global::Epsitec.Common.Types.Caption LightBlue
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 38));
					}
				}
				//	designer:cap/JUK5101
				public static global::Epsitec.Common.Types.Caption LightGreen
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 37));
					}
				}
				//	designer:cap/JUK0101
				public static global::Epsitec.Common.Types.Caption LightGrey
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 32));
					}
				}
				//	designer:cap/JUK8101
				public static global::Epsitec.Common.Types.Caption LightPurple
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 40));
					}
				}
				//	designer:cap/JUK4101
				public static global::Epsitec.Common.Types.Caption LightRed
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 36));
					}
				}
				//	designer:cap/JUK7101
				public static global::Epsitec.Common.Types.Caption LightYellow
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 39));
					}
				}
				//	designer:cap/JUKU001
				public static global::Epsitec.Common.Types.Caption Transparent
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 30));
					}
				}
				//	designer:cap/JUKT001
				public static global::Epsitec.Common.Types.Caption Unknown
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 29));
					}
				}
				//	designer:cap/JUKV001
				public static global::Epsitec.Common.Types.Caption White
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 31));
					}
				}
			}
			
			public static class ExportFormat
			{
				//	designer:cap/JUKF001
				public static global::Epsitec.Common.Types.Caption Csv
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 15));
					}
				}
				//	designer:cap/JUKI001
				public static global::Epsitec.Common.Types.Caption Json
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 18));
					}
				}
				//	designer:cap/JUKJ001
				public static global::Epsitec.Common.Types.Caption Pdf
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 19));
					}
				}
				//	designer:cap/JUKE001
				public static global::Epsitec.Common.Types.Caption Txt
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 14));
					}
				}
				//	designer:cap/JUKD001
				public static global::Epsitec.Common.Types.Caption Unknown
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 13));
					}
				}
				//	designer:cap/JUKG001
				public static global::Epsitec.Common.Types.Caption Xml
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 16));
					}
				}
				//	designer:cap/JUKH001
				public static global::Epsitec.Common.Types.Caption Yaml
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 17));
					}
				}
			}
			
			public static class PageFormatType
			{
				//	designer:cap/JUKM001
				public static global::Epsitec.Common.Types.Caption A2
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 22));
					}
				}
				//	designer:cap/JUKN001
				public static global::Epsitec.Common.Types.Caption A3
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 23));
					}
				}
				//	designer:cap/JUKO001
				public static global::Epsitec.Common.Types.Caption A4
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 24));
					}
				}
				//	designer:cap/JUKP001
				public static global::Epsitec.Common.Types.Caption A5
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 25));
					}
				}
				//	designer:cap/JUKR001
				public static global::Epsitec.Common.Types.Caption Legal
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 27));
					}
				}
				//	designer:cap/JUKQ001
				public static global::Epsitec.Common.Types.Caption Letter
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 26));
					}
				}
				//	designer:cap/JUKL001
				public static global::Epsitec.Common.Types.Caption Unknown
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 21));
					}
				}
			}
			
			public static class PfdPredefinedStyle
			{
				//	designer:cap/JUKG101
				public static global::Epsitec.Common.Types.Caption BlueEvenOdd
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 48));
					}
				}
				//	designer:cap/JUKE101
				public static global::Epsitec.Common.Types.Caption BoldFrame
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 46));
					}
				}
				//	designer:cap/JUKK101
				public static global::Epsitec.Common.Types.Caption Colored
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 52));
					}
				}
				//	designer:cap/JUKL101
				public static global::Epsitec.Common.Types.Caption Contrast
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 53));
					}
				}
				//	designer:cap/JUKB101
				public static global::Epsitec.Common.Types.Caption Frameless
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 43));
					}
				}
				//	designer:cap/JUKJ101
				public static global::Epsitec.Common.Types.Caption GreenEvenOdd
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 51));
					}
				}
				//	designer:cap/JUKF101
				public static global::Epsitec.Common.Types.Caption GreyEvenOdd
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 47));
					}
				}
				//	designer:cap/JUKM101
				public static global::Epsitec.Common.Types.Caption Kitch
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 54));
					}
				}
				//	designer:cap/JUKC101
				public static global::Epsitec.Common.Types.Caption LightFrame
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 44));
					}
				}
				//	designer:cap/JUKI101
				public static global::Epsitec.Common.Types.Caption RedEvenOdd
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 50));
					}
				}
				//	designer:cap/JUKD101
				public static global::Epsitec.Common.Types.Caption StandardFrame
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 45));
					}
				}
				//	designer:cap/JUKA101
				public static global::Epsitec.Common.Types.Caption Unknown
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 42));
					}
				}
				//	designer:cap/JUKH101
				public static global::Epsitec.Common.Types.Caption YellowEvenOdd
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 49));
					}
				}
			}
			
			public static class ReportType
			{
				//	designer:cap/JUKQ101
				public static global::Epsitec.Common.Types.Caption AssetsList
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 58));
					}
				}
				//	designer:cap/JUKP101
				public static global::Epsitec.Common.Types.Caption MCH2Summary
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 57));
					}
				}
				//	designer:cap/JUKR101
				public static global::Epsitec.Common.Types.Caption PersonsList
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 59));
					}
				}
				//	designer:cap/JUKO101
				public static global::Epsitec.Common.Types.Caption Unknown
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 56));
					}
				}
			}
			
		}
		
		//	Code mapping for 'String' resources
		
		public static class Strings
		{
			internal static void _Initialize()
			{
				global::System.Object.Equals (_stringsBundle, null);
			}
			
			public static class AccountsImport
			{
				public static class Message
				{
					//	designer:str/JUK5601
					public static global::Epsitec.Common.Types.FormattedText Equal
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544517));
						}
					}
					//	designer:str/JUK7601
					public static global::Epsitec.Common.Types.FormattedText New
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544519));
						}
					}
					//	designer:str/JUK6601
					public static global::Epsitec.Common.Types.FormattedText Update
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544518));
						}
					}
				}
			}
			
			public static class DataFillers
			{
				public static class LastViewsTreeTable
				{
					//	designer:str/JUKG501
					public static global::Epsitec.Common.Types.FormattedText Date
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544496));
						}
					}
					//	designer:str/JUKH501
					public static global::Epsitec.Common.Types.FormattedText Description
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544497));
						}
					}
					//	designer:str/JUKF501
					public static global::Epsitec.Common.Types.FormattedText Page
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544495));
						}
					}
					//	designer:str/JUKE501
					public static global::Epsitec.Common.Types.FormattedText Type
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544494));
						}
					}
				}
				
				public static class MessagesTreeTable
				{
					//	designer:str/JUKI501
					public static global::Epsitec.Common.Types.FormattedText Title
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544498));
						}
					}
				}
				
				public static class WarningsTreeTable
				{
					public static class Date
					{
						//	designer:str/JUKM501
						public static global::Epsitec.Common.Types.FormattedText Title
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544502));
							}
						}
						//	designer:str/JUKN501
						public static global::Epsitec.Common.Types.FormattedText Tooltip
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544503));
							}
						}
					}
					
					public static class Description
					{
						//	designer:str/JUKQ501
						public static global::Epsitec.Common.Types.FormattedText Title
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544506));
							}
						}
					}
					
					public static class EventGlyph
					{
						//	designer:str/JUKO501
						public static global::Epsitec.Common.Types.FormattedText Tooltip
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544504));
							}
						}
					}
					
					public static class Field
					{
						//	designer:str/JUKP501
						public static global::Epsitec.Common.Types.FormattedText Title
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544505));
							}
						}
					}
					
					public static class Glyph
					{
						//	designer:str/JUKJ501
						public static global::Epsitec.Common.Types.FormattedText Title
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544499));
							}
						}
						//	designer:str/JUKK501
						public static global::Epsitec.Common.Types.FormattedText Tooltip
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544500));
							}
						}
					}
					
					public static class Object
					{
						//	designer:str/JUKL501
						public static global::Epsitec.Common.Types.FormattedText Title
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544501));
							}
						}
					}
				}
			}
			
			public static class DateRange
			{
				//	designer:str/JUK3601
				public static global::Epsitec.Common.Types.FormattedText FromTo
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544515));
					}
				}
			}
			
			public static class DateTime
			{
				//	designer:str/JUK4601
				public static global::Epsitec.Common.Types.FormattedText WeekOfYear
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544516));
					}
				}
			}
			
			public static class EditorPages
			{
				public static class AmortizationDefinition
				{
					//	designer:str/JUK4901
					public static global::Epsitec.Common.Types.FormattedText Import
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544612));
						}
					}
					//	designer:str/JUK5901
					public static global::Epsitec.Common.Types.FormattedText ImportHelp
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544613));
						}
					}
				}
				
				public static class Category
				{
					//	designer:str/JUK3901
					public static global::Epsitec.Common.Types.FormattedText AccountsSubtitle
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544611));
						}
					}
					//	designer:str/JUK6901
					public static global::Epsitec.Common.Types.FormattedText CalculatorButton
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544614));
						}
					}
				}
				
				public static class ColorsExplanation
				{
					public static class Automatic
					{
						//	designer:str/JUKP801
						public static global::Epsitec.Common.Types.FormattedText Description
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544601));
							}
						}
						//	designer:str/JUKQ801
						public static global::Epsitec.Common.Types.FormattedText Tooltip
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544602));
							}
						}
					}
					
					public static class Defined
					{
						//	designer:str/JUKR801
						public static global::Epsitec.Common.Types.FormattedText Description
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544603));
							}
						}
						//	designer:str/JUKS801
						public static global::Epsitec.Common.Types.FormattedText Tooltip
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544604));
							}
						}
					}
					
					public static class Editable
					{
						//	designer:str/JUKN801
						public static global::Epsitec.Common.Types.FormattedText Description
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544599));
							}
						}
						//	designer:str/JUKO801
						public static global::Epsitec.Common.Types.FormattedText Tooltip
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544600));
							}
						}
					}
					
					public static class Error
					{
						//	designer:str/JUK1901
						public static global::Epsitec.Common.Types.FormattedText Description
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544609));
							}
						}
						//	designer:str/JUK2901
						public static global::Epsitec.Common.Types.FormattedText Tooltip
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544610));
							}
						}
					}
					
					public static class Readonly
					{
						//	designer:str/JUKT801
						public static global::Epsitec.Common.Types.FormattedText Description
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544605));
							}
						}
						//	designer:str/JUKU801
						public static global::Epsitec.Common.Types.FormattedText Tooltip
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544606));
							}
						}
					}
					
					public static class Result
					{
						//	designer:str/JUKV801
						public static global::Epsitec.Common.Types.FormattedText Description
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544607));
							}
						}
						//	designer:str/JUK0901
						public static global::Epsitec.Common.Types.FormattedText Tooltip
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544608));
							}
						}
					}
				}
				
				public static class OneShot
				{
					//	designer:str/JUK7901
					public static global::Epsitec.Common.Types.FormattedText Info
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544615));
						}
					}
				}
				
				public static class Summary
				{
					//	designer:str/JUKC901
					public static global::Epsitec.Common.Types.FormattedText Amortizations
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544620));
						}
					}
					//	designer:str/JUK8901
					public static global::Epsitec.Common.Types.FormattedText Event
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544616));
						}
					}
					//	designer:str/JUK9901
					public static global::Epsitec.Common.Types.FormattedText Groups
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544617));
						}
					}
					//	designer:str/JUKA901
					public static global::Epsitec.Common.Types.FormattedText Main
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544618));
						}
					}
					//	designer:str/JUKB901
					public static global::Epsitec.Common.Types.FormattedText MainValue
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544619));
						}
					}
				}
				
				public static class UserFields
				{
					//	designer:str/JUKE901
					public static global::Epsitec.Common.Types.FormattedText Edition
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544622));
						}
					}
					//	designer:str/JUKF901
					public static global::Epsitec.Common.Types.FormattedText Summary
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544623));
						}
					}
					//	designer:str/JUKD901
					public static global::Epsitec.Common.Types.FormattedText TreeTable
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544621));
						}
					}
				}
			}
			
			public static class Encoding
			{
				//	designer:str/JUK0601
				public static global::Epsitec.Common.Types.FormattedText Ascii
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544512));
					}
				}
				//	designer:str/JUKV501
				public static global::Epsitec.Common.Types.FormattedText BigEndianUnicode
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544511));
					}
				}
				//	designer:str/JUKU501
				public static global::Epsitec.Common.Types.FormattedText Unicode
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544510));
					}
				}
				//	designer:str/JUKT501
				public static global::Epsitec.Common.Types.FormattedText UTF32
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544509));
					}
				}
				//	designer:str/JUKR501
				public static global::Epsitec.Common.Types.FormattedText UTF7
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544507));
					}
				}
				//	designer:str/JUKS501
				public static global::Epsitec.Common.Types.FormattedText UTF8
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544508));
					}
				}
			}
			
			public static class Event
			{
				public static class AmortizationExtra
				{
					//	designer:str/JUK6201
					public static global::Epsitec.Common.Types.FormattedText Help
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544390));
						}
					}
					//	designer:str/JUKU101
					public static global::Epsitec.Common.Types.FormattedText ShortName
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544382));
						}
					}
				}
				
				public static class Input
				{
					//	designer:str/JUK1201
					public static global::Epsitec.Common.Types.FormattedText Help
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544385));
						}
					}
					//	designer:str/JUKP101
					public static global::Epsitec.Common.Types.FormattedText ShortName
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544377));
						}
					}
				}
				
				public static class Locked
				{
					//	designer:str/JUK7201
					public static global::Epsitec.Common.Types.FormattedText Help
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544391));
						}
					}
					//	designer:str/JUKV101
					public static global::Epsitec.Common.Types.FormattedText ShortName
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544383));
						}
					}
				}
				
				public static class MainValue
				{
					//	designer:str/JUK5201
					public static global::Epsitec.Common.Types.FormattedText Help
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544389));
						}
					}
					//	designer:str/JUKT101
					public static global::Epsitec.Common.Types.FormattedText ShortName
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544381));
						}
					}
				}
				
				public static class Modification
				{
					//	designer:str/JUK2201
					public static global::Epsitec.Common.Types.FormattedText Help
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544386));
						}
					}
					//	designer:str/JUKQ101
					public static global::Epsitec.Common.Types.FormattedText ShortName
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544378));
						}
					}
				}
				
				public static class Output
				{
					//	designer:str/JUK8201
					public static global::Epsitec.Common.Types.FormattedText Help
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544392));
						}
					}
					//	designer:str/JUK0201
					public static global::Epsitec.Common.Types.FormattedText ShortName
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544384));
						}
					}
				}
				
				public static class Revalorization
				{
					//	designer:str/JUK4201
					public static global::Epsitec.Common.Types.FormattedText Help
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544388));
						}
					}
					//	designer:str/JUKS101
					public static global::Epsitec.Common.Types.FormattedText ShortName
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544380));
						}
					}
				}
				
				public static class Revaluation
				{
					//	designer:str/JUK3201
					public static global::Epsitec.Common.Types.FormattedText Help
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544387));
						}
					}
					//	designer:str/JUKR101
					public static global::Epsitec.Common.Types.FormattedText ShortName
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544379));
						}
					}
				}
			}
			
			public static class Export
			{
				public static class Engine
				{
					//	designer:str/JUK1601
					public static global::Epsitec.Common.Types.FormattedText UnknownFormat
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544513));
						}
					}
				}
			}
			
			public static class FieldControllers
			{
				//	designer:str/JUKH901
				public static global::Epsitec.Common.Types.FormattedText ClearModification
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544625));
					}
				}
				//	designer:str/JUKG901
				public static global::Epsitec.Common.Types.FormattedText ShowHistory
				{
					get
					{
						return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544624));
					}
				}
				public static class Account
				{
					//	designer:str/JUKI901
					public static global::Epsitec.Common.Types.FormattedText Goto
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544626));
						}
					}
				}
				
				public static class Date
				{
					//	designer:str/JUKJ901
					public static global::Epsitec.Common.Types.FormattedText Begin
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544627));
						}
					}
					//	designer:str/JUKN901
					public static global::Epsitec.Common.Types.FormattedText Calendar
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544631));
						}
					}
					//	designer:str/JUKO901
					public static global::Epsitec.Common.Types.FormattedText Delete
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544632));
						}
					}
					//	designer:str/JUKL901
					public static global::Epsitec.Common.Types.FormattedText End
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544629));
						}
					}
					//	designer:str/JUKQ901
					public static global::Epsitec.Common.Types.FormattedText NextDay
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544634));
						}
					}
					//	designer:str/JUKS901
					public static global::Epsitec.Common.Types.FormattedText NextMonth
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544636));
						}
					}
					//	designer:str/JUKU901
					public static global::Epsitec.Common.Types.FormattedText NextYear
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544638));
						}
					}
					//	designer:str/JUKK901
					public static global::Epsitec.Common.Types.FormattedText Now
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544628));
						}
					}
					//	designer:str/JUKM901
					public static global::Epsitec.Common.Types.FormattedText Predefined
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544630));
						}
					}
					//	designer:str/JUKP901
					public static global::Epsitec.Common.Types.FormattedText PrevDay
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544633));
						}
					}
					//	designer:str/JUKR901
					public static global::Epsitec.Common.Types.FormattedText PrevMonth
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544635));
						}
					}
					//	designer:str/JUKT901
					public static global::Epsitec.Common.Types.FormattedText PrevYear
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544637));
						}
					}
				}
				
				public static class GuidRatio
				{
					//	designer:str/JUKV901
					public static global::Epsitec.Common.Types.FormattedText List
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544639));
						}
					}
					//	designer:str/JUK0A01
					public static global::Epsitec.Common.Types.FormattedText New
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544640));
						}
					}
				}
				
				public static class Person
				{
					//	designer:str/JUK1A01
					public static global::Epsitec.Common.Types.FormattedText Goto
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544641));
						}
					}
				}
			}
			
			public static class Popup
			{
				public static class Accounts
				{
					//	designer:str/JUKJ001
					public static global::Epsitec.Common.Types.FormattedText Choice
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544339));
						}
					}
					//	designer:str/JUKI001
					public static global::Epsitec.Common.Types.FormattedText NoAccounts
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544338));
						}
					}
				}
				
				public static class AccountsImport
				{
					//	designer:str/JUKH001
					public static global::Epsitec.Common.Types.FormattedText File
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544337));
						}
					}
				}
				
				public static class Amortizations
				{
					//	designer:str/JUKL001
					public static global::Epsitec.Common.Types.FormattedText FromDate
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544341));
						}
					}
					//	designer:str/JUKN001
					public static global::Epsitec.Common.Types.FormattedText Object
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544343));
						}
					}
					//	designer:str/JUKM001
					public static global::Epsitec.Common.Types.FormattedText ToDate
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544342));
						}
					}
					public static class Delete
					{
						//	designer:str/JUK3101
						public static global::Epsitec.Common.Types.FormattedText All
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544355));
							}
						}
						//	designer:str/JUK2101
						public static global::Epsitec.Common.Types.FormattedText One
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544354));
							}
						}
						//	designer:str/JUK1101
						public static global::Epsitec.Common.Types.FormattedText Title
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544353));
							}
						}
					}
					
					public static class Fix
					{
						//	designer:str/JUKT001
						public static global::Epsitec.Common.Types.FormattedText All
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544349));
							}
						}
						//	designer:str/JUKS001
						public static global::Epsitec.Common.Types.FormattedText One
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544348));
							}
						}
						//	designer:str/JUKR001
						public static global::Epsitec.Common.Types.FormattedText Title
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544347));
							}
						}
					}
					
					public static class Preview
					{
						//	designer:str/JUKQ001
						public static global::Epsitec.Common.Types.FormattedText All
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544346));
							}
						}
						//	designer:str/JUKP001
						public static global::Epsitec.Common.Types.FormattedText One
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544345));
							}
						}
						//	designer:str/JUKO001
						public static global::Epsitec.Common.Types.FormattedText Title
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544344));
							}
						}
					}
					
					public static class ToExtra
					{
						//	designer:str/JUKE701
						public static global::Epsitec.Common.Types.FormattedText Title
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544558));
							}
						}
					}
					
					public static class Unpreview
					{
						//	designer:str/JUK0101
						public static global::Epsitec.Common.Types.FormattedText All
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544352));
							}
						}
						//	designer:str/JUKV001
						public static global::Epsitec.Common.Types.FormattedText One
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544351));
							}
						}
						//	designer:str/JUKU001
						public static global::Epsitec.Common.Types.FormattedText Title
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544350));
							}
						}
					}
				}
				
				public static class AssetCopy
				{
					//	designer:str/JUK7101
					public static global::Epsitec.Common.Types.FormattedText StateDate
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544359));
						}
					}
					//	designer:str/JUK5101
					public static global::Epsitec.Common.Types.FormattedText StateGlobal
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544357));
						}
					}
					//	designer:str/JUK6101
					public static global::Epsitec.Common.Types.FormattedText StateInput
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544358));
						}
					}
					//	designer:str/JUK4101
					public static global::Epsitec.Common.Types.FormattedText Title
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544356));
						}
					}
				}
				
				public static class AssetPaste
				{
					//	designer:str/JUK9101
					public static global::Epsitec.Common.Types.FormattedText Date
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544361));
						}
					}
					//	designer:str/JUK8101
					public static global::Epsitec.Common.Types.FormattedText Title
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544360));
						}
					}
				}
				
				public static class AssetsReport
				{
					//	designer:str/JUKC101
					public static global::Epsitec.Common.Types.FormattedText Group
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544364));
						}
					}
					//	designer:str/JUKD101
					public static global::Epsitec.Common.Types.FormattedText Level
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544365));
						}
					}
					//	designer:str/JUKB101
					public static global::Epsitec.Common.Types.FormattedText State
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544363));
						}
					}
					//	designer:str/JUKA101
					public static global::Epsitec.Common.Types.FormattedText Title
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544362));
						}
					}
				}
				
				public static class Button
				{
					//	designer:str/JUK6001
					public static global::Epsitec.Common.Types.FormattedText Cancel
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544326));
						}
					}
					//	designer:str/JUKD001
					public static global::Epsitec.Common.Types.FormattedText Close
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544333));
						}
					}
					//	designer:str/JUKE001
					public static global::Epsitec.Common.Types.FormattedText Compute
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544334));
						}
					}
					//	designer:str/JUK8001
					public static global::Epsitec.Common.Types.FormattedText Copy
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544328));
						}
					}
					//	designer:str/JUK4001
					public static global::Epsitec.Common.Types.FormattedText Create
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544324));
						}
					}
					//	designer:str/JUKB001
					public static global::Epsitec.Common.Types.FormattedText Export
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544331));
						}
					}
					//	designer:str/JUK7001
					public static global::Epsitec.Common.Types.FormattedText Import
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544327));
						}
					}
					//	designer:str/JUKG001
					public static global::Epsitec.Common.Types.FormattedText No
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544336));
						}
					}
					//	designer:str/JUK5001
					public static global::Epsitec.Common.Types.FormattedText Ok
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544325));
						}
					}
					//	designer:str/JUKC001
					public static global::Epsitec.Common.Types.FormattedText Open
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544332));
						}
					}
					//	designer:str/JUK9001
					public static global::Epsitec.Common.Types.FormattedText Paste
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544329));
						}
					}
					//	designer:str/JUKA001
					public static global::Epsitec.Common.Types.FormattedText Show
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544330));
						}
					}
					//	designer:str/JUKF001
					public static global::Epsitec.Common.Types.FormattedText Yes
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544335));
						}
					}
				}
				
				public static class Calendar
				{
					//	designer:str/JUKE101
					public static global::Epsitec.Common.Types.FormattedText Title
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544366));
						}
					}
				}
				
				public static class Categories
				{
					//	designer:str/JUKF101
					public static global::Epsitec.Common.Types.FormattedText Title
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544367));
						}
					}
				}
				
				public static class CreateAsset
				{
					//	designer:str/JUKK101
					public static global::Epsitec.Common.Types.FormattedText Category
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544372));
						}
					}
					//	designer:str/JUKH101
					public static global::Epsitec.Common.Types.FormattedText Date
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544369));
						}
					}
					//	designer:str/JUKI101
					public static global::Epsitec.Common.Types.FormattedText Name
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544370));
						}
					}
					//	designer:str/JUKG101
					public static global::Epsitec.Common.Types.FormattedText Title
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544368));
						}
					}
					//	designer:str/JUKJ101
					public static global::Epsitec.Common.Types.FormattedText Value
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544371));
						}
					}
				}
				
				public static class CreateCategory
				{
					//	designer:str/JUKN101
					public static global::Epsitec.Common.Types.FormattedText Model
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544375));
						}
					}
					//	designer:str/JUKM101
					public static global::Epsitec.Common.Types.FormattedText Name
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544374));
						}
					}
					//	designer:str/JUKL101
					public static global::Epsitec.Common.Types.FormattedText Title
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544373));
						}
					}
				}
				
				public static class CreateEvent
				{
					//	designer:str/JUKO101
					public static global::Epsitec.Common.Types.FormattedText Title
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544376));
						}
					}
				}
				
				public static class CreateGroup
				{
					//	designer:str/JUKA201
					public static global::Epsitec.Common.Types.FormattedText Name
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544394));
						}
					}
					//	designer:str/JUKB201
					public static global::Epsitec.Common.Types.FormattedText Parent
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544395));
						}
					}
					//	designer:str/JUK9201
					public static global::Epsitec.Common.Types.FormattedText Title
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544393));
						}
					}
				}
				
				public static class CreatePerson
				{
					//	designer:str/JUKE201
					public static global::Epsitec.Common.Types.FormattedText Model
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544398));
						}
					}
					//	designer:str/JUKD201
					public static global::Epsitec.Common.Types.FormattedText Name
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544397));
						}
					}
					//	designer:str/JUKC201
					public static global::Epsitec.Common.Types.FormattedText Title
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544396));
						}
					}
				}
				
				public static class Date
				{
					//	designer:str/JUKF201
					public static global::Epsitec.Common.Types.FormattedText Title
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544399));
						}
					}
				}
				
				public static class Errors
				{
					//	designer:str/JUKG201
					public static global::Epsitec.Common.Types.FormattedText Title
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544400));
						}
					}
				}
				
				public static class EventPaste
				{
					//	designer:str/JUKI201
					public static global::Epsitec.Common.Types.FormattedText Date
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544402));
						}
					}
					//	designer:str/JUKH201
					public static global::Epsitec.Common.Types.FormattedText Title
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544401));
						}
					}
				}
				
				public static class Export
				{
					//	designer:str/JUKM201
					public static global::Epsitec.Common.Types.FormattedText CamelCase
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544406));
						}
					}
					//	designer:str/JUKO201
					public static global::Epsitec.Common.Types.FormattedText Encoding
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544408));
						}
					}
					//	designer:str/JUKN201
					public static global::Epsitec.Common.Types.FormattedText EndOfLines
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544407));
						}
					}
				}
				
				public static class ExportInstructions
				{
					//	designer:str/JUKK201
					public static global::Epsitec.Common.Types.FormattedText Filename
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544404));
						}
					}
					//	designer:str/JUKJ201
					public static global::Epsitec.Common.Types.FormattedText Title
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544403));
						}
					}
				}
				
				public static class ExportJson
				{
					//	designer:str/JUKL201
					public static global::Epsitec.Common.Types.FormattedText Title
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544405));
						}
					}
				}
				
				public static class ExportOpen
				{
					//	designer:str/JUKQ201
					public static global::Epsitec.Common.Types.FormattedText Radios
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544410));
						}
					}
					//	designer:str/JUKP201
					public static global::Epsitec.Common.Types.FormattedText Title
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544409));
						}
					}
				}
				
				public static class ExportPdf
				{
					//	designer:str/JUK2301
					public static global::Epsitec.Common.Types.FormattedText AutomaticColumnWidths
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544418));
						}
					}
					//	designer:str/JUK1301
					public static global::Epsitec.Common.Types.FormattedText CellMargins
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544417));
						}
					}
					//	designer:str/JUKV201
					public static global::Epsitec.Common.Types.FormattedText FontName
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544415));
						}
					}
					//	designer:str/JUK0301
					public static global::Epsitec.Common.Types.FormattedText FontSize
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544416));
						}
					}
					//	designer:str/JUK4301
					public static global::Epsitec.Common.Types.FormattedText Footer
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544420));
						}
					}
					//	designer:str/JUK3301
					public static global::Epsitec.Common.Types.FormattedText Header
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544419));
						}
					}
					//	designer:str/JUK5301
					public static global::Epsitec.Common.Types.FormattedText Indent
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544421));
						}
					}
					//	designer:str/JUKT201
					public static global::Epsitec.Common.Types.FormattedText PageFormat
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544413));
						}
					}
					//	designer:str/JUKU201
					public static global::Epsitec.Common.Types.FormattedText PageMargins
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544414));
						}
					}
					//	designer:str/JUKS201
					public static global::Epsitec.Common.Types.FormattedText Style
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544412));
						}
					}
					//	designer:str/JUKR201
					public static global::Epsitec.Common.Types.FormattedText Title
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544411));
						}
					}
					//	designer:str/JUK6301
					public static global::Epsitec.Common.Types.FormattedText Watermark
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544422));
						}
					}
				}
				
				public static class ExportText
				{
					//	designer:str/JUKB301
					public static global::Epsitec.Common.Types.FormattedText ColumnBracket
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544427));
						}
					}
					//	designer:str/JUKA301
					public static global::Epsitec.Common.Types.FormattedText ColumnSeparator
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544426));
						}
					}
					//	designer:str/JUKC301
					public static global::Epsitec.Common.Types.FormattedText Escape
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544428));
						}
					}
					//	designer:str/JUK8301
					public static global::Epsitec.Common.Types.FormattedText HasHeader
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544424));
						}
					}
					//	designer:str/JUK9301
					public static global::Epsitec.Common.Types.FormattedText Inverted
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544425));
						}
					}
					//	designer:str/JUK7301
					public static global::Epsitec.Common.Types.FormattedText Title
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544423));
						}
					}
				}
				
				public static class ExportXml
				{
					//	designer:str/JUKE301
					public static global::Epsitec.Common.Types.FormattedText BodyTag
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544430));
						}
					}
					//	designer:str/JUKD301
					public static global::Epsitec.Common.Types.FormattedText Compact
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544429));
						}
					}
					//	designer:str/JUKG301
					public static global::Epsitec.Common.Types.FormattedText Indent
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544432));
						}
					}
					//	designer:str/JUKF301
					public static global::Epsitec.Common.Types.FormattedText RecordTag
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544431));
						}
					}
				}
				
				public static class ExportYaml
				{
					//	designer:str/JUKH301
					public static global::Epsitec.Common.Types.FormattedText Indent
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544433));
						}
					}
				}
				
				public static class Filter
				{
					//	designer:str/JUKJ301
					public static global::Epsitec.Common.Types.FormattedText GroupCancel
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544435));
						}
					}
					//	designer:str/JUKI301
					public static global::Epsitec.Common.Types.FormattedText Title
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544434));
						}
					}
				}
				
				public static class Groups
				{
					//	designer:str/JUKK301
					public static global::Epsitec.Common.Types.FormattedText Title
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544436));
						}
					}
				}
				
				public static class History
				{
					//	designer:str/JUKM301
					public static global::Epsitec.Common.Types.FormattedText Title
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544438));
						}
					}
					//	designer:str/JUKL301
					public static global::Epsitec.Common.Types.FormattedText Undefined
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544437));
						}
					}
				}
				
				public static class LastViews
				{
					//	designer:str/JUKN301
					public static global::Epsitec.Common.Types.FormattedText Title
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544439));
						}
					}
				}
				
				public static class Locked
				{
					//	designer:str/JUKP301
					public static global::Epsitec.Common.Types.FormattedText Date
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544441));
						}
					}
					//	designer:str/JUKO301
					public static global::Epsitec.Common.Types.FormattedText Title
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544440));
						}
					}
					public static class Button
					{
						//	designer:str/JUKU301
						public static global::Epsitec.Common.Types.FormattedText LockAll
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544446));
							}
						}
						//	designer:str/JUKV301
						public static global::Epsitec.Common.Types.FormattedText LockOne
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544447));
							}
						}
						//	designer:str/JUKS301
						public static global::Epsitec.Common.Types.FormattedText UnlockAll
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544444));
							}
						}
						//	designer:str/JUKT301
						public static global::Epsitec.Common.Types.FormattedText UnlockOne
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544445));
							}
						}
					}
					
					public static class Radios
					{
						//	designer:str/JUKR301
						public static global::Epsitec.Common.Types.FormattedText IsAll
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544443));
							}
						}
						//	designer:str/JUKQ301
						public static global::Epsitec.Common.Types.FormattedText IsDelete
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544442));
							}
						}
					}
				}
				
				public static class Margins
				{
					//	designer:str/JUK4401
					public static global::Epsitec.Common.Types.FormattedText Bottom
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544452));
						}
					}
					//	designer:str/JUK1401
					public static global::Epsitec.Common.Types.FormattedText Left
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544449));
						}
					}
					//	designer:str/JUK2401
					public static global::Epsitec.Common.Types.FormattedText Right
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544450));
						}
					}
					//	designer:str/JUK3401
					public static global::Epsitec.Common.Types.FormattedText Top
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544451));
						}
					}
					//	designer:str/JUK0401
					public static global::Epsitec.Common.Types.FormattedText Unified
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544448));
						}
					}
				}
				
				public static class MCH2SummaryReport
				{
					//	designer:str/JUK7401
					public static global::Epsitec.Common.Types.FormattedText FinalDate
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544455));
						}
					}
					//	designer:str/JUK9401
					public static global::Epsitec.Common.Types.FormattedText GroupEnable
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544457));
						}
					}
					//	designer:str/JUK6401
					public static global::Epsitec.Common.Types.FormattedText InitialDate
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544454));
						}
					}
					//	designer:str/JUKA401
					public static global::Epsitec.Common.Types.FormattedText Level
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544458));
						}
					}
					//	designer:str/JUK8401
					public static global::Epsitec.Common.Types.FormattedText MonthCount
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544456));
						}
					}
					//	designer:str/JUK5401
					public static global::Epsitec.Common.Types.FormattedText Title
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544453));
						}
					}
				}
				
				public static class Message
				{
					//	designer:str/JUKF401
					public static global::Epsitec.Common.Types.FormattedText ErrorTitle
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544463));
						}
					}
					//	designer:str/JUKH401
					public static global::Epsitec.Common.Types.FormattedText MessageTitle
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544465));
						}
					}
					//	designer:str/JUKB401
					public static global::Epsitec.Common.Types.FormattedText WarningTitle
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544459));
						}
					}
					public static class AccountsImport
					{
						//	designer:str/JUK8601
						public static global::Epsitec.Common.Types.FormattedText Title
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544520));
							}
						}
					}
					
					public static class DeleteEventWarning
					{
						//	designer:str/JUKC401
						public static global::Epsitec.Common.Types.FormattedText Text
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544460));
							}
						}
					}
					
					public static class ExportError
					{
						//	designer:str/JUK2601
						public static global::Epsitec.Common.Types.FormattedText Text
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544514));
							}
						}
					}
					
					public static class PasteError
					{
						//	designer:str/JUKI401
						public static global::Epsitec.Common.Types.FormattedText Text
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544466));
							}
						}
					}
					
					public static class PreviewEventWarning
					{
						//	designer:str/JUKE401
						public static global::Epsitec.Common.Types.FormattedText Text
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544462));
							}
						}
					}
					
					public static class Todo
					{
						//	designer:str/JUKG401
						public static global::Epsitec.Common.Types.FormattedText Text
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544464));
							}
						}
					}
				}
				
				public static class NewMandat
				{
					//	designer:str/JUK3001
					public static global::Epsitec.Common.Types.FormattedText Date
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544323));
						}
					}
					//	designer:str/JUK2001
					public static global::Epsitec.Common.Types.FormattedText Name
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544322));
						}
					}
					//	designer:str/JUK1001
					public static global::Epsitec.Common.Types.FormattedText Sample
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544321));
						}
					}
					//	designer:str/JUK0001
					public static global::Epsitec.Common.Types.FormattedText Title
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544320));
						}
					}
				}
				
				public static class PageSize
				{
					//	designer:str/JUKK401
					public static global::Epsitec.Common.Types.FormattedText Format
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544468));
						}
					}
					//	designer:str/JUKN401
					public static global::Epsitec.Common.Types.FormattedText Height
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544471));
						}
					}
					//	designer:str/JUKL401
					public static global::Epsitec.Common.Types.FormattedText Radios
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544469));
						}
					}
					//	designer:str/JUKJ401
					public static global::Epsitec.Common.Types.FormattedText Title
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544467));
						}
					}
					//	designer:str/JUKM401
					public static global::Epsitec.Common.Types.FormattedText Width
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544470));
						}
					}
					public static class Description
					{
						//	designer:str/JUKP401
						public static global::Epsitec.Common.Types.FormattedText Landscape
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544473));
							}
						}
						//	designer:str/JUKO401
						public static global::Epsitec.Common.Types.FormattedText Portrait
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544472));
							}
						}
					}
				}
				
				public static class PdfStyle
				{
					//	designer:str/JUK6501
					public static global::Epsitec.Common.Types.FormattedText BorderColor
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544486));
						}
					}
					//	designer:str/JUK4501
					public static global::Epsitec.Common.Types.FormattedText EvenColor
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544484));
						}
					}
					//	designer:str/JUK3501
					public static global::Epsitec.Common.Types.FormattedText LabelColor
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544483));
						}
					}
					//	designer:str/JUK5501
					public static global::Epsitec.Common.Types.FormattedText OddColor
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544485));
						}
					}
					//	designer:str/JUK2501
					public static global::Epsitec.Common.Types.FormattedText Predefined
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544482));
						}
					}
					//	designer:str/JUK7501
					public static global::Epsitec.Common.Types.FormattedText Thickness
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544487));
						}
					}
					//	designer:str/JUK1501
					public static global::Epsitec.Common.Types.FormattedText Title
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544481));
						}
					}
				}
				
				public static class Persons
				{
					//	designer:str/JUK8501
					public static global::Epsitec.Common.Types.FormattedText Title
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544488));
						}
					}
				}
				
				public static class RateCalculator
				{
					//	designer:str/JUKB501
					public static global::Epsitec.Common.Types.FormattedText Result
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544491));
						}
					}
					//	designer:str/JUK9501
					public static global::Epsitec.Common.Types.FormattedText Title
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544489));
						}
					}
					//	designer:str/JUKA501
					public static global::Epsitec.Common.Types.FormattedText TotalYears
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544490));
						}
					}
				}
				
				public static class YesNo
				{
					//	designer:str/JUKD501
					public static global::Epsitec.Common.Types.FormattedText DeleteEventQuestion
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544493));
						}
					}
					//	designer:str/JUKC501
					public static global::Epsitec.Common.Types.FormattedText Title
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544492));
						}
					}
				}
			}
			
			public static class Toolbar
			{
				public static class Edit
				{
					//	designer:str/JUK9601
					public static global::Epsitec.Common.Types.FormattedText Accept
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544521));
						}
					}
					//	designer:str/JUKA601
					public static global::Epsitec.Common.Types.FormattedText Cancel
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544522));
						}
					}
				}
				
				public static class Main
				{
					//	designer:str/JUKO601
					public static global::Epsitec.Common.Types.FormattedText Accept
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544536));
						}
					}
					//	designer:str/JUKN601
					public static global::Epsitec.Common.Types.FormattedText Cancel
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544535));
						}
					}
					//	designer:str/JUKP601
					public static global::Epsitec.Common.Types.FormattedText ChoiceSettings
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544537));
						}
					}
					//	designer:str/JUKK601
					public static global::Epsitec.Common.Types.FormattedText Edit
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544532));
						}
					}
					//	designer:str/JUKL601
					public static global::Epsitec.Common.Types.FormattedText Locked
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544533));
						}
					}
					//	designer:str/JUKB601
					public static global::Epsitec.Common.Types.FormattedText New
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544523));
						}
					}
					//	designer:str/JUKC601
					public static global::Epsitec.Common.Types.FormattedText Open
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544524));
						}
					}
					//	designer:str/JUKD601
					public static global::Epsitec.Common.Types.FormattedText Save
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544525));
						}
					}
					//	designer:str/JUKM601
					public static global::Epsitec.Common.Types.FormattedText Simulation
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544534));
						}
					}
					public static class Navigate
					{
						//	designer:str/JUKE601
						public static global::Epsitec.Common.Types.FormattedText Back
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544526));
							}
						}
						//	designer:str/JUKF601
						public static global::Epsitec.Common.Types.FormattedText Forward
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544527));
							}
						}
						//	designer:str/JUKG601
						public static global::Epsitec.Common.Types.FormattedText Menu
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544528));
							}
						}
					}
					
					public static class Show
					{
						//	designer:str/JUKI601
						public static global::Epsitec.Common.Types.FormattedText TimelineEvent
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544530));
							}
						}
						//	designer:str/JUKJ601
						public static global::Epsitec.Common.Types.FormattedText TimelineMultiple
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544531));
							}
						}
						//	designer:str/JUKH601
						public static global::Epsitec.Common.Types.FormattedText TimelineSingle
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544529));
							}
						}
					}
				}
				
				public static class Reports
				{
					//	designer:str/JUK2701
					public static global::Epsitec.Common.Types.FormattedText Close
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544546));
						}
					}
					//	designer:str/JUKR601
					public static global::Epsitec.Common.Types.FormattedText CompactAll
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544539));
						}
					}
					//	designer:str/JUKS601
					public static global::Epsitec.Common.Types.FormattedText CompactOne
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544540));
						}
					}
					//	designer:str/JUKU601
					public static global::Epsitec.Common.Types.FormattedText ExpandAll
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544542));
						}
					}
					//	designer:str/JUKT601
					public static global::Epsitec.Common.Types.FormattedText ExpandOne
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544541));
						}
					}
					//	designer:str/JUK1701
					public static global::Epsitec.Common.Types.FormattedText Export
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544545));
						}
					}
					//	designer:str/JUK0701
					public static global::Epsitec.Common.Types.FormattedText NextPeriod
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544544));
						}
					}
					//	designer:str/JUKQ601
					public static global::Epsitec.Common.Types.FormattedText Params
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544538));
						}
					}
					//	designer:str/JUKV601
					public static global::Epsitec.Common.Types.FormattedText PrevPeriod
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544543));
						}
					}
				}
				
				public static class Timeline
				{
					//	designer:str/JUKG701
					public static global::Epsitec.Common.Types.FormattedText Compacted
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544560));
						}
					}
					//	designer:str/JUKU701
					public static global::Epsitec.Common.Types.FormattedText Copy
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544574));
						}
					}
					//	designer:str/JUKQ701
					public static global::Epsitec.Common.Types.FormattedText Date
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544570));
						}
					}
					//	designer:str/JUKJ701
					public static global::Epsitec.Common.Types.FormattedText DaysOfWeek
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544563));
						}
					}
					//	designer:str/JUKS701
					public static global::Epsitec.Common.Types.FormattedText Delete
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544572));
						}
					}
					//	designer:str/JUKT701
					public static global::Epsitec.Common.Types.FormattedText Deselect
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544573));
						}
					}
					//	designer:str/JUKH701
					public static global::Epsitec.Common.Types.FormattedText Expanded
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544561));
						}
					}
					//	designer:str/JUKL701
					public static global::Epsitec.Common.Types.FormattedText First
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544565));
						}
					}
					//	designer:str/JUKK701
					public static global::Epsitec.Common.Types.FormattedText Graph
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544564));
						}
					}
					//	designer:str/JUKF701
					public static global::Epsitec.Common.Types.FormattedText Labels
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544559));
						}
					}
					//	designer:str/JUKO701
					public static global::Epsitec.Common.Types.FormattedText Last
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544568));
						}
					}
					//	designer:str/JUKR701
					public static global::Epsitec.Common.Types.FormattedText New
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544571));
						}
					}
					//	designer:str/JUKN701
					public static global::Epsitec.Common.Types.FormattedText Next
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544567));
						}
					}
					//	designer:str/JUKP701
					public static global::Epsitec.Common.Types.FormattedText Now
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544569));
						}
					}
					//	designer:str/JUKV701
					public static global::Epsitec.Common.Types.FormattedText Paste
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544575));
						}
					}
					//	designer:str/JUKM701
					public static global::Epsitec.Common.Types.FormattedText Prev
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544566));
						}
					}
					//	designer:str/JUKI701
					public static global::Epsitec.Common.Types.FormattedText WeeksOfYear
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544562));
						}
					}
				}
				
				public static class Timelines
				{
					//	designer:str/JUKC701
					public static global::Epsitec.Common.Types.FormattedText CopyEvent
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544556));
						}
					}
					//	designer:str/JUKA701
					public static global::Epsitec.Common.Types.FormattedText DeleteEvent
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544554));
						}
					}
					//	designer:str/JUKB701
					public static global::Epsitec.Common.Types.FormattedText DeselectEvent
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544555));
						}
					}
					//	designer:str/JUK5701
					public static global::Epsitec.Common.Types.FormattedText First
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544549));
						}
					}
					//	designer:str/JUK8701
					public static global::Epsitec.Common.Types.FormattedText Last
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544552));
						}
					}
					//	designer:str/JUK3701
					public static global::Epsitec.Common.Types.FormattedText Narrow
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544547));
						}
					}
					//	designer:str/JUK9701
					public static global::Epsitec.Common.Types.FormattedText NewEvent
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544553));
						}
					}
					//	designer:str/JUK7701
					public static global::Epsitec.Common.Types.FormattedText Next
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544551));
						}
					}
					//	designer:str/JUKD701
					public static global::Epsitec.Common.Types.FormattedText PasteEvent
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544557));
						}
					}
					//	designer:str/JUK6701
					public static global::Epsitec.Common.Types.FormattedText Prev
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544550));
						}
					}
					//	designer:str/JUK4701
					public static global::Epsitec.Common.Types.FormattedText Wide
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544548));
						}
					}
				}
				
				public static class TreeTable
				{
					//	designer:str/JUK7801
					public static global::Epsitec.Common.Types.FormattedText CompactAll
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544583));
						}
					}
					//	designer:str/JUK8801
					public static global::Epsitec.Common.Types.FormattedText CompactOne
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544584));
						}
					}
					//	designer:str/JUKI801
					public static global::Epsitec.Common.Types.FormattedText Copy
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544594));
						}
					}
					//	designer:str/JUK1801
					public static global::Epsitec.Common.Types.FormattedText DateRange
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544577));
						}
					}
					//	designer:str/JUKG801
					public static global::Epsitec.Common.Types.FormattedText Delete
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544592));
						}
					}
					//	designer:str/JUKH801
					public static global::Epsitec.Common.Types.FormattedText Deselect
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544593));
						}
					}
					//	designer:str/JUKA801
					public static global::Epsitec.Common.Types.FormattedText ExpandAll
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544586));
						}
					}
					//	designer:str/JUK9801
					public static global::Epsitec.Common.Types.FormattedText ExpandOne
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544585));
						}
					}
					//	designer:str/JUKK801
					public static global::Epsitec.Common.Types.FormattedText Export
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544596));
						}
					}
					//	designer:str/JUK0801
					public static global::Epsitec.Common.Types.FormattedText Filter
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544576));
						}
					}
					//	designer:str/JUK3801
					public static global::Epsitec.Common.Types.FormattedText First
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544579));
						}
					}
					//	designer:str/JUKM801
					public static global::Epsitec.Common.Types.FormattedText Goto
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544598));
						}
					}
					//	designer:str/JUK2801
					public static global::Epsitec.Common.Types.FormattedText Graphic
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544578));
						}
					}
					//	designer:str/JUKL801
					public static global::Epsitec.Common.Types.FormattedText Import
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544597));
						}
					}
					//	designer:str/JUK6801
					public static global::Epsitec.Common.Types.FormattedText Last
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544582));
						}
					}
					//	designer:str/JUKE801
					public static global::Epsitec.Common.Types.FormattedText MoveBottom
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544590));
						}
					}
					//	designer:str/JUKD801
					public static global::Epsitec.Common.Types.FormattedText MoveDown
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544589));
						}
					}
					//	designer:str/JUKB801
					public static global::Epsitec.Common.Types.FormattedText MoveTop
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544587));
						}
					}
					//	designer:str/JUKC801
					public static global::Epsitec.Common.Types.FormattedText MoveUp
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544588));
						}
					}
					//	designer:str/JUKF801
					public static global::Epsitec.Common.Types.FormattedText New
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544591));
						}
					}
					//	designer:str/JUK5801
					public static global::Epsitec.Common.Types.FormattedText Next
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544581));
						}
					}
					//	designer:str/JUKJ801
					public static global::Epsitec.Common.Types.FormattedText Paste
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544595));
						}
					}
					//	designer:str/JUK4801
					public static global::Epsitec.Common.Types.FormattedText Prev
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544580));
						}
					}
				}
			}
			
			public static class ToolbarController
			{
				public static class AssetsTreeTable
				{
					//	designer:str/JUKDA01
					public static global::Epsitec.Common.Types.FormattedText New
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544653));
						}
					}
				}
			}
			
			public static class ToolbarControllers
			{
				public static class AccountsTreeTable
				{
					//	designer:str/JUK6A01
					public static global::Epsitec.Common.Types.FormattedText Copy
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544646));
						}
					}
					//	designer:str/JUK2A01
					public static global::Epsitec.Common.Types.FormattedText DateRange
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544642));
						}
					}
					//	designer:str/JUK4A01
					public static global::Epsitec.Common.Types.FormattedText Delete
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544644));
						}
					}
					//	designer:str/JUK5A01
					public static global::Epsitec.Common.Types.FormattedText Deselect
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544645));
						}
					}
					//	designer:str/JUK8A01
					public static global::Epsitec.Common.Types.FormattedText Export
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544648));
						}
					}
					//	designer:str/JUK9A01
					public static global::Epsitec.Common.Types.FormattedText Import
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544649));
						}
					}
					//	designer:str/JUK3A01
					public static global::Epsitec.Common.Types.FormattedText New
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544643));
						}
					}
					//	designer:str/JUK7A01
					public static global::Epsitec.Common.Types.FormattedText Paste
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544647));
						}
					}
				}
				
				public static class AssetsTimeline
				{
					public static class Copy
					{
						//	designer:str/JUKAA01
						public static global::Epsitec.Common.Types.FormattedText EmptySelection
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544650));
							}
						}
					}
					
					public static class Paste
					{
						//	designer:str/JUKCA01
						public static global::Epsitec.Common.Types.FormattedText Empty
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544652));
							}
						}
						//	designer:str/JUKBA01
						public static global::Epsitec.Common.Types.FormattedText Wrong
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544651));
							}
						}
					}
					
					public static class Row
					{
						//	designer:str/JUKGA01
						public static global::Epsitec.Common.Types.FormattedText Days
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544656));
							}
						}
						//	designer:str/JUKHA01
						public static global::Epsitec.Common.Types.FormattedText DaysMonths
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544657));
							}
						}
						//	designer:str/JUKFA01
						public static global::Epsitec.Common.Types.FormattedText Events
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544655));
							}
						}
						//	designer:str/JUKJA01
						public static global::Epsitec.Common.Types.FormattedText Months
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544659));
							}
						}
						//	designer:str/JUKEA01
						public static global::Epsitec.Common.Types.FormattedText Values
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544654));
							}
						}
						//	designer:str/JUKIA01
						public static global::Epsitec.Common.Types.FormattedText WeekOfYear
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544658));
							}
						}
						//	designer:str/JUKKA01
						public static global::Epsitec.Common.Types.FormattedText Years
						{
							get
							{
								return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544660));
							}
						}
					}
				}
			}
			
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
				return new global::Epsitec.Common.Types.FormattedText (global::Epsitec.Cresus.Assets.App.Res.Strings.GetString (bundle, path));
			}
			
			private static global::Epsitec.Common.Types.FormattedText GetText(global::Epsitec.Common.Support.Druid druid)
			{
				return new global::Epsitec.Common.Types.FormattedText (global::Epsitec.Cresus.Assets.App.Res.Strings.GetString (druid));
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
		
		public static class StringIds
		{
			public static class AccountsImport
			{
				public static class Message
				{
					//	designer:str/JUK5601
					public static global::Epsitec.Common.Support.Druid Equal
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544517);
						}
					}
					//	designer:str/JUK7601
					public static global::Epsitec.Common.Support.Druid New
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544519);
						}
					}
					//	designer:str/JUK6601
					public static global::Epsitec.Common.Support.Druid Update
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544518);
						}
					}
				}
			}
			
			public static class DataFillers
			{
				public static class LastViewsTreeTable
				{
					//	designer:str/JUKG501
					public static global::Epsitec.Common.Support.Druid Date
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544496);
						}
					}
					//	designer:str/JUKH501
					public static global::Epsitec.Common.Support.Druid Description
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544497);
						}
					}
					//	designer:str/JUKF501
					public static global::Epsitec.Common.Support.Druid Page
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544495);
						}
					}
					//	designer:str/JUKE501
					public static global::Epsitec.Common.Support.Druid Type
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544494);
						}
					}
				}
				
				public static class MessagesTreeTable
				{
					//	designer:str/JUKI501
					public static global::Epsitec.Common.Support.Druid Title
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544498);
						}
					}
				}
				
				public static class WarningsTreeTable
				{
					public static class Date
					{
						//	designer:str/JUKM501
						public static global::Epsitec.Common.Support.Druid Title
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544502);
							}
						}
						//	designer:str/JUKN501
						public static global::Epsitec.Common.Support.Druid Tooltip
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544503);
							}
						}
					}
					
					public static class Description
					{
						//	designer:str/JUKQ501
						public static global::Epsitec.Common.Support.Druid Title
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544506);
							}
						}
					}
					
					public static class EventGlyph
					{
						//	designer:str/JUKO501
						public static global::Epsitec.Common.Support.Druid Tooltip
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544504);
							}
						}
					}
					
					public static class Field
					{
						//	designer:str/JUKP501
						public static global::Epsitec.Common.Support.Druid Title
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544505);
							}
						}
					}
					
					public static class Glyph
					{
						//	designer:str/JUKJ501
						public static global::Epsitec.Common.Support.Druid Title
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544499);
							}
						}
						//	designer:str/JUKK501
						public static global::Epsitec.Common.Support.Druid Tooltip
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544500);
							}
						}
					}
					
					public static class Object
					{
						//	designer:str/JUKL501
						public static global::Epsitec.Common.Support.Druid Title
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544501);
							}
						}
					}
				}
			}
			
			public static class DateRange
			{
				//	designer:str/JUK3601
				public static global::Epsitec.Common.Support.Druid FromTo
				{
					get
					{
						return global::Epsitec.Common.Support.Druid.FromFieldId (335544515);
					}
				}
			}
			
			public static class DateTime
			{
				//	designer:str/JUK4601
				public static global::Epsitec.Common.Support.Druid WeekOfYear
				{
					get
					{
						return global::Epsitec.Common.Support.Druid.FromFieldId (335544516);
					}
				}
			}
			
			public static class EditorPages
			{
				public static class AmortizationDefinition
				{
					//	designer:str/JUK4901
					public static global::Epsitec.Common.Support.Druid Import
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544612);
						}
					}
					//	designer:str/JUK5901
					public static global::Epsitec.Common.Support.Druid ImportHelp
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544613);
						}
					}
				}
				
				public static class Category
				{
					//	designer:str/JUK3901
					public static global::Epsitec.Common.Support.Druid AccountsSubtitle
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544611);
						}
					}
					//	designer:str/JUK6901
					public static global::Epsitec.Common.Support.Druid CalculatorButton
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544614);
						}
					}
				}
				
				public static class ColorsExplanation
				{
					public static class Automatic
					{
						//	designer:str/JUKP801
						public static global::Epsitec.Common.Support.Druid Description
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544601);
							}
						}
						//	designer:str/JUKQ801
						public static global::Epsitec.Common.Support.Druid Tooltip
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544602);
							}
						}
					}
					
					public static class Defined
					{
						//	designer:str/JUKR801
						public static global::Epsitec.Common.Support.Druid Description
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544603);
							}
						}
						//	designer:str/JUKS801
						public static global::Epsitec.Common.Support.Druid Tooltip
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544604);
							}
						}
					}
					
					public static class Editable
					{
						//	designer:str/JUKN801
						public static global::Epsitec.Common.Support.Druid Description
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544599);
							}
						}
						//	designer:str/JUKO801
						public static global::Epsitec.Common.Support.Druid Tooltip
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544600);
							}
						}
					}
					
					public static class Error
					{
						//	designer:str/JUK1901
						public static global::Epsitec.Common.Support.Druid Description
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544609);
							}
						}
						//	designer:str/JUK2901
						public static global::Epsitec.Common.Support.Druid Tooltip
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544610);
							}
						}
					}
					
					public static class Readonly
					{
						//	designer:str/JUKT801
						public static global::Epsitec.Common.Support.Druid Description
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544605);
							}
						}
						//	designer:str/JUKU801
						public static global::Epsitec.Common.Support.Druid Tooltip
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544606);
							}
						}
					}
					
					public static class Result
					{
						//	designer:str/JUKV801
						public static global::Epsitec.Common.Support.Druid Description
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544607);
							}
						}
						//	designer:str/JUK0901
						public static global::Epsitec.Common.Support.Druid Tooltip
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544608);
							}
						}
					}
				}
				
				public static class OneShot
				{
					//	designer:str/JUK7901
					public static global::Epsitec.Common.Support.Druid Info
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544615);
						}
					}
				}
				
				public static class Summary
				{
					//	designer:str/JUKC901
					public static global::Epsitec.Common.Support.Druid Amortizations
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544620);
						}
					}
					//	designer:str/JUK8901
					public static global::Epsitec.Common.Support.Druid Event
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544616);
						}
					}
					//	designer:str/JUK9901
					public static global::Epsitec.Common.Support.Druid Groups
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544617);
						}
					}
					//	designer:str/JUKA901
					public static global::Epsitec.Common.Support.Druid Main
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544618);
						}
					}
					//	designer:str/JUKB901
					public static global::Epsitec.Common.Support.Druid MainValue
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544619);
						}
					}
				}
				
				public static class UserFields
				{
					//	designer:str/JUKE901
					public static global::Epsitec.Common.Support.Druid Edition
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544622);
						}
					}
					//	designer:str/JUKF901
					public static global::Epsitec.Common.Support.Druid Summary
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544623);
						}
					}
					//	designer:str/JUKD901
					public static global::Epsitec.Common.Support.Druid TreeTable
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544621);
						}
					}
				}
			}
			
			public static class Encoding
			{
				//	designer:str/JUK0601
				public static global::Epsitec.Common.Support.Druid Ascii
				{
					get
					{
						return global::Epsitec.Common.Support.Druid.FromFieldId (335544512);
					}
				}
				//	designer:str/JUKV501
				public static global::Epsitec.Common.Support.Druid BigEndianUnicode
				{
					get
					{
						return global::Epsitec.Common.Support.Druid.FromFieldId (335544511);
					}
				}
				//	designer:str/JUKU501
				public static global::Epsitec.Common.Support.Druid Unicode
				{
					get
					{
						return global::Epsitec.Common.Support.Druid.FromFieldId (335544510);
					}
				}
				//	designer:str/JUKT501
				public static global::Epsitec.Common.Support.Druid UTF32
				{
					get
					{
						return global::Epsitec.Common.Support.Druid.FromFieldId (335544509);
					}
				}
				//	designer:str/JUKR501
				public static global::Epsitec.Common.Support.Druid UTF7
				{
					get
					{
						return global::Epsitec.Common.Support.Druid.FromFieldId (335544507);
					}
				}
				//	designer:str/JUKS501
				public static global::Epsitec.Common.Support.Druid UTF8
				{
					get
					{
						return global::Epsitec.Common.Support.Druid.FromFieldId (335544508);
					}
				}
			}
			
			public static class Event
			{
				public static class AmortizationExtra
				{
					//	designer:str/JUK6201
					public static global::Epsitec.Common.Support.Druid Help
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544390);
						}
					}
					//	designer:str/JUKU101
					public static global::Epsitec.Common.Support.Druid ShortName
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544382);
						}
					}
				}
				
				public static class Input
				{
					//	designer:str/JUK1201
					public static global::Epsitec.Common.Support.Druid Help
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544385);
						}
					}
					//	designer:str/JUKP101
					public static global::Epsitec.Common.Support.Druid ShortName
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544377);
						}
					}
				}
				
				public static class Locked
				{
					//	designer:str/JUK7201
					public static global::Epsitec.Common.Support.Druid Help
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544391);
						}
					}
					//	designer:str/JUKV101
					public static global::Epsitec.Common.Support.Druid ShortName
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544383);
						}
					}
				}
				
				public static class MainValue
				{
					//	designer:str/JUK5201
					public static global::Epsitec.Common.Support.Druid Help
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544389);
						}
					}
					//	designer:str/JUKT101
					public static global::Epsitec.Common.Support.Druid ShortName
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544381);
						}
					}
				}
				
				public static class Modification
				{
					//	designer:str/JUK2201
					public static global::Epsitec.Common.Support.Druid Help
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544386);
						}
					}
					//	designer:str/JUKQ101
					public static global::Epsitec.Common.Support.Druid ShortName
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544378);
						}
					}
				}
				
				public static class Output
				{
					//	designer:str/JUK8201
					public static global::Epsitec.Common.Support.Druid Help
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544392);
						}
					}
					//	designer:str/JUK0201
					public static global::Epsitec.Common.Support.Druid ShortName
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544384);
						}
					}
				}
				
				public static class Revalorization
				{
					//	designer:str/JUK4201
					public static global::Epsitec.Common.Support.Druid Help
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544388);
						}
					}
					//	designer:str/JUKS101
					public static global::Epsitec.Common.Support.Druid ShortName
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544380);
						}
					}
				}
				
				public static class Revaluation
				{
					//	designer:str/JUK3201
					public static global::Epsitec.Common.Support.Druid Help
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544387);
						}
					}
					//	designer:str/JUKR101
					public static global::Epsitec.Common.Support.Druid ShortName
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544379);
						}
					}
				}
			}
			
			public static class Export
			{
				public static class Engine
				{
					//	designer:str/JUK1601
					public static global::Epsitec.Common.Support.Druid UnknownFormat
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544513);
						}
					}
				}
			}
			
			public static class FieldControllers
			{
				//	designer:str/JUKH901
				public static global::Epsitec.Common.Support.Druid ClearModification
				{
					get
					{
						return global::Epsitec.Common.Support.Druid.FromFieldId (335544625);
					}
				}
				//	designer:str/JUKG901
				public static global::Epsitec.Common.Support.Druid ShowHistory
				{
					get
					{
						return global::Epsitec.Common.Support.Druid.FromFieldId (335544624);
					}
				}
				public static class Account
				{
					//	designer:str/JUKI901
					public static global::Epsitec.Common.Support.Druid Goto
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544626);
						}
					}
				}
				
				public static class Date
				{
					//	designer:str/JUKJ901
					public static global::Epsitec.Common.Support.Druid Begin
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544627);
						}
					}
					//	designer:str/JUKN901
					public static global::Epsitec.Common.Support.Druid Calendar
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544631);
						}
					}
					//	designer:str/JUKO901
					public static global::Epsitec.Common.Support.Druid Delete
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544632);
						}
					}
					//	designer:str/JUKL901
					public static global::Epsitec.Common.Support.Druid End
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544629);
						}
					}
					//	designer:str/JUKQ901
					public static global::Epsitec.Common.Support.Druid NextDay
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544634);
						}
					}
					//	designer:str/JUKS901
					public static global::Epsitec.Common.Support.Druid NextMonth
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544636);
						}
					}
					//	designer:str/JUKU901
					public static global::Epsitec.Common.Support.Druid NextYear
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544638);
						}
					}
					//	designer:str/JUKK901
					public static global::Epsitec.Common.Support.Druid Now
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544628);
						}
					}
					//	designer:str/JUKM901
					public static global::Epsitec.Common.Support.Druid Predefined
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544630);
						}
					}
					//	designer:str/JUKP901
					public static global::Epsitec.Common.Support.Druid PrevDay
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544633);
						}
					}
					//	designer:str/JUKR901
					public static global::Epsitec.Common.Support.Druid PrevMonth
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544635);
						}
					}
					//	designer:str/JUKT901
					public static global::Epsitec.Common.Support.Druid PrevYear
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544637);
						}
					}
				}
				
				public static class GuidRatio
				{
					//	designer:str/JUKV901
					public static global::Epsitec.Common.Support.Druid List
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544639);
						}
					}
					//	designer:str/JUK0A01
					public static global::Epsitec.Common.Support.Druid New
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544640);
						}
					}
				}
				
				public static class Person
				{
					//	designer:str/JUK1A01
					public static global::Epsitec.Common.Support.Druid Goto
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544641);
						}
					}
				}
			}
			
			public static class Popup
			{
				public static class Accounts
				{
					//	designer:str/JUKJ001
					public static global::Epsitec.Common.Support.Druid Choice
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544339);
						}
					}
					//	designer:str/JUKI001
					public static global::Epsitec.Common.Support.Druid NoAccounts
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544338);
						}
					}
				}
				
				public static class AccountsImport
				{
					//	designer:str/JUKH001
					public static global::Epsitec.Common.Support.Druid File
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544337);
						}
					}
				}
				
				public static class Amortizations
				{
					//	designer:str/JUKL001
					public static global::Epsitec.Common.Support.Druid FromDate
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544341);
						}
					}
					//	designer:str/JUKN001
					public static global::Epsitec.Common.Support.Druid Object
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544343);
						}
					}
					//	designer:str/JUKM001
					public static global::Epsitec.Common.Support.Druid ToDate
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544342);
						}
					}
					public static class Delete
					{
						//	designer:str/JUK3101
						public static global::Epsitec.Common.Support.Druid All
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544355);
							}
						}
						//	designer:str/JUK2101
						public static global::Epsitec.Common.Support.Druid One
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544354);
							}
						}
						//	designer:str/JUK1101
						public static global::Epsitec.Common.Support.Druid Title
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544353);
							}
						}
					}
					
					public static class Fix
					{
						//	designer:str/JUKT001
						public static global::Epsitec.Common.Support.Druid All
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544349);
							}
						}
						//	designer:str/JUKS001
						public static global::Epsitec.Common.Support.Druid One
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544348);
							}
						}
						//	designer:str/JUKR001
						public static global::Epsitec.Common.Support.Druid Title
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544347);
							}
						}
					}
					
					public static class Preview
					{
						//	designer:str/JUKQ001
						public static global::Epsitec.Common.Support.Druid All
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544346);
							}
						}
						//	designer:str/JUKP001
						public static global::Epsitec.Common.Support.Druid One
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544345);
							}
						}
						//	designer:str/JUKO001
						public static global::Epsitec.Common.Support.Druid Title
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544344);
							}
						}
					}
					
					public static class ToExtra
					{
						//	designer:str/JUKE701
						public static global::Epsitec.Common.Support.Druid Title
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544558);
							}
						}
					}
					
					public static class Unpreview
					{
						//	designer:str/JUK0101
						public static global::Epsitec.Common.Support.Druid All
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544352);
							}
						}
						//	designer:str/JUKV001
						public static global::Epsitec.Common.Support.Druid One
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544351);
							}
						}
						//	designer:str/JUKU001
						public static global::Epsitec.Common.Support.Druid Title
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544350);
							}
						}
					}
				}
				
				public static class AssetCopy
				{
					//	designer:str/JUK7101
					public static global::Epsitec.Common.Support.Druid StateDate
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544359);
						}
					}
					//	designer:str/JUK5101
					public static global::Epsitec.Common.Support.Druid StateGlobal
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544357);
						}
					}
					//	designer:str/JUK6101
					public static global::Epsitec.Common.Support.Druid StateInput
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544358);
						}
					}
					//	designer:str/JUK4101
					public static global::Epsitec.Common.Support.Druid Title
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544356);
						}
					}
				}
				
				public static class AssetPaste
				{
					//	designer:str/JUK9101
					public static global::Epsitec.Common.Support.Druid Date
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544361);
						}
					}
					//	designer:str/JUK8101
					public static global::Epsitec.Common.Support.Druid Title
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544360);
						}
					}
				}
				
				public static class AssetsReport
				{
					//	designer:str/JUKC101
					public static global::Epsitec.Common.Support.Druid Group
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544364);
						}
					}
					//	designer:str/JUKD101
					public static global::Epsitec.Common.Support.Druid Level
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544365);
						}
					}
					//	designer:str/JUKB101
					public static global::Epsitec.Common.Support.Druid State
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544363);
						}
					}
					//	designer:str/JUKA101
					public static global::Epsitec.Common.Support.Druid Title
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544362);
						}
					}
				}
				
				public static class Button
				{
					//	designer:str/JUK6001
					public static global::Epsitec.Common.Support.Druid Cancel
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544326);
						}
					}
					//	designer:str/JUKD001
					public static global::Epsitec.Common.Support.Druid Close
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544333);
						}
					}
					//	designer:str/JUKE001
					public static global::Epsitec.Common.Support.Druid Compute
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544334);
						}
					}
					//	designer:str/JUK8001
					public static global::Epsitec.Common.Support.Druid Copy
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544328);
						}
					}
					//	designer:str/JUK4001
					public static global::Epsitec.Common.Support.Druid Create
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544324);
						}
					}
					//	designer:str/JUKB001
					public static global::Epsitec.Common.Support.Druid Export
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544331);
						}
					}
					//	designer:str/JUK7001
					public static global::Epsitec.Common.Support.Druid Import
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544327);
						}
					}
					//	designer:str/JUKG001
					public static global::Epsitec.Common.Support.Druid No
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544336);
						}
					}
					//	designer:str/JUK5001
					public static global::Epsitec.Common.Support.Druid Ok
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544325);
						}
					}
					//	designer:str/JUKC001
					public static global::Epsitec.Common.Support.Druid Open
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544332);
						}
					}
					//	designer:str/JUK9001
					public static global::Epsitec.Common.Support.Druid Paste
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544329);
						}
					}
					//	designer:str/JUKA001
					public static global::Epsitec.Common.Support.Druid Show
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544330);
						}
					}
					//	designer:str/JUKF001
					public static global::Epsitec.Common.Support.Druid Yes
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544335);
						}
					}
				}
				
				public static class Calendar
				{
					//	designer:str/JUKE101
					public static global::Epsitec.Common.Support.Druid Title
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544366);
						}
					}
				}
				
				public static class Categories
				{
					//	designer:str/JUKF101
					public static global::Epsitec.Common.Support.Druid Title
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544367);
						}
					}
				}
				
				public static class CreateAsset
				{
					//	designer:str/JUKK101
					public static global::Epsitec.Common.Support.Druid Category
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544372);
						}
					}
					//	designer:str/JUKH101
					public static global::Epsitec.Common.Support.Druid Date
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544369);
						}
					}
					//	designer:str/JUKI101
					public static global::Epsitec.Common.Support.Druid Name
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544370);
						}
					}
					//	designer:str/JUKG101
					public static global::Epsitec.Common.Support.Druid Title
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544368);
						}
					}
					//	designer:str/JUKJ101
					public static global::Epsitec.Common.Support.Druid Value
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544371);
						}
					}
				}
				
				public static class CreateCategory
				{
					//	designer:str/JUKN101
					public static global::Epsitec.Common.Support.Druid Model
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544375);
						}
					}
					//	designer:str/JUKM101
					public static global::Epsitec.Common.Support.Druid Name
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544374);
						}
					}
					//	designer:str/JUKL101
					public static global::Epsitec.Common.Support.Druid Title
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544373);
						}
					}
				}
				
				public static class CreateEvent
				{
					//	designer:str/JUKO101
					public static global::Epsitec.Common.Support.Druid Title
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544376);
						}
					}
				}
				
				public static class CreateGroup
				{
					//	designer:str/JUKA201
					public static global::Epsitec.Common.Support.Druid Name
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544394);
						}
					}
					//	designer:str/JUKB201
					public static global::Epsitec.Common.Support.Druid Parent
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544395);
						}
					}
					//	designer:str/JUK9201
					public static global::Epsitec.Common.Support.Druid Title
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544393);
						}
					}
				}
				
				public static class CreatePerson
				{
					//	designer:str/JUKE201
					public static global::Epsitec.Common.Support.Druid Model
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544398);
						}
					}
					//	designer:str/JUKD201
					public static global::Epsitec.Common.Support.Druid Name
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544397);
						}
					}
					//	designer:str/JUKC201
					public static global::Epsitec.Common.Support.Druid Title
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544396);
						}
					}
				}
				
				public static class Date
				{
					//	designer:str/JUKF201
					public static global::Epsitec.Common.Support.Druid Title
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544399);
						}
					}
				}
				
				public static class Errors
				{
					//	designer:str/JUKG201
					public static global::Epsitec.Common.Support.Druid Title
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544400);
						}
					}
				}
				
				public static class EventPaste
				{
					//	designer:str/JUKI201
					public static global::Epsitec.Common.Support.Druid Date
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544402);
						}
					}
					//	designer:str/JUKH201
					public static global::Epsitec.Common.Support.Druid Title
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544401);
						}
					}
				}
				
				public static class Export
				{
					//	designer:str/JUKM201
					public static global::Epsitec.Common.Support.Druid CamelCase
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544406);
						}
					}
					//	designer:str/JUKO201
					public static global::Epsitec.Common.Support.Druid Encoding
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544408);
						}
					}
					//	designer:str/JUKN201
					public static global::Epsitec.Common.Support.Druid EndOfLines
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544407);
						}
					}
				}
				
				public static class ExportInstructions
				{
					//	designer:str/JUKK201
					public static global::Epsitec.Common.Support.Druid Filename
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544404);
						}
					}
					//	designer:str/JUKJ201
					public static global::Epsitec.Common.Support.Druid Title
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544403);
						}
					}
				}
				
				public static class ExportJson
				{
					//	designer:str/JUKL201
					public static global::Epsitec.Common.Support.Druid Title
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544405);
						}
					}
				}
				
				public static class ExportOpen
				{
					//	designer:str/JUKQ201
					public static global::Epsitec.Common.Support.Druid Radios
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544410);
						}
					}
					//	designer:str/JUKP201
					public static global::Epsitec.Common.Support.Druid Title
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544409);
						}
					}
				}
				
				public static class ExportPdf
				{
					//	designer:str/JUK2301
					public static global::Epsitec.Common.Support.Druid AutomaticColumnWidths
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544418);
						}
					}
					//	designer:str/JUK1301
					public static global::Epsitec.Common.Support.Druid CellMargins
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544417);
						}
					}
					//	designer:str/JUKV201
					public static global::Epsitec.Common.Support.Druid FontName
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544415);
						}
					}
					//	designer:str/JUK0301
					public static global::Epsitec.Common.Support.Druid FontSize
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544416);
						}
					}
					//	designer:str/JUK4301
					public static global::Epsitec.Common.Support.Druid Footer
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544420);
						}
					}
					//	designer:str/JUK3301
					public static global::Epsitec.Common.Support.Druid Header
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544419);
						}
					}
					//	designer:str/JUK5301
					public static global::Epsitec.Common.Support.Druid Indent
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544421);
						}
					}
					//	designer:str/JUKT201
					public static global::Epsitec.Common.Support.Druid PageFormat
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544413);
						}
					}
					//	designer:str/JUKU201
					public static global::Epsitec.Common.Support.Druid PageMargins
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544414);
						}
					}
					//	designer:str/JUKS201
					public static global::Epsitec.Common.Support.Druid Style
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544412);
						}
					}
					//	designer:str/JUKR201
					public static global::Epsitec.Common.Support.Druid Title
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544411);
						}
					}
					//	designer:str/JUK6301
					public static global::Epsitec.Common.Support.Druid Watermark
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544422);
						}
					}
				}
				
				public static class ExportText
				{
					//	designer:str/JUKB301
					public static global::Epsitec.Common.Support.Druid ColumnBracket
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544427);
						}
					}
					//	designer:str/JUKA301
					public static global::Epsitec.Common.Support.Druid ColumnSeparator
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544426);
						}
					}
					//	designer:str/JUKC301
					public static global::Epsitec.Common.Support.Druid Escape
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544428);
						}
					}
					//	designer:str/JUK8301
					public static global::Epsitec.Common.Support.Druid HasHeader
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544424);
						}
					}
					//	designer:str/JUK9301
					public static global::Epsitec.Common.Support.Druid Inverted
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544425);
						}
					}
					//	designer:str/JUK7301
					public static global::Epsitec.Common.Support.Druid Title
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544423);
						}
					}
				}
				
				public static class ExportXml
				{
					//	designer:str/JUKE301
					public static global::Epsitec.Common.Support.Druid BodyTag
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544430);
						}
					}
					//	designer:str/JUKD301
					public static global::Epsitec.Common.Support.Druid Compact
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544429);
						}
					}
					//	designer:str/JUKG301
					public static global::Epsitec.Common.Support.Druid Indent
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544432);
						}
					}
					//	designer:str/JUKF301
					public static global::Epsitec.Common.Support.Druid RecordTag
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544431);
						}
					}
				}
				
				public static class ExportYaml
				{
					//	designer:str/JUKH301
					public static global::Epsitec.Common.Support.Druid Indent
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544433);
						}
					}
				}
				
				public static class Filter
				{
					//	designer:str/JUKJ301
					public static global::Epsitec.Common.Support.Druid GroupCancel
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544435);
						}
					}
					//	designer:str/JUKI301
					public static global::Epsitec.Common.Support.Druid Title
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544434);
						}
					}
				}
				
				public static class Groups
				{
					//	designer:str/JUKK301
					public static global::Epsitec.Common.Support.Druid Title
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544436);
						}
					}
				}
				
				public static class History
				{
					//	designer:str/JUKM301
					public static global::Epsitec.Common.Support.Druid Title
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544438);
						}
					}
					//	designer:str/JUKL301
					public static global::Epsitec.Common.Support.Druid Undefined
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544437);
						}
					}
				}
				
				public static class LastViews
				{
					//	designer:str/JUKN301
					public static global::Epsitec.Common.Support.Druid Title
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544439);
						}
					}
				}
				
				public static class Locked
				{
					//	designer:str/JUKP301
					public static global::Epsitec.Common.Support.Druid Date
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544441);
						}
					}
					//	designer:str/JUKO301
					public static global::Epsitec.Common.Support.Druid Title
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544440);
						}
					}
					public static class Button
					{
						//	designer:str/JUKU301
						public static global::Epsitec.Common.Support.Druid LockAll
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544446);
							}
						}
						//	designer:str/JUKV301
						public static global::Epsitec.Common.Support.Druid LockOne
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544447);
							}
						}
						//	designer:str/JUKS301
						public static global::Epsitec.Common.Support.Druid UnlockAll
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544444);
							}
						}
						//	designer:str/JUKT301
						public static global::Epsitec.Common.Support.Druid UnlockOne
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544445);
							}
						}
					}
					
					public static class Radios
					{
						//	designer:str/JUKR301
						public static global::Epsitec.Common.Support.Druid IsAll
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544443);
							}
						}
						//	designer:str/JUKQ301
						public static global::Epsitec.Common.Support.Druid IsDelete
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544442);
							}
						}
					}
				}
				
				public static class Margins
				{
					//	designer:str/JUK4401
					public static global::Epsitec.Common.Support.Druid Bottom
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544452);
						}
					}
					//	designer:str/JUK1401
					public static global::Epsitec.Common.Support.Druid Left
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544449);
						}
					}
					//	designer:str/JUK2401
					public static global::Epsitec.Common.Support.Druid Right
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544450);
						}
					}
					//	designer:str/JUK3401
					public static global::Epsitec.Common.Support.Druid Top
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544451);
						}
					}
					//	designer:str/JUK0401
					public static global::Epsitec.Common.Support.Druid Unified
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544448);
						}
					}
				}
				
				public static class MCH2SummaryReport
				{
					//	designer:str/JUK7401
					public static global::Epsitec.Common.Support.Druid FinalDate
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544455);
						}
					}
					//	designer:str/JUK9401
					public static global::Epsitec.Common.Support.Druid GroupEnable
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544457);
						}
					}
					//	designer:str/JUK6401
					public static global::Epsitec.Common.Support.Druid InitialDate
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544454);
						}
					}
					//	designer:str/JUKA401
					public static global::Epsitec.Common.Support.Druid Level
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544458);
						}
					}
					//	designer:str/JUK8401
					public static global::Epsitec.Common.Support.Druid MonthCount
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544456);
						}
					}
					//	designer:str/JUK5401
					public static global::Epsitec.Common.Support.Druid Title
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544453);
						}
					}
				}
				
				public static class Message
				{
					//	designer:str/JUKF401
					public static global::Epsitec.Common.Support.Druid ErrorTitle
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544463);
						}
					}
					//	designer:str/JUKH401
					public static global::Epsitec.Common.Support.Druid MessageTitle
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544465);
						}
					}
					//	designer:str/JUKB401
					public static global::Epsitec.Common.Support.Druid WarningTitle
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544459);
						}
					}
					public static class AccountsImport
					{
						//	designer:str/JUK8601
						public static global::Epsitec.Common.Support.Druid Title
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544520);
							}
						}
					}
					
					public static class DeleteEventWarning
					{
						//	designer:str/JUKC401
						public static global::Epsitec.Common.Support.Druid Text
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544460);
							}
						}
					}
					
					public static class ExportError
					{
						//	designer:str/JUK2601
						public static global::Epsitec.Common.Support.Druid Text
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544514);
							}
						}
					}
					
					public static class PasteError
					{
						//	designer:str/JUKI401
						public static global::Epsitec.Common.Support.Druid Text
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544466);
							}
						}
					}
					
					public static class PreviewEventWarning
					{
						//	designer:str/JUKE401
						public static global::Epsitec.Common.Support.Druid Text
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544462);
							}
						}
					}
					
					public static class Todo
					{
						//	designer:str/JUKG401
						public static global::Epsitec.Common.Support.Druid Text
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544464);
							}
						}
					}
				}
				
				public static class NewMandat
				{
					//	designer:str/JUK3001
					public static global::Epsitec.Common.Support.Druid Date
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544323);
						}
					}
					//	designer:str/JUK2001
					public static global::Epsitec.Common.Support.Druid Name
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544322);
						}
					}
					//	designer:str/JUK1001
					public static global::Epsitec.Common.Support.Druid Sample
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544321);
						}
					}
					//	designer:str/JUK0001
					public static global::Epsitec.Common.Support.Druid Title
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544320);
						}
					}
				}
				
				public static class PageSize
				{
					//	designer:str/JUKK401
					public static global::Epsitec.Common.Support.Druid Format
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544468);
						}
					}
					//	designer:str/JUKN401
					public static global::Epsitec.Common.Support.Druid Height
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544471);
						}
					}
					//	designer:str/JUKL401
					public static global::Epsitec.Common.Support.Druid Radios
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544469);
						}
					}
					//	designer:str/JUKJ401
					public static global::Epsitec.Common.Support.Druid Title
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544467);
						}
					}
					//	designer:str/JUKM401
					public static global::Epsitec.Common.Support.Druid Width
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544470);
						}
					}
					public static class Description
					{
						//	designer:str/JUKP401
						public static global::Epsitec.Common.Support.Druid Landscape
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544473);
							}
						}
						//	designer:str/JUKO401
						public static global::Epsitec.Common.Support.Druid Portrait
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544472);
							}
						}
					}
				}
				
				public static class PdfStyle
				{
					//	designer:str/JUK6501
					public static global::Epsitec.Common.Support.Druid BorderColor
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544486);
						}
					}
					//	designer:str/JUK4501
					public static global::Epsitec.Common.Support.Druid EvenColor
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544484);
						}
					}
					//	designer:str/JUK3501
					public static global::Epsitec.Common.Support.Druid LabelColor
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544483);
						}
					}
					//	designer:str/JUK5501
					public static global::Epsitec.Common.Support.Druid OddColor
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544485);
						}
					}
					//	designer:str/JUK2501
					public static global::Epsitec.Common.Support.Druid Predefined
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544482);
						}
					}
					//	designer:str/JUK7501
					public static global::Epsitec.Common.Support.Druid Thickness
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544487);
						}
					}
					//	designer:str/JUK1501
					public static global::Epsitec.Common.Support.Druid Title
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544481);
						}
					}
				}
				
				public static class Persons
				{
					//	designer:str/JUK8501
					public static global::Epsitec.Common.Support.Druid Title
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544488);
						}
					}
				}
				
				public static class RateCalculator
				{
					//	designer:str/JUKB501
					public static global::Epsitec.Common.Support.Druid Result
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544491);
						}
					}
					//	designer:str/JUK9501
					public static global::Epsitec.Common.Support.Druid Title
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544489);
						}
					}
					//	designer:str/JUKA501
					public static global::Epsitec.Common.Support.Druid TotalYears
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544490);
						}
					}
				}
				
				public static class YesNo
				{
					//	designer:str/JUKD501
					public static global::Epsitec.Common.Support.Druid DeleteEventQuestion
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544493);
						}
					}
					//	designer:str/JUKC501
					public static global::Epsitec.Common.Support.Druid Title
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544492);
						}
					}
				}
			}
			
			public static class Toolbar
			{
				public static class Edit
				{
					//	designer:str/JUK9601
					public static global::Epsitec.Common.Support.Druid Accept
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544521);
						}
					}
					//	designer:str/JUKA601
					public static global::Epsitec.Common.Support.Druid Cancel
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544522);
						}
					}
				}
				
				public static class Main
				{
					//	designer:str/JUKO601
					public static global::Epsitec.Common.Support.Druid Accept
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544536);
						}
					}
					//	designer:str/JUKN601
					public static global::Epsitec.Common.Support.Druid Cancel
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544535);
						}
					}
					//	designer:str/JUKP601
					public static global::Epsitec.Common.Support.Druid ChoiceSettings
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544537);
						}
					}
					//	designer:str/JUKK601
					public static global::Epsitec.Common.Support.Druid Edit
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544532);
						}
					}
					//	designer:str/JUKL601
					public static global::Epsitec.Common.Support.Druid Locked
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544533);
						}
					}
					//	designer:str/JUKB601
					public static global::Epsitec.Common.Support.Druid New
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544523);
						}
					}
					//	designer:str/JUKC601
					public static global::Epsitec.Common.Support.Druid Open
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544524);
						}
					}
					//	designer:str/JUKD601
					public static global::Epsitec.Common.Support.Druid Save
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544525);
						}
					}
					//	designer:str/JUKM601
					public static global::Epsitec.Common.Support.Druid Simulation
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544534);
						}
					}
					public static class Navigate
					{
						//	designer:str/JUKE601
						public static global::Epsitec.Common.Support.Druid Back
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544526);
							}
						}
						//	designer:str/JUKF601
						public static global::Epsitec.Common.Support.Druid Forward
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544527);
							}
						}
						//	designer:str/JUKG601
						public static global::Epsitec.Common.Support.Druid Menu
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544528);
							}
						}
					}
					
					public static class Show
					{
						//	designer:str/JUKI601
						public static global::Epsitec.Common.Support.Druid TimelineEvent
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544530);
							}
						}
						//	designer:str/JUKJ601
						public static global::Epsitec.Common.Support.Druid TimelineMultiple
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544531);
							}
						}
						//	designer:str/JUKH601
						public static global::Epsitec.Common.Support.Druid TimelineSingle
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544529);
							}
						}
					}
				}
				
				public static class Reports
				{
					//	designer:str/JUK2701
					public static global::Epsitec.Common.Support.Druid Close
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544546);
						}
					}
					//	designer:str/JUKR601
					public static global::Epsitec.Common.Support.Druid CompactAll
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544539);
						}
					}
					//	designer:str/JUKS601
					public static global::Epsitec.Common.Support.Druid CompactOne
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544540);
						}
					}
					//	designer:str/JUKU601
					public static global::Epsitec.Common.Support.Druid ExpandAll
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544542);
						}
					}
					//	designer:str/JUKT601
					public static global::Epsitec.Common.Support.Druid ExpandOne
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544541);
						}
					}
					//	designer:str/JUK1701
					public static global::Epsitec.Common.Support.Druid Export
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544545);
						}
					}
					//	designer:str/JUK0701
					public static global::Epsitec.Common.Support.Druid NextPeriod
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544544);
						}
					}
					//	designer:str/JUKQ601
					public static global::Epsitec.Common.Support.Druid Params
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544538);
						}
					}
					//	designer:str/JUKV601
					public static global::Epsitec.Common.Support.Druid PrevPeriod
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544543);
						}
					}
				}
				
				public static class Timeline
				{
					//	designer:str/JUKG701
					public static global::Epsitec.Common.Support.Druid Compacted
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544560);
						}
					}
					//	designer:str/JUKU701
					public static global::Epsitec.Common.Support.Druid Copy
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544574);
						}
					}
					//	designer:str/JUKQ701
					public static global::Epsitec.Common.Support.Druid Date
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544570);
						}
					}
					//	designer:str/JUKJ701
					public static global::Epsitec.Common.Support.Druid DaysOfWeek
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544563);
						}
					}
					//	designer:str/JUKS701
					public static global::Epsitec.Common.Support.Druid Delete
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544572);
						}
					}
					//	designer:str/JUKT701
					public static global::Epsitec.Common.Support.Druid Deselect
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544573);
						}
					}
					//	designer:str/JUKH701
					public static global::Epsitec.Common.Support.Druid Expanded
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544561);
						}
					}
					//	designer:str/JUKL701
					public static global::Epsitec.Common.Support.Druid First
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544565);
						}
					}
					//	designer:str/JUKK701
					public static global::Epsitec.Common.Support.Druid Graph
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544564);
						}
					}
					//	designer:str/JUKF701
					public static global::Epsitec.Common.Support.Druid Labels
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544559);
						}
					}
					//	designer:str/JUKO701
					public static global::Epsitec.Common.Support.Druid Last
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544568);
						}
					}
					//	designer:str/JUKR701
					public static global::Epsitec.Common.Support.Druid New
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544571);
						}
					}
					//	designer:str/JUKN701
					public static global::Epsitec.Common.Support.Druid Next
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544567);
						}
					}
					//	designer:str/JUKP701
					public static global::Epsitec.Common.Support.Druid Now
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544569);
						}
					}
					//	designer:str/JUKV701
					public static global::Epsitec.Common.Support.Druid Paste
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544575);
						}
					}
					//	designer:str/JUKM701
					public static global::Epsitec.Common.Support.Druid Prev
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544566);
						}
					}
					//	designer:str/JUKI701
					public static global::Epsitec.Common.Support.Druid WeeksOfYear
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544562);
						}
					}
				}
				
				public static class Timelines
				{
					//	designer:str/JUKC701
					public static global::Epsitec.Common.Support.Druid CopyEvent
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544556);
						}
					}
					//	designer:str/JUKA701
					public static global::Epsitec.Common.Support.Druid DeleteEvent
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544554);
						}
					}
					//	designer:str/JUKB701
					public static global::Epsitec.Common.Support.Druid DeselectEvent
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544555);
						}
					}
					//	designer:str/JUK5701
					public static global::Epsitec.Common.Support.Druid First
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544549);
						}
					}
					//	designer:str/JUK8701
					public static global::Epsitec.Common.Support.Druid Last
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544552);
						}
					}
					//	designer:str/JUK3701
					public static global::Epsitec.Common.Support.Druid Narrow
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544547);
						}
					}
					//	designer:str/JUK9701
					public static global::Epsitec.Common.Support.Druid NewEvent
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544553);
						}
					}
					//	designer:str/JUK7701
					public static global::Epsitec.Common.Support.Druid Next
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544551);
						}
					}
					//	designer:str/JUKD701
					public static global::Epsitec.Common.Support.Druid PasteEvent
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544557);
						}
					}
					//	designer:str/JUK6701
					public static global::Epsitec.Common.Support.Druid Prev
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544550);
						}
					}
					//	designer:str/JUK4701
					public static global::Epsitec.Common.Support.Druid Wide
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544548);
						}
					}
				}
				
				public static class TreeTable
				{
					//	designer:str/JUK7801
					public static global::Epsitec.Common.Support.Druid CompactAll
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544583);
						}
					}
					//	designer:str/JUK8801
					public static global::Epsitec.Common.Support.Druid CompactOne
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544584);
						}
					}
					//	designer:str/JUKI801
					public static global::Epsitec.Common.Support.Druid Copy
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544594);
						}
					}
					//	designer:str/JUK1801
					public static global::Epsitec.Common.Support.Druid DateRange
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544577);
						}
					}
					//	designer:str/JUKG801
					public static global::Epsitec.Common.Support.Druid Delete
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544592);
						}
					}
					//	designer:str/JUKH801
					public static global::Epsitec.Common.Support.Druid Deselect
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544593);
						}
					}
					//	designer:str/JUKA801
					public static global::Epsitec.Common.Support.Druid ExpandAll
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544586);
						}
					}
					//	designer:str/JUK9801
					public static global::Epsitec.Common.Support.Druid ExpandOne
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544585);
						}
					}
					//	designer:str/JUKK801
					public static global::Epsitec.Common.Support.Druid Export
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544596);
						}
					}
					//	designer:str/JUK0801
					public static global::Epsitec.Common.Support.Druid Filter
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544576);
						}
					}
					//	designer:str/JUK3801
					public static global::Epsitec.Common.Support.Druid First
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544579);
						}
					}
					//	designer:str/JUKM801
					public static global::Epsitec.Common.Support.Druid Goto
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544598);
						}
					}
					//	designer:str/JUK2801
					public static global::Epsitec.Common.Support.Druid Graphic
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544578);
						}
					}
					//	designer:str/JUKL801
					public static global::Epsitec.Common.Support.Druid Import
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544597);
						}
					}
					//	designer:str/JUK6801
					public static global::Epsitec.Common.Support.Druid Last
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544582);
						}
					}
					//	designer:str/JUKE801
					public static global::Epsitec.Common.Support.Druid MoveBottom
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544590);
						}
					}
					//	designer:str/JUKD801
					public static global::Epsitec.Common.Support.Druid MoveDown
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544589);
						}
					}
					//	designer:str/JUKB801
					public static global::Epsitec.Common.Support.Druid MoveTop
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544587);
						}
					}
					//	designer:str/JUKC801
					public static global::Epsitec.Common.Support.Druid MoveUp
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544588);
						}
					}
					//	designer:str/JUKF801
					public static global::Epsitec.Common.Support.Druid New
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544591);
						}
					}
					//	designer:str/JUK5801
					public static global::Epsitec.Common.Support.Druid Next
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544581);
						}
					}
					//	designer:str/JUKJ801
					public static global::Epsitec.Common.Support.Druid Paste
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544595);
						}
					}
					//	designer:str/JUK4801
					public static global::Epsitec.Common.Support.Druid Prev
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544580);
						}
					}
				}
			}
			
			public static class ToolbarController
			{
				public static class AssetsTreeTable
				{
					//	designer:str/JUKDA01
					public static global::Epsitec.Common.Support.Druid New
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544653);
						}
					}
				}
			}
			
			public static class ToolbarControllers
			{
				public static class AccountsTreeTable
				{
					//	designer:str/JUK6A01
					public static global::Epsitec.Common.Support.Druid Copy
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544646);
						}
					}
					//	designer:str/JUK2A01
					public static global::Epsitec.Common.Support.Druid DateRange
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544642);
						}
					}
					//	designer:str/JUK4A01
					public static global::Epsitec.Common.Support.Druid Delete
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544644);
						}
					}
					//	designer:str/JUK5A01
					public static global::Epsitec.Common.Support.Druid Deselect
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544645);
						}
					}
					//	designer:str/JUK8A01
					public static global::Epsitec.Common.Support.Druid Export
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544648);
						}
					}
					//	designer:str/JUK9A01
					public static global::Epsitec.Common.Support.Druid Import
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544649);
						}
					}
					//	designer:str/JUK3A01
					public static global::Epsitec.Common.Support.Druid New
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544643);
						}
					}
					//	designer:str/JUK7A01
					public static global::Epsitec.Common.Support.Druid Paste
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544647);
						}
					}
				}
				
				public static class AssetsTimeline
				{
					public static class Copy
					{
						//	designer:str/JUKAA01
						public static global::Epsitec.Common.Support.Druid EmptySelection
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544650);
							}
						}
					}
					
					public static class Paste
					{
						//	designer:str/JUKCA01
						public static global::Epsitec.Common.Support.Druid Empty
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544652);
							}
						}
						//	designer:str/JUKBA01
						public static global::Epsitec.Common.Support.Druid Wrong
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544651);
							}
						}
					}
					
					public static class Row
					{
						//	designer:str/JUKGA01
						public static global::Epsitec.Common.Support.Druid Days
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544656);
							}
						}
						//	designer:str/JUKHA01
						public static global::Epsitec.Common.Support.Druid DaysMonths
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544657);
							}
						}
						//	designer:str/JUKFA01
						public static global::Epsitec.Common.Support.Druid Events
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544655);
							}
						}
						//	designer:str/JUKJA01
						public static global::Epsitec.Common.Support.Druid Months
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544659);
							}
						}
						//	designer:str/JUKEA01
						public static global::Epsitec.Common.Support.Druid Values
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544654);
							}
						}
						//	designer:str/JUKIA01
						public static global::Epsitec.Common.Support.Druid WeekOfYear
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544658);
							}
						}
						//	designer:str/JUKKA01
						public static global::Epsitec.Common.Support.Druid Years
						{
							get
							{
								return global::Epsitec.Common.Support.Druid.FromFieldId (335544660);
							}
						}
					}
				}
			}
		}
		
		static Res()
		{
			Res._manager = new global::Epsitec.Common.Support.ResourceManager (typeof (Res));
			Res._manager.DefineDefaultModuleName ("Assets.App");
			Commands.Edit._Initialize ();
			Commands.View._Initialize ();
			Types._Initialize ();
			Strings._Initialize ();
		}
		
		public static void Initialize()
		{
			global::System.Object.Equals (Res._manager, null);
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
		private const int _moduleId = 2003;
	}
}
