//******************************************************** 
//单元描述： XXXXXXXXXXXXXXX
//编辑内容：
//2016/2/4 15:06:35 新增  刘挺  创建文件
//******************************************************** 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Z.Utilities
{
    public class ApiRest
    {

        /// <summary>
        /// 获取API提交的参数
        /// </summary>
        /// <param name="request">request对象</param>
        /// <returns>参数数组</returns>
        public static HBParam[] GetParamsFromRequest(HttpRequestBase request)
        {
            List<HBParam> list = new List<HBParam>();
            foreach (string key in request.QueryString.AllKeys)
            {
                list.Add(HBParam.Create(key, request.QueryString[key]));
            }
            foreach (string key in request.Form.AllKeys)
            {
                list.Add(HBParam.Create(key, request.Form[key]));
            }
            list.Sort();
            return list.ToArray();
        }
         
        /// <summary>
        /// 根据参数和密码生成签名字符串
        /// </summary>
        /// <param name="parameters">API参数</param>
        /// <param name="secret">密码</param>
        /// <returns>签名字符串</returns>
        public static string GetSignature(HBParam[] parameters, string secret)
        {
            StringBuilder values = new StringBuilder();

            //foreach (HBParam param in parameters)
            //{
            //    if (param.Name == "sign" || string.IsNullOrEmpty(param.Value))
            //        continue;
            //    values.Append(param.ToString());
            //}
            //values.Append(secret);

            //string hbsgin =
            //    MD5.MD5EncryptPlus(

            //       "appID=" + appID.Text +

            //       "&settlementDate=" + settlementDate.Text +

            //       "&serialNo=" + serialNo.Text +

            //       "&timeStamp=" + TimeStamp.Text +

            //       "&orderList=" + orderList.Text +

            //       "&secretKey=" + this.signKey.Text
            //       , "UTF-8");


            foreach (HBParam param in parameters)
            {
                if (param.Name == "sign")
                    continue;
                values.Append(param.ToString()+"&");
            }
            values.Append("secretKey="+secret);

            byte[] md5_result = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(values.ToString()));

            StringBuilder sig_builder = new StringBuilder();

            foreach (byte b in md5_result)
                sig_builder.Append(b.ToString("x2"));

            return sig_builder.ToString();
        }
    }
}
