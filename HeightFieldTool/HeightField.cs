using Amicitia.IO.Binary;
using System.Numerics;

namespace HeightFieldTool;

public class HeightField
{
    public string Signature { get; set; }
    public int Field08 { get; set; }
    public int Field0c { get; set; }
    public int Field10 { get; set; }
    public Vector2Int ImageSize { get; set; }
    public float Field1c { get; set; }
    public float Field20 { get; set; }
    public float Field24 { get; set; }
    public float Field28 { get; set; }
    public UInt16[] Pixels { get; set; }
    public Int32 Count { get; set; }
    public Height[] Unk1 { get; set; }
    public byte[] Data { get; set; }

    public void Read(BinaryObjectReader reader)
    {
        Signature = reader.ReadString(StringBinaryFormat.FixedLength, 4);
        reader.Skip(4);
        Field08 = reader.Read<int>();
        Field0c = reader.Read<int>();
        Field10 = reader.Read<int>();
        ImageSize = reader.Read<Vector2Int>();
        Field1c = reader.Read<float>();
        Field20 = reader.Read<float>();
        Field24 = reader.Read<float>();
        Field28 = reader.Read<float>();
        Pixels = reader.ReadArray<UInt16>(ImageSize.X*ImageSize.Y);
        Count = reader.Read<int>();
        Unk1 = reader.ReadArray<Height>(Count);
        Data = reader.ReadArray<byte>(16769024);
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteString(StringBinaryFormat.FixedLength, Signature, 4);
        writer.Skip(4);
        writer.Write(Field08);
        writer.Write(Field0c);
        writer.Write(Field10);
        writer.Write(ImageSize);
        writer.Write(Field1c);
        writer.Write(Field20);
        writer.Write(Field24);
        writer.Write(Field28);
        writer.WriteArray(Pixels);
        writer.Write(Count);
        writer.WriteArray(Unk1);
        writer.WriteArray(Data);

        long size = writer.Position;

        writer.Seek(4, SeekOrigin.Begin);
        writer.Write((int)size);
    }
}

public struct Height
{
    public UInt16 Field00 { get; set; }
    public UInt16 Field04 { get; set; }
    public float Field08 { get; set; }
}

public struct Vector2Int
{
    public int X { get; set; }
    public int Y { get; set; }
}
