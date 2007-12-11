//	Copyright © 2004-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// La classe Dialog permet d'ouvrir et de gérer un dialogue à partir d'une
	/// ressource et d'une source de données.
	/// </summary>
	public class Dialog : AbstractDialog
	{
		public Dialog(ResourceManager resourceManager)
			: this (resourceManager, "AnonymousDialog")
		{
		}
		
		public Dialog(ResourceManager resourceManager, string name)
		{
			this.name            = name;
			this.dispatcher      = new CommandDispatcher (this.name, CommandDispatcherLevel.Secondary);
			this.resourceManager = resourceManager;
		}
		
		
		public CommandDispatcher				CommandDispatcher
		{
			get
			{
				return this.dispatcher;
			}
		}
		
		
		public static Dialog Load(ResourceManager resourceManager, Druid resourceId)
		{
			//	TODO: ...
			
			Dialog dialog = new Dialog (resourceManager);

			return dialog;
		}
		
		public void AddController(object controller)
		{
			this.CommandDispatcher.RegisterController (controller);
		}
		
		
		private readonly ResourceManager		resourceManager;
		private readonly CommandDispatcher		dispatcher;
		private readonly string					name;
	}
}
