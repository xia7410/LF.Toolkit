using System;
using System.IO;
using System.Net;
using System.Text;

namespace LF.Toolkit.Util
{
    /// <summary>
    /// 上传文件类
    /// </summary>
    public partial class UploadFileStream
    {
        public string Name { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }

        public Stream FileStream { get; set; }
    }

    /// <summary>
    /// HTTP相关常量类
    /// </summary>
    internal partial class HttpConst
    {
        public const string HTTPMETHOD_HEAD = "HEAD";
        public const string HTTPMETHOD_GET = "GET";
        public const string HTTPMETHOD_POST = "POST";
        public const string HTTPMETHOD_DELETE = "DELETE";
        public const string HTTPMETHOD_PUT = "PUT";

        /// <summary>
        /// HTTP POST FROM x-www-form-urlencoded
        /// </summary>
        public const string CONTENTTYPE_URLENCODED = "application/x-www-form-urlencoded";
        /// <summary>
        /// HTTP POST FROM multipart/form-data
        /// </summary>
        public const string CONTENTTYPE_FORMDATA = "multipart/form-data; boundary=";
    }

    public class HttpProvider
    {
        #region Private

        /// <summary>
        /// 序列化参数
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        static string SerializeQueryString(object parameters)
        {
            if (parameters == null) throw new ArgumentNullException("parameters object can't be null");

            string querystring = "";
            foreach (var property in parameters.GetType().GetProperties())
            {
                querystring += property.Name + "="
                    + System.Uri.EscapeDataString(property.GetValue(parameters, null).ToString()) + "&";
            }
            if (!string.IsNullOrEmpty(querystring)) querystring = querystring.Trim('&');

            return querystring;
        }

