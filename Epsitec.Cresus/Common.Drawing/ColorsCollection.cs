using System.IO;
using System.Runtime.Serialization;

namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// La classe ColorsCollection contient une collection de couleurs.
	/// </summary>

	public enum ColorsCollectionType
	{
		Default,	// palette standard
		Rainbow,	// palette arc-en-ciel
		Light,		// palette pastel
		Dark,		// palette foncée
		Gray,		// palette monochrome
	}

	[System.Serializable()]
	public class ColorsCollection : ISerializable
	{
		public ColorsCollection()
		{
			this.list = new System.Collections.ArrayList();

			for ( int i=0 ; i<32 ; i++ )
			{
				this.list.Add(Color.Empty);
			}
			this.Initialise(ColorsCollectionType.Default);
		}


		public void Initialise(ColorsCollectionType type)
		{
			if ( type == ColorsCollectionType.Gray )
			{
				this.list[0+8*0] = Color.FromARGB(0.0, 1.0, 1.0, 1.0);
				this.list[0+8*1] = Color.FromARGB(1.0, 1.0, 0.0, 0.0);
				this.list[0+8*2] = Color.FromARGB(1.0, 0.0, 1.0, 0.0);
				this.list[0+8*3] = Color.FromARGB(1.0, 0.0, 0.0, 1.0);

				for ( int i=0 ; i<28 ; i++ )
				{
					int j = 1 + (i/4) + (i%4)*8;
					this.list[j] = Color.FromBrightness(1.0-(i/27.0));
				}
			}
			else if ( type == ColorsCollectionType.Rainbow )
			{
				this.list[0+8*0] = Color.FromARGB(0.0, 1.00, 1.00, 1.00);
				this.list[0+8*1] = Color.FromARGB(1.0, 1.00, 1.00, 1.00);
				this.list[0+8*2] = Color.FromARGB(1.0, 0.83, 0.83, 0.83);
				this.list[0+8*3] = Color.FromARGB(1.0, 0.67, 0.67, 0.67);
				this.list[1+8*0] = Color.FromARGB(1.0, 0.50, 0.50, 0.50);
				this.list[1+8*1] = Color.FromARGB(1.0, 0.33, 0.33, 0.33);
				this.list[1+8*2] = Color.FromARGB(1.0, 0.17, 0.17, 0.17);
				this.list[1+8*3] = Color.FromARGB(1.0, 0.00, 0.00, 0.00);

				for ( int i=0 ; i<24 ; i++ )
				{
					int j = 2 + (i/4) + (i%4)*8;
					double h = 1.0-(i/24.0);
					this.list[j] = Color.FromHSV(h*360, 1.0, 1.0);
				}
			}
			else if ( type == ColorsCollectionType.Light )
			{
				this.list[0+8*0] = Color.FromARGB(0.0, 1.00, 1.00, 1.00);
				this.list[0+8*1] = Color.FromARGB(1.0, 1.00, 1.00, 1.00);
				this.list[0+8*2] = Color.FromARGB(1.0, 0.83, 0.83, 0.83);
				this.list[0+8*3] = Color.FromARGB(1.0, 0.67, 0.67, 0.67);
				this.list[1+8*0] = Color.FromARGB(1.0, 0.50, 0.50, 0.50);
				this.list[1+8*1] = Color.FromARGB(1.0, 0.33, 0.33, 0.33);
				this.list[1+8*2] = Color.FromARGB(1.0, 0.17, 0.17, 0.17);
				this.list[1+8*3] = Color.FromARGB(1.0, 0.00, 0.00, 0.00);

				for ( int i=0 ; i<24 ; i++ )
				{
					int j = 2 + (i/4) + (i%4)*8;
					double h = 1.0-(i/24.0);
					this.list[j] = Color.FromHSV(h*360, 0.3, 1.0);
				}
			}
			else if ( type == ColorsCollectionType.Dark )
			{
				this.list[0+8*0] = Color.FromARGB(0.0, 1.00, 1.00, 1.00);
				this.list[0+8*1] = Color.FromARGB(1.0, 1.00, 1.00, 1.00);
				this.list[0+8*2] = Color.FromARGB(1.0, 0.83, 0.83, 0.83);
				this.list[0+8*3] = Color.FromARGB(1.0, 0.67, 0.67, 0.67);
				this.list[1+8*0] = Color.FromARGB(1.0, 0.50, 0.50, 0.50);
				this.list[1+8*1] = Color.FromARGB(1.0, 0.33, 0.33, 0.33);
				this.list[1+8*2] = Color.FromARGB(1.0, 0.17, 0.17, 0.17);
				this.list[1+8*3] = Color.FromARGB(1.0, 0.00, 0.00, 0.00);

				for ( int i=0 ; i<24 ; i++ )
				{
					int j = 2 + (i/4) + (i%4)*8;
					double h = 1.0-(i/24.0);
					this.list[j] = Color.FromHSV(h*360, 1.0, 0.7);
				}
			}
			else
			{
				int i = 0;

				this.list[i++] = Color.FromARGB(0.0, 1.0, 1.0, 1.0);
				this.list[i++] = Color.FromARGB(1.0, 1.0, 0.7, 0.7);
				this.list[i++] = Color.FromARGB(1.0, 1.0, 1.0, 0.7);
				this.list[i++] = Color.FromARGB(1.0, 0.7, 1.0, 0.7);
				this.list[i++] = Color.FromARGB(1.0, 0.7, 1.0, 1.0);
				this.list[i++] = Color.FromARGB(1.0, 0.7, 0.7, 1.0);
				this.list[i++] = Color.FromARGB(1.0, 1.0, 0.7, 1.0);
				this.list[i++] = Color.FromARGB(0.2, 0.5, 0.5, 0.5);

				this.list[i++] = Color.FromARGB(0.3, 1.0, 1.0, 1.0);
				this.list[i++] = Color.FromARGB(1.0, 1.0, 0.0, 0.0);
				this.list[i++] = Color.FromARGB(1.0, 1.0, 1.0, 0.0);
				this.list[i++] = Color.FromARGB(1.0, 0.0, 1.0, 0.0);
				this.list[i++] = Color.FromARGB(1.0, 0.0, 1.0, 1.0);
				this.list[i++] = Color.FromARGB(1.0, 0.0, 0.0, 1.0);
				this.list[i++] = Color.FromARGB(1.0, 1.0, 0.0, 1.0);
				this.list[i++] = Color.FromARGB(0.5, 0.5, 0.5, 0.5);

				this.list[i++] = Color.FromARGB(0.6, 1.0, 1.0, 1.0);
				this.list[i++] = Color.FromARGB(1.0, 0.7, 0.0, 0.0);
				this.list[i++] = Color.FromARGB(1.0, 0.7, 0.7, 0.0);
				this.list[i++] = Color.FromARGB(1.0, 0.0, 0.7, 0.0);
				this.list[i++] = Color.FromARGB(1.0, 0.0, 0.7, 0.7);
				this.list[i++] = Color.FromARGB(1.0, 0.0, 0.0, 0.7);
				this.list[i++] = Color.FromARGB(1.0, 0.7, 0.0, 0.7);
				this.list[i++] = Color.FromARGB(0.8, 0.5, 0.5, 0.5);

				this.list[i++] = Color.FromARGB(1.0, 1.0, 1.0, 1.0);
				this.list[i++] = Color.FromARGB(1.0, 0.9, 0.9, 0.9);
				this.list[i++] = Color.FromARGB(1.0, 0.8, 0.8, 0.8);
				this.list[i++] = Color.FromARGB(1.0, 0.7, 0.7, 0.7);
				this.list[i++] = Color.FromARGB(1.0, 0.6, 0.6, 0.6);
				this.list[i++] = Color.FromARGB(1.0, 0.5, 0.5, 0.5);
				this.list[i++] = Color.FromARGB(1.0, 0.4, 0.4, 0.4);
				this.list[i++] = Color.FromARGB(1.0, 0.0, 0.0, 0.0);
			}

			this.OnChanged();
		}
		

		public int								Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public Color this[int index]
		{
			get
			{
				if ( index >= 0 && index < this.list.Count )
				{
					return (Color) this.list[index];
				}
				else
				{
					return Color.Empty;
				}
			}

			set
			{
				if ( index >= 0 && index < this.list.Count )
				{
					if ( (Color) this.list[index] != value )
					{
						this.list[index] = value;
						this.OnChanged();
					}
				}
			}
		}


		// Copie toutes les couleurs.
		public void CopyTo(ColorsCollection dst)
		{
			for ( int i=0 ; i<this.list.Count ; i++ )
			{
				dst[i] = this[i];
			}
			dst.OnChanged();
		}


		#region Serialization
		// Sérialise l'objet.
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Revision", 0);
			info.AddValue("Version", 0);
			info.AddValue("Count", this.list.Count);
			info.AddValue("List", this.list);
		}

		// Constructeur qui désérialise l'objet.
		protected ColorsCollection(SerializationInfo info, StreamingContext context)
		{
			int revision = info.GetInt32("Revision");
			int version = info.GetInt32("Version");
			int count = info.GetInt32("Count");
			this.list = (System.Collections.ArrayList) info.GetValue("List", typeof(System.Collections.ArrayList));
			this.OnChanged();
		}
		#endregion


		// Génère un événement pour dire qu'une couleur a changé.
		protected virtual void OnChanged()
		{
			if ( this.Changed != null )  // qq'un écoute ?
			{
				this.Changed(this);
			}
		}


		public event Support.EventHandler		Changed;

		protected System.Collections.ArrayList	list;
	}
}
