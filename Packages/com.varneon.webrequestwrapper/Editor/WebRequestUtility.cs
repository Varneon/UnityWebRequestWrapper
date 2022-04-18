
using System;
using System.Collections;
using System.IO;
using UnityEditor;

namespace Varneon.WebRequest
{
    /// <summary>
    /// Web request utility for handling any standard web requests
    /// </summary>
    internal static class WebRequestUtility
    {
        /// <summary>
        /// Standard GET UnityWebRequest
        /// </summary>
        /// <param name="url"></param>
        /// <param name="returnValues">What data from the download handler should be returned in the response</param>
        /// <returns>WebRequest Response</returns>
        internal static WebRequestHandler.Response Get(string url, WebRequestHandler.ReturnValue returnValues = WebRequestHandler.ReturnValue.None, bool displayProgressBar = true)
        {
            IEnumerator request = WebRequestHandler.Instance.Request(url, returnValues);

            while (request.MoveNext())
            {
                if (displayProgressBar)
                {
                    EditorUtility.DisplayProgressBar("Web Request", url, ((WebRequestHandler.Response)request.Current).DownloadProgress);
                }
            }

            if (displayProgressBar)
            {
                EditorUtility.ClearProgressBar();
            }

            WebRequestHandler.Response response = (WebRequestHandler.Response)request.Current;

            return response;
        }

        internal static FileDownloadAndSaveResponse DownloadAndSaveFile(string url, string savePath)
        {
            FileDownloadAndSaveResponse fileResponse = new FileDownloadAndSaveResponse();

            WebRequestHandler.Response response = Get(url, WebRequestHandler.ReturnValue.Data);

            if (response.ResponseCode != 200)
            {
                fileResponse.Error = $"Web Request Error: {response.ResponseCode}";

                return fileResponse;
            }

            try
            {
                File.WriteAllBytes(savePath, response.Data);

                fileResponse.Success = true;
            }
            catch (Exception e)
            {
                fileResponse.Error = e.Message;
            }

            return fileResponse;
        }

        internal struct FileDownloadAndSaveResponse
        {
            internal bool Success;

            internal string Error;
        }
    }
}
