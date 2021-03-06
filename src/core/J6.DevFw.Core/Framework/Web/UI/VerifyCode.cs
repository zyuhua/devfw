﻿/*
 * VerifyCode 验证码
 * Copyright 2010 OPSoft ,All right reseved!
 * Newmin(ops.cc)  @  2010/11/18
 */

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Web;

namespace JR.DevFw.Framework.Web.UI
{
    /// <summary>
    /// 验证码组成字符选项
    /// </summary>
    public enum VerifyWordOptions
    {
        /// <summary>
        /// 全数字
        /// </summary>
        Number,

        /// <summary>
        /// 全字母
        /// </summary>
        Letter,

        /// <summary>
        /// 字母和数字
        /// </summary>
        LetterAndNumber

        /// <summary>
        /// 中文字符
        /// </summary>
        //Chinese
    }

    /// <summary>
    /// 验证码
    /// </summary>
    public class VerifyCode
    {
        private delegate bool TestCondition(int number, int[] array);

        private const int _n_s = 48; //数字开始
        private const int _n_e = 57; //数字结束
        private const int _ul_s = 65; //大写字母开始
        private const int _ul_e = 90; //大写字母结束
        private const int _ll_s = 97; //小写字母开始
        private const int _ll_e = 122; //小写字母结束
        private const int wordLength = 62;

        private static int[] wordArray = new int[62];

        private bool allowRepeat = true;

        static VerifyCode()
        {
            //初始化，将0-9,A-Z,a-z添加到数组中去
            for (int i = 0; i < 10; i++)
            {
                wordArray[i] = _n_s + i;
            }
            for (int i = 0; i < 26; i++)
            {
                wordArray[36 + i] = _ul_s + i;
                wordArray[10 + i] = _ll_s + i;
            }
        }

        /// <summary>
        /// 是否允许重复出现
        /// </summary>
        public bool AllowRepeat
        {
            get { return allowRepeat; }
            set { allowRepeat = value; }
        }


        /*
        /// <summary>
        /// 验证是否与当前验证码输入一致(区分大小写)
        /// </summary>
        /// <param name="verifyString"></param>
        /// <returns></returns>
        public static bool Verify(string verifyString)
        {
            return Verify(verifyString, false);
        }

        
        /// <summary>
        /// 验证是否与当前验证码输入一致
        /// </summary>
        /// <param name="verifyString"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static bool Verify(string verifyString, bool ignoreCase)
        {
            string verifyCode = HttpContext.Current.Session["current_verifycode"] as string;
            if (String.IsNullOrEmpty(verifyString)) return false;
            return String.Compare(verifyString, verifyCode, ignoreCase) == 0;
        }
         */

        /// <summary>
        /// 获取默认字体
        /// </summary>
        /// <returns></returns>
        public Font GetDefaultFont()
        {
            FontFamily fontFamily;
            try
            {
                fontFamily = FontFamily.GenericSansSerif;
            }
            catch
            {
                if (FontFamily.Families.Length != 0)
                {
                    fontFamily = FontFamily.Families[0];
                }
                else
                {
                    throw new Exception("计算机上找不到字体!");
                }
            }
            return new Font(fontFamily, 14, FontStyle.Bold, GraphicsUnit.Pixel);
        }

        /// <summary>
        /// 显示验证码图片
        /// </summary>
        public byte[] GraphicDrawImage(int number, VerifyWordOptions opt, bool simpleMode, out string words)
        {
            return GraphicDrawImage(number, opt, simpleMode, GetDefaultFont(), 22, out words);
        }

        /// <summary>
        /// 显示验证码图片
        /// </summary>
        public void RenderGraphicImage(int number, VerifyWordOptions opt, bool simpleMode, out string words,
            string contentType)
        {
            HttpContext context = HttpContext.Current;
            byte[] data = GraphicDrawImage(number, opt, simpleMode, out words);

            context.Response.BinaryWrite(data);
            context.Response.ContentType = contentType ?? "Image/Jpeg";
        }

        /// <summary>
        /// 显示验证码图片
        /// </summary>
        public byte[] GraphicDrawImage(int number, VerifyWordOptions opt, bool simpleMode, Font font, int imageHeight,
            out string words)
        {
            HttpContext context = HttpContext.Current;

            int[] verifyWords = new int[number];
            Random rd = new Random();

            TestCondition test;
            int _tempInt;


            switch (opt)
            {
                //纯数字
                case VerifyWordOptions.Number:

                    test = (i, array) =>
                    {
                        if (i == 0) return false;
                        else if (i < _n_s || i > _n_e) return false;
                        else if (!AllowRepeat && Array.Exists(array, a => a == i)) return false;
                        return true;
                    };

                    for (int i = 0; i < number; i++)
                    {
                        while (verifyWords[i] == 0)
                        {
                            _tempInt = wordArray[rd.Next(wordLength)];
                            if (test(_tempInt, verifyWords))
                            {
                                verifyWords[i] = _tempInt;
                            }
                        }
                    }

                    break;


                //纯字母
                case VerifyWordOptions.Letter:

                    test = (i, array) =>
                    {
                        if (i == 0) return false;
                        else if (i < _ul_s || i > _ll_e || (i > _ul_e && i < _ll_s)) return false;
                        else if (!AllowRepeat && Array.Exists(array, a => a == i)) return false;
                        return true;
                    };

                    for (int i = 0; i < number; i++)
                    {
                        while (verifyWords[i] == 0)
                        {
                            _tempInt = wordArray[rd.Next(wordLength)];
                            if (test(_tempInt, verifyWords))
                            {
                                verifyWords[i] = _tempInt;
                            }
                        }
                    }

                    break;


                //字母和数字
                case VerifyWordOptions.LetterAndNumber:

                    test = (i, array) =>
                    {
                        if (i == 0) return false;
                        else if (!Array.Exists(wordArray, a => a == i)) return false;
                        else if (!AllowRepeat && Array.Exists(array, a => a == i)) return false;
                        return true;
                    };

                    for (int i = 0; i < number; i++)
                    {
                        while (verifyWords[i] == 0)
                        {
                            _tempInt = wordArray[rd.Next(wordLength)];
                            if (test(_tempInt, verifyWords))
                            {
                                verifyWords[i] = _tempInt;
                            }
                        }
                    }

                    break;
            }

            //转换成字母
            StringBuilder sb = new StringBuilder();
            foreach (int i in verifyWords)
            {
                sb.Append((char) i);
            }

            //context.Session["current_verifycode"] = sb.ToString();
            words = sb.ToString();

            //绘图
            return DrawingImage(verifyWords, simpleMode, font, imageHeight);
        }

