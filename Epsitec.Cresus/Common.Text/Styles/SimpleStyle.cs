//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Styles
{
	/// <summary>
	/// La classe SimpleStyle permet de décrire un style simple, constitué d'une
	/// défition de fonte et de paragraphe, plus quelques autres détails.
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
			//	Recalcule le numéro de version correspondant à ce style
			//	et regénère le résumé synthétique des propriétés si cela
			//	est nécessaire.
			
			//	Sélectionne le plus grand numéro trouvé parmi les divers
			//	composants et l'affecte à la synthèse.
			
			if ((this.components != null) &&
				(this.components.Length > 0))
			{
				long version = 0;
				
				for (int i = 0; i < this.components.Length; i++)
				{
					version = System.Math.Max (version, this.components[i].Version);
				}
				
				//	La synthèse doit-elle être regénérée et/ou son numéro de
				//	version est-il périmé ?
				
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
			//	composants. La synthèse résultant de la fusion des composants,
			//	si tous les composants sont égaux, la synthèse le sera aussi.
			
			//	Cette façon de faire évite de devoir recalculer la synthèse si
			//	celle-ci a été invalidée.
			
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
