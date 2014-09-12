//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.App.Views.CommandToolbars;
using Epsitec.Cresus.Assets.App.Views;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Ce widget doit forcément être le premier créé par la UI dans la fenêtre de l'application.
	/// Il occupe toute la surface de la fenêtre et s'occupe d'attraper les raccourcis clavier.
	/// </summary>
	public class ShortcutCatcher : FrameBox
	{
		public ShortcutCatcher()
		{
			this.attachedCommands = new List<AttachedCommand> ();
		}


		public int								Level
		{
			//	Le niveau correspond au degré d'empilement des fenêtres modales.
			//	Au départ, on est au niveau zéro. Les raccourcis de la toolbar principale
			//	sont donc enregistrés au niveau zéro. Lorsqu'on ouvre un Popup (forcément
			//	modal), on passe au niveau 1. Les raccourcis du niveau zéro sont toujours
			//	présents, mais inactifs. On peut crééer des raccourcis au niveau 1. A la
			//	fermeture du Popup, ils seront purgés.
			get
			{
				return this.level;
			}
			set
			{
				this.level = value;
				this.PurgeHigherLevelCommands ();
			}
		}


		public void Attach(AbstractCommandToolbar toolbar, ToolbarCommand command, Shortcut shortcut)
		{
			//	Attache un raccourci au niveau courant. Pour un niveau donné, il ne peut y
			//	avoir qu'un seul raccourci par combinaison de touche clavier.
			if (shortcut != null)
			{
				if (this.attachedCommands.Where (x => x.Shortcut == shortcut && x.Level == this.level).Any ())
				{
					//?throw new System.InvalidOperationException (string.Format ("Shortcut is already defined {0} at level {1}", shortcut, this.level));
				}

				var a = new AttachedCommand (toolbar, command, shortcut, this.level);
				this.attachedCommands.Add (a);
			}
		}

		public void Detach(AbstractCommandToolbar toolbar)
		{
			//	Détache toutes les commandes d'une toolbar donnée.
			var toRemove = this.attachedCommands.Where (x => x.Toolbar == toolbar).ToArray ();

			foreach (var x in toRemove)
			{
				this.attachedCommands.Remove (x);
			}
		}


		protected override void ProcessMessage(Message message, Point pos)
		{
			if (message.IsKeyType)
			{
				if (message.MessageType == MessageType.KeyDown)
				{
					if (this.Process (message, isDown: true))
					{
						message.Swallowed = true;
						message.Consumer  = this;
					}
				}
				else if (message.MessageType == MessageType.KeyUp)
				{
					if (this.Process (message, isDown: false))
					{
						message.Swallowed = true;
						message.Consumer  = this;
					}
				}
			}

			base.ProcessMessage (message, pos);
		}


		private bool Process(Message message, bool isDown)
		{
			//	Active le raccourci correspondant au message, s'il existe. Dans ce cas,
			//	on retourne true, et le message sera "avalé". Il faut avaler les messages
			//	up et down, bien que la commande ne soit activée que sur le down.

#if false
			var a = this.attachedCommands
				.Where (x => x.Level == this.level && x.ToolbarEnable && x.Match (message))
				.ToArray ();

			if (a.Length == 0)  // aucun raccourci attaché ?
			{
				return false;
			}
			else if (a.Length == 1)  // existe-t-il un raccourci attaché ?
			{
				if (isDown)
				{
					var c = a.First ();
					c.Toolbar.ActivateCommand (c.Command);  // comme si l'utilisateur avait cliqué le bouton
				}

				return true;
			}
			else
			{
				throw new System.InvalidOperationException (string.Format ("There are {0} shortcuts defined !", a.Length));
			}
#else
			var a = this.attachedCommands
				.Where (x => x.Level == this.level && x.ToolbarEnable && x.Match (message))
				.LastOrDefault ();  // cherche le dernier raccourci défini

			if (a == null)  // aucun raccourci attaché ?
			{
				return false;
			}
			else  // existe-t-il un raccourci attaché ?
			{
				if (isDown)
				{
					a.Toolbar.ActivateCommand (a.Command);  // comme si l'utilisateur avait cliqué le bouton
				}

				return true;
			}
#endif
		}

		private void PurgeHigherLevelCommands()
		{
			//	Purge toutes les commandes d'un niveau supérieur au niveau actuel.
			var toRemove = this.attachedCommands.Where (x => x.Level > this.level).ToArray ();

			foreach (var x in toRemove)
			{
				this.attachedCommands.Remove (x);
			}
		}


		private class AttachedCommand
		{
			public AttachedCommand(AbstractCommandToolbar toolbar, ToolbarCommand command, Shortcut shortcut, int level)
			{
				this.Toolbar  = toolbar;
				this.Command  = command;
				this.Shortcut = shortcut;
				this.Level    = level;
			}

			public bool							ToolbarEnable
			{
				get
				{
					return this.Toolbar.IsParentVisible;
				}
			}

			public bool Match(Message message)
			{
				return message.KeyCode      == this.ShortcutKeyCode
					&& message.ModifierKeys == this.ShortcutModifierKeys;
			}

			private KeyCode						ShortcutKeyCode
			{
				get
				{
					return this.Shortcut.KeyCode & (KeyCode) 0xffff;
				}
			}

			private ModifierKeys				ShortcutModifierKeys
			{
				get
				{
					return (ModifierKeys) this.Shortcut.KeyCode & (ModifierKeys) 0xff0000;
				}
			}

			public override string ToString()
			{
				//	Pour le debug.
				var t = this.Toolbar.GetType ().ToString ().Split ('.').LastOrDefault ();
				return string.Format ("Toolbar={0}/{1} Command={2} Shortcut={3} Level={4}", t, this.ToolbarEnable, this.Command, this.Shortcut, this.Level);
			}

			public readonly AbstractCommandToolbar	Toolbar;
			public readonly ToolbarCommand			Command;
			public readonly Shortcut				Shortcut;
			public readonly int						Level;
		}



		private readonly List<AttachedCommand>	attachedCommands;
		private int								level;
	}
}
