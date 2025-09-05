using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Utility.Helper
{
    /// <summary>
    /// 生成验证码图片类
    /// </summary>
    public class VerifyCodeHelper
    {
        private static readonly Random _random = new Random();
        /// <summary>
        /// 老方法
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static Bitmap CreateVerifyCode(out string code)
        {
            //建立Bitmap对象，绘图
            Bitmap bitmap = new Bitmap(200, 60);
            Graphics graph = Graphics.FromImage(bitmap);
            graph.FillRectangle(new SolidBrush(Color.White), 0, 0, 200, 60);
            Font font = new Font(FontFamily.GenericSerif, 48, FontStyle.Bold, GraphicsUnit.Pixel);
            Random r = new Random();
            string letters = "ABCDEFGHIJKLMNPQRSTUVWXYZ0123456789";

            StringBuilder sb = new StringBuilder();

            //添加随机的五个字母
            for (int x = 0; x < 5; x++)
            {
                string letter = letters.Substring(r.Next(0, letters.Length - 1), 1);
                sb.Append(letter);
                graph.DrawString(letter, font, new SolidBrush(Color.Black), x * 38, r.Next(0, 15));
            }
            code = sb.ToString();

            //混淆背景
            Pen linePen = new Pen(new SolidBrush(Color.Black), 2);
            for (int x = 0; x < 6; x++)
                graph.DrawLine(linePen, new Point(r.Next(0, 199), r.Next(0, 59)), new Point(r.Next(0, 199), r.Next(0, 59)));
            return bitmap;
        }




        /// <summary>
        /// 生成验证码图片，并通过 out 参数返回验证码文本
        /// </summary>
        /// <param name="code">输出的验证码字符串</param>
        /// <param name="width">图片宽度（默认 200）</param>
        /// <param name="height">图片高度（默认 60）</param>
        /// <param name="codeLength">验证码长度（默认 5）</param>
        /// <param name="hasNoise">是否添加干扰元素（默认 true）</param>
        /// <returns>验证码图片（Bitmap）</returns>
        public static Bitmap GenerateCaptcha(
            out string code,
            int width = 300,
            int height = 90,
            int codeLength = 5,
            bool hasNoise = true)
        {
            const string letters = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // 去除易混淆字符
            var bitmap = new Bitmap(width, height);
            var sb = new StringBuilder();

            using (var graph = Graphics.FromImage(bitmap))
            {
                graph.SmoothingMode = SmoothingMode.AntiAlias;
                graph.Clear(Color.White);

                // 1. 生成随机验证码
                for (int i = 0; i < codeLength; i++)
                {
                    sb.Append(letters[_random.Next(letters.Length)]);
                }
                code = sb.ToString(); // 通过 out 参数返回

                // 2. 绘制验证码字符（带随机旋转、位置、颜色）
                for (int i = 0; i < code.Length; i++)
                {
                    using (var font = new Font(
                        FontFamily.GenericSerif,
                        _random.Next(32, 48),
                        FontStyle.Bold | FontStyle.Italic))
                    {
                        var brush = new SolidBrush(Color.FromArgb(
                            _random.Next(50, 150),
                            _random.Next(50, 150),
                            _random.Next(50, 150)));

                        graph.TranslateTransform(15 + i * 38, 25); // 调整字符位置
                        graph.RotateTransform(_random.Next(-30, 30)); // 随机旋转
                        graph.DrawString(code[i].ToString(), font, brush, 0, 0);
                        graph.ResetTransform(); // 重置变换
                    }
                }

                // 3. 添加干扰线（可选）
                if (hasNoise)
                {
                    using (var pen = new Pen(Color.FromArgb(200, 200, 200)))
                    {
                        for (int i = 0; i < 10; i++) // 10 条干扰线
                        {
                            graph.DrawLine(
                                pen,
                                _random.Next(0, width), _random.Next(0, height),
                                _random.Next(0, width), _random.Next(0, height));
                        }
                    }

                    // 添加噪点
                    for (int i = 0; i < 100; i++)
                    {
                        int x = _random.Next(0, width);
                        int y = _random.Next(0, height);
                        graph.FillRectangle(Brushes.LightGray, x, y, 1, 1);
                    }
                }
            }

            return bitmap;
        }
    }
}
