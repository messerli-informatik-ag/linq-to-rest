using System.Collections.Generic;
using System.Net.Http;
using RichardSzalay.MockHttp;

namespace Messerli.LinqToRest.Test
{
    internal class HttpClientMockBuilder
    {
        private readonly MockHttpMessageHandler _mockHttp;

        public HttpClientMockBuilder()
        {
            _mockHttp = new MockHttpMessageHandler();
        }

        public HttpClientMockBuilder JsonResponse(string uri, string jsonResponse)
        {
            _mockHttp.When(uri).Respond("application/json", jsonResponse);

            return this;
        }

        public HttpClientMockBuilder FileResponse(string uri, string fileResponse)
        {
            var contentLength = System.Text.Encoding.Unicode.GetByteCount(fileResponse).ToString();
            var headers = new[] { new KeyValuePair<string, string>("content-length", contentLength) };

            _mockHttp.When(uri).Respond(headers, "application/zip", fileResponse);

            return this;
        }

        public HttpClient Build()
        {
            return _mockHttp.ToHttpClient();
        }
    }
}