        /// <summary>
        /// 异步响应处理
        /// </summary>
        /// <param name="successCallback"></param>
        /// <param name="failCallback"></param>
        /// <returns></returns>
        static AsyncCallback ProcessCallback(Action<string> successCallback, Action<WebException> failCallback)
        {
            return new AsyncCallback((ar) =>
            {
                var request = (HttpWebRequest)ar.AsyncState;

                try
                {
                    using (HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(ar))
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            successCallback(reader.ReadToEnd());
                        }
                    }
                }
                catch (WebException e)
                {
                    failCallback(e);
                }
            });
        }

        /// <summary>
        /// 生成请求
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="method"></param>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <param name="headers"></param>
        /// <param name="successCallback"></param>
        /// <param name="failCallback"></param>
        static void BuildRequest(string contentType, string method, string url, object parameters, object headers, Action<string> successCallback, Action<WebException> failCallback)
        {
            if (parameters == null) throw new ArgumentNullException("parameters object can't be null");
            if (headers == null) throw new ArgumentNullException("headers object can't be null");
            if (string.IsNullOrEmpty(contentType)) throw new ArgumentNullException("contentType");
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException("url");
            if (method != HttpConst.HTTPMETHOD_HEAD && method != HttpConst.HTTPMETHOD_GET && method != HttpConst.HTTPMETHOD_POST
                && method != HttpConst.HTTPMETHOD_DELETE && method != HttpConst.HTTPMETHOD_PUT)
            {
                throw new ArgumentException("invalid http method");
            }

            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
                request.ContentType = contentType;
                request.Method = method;

                //add headers
                foreach (var property in headers.GetType().GetProperties())
                {
                    request.Headers.Add(property.Name, Uri.EscapeDataString(property.GetValue(headers, null).ToString()));
                }

                if (method == HttpConst.HTTPMETHOD_POST || method == HttpConst.HTTPMETHOD_PUT
                    || method == HttpConst.HTTPMETHOD_DELETE)
                {
                    request.BeginGetRequestStream((ar) =>
                    {
                        try
                        {
                            var _request = ar.AsyncState as HttpWebRequest;
                            Stream stream = _request.EndGetRequestStream(ar);
                            byte[] data = Encoding.UTF8.GetBytes(SerializeQueryString(parameters));
                            stream.Write(data, 0, data.Length);
                            stream.Flush();
                            stream.Dispose();

                            _request.BeginGetResponse(ProcessCallback(successCallback, failCallback), _request);
                        }
                        catch (WebException e)
                        {
                            failCallback(e);
                        }
                    }, request);
                }
                if (method == HttpConst.HTTPMETHOD_HEAD || method == HttpConst.HTTPMETHOD_GET)
                {
                    request.BeginGetResponse(ProcessCallback(successCallback, failCallback), request);
                }
            }
            catch (WebException e)
            {
                failCallback(e);
            }
        }

        #endregion

        #region GET

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="url"></param>
        /// <param name="successCallback">成功回发</param>
        /// <param name="failCallback">失败回发</param>
        public static void Get(string url, Action<string> successCallback, Action<WebException> failCallback)
        {
            Get(url, new { }, successCallback, failCallback);
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <param name="successCallback">成功回发</param>
        /// <param name="failCallback">失败回发</param>
        public static void Get(string url, object parameters, Action<string> successCallback, Action<WebException> failCallback)
        {
            Get(url, parameters, new { }, successCallback, failCallback);
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <param name="headers">http headers对象</param>
        /// <param name="successCallback">成功回发</param>
        /// <param name="failCallback">失败回发</param>
        public static void Get(string url, object parameters, object headers, Action<string> successCallback, Action<WebException> failCallback)
        {
            UriBuilder b = new UriBuilder(url);

            if (parameters != null)
            {
                if (!string.IsNullOrEmpty(b.Query))
                {
                    b.Query = b.Query.Substring(1) + "&" + SerializeQueryString(parameters);
                }
                else
                {
                    b.Query = SerializeQueryString(parameters);
                }
            }

            BuildRequest(HttpConst.CONTENTTYPE_URLENCODED, HttpConst.HTTPMETHOD_GET, b.Uri.ToString(), new { }, headers, successCallback, failCallback);
        }

        #endregion

        #region POST

        /// <summary>
        /// Post
        /// </summary>
        /// <param name="url"></param>
        /// <param name="successCallback">成功回发</param>
        /// <param name="failCallback">失败回发</param>
        public static void Post(string url, Action<string> successCallback, Action<WebException> failCallback)
        {
            Post(url, new { }, successCallback, failCallback);
        }

        /// <summary>
        /// Post
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <param name="successCallback">成功回发</param>
        /// <param name="failCallback">失败回发</param>
        public static void Post(string url, object parameters, Action<string> successCallback, Action<WebException> failCallback)
        {
            Post(url, parameters, new { }, successCallback, failCallback);
        }

        /// <summary>
        /// Post
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <param name="headers">http headers对象</param>
        /// <param name="successCallback">成功回发</param>
        /// <param name="failCallback">失败回发</param>
        public static void Post(string url, object parameters, object headers, Action<string> successCallback, Action<WebException> failCallback)
        {
            BuildRequest(HttpConst.CONTENTTYPE_URLENCODED, HttpConst.HTTPMETHOD_POST, url, parameters, headers, successCallback, failCallback);
        }

        #endregion

        #region UPLOAD

        /// <summary>
        /// Upload
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <param name="files"></param>
        /// <param name="successCallback">成功回发</param>
        /// <param name="failCallback">失败回发</param>
        public static void Upload(string url, object parameters, UploadFileStream[] files, Action<string> successCallback, Action<WebException> failCallback)
        {
            Upload(url, parameters, new { }, files, successCallback, failCallback);
        }

        /// <summary>
        /// Upload
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <param name="headers">http headers对象</param>
        /// <param name="files"></param>
        /// <param name="successCallback">成功回发</param>
        /// <param name="failCallback">失败回发</param>
        public static void Upload(string url, object parameters, object headers, UploadFileStream[] files, Action<string> successCallback, Action<WebException> failCallback)
        {
            try
            {
                var charSet = Encoding.UTF8;
                string boundary = "----HttpProviderBoundary" + DateTime.Now.Ticks.ToString("x");
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
                request.KeepAlive = true;
                request.Method = HttpConst.HTTPMETHOD_POST;
                request.ContentType = HttpConst.CONTENTTYPE_FORMDATA + boundary;

                //add headers
                foreach (var property in headers.GetType().GetProperties())
                {
                    //if need Uri.EscapeDataString ??
                    request.Headers.Add(property.Name, property.GetValue(headers, null).ToString());
                }

                request.BeginGetRequestStream((ar) =>
                {
                    #region  [RFC2045]

                    //Content-Type: multipart/form-data; boundary=AaB03x

                    //--AaB03x
                    //Content-Disposition: form-data; name="submit-name"

                    //Larry
                    //--AaB03x
                    //Content-Disposition: form-data; name="files"; filename="file1.txt"
                    //Content-Type: text/plain

                    //... contents of file1.txt ...
                    //--AaB03x--                    

                    #endregion

                    var _request = (HttpWebRequest)ar.AsyncState;
                    Stream reqStream = _request.EndGetRequestStream(ar);
                    byte[] boundarybytes = charSet.GetBytes("\r\n--" + boundary + "\r\n");
                    byte[] endbytes = charSet.GetBytes("\r\n--" + boundary + "--\r\n");

                    var tempStream = new MemoryStream();
                    //Serialize parameters in multipart manner
                    string formdataTemplate = "--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";

                    foreach (var property in parameters.GetType().GetProperties())
                    {
                        string formitem = string.Format(formdataTemplate, property.Name, property.GetValue(parameters, null).ToString());
                        byte[] formitembytes = charSet.GetBytes(formitem);
                        tempStream.Write(formitembytes, 0, formitembytes.Length);
                    }

                    tempStream.Write(boundarybytes, 0, boundarybytes.Length);

                    string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                    int count = files.Length;
                    foreach (var file in files)
                    {
                        var headerbytes = charSet.GetBytes(string.Format(headerTemplate, file.Name, file.FileName, file.ContentType));
                        tempStream.Write(headerbytes, 0, headerbytes.Length);

                        //write filestream
                        file.FileStream.Position = 0;
                        byte[] buffer = new byte[4096];
                        int bytesRead = 0;
                        while ((bytesRead = file.FileStream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            tempStream.Write(buffer, 0, bytesRead);
                        }

                        count--;
                        if (count == 0)
                        {
                            tempStream.Write(endbytes, 0, endbytes.Length);
                        }
                        else
                        {
                            tempStream.Write(boundarybytes, 0, boundarybytes.Length);
                        }
                        
                        file.FileStream.Close();
                    }
                    tempStream.Flush();
                    tempStream.Position = 0;
                    tempStream.CopyTo(reqStream);
                    tempStream.Dispose();
                    _request.BeginGetResponse(ProcessCallback(successCallback, failCallback), _request);
                }, request);
            }
            catch (WebException e)
            {
                failCallback(e);
            }
        }

        #endregion
    }

}
