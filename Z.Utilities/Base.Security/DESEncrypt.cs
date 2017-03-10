//=====================================================================================
// All Rights Reserved , Copyright © Learun 2013
//=====================================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Z.Utilities
{
    /// <summary>
    /// 加密、解密帮助类
    /// 版本：2.0
    /// <author>
    ///		<name>shecixiong</name>
    ///		<date>2013.09.27</date>
    /// </author>
    /// </summary>
    public class DESEncrypt
    {
        #region ========加密========
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string Encrypt(string Text)
        {
            return Encrypt(Text, "HuiBing###***");
        }
        #region 郭森 DES 时间戳 签名 加密
        /// <summary>
        /// 加密（扩展有签名)
        /// </summary>
        /// <param name="Text">加密内容</param>
        /// <param name="signticks">加密签名戳</param>
        /// <returns></returns>
        public static string Encrypt(string Text, out long signticks)
        {
            signticks = DateTime.Now.Ticks;
            return Encrypt(Text, string.Format("HuiBing###***{0}", signticks));
        }
        #endregion
        /// <summary> 
        /// 加密数据 
        /// </summary> 
        /// <param name="Text"></param> 
        /// <param name="sKey"></param> 
        /// <returns></returns> 
        public static string Encrypt(string Text, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray;
            inputByteArray = Encoding.Default.GetBytes(Text);
            des.Key = ASCIIEncoding.ASCII.GetBytes(CreateKey(sKey, "md5").Substring(0, 8));
            des.IV = ASCIIEncoding.ASCII.GetBytes(CreateKey(sKey, "md5").Substring(0, 8));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();
        }

        #endregion

        #region ========解密========
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string Decrypt(string Text)
        {
            if (!string.IsNullOrEmpty(Text))
            {
                return Decrypt(Text, "HuiBing###***");
            }
            else
            {
                return "";
            }
        }
        #region 郭森 DES 时间戳 签名 解密

        /// <summary>
        /// 数据解密 解密超过时限时,signticks 返回 -1，并返回[签名已过期]
        /// </summary>
        /// <param name="Text">解密内容</param>
        /// <param name="signticks">签名的时间戳</param>
        /// <param name="signcycle">签名有效期（秒) 默认60秒 </param>
        /// <returns>解密超过时限时,signticks 返回 -1，并返回[签名已过期]</returns>
        public static string Decrypt(string Text, ref long signticks, int signcycle = 60)
        {
            var signtime = new DateTime(signticks);
            if (signtime.AddSeconds(signcycle) < DateTime.Now)
            {
                signticks = -1;
                return "签名已过期";
            }
            return Decrypt(Text, string.Format("HuiBing###***{0}", signticks));
        }
        #endregion
        /// <summary> 
        /// 解密数据 
        /// </summary> 
        /// <param name="Text"></param> 
        /// <param name="sKey"></param> 
        /// <returns></returns> 
        public static string Decrypt(string Text, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            int len;
            len = Text.Length / 2;
            byte[] inputByteArray = new byte[len];
            int x, i;
            for (x = 0; x < len; x++)
            {
                i = Convert.ToInt32(Text.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)i;
            }
            des.Key = ASCIIEncoding.ASCII.GetBytes(CreateKey(sKey, "md5").Substring(0, 8));
            des.IV = ASCIIEncoding.ASCII.GetBytes(CreateKey(sKey, "md5").Substring(0, 8));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Encoding.Default.GetString(ms.ToArray());
        }

        #endregion

        #region MD5
        public static string CreateKey(string text, string token = "")
        {
            if (!string.IsNullOrEmpty(token))
            {
                text = string.Format("{1}({0})", text, token);
            }
            var md5 = System.Security.Cryptography.MD5.Create();
            var buff = System.Text.ASCIIEncoding.UTF8.GetBytes(text);
            var md5resultbytes = md5.ComputeHash(buff);
            var md5result = new StringBuilder();
            foreach (var b in md5resultbytes)
            {
                md5result.Append(b.ToString("X2"));
            }
            return md5result.ToString();
        }

        #endregion

        #region 短地址加密算法
        public static string GetShortCode(string str)
        {
            return ShortUrl(str)[0];
        }

        private static string[] ShortUrl(string url)
        {
            //可以自定义生成MD5加密字符传前的混合KEY
            string key = "Leejor";
            //要使用生成URL的字符
            string[] chars = new string[]{
 		    "a","b","c","d","e","f","g","h",
     	    "i","j","k","l","m","n","o","p",
		    "q","r","s","t","u","v","w","x",
   		    "y","z","0","1","2","3","4","5",
 		    "6","7","8","9","A","B","C","D",
  		    "E","F","G","H","I","J","K","L",
  		    "M","N","O","P","Q","R","S","T",
		    "U","V","W","X","Y","Z"
	        };

            //对传入网址进行MD5加密
            string hex = StringHelper.GetMD5(key + url);
            hex = StringHelper.GetMD5(hex); 
            //string hex = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(key + url, "md5");
            //hex = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(hex, "md5");

            string[] resUrl = new string[4];

            for (int i = 0; i < 4; i++)
            {
                //把加密字符按照8位一组16进制与0x3FFFFFFF进行位与运算
                int hexint = 0x3FFFFFFF & Convert.ToInt32("0x" + hex.Substring(i * 8, 8), 16);
                string outChars = string.Empty;
                for (int j = 0; j < 6; j++)
                {
                    //把得到的值与0x0000003D进行位与运算，取得字符数组chars索引
                    int index = 0x0000003D & hexint;
                    //把取得的字符相加
                    outChars += chars[index];
                    //每次循环按位右移5位
                    hexint = hexint >> 5;
                }
                //把字符串存入对应索引的输出数组
                resUrl[i] = outChars;
            }
            return resUrl;
        }
        #endregion 短地址加密算法
    }
}