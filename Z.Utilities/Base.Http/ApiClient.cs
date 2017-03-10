using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace Z.Utilities
{
    public class ApiClient
    {
        //
        public static ApiResponse<T> Get<T>(string apiUrl)
        {
            return NExecute<T>(apiUrl, "Get");
        }

        public static ApiResponse<T> Post<T>(string apiUrl, Object postData = null)
        {
            return NExecute<T>(apiUrl, "Post", postData);
        }

        private static ApiResponse<T> NExecute<T>(string apiUrl, string method, Object postData = null)
        {
            string apiAddress = System.Configuration.ConfigurationManager.AppSettings["ApiAddress"].ToString() + apiUrl;
            var result = new ApiResponse<T>();
            HttpClient client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.143 Safari/537.36");
            HttpResponseMessage response = null;
            try
            {
                using (client)
                {
                    switch (method)
                    {
                        case "Get":
                            response = client.GetAsync(apiAddress).Result;
                            break;
                        case "Post":
                            response = client.PostAsJsonAsync(apiAddress, postData).Result;
                            break;
                        case "Delete":
                            response = client.DeleteAsync(apiAddress).Result;
                            break;
                        case "Put":
                            response = client.PutAsJsonAsync(apiAddress, postData).Result;
                            break;
                    }
                    result.StatusCode = response.StatusCode;
                    if (response.IsSuccessStatusCode)
                    {
                        var operateResult = JsonConvert.DeserializeObject<OperateResult<T>>(response.Content.ReadAsStringAsync().Result);
                        result.ResponseResult = operateResult.ResultData;
                        result.Message = operateResult.Message;
                        result.Code = operateResult.Code;
                    }
                    else
                    {
                        result.Message = "服务器未响应";
                        result.Code = "1";
                    }
                }
            }
            catch (Exception ex)
            {
                //如Http请求发生异常,直接记录异常信息
                result.Message = ex.Message;
                result.Code = "1";
            }
            return result;
        }
    }
    internal class OperateResult<T>
    {
        public string Message { get; set; }
        public string Code { get; set; }
        public T ResultData { get; set; }
    }
    public class ApiResponse<T>
    {
        public T ResponseResult { get; set; }

        [JsonIgnore]
        public string Code { get; set; }

        [JsonIgnore]
        public string Message { get; set; }

        [JsonIgnore]
        public string Content { get; set; }
        [JsonIgnore]
        public System.Net.HttpStatusCode StatusCode { get; set; }

    }
}
