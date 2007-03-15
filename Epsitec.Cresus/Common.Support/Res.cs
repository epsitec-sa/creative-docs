﻿//	Automatically generated by ResGenerator, on 15.03.2007
//	Do not edit manually.

namespace Epsitec.Common.Support
{
	public static class Res
	{
		//	Code mapping for 'Caption' resources
		
		//	Code mapping for 'String' resources
		
		public static class Strings
		{
			public static class Image
			{
				public static string Description { get { return Epsitec.Common.Support.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (0)); } }
			}
			
			public static string GetString(params string[] path)
			{
				string field = string.Join (".", path);
				return _stringsBundle[field].AsString;
			}
			
			#region Internal Support Code
			private static string GetText(string bundle, params string[] path)
			{
				string field = string.Join (".", path);
				return _stringsBundle[field].AsString;
			}
			private static string GetText(Epsitec.Common.Support.Druid druid)
			{
				return _stringsBundle[druid].AsString;
			}
			private static Epsitec.Common.Support.ResourceBundle _stringsBundle = Res._manager.GetBundle ("Strings");
			#endregion
		}
		
		//	Code mapping for 'Panel' resources
		
		static Res()
		{
			Res.Initialize (typeof (Res), "Common.Support");
		}

		public static void Initialize()
		{
		}

		private static void Initialize(System.Type type, string name)
		{
			Res._manager = new Epsitec.Common.Support.ResourceManager (type);
			Res._manager.DefineDefaultModuleName (name);
		}
		
		public static Epsitec.Common.Support.ResourceManager Manager
		{
			get { return Res._manager; }
		}
		
		public static int ModuleId
		{
			get { return _moduleId; }
		}
		
		private static Epsitec.Common.Support.ResourceManager _manager = Epsitec.Common.Support.Resources.DefaultManager;
		private const int _moduleId = 7;
	}
}