        /// <summary>
        /// 绘图
        /// </summary>
        /// <param name="charNumberArray"></param>
        private byte[] DrawingImage(int[] charNumberArray, bool simpleMode, Font font, int imageHeight)
        {
            float _fontSize = font.Size;
            int _height = imageHeight;
            const int _offset = 5;

            Bitmap img = new Bitmap(charNumberArray.Length*(int) _fontSize + _offset, _height);
            Graphics g = Graphics.FromImage(img);
            g.Clear(Color.White);

            //生成随机生成器   
            Random rd = new Random();

            //画图片的干扰线   
            for (int i = 0; i < 25; i++)
            {
                int x1 = rd.Next(img.Width);
                int x2 = rd.Next(img.Width);
                int y1 = rd.Next(img.Height);
                int y2 = rd.Next(img.Height);
                g.DrawLine(new Pen(Color.FromArgb(200, 200, 200)), x1, y1, x2, y2);
            }

            FontFamily ffamily = font.FontFamily;
            //try
            //{
            //    ffamily = FontFamily.GenericSerif;
            //}
            //catch
            //{
            //    FontFamily[] ffs = FontFamily.Families;
            //    if (ffs.Length > 1) ffamily = ffs[0];
            //    else
            //    {
            //        throw new Exception("系统中未找到任何字体!");
            //    }
            //}

            Brush[] brushs = new SolidBrush[]
            {
                new SolidBrush(Color.Green),
                new SolidBrush(Color.Blue),
                new SolidBrush(Color.Red),
                new SolidBrush(Color.Black)
                //new SolidBrush(Color.Orange)
            };


            for (int i = 0; i < charNumberArray.Length; i++)
            {
                g.DrawString(((char) charNumberArray[i]).ToString(), font, brushs[rd.Next(brushs.Length)],
                    new PointF(_offset + i*(_fontSize - 1), (_height - _fontSize)/2));
            }

            if (!simpleMode)
            {
                //弯曲图片
                img = TwistImage(img, true, 2, 1);

                //画图片的前景干扰点   
                for (int i = 0; i < 100; i++)
                {
                    int x = rd.Next(img.Width);
                    int y = rd.Next(img.Height);
                    img.SetPixel(x, y, Color.FromArgb(235, 235, 235));
                }
            }


            MemoryStream stream = new MemoryStream();
            img.Save(stream, ImageFormat.Jpeg);
            font.Dispose();
            g.Dispose();
            img.Dispose();

            byte[] data = stream.ToArray();
            stream.Dispose();

            return data;
        }

        /// <summary>
        /// 正弦曲线Wave扭曲图片（http://www.51aspx.com/CV/VerifyColorTwistCode/）
        /// </summary>
        /// <param name="srcBmp">图片路径</param>
        /// <param name="bXDir">如果扭曲则选择为True</param>
        /// <param name="nMultValue">波形的幅度倍数，越大扭曲的程度越高，一般为3</param>
        /// <param name="dPhase">波形的起始相位，取值区间[0-2*PI)</param>
        /// <returns></returns>
        private Bitmap TwistImage(Bitmap srcBmp, bool bXDir, double dMultValue, double dPhase)
        {
            // const double PI = 3.1415926535897932384626433832795;

            const double PI2 = 6.283185307179586476925286766559;

            Bitmap destBmp = new Bitmap(srcBmp.Width, srcBmp.Height);

            // 将位图背景填充为白色
            System.Drawing.Graphics graph = System.Drawing.Graphics.FromImage(destBmp);
            graph.FillRectangle(new SolidBrush(System.Drawing.Color.White), 0, 0, destBmp.Width, destBmp.Height);
            graph.Dispose();

            double dBaseAxisLen = bXDir ? (double) destBmp.Height : (double) destBmp.Width;

            for (int i = 0; i < destBmp.Width; i++)
            {
                for (int j = 0; j < destBmp.Height; j++)
                {
                    double dx = 0;
                    dx = bXDir ? (PI2*(double) j)/dBaseAxisLen : (PI2*(double) i)/dBaseAxisLen;
                    dx += dPhase;
                    double dy = Math.Sin(dx);

                    // 取得当前点的颜色
                    int nOldX = 0, nOldY = 0;
                    nOldX = bXDir ? i + (int) (dy*dMultValue) : i;
                    nOldY = bXDir ? j : j + (int) (dy*dMultValue);

                    System.Drawing.Color color = srcBmp.GetPixel(i, j);
                    if (nOldX >= 0 && nOldX < destBmp.Width
                        && nOldY >= 0 && nOldY < destBmp.Height)
                    {
                        destBmp.SetPixel(nOldX, nOldY, color);
                    }
                }
            }
            return destBmp;
        }
    }
}