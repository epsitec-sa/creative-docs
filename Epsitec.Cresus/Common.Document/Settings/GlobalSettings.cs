using Epsitec.Common.Document;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Settings
{
	public enum FirstAction
	{
		Nothing         = 0,
		OpenNewDocument = 1,
		OpenLastFile    = 2,
		OpenLastFiles   = 3,
	}

	/// <summary>
	/// La classe GlobalSettings mémorise les paramètres de l'application.
	/// </summary>
	[System.Serializable()]
	public class GlobalSettings : ISerializable
	{
		public GlobalSettings()
		{
			this.windowLocation = new Drawing.Point(100, 100);
			this.windowSize = new Drawing.Size(830, 580);
			this.isFullScreen = false;
			this.screenDpi = 96.0;
			this.adorner = "LookMetal";
			this.splashScreen = true;
			this.firstAction = FirstAction.OpenNewDocument;
			this.lastFilename = new System.Collections.ArrayList();
			this.lastFilenameMax = 10;
		}

		public Drawing.Point WindowLocation
		{
			get
			{
				return this.windowLocation;
			}

			set
			{
				this.windowLocation = value;
			}
		}

		public Drawing.Size WindowSize
		{
			get
			{
				return this.windowSize;
			}

			set
			{
				this.windowSize = value;
			}
		}


		public bool IsFullScreen
		{
			get
			{
				return this.isFullScreen;
			}

			set
			{
				this.isFullScreen = value;
			}
		}

		public double ScreenDpi
		{
			get
			{
				return this.screenDpi;
			}

			set
			{
				this.screenDpi = value;
			}
		}


		public string Adorner
		{
			get
			{
				return this.adorner;
			}

			set
			{
				this.adorner = value;
			}
		}

		public bool SplashScreen
		{
			get
			{
				return this.splashScreen;
			}

			set
			{
				this.splashScreen = value;
			}
		}

		public FirstAction FirstAction
		{
			get
			{
				return this.firstAction;
			}

			set
			{
				this.firstAction = value;
			}
		}


		public int LastFilenameCount
		{
			get
			{
				return this.lastFilename.Count;
			}
		}

		public string LastFilenameGet(int index)
		{
			return this.lastFilename[index] as string;
		}

		public string LastFilenameGetShort(int index)
		{
			return Misc.ExtractName(this.LastFilenameGet(index));
		}

		public void LastFilenameAdd(string filename)
		{
			int index = this.LastFilenameSearch(filename);
			if ( index < 0 )
			{
				this.LastFilenameTrunc();
			}
			else
			{
				this.lastFilename.RemoveAt(index);
			}
			this.lastFilename.Insert(0, filename);
		}

		protected int LastFilenameSearch(string filename)
		{
			for ( int i=0 ; i<this.lastFilename.Count ; i++ )
			{
				string s = this.lastFilename[i] as string;
				if ( s == filename )  return i;
			}
			return -1;
		}

		protected void LastFilenameTrunc()
		{
			if ( this.lastFilename.Count < this.lastFilenameMax )  return;
			this.lastFilename.RemoveAt(this.lastFilename.Count-1);
		}


		#region Serialization
		// Sérialise les réglages.
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("WindowLocation", this.windowLocation);
			info.AddValue("WindowSize", this.windowSize);
			info.AddValue("IsFullScreen", this.isFullScreen);
			info.AddValue("ScreenDpi", this.screenDpi);
			info.AddValue("Adorner", this.adorner);
			info.AddValue("SplashScreen", this.splashScreen);
			info.AddValue("FirstAction", this.firstAction);
			info.AddValue("LastFilename", this.lastFilename);
		}

		// Constructeur qui désérialise les réglages.
		protected GlobalSettings(SerializationInfo info, StreamingContext context) : this()
		{
			this.windowLocation = (Drawing.Point) info.GetValue("WindowLocation", typeof(Drawing.Point));
			this.windowSize = (Drawing.Size) info.GetValue("WindowSize", typeof(Drawing.Size));
			this.isFullScreen = info.GetBoolean("IsFullScreen");
			this.screenDpi = info.GetDouble("ScreenDpi");
			this.adorner = info.GetString("Adorner");
			this.splashScreen = info.GetBoolean("SplashScreen");
			this.firstAction = (FirstAction) info.GetValue("FirstAction", typeof(FirstAction));
			this.lastFilename = (System.Collections.ArrayList) info.GetValue("LastFilename", typeof(System.Collections.ArrayList));
		}
		#endregion


		protected Drawing.Point					windowLocation;
		protected Drawing.Size					windowSize;
		protected bool							isFullScreen;
		protected double						screenDpi;
		protected string						adorner;
		protected bool							splashScreen;
		protected FirstAction					firstAction;
		protected System.Collections.ArrayList	lastFilename;
		protected int							lastFilenameMax;
	}
}
