//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Styles
{
	/// <summary>
	/// La classe SimpleStyle permet de d�crire un style simple, constitu� d'une
	/// d�fition de fonte et de paragraphe, plus quelques autres d�tails.
	/// </summary>
	public sealed class SimpleStyle : BaseStyle
	{
		public SimpleStyle()
		{
		}
		
		public SimpleStyle(System.Collections.ICollection components)
		{
			this.Initialise (components);
		}
		
		
		public override bool					IsRichStyle
		{
			get
			{
				return false;
			}
		}
		
		
		public TextStyle						Synthesis
		{
			get
			{
				if (this.synthesis == null)
				{
					this.Update ();
				}
				
				return this.synthesis;
			}
		}
		
		
		public void Invalidate()
		{
			this.ClearContentsSignature ();
			
			this.synthesis = null;
		}
		
		public void Update()
		{
			//	Recalcule le num�ro de version correspondant � ce style
			//	et reg�n�re le r�sum� synth�tique des propri�t�s si cela
			//	est n�cessaire.
			
			//	S�lectionne le plus grand num�ro trouv� parmi les divers
			//	composants et l'affecte � la synth�se.
			
			if ((this.components != null) &&
				(this.components.Length > 0))
			{
				long version = 0;
				
				for (int i = 0; i < this.components.Length; i++)
				{
					version = System.Math.Max (version, this.components[i].Version);
				}
				
				//	La synth�se doit-elle �tre reg�n�r�e et/ou son num�ro de
				//	version est-il p�rim� ?
				
				if ((this.synthesis == null) ||
					(this.synthesis.Version != version))
				{
					this.synthesis = new TextStyle ();
					
					TextStyle.Accumulator accumulator = this.synthesis.StartAccumulation ();
					
					for (int i = 0; i < this.components.Length; i++)
					{
						accumulator.Accumulate (this.components[i]);
					}
					
					accumulator.Done ();
					
					this.ClearContentsSignature ();
				}
			}
			else
			{
				this.synthesis = null;
				this.ClearContentsSignature ();
			}
		}
		
		
		private void Initialise(System.Collections.ICollection components)
		{
			int count = components.Count;
			
			this.components = new TextStyle[count];
			components.CopyTo (this.components, 0);
			
			this.Invalidate ();
		}
		
		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			//	Calcule la signature en se basant exclusivement sur celle des
			//	composants. La synth�se r�sultant de la fusion des composants,
			//	si tous les composants sont �gaux, la synth�se le sera aussi.
			
			//	Cette fa�on de faire �vite de devoir recalculer la synth�se si
			//	celle-ci a �t� invalid�e.
			
			if ((this.components != null) &&
				(this.components.Length > 0))
			{
				for (int i = 0; i < this.components.Length; i++)
				{
					checksum.UpdateValue (this.components[i].GetContentsSignature ());
				}
			}
		}
		
		public override bool CompareEqualContents(object value)
		{
			return SimpleStyle.CompareEqualContents(this, value as Styles.SimpleStyle);
		}
		
		
		public static bool CompareEqualContents(Styles.SimpleStyle a, Styles.SimpleStyle b)
		{
			if ((a.components == null) &&
				(b.components == null))
			{
				return true;
			}
			if ((a.components == null) ||
				(b.components == null))
			{
				return false;
			}
			if (a.components.Length != b.components.Length)
			{
				return false;
			}
			
			for (int i = 0; i < a.components.Length; i++)
			{
				TextStyle sa = a.components[i];
				TextStyle sb = b.components[i];
				
				if (sa.GetContentsSignature () != sb.GetContentsSignature ())
				{
					return false;
				}
			}
			
			for (int i = 0; i < a.components.Length; i++)
			{
				TextStyle sa = a.components[i];
				TextStyle sb = b.components[i];
				
				if (TextStyle.CompareEqualContents (sa, sb) == false)
				{
					return false;
				}
			}
			
			return true;
		}
		
		
		private TextStyle[]						components;
		private TextStyle						synthesis;
	}
}
