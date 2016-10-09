using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.IO;

namespace BankSystem
{
    public class Picture
    {
        const double maxHeight = 400;
        const double maxWith = 600;
        public byte[] Image { get; set; }

        public string HtmlRaw { get; set; }

        public Picture(byte[] image)
        {
            Image = image;
            GetHtmlRaw();
        }

        private void GetHtmlRaw()
        {
            Image im = byteArrayToImage();
            this.HtmlRaw = "";
            if (im != null)
            {
                double k = maxHeight / im.Height;
                double height = im.Height;
                double with = im.Width;
                height *= k;
                with *= k;
                if (with > maxWith)
                {
                    k = maxWith / with;
                    with *= k;
                    height *= k;
                }
                this.HtmlRaw = String.Format("<img style='width:{0}px; height:{1}px;' src=\"data:image/jpeg;base64,", with.ToString(), height.ToString());
            }

        }

        private Image byteArrayToImage()
        {
            if (this.Image != null)
            {
                MemoryStream ms = new MemoryStream(this.Image);
                Image returnImage = System.Drawing.Image.FromStream(ms);
                return returnImage;
            }
            return null;
        }
    }
}