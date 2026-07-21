namespace OnlineStore.Controllers
{
    public class UploadHandler
    {
        public string imageHandler(IFormFile image)
        {
            if (image != null && image.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    image.CopyTo(stream);
                }

                return "/images/" + fileName;
            }

            return "/images/default.png";
        }
    }
}
