using System.Threading.Tasks;
using UnityEngine;

namespace UnityCommon
{
    /// <summary>
    /// Converts <see cref="T:byte[]"/> raw data of a .png or .jpg image to <see cref="Texture2D"/>.
    /// </summary>
    public class JpgOrPngToTextureConverter : IRawConverter<Texture2D>
    {
        public RawDataRepresentation[] Representations { get { return new RawDataRepresentation[] {
            new RawDataRepresentation(".png", "image/png"),
            new RawDataRepresentation(".jpg", "image/jpeg")
        }; } }

        public Texture2D Convert (byte[] obj)
        {
            var texture = new Texture2D(2, 2);
            texture.LoadImage(obj, true);
            return texture;
        }

        public Task<Texture2D> ConvertAsync (byte[] obj)
        {
            var texture = Convert(obj);
            return Task.FromResult(texture);
        }

        public object Convert (object obj) => Convert(obj as byte[]);

        public async Task<object> ConvertAsync (object obj) => await ConvertAsync(obj as byte[]);
    }
}
