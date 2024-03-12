using ImageExtentionConverter.Models;
using ImageProcessor;
using ImageProcessorCore.Plugins.WebP.Formats;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using System.Diagnostics;

namespace ImageExtentionConverter.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;

        public HomeController(ILogger<HomeController> logger, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment = null)
        {
            _logger = logger;
            _environment = environment;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(IFormFile image)
        {
            if (image == null) return View();
            if(image.Length<0) return View();


            string[] alloweImageType = new string[] { "image/jpeg", "image/png" };
            if(!alloweImageType.Contains(image.ContentType.ToLower())) return View();

            string imagesPath = Path.Combine(_environment.WebRootPath, "Images");
            string webPFileName = Path.GetFileNameWithoutExtension(image.FileName)+".webp";
            string normalImage = Path.Combine(imagesPath,image.FileName);
            string webPFilePath = Path.Combine(imagesPath,webPFileName);

            if(!Directory.Exists(imagesPath))
            {
                Directory.CreateDirectory(imagesPath);
            }
            //orignal file save 
            using (var normalFileStream = new FileStream(normalImage,FileMode.Create))
            {
               image.CopyTo(normalFileStream);
            }

            using (var webpImageFileStream = new FileStream(webPFilePath,FileMode.Create))
            {
                using(var imageFactory = new ImageFactory(preserveExifData:false))
                {
                    imageFactory.Load(image.OpenReadStream())
                                .Format( new WebPFormat())
                                .Quality(100)
                                .Save(webpImageFileStream);
                }
            }
            Images viewModel = new Images();
            viewModel.NormalImage = "/Images/" + image.FileName;
            viewModel.WebPImage = "/Images/" + webPFileName; 



                return View(viewModel);
        }






        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
