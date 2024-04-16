using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rg_chat_toolkit_test_harness
{

    public static class Resources_Extensions
    {
        public static byte[] ConvertBitmapToJpegBytes(Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Save the bitmap to the stream as JPEG
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

                // Convert the MemoryStream to byte array
                return ms.ToArray();
            }
        }

        public static byte[] L015_Bytes
        {
            get
            {
                return ConvertBitmapToJpegBytes(Resources.L015);
            }
        }

        public static byte[] L484_Bytes
        {
            get
            {
                return ConvertBitmapToJpegBytes(Resources.L484);
            }
        }
    }

}
