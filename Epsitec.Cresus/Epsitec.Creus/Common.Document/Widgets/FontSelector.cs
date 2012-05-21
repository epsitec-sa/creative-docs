using Epsitec.Common.Support;
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

			this.InternalState |= WidgetInternalState.Focusable;
			this.InternalState |= WidgetInternalState.Engageable;

			this.scroller = new VScroller(this);
			this.scroller.IsInverted = true;  // zéro en haut
			this.scroller.ValueChanged += this.ScrollerValueChanged;
			
			this.samples = new Common.Document.Widgets.FontSample[0];
		}

		public FontSelector(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		public double SampleHeight
		{
			//	Hauteur d'un échantillon.
			get
			{
				return this.sampleHeight;
			}

			set
			{
				if ( this.sampleHeight != value )
				{
					this.sampleHeight = value;
					this.UpdateClientGeometry();
					this.UpdateContents();
				}
			}
		}

		public bool SampleAbc
		{
			//	Type d'un échantillon.
			get
			{
				return this.sampleAbc;
			}

			set
			{
				if ( this.sampleAbc != value )
				{
					this.sampleAbc = value;
					this.UpdateList();
				}
			}
		}


		public static double BestWidth(double sampleHeight, bool sampleAbc)
		{
			//	Retourne la meilleure largeur.
			if ( sampleAbc )
			{
				return 220 + (sampleHeight-20)*1.7;
			}
			else
			{
				return 260 + (sampleHeight-20)*4.5;
			}
		}

		public static double BestHeight(double height, int totalLines, double sampleHeight)
		{
			//	Retourne la meilleure hauteur possible, en principe plus petite que la hauteur demandée.
			int lines = (int) (height / sampleHeight);
			if ( lines == 0 )  lines ++;  // au moins une ligne, faut pas pousser

			lines = System.Math.Min(lines, totalLines);  // pas plus que nécessaire

			return lines*sampleHeight;
		}


		public System.Collections.ArrayList FontList
		{
			//	Liste des OpenType.FontIdentity représentée.
			get
			{
				return this.fontList;
			}

			set
			{
				this.fontList = value;
			}
		}

		public int QuickCount
		{
			//	Nombre de fontes rapides en tête de liste.
			get
			{
				return this.quickCount;
			}

			set
			{
				this.quickCount = value;
			}
		}

		public System.Collections.ArrayList SelectedList
		{
			//	Liste des FontFace (string) sélectionné (sélection multiple).
			get
			{
				return this.selectedList;
			}

			set
			{
				this.selectedList = value;
				this.UpdateContents();
			}
		}

		public string SelectedFontFace
		{
			//	Police sélectionnée.
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


		public override Size GetBestFitSize()
		{
			double maxHeight = System.Math.Min (500, this.MaxHeight);
			
			double w = FontSelector.BestWidth(this.sampleHeight, this.sampleAbc);
			double h = FontSelector.BestHeight(maxHeight, this.fontList.Count, this.sampleHeight);
			
			return new Drawing.Size(w, h);
		}

		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();
			if ( this.RebuildContents() )
			{
				this.UpdateContents();
			}
		}
		
		public bool RebuildContents()
		{
			if ( this.scroller == null )  return false;

			Rectangle rect = this.Client.Bounds;
			bool changed = false;

			//	Crée les échantillons, si nécessaire.
			int lines = (int) (rect.Height/this.sampleHeight);
			double suppl = rect.Height - lines*this.sampleHeight;
			double supplAll  = (double) ((int) (suppl/lines));  // supplément pour toutes les lignes
			double supplLast = (double) ((int) (suppl%lines));  // supplément pour la dernière ligne

			if ( this.samples == null || this.samples.Length != lines )
			{
				if ( this.samples != null )
				{
					foreach ( Widget widget in this.samples )
					{
						widget.Dispose();
					}
					this.samples = null;
				}

				this.samples = new Common.Document.Widgets.FontSample[lines];

				for ( int i=0 ; i<lines ; i++ )
				{
					this.samples[i] = new Epsitec.Common.Document.Widgets.FontSample(this);
				}
				
				changed = true;
			}

			//	Positionne l'ascenseur.
			Rectangle r = rect;
			r.Left = r.Right-this.scroller.PreferredWidth;
			this.scroller.SetManualBounds(r);

			//	Positionne les échantillons.
			double top = rect.Top;
			for ( int i=0 ; i<lines ; i++ )
			{
				double h = this.sampleHeight+supplAll;
				if ( i == lines-1 )  h += supplLast;  // dernière ligne ?
				r = new Rectangle(rect.Left, top-h, rect.Width-this.scroller.ActualWidth, h);
				this.samples[i].SetManualBounds(r);
				this.samples[i].FontHeight = this.sampleHeight;

				top -= h;
			}
			
			return changed;
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			//	Dessine le cadre.
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			Rectangle rect = this.Client.Bounds;
			rect.Width -= this.scroller.ActualWidth;

			Color frameColor = adorner.ColorTextFieldBorder(this.IsEnabled);

			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(frameColor);  // dessine le cadre
		}

		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			//	Gestion des événements clavier/souris.
			if ( message.MessageType == MessageType.MouseDown )
			{
				if ( pos.X < this.ActualBounds.Right-this.scroller.ActualWidth )
				{
					int sel = this.Detect(pos);
					if ( sel < this.fontList.Count )
					{
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
			}
			else if ( message.MessageType == MessageType.MouseWheel )
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
			else if ( message.MessageType == MessageType.KeyDown )
			{
				bool ok = true;
				
				switch ( message.KeyCode )
				{
					case KeyCode.Escape:
						ok = false;
						break;

					case KeyCode.NumericEnter:
					case KeyCode.Return:
						if ( this.selectedList == null && this.selectedLine != -1 )  // sélection unique ?
						{
							string face = this.RankToFace(this.selectedLine);
							this.SelectedFontFace = face;
						}
						this.OnSelectionChanged();
						break;

					case KeyCode.ArrowUp:
						if ( message.IsControlPressed )
						{
							this.FirstLine = this.FirstLine-1;
						}
						else
						{
							this.SelectedLine = this.SelectedLine-1;
						}
						break;

					case KeyCode.ArrowDown:
						if ( message.IsControlPressed )
						{
							this.FirstLine = this.FirstLine+1;
						}
						else
						{
							this.SelectedLine = this.SelectedLine+1;
						}
						break;

					case KeyCode.PageUp:
						if ( message.IsControlPressed )
						{
							this.FirstLine = this.FirstLine-this.samples.Length;
						}
						else
						{
							this.SelectedLine = this.SelectedLine-this.samples.Length;
						}
						break;

					case KeyCode.PageDown:
						if ( message.IsControlPressed )
						{
							this.FirstLine = this.FirstLine+this.samples.Length;
						}
						else
						{
							this.SelectedLine = this.SelectedLine+this.samples.Length;
						}
						break;

					case KeyCode.Home:
						if ( message.IsControlPressed )
						{
							this.FirstLine = 0;
						}
						else
						{
							this.SelectedLine = 0;
						}
						break;

					case KeyCode.End:
						if ( message.IsControlPressed )
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
				
				//	Indique que l'événement clavier a été consommé, sinon il sera
				//	traité par le parent, son parent, etc.
				if ( ok )
				{
					message.Consumer = this;
					this.searchOnTheFly = "";
				}
			}
			else if ( message.MessageType == MessageType.KeyPress )
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

		protected int Detect(Point pos)
		{
			//	Détecte la ligne visée par la souris.
			for ( int i=0 ; i<this.samples.Length ; i++ )
			{
				if ( this.samples[i].ActualBounds.Contains(pos) )  return this.firstLine+i;
			}
			return -1;
		}

		protected int SelectedLine
		{
			//	Ligne sélectionnée.
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

					if (this.samples.Length > 0)
					{
						if ((this.selectedLine <  this.firstLine) ||
							(this.selectedLine >= this.firstLine+this.samples.Length))  // sélection cachée ?
						{
							int first = this.selectedLine;
							first = System.Math.Min (first+this.samples.Length/2, this.fontList.Count-1);
							first = System.Math.Max (first-(this.samples.Length-1), 0);
							this.firstLine = first;  // montre la sélection
						}

						this.UpdateContents ();
					}
				}
			}
		}

		protected int FirstLine
		{
			//	Première ligne visible.
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

					this.UpdateContents();
				}
			}
		}


		protected void UpdateScroller()
		{
			//	Met à jour l'ascenseur.
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
		
		public void UpdateContents()
		{
			this.UpdateScroller();
			this.UpdateList();
		}

		public void UpdateList()
		{
			//	Met à jour le contenu de la liste.
			for (int i=0; i<this.samples.Length; i++)
			{
				int ii = this.firstLine+i;

				if (ii < this.fontList.Count)
				{
					Common.OpenType.FontIdentity id = this.fontList[ii] as Common.OpenType.FontIdentity;
					this.samples[i].FontIdentity = id;

					string prefix = "";
					string face   = id.InvariantFaceName;
					string suffix = "";
					if (ii < this.quickCount)  // police rapide ?
					{
						if (ii < 9)  // police rapide avec un raccourci [1]..[9] ?
						{
							prefix = string.Format ("{0}: <b>", (ii+1).ToString (System.Globalization.CultureInfo.InvariantCulture));
							suffix = "</b>";
						}
						else	// police rapide sans raccourci
						{
							prefix = "<b>";
							suffix = "</b>";
						}
					}
					this.samples[i].SetFontFace (prefix, face, suffix);
					this.samples[i].IsSampleAbc = this.sampleAbc;

					if (this.selectedList == null)  // sélection unique ?
					{
						this.samples[i].ActiveState = (ii == this.selectedLine) ? ActiveState.Yes : ActiveState.No;
					}
					else	// sélection multiple ?
					{
						face = this.samples[i].FontIdentity.InvariantFaceName;
						this.samples[i].ActiveState = this.selectedList.Contains (face) ? ActiveState.Yes : ActiveState.No;
					}
				}
				else
				{
					this.samples[i].FontIdentity = null;
					this.samples[i].SetFontFace (null, null, null);
					this.samples[i].IsSampleAbc  = false;
					this.samples[i].ActiveState  = ActiveState.No;
				}

				this.samples[i].IsSeparator = (ii == this.quickCount-1 && this.quickCount != this.fontList.Count);
				this.samples[i].IsLast      = (i == samples.Length-1);
			}
		}


		private void ScrollerValueChanged(object sender)
		{
			//	La valeur de l'ascenseur a changé.
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


		protected virtual void OnSelectionChanged()
		{
			//	Génère un événement pour dire que la fermeture est nécessaire.
			var handler = this.GetUserEventHandler("SelectionChanged");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event EventHandler			SelectionChanged
		{
			add
			{
				this.AddUserEventHandler("SelectionChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("SelectionChanged", value);
			}
		}

		
		protected double								sampleHeight = 30;
		protected bool									sampleAbc = false;
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
