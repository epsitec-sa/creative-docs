//	Copyright © 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// La classe CommandAttribute définit un attribut [Command] qui est
	/// utilisé par le CommandDispatcher pour identifier l'implémentation
	/// de commandes (l'attribut s'utilise comme préfixe de méthode).
	/// </summary>
	
	[System.Serializable]
	[System.AttributeUsage (System.AttributeTargets.Method, AllowMultiple = true)]
	
	public class CommandAttribute : System.Attribute
	{
		public CommandAttribute()
		{
		}
		
		public CommandAttribute(string command_name)
		{
			this.command_name = command_name;
		}
		
		
		public string					CommandName
		{
			get { return this.command_name; }
			set { this.command_name = value; }
		}
		
		
		protected string				command_name;
	}
}
