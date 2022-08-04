using USETHISAddressBookMVC.Services.Interfaces;

namespace USETHISAddressBookMVC.Services
{
    public class ImageService : IImageService
    {

        #region Globals
        private readonly string[] suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };
        private readonly string defaultImageSrc = "img/ContactImages/DefaultContactImage.png";
        #endregion

        public string? ConvertByteArrayToFile(byte[] fileData, string? extension)
        {
            if (fileData == null)
            {
                return defaultImageSrc;
            }

            try
            {
                string? imageBase64Data = Convert.ToBase64String(fileData);
                return string.Format($"data:{extension};base64, {imageBase64Data}");
                                                   //^^^^^ Interpolated code
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
        {
            try
            {
                using MemoryStream memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                byte[] byteArray = memoryStream.ToArray();
                return byteArray;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
