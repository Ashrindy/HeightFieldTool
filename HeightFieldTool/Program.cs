using Amicitia.IO.Binary;
using System.Text;
using System.Drawing;

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

            Bitmap image = new(width: (int)hf.ImageSize.X, height: (int)hf.ImageSize.Y);

            Graphics graphics = Graphics.FromImage(image);

            int index = 0;

            foreach (UInt16 i in hf.Pixels)
            {
                string temp = i.ToString();
                int tempI = Int32.Parse(temp);
                Pen pen = new Pen(Color.FromArgb(255, (int)(((float)tempI / 32768) * 255), (int)(((float)tempI / 32768) * 255), (int)(((float)tempI / 32768) * 255)));
                Rectangle rectangle = new Rectangle(index - (int)hf.ImageSize.X * (index / (int)hf.ImageSize.X), (index / (int)hf.ImageSize.X), 1, 1);
                graphics.DrawRectangle(pen, rectangle);
                index++;
            }

            string filePath = filename + ".jpg";
            image.Save(filePath);

            graphics.Dispose();
            image.Dispose();
        }
        else if(filename.EndsWith(".heightfield.jpg"))
        {
            BinaryObjectReader reader = new(filename.Replace(".jpg", ""), Endianness.Little, Encoding.UTF8);

            HeightField hf = new HeightField();
            hf.Read(reader);

            reader.Dispose();

            Console.WriteLine(hf.Signature);

            Bitmap image = new(filename);

            int index = 0;

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color clr = image.GetPixel(x, y);

                    string temp = ((int)(((float)clr.R / 255) * 32768)).ToString();
                    ushort tempI = UInt16.Parse(temp);

                    hf.Pixels[index] = tempI;

                    index++;
                }
            }

            image.Dispose();

            BinaryObjectWriter writer = new(filename.Replace(".jpg", ""), Endianness.Little, Encoding.UTF8);

            hf.Write(writer);
        }
    }
}