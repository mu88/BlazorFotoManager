using FotoManagerLogic.API;
using Microsoft.AspNetCore.Mvc;

namespace FotoManager.API
{
    [Route("api/images")]
    public class ImageController : Controller
    {
        public ImageController(IServerImageRepository serverImageRepository)
        {
            ServerImageRepository = serverImageRepository;
        }

        private IServerImageRepository ServerImageRepository { get; }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            var image = System.IO.File.OpenRead(ServerImageRepository.GetPath(id));
            return File(image, "image/jpeg");
        }

        [HttpPost]
        public void Post([FromBody] ServerImage entry)
        {
            ServerImageRepository.Add(entry);
        }
    }
}