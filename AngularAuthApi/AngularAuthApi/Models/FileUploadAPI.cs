namespace AngularAuthApi.Models
{
    public class FileUploadAPI
    {
        public int ImgID { get; set; }
        public string? Name { get; set; }
        public IFormFile? files { get; set; }
        public string ImgName { get; set; }
    }
}
