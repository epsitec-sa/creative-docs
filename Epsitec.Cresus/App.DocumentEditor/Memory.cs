using Epsitec.Common.Document;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.App.DocumentEditor
{
	/// <summary>
	/// La classe Memory mémorise les paramètres de l'application.
	/// </summary>
	[System.Serializable()]
	public class Memory : ISerializable
	{
		public Memory()
		{
			this.windowLocation = new Point(100, 100);
			this.windowSize = new Size(830, 580);
			this.adorner = "LookMetal";
			this.lastFilename = new System.Collections.ArrayList();
			this.lastFilenameMax = 10;
		}

		public Point WindowLocation
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

		public Size WindowSize
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
		// Sérialise la mémoire.
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("WindowLocation", this.windowLocation);
			info.AddValue("WindowSize", this.windowSize);
			info.AddValue("Adorner", this.adorner);
			info.AddValue("LastFilename", this.lastFilename);
		}

		// Constructeur qui désérialise la mémoire.
		protected Memory(SerializationInfo info, StreamingContext context) : this()
		{
			this.windowLocation = (Point) info.GetValue("WindowLocation", typeof(Point));
			this.windowSize = (Size) info.GetValue("WindowSize", typeof(Size));
			this.adorner = info.GetString("Adorner");
			this.lastFilename = (System.Collections.ArrayList) info.GetValue("LastFilename", typeof(System.Collections.ArrayList));
		}
		#endregion


		protected Point							windowLocation;
		protected Size							windowSize;
		protected string						adorner;
		protected System.Collections.ArrayList	lastFilename;
		protected int							lastFilenameMax;
	}
}
