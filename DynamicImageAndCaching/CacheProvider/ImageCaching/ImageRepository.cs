// -----------------------------------------------------------------------
// <copyright file="ImageRepositoryHelper.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace CacheProvider.ImageCaching
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Mime;
    using System.Text;
    using System.Web;

    using CacheProvider.AspNetRuntimeCache;

    /// <summary>
    /// Saves files to disk
    /// Omitting interface
    /// </summary>
    public class ImageRepository
    {
        private ICacheProvider CacheProvider;
        private const string _ImageDictionaryCacheKey = "APPNAME_IMAGE_DICTIONARY";
        private IDictionary<ImageDescriptor, string> _ImageDictionaryCache;
        private const string _BaseFilePath = "App_Data/GeneratedImages";

        private static object _SyncLock = new object();

        private static ImageRepository _Instance;

        private ImageRepository(ICacheProvider cacheProvider)
        {
            this.CacheProvider = cacheProvider;

            _ImageDictionaryCache = new Dictionary<ImageDescriptor, string>(new ImageDescriptionSelector());
            this.LoadFilesToDictionary(_ImageDictionaryCache);
            cacheProvider.Get(_ImageDictionaryCacheKey, () =>
                {
                    return _ImageDictionaryCache;
                });
        }

        public static ImageRepository Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (_SyncLock)
                    {
                        if (_Instance == null)
                        {
                            _Instance = CreateInstance();
                        }
                    }
                }

                return _Instance;
            }
        }

        public string GetFilePath(ImageDescriptor imageDescriptor)
        {
            if (!_ImageDictionaryCache.ContainsKey(imageDescriptor))
            {
                lock (_SyncLock)
                {
                    if (!_ImageDictionaryCache.ContainsKey(imageDescriptor))
                    {
                        var temp = CacheProvider.Get<IDictionary<ImageDescriptor, string>>(_ImageDictionaryCacheKey, null);
                        var fileNameWithPath = GenerateFile(imageDescriptor);
                        temp.Add(imageDescriptor, fileNameWithPath);
                        _ImageDictionaryCache = temp;
                    }
                }
            }

            return _ImageDictionaryCache[imageDescriptor];
        }

        private static ImageRepository CreateInstance()
        {
            return new ImageRepository(new AspNetRuntimeCache());
        }

        private void LoadFilesToDictionary(IDictionary<ImageDescriptor, string> dictionary)
        {
            var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _BaseFilePath);
            foreach (var file in Directory.GetFiles(basePath))
            {
                var fileInfo = new FileInfo(file);
                dictionary.Add(new ImageDescriptor(Guid.Parse(fileInfo.Name)), fileInfo.FullName);
            }
        }

        private string GenerateFile(ImageDescriptor descriptor)
        {
            //context is resolved thru web/windows
            var resolvedFileBasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _BaseFilePath, 
                descriptor.ImageId.ToString().ToLowerInvariant());
            if (!File.Exists(resolvedFileBasePath))
            {
                lock (_SyncLock)
                {
                    if (!File.Exists(resolvedFileBasePath))
                    {
                        //do the image processing
                        using (var fs = File.Create(resolvedFileBasePath))
                        {
                            //do bunch of stuffs
                        }
                    }
                }                
            }

            return resolvedFileBasePath;
        }
    }

    internal class ImageDescriptionSelector : IEqualityComparer<ImageDescriptor>
    {
        public bool Equals(ImageDescriptor x, ImageDescriptor y)
        {
            return this.GetLower(x.ImageId) == this.GetLower(y.ImageId);
        }

        public int GetHashCode(ImageDescriptor obj)
        {
            unchecked
            {
                return 311 * GetLower(obj.ImageId).GetHashCode();
                //obj.Height.GetHashCode() ^
                //obj.Width.GetHashCode() ^
                //obj.Mode.GetHashCode();
            }
        }

        private string GetLower(Guid guid)
        {
            return guid.ToString().ToLowerInvariant();
        }
    }
}
