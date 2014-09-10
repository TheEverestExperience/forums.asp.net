// -----------------------------------------------------------------------
// <copyright file="ImageCacheDescriptor.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace CacheProvider.ImageCaching
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Provides image cache key for dictionary
    /// </summary>
    public class ImageDescriptor
    {
        public Guid ImageId { get; private set; }

        //public uint Width { get; private set; }
        //public uint Height { get; private set; }
        //public ImageMode Mode { get; private set; }       

        public ImageDescriptor
            (
            Guid imageId
            //uint height,
            //uint width,
            //ImageMode imageMode
            )
        {
            this.ImageId = imageId;
            //this.Height = height;
            //this.Width = width;
            //this.Mode = imageMode;
        }
    }

    /// <summary>
    /// Better to use enums than constants
    /// </summary>
    public enum ImageMode
    {
        Fill = 0x01, //safe from bitwise OR-ing
        Transparent = 0x02
        //TODO::
    }
}
