using Microsoft.AspNetCore.Http;
using System;

namespace PhotoWebAPI.Models
{
    public class Photo
    {
        public int IdPerson { get; set; }
        public int Id  { get; set; }
        public byte[] ImageData { get; set; }
        
        public Photo()
        {  }

        public Photo(int id, int idPerson, IFormFile file)
        {
            Id = id;
            IdPerson = idPerson;
            ImageData = ReadBytesOfFile(file);
        }        

        public static byte[] ReadBytesOfFile (IFormFile file)
        {
            var bytes = new byte[file.Length];
            file.OpenReadStream().ReadAsync(bytes);
            return bytes;
        }

        public static Tuple<bool, string> IsFileOK (IFormFile file)
        {            
            if (file == null)
                return Tuple.Create(false, "NoFile");
            if (file.ContentType != "image/jpeg")
                return Tuple.Create(false, "A file must be jpeg-format");
            if (file.Length > 6150)
                return Tuple.Create(false, "File is too large");
            return Tuple.Create( true, "IOK");
        }
    }
}
