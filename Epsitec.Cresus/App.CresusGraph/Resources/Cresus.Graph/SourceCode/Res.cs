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
			public static class ChartOptions
			{
				internal static void _Initialize()
				{
				}
				
				//	designer:cap/BVAT
				public static readonly global::Epsitec.Common.Widgets.Command ShowSeriesCaptions = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 29));
				//	designer:cap/BVAU
				public static readonly global::Epsitec.Common.Widgets.Command ShowSummaryCaptions = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 30));
			}
			
			public static class File
			{
				internal static void _Initialize()
				{
				}
				
				//	designer:cap/BVA5
				public static readonly global::Epsitec.Common.Widgets.Command ExportImage = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 5));
			}
			
			public static class General
			{
				internal static void _Initialize()
				{
				}
				
				//	designer:cap/BVAN
				public static readonly global::Epsitec.Common.Widgets.Command DownloadUpdate = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 23));
				//	designer:cap/BVA6
				public static readonly global::Epsitec.Common.Widgets.Command Kill = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 6));
			}
			
			public static class GraphType
			{
				internal static void _Initialize()
				{
				}
				
				//	designer:cap/BVA2
				public static readonly global::Epsitec.Common.Widgets.Command UseBarChartHorizontal = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 2));
				//	designer:cap/BVA1
				public static readonly global::Epsitec.Common.Widgets.Command UseBarChartVertical = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 1));
				//	designer:cap/BVAS
				public static readonly global::Epsitec.Common.Widgets.Command UseGeoChart = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 28));
				//	designer:cap/BVA
				public static readonly global::Epsitec.Common.Widgets.Command UseLineChart = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 0));
				//	designer:cap/BVAR
				public static readonly global::Epsitec.Common.Widgets.Command UsePieChart = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 27));
			}
			
			internal static void _Initialize()
			{
				ChartOptions._Initialize ();
				File._Initialize ();
				General._Initialize ();
				GraphType._Initialize ();
			}
		}
		
		public static class CommandIds
		{
			public static class ChartOptions
			{
				//	designer:cap/BVAT
				public const long ShowSeriesCaptions = 0x3EB0000A00001DL;
				//	designer:cap/BVAU
				public const long ShowSummaryCaptions = 0x3EB0000A00001EL;
			}
			
			public static class File
			{
				//	designer:cap/BVA5
				public const long ExportImage = 0x3EB0000A000005L;
			}
			
			public static class General
			{
				//	designer:cap/BVAN
				public const long DownloadUpdate = 0x3EB0000A000017L;
				//	designer:cap/BVA6
				public const long Kill = 0x3EB0000A000006L;
			}
			
			public static class GraphType
			{
				//	designer:cap/BVA2
				public const long UseBarChartHorizontal = 0x3EB0000A000002L;
				//	designer:cap/BVA1
				public const long UseBarChartVertical = 0x3EB0000A000001L;
				//	designer:cap/BVAS
				public const long UseGeoChart = 0x3EB0000A00001CL;
				//	designer:cap/BVA
				public const long UseLineChart = 0x3EB0000A000000L;
				//	designer:cap/BVAR
				public const long UsePieChart = 0x3EB0000A00001BL;
			}
			
		}
		
		public static class Captions
		{
			public static class DocumentView
			{
				public static class Options
				{
					//	designer:cap/BVA3
					public static global::Epsitec.Common.Types.Caption AccumulateValues
					{
						get
						{
							return global::Epsitec.Cresus.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 3));
						}
					}
					//	designer:cap/BVA4
					public static global::Epsitec.Common.Types.Caption StackValues
					{
						get
						{
							return global::Epsitec.Cresus.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 4));
						}
					}
				}
			}
			
			public static class Message
			{
				public static class DataImport
				{
					//	designer:cap/BVA9
					public static global::Epsitec.Common.Types.Caption WhatToDo
					{
						get
						{
							return global::Epsitec.Cresus.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 9));
						}
					}
					//	designer:cap/BVAB
					public static global::Epsitec.Common.Types.Caption WhatToDoAdd
					{
						get
						{
							return global::Epsitec.Cresus.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 11));
						}
					}
					//	designer:cap/BVAC
					public static global::Epsitec.Common.Types.Caption WhatToDoCancel
					{
						get
						{
							return global::Epsitec.Cresus.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 12));
						}
					}
					//	designer:cap/BVAA
					public static global::Epsitec.Common.Types.Caption WhatToDoMerge
					{
						get
						{
							return global::Epsitec.Cresus.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 10));
						}
					}
					public static class Failure
					{
						//	designer:cap/BVA8
						public static global::Epsitec.Common.Types.Caption MultipleSources
						{
							get
							{
								return global::Epsitec.Cresus.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 8));
							}
						}
						//	designer:cap/BVA7
						public static global::Epsitec.Common.Types.Caption NoSource
						{
							get
							{
								return global::Epsitec.Cresus.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 7));
							}
						}
					}
				}
				
				public static class LicenseInvalid
				{
					//	designer:cap/BVAE
					public static global::Epsitec.Common.Types.Caption Option1BuyGraph
					{
						get
						{
							return global::Epsitec.Cresus.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 14));
						}
					}
					//	designer:cap/BVAI
					public static global::Epsitec.Common.Types.Caption Option1UpdateGraph
					{
						get
						{
							return global::Epsitec.Cresus.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 18));
						}
					}
					//	designer:cap/BVAG
					public static global::Epsitec.Common.Types.Caption Option2UpdateCompta
					{
						get
						{
							return global::Epsitec.Cresus.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 16));
						}
					}
					//	designer:cap/BVAF
					public static global::Epsitec.Common.Types.Caption Option3Quit
					{
						get
						{
							return global::Epsitec.Cresus.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 15));
						}
					}
					//	designer:cap/BVAD
					public static global::Epsitec.Common.Types.Caption Question
					{
						get
						{
							return global::Epsitec.Cresus.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 13));
						}
					}
					//	designer:cap/BVAH
					public static global::Epsitec.Common.Types.Caption QuestionStandalone
					{
						get
						{
							return global::Epsitec.Cresus.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 17));
						}
					}
				}
				
				public static class Quit
				{
					//	designer:cap/BVAK
					public static global::Epsitec.Common.Types.Caption Option1Save
					{
						get
						{
							return global::Epsitec.Cresus.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 20));
						}
					}
					//	designer:cap/BVAL
					public static global::Epsitec.Common.Types.Caption Option2DoNotSave
					{
						get
						{
							return global::Epsitec.Cresus.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 21));
						}
					}
					//	designer:cap/BVAM
					public static global::Epsitec.Common.Types.Caption Option3Cancel
					{
						get
						{
							return global::Epsitec.Cresus.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 22));
						}
					}
					//	designer:cap/BVAJ
					public static global::Epsitec.Common.Types.Caption Question
					{
						get
						{
							return global::Epsitec.Cresus.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 19));
						}
					}
				}
				
				public static class Update
				{
					//	designer:cap/BVAP
					public static global::Epsitec.Common.Types.Caption Option1DownloadAndInstall
					{
						get
						{
							return global::Epsitec.Cresus.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 25));
						}
					}
					//	designer:cap/BVAQ
					public static global::Epsitec.Common.Types.Caption Option2Cancel
					{
						get
						{
							return global::Epsitec.Cresus.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 26));
						}
					}
					//	designer:cap/BVAO
					public static global::Epsitec.Common.Types.Caption Question
					{
						get
						{
							return global::Epsitec.Cresus.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 24));
						}
					}
				}
			}
			
		}
		
		public static class CaptionIds
		{
			public static class DocumentView
			{
				public static class Options
				{
					//	designer:cap/BVA3
					public static readonly global::Epsitec.Common.Support.Druid AccumulateValues = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 3);
					//	designer:cap/BVA4
					public static readonly global::Epsitec.Common.Support.Druid StackValues = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 4);
				}
			}
			
			public static class Message
			{
				public static class DataImport
				{
					//	designer:cap/BVA9
					public static readonly global::Epsitec.Common.Support.Druid WhatToDo = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 9);
					//	designer:cap/BVAB
					public static readonly global::Epsitec.Common.Support.Druid WhatToDoAdd = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 11);
					//	designer:cap/BVAC
					public static readonly global::Epsitec.Common.Support.Druid WhatToDoCancel = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 12);
					//	designer:cap/BVAA
					public static readonly global::Epsitec.Common.Support.Druid WhatToDoMerge = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 10);
					public static class Failure
					{
						//	designer:cap/BVA8
						public static readonly global::Epsitec.Common.Support.Druid MultipleSources = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 8);
						//	designer:cap/BVA7
						public static readonly global::Epsitec.Common.Support.Druid NoSource = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 7);
					}
				}
				
				public static class LicenseInvalid
				{
					//	designer:cap/BVAE
					public static readonly global::Epsitec.Common.Support.Druid Option1BuyGraph = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 14);
					//	designer:cap/BVAI
					public static readonly global::Epsitec.Common.Support.Druid Option1UpdateGraph = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 18);
					//	designer:cap/BVAG
					public static readonly global::Epsitec.Common.Support.Druid Option2UpdateCompta = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 16);
					//	designer:cap/BVAF
					public static readonly global::Epsitec.Common.Support.Druid Option3Quit = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 15);
					//	designer:cap/BVAD
					public static readonly global::Epsitec.Common.Support.Druid Question = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 13);
					//	designer:cap/BVAH
					public static readonly global::Epsitec.Common.Support.Druid QuestionStandalone = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 17);
				}
				
				public static class Quit
				{
					//	designer:cap/BVAK
					public static readonly global::Epsitec.Common.Support.Druid Option1Save = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 20);
					//	designer:cap/BVAL
					public static readonly global::Epsitec.Common.Support.Druid Option2DoNotSave = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 21);
					//	designer:cap/BVAM
					public static readonly global::Epsitec.Common.Support.Druid Option3Cancel = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 22);
					//	designer:cap/BVAJ
					public static readonly global::Epsitec.Common.Support.Druid Question = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 19);
				}
				
				public static class Update
				{
					//	designer:cap/BVAP
					public static readonly global::Epsitec.Common.Support.Druid Option1DownloadAndInstall = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 25);
					//	designer:cap/BVAQ
					public static readonly global::Epsitec.Common.Support.Druid Option2Cancel = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 26);
					//	designer:cap/BVAO
					public static readonly global::Epsitec.Common.Support.Druid Question = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 24);
				}
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
			
			public static class Message
			{
				//	designer:str/BVA2
				public static global::Epsitec.Common.Types.FormattedText FreePiccoloBecauseOfCompta
				{
					get
					{
						return global::Epsitec.Cresus.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (167772162));
					}
				}
				//	designer:str/BVA3
				public static global::Epsitec.Common.Types.FormattedText MoreThanPiccolo
				{
					get
					{
						return global::Epsitec.Cresus.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (167772163));
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
