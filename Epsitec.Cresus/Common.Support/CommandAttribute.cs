//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

using System;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// La classe CommandAttribute définit un attribut [Command] qui est
	/// utilisé par le CommandDispatcher pour identifier l'implémentation
	/// de commandes (l'attribut s'utilise comme préfixe de méthode).
	/// </summary>
	
	[AttributeUsage (AttributeTargets.Method, AllowMultiple = true)]
	
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
