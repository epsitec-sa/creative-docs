//  Copyright (c) 2006, Gustavo Franco
//  Email:  gustavo_franco@hotmail.com
//  All rights reserved.

//  Redistribution and use in source and binary forms, with or without modification, 
//  are permitted provided that the following conditions are met:

//  Redistributions of source code must retain the above copyright notice, 
//  this list of conditions and the following disclaimer. 
//  Redistributions in binary form must reproduce the above copyright notice, 
//  this list of conditions and the following disclaimer in the documentation 
//  and/or other materials provided with the distribution. 

//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
//  PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
//  REMAINS UNCHANGED.
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Drawing.IconLib.Exceptions;
using System.Drawing.IconLib.EncodingFormats;

namespace System.Drawing.IconLib
{
    [Author("Franco, Gustavo")]
    public class SingleIcon : IEnumerable<IconImage>
    {
        #region Variables Declaration
        private string          mName       = "";
        private List<IconImage> mIconImages = new List<IconImage>();
        #endregion

        #region Constructors
        internal SingleIcon(string name)
        {
            mName       = name;
        }
        #endregion

        #region Properties
        public int Count
        {
            get {return mIconImages.Count;}
        }

        public string Name
        {
            get {return mName;}
            set {mName = value != null ? value : string.Empty;}
        }

        public Icon Icon
        {
            get
            {
                if (mIconImages.Count == 0)
                    return null;

                MemoryStream ms = new MemoryStream();
                Save(ms);
                ms.Position = 0;
                Icon icon = new Icon(ms);
                return icon;
            }
        }
        #endregion

        #region Public Methods
        public void Clear()
        {
            mIconImages.Clear();
        }

        public void RemoveAt(int index)
        {
            if (index<0 || index>=mIconImages.Count)
                return;

            mIconImages.RemoveAt(index);
        }

        public IEnumerator<IconImage> GetEnumerator()
        {
            return new SingleIcon.Enumerator(this);
        }

        public void Load(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            try
            {
                Load(fs);
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
        }

        public void Load(Stream stream)
        {
            IconFormat iconFormat = new IconFormat();
            if (!iconFormat.IsRecognizedFormat(stream))
                throw new InvalidFileException();

            MultiIcon multiIcon = iconFormat.Load(stream);
            if (multiIcon.Count < 1)
                return;

            this.CopyFrom(multiIcon[0]);
        }

        public void Save(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
            try
            {
                Save(fs);
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
        }

        public void Save(Stream stream)
        {
            new IconFormat().Save(new MultiIcon(this), stream);
        }
        
        public IconImage Add(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            return Add(bitmap, null, bitmap.GetPixel(0,0));
        }

        public IconImage Add(Bitmap bitmap, Color transparentColor)
        {
            return Add(bitmap, null, transparentColor);
        }

        public IconImage Add(Bitmap bitmap, Bitmap bitmapMask)
        {
            if (bitmapMask == null)
                throw new ArgumentNullException("bitmapMask");

            return Add(bitmap, bitmapMask, Color.Empty);
        }

        public IconImage Add(Icon icon)
        {
            if (icon == null)
                throw new ArgumentNullException("icon");

            ICONINFO iconInfo;
            bool bResult = Win32.GetIconInfo(icon.Handle, out iconInfo);

            if (!bResult)
                throw new InvalidMultiIconFileException();

            Bitmap XORImage = null;
            Bitmap ANDImage = null;
            try
            {
                XORImage = Bitmap.FromHbitmap(iconInfo.hbmColor);
                ANDImage = Bitmap.FromHbitmap(iconInfo.hbmMask);

                // Bitmap.FromHbitmap will give a DDB and not a DIB, if the screen is 16 bits Icon with 16bits are not supported
                // then make them XP format Icons
				if (Tools.BitsFromPixelFormat (XORImage.PixelFormat) == 16)
				{
					XORImage.Dispose ();
					ANDImage.Dispose ();

					return Add (icon.ToBitmap (), Color.Transparent);
				}
				else
				{
					return Add (XORImage, ANDImage, Color.Empty);
				}
            }
            finally
            {
                if (XORImage != null)
                    XORImage.Dispose();
                if (ANDImage != null)
                    ANDImage.Dispose();
            }
        }

        internal void Add(IconImage iconImage)
        {
            mIconImages.Add(iconImage);
        }

        internal void CopyFrom(SingleIcon singleIcon)
        {
            mName       = singleIcon.mName;
            mIconImages = singleIcon.mIconImages;
        }
        #endregion

        #region Private Methods
        private unsafe IconImage Add(Bitmap bitmap, Bitmap bitmapMask, Color transparentColor)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            if (IndexOf(bitmap.Size, Tools.BitsFromPixelFormat(bitmap.PixelFormat)) != -1)
                throw new ImageAlreadyExistsException();

            if (bitmap.Width > 256 || bitmap.Height > 256)
                throw new ImageToBigException();

            IconImage iconImage = new IconImage();
            iconImage.Set(bitmap, bitmapMask, transparentColor);
            mIconImages.Add(iconImage);
			return iconImage;
        }

        private int IndexOf(Size size, int bitCount)
        {
            for(int i=0; i<Count; i++)
                if (this[i].Size == size && Tools.BitsFromPixelFormat(this[i].PixelFormat) == bitCount)
                    return i;
            return -1;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return this.Name;
        }
        #endregion

        #region Indexers
        public IconImage this[int index]
        {
            get {return mIconImages[index];}
        }
        #endregion

        #region Helper Classes
        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct Enumerator : IEnumerator<IconImage>, IDisposable, IEnumerator
        {
            #region Variables Declaration
            private SingleIcon  mList;
            private int         mIndex;
            private IconImage   mCurrent;
            #endregion

            #region Constructors
            internal Enumerator(SingleIcon list)
            {
                mList   = list;
                mIndex  = 0;
                mCurrent= null;;
            }
            #endregion

            #region Properties
            public IconImage Current
            {
                get {return mCurrent;}
            }
            #endregion

            #region Methods
            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (mIndex < mList.Count)
                {
                    mCurrent = mList[mIndex];
                    mIndex++;
                    return true;
                }
                mIndex      = mList.Count + 1;
                mCurrent    = null;
                return false;
            }

            object IEnumerator.Current
            {
                get {return Current;}
            }

            void IEnumerator.Reset()
            {
                mIndex      = 0;
                mCurrent    = null;
            }
            #endregion
        }
        #endregion
    }
}
