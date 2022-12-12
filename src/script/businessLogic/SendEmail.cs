using Godot;
using System;
using System.Net;
using System.Text;
using System.IO;
using System.Text.Json;
using System.Collections.Immutable;

namespace ReversiFEI.Email
{

    class TelegramMessageSender
    {
        // TODO: Replace the following with your gateway instance ID, Premium Account client ID and secret:
        private static string INSTANCE_ID = "10";
        private static string CLIENT_ID = "franciscoxavieram@gmail.com";
        private static string CLIENT_SECRET = "07351173b43b49bb83b8131c9629384d";

        private static string API_URL = "https://api.whatsmate.net/v3/telegram/single/text/message/" + INSTANCE_ID;

        static void Main(string[] args)
        {
            TelegramMessageSender msgSender = new TelegramMessageSender();
            msgSender.sendMessage("522281300683", "Isn't this excting?");  //  Specify the recipient's number here. NOT the gateway number
            GD.Print("Press Enter to exit.");
            Console.ReadLine();
        }

        public bool sendMessage(string number, string message)
        {
            bool success = true;
            try
            {
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(API_URL);
                httpRequest.Method = "POST";
                httpRequest.Accept = "application/json";
                httpRequest.ContentType = "application/json";
                httpRequest.Headers["X-WM-CLIENT-ID"] = CLIENT_ID;
                httpRequest.Headers["X-WM-CLIENT-SECRET"] = CLIENT_SECRET;

                Payload payloadObj = new Payload() { number = number, message = message };
                string postData = JsonSerializer.Serialize(payloadObj);

                using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                {
                    streamWriter.Write(postData);
                }

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    Console.WriteLine(result);
                }
            }
            catch (WebException webExcp)
            {
                Console.WriteLine("A WebException has been caught.");
                Console.WriteLine(webExcp.ToString());
                WebExceptionStatus status = webExcp.Status;
                if (status == WebExceptionStatus.ProtocolError)
                {
                    /*
                    Console.Write("The REST API server returned a protocol error: ");
                    HttpWebResponse? httpResponse = webExcp.Response as HttpWebResponse;
                    Stream stream = httpResponse.GetResponseStream();
                    StreamReader reader = new StreamReader(stream);
                    String body = reader.ReadToEnd();
                    Console.WriteLine((int)httpResponse.StatusCode + " - " + body);
                    success = false;
                }*/
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("A general exception has been caught.");
                Console.WriteLine(e.ToString());
                success = false;
            }


            return success;
        }

        public class Payload
        {
            public string number { get; set; }
            public string message { get; set; }
        }

    }
}