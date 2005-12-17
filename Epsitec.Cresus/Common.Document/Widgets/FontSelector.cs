using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.OpenType;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// La classe FontSelector est un widget permettant de choisir une police.
	/// </summary>
	public class FontSelector : Widget
	{
		public FontSelector()
		{
			this.AutoEngage = false;
			this.AutoFocus  = true;

			this.InternalState |= InternalState.Focusable;
			this.InternalState |= InternalState.Engageable;
		}

		public FontSelector(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		// Retourne la meilleure largeur.
		public static double BestWidth()
		{
			return 300;
		}

		// Retourne la meilleure hauteur possible, en principe plus petite que la hauteur demandée.
		public static double BestHeight(double height, bool enableSymbols)
		{
			int lines = (int) (height / FontSelector.sampleHeight);
			if ( lines == 0 )  lines ++;  // au moins une ligne, faut pas pousser

			System.Collections.ArrayList list = Misc.GetFontList(enableSymbols);
			if ( lines > list.Count )  lines = list.Count;  // il ne sert à rien que la liste soit plus haute
			
			return lines*FontSelector.sampleHeight;
		}


		// Peuple le widget seulement lorsqu'il a les dimensions définitives.
		// Ensuite, il ne devra plus être redimensionné.
		public void Build(bool enableSymbols, System.Collections.ArrayList quickList)
		{
			if ( quickList == null )  // liste brutte ?
			{
				this.fontList = Misc.GetFontList(enableSymbols);
				this.quickLine = -1;
			}
			else	// liste des fontes rapides ?
			{
				// Copie la liste en enlevant toutes les fontes rapides.
				System.Collections.ArrayList inList = Misc.GetFontList(enableSymbols);
				this.fontList = new System.Collections.ArrayList();
				System.Collections.ArrayList begin = new System.Collections.ArrayList();
				foreach ( Common.OpenType.FontIdentity id in inList )
				{
					if ( quickList.Contains(id.InvariantFaceName) )
					{
						begin.Add(id.InvariantFaceName);
					}
					else
					{
						this.fontList.Add(id);
					}
				}

				// Remet les fontes rapides au début.
				int ii = 0;
				foreach ( string quick in begin )
				{
					this.fontList.Insert(ii++, Misc.DefaultFontIdentityStyle(quick));
				}

				this.quickLine = begin.Count-1;
			}

			Rectangle rect = this.Client.Bounds;

			this.scroller = new VScroller(this);
			this.scroller.Bounds = new Rectangle(rect.Right-this.scroller.DefaultWidth, rect.Bottom, this.scroller.DefaultWidth, rect.Height);
			this.scroller.IsInverted = true;  // zéro en haut
			this.scroller.ValueChanged += new Epsitec.Common.Support.EventHandler(this.ScrollerValueChanged);

			int lines = (int) (this.Height / FontSelector.sampleHeight);
			this.samples = new Common.Document.Widgets.FontSample[lines];

			rect.Width -= this.scroller.DefaultWidth;
			rect.Bottom = rect.Top-FontSelector.sampleHeight;
			for ( int i=0 ; i<lines ; i++ )
			{
				this.samples[i] = new Epsitec.Common.Document.Widgets.FontSample(this);
				this.samples[i].Bounds = rect;

				rect.Offset(0, -FontSelector.sampleHeight);
			}

			this.UpdateScroller();
			this.UpdateList();
		}


		// Police sélectionnée.
		public string SelectedFontFace
		{
			get
			{
				return this.fontFace;
			}

			set
			{
				if ( this.fontFace != value )
				{
					this.fontFace = value;

					this.selectedLine = this.FaceToRank(this.fontFace);
					
					if ( this.selectedLine < this.firstLine || this.selectedLine >= this.firstLine+this.samples.Length )
					{
						this.firstLine = System.Math.Min(this.selectedLine+this.samples.Length/2, this.fontList.Count-1);
						this.firstLine = System.Math.Max(this.firstLine-(this.samples.Length-1), 0);
					}

					this.UpdateScroller();
					this.UpdateList();
				}
			}
		}

		public void SelectedList(System.Collections.ArrayList list)
		{
			this.selectedList = list;
			this.UpdateList();
		}


		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			if ( message.Type == MessageType.MouseDown )
			{
				if ( pos.X < this.Bounds.Right-this.scroller.Width )
				{
					int sel = this.firstLine + (int)((this.Bounds.Height-pos.Y) / FontSelector.sampleHeight);
					string face = this.RankToFace(sel);

					if ( this.selectedList == null )  // sélection unique ?
					{
						if ( this.SelectedFontFace != face )
						{
							this.SelectedFontFace = face;
							this.OnSelectionChanged();
						}
					}
					else	// sélection multiple ?
					{
						if ( this.selectedList.Contains(face) )
						{
							this.selectedList.Remove(face);
						}
						else
						{
							this.selectedList.Add(face);
						}
						this.UpdateList();
						this.OnSelectionChanged();
					}
				}
			}

			if ( message.Type == MessageType.MouseWheel )
			{
				if ( message.Wheel > 0 )
				{
					this.FirstLine = this.FirstLine-3;
				}
				if ( message.Wheel < 0 )
				{
					this.FirstLine = this.FirstLine+3;
				}
			}

			if ( message.Type == MessageType.KeyDown )
			{
				if ( message.KeyCode == KeyCode.Escape ||
					 message.KeyCode == KeyCode.Return )
				{
					this.OnCloseNeeded();
				}

				if ( message.KeyCode == KeyCode.ArrowUp )
				{
					this.FirstLine = this.FirstLine-1;
				}
				if ( message.KeyCode == KeyCode.ArrowDown )
				{
					this.FirstLine = this.FirstLine+1;
				}

				if ( message.KeyCode == KeyCode.PageUp )
				{
					this.FirstLine = this.FirstLine-this.samples.Length;
				}
				if ( message.KeyCode == KeyCode.PageDown )
				{
					this.FirstLine = this.FirstLine+this.samples.Length;
				}

				if ( message.KeyCode == KeyCode.Home )
				{
					this.FirstLine = 0;
				}
				if ( message.KeyCode == KeyCode.End )
				{
					this.FirstLine = 100000;
				}
			}

			if ( message.Type == MessageType.KeyPress )
			{
				string start = string.Format("{0}", (char)message.KeyChar);
				int first = this.StartToRank(start);
				if ( first != -1 )
				{
					this.FirstLine = first;
				}
			}
		}

		// Première ligne visible.
		protected int FirstLine
		{
			get
			{
				return this.firstLine;
			}

			set
			{
				value = System.Math.Max(value, 0);
				value = System.Math.Min(value, this.fontList.Count-this.samples.Length);

				if ( this.firstLine != value )
				{
					this.firstLine = value;

					this.UpdateScroller();
					this.UpdateList();
				}
			}
		}


		// Met à jour l'ascenseur.
		protected void UpdateScroller()
		{
			this.ignoreChange = true;

			if ( this.samples.Length >= this.fontList.Count )
			{
				this.scroller.Enable = false;
				this.scroller.MinValue = 0M;
				this.scroller.MaxValue = 1M;
				this.scroller.VisibleRangeRatio = 1M;
				this.scroller.Value = 0M;
			}
			else
			{
				this.scroller.Enable = true;
				this.scroller.MinValue = 0M;
				this.scroller.MaxValue = (decimal) (this.fontList.Count - this.samples.Length);
				this.scroller.VisibleRangeRatio = (decimal) ((double)this.samples.Length / this.fontList.Count);
				this.scroller.Resolution = 1M;
				this.scroller.SmallChange = 1M;
				this.scroller.LargeChange = (decimal) this.samples.Length;
				this.scroller.Value = (decimal) this.firstLine;
			}

			this.ignoreChange = false;
		}

		// Met à jour le contenu de la liste.
		protected void UpdateList()
		{
			for ( int i=0 ; i<samples.Length ; i++ )
			{
				int ii = this.firstLine+i;
				
				if ( ii < this.fontList.Count )
				{
					this.samples[i].FontIdentity = this.fontList[ii] as Common.OpenType.FontIdentity;

					if ( this.selectedList == null )  // sélection unique ?
					{
						this.samples[i].SetSelected(ii == this.selectedLine);
					}
					else	// sélection multiple ?
					{
						string face = this.samples[i].FontIdentity.InvariantFaceName;
						this.samples[i].SetSelected(this.selectedList.Contains(face));
					}
				}
				else
				{
					this.samples[i].FontIdentity = null;
					this.samples[i].SetSelected(false);
				}

				this.samples[i].Separator = (ii == this.quickLine);
				this.samples[i].Last      = (i == samples.Length-1);
			}
		}


		// La valeur de l'ascenseur a changé.
		private void ScrollerValueChanged(object sender)
		{
			if ( this.ignoreChange )  return;

			if ( this.firstLine != (int) this.scroller.Value )
			{
				this.firstLine = (int) this.scroller.Value;
				this.UpdateList();
			}
		}


		protected int StartToRank(string start)
		{
			for ( int i=0 ; i<this.fontList.Count ; i++ )
			{
				Common.OpenType.FontIdentity id = this.fontList[i] as Common.OpenType.FontIdentity;
				if ( id.InvariantFaceName.StartsWith(start) )  return i;
			}
			return -1;
		}

		protected int FaceToRank(string face)
		{
			for ( int i=0 ; i<this.fontList.Count ; i++ )
			{
				Common.OpenType.FontIdentity id = this.fontList[i] as Common.OpenType.FontIdentity;
				if ( id.InvariantFaceName == face )  return i;
			}
			return -1;
		}

		protected string RankToFace(int rank)
		{
			if ( rank == -1 )  return null;
			Common.OpenType.FontIdentity id = this.fontList[rank] as Common.OpenType.FontIdentity;
			return id.InvariantFaceName;
		}


		// Génère un événement pour dire que la fermeture est nécessaire.
		protected virtual void OnSelectionChanged()
		{
			if ( this.SelectionChanged != null )  // qq'un écoute ?
			{
				this.SelectionChanged(this);
			}
		}

		public event Support.EventHandler SelectionChanged;

		
		// Génère un événement pour dire que la fermeture est nécessaire.
		protected virtual void OnCloseNeeded()
		{
			if ( this.CloseNeeded != null )  // qq'un écoute ?
			{
				this.CloseNeeded(this);
			}
		}

		public event Support.EventHandler CloseNeeded;

		
		protected static readonly double				sampleHeight = 30;

		protected string								fontFace;
		protected System.Collections.ArrayList			selectedList = null;
		protected System.Collections.ArrayList			fontList;
		protected Common.Document.Widgets.FontSample[]	samples;
		protected VScroller								scroller;
		protected int									firstLine = 0;
		protected int									quickLine = -1;
		protected int									selectedLine = 0;
		protected bool									ignoreChange = false;
	}
}
