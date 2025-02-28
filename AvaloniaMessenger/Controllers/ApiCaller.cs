﻿using AvaloniaMessenger.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaMessenger.Controllers
{
    class ApiCaller
    {
        public Uri ServerUrl { get; set; }
        public string Token = "1";
        // TODO: Make exceptions handler
        public ApiCaller(Uri serverUrl)
        {
            ServerUrl = serverUrl;
        }
        #region Requests

        public T? PutRequest<T>(string method, HttpContent content)
        {
            var response = Put(ServerUrl.AbsoluteUri + method, content);
            return DeserializeResponse<T>(response);
        }
        public T? GetRequest<T>(string method)
        {
            var response = Get(ServerUrl.AbsoluteUri + method);
            return DeserializeResponse<T>(response);
        }
        public T? GetRequest<T>(string method, string query)
        {
            string requestUri = BuildQuery(method, query);
            var response = Get(requestUri);
            return DeserializeResponse<T>(response);
        }

        public HttpResponseMessage GetRequest(string method)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", Token);

            string requestUri = ServerUrl.ToString() + method;

            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = client.GetAsync(requestUri).Result;
            }
            catch
            {
                throw new NoConnectionException("Couldn't reach server");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException("Bad Request", null, response.StatusCode);
            }
            if (response.Content.ToString() == null)
            {
                throw new Exception("Got null response");
            }

            return response;
        }


        public HttpResponseMessage GetRequest(string method, string query)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", Token);

            string requestUri = ServerUrl.ToString() + method + query;

            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = client.GetAsync(requestUri).Result;
            }
            catch
            {
                throw new NoConnectionException("Couldn't reach server");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException("Bad Request", null, response.StatusCode);
            }
            if (response.Content.ToString() == null)
            {
                throw new Exception("Got null response");
            }

            return response;
        }
        public T? PostRequest<T>(string method, string query, HttpContent content)
        {
            string requestUri = BuildQuery(method, query);
            var response = Post(requestUri, content);
            return DeserializeResponse<T>(response);
        }

        public HttpResponseMessage PostRequest(string method, string query, HttpContent content)
        {
            string requestUri = BuildQuery(method, query);
            return Post(requestUri, content);
        }
        #endregion
        #region ClientRequests
        private HttpResponseMessage Get(string requestUri)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", Token);
                try
                {
                    response = client.GetAsync(requestUri).Result;
                }
                catch
                {
                    throw new HttpRequestException("Couldn't reach server");
                }
            }
            return response;
        }
        private HttpResponseMessage Post(string requestUri, HttpContent content)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Add("Authorization", Token);
                content.Headers.Remove("Content-Type");
                content.Headers.Add("Content-Type", "application/json");
                try
                {
                    response = client.PostAsync(requestUri, content).Result;
                }
                catch
                {
                    throw new HttpRequestException("Couldn't reach server");
                }
            }
            return response;
        }
        private HttpResponseMessage Put(string requestUri, HttpContent content)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", Token);

                content.Headers.Remove("Content-Type");
                content.Headers.Add("Content-Type", "application/json");
                try
                {
                    response = client.PutAsync(requestUri, content).Result;
                    string ass = response.Content.ReadAsStringAsync().Result;
                }
                catch
                {
                    throw new HttpRequestException("Couldn't reach server");
                }
            }
            return response;
        }
        private HttpResponseMessage Delete(string requestUri)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", Token);
                try
                {
                    response = client.DeleteAsync(requestUri).Result;
                }
                catch
                {
                    throw new HttpRequestException("Couldn't reach server");
                }
            }
            return response;
        }
        #endregion
        #region AdditionalMethods
        private string BuildQuery(string method, string query)
        {
            return ServerUrl.ToString() + method + query;
        }
        private T? DeserializeResponse<T>(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                return default(T);
            }
            string json = response.Content.ReadAsStringAsync().Result;
            var obj = JsonConvert.DeserializeObject<T>(json);

            return obj;
        }
        #endregion
    }
}