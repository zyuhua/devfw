﻿using System.Collections;
using System.Collections.Generic;

namespace Ops.Framework.Extensions
{
    using System;
    using System.Text.RegularExpressions;
    using System.Web;

    public static class StringExtensions
    {
        /// <summary>
        /// 为字符串追加指定长度的随机字符并返回该字符串
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public static string RandomLetters(this String str, int length)
        {
            length = length < 1 ? 1 : length;
            char[] cs =
            {
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't'
                , 'u', 'v', 'w', 'x', 'y', 'z'
            };
            Random rd = new Random();
            for (int i = 0; i < length; i++)
            {
                str += cs[rd.Next(24)]; //24为cs.Length
            }
            return str;
        }

        /// <summary>
        /// 为字符串追加指定长度的随机字符并返回该字符串
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public static string RandomUpperLetters(this String str, int length)
        {
            length = length < 1 ? 1 : length;
            char[] cs =
            {
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't'
                , 'u', 'v', 'w', 'x', 'y', 'z'
            };
            Random rd = new Random();
            int randomNum = 0;
            for (int i = 0; i < length; i++)
            {
                randomNum = rd.Next(24); //24为cs.Length
                str += randomNum > 12 ? cs[rd.Next(randomNum)].ToString().ToUpper() : cs[rd.Next(randomNum)].ToString();
            }
            return str;
        }

        /// <summary>
        /// 用md5加密
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static string EncodeMD5(this String str)
        {
            return Ops.Framework.Security.Md5Crypto.EncodeMD5(str);
        }

        /// <summary>
        /// 16位MD5加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Encode16MD5(this String str)
        {
            return Ops.Framework.Security.Md5Crypto.Encode16MD5(str);
        }


        /// <summary>
        /// HTML编码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string HtmlEncode(this String str)
        {
            return HttpContext.Current.Server.HtmlEncode(str);
        }

        /// <summary>
        /// HTML解码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string HtmlDecode(this String str)
        {
            return HttpContext.Current.Server.HtmlDecode(str);
        }

        /// <summary>
        /// 替换类似%tag%的标签
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Template(this string str, Func<string, string> func)
        {
            Regex regex = new Regex("%([^%]+)%");

            return regex.Replace(str, a => { return func(a.Groups[1].Value); });
        }


        /// <summary>
        /// 替换类似%tag%的标签
        /// </summary>
        /// <param name="str"></param>
        /// <param name="data">传递标签值的数组</param>
        /// <returns></returns>
        public static string Template(this string str, params string[] data)
        {
            Regex regex = new Regex("%([^%]+)%");
            MatchCollection mcs = regex.Matches(str);
            if (mcs.Count != data.Length)
            {
                throw new ArgumentOutOfRangeException();
            }
            else
            {
                for (int i = 0; i < mcs.Count; i++)
                {
                    str = Regex.Replace(str, mcs[i].Groups[0].Value, data[i]);
                }
            }
            return str;
        }

        public static string Template(this string str, Hashtable hash)
        {
            if (hash == null || hash.Count == 0) return str;
            Regex regex = new Regex("{([^{]+)}");
            MatchCollection mcs = regex.Matches(str);
            string key;
            for (int i = 0; i < mcs.Count; i++)
            {
                key = mcs[i].Groups[1].Value;
                if (hash.ContainsKey(key) && hash[key] != null)
                {
                    str = Regex.Replace(str, mcs[i].Groups[0].Value, hash[key].ToString());
                }
            }
            return str;
        }

        public static string Template(this string str, IDictionary<string, string> hash)
        {
            if (hash == null || hash.Count == 0) return str;
            Regex regex = new Regex("{([^{]+)}");
            MatchCollection mcs = regex.Matches(str);
            string key;
            for (int i = 0; i < mcs.Count; i++)
            {
                key = mcs[i].Groups[1].Value;
                if (hash.ContainsKey(key) && hash[key] != null)
                {
                    str = Regex.Replace(str, mcs[i].Groups[0].Value, hash[key]);
                }
            }
            return str;
        }

        /// <summary>
        /// 是否匹配文本
        /// </summary>
        /// <param name="str"></param>
        /// <param name="pattern"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static bool IsMatch(this string str, string pattern, RegexOptions option = RegexOptions.None)
        {
            return Regex.IsMatch(str, pattern, option);
        }
    }
}