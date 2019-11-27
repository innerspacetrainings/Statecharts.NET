using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Xml.Linq;
using Jint;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Statecharts.NET.Interpreter;

namespace Statecharts.NET.SCION.SCXML.Tests
{
    internal static class TestServer
    {
        private static string GetResponseJson(int session, IEnumerable<string> configuration)
        => new JObject(
                new JProperty("sessionToken", session),
                new JProperty("nextConfiguration", new JArray(configuration))).ToString();

        private static void SendResponse(HttpListenerResponse response, string @string)
        {
            var buffer = System.Text.Encoding.UTF8.GetBytes(@string);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
        }

        private static IRequest DeserializeJsonRequest(string json)
        {
            T DeserializeStrict<T>()
                where T : class
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Error });
                }
                catch (Exception)
                {
                    return null;
                }
            }
            var loadRequest = DeserializeStrict<LoadRequest>();
            var sendEventRequest = DeserializeStrict<SendEventRequest>();
            return new IRequest[] { loadRequest, sendEventRequest, new InvalidRequest() }
                .FirstOrDefault(o => o != null);
        }

        public static void Main(string[] args)
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }

            var listener = new HttpListener();
            var sessionCounter = 0;
            var testSessions = new Dictionary<int, Service<EcmaScriptContext>>();

            listener.Prefixes.Add("http://+:42000/");
            listener.Start();

            while (true)
            {
                var context = listener.GetContext(); // Note: The GetContext method blocks while waiting for a request. 
                var response = context.Response;
                var request = DeserializeJsonRequest(new StreamReader(context.Request.InputStream).ReadToEnd());
                void Respond(IEnumerable<string> configuration) =>
                    SendResponse(response, GetResponseJson(sessionCounter, configuration));

                try
                {
                    switch (request)
                    {
                        case LoadRequest loadRequest:
                            sessionCounter++;
                            var scxmlDefinition = new WebClient().DownloadString(new Uri(loadRequest.FileName));
                            var definition = SCXMLEcmascript.TestXML(scxmlDefinition);
                            var parsed = definition.Parse();
                            if (parsed is ExecutableStatechart<EcmaScriptContext> executable)
                                testSessions[sessionCounter] = executable.Interpret();
                            else
                                break; // TODO: send invalid Statechart Request
                            var initialState = testSessions[sessionCounter].Start();
                            var initialConfiguration = initialState.StateConfiguration.StateNodeIds
                                .Select(id => string.Join(".", id.Path.Select(key => key.Map(_ => null, named => named.StateName)).Where(key => key != null))).Where(id => !string.IsNullOrEmpty(id));
                            Respond(initialConfiguration);
                            break;
                        case SendEventRequest sendEventRequest:
                            var nextState = testSessions[sessionCounter].Send(new NET.Event(sendEventRequest.Event.Name));
                            var nextConfiguration = nextState.StateConfiguration.StateNodeIds
                                .Select(id => string.Join(".", id.Path.Select(key => key.Map(_ => null, named => named.StateName)).Where(key => key != null))).Where(id => !string.IsNullOrEmpty(id));
                            Respond(nextConfiguration);
                            break;
                        case InvalidRequest _:
                            SendResponse(response, "Unrecognized request.");
                            break;
                    }
                }
                catch (Exception e)
                {
                    SendResponse(response, "An error occured: " + e);
                }
                response.OutputStream.Close();
            }
        }
    }
}
