using System;
using System.Collections.Generic;
using System.Net.Http;
using RichardSzalay.MockHttp;

namespace Messerli.LinqToRest.Test
{
    internal class HttpClientMock
    {
        private readonly string _root;
        private readonly MockHttpMessageHandler _mockHttp;

        public HttpClientMock(Uri root)
        {
            _root = root.ToString();
            _mockHttp = new MockHttpMessageHandler();
        }

        public HttpClientMock RegisterJsonResponse(string uri, string jsonResponse)
        {
            _mockHttp.When(uri).Respond("application/json", jsonResponse);

            return this;
        }

        public HttpClientMock RegisterFileResponse(string uri, string fileResponse)
        {
            var contentLength = System.Text.Encoding.Unicode.GetByteCount(fileResponse).ToString();
            var headers = new[] { new KeyValuePair<string, string>("content-length", contentLength) };

            _mockHttp.When(uri).Respond(headers, "application/zip", fileResponse);

            return this;
        }

        public HttpClient ToHttpClient()
        {
            return _mockHttp.ToHttpClient();
        }
    }
}
