//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe ParagraphManager ...
	/// </summary>
	public abstract class ParagraphManager
	{
		public ParagraphManager(string name)
		{
			this.name = name;
		}
		
		
		public string							Name
		{
			get
			{
				return this.name;
			}
		}
		
		
		public abstract void GenerateText(TextStory story, ICursor cursor);
		
		
		private string							name;
	}
}
