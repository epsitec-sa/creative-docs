using System.IO;
using System.Runtime.Serialization;

namespace Epsitec.Common.Drawing
{
	public enum ColorCollectionType
	{
		Default,			//	palette standard
		Rainbow,			//	palette arc-en-ciel
		Light,				//	palette pastel
		Dark,				//	palette foncée
		Gray,				//	palette monochrome
	}
	
	/// <summary>
	/// La classe ColorCollection contient une collection de couleurs.
	/// </summary>

	[System.Serializable()]
	public class ColorCollection : ISerializable, System.Collections.ICollection
	{
		public ColorCollection()
		{
			this.colors = new Color[ColorCollection.DefaultCount];
			
			for ( int i=0 ; i<ColorCollection.DefaultCount ; i++ )
			{
				this.colors[i] = Color.Empty;
			}
			
			this.Initialise(ColorCollectionType.Default);
		}

		public ColorCollection(ColorCollectionType type) : this ()
		{
			this.Initialise (type);
		}

		
		public void Initialise(ColorCollectionType type)
		{
			switch (type)
			{
				case ColorCollectionType.Gray:
					
					this.colors[0+8*0] = Color.FromARGB(0.0, 1.0, 1.0, 1.0);
					this.colors[0+8*1] = Color.FromARGB(1.0, 1.0, 0.0, 0.0);
					this.colors[0+8*2] = Color.FromARGB(1.0, 0.0, 1.0, 0.0);
					this.colors[0+8*3] = Color.FromARGB(1.0, 0.0, 0.0, 1.0);

					for ( int i=0 ; i<28 ; i++ )
					{
						int j = 1 + (i/4) + (i%4)*8;
						this.colors[j] = Color.FromBrightness(1.0-(i/27.0));
					}
					
					break;
			
				case ColorCollectionType.Rainbow:
					
					this.colors[0+8*0] = Color.FromARGB(0.0, 1.00, 1.00, 1.00);
					this.colors[0+8*1] = Color.FromARGB(1.0, 1.00, 1.00, 1.00);
					this.colors[0+8*2] = Color.FromARGB(1.0, 0.83, 0.83, 0.83);
					this.colors[0+8*3] = Color.FromARGB(1.0, 0.67, 0.67, 0.67);
					this.colors[1+8*0] = Color.FromARGB(1.0, 0.50, 0.50, 0.50);
					this.colors[1+8*1] = Color.FromARGB(1.0, 0.33, 0.33, 0.33);
					this.colors[1+8*2] = Color.FromARGB(1.0, 0.17, 0.17, 0.17);
					this.colors[1+8*3] = Color.FromARGB(1.0, 0.00, 0.00, 0.00);

					for ( int i=0 ; i<24 ; i++ )
					{
						int j = 2 + (i/4) + (i%4)*8;
						double h = 1.0-(i/24.0);
						this.colors[j] = Color.FromHSV(h*360, 1.0, 1.0);
					}
					
					break;
			
				case ColorCollectionType.Light:
					this.colors[0+8*0] = Color.FromARGB(0.0, 1.00, 1.00, 1.00);
					this.colors[0+8*1] = Color.FromARGB(1.0, 1.00, 1.00, 1.00);
					this.colors[0+8*2] = Color.FromARGB(1.0, 0.83, 0.83, 0.83);
					this.colors[0+8*3] = Color.FromARGB(1.0, 0.67, 0.67, 0.67);
					this.colors[1+8*0] = Color.FromARGB(1.0, 0.50, 0.50, 0.50);
					this.colors[1+8*1] = Color.FromARGB(1.0, 0.33, 0.33, 0.33);
					this.colors[1+8*2] = Color.FromARGB(1.0, 0.17, 0.17, 0.17);
					this.colors[1+8*3] = Color.FromARGB(1.0, 0.00, 0.00, 0.00);

					for ( int i=0 ; i<24 ; i++ )
					{
						int j = 2 + (i/4) + (i%4)*8;
						double h = 1.0-(i/24.0);
						this.colors[j] = Color.FromHSV(h*360, 0.3, 1.0);
					}
					
					break;
			
				case ColorCollectionType.Dark:
					
					this.colors[0+8*0] = Color.FromARGB(0.0, 1.00, 1.00, 1.00);
					this.colors[0+8*1] = Color.FromARGB(1.0, 1.00, 1.00, 1.00);
					this.colors[0+8*2] = Color.FromARGB(1.0, 0.83, 0.83, 0.83);
					this.colors[0+8*3] = Color.FromARGB(1.0, 0.67, 0.67, 0.67);
					this.colors[1+8*0] = Color.FromARGB(1.0, 0.50, 0.50, 0.50);
					this.colors[1+8*1] = Color.FromARGB(1.0, 0.33, 0.33, 0.33);
					this.colors[1+8*2] = Color.FromARGB(1.0, 0.17, 0.17, 0.17);
					this.colors[1+8*3] = Color.FromARGB(1.0, 0.00, 0.00, 0.00);

					for ( int i=0 ; i<24 ; i++ )
					{
						int j = 2 + (i/4) + (i%4)*8;
						double h = 1.0-(i/24.0);
						this.colors[j] = Color.FromHSV(h*360, 1.0, 0.7);
					}
					
					break;
				
				case ColorCollectionType.Default:
					
					this.colors[0*8+0] = Color.FromARGB(0.0, 1.0, 1.0, 1.0);
					this.colors[0*8+1] = Color.FromARGB(1.0, 1.0, 0.7, 0.7);
					this.colors[0*8+2] = Color.FromARGB(1.0, 1.0, 1.0, 0.7);
					this.colors[0*8+3] = Color.FromARGB(1.0, 0.7, 1.0, 0.7);
					this.colors[0*8+4] = Color.FromARGB(1.0, 0.7, 1.0, 1.0);
					this.colors[0*8+5] = Color.FromARGB(1.0, 0.7, 0.7, 1.0);
					this.colors[0*8+6] = Color.FromARGB(1.0, 1.0, 0.7, 1.0);
					this.colors[0*8+7] = Color.FromARGB(0.2, 0.5, 0.5, 0.5);

					this.colors[1*8+0] = Color.FromARGB(0.3, 1.0, 1.0, 1.0);
					this.colors[1*8+1] = Color.FromARGB(1.0, 1.0, 0.0, 0.0);
					this.colors[1*8+2] = Color.FromARGB(1.0, 1.0, 1.0, 0.0);
					this.colors[1*8+3] = Color.FromARGB(1.0, 0.0, 1.0, 0.0);
					this.colors[1*8+4] = Color.FromARGB(1.0, 0.0, 1.0, 1.0);
					this.colors[1*8+5] = Color.FromARGB(1.0, 0.0, 0.0, 1.0);
					this.colors[1*8+6] = Color.FromARGB(1.0, 1.0, 0.0, 1.0);
					this.colors[1*8+7] = Color.FromARGB(0.5, 0.5, 0.5, 0.5);

					this.colors[2*8+0] = Color.FromARGB(0.6, 1.0, 1.0, 1.0);
					this.colors[2*8+1] = Color.FromARGB(1.0, 0.7, 0.0, 0.0);
					this.colors[2*8+2] = Color.FromARGB(1.0, 0.7, 0.7, 0.0);
					this.colors[2*8+3] = Color.FromARGB(1.0, 0.0, 0.7, 0.0);
					this.colors[2*8+4] = Color.FromARGB(1.0, 0.0, 0.7, 0.7);
					this.colors[2*8+5] = Color.FromARGB(1.0, 0.0, 0.0, 0.7);
					this.colors[2*8+6] = Color.FromARGB(1.0, 0.7, 0.0, 0.7);
					this.colors[2*8+7] = Color.FromARGB(0.8, 0.5, 0.5, 0.5);

					this.colors[3*8+0] = Color.FromARGB(1.0, 1.0, 1.0, 1.0);
					this.colors[3*8+1] = Color.FromARGB(1.0, 0.9, 0.9, 0.9);
					this.colors[3*8+2] = Color.FromARGB(1.0, 0.8, 0.8, 0.8);
					this.colors[3*8+3] = Color.FromARGB(1.0, 0.7, 0.7, 0.7);
					this.colors[3*8+4] = Color.FromARGB(1.0, 0.6, 0.6, 0.6);
					this.colors[3*8+5] = Color.FromARGB(1.0, 0.5, 0.5, 0.5);
					this.colors[3*8+6] = Color.FromARGB(1.0, 0.4, 0.4, 0.4);
					this.colors[3*8+7] = Color.FromARGB(1.0, 0.0, 0.0, 0.0);
					
					break;
				
				default:
					throw new System.ArgumentOutOfRangeException ("type", type, "Type not supported.");
			}
			
			this.OnChanged();
		}
		

