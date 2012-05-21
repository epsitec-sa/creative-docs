using System.Runtime.Serialization;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Settings
{
    public enum ICOFormat
    {
        XP,
        Vista,
    }

	/// <summary>
	/// La classe ExportICOInfo contient tous les réglages pour l'exportation d'une icône.
	/// </summary>
	[System.Serializable()]
	public class ExportICOInfo : ISerializable
	{
		public ExportICOInfo(Document document)
		{
			this.document = document;
			this.Initialize();
		}

		protected void Initialize()
		{
			this.format = ICOFormat.Vista;
		}

        public ICOFormat Format
		{
			get { return this.format; }
			set { this.format = value; }
		}


		#region Serialization
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise les réglages.
			info.AddValue("Rev", 0);
            info.AddValue("ICOFormat", this.format);
		}

		protected ExportICOInfo(SerializationInfo info, StreamingContext context)
		{
			//	Constructeur qui désérialise les réglages.
			this.document = Document.ReadDocument;
			this.Initialize();

			int rev = info.GetInt32("Rev");
            this.format = (ICOFormat)info.GetValue("ICOFormat", typeof(ICOFormat));
		}
		#endregion

		
		protected Document					document;
		protected ICOFormat 				format;
	}
}
