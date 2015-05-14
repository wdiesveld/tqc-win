using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace tqc
{
    class Compiler
    {
        private string apiKey;
        private string version;
        private string folder;
        private string server;

        public Compiler()
        {
            server = "https://compiler1.tinyqueries.com";
            folder = ".";
        }
        
        public void Init(string[] args)
        {
            foreach (string arg in args)
                ParseArg(arg);
        }

        public void Compile()
        {
            if (apiKey == null)
                throw new Exception("You need to specify an API-key");

            // Init post vars
            var postVars = new NameValueCollection()
            {
                { "api_key", apiKey }
            };

            string[] files = Directory.GetFiles(folder + "\\tiny");

            // Read all source files and add them as post var
            foreach (var file in files)
            {
                var sourceID = Path.GetFileNameWithoutExtension(file);
                var content = File.ReadAllText(file);

                postVars.Set( "code[" + sourceID + "]", content );
            }

            // Check if server can be reached
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead(server))
                {
                }
            }
            catch (Exception)
            {
                throw new Exception("Could not connect to compiler server - is there an internet connection?");
            }

            // Do POST to server
            using (WebClient client = new WebClient())
            {

                string responsBody;
                XmlDocument xml;

                try
                {
                    byte[] respons = client.UploadValues(server, postVars);

                    responsBody = System.Text.Encoding.UTF8.GetString(respons);
                }
                catch (WebException e)
                {
                    var stream = e.Response.GetResponseStream();
                    responsBody = Stream2string(stream);

                    try
                    {
                        xml = String2xml(responsBody);
                    }
                    catch (Exception)
                    {
                        throw new Exception(responsBody);    
                    }

                    var errorMessage = xml.DocumentElement.SelectSingleNode("/error/message").InnerText;

                    throw new Exception(errorMessage);
                }

                try
                {
                    xml = String2xml(responsBody);
                }
                catch (Exception)
                {
                    throw new Exception("Cannot parse respons coming from compiler as XML:\n\n" + responsBody);
                }

                // Parse XML and write SQL & json files
                foreach (XmlNode query in xml.DocumentElement.SelectNodes("/compiled/query"))
                {
                    var queryID = query.Attributes["id"].InnerText;
                    var queryInterface = query.SelectSingleNode("interface");
                    var querySQL = query.SelectSingleNode("sql");

                    if (querySQL != null)
                        File.WriteAllText(folder + "\\sql\\" + queryID + ".sql", querySQL.InnerText);

                    if (queryInterface != null)
                        File.WriteAllText(folder + "\\interface\\" + queryID + ".json", queryInterface.InnerText);
                }
            }

        }

        private string Stream2string(Stream stream)
        {
            string str = "";
            byte[] buffer = new byte[2048]; // read in chunks of 2KB
            int bytesRead;
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                str += System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);

            return str;
        }

        private XmlDocument String2xml(string str)
        {
            var xml = new XmlDocument();
            xml.LoadXml(str);
            return xml;
        }

        private void ParseArg(string arg)
        {
            // The argument which has no '/' is supposed to be the query folder
            if (!arg.StartsWith("/"))
            {
                folder = arg;
                return;
            }

            var sep = arg.IndexOf(':'); 
            
            if (sep == -1)
                throw new Exception("Cannot parse argument '" + arg + "' - each argument should be of the form /key:value");

            var key = arg.Substring(0, sep);
            var value = arg.Substring(sep + 1);

            switch (key)
            {
                case "/apikey": apiKey = value; break;
                case "/server": server = value; break;
                case "/version": version = value; break;
                default: throw new Exception("Unknown argument: " + key);
            }

        }
    }

    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage:\ntqc /apikey:[YOUR-API-KEY] [PATH-TO-QUERIES-FOLDER]\n");
                Console.WriteLine("Optional parameters:");
                Console.WriteLine("/server:[URL-TO-COMPILER-SERVER]");
                Console.WriteLine("/version:[COMPILER-VERSION]");
                return 0;
            }

            var compiler = new Compiler();

            try
            {
                compiler.Init(args);

                Console.WriteLine("TinyQueries compiler being called...");

                compiler.Compile();
            } 
            catch (Exception e)
            {
                Console.Error.WriteLine("TinyQueries error: " + e.Message);
                return 1;
            }

            Console.WriteLine("TinyQueries were compiled successfully");

            return 0;
        }
    }
}