		public Color							this[int index]
		{
			get
			{
				if ( index >= 0 && index < this.colors.Length )
				{
					return this.colors[index];
				}
				else
				{
					return Color.Empty;
				}
			}
			
			set
			{
				if ( index >= 0 && index < this.colors.Length )
				{
					if ( this.colors[index] != value )
					{
						this.colors[index] = value;
						this.OnChanged();
					}
				}
			}
		}
		
		
		#region ICollection Members
		public int								Count
		{
			get
			{
				return this.colors.Length;
			}
		}
		
		public bool								IsSynchronized
		{
			get
			{
				return false;
			}
		}
		
		public object							SyncRoot
		{
			get
			{
				return this;
			}
		}
		
		void System.Collections.ICollection.CopyTo(System.Array array, int index)
		{
			this.colors.CopyTo (array, index);
		}
		#endregion
		
		#region IEnumerable Members
		public System.Collections.IEnumerator GetEnumerator()
		{
			return this.colors.GetEnumerator ();
		}
		#endregion
		
		public void CopyTo(ColorCollection dst)
		{
			this.colors.CopyTo (dst.colors, 0);
			dst.OnChanged();
		}


		#region Serialization
		protected ColorCollection(SerializationInfo info, StreamingContext context)
		{
			int revision = info.GetInt32("Rev");
			
			System.Diagnostics.Debug.Assert (revision == 0);
			
			this.colors = (Color[]) info.GetValue("Colors", typeof(Color[]));
			
			this.OnChanged();
		}
		
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Rev", 0);
			info.AddValue("Colors", this.colors);
		}

		#endregion


		protected virtual void OnChanged()
		{
			if ( this.Changed != null )  // qq'un écoute ?
			{
				this.Changed(this);
			}
		}


		public event Support.EventHandler		Changed;

		protected Color[]						colors;
		protected const int						DefaultCount = 32;
	}
}
