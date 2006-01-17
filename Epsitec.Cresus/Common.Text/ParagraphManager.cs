//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe ParagraphManager implémente l'ossature de base de toute les
	/// classes d'implémentation de IParagraphManager.
	/// </summary>
	public abstract class ParagraphManager : IParagraphManager
	{
		protected ParagraphManager()
		{
		}
		
		
		public virtual string					Name
		{
			get
			{
				string name    = this.GetType ().Name;
				string manager = "Manager";
				
				if (name.EndsWith (manager))
				{
					return name.Substring (0, name.Length - manager.Length);
				}
				
				return name;
			}
		}
		
		
		public abstract void AttachToParagraph(TextStory story, ICursor cursor, Properties.ManagedParagraphProperty property);
		public abstract void DetachFromParagraph(TextStory story, ICursor cursor, Properties.ManagedParagraphProperty property);
	}
}
