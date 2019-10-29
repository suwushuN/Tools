using Microsoft.International.Converters.PinYinConverter;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinYinConverter
{
    /// <summary>
    /// 优化微软库的汉字判定,提高汉字转拼音的效率
    /// 
    /// </summary>
    public class PinYinConverter
    {
        static PinYinConverter()
        {
            //找到微软无法识别的无效中文方法
            //StringBuilder sb = new StringBuilder();
            //int start = '\u4e00';
            //do
            //{
            //    var ch = (char)start;
            //    if (!ChineseChar.IsValidChar(ch))
            //    {
            //        sb.Append(ch.ToString());
            //    }
            //} while (start++ < '\u9fa5');
            //str = sb.ToString();
            //初始化无效的中文数据
            var str = "丒乄乤乥乧乫乬乭乮乯乲乶乷乺乻乼乽亪伈侤俧兺凧凩凪剦匁匇匴厼叾呠哘哛唜唞唟啂営喸嗭噛噺嚒嚰囕囖圷圸垪垰垳埖塀塩塰墸壊壭壱売夞妵婲嬢嬶孻対専岃岼岾峅嵶巪巼幉広廃廤弐弾彁応怺怾恷悩愥戦扖扥扽抙抜掵掻摂敨敻旀旕昻曢朑朩朰杁杤杦枠枩柡栃栄栍桛桜桟梺梻椙椚椛椡椦椧楾楿榁榊榋槈槗槝樮樰橲橳橴橻櫉櫦欕欟歚歯歳毮気氞浌涜渇渋渓溌澚濏濹灐炞烪焔熋燵爳犞犠猠獣珱琑瓧瓩瓰瓱瓲瓸瓼甅甼畑畓畩畳疂癦癪癷発硳硴礖穃穒穝笂笹笽篒簓簗簯簱籂籎籏籖籡粀粁粂粌粏粐粫粭糀糓紏絵続綛緕縅縇縨繍繧繷缼羪羺翸耨聁聢聣肀肏脳脵腉膤臓舗艝苆荘莻菐萙蒅蒊蓙蓜蔶蘣蘰虄虲蛍蜶蝿螧蟵袮袰裃裄褄褜褝襙襨覅訳読誮譨譳讝蹹躮躵躾軅軈転軣轌辷辺迲逤逧遖郉酛醗釈釻鈨鈬銰鋲錺錻鎒鐞鐢閊閕閪闏陥険隲霯霻靍靎靏鞆鞐鞥顕颪餎饂饹駅駲験騨魞魸魹鮴鯐鯑鯱鯲鯳鰚鰰鱛鱜鱫鳰鴫鵆鵇鵈鵥鶍鶎鶑鶫麿黁黈鼡";
            foreach(var item in str)
            {
                _dictInvalidChar.Add(item, 1);
            }             
        }
        /// <summary>
        /// 在\u4E00-\u9FA5中，不能被微软库识别的中文
        /// </summary>
        private static Dictionary<char, byte> _dictInvalidChar = new Dictionary<char, byte>();

        /// <summary>
        /// 缓存已经创建过的中文
        /// </summary>
        private static ConcurrentDictionary<char, ChineseChar> _dictCachesChar = new ConcurrentDictionary<char, ChineseChar>();

        /// <summary>
        /// 获取中文词汇全拼（含多音字的所有组合） 未实现 GetFullSpellAndFirstLetter
        /// </summary>
        /// <param name="ChineseCharacter">中文字、词</param>
        /// <returns></returns>
        public static List<string> GetFullSpell(string ChineseCharacter)
        {
            var res = new List<string>();


            return res;
        }
        /// <summary>
        /// 获取中文首字母（含多音字的所有组合） 未实现 GetFullSpellAndFirstLetter
        /// </summary>
        /// <param name="ChineseCharacter">中文字、词</param>
        /// <returns></returns>
        public static List<string> GetFirstLetter(string ChineseCharacter)
        {
            var res = new List<string>();

            return res;
        }
        /// <summary>
        ///  获取中文词汇全拼和首字母（含多音字的所有组合）
        /// </summary>
        /// <param name="ChineseCharacter"></param>
        /// <returns>item1,首字母的列表； item2：全拼列表</returns>
        public static Tuple<List<string>, List<string>> GetFullSpellAndFirstLetter(string ChineseCharacter)
        {
            var resFull = new List<List<string>>();
            var resFirstLetter = new List<List<string>>();

            //获取每个汉字的全拼和首字母列表（一个字一行）
            foreach (var item in ChineseCharacter)
            {
                if (IsChineseChar(item))
                {
                    var ch = GetChineseChar(item);
                    var temp = ch.Pinyins.Take(ch.PinyinCount)//获取有效拼音
                                        .Select(c => c.Substring(0, c.Length - 1))//去除音标
                                        .Distinct();//去重
                    resFull.Add(temp.ToList());
                    resFirstLetter.Add(temp.Select(c => c[0].ToString()).Distinct().ToList());
                }
                else
                {
                    resFull.Add(new List<string> { item.ToString() });
                    resFirstLetter.Add(new List<string> { item.ToString() });
                }
            }

            //拼接
            if (resFull.Count == 0)
            {
                return new Tuple<List<string>, List<string>>(new List<string>(), new List<string>());
            }
            var fullList = CombineResult(resFull);
            var firstList = CombineResult(resFirstLetter);

            return new Tuple<List<string>, List<string>>(firstList, fullList);
        }


        private static bool IsChineseChar(char ch)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(ch.ToString(), "[\u4e00-\u9fa5]") && !_dictInvalidChar.ContainsKey(ch);
        }

        private static ChineseChar GetChineseChar(char ch)
        {
            ChineseChar word;
            if (!_dictCachesChar.TryGetValue(ch, out word))
            {
                word = new ChineseChar(ch);
                _dictCachesChar.TryAdd(ch, word);
            }
            return word;
        }


        /// <summary>
        /// 拼接m行字符串（每行包含字符串长度不定）
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
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
                curList.ForEach(c => matrix[flag].ForEach(next => tempLi.Add(c  + next)));//当前行和下一行进行拼接
                curList = new List<string>();//清空
                curList.AddRange(tempLi);
                flag++;
            };
            return curList;
        }
    }
}
