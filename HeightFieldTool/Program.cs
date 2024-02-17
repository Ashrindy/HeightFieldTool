using Amicitia.IO.Binary;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Tiff;

using Color = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace HeightFieldTool;

internal class Program
{
    static void Main(string[] args)
    {
        string filename;

        if(args.Length > 0) 
        {
            filename = args[0];
        }
        else
        {
            Console.WriteLine("What's the file?");
            filename = Console.ReadLine();
        }

        if(filename.EndsWith(".heightfield"))
        {
            BinaryObjectReader reader = new(filename, Endianness.Little, Encoding.UTF8);

            HeightField hf = new();
            hf.Read(reader);

            using (Image<RgbaVector> img = new(hf.ImageSize.X, hf.ImageSize.Y))
            {
                int ind = 0;

                for(int y = 0; y < hf.ImageSize.Y; y++)
                {
                    for (int x = 0; x < hf.ImageSize.X; x++)
                    {
                        string temp = hf.Pixels[ind].ToString();
                        float tempI = float.Parse(temp);

                        img[x, y] = new((tempI / 32768), (tempI / 32768), (tempI / 32768));

                        ind++;
                    }
                }

                img.SaveAsTiff(filename + ".tiff");
            }

            Bitmap image = new(width: (int)hf.ImageSize.X, height: (int)hf.ImageSize.Y, PixelFormat.Format32bppRgb);

            Graphics graphics = Graphics.FromImage(image);

            string filePath = filename + ".jpg";

            image = new(width: (int)hf.ImageSize.X, height: (int)hf.ImageSize.Y);

            graphics = Graphics.FromImage(image);

            int index = 0;

            foreach (UInt16 i in hf.Data)
            {
                string temp = i.ToString();
                int tempI = Int32.Parse(temp);
                Pen pen = new Pen(Color.FromArgb(255, (int)(((float)tempI / hf.Count) * 255), (int)(((float)tempI / hf.Count) * 255), (int)(((float)tempI / hf.Count) * 255)));
                Rectangle rectangle = new Rectangle(index - (int)hf.ImageSize.X * (index / (int)hf.ImageSize.X) + (index / (int)hf.ImageSize.X), (index / (int)hf.ImageSize.X), 1, 1);
                graphics.DrawRectangle(pen, rectangle);
                index++;
            }

            index = 0;

            foreach (UInt16 i in hf.Data)
            {
                string temp = i.ToString();
                int tempI = Int32.Parse(temp);
                Pen pen = new Pen(Color.FromArgb(255, (int)(((float)tempI / hf.Count) * 255), (int)(((float)tempI / hf.Count) * 255), (int)(((float)tempI / hf.Count) * 255)));
                Rectangle rectangle = new Rectangle(index - (int)hf.ImageSize.X * (index / (int)hf.ImageSize.X) + (index / (int)hf.ImageSize.X) - hf.ImageSize.X, (index / (int)hf.ImageSize.X), 1, 1);
                graphics.DrawRectangle(pen, rectangle);
                index++;
            }

            filePath = filename + "_mat.jpg";
            image.Save(filePath);

            graphics.Dispose();
            image.Dispose();
        }
        else if(filename.EndsWith(".heightfield.tiff"))
        {
            BinaryObjectReader reader = new(filename.Replace(".tiff", ""), Endianness.Little, Encoding.UTF8);

            HeightField hf = new HeightField();
            hf.Read(reader);

            reader.Dispose();

            Image<RgbaVector> img = SixLabors.ImageSharp.Image.Load<RgbaVector>(filename);

            int index = 0;

            img.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < img.Height; y++)
                {
                    Span<RgbaVector> pixelRow = accessor.GetRowSpan(y);

                for (int x = 0; x < img.Width; x++)
                    {
                        ref RgbaVector pixel = ref pixelRow[x];

                        if(pixel.R != 1)
                        {
                            Console.WriteLine(pixel.R);
                            Console.WriteLine(pixel.R * 32768);
                        }

                        string temp = ((int)(pixel.R * 32768)).ToString();
                        ushort tempI = UInt16.Parse(temp);

                        hf.Pixels[index] = tempI;

                        index++;
                    }
                }
            });

            BinaryObjectWriter writer = new(filename.Replace(".tiff", ""), Endianness.Little, Encoding.UTF8);

            hf.Write(writer);
        }
        else if (filename.EndsWith(".heightfield_mat.jpg"))
        {
            BinaryObjectReader reader = new(filename.Replace(".jpg", ""), Endianness.Little, Encoding.UTF8);

            HeightField hf = new HeightField();
            hf.Read(reader);

            reader.Dispose();

            Bitmap image = new(filename);

            int index = 0;

            for (int y = 0; y < image.Height - image.Height * 2; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color clr = image.GetPixel(x, y);

                    byte temp = ((byte)(((float)clr.R / 255) * hf.Count));

                    hf.Data[index] = temp;

                    index++;
                }
            }

            image.Dispose();

            BinaryObjectWriter writer = new(filename.Replace("_mat.jpg", ""), Endianness.Little, Encoding.UTF8);

            hf.Write(writer);
        }
    }
}