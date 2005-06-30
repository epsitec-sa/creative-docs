//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	public delegate void VersionChangedEventHandler(TextVersion sender);
	
	/// <summary>
	/// La classe TextVersion permet de déterminer simplement s'il y a eu des
	/// modifications depuis la dernière consultation, mais aussi de générer
	/// des événements spécifiques, par un appel explicite à Update.
	/// </summary>
	public sealed class TextVersion
	{
		public TextVersion(TextStory story) : this (story, null)
		{
		}
		
		public TextVersion(TextStory story, object argument)
		{
			this.story    = story;
			this.argument = argument;
		}
		
		
		public bool								HasTextChanged
		{
			get
			{
				return this.text_version != this.story.Version;
			}
		}
		
		public bool								HasStyleChanged
		{
			get
			{
				return this.style_version != this.story.TextContext.StyleList.Version;
			}
		}
		
		public bool								HasAnythingChanged
		{
			get
			{
				return this.HasTextChanged || this.HasStyleChanged;
			}
		}
		
		
		public TextStory						TextStory
		{
			get
			{
				return this.story;
			}
		}
		
		public object							Argument
		{
			get
			{
				return this.argument;
			}
		}
		
		
		public void Update()
		{
			long text_version  = this.story.Version;
			long style_version = this.story.TextContext.StyleList.Version;
			
			if (this.HasTextChanged)
			{
				this.OnTextChanged ();
			}
			
			if (this.HasStyleChanged)
			{
				this.OnStyleChanged ();
			}
			
			this.text_version  = text_version;
			this.style_version = style_version;
		}
		
		
		private void OnTextChanged()
		{
			if (this.TextChanged != null)
			{
				this.TextChanged (this);
			}
		}
		
		private void OnStyleChanged()
		{
			if (this.StyleChanged != null)
			{
				this.StyleChanged (this);
			}
		}
		
		
		public event VersionChangedEventHandler	TextChanged;
		public event VersionChangedEventHandler	StyleChanged;
		
		private TextStory						story;
		private object							argument;
		
		private long							text_version;
		private long							style_version;
	}
}
