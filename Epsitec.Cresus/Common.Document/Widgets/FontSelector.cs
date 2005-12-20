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
		public static double BestHeight(double height, int totalLines)
		{
			int lines = (int) (height / FontSelector.sampleHeight);
			if ( lines == 0 )  lines ++;  // au moins une ligne, faut pas pousser

			lines = System.Math.Min(lines, totalLines);  // pas plus que nécessaire

			return lines*FontSelector.sampleHeight;
		}


		// Liste des OpenType.FontIdentity représentée.
		public System.Collections.ArrayList FontList
		{
			get
			{
				return this.fontList;
			}

			set
			{
				this.fontList = value;
			}
		}

		// Nombre de fontes rapides en tête de liste.
		public int QuickCount
		{
			get
			{
				return this.quickCount;
			}

			set
			{
				this.quickCount = value;
			}
		}

		// Liste des FontFace (string) sélectionné (sélection multiple).
		public System.Collections.ArrayList SelectedList
		{
			get
			{
				return this.selectedList;
			}

			set
			{
				this.selectedList = value;
				this.UpdateScroller();
				this.UpdateList();
			}
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
					this.SelectedLine = this.FaceToRank(this.fontFace);
				}
			}
		}


		// Peuple le widget seulement lorsqu'il a les dimensions définitives.
		// Ensuite, il ne devra plus être redimensionné.
		public void Build()
		{
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
						this.SelectedFontFace = face;
						this.OnSelectionChanged();
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
			else if ( message.Type == MessageType.MouseWheel )
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
			else if ( message.Type == MessageType.KeyDown )
			{
				bool ok = true;
				
				switch ( message.KeyCode )
				{
					case KeyCode.Escape:
						break;

					case KeyCode.Return:
						this.OnSelectionChanged();
						break;

					case KeyCode.ArrowUp:
						if ( message.IsCtrlPressed )
						{
							this.FirstLine = this.FirstLine-1;
						}
						else
						{
							this.SelectedLine = this.SelectedLine-1;
						}
						break;

					case KeyCode.ArrowDown:
						if ( message.IsCtrlPressed )
						{
							this.FirstLine = this.FirstLine+1;
						}
						else
						{
							this.SelectedLine = this.SelectedLine+1;
						}
						break;

					case KeyCode.PageUp:
						if ( message.IsCtrlPressed )
						{
							this.FirstLine = this.FirstLine-this.samples.Length;
						}
						else
						{
							this.SelectedLine = this.SelectedLine-this.samples.Length;
						}
						break;

					case KeyCode.PageDown:
						if ( message.IsCtrlPressed )
						{
							this.FirstLine = this.FirstLine+this.samples.Length;
						}
						else
						{
							this.SelectedLine = this.SelectedLine+this.samples.Length;
						}
						break;

					case KeyCode.Home:
						if ( message.IsCtrlPressed )
						{
							this.FirstLine = 0;
						}
						else
						{
							this.SelectedLine = 0;
						}
						break;

					case KeyCode.End:
						if ( message.IsCtrlPressed )
						{
							this.FirstLine = 100000;
						}
						else
						{
							this.SelectedLine = 100000;
						}
						break;

					case KeyCode.Back:
						if ( this.searchOnTheFly.Length > 0 )
						{
							this.searchOnTheFly = this.searchOnTheFly.Substring(0, this.searchOnTheFly.Length-1);
							this.searchTime = Types.Time.Now;
							this.SelectedLine = this.StartToRank(this.searchOnTheFly.ToUpper());
							message.Consumer = this;
							ok = false;
						}
						break;

					default:
						ok = false;
						break;
				}
				
				// Indique que l'événement clavier a été consommé, sinon il sera
				// traité par le parent, son parent, etc.
				if ( ok )
				{
					message.Consumer = this;
					this.searchOnTheFly = "";
				}
			}
			else if ( message.Type == MessageType.KeyPress )
			{
				System.TimeSpan time = Types.Time.Now - this.searchTime;
				double timeSeconds = time.TotalSeconds;
				
				if ( timeSeconds > 0.8 || timeSeconds < 0 )
				{
					this.searchOnTheFly = "";
				}
				
				char key = (char)message.KeyChar;
				int sel = -1;

				if ( (key >= 'a' && key <= 'z') || (key >= 'A' && key <= 'Z') )
				{
					this.searchOnTheFly = string.Format("{0}{1}", this.searchOnTheFly, key);
					this.searchTime = Types.Time.Now;
					sel = this.StartToRank(this.searchOnTheFly.ToUpper());
				}
				else
				{
					this.searchOnTheFly = "";
				}

				if ( key >= '1' && key <= '9' )
				{
					int i = (int) key-'1';
					sel = System.Math.Min(i, this.quickCount-1);
				}

				if ( sel != -1 )
				{
					this.SelectedLine = sel;
				}
				
				message.Consumer = this;
			}
			
			if ( message.IsMouseType )
			{
				message.Consumer = this;
			}
		}

		// Ligne sélectionnée.
		protected int SelectedLine
		{
			get
			{
				return this.selectedLine;
			}

			set
			{
				value = System.Math.Max(value, 0);
				value = System.Math.Min(value, this.fontList.Count-1);

				if ( this.selectedLine != value )
				{
					this.selectedLine = value;

					if ( this.selectedLine <  this.firstLine ||
						 this.selectedLine >= this.firstLine+this.samples.Length )  // sélection cachée ?
					{
						int first = this.selectedLine;
						first = System.Math.Min(first+this.samples.Length/2, this.fontList.Count-1);
						first = System.Math.Max(first-(this.samples.Length-1), 0);
						this.firstLine = first;  // montre la sélection
					}

					this.UpdateScroller();
					this.UpdateList();
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
		public void UpdateList()
		{
			for ( int i=0 ; i<samples.Length ; i++ )
			{
				int ii = this.firstLine+i;
				
				if ( ii < this.fontList.Count )
				{
					Common.OpenType.FontIdentity id = this.fontList[ii] as Common.OpenType.FontIdentity;
					this.samples[i].FontIdentity = id;

					string face = id.InvariantFaceName;
					if ( ii < this.quickCount )  // police rapide ?
					{
						if ( ii < 9 )  // police rapide avec un raccourci [1]..[9] ?
						{
							face = string.Format("{0}: {1}", (ii+1).ToString(System.Globalization.CultureInfo.InvariantCulture), Misc.Bold(face));
						}
						else	// police rapide sans raccourci
						{
							face = Misc.Bold(face);
						}
					}
					this.samples[i].FontFace = face;

					if ( this.selectedList == null )  // sélection unique ?
					{
						this.samples[i].SetSelected(ii == this.selectedLine);
					}
					else	// sélection multiple ?
					{
						face = this.samples[i].FontIdentity.InvariantFaceName;
						this.samples[i].SetSelected(this.selectedList.Contains(face));
					}
				}
				else
				{
					this.samples[i].FontIdentity = null;
					this.samples[i].SetSelected(false);
				}

				this.samples[i].Separator = (ii == this.quickCount-1 && this.quickCount != this.fontList.Count);
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
			for ( int i=this.quickCount ; i<this.fontList.Count ; i++ )
			{
				Common.OpenType.FontIdentity id = this.fontList[i] as Common.OpenType.FontIdentity;
				if ( id.InvariantFaceName.ToUpper().StartsWith(start) )  return i;
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

		
		protected static readonly double				sampleHeight = 30;

		protected string								fontFace;
		protected System.Collections.ArrayList			fontList = null;
		protected int									quickCount = 0;
		protected System.Collections.ArrayList			selectedList = null;
		protected Common.Document.Widgets.FontSample[]	samples;
		protected VScroller								scroller;
		protected int									firstLine = 0;
		protected int									selectedLine = -1;
		protected bool									ignoreChange = false;
		protected Types.Time							searchTime;
		protected string								searchOnTheFly = "";
	}
}
