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

        BinaryObjectReader reader = new(filename, Endianness.Little, Encoding.UTF8);

        if(filename.EndsWith(".heightfield"))
        {
            HeightField hf = new();
            hf.Read(reader);

            Bitmap image = new(width: (int)hf.ImageSize.X, height: (int)hf.ImageSize.Y);

            Graphics graphics = Graphics.FromImage(image);

            int index = 0;

            foreach (uint i in hf.Pixels)
            {
                Pen pen = new Pen(Color.FromArgb(255, checked((int)i), checked((int)i), checked((int)i)));
                Rectangle rectangle = new Rectangle(index - (int)hf.ImageSize.X * (index / (int)hf.ImageSize.X), (index / (int)hf.ImageSize.X), 1, 1);
                graphics.DrawRectangle(pen, rectangle);
                index++;
            }

            string filePath = filename + ".jpg";
            image.Save(filePath);

            graphics.Dispose();
            image.Dispose();
        }
    }
}