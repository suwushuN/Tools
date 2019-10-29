using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HanZiPinYin
{
    public class ChineseCharToPinYinHelper
    {
        public static string CombineSign = "|";
        public static Dictionary<char, HanZi> HanziDict;
        public static Dictionary<int, string> PinYinDict;

        static ChineseCharToPinYinHelper()
        {
            HanziDict = new Dictionary<char, HanZi>();
            PinYinDict = new Dictionary<int, string>();
            InitHanzi2();
            InitPinYin2();
        }
               

        /// <summary>
        /// 获取全拼 分隔符使用英文的【|】 
        /// </summary>
        /// <returns></returns>
        public static string GetFullSpell(string ChineseCharacter)
        {
            var resFull = new List<List<string>>();

            //获取每个汉字的全拼和首字母列表（一个字一行）
            foreach (var item in ChineseCharacter)
            {
                HanZi hz = null;
                if(HanziDict.TryGetValue(item,out hz))
                {
                    var temp = hz.PinyinIndexList.Select(c => PinYinDict[c]).ToList();//取拼音
                    resFull.Add(temp);
                }
                else
                {
                    resFull.Add(new List<string> { item.ToString().ToUpper() });
                }
              }

            var fullList = CombineResult(resFull);
            var fullStr = "";
            fullList.ForEach(c => fullStr = fullStr + c+ CombineSign);
            return fullStr.TrimEnd(CombineSign[0]);
        }

        /// <summary>
        /// 获取首字母
        /// </summary>
        /// <param name="ChineseCharacter">字符串</param>
        /// <returns></returns>
        public static string GetFullLetterSpell(string ChineseCharacter)
        {
            var resFull = new List<List<string>>();

            //获取每个汉字的全拼和首字母列表（一个字一行）
            foreach (var item in ChineseCharacter)
            {
                HanZi hz = null;
                if (HanziDict.TryGetValue(item, out hz))
                {
                    var temp = hz.PinyinIndexList.Select(c => PinYinDict[c][0].ToString()).ToList();//取首字母
                    resFull.Add(temp);
                }
                else
                {
                    resFull.Add(new List<string> { item.ToString().ToUpper() });
                }
            }

            var fullList = CombineResult(resFull);
            var fullStr = "";
            fullList.ForEach(c => fullStr = fullStr + c + CombineSign);
            return fullStr.TrimEnd(CombineSign[0]);
        }

        private static List<string> CombineResult(List<List<string>> matrix)
        {
            if (matrix == null || matrix.Count == 0)
            {
                return new List<string>();
            }
            var curList = new List<string>(matrix[0]);//缓存第一行
            int flag = 1;
            while (flag < matrix.Count)
            {
                var tempLi = new List<string>();
                curList.ForEach(c => matrix[flag].ForEach(next => tempLi.Add(c +  next)));//当前行和下一行进行拼接
                curList = new List<string>();//清空
                curList.AddRange(tempLi);
                flag++;
            };
            return curList;
        }
        /// <summary>
        /// 初始化汉字字典
        /// </summary>
        private static void InitHanzi2()
        {
            try
            {

                Assembly assembly = typeof(ChineseCharToPinYinHelper).Assembly;
                using (BinaryReader binaryReader = new BinaryReader(assembly.GetManifestResourceStream("HanZiPinYin.Files.SmallHanZiDictionary")))
                {
                    // binaryReader.ReadInt32();
                    var count = binaryReader.ReadInt32();
                    for (int index = 0; index < count; ++index)
                    {
                        var hanzi = new HanZi()
                        {
                            Char = binaryReader.ReadChar(),
                            PinyinCount = binaryReader.ReadByte()
                        };
                        hanzi.PinyinIndexList = new short[(int)hanzi.PinyinCount];
                        for (int i = 0; i < (int)hanzi.PinyinCount; ++i)
                        {
                            hanzi.PinyinIndexList[i] = binaryReader.ReadInt16();
                        }
                        HanziDict.Add(hanzi.Char, hanzi);
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }


        /// <summary>
        /// 初始化拼音字典
        /// </summary>
        private static void InitPinYin2()
        {
            try
            {
                Assembly assembly = typeof(ChineseCharToPinYinHelper).Assembly;

                using (BinaryReader binaryReader = new BinaryReader(assembly.GetManifestResourceStream("HanZiPinYin.Files.SmallPinyinDictionary")))
                {
                    // binaryReader.ReadInt32();
                    var count = binaryReader.ReadInt32();
                    for (int index = 0; index < count; ++index)
                    {
                        byte[] bytes = binaryReader.ReadBytes(7);
                        var py = Encoding.ASCII.GetString(bytes, 0, 7);
                        char[] chArray = new char[1];
                        py = py.TrimEnd(chArray);
                        PinYinDict.Add(index, py);
                    }
                }
            }
           catch(Exception ex)
            {

            }
        }


        #region 
        /// <summary>
        /// 使用HanZiDictionary原始文件进行创建字典HanziDict
        /// </summary>
        private static void InitHanzi()
        {
            try
            {

                Assembly assembly = typeof(ChineseCharToPinYinHelper).Assembly;
                using (BinaryReader binaryReader = new BinaryReader(assembly.GetManifestResourceStream("HanZiPinYin.Files.HanZiDictionary")))
                {
                    // binaryReader.ReadInt32();
                    binaryReader.ReadInt32();
                    var count = binaryReader.ReadInt32();
                    binaryReader.ReadInt16();
                    binaryReader.ReadBytes(24);
                    for (int index = 0; index < count; ++index)
                    {
                        var hanzi = new HanZi()
                        {
                            Char = binaryReader.ReadChar(),
                            StrokeNumber = binaryReader.ReadByte(),
                            PinyinCount = binaryReader.ReadByte()
                        };
                        hanzi.PinyinIndexList = new short[(int)hanzi.PinyinCount];
                        for (int i = 0; i < (int)hanzi.PinyinCount; ++i)
                        {
                            hanzi.PinyinIndexList[i] = binaryReader.ReadInt16();
                        }
                        HanziDict.Add(hanzi.Char, hanzi);
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

        /// <summary>
        /// 使用PinyinDictionary原始文件进行创建字典PinYinDict
        /// </summary>
        private static void InitPinYin()
        {
            PinYinDict = new Dictionary<int, string>();
            Assembly assembly = typeof(ChineseCharToPinYinHelper).Assembly;
            using (BinaryReader binaryReader = new BinaryReader(assembly.GetManifestResourceStream("HanZiPinYin.Files.PinyinDictionary")))
            {
                // binaryReader.ReadInt32();
                binaryReader.ReadInt16();
                var count = binaryReader.ReadInt16();
                binaryReader.ReadInt16();
                binaryReader.ReadBytes(8);
                for (int index = 0; index < count; ++index)
                {
                    byte[] bytes = binaryReader.ReadBytes(7);
                    var py = Encoding.ASCII.GetString(bytes, 0, 7);
                    char[] chArray = new char[1];
                    py = py.TrimEnd(chArray);
                    py = py.Substring(0, py.Length - 1);
                    PinYinDict.Add(index, py);
                }
            }
        }
        /// <summary>
        /// 精简原始的汉字和拼音文件
        /// </summary>
        private static void RebuildFiles()
        {
            Dictionary<string, short> piyins = new Dictionary<string, short>();

            {
                //优化文件
                var li = PinYinDict.Select(c => c.Value).Distinct().ToList();
                for (short i = 0; i < li.Count; ++i)
                {
                    piyins.Add(li[i], i);
                }
                FileStream fs = new FileStream("SmallPinyinDictionary", FileMode.OpenOrCreate);
                BinaryWriter w = new BinaryWriter(fs);

                //以二进制方式向创建的文件中写入内容 
                w.Write(li.Count);                   //  整型
                li.ForEach(c =>
                {
                    byte[] numArray = new byte[7];
                    Encoding.ASCII.GetBytes(c, 0, c.Length, numArray, 0);
                    w.Write(numArray);
                });

                w.Close();
                fs.Close();
            }
            {
                FileStream fs = new FileStream("SmallHanZiDictionary", FileMode.OpenOrCreate);
                BinaryWriter w = new BinaryWriter(fs);
                w.Write(HanziDict.Count);
                foreach (var item in HanziDict)
                {
                    var hz = item.Value;
                    var tempLi = new List<string>();

                    for (int i = 0; i < hz.PinyinCount; ++i)
                    {
                        var str = PinYinDict[hz.PinyinIndexList[i]];
                        str = str.Substring(0, str.Length);
                        tempLi.Add(str);
                        if (i > 1)
                        {

                        }
                    }
                    tempLi = tempLi.Distinct().ToList();
                    if (tempLi.Count > 8)
                    {

                    }
                    byte pyNum = (byte)tempLi.Count;

                    var pyIndex = new List<short>();
                    foreach (var py in tempLi)
                    {
                        pyIndex.Add(piyins[py]);
                    }


                    w.Write(hz.Char);
                    w.Write(pyNum);
                    pyIndex.ForEach(c => w.Write(c));
                }

                w.Close();
                fs.Close();
            }

        }
        #endregion
    }
}
