using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Settings
{
	/// <summary>
	/// La classe PrintInfo comtient tous les réglages secondaires pour l'impression.
	/// </summary>
	[System.Serializable()]
	public class PrintInfo : ISerializable
	{
		public PrintInfo()
		{
			this.dpi = 300.0;
			this.zoom = 1.0;  // 100%
			this.gamma = 0.0;  // pas d'AA
			this.autoLandscape = true;
			this.autoZoom = false;
			this.forceSimply = false;
			this.forceComplex = false;
			this.perfectJoin = false;
			this.debugArea = false;
		}

		public double Zoom
		{
			get { return this.zoom; }
			set { this.zoom = value; }
		}

		public double Gamma
		{
			get { return this.gamma; }
			set { this.gamma = value; }
		}

		public double Dpi
		{
			get { return this.dpi; }
			set { this.dpi = value; }
		}

		public bool AutoZoom
		{
			get { return this.autoZoom; }
			set { this.autoZoom = value; }
		}

		public bool AutoLandscape
		{
			get { return this.autoLandscape; }
			set { this.autoLandscape = value; }
		}

		public bool ForceSimply
		{
			get { return this.forceSimply; }
			set { this.forceSimply = value; }
		}

		public bool ForceComplex
		{
			get { return this.forceComplex; }
			set { this.forceComplex = value; }
		}

		public bool PerfectJoin
		{
			get { return this.perfectJoin; }
			set { this.perfectJoin = value; }
		}

		public bool DebugArea
		{
			get { return this.debugArea; }
			set { this.debugArea = value; }
		}


		#region Serialization
		// Sérialise les réglages.
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Dpi", this.dpi);
			info.AddValue("Gamma", this.gamma);
			info.AddValue("Zoom", this.zoom);
			info.AddValue("AutoZoom", this.autoZoom);
			info.AddValue("AutoLandscape", this.autoLandscape);
			info.AddValue("ForceSimply", this.forceSimply);
			info.AddValue("ForceComplex", this.forceComplex);
		}

		// Constructeur qui désérialise les réglages.
		protected PrintInfo(SerializationInfo info, StreamingContext context) : this()
		{
			this.dpi = info.GetDouble("Dpi");
			this.gamma = info.GetDouble("Gamma");
			this.zoom = info.GetDouble("Zoom");
			this.autoZoom = info.GetBoolean("AutoZoom");
			this.autoLandscape = info.GetBoolean("AutoLandscape");
			this.forceSimply = info.GetBoolean("ForceSimply");
			this.forceComplex = info.GetBoolean("ForceComplex");
		}
		#endregion

		
		protected double			dpi;
		protected double			gamma;
		protected double			zoom;
		protected bool				autoZoom;
		protected bool				autoLandscape;
		protected bool				forceSimply;
		protected bool				forceComplex;
		protected bool				perfectJoin;
		protected bool				debugArea;
	}
}
