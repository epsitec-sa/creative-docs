//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe ParagraphManager ...
	/// </summary>
	public abstract class ParagraphManager : IParagraphManager
	{
		public ParagraphManager(string name)
		{
		}
		
		
		public virtual string					Name
		{
			get
			{
				return this.GetType ().Name;
			}
		}
		
		
		public abstract void AttachToParagraph(TextStory story, ICursor cursor, string[] parameters);
		public abstract void DetachFromParagraph(TextStory story, ICursor cursor, string[] parameters);
	}
}
