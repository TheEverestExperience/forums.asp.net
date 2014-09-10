using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImageCaching.Controllers
{
    using CacheProvider.ImageCaching;

    public class HomeController : Controller
    {
        private ImageRepository ImageRepository = ImageRepository.Instance;       

        /// <summary>
        /// http://forums.asp.net/t/2007059.aspx?multiple+users+access+dynamic+generated+image+in+mvc+action
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="imageMode"></param>
        /// <returns></returns>
        public ActionResult GetImage(
            /*deserves view model*/
            Guid? guid,
            uint width = 100,
            uint height = 200, 
            ImageMode imageMode = ImageMode.Fill)
        {
            if(guid == null)
            {
                guid = Guid.NewGuid();
            }

            var fileUrl =  ImageRepository.GetFilePath(new ImageDescriptor(guid.Value));
            return new FilePathResult(fileUrl, "html/text");//change mime type to image
        }        
    }
}
