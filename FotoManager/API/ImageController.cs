using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using FotoManagerLogic.API;
using Microsoft.AspNetCore.Mvc;

namespace FotoManager.API;

[Route("api/images")]
public class ImageController : Controller
{
    private const int ExifOrientationId = 0x112; //274

    public ImageController(IServerImageRepository serverImageRepository)
    {
        ServerImageRepository = serverImageRepository;
    }

    private IServerImageRepository ServerImageRepository { get; }

    [HttpGet("{id}")]
    public IActionResult Get(string id)
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException();
        }
                
        var originalImage = Image.FromFile(ServerImageRepository.GetPath(id));
        ExifRotate(originalImage);
        var newImage = new Bitmap(originalImage);

        var memoryStream = new MemoryStream();
        newImage.Save(memoryStream, ImageFormat.Jpeg);
        return File(memoryStream.GetBuffer(), "image/jpeg");
    }

    [HttpPost]
    public void Post([FromBody] ServerImage entry)
    {
        ServerImageRepository.Add(entry);
    }

    private void ExifRotate(Image image)
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException();
        }
            
        if (!image.PropertyIdList.Contains(ExifOrientationId))
        {
            return;
        }

        var exifProperty = image.GetPropertyItem(ExifOrientationId);
        int exifValue = BitConverter.ToUInt16(exifProperty?.Value ?? throw new NullReferenceException(), 0);

        var rotation = exifValue switch
        {
            3 => RotateFlipType.Rotate180FlipNone,
            4 => RotateFlipType.Rotate180FlipNone,
            5 => RotateFlipType.Rotate90FlipNone,
            6 => RotateFlipType.Rotate90FlipNone,
            7 => RotateFlipType.Rotate270FlipNone,
            8 => RotateFlipType.Rotate270FlipNone,
            _ => RotateFlipType.RotateNoneFlipNone
        };

        if (rotation != RotateFlipType.RotateNoneFlipNone)
        {
            image.RotateFlip(rotation);
        }
    }
}