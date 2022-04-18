
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Varneon.WebRequest
{
    /// <summary>
    /// Core web request handler
    /// </summary>
    internal class WebRequestHandler
    {
        private static WebRequestHandler instance = null;

        internal static WebRequestHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new WebRequestHandler();
                }
                return instance;
            }
        }

        /// <summary>
        /// Enum flag for indicating the type of data contained in a WebRequestResponse
        /// </summary>
        [Flags]
        internal enum ReturnValue
        {
            None = 0,
            Text = 1,
            Data = 2
        }

        /// <summary>
        /// Response struct for data returned from web request
        /// </summary>
        internal struct Response
        {
            /// <summary>
            /// A floating-point value between 0.0 and 1.0, indicating the progress of downloading body data from the server.
            /// </summary>
            internal float DownloadProgress;

            /// <summary>
            /// The numeric HTTP response code returned by the server, such as 200, 404 or 500.
            /// </summary>
            internal long ResponseCode;

            /// <summary>
            /// What data does the current response hold
            /// </summary>
            internal ReturnValue DownloadHandlerData;

            /// <summary>
            /// The raw bytes downloaded from the remote server, or null.
            /// </summary>
            internal byte[] Data;

            /// <summary>
            /// The bytes from data interpreted as a UTF8 string.
            /// </summary>
            internal string Text;
        }

        /// <summary>
        /// Standard Unity Web Request Enumerator
        /// </summary>
        /// <param name="url"></param>
        /// <returns>WebRequestResponse</returns>
        internal IEnumerator Request(string url, ReturnValue returnValues = ReturnValue.None)
        {
            Response response = new Response()
            {
                DownloadHandlerData = returnValues
            };

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.SendWebRequest();

                while (!request.isDone)
                {
                    response.DownloadProgress = request.downloadProgress;

                    yield return response;
                }

                response.ResponseCode = request.responseCode;

                if (request.isHttpError || request.isNetworkError)
                {
                    Debug.LogError(request.error);
                }
                else
                {
                    if (returnValues.HasFlag(ReturnValue.Data)) { response.Data = request.downloadHandler.data; }
                    if (returnValues.HasFlag(ReturnValue.Text)) { response.Text = request.downloadHandler.text; }
                }
            }

            yield return response;
        }
    }
}
