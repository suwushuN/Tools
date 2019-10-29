using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEst
{
    class ReadPinYin
    {
        public static void ReadFiles()
        {
            //中文字
            var resourceData = File.ReadAllBytes(@"E:\Code\GitCode\Tools\Tools\PinYinConverter\TEst\files\CharDictionary");
            //using (BinaryReader binaryReader = new BinaryReader((Stream)new MemoryStream(resourceData)))
            //{
            //  var res=  CharDictionary.Deserialize(binaryReader);
            //}

            // resourceData = File.ReadAllBytes(@"E:\Code\GitCode\Tools\Tools\PinYinConverter\TEst\files\HomophoneDictionary");
            //using (BinaryReader binaryReader = new BinaryReader((Stream)new MemoryStream(resourceData)))
            //{
            //    var res = HomophoneDictionary.Deserialize(binaryReader);
            //}
            resourceData = File.ReadAllBytes(@"E:\Code\GitCode\Tools\Tools\PinYinConverter\TEst\files\PinyinDictionary");
            using (BinaryReader binaryReader = new BinaryReader((Stream)new MemoryStream(resourceData)))
            {
                var res = PinyinDictionary.Deserialize(binaryReader);
            }
        }
    }

    internal class CharDictionary
    {
        internal readonly byte[] Reserved = new byte[24];
        internal readonly short EndMark = short.MaxValue;
        internal int Length;
        internal int Count;
        internal short Offset;
        internal List<CharUnit> CharUnitTable;

        internal void Serialize(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(this.Length);
            binaryWriter.Write(this.Count);
            binaryWriter.Write(this.Offset);
            binaryWriter.Write(this.Reserved);
            for (int index = 0; index < this.Count; ++index)
                this.CharUnitTable[index].Serialize(binaryWriter);
            binaryWriter.Write(this.EndMark);
        }

        internal static CharDictionary Deserialize(BinaryReader binaryReader)
        {
            StringBuilder sb = new StringBuilder();
            CharDictionary charDictionary = new CharDictionary();
           // binaryReader.ReadInt32();
            charDictionary.Length = binaryReader.ReadInt32();
            charDictionary.Count = binaryReader.ReadInt32();
            charDictionary.Offset = binaryReader.ReadInt16();
            binaryReader.ReadBytes(24);
            charDictionary.CharUnitTable = new List<CharUnit>();
            for (int index = 0; index < charDictionary.Count; ++index)
                charDictionary.CharUnitTable.Add(CharUnit.Deserialize(binaryReader,sb));
            int num = (int)binaryReader.ReadInt16();
            File.WriteAllText("charDict.txt",sb.ToString(), Encoding.UTF8);
            return charDictionary;
        }

        //internal CharUnit GetCharUnit(int index)
        //{
        //    if (index < 0 || index >= this.Count)
        //        throw new ArgumentOutOfRangeException(nameof(index), AssemblyResource.INDEX_OUT_OF_RANGE);
        //    return this.CharUnitTable[index];
        //}

        //internal CharUnit GetCharUnit(char ch)
        //{
        //    return this.CharUnitTable.Find(new Predicate<CharUnit>(new CharUnitPredicate(ch).Match));
        //}
    }

    internal class CharUnit
    {
        internal char Char;
        internal byte StrokeNumber;
        internal byte PinyinCount;
        internal short[] PinyinIndexList;

        internal static CharUnit Deserialize(BinaryReader binaryReader,StringBuilder sb)
        {
          
            CharUnit charUnit = new CharUnit()
            {
                Char = binaryReader.ReadChar(),
                StrokeNumber = binaryReader.ReadByte(),
                PinyinCount = binaryReader.ReadByte()
            };
            sb.Append(string.Format("{0},{1},{2},", charUnit.Char, charUnit.StrokeNumber, charUnit.PinyinCount));
            charUnit.PinyinIndexList = new short[(int)charUnit.PinyinCount];
            for (int index = 0; index < (int)charUnit.PinyinCount; ++index)
            {
                charUnit.PinyinIndexList[index] = binaryReader.ReadInt16();
                sb.Append(charUnit.PinyinIndexList[index]).Append(",");
            }
            sb.Append(System.Environment.NewLine);
            return charUnit;
        }

        internal void Serialize(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(this.Char);
            binaryWriter.Write(this.StrokeNumber);
            binaryWriter.Write(this.PinyinCount);
            for (int index = 0; index < (int)this.PinyinCount; ++index)
                binaryWriter.Write(this.PinyinIndexList[index]);
        }
    }

    internal class HomophoneUnit
    {
        internal short Count;
        internal char[] HomophoneList;

        internal static HomophoneUnit Deserialize(BinaryReader binaryReader, StringBuilder sb)
        {
            HomophoneUnit homophoneUnit = new HomophoneUnit()
            {
                Count = binaryReader.ReadInt16()
            };
            sb.Append(string.Format("{0},", homophoneUnit.Count));
            homophoneUnit.HomophoneList = new char[(int)homophoneUnit.Count];
            for (int index = 0; index < (int)homophoneUnit.Count; ++index)
            {
                homophoneUnit.HomophoneList[index] = binaryReader.ReadChar();
                sb.Append(homophoneUnit.HomophoneList[index]).Append(",");
            }
            sb.Append(System.Environment.NewLine);
            return homophoneUnit;
        }

        internal void Serialize(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(this.Count);
            for (int index = 0; index < (int)this.Count; ++index)
                binaryWriter.Write(this.HomophoneList[index]);
        }
    }
    internal class HomophoneDictionary
    {
        internal readonly byte[] Reserved = new byte[8];
        internal readonly short EndMark = short.MaxValue;
        internal int Length;
        internal short Offset;
        internal short Count;
        internal List<HomophoneUnit> HomophoneUnitTable;

        internal void Serialize(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(this.Length);
            binaryWriter.Write(this.Count);
            binaryWriter.Write(this.Offset);
            binaryWriter.Write(this.Reserved);
            for (int index = 0; index < (int)this.Count; ++index)
                this.HomophoneUnitTable[index].Serialize(binaryWriter);
            binaryWriter.Write(this.EndMark);
        }

        internal static HomophoneDictionary Deserialize(BinaryReader binaryReader)
        {
            HomophoneDictionary homophoneDictionary = new HomophoneDictionary();
          //  binaryReader.ReadInt32();
            homophoneDictionary.Length = binaryReader.ReadInt32();
            homophoneDictionary.Count = binaryReader.ReadInt16();
            homophoneDictionary.Offset = binaryReader.ReadInt16();
            binaryReader.ReadBytes(8);
            homophoneDictionary.HomophoneUnitTable = new List<HomophoneUnit>();
            StringBuilder sb = new StringBuilder();
            for (int index = 0; index < (int)homophoneDictionary.Count; ++index)
                homophoneDictionary.HomophoneUnitTable.Add(HomophoneUnit.Deserialize(binaryReader,sb));
            int num = (int)binaryReader.ReadInt16();
            File.WriteAllText("homophoneDictionary.txt", sb.ToString(), Encoding.UTF8);
            return homophoneDictionary;
        }

        //internal HomophoneUnit GetHomophoneUnit(int index)
        //{
        //    if (index < 0 || index >= (int)this.Count)
        //        throw new ArgumentOutOfRangeException(nameof(index), AssemblyResource.INDEX_OUT_OF_RANGE);
        //    return this.HomophoneUnitTable[index];
        //}

        //internal HomophoneUnit GetHomophoneUnit(
        //  PinyinDictionary pinyinDictionary,
        //  string pinyin)
        //{
        //    return this.GetHomophoneUnit(pinyinDictionary.GetPinYinUnitIndex(pinyin));
        //}
    }


    internal class PinyinUnit
    {
        internal string Pinyin;

        internal static PinyinUnit Deserialize(BinaryReader binaryReader,StringBuilder sb )
        {
            PinyinUnit pinyinUnit = new PinyinUnit();
            byte[] bytes = binaryReader.ReadBytes(7);
            pinyinUnit.Pinyin = Encoding.ASCII.GetString(bytes, 0, 7);
            char[] chArray = new char[1];
            pinyinUnit.Pinyin = pinyinUnit.Pinyin.TrimEnd(chArray);

            sb.AppendLine(string.Format("{0}", pinyinUnit.Pinyin));
            return pinyinUnit;
        }

        internal void Serialize(BinaryWriter binaryWriter)
        {
            byte[] numArray = new byte[7];
            Encoding.ASCII.GetBytes(this.Pinyin, 0, this.Pinyin.Length, numArray, 0);
            binaryWriter.Write(numArray);
        }
    }

    internal class PinyinDictionary
    {
        internal readonly byte[] Reserved = new byte[8];
        internal readonly short EndMark = short.MaxValue;
        internal short Length;
        internal short Count;
        internal short Offset;
        internal List<PinyinUnit> PinyinUnitTable;

        internal void Serialize(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(this.Length);
            binaryWriter.Write(this.Count);
            binaryWriter.Write(this.Offset);
            binaryWriter.Write(this.Reserved);
            for (int index = 0; index < (int)this.Count; ++index)
                this.PinyinUnitTable[index].Serialize(binaryWriter);
            binaryWriter.Write(this.EndMark);
        }

        internal static PinyinDictionary Deserialize(BinaryReader binaryReader)
        {//文件18个字节后的为正常可解析部分【按7个长度依次类推】  binaryReader.ReadBytes(18);
            PinyinDictionary pinyinDictionary = new PinyinDictionary();
          //var gg=  binaryReader.ReadBytes(14);
            pinyinDictionary.Length = binaryReader.ReadInt16();
            pinyinDictionary.Count = binaryReader.ReadInt16();
            pinyinDictionary.Offset = binaryReader.ReadInt16();
            binaryReader.ReadBytes(8);
            pinyinDictionary.PinyinUnitTable = new List<PinyinUnit>();

            StringBuilder sb = new StringBuilder();
            for (int index = 0; index < (int)pinyinDictionary.Count; ++index)
                pinyinDictionary.PinyinUnitTable.Add(PinyinUnit.Deserialize(binaryReader,sb));
            int num = (int)binaryReader.ReadInt16();
            File.WriteAllText("PinyinDictionary.txt", sb.ToString(), Encoding.UTF8);
            return pinyinDictionary;
        }

        //internal int GetPinYinUnitIndex(string pinyin)
        //{
        //    return this.PinyinUnitTable.FindIndex(new Predicate<PinyinUnit>(new PinyinUnitPredicate(pinyin).Match));
        //}

        //internal PinyinUnit GetPinYinUnit(string pinyin)
        //{
        //    return this.PinyinUnitTable.Find(new Predicate<PinyinUnit>(new PinyinUnitPredicate(pinyin).Match));
        //}

        //internal PinyinUnit GetPinYinUnitByIndex(int index)
        //{
        //    if (index < 0 || index >= (int)this.Count)
        //        throw new ArgumentOutOfRangeException(nameof(index), AssemblyResource.INDEX_OUT_OF_RANGE);
        //    return this.PinyinUnitTable[index];
        //}
    }
}
