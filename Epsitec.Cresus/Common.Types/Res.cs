﻿//	Automatically generated by ResGenerator, on 23.09.2006 13:50:17
//	Do not edit manually.

namespace Epsitec.Common.Types
{
	public static class Res
	{
		public static class Strings
		{
			public static string Empty { get { return Epsitec.Common.Types.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (0)); } }
			
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
			private static Epsitec.Common.Support.ResourceBundle _stringsBundle = _manager.GetBundle ("Strings");
			#endregion
		}
		
		public static void Initialize(System.Type type, string name)
		{
			_manager = new Epsitec.Common.Support.ResourceManager (type);
			_manager.DefineDefaultModuleName (name);
		}
		
		public static Epsitec.Common.Support.ResourceManager Manager
		{
			get { return _manager; }
		}
		
		public static int ModuleId
		{
			get { return _moduleId; }
		}
		
		private static Epsitec.Common.Support.ResourceManager _manager = Epsitec.Common.Support.Resources.DefaultManager;
		private static int _moduleId = 1;
	}
}
