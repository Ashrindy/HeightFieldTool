using Amicitia.IO.Binary;
using System.Numerics;

namespace HeightFieldTool;

public class HeightField
{
    public string Signature { get; set; }
    public int Field08 { get; set; }
    public int Field0c { get; set; }
    public int Field10 { get; set; }
    public Vector2 ImageSize { get; set; }
    public float Field1c { get; set; }
    public float Field20 { get; set; }
    public float Field24 { get; set; }
    public float Field28 { get; set; }
    public UInt16[] Pixels { get; set; }
    public Int32 Field30 { get; set; }
    public Vector3[] Unk1 { get; set; }
    public byte[] Data { get; set; }

    public void Read(BinaryObjectReader reader)
    {
        Signature = reader.ReadString(StringBinaryFormat.FixedLength, 4);
        Field08 = reader.Read<int>();
        Field0c = reader.Read<int>();
        Field10 = reader.Read<int>();
        ImageSize = reader.Read<Vector2>();
        Field1c = reader.Read<float>();
        Field20 = reader.Read<float>();
        Field24 = reader.Read<float>();
        Field28 = reader.Read<float>();
        Pixels = reader.ReadArray<UInt16>(16777216);
        Field30 = reader.Read<int>();
        Unk1 = reader.ReadArray<Vector3>(11);
        Data = reader.ReadArray<byte>(16769024);
    }
}

