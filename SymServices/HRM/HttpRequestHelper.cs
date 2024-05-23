using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using SymViewModel.HRM;

namespace SymServices.HRM
{
    public class HttpRequestHelper
    {
        string BaseURL = "";

        public HttpRequestHelper()
        {
            BaseURL = ConfigurationManager.AppSettings["baseUrl"];
        }
        
        public AuthModel GetAuthentication(CredentialModel credentialModel)
        {
            try
            {
                var keyValues = new Dictionary<string, string>();

                keyValues.Add("UserName", credentialModel.UserName);
                keyValues.Add("Password", credentialModel.Password);
                keyValues.Add("ApiKey", credentialModel.ApiKey);
                keyValues.Add("Grant_type", credentialModel.Grant_type);

                var result = PostFormUrlEncoded("/token", new FormUrlEncodedContent(keyValues));

                return JsonConvert.DeserializeObject<AuthModel>(result);
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public string PostData(string url, AuthModel auth, string payLoad)
        {
            try
            {
                WebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + url);
                request.Method = "POST";
                request.Headers.Add("Authorization", "Bearer " + auth.Access_token);
                byte[] byteArray = Encoding.UTF8.GetBytes(payLoad);
                request.ContentLength = byteArray.Length;
                request.ContentType = "application/json";
                //request.ContentType = "application/json charset=utf-8";
                ////NetworkCredential creds = GetCredentials();
                ////request.Credentials = creds;

                Stream datastream = request.GetRequestStream();
                datastream.Write(byteArray, 0, byteArray.Length);
                datastream.Close();

                //WebResponse response = request.GetResponse();
                //datastream = response.GetResponseStream();

                //StreamReader reader = new StreamReader(datastream);

                //string responseMessage = reader.ReadToEnd();

                //reader.Close();

                WebResponse response = request.GetResponse();
                datastream = response.GetResponseStream();

                StreamReader reader = new StreamReader(datastream);
                string responseMessage = reader.ReadToEnd();

                reader.Close();

                return responseMessage;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string PostFormUrlEncoded(string url, FormUrlEncodedContent formUrlEncodedContent)
        {
            try
            {
                WebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + url);
                request.Method = "POST";

                byte[] byteArray = Encoding.UTF8.GetBytes(formUrlEncodedContent.ReadAsStringAsync().Result);
                request.ContentLength = byteArray.Length;

                request.ContentType = "application/x-www-form-urlencoded";

                //NetworkCredential creds = GetCredentials();
                //request.Credentials = creds;

                Stream datastream = request.GetRequestStream();
                datastream.Write(byteArray, 0, byteArray.Length);
                datastream.Close();

                WebResponse response = request.GetResponse();
                datastream = response.GetResponseStream();

                StreamReader reader = new StreamReader(datastream);

                string responseMessage = reader.ReadToEnd();

                reader.Close();

                return responseMessage;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string GetData(string url, AuthModel auth)
        {
            try
            {
                WebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + url);
                request.Method = "GET";

                request.Headers.Add("Authorization", "Bearer " + auth.Access_token);

                //byte[] byteArray = Encoding.UTF8.GetBytes(payLoad);
                //request.ContentLength = byteArray.Length;

                //request.ContentType = "text/xml;charset=UTF-8";

                //NetworkCredential creds = GetCredentials();
                //request.Credentials = creds;

                //datastream.Write(byteArray, 0, byteArray.Length);
                //datastream.Close();

                WebResponse response = request.GetResponse();
                Stream datastream = response.GetResponseStream();

                StreamReader reader = new StreamReader(datastream);
                string responseMessage = reader.ReadToEnd();

                reader.Close();

                return responseMessage;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private NetworkCredential GetCredentials()
        {

            return new NetworkCredential("vatuser", "123456");

        }

        public object PostDataReport(string url, AuthModel authModel, string data)
        {
            try
            {
                WebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";

                byte[] byteArray = Encoding.UTF8.GetBytes(data);
                request.ContentLength = byteArray.Length;
                request.ContentType = "application/json";
                Stream datastream = request.GetRequestStream();
                datastream.Write(byteArray, 0, byteArray.Length);
                datastream.Close();

                WebResponse response = request.GetResponse();
                datastream = response.GetResponseStream();
                return datastream;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static string DataTableToJson(System.Data.DataTable dataTable)
        {
            try
            {
                string json = JsonConvert.SerializeObject(dataTable, Formatting.Indented);
                return json;
            }
            catch (Exception)
            {
                return "";
            }

        }



    }

}
