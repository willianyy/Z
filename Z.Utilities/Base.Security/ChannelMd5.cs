// -----------------------------------------------------------------------
// <copyright file="ChannelMd5.cs" company="HuiBing">
//     Copyright  HuiBing. All rights reserved.
//  </copyright>
// <author>JiangWeiPeng</author>
// <date>2016-08-08</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Z.Utilities
{
    /// <summary>
    /// 类名：MD5
    /// 功能：MD5加密
    /// 修改日期：2016-08-08
    /// </summary>
    public sealed class ChannelMd5
    {
        public ChannelMd5()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        /// <summary>
        /// 签名字符串
        /// </summary>
        /// <param name="prestr">需要签名的字符串</param>
        /// <param name="key">密钥</param>
        /// <param name="inputCharset">编码格式: utf-8</param>
        /// <returns>签名结果</returns>
        public static string Sign(string prestr, string key, string inputCharset)
        {
            StringBuilder sb = new StringBuilder(32);

            prestr = prestr + key;

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.GetEncoding(inputCharset).GetBytes(prestr));
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }

            return sb.ToString();
        }

        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="prestr">需要签名的字符串</param>
        /// <param name="sign">签名结果</param>
        /// <param name="key">密钥</param>
        /// <param name="inputCharset">编码格式: utf-8</param>
        /// <returns>验证结果</returns>
        public static bool Verify(string prestr, string sign, string key, string inputCharset)
        {
            string mysign = Sign(prestr, key, inputCharset);
            if (mysign == sign)
            {
                return true;
            }
            else
            {
                return false;
            }
        }




        #region MD5
        /// <summary>
        /// 16位MD5加密方法
        /// </summary>
        /// <param name="strSource">待加密字串</param>
        /// <returns>加密后的字串</returns>
        public static string MD5Encrypt(string strSource)
        {
            return MD5Encrypt(strSource, 16);
        }

        /// <summary>
        /// MD5加密,16/32位MD5加密，适用于非中文32位加密
        /// </summary>
        /// <param name="strSource">待加密字串</param>
        /// <param name="length">16或32值之一,其它则采用.net默认MD5加密算法</param>
        /// <returns>加密后的字串</returns>
        public static string MD5Encrypt(string strSource, int length)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(strSource);
            byte[] hashValue = ((System.Security.Cryptography.HashAlgorithm)System.Security.Cryptography.CryptoConfig.CreateFromName("MD5")).ComputeHash(bytes);
            StringBuilder sb = new StringBuilder();
            switch (length)
            {
                case 16:
                    for (int i = 4; i < 12; i++)
                        sb.Append(hashValue[i].ToString("x2"));
                    break;
                case 32:
                    for (int i = 0; i < 16; i++)
                    {
                        sb.Append(hashValue[i].ToString("x2"));
                    }
                    break;
                default:
                    for (int i = 0; i < hashValue.Length; i++)
                    {
                        sb.Append(hashValue[i].ToString("x2"));
                    }
                    break;
            }
            return sb.ToString().ToLower();
        }


        /// <summary>
        /// 32位MD5加密，适用于含中文32位加密
        /// </summary>
        /// <param name="strSource"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string MD5EncryptPlus(string strSource, string encode)
        {
            MD5CryptoServiceProvider md = new MD5CryptoServiceProvider();
            //return BitConverter.ToString(md.ComputeHash(Encoding.GetEncoding("GBK").GetBytes(strSource))).Replace("-", "");//返回加密后的字符串，支持中文
            return BitConverter.ToString(md.ComputeHash(Encoding.GetEncoding(encode).GetBytes(strSource))).Replace("-", "").ToLower();//返回加密后的字符串，支持中文
        }
        #endregion
    }
}
