using System.Runtime.Serialization;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Settings
{
    public enum ICOFormat
    {
        XP,
        Vista,
		Paginated,
    }

	/// <summary>
	/// La classe ExportICOInfo contient tous les r�glages pour l'exportation d'une ic�ne.
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
			this.format = ICOFormat.Paginated;
		}

        public ICOFormat Format
		{
			get { return this.format; }
			set { this.format = value; }
		}


		#region Serialization
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	S�rialise les r�glages.
			info.AddValue("Rev", 0);
            info.AddValue("ICOFormat", this.format);
		}

		protected ExportICOInfo(SerializationInfo info, StreamingContext context)
		{
			//	Constructeur qui d�s�rialise les r�glages.
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
