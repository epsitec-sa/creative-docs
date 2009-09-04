//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
				return this.textVersion != this.story.Version;
			}
		}
		
		public bool								HasStyleChanged
		{
			get
			{
				return this.styleVersion != this.story.TextContext.StyleList.Version;
			}
		}
		
		public bool								HasFitterChanged
		{
			get
			{
				long version = this.fitter == null ? 0 : this.fitter.Version;
				
				return this.fitterVersion != version;
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
			long textVersion   = this.story.Version;
			long styleVersion  = this.story.TextContext.StyleList.Version;
			long fitterVersion = this.fitter == null ? 0 : this.fitter.Version;
			
			if (this.textVersion != textVersion)
			{
				this.OnTextChanged ();
			}
			if (this.styleVersion != styleVersion)
			{
				this.OnStyleChanged ();
			}
			if (this.fitterVersion != fitterVersion)
			{
				this.OnFitterChanged ();
			}
			
			this.textVersion   = textVersion;
			this.styleVersion  = styleVersion;
			this.fitterVersion = fitterVersion;
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
		
		private long							textVersion;
		private long							styleVersion;
		private long							fitterVersion;
	}
}
