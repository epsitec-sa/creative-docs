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
		public TextVersion(TextStory story) : this (story, null, null)
		{
		}
		
		public TextVersion(TextFitter fitter) : this (fitter.TextStory, fitter, null)
		{
		}
		
		public TextVersion(TextStory story, TextFitter fitter, object argument)
		{
			this.story    = story;
			this.fitter   = fitter;
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
		
		public bool								HasFitterChanged
		{
			get
			{
				long version = this.fitter == null ? 0 : this.fitter.Version;
				
				return this.fitter_version != version;
			}
		}
		
		public bool								HasAnythingChanged
		{
			get
			{
				return this.HasTextChanged || this.HasStyleChanged || this.HasFitterChanged;
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
			long text_version   = this.story.Version;
			long style_version  = this.story.TextContext.StyleList.Version;
			long fitter_version = this.fitter == null ? 0 : this.fitter.Version;
			
			if (this.text_version != text_version)
			{
				this.OnTextChanged ();
			}
			if (this.style_version != style_version)
			{
				this.OnStyleChanged ();
			}
			if (this.fitter_version != fitter_version)
			{
				this.OnFitterChanged ();
			}
			
			this.text_version   = text_version;
			this.style_version  = style_version;
			this.fitter_version = fitter_version;
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
		
		private void OnFitterChanged()
		{
			if (this.FitterChanged != null)
			{
				this.FitterChanged (this);
			}
		}
		
		
		public event VersionChangedEventHandler	TextChanged;
		public event VersionChangedEventHandler	StyleChanged;
		public event VersionChangedEventHandler	FitterChanged;
		
		private TextStory						story;
		private TextFitter						fitter;
		private object							argument;
		
		private long							text_version;
		private long							style_version;
		private long							fitter_version;
	}
}
