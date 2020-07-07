namespace Indabo.Host
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using Indabo.Core;
    using Newtonsoft.Json;

    internal class WebServer
    {
        private static readonly string ROOT_DIRECTORY = new FileInfo(new Uri(Assembly.GetEntryAssembly().GetName().CodeBase).LocalPath).Directory.FullName;

        private HttpListener listener;

        private bool running;

        public WebServer(int port)
        {
            this.listener = new HttpListener();
            this.listener.Prefixes.Add($"http://localhost:{port}/");

            Logging.Info($"Webserver initalized with Port '{port}'!");
        }

        public void Start()
        {
            this.listener.Start();

            this.running = true;

            while (this.running)
            {
                try
                {
                    HttpListenerContext context = this.listener.GetContext();

                    Logging.Info("New request from: " + context.Request.UserHostName);

                    new Thread(() =>
                    {
                        this.HandleCallback(context);
                    }).Start();
                }
                catch (Exception ex)
                {
                    Logging.Error("Error while connecting to client!", ex);
                }
            }
        }

        private void HandleCallback(HttpListenerContext context)
        {
            try
            {
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                byte[] buffer = Encoding.UTF8.GetBytes("Page not found...");

                if (request.Url.AbsolutePath == "/favicon.png" || request.Url.AbsolutePath == "/favicon.ico")
                {
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    string resourceName = "Indabo.Host.Content.GUI.Frame.favicon.png";
                    using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                    {
                        using (BinaryReader reader = new BinaryReader(stream))
                        {
                            buffer = reader.ReadBytes((int)stream.Length);
                        }
                    }
                }
                else if (request.Url.AbsolutePath == "/Panels")
                {
                    List<string> panels = DynamicHtmlLoader.GetFileNamesFromFolder(DynamicHtmlLoader.PANEL_FOLDER);

                    string json = JsonConvert.SerializeObject(panels, Formatting.None);
                    buffer = Encoding.UTF8.GetBytes(json);
                }
                else if (request.Url.AbsolutePath.StartsWith("/Panel/"))
                {
                    string absolutePanelPath = Path.Combine(ROOT_DIRECTORY, request.Url.AbsolutePath.TrimStart('/').Replace("/", "\\") + ".html");
                    if (File.Exists(absolutePanelPath))
                    {
                        buffer = Encoding.UTF8.GetBytes(File.ReadAllText(absolutePanelPath));

                        Logging.Info($"Responsed Panel: '{request.Url.AbsolutePath}'");
                    }
                    else
                    {
                        buffer = Encoding.UTF8.GetBytes("Panel not found...");

                        Logging.Warning($"Request of unkonwn Panel: '{request.Url.AbsolutePath}'");
                    }
                }
                else
                {
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    string resourceName = "Indabo.Host.Content.GUI.Frame.Frame.html";
                    using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string frame = reader.ReadToEnd();

                            string libraries = DynamicJavascriptLoader.LoadFromFolderAsHtmlTags(Path.Combine(ROOT_DIRECTORY, DynamicJavascriptLoader.LIBRARY_FOLDER));
                            string widgets = DynamicJavascriptLoader.LoadFromFolderAsHtmlTags(Path.Combine(ROOT_DIRECTORY, DynamicJavascriptLoader.WIDGET_FOLDER));

                            frame = frame.Replace("%Library%", libraries);
                            frame = frame.Replace("%Widget%", widgets);

                            buffer = Encoding.UTF8.GetBytes(frame);
                        }
                    }

                    Logging.Info($"Responsed Main Frame");
                }

                response.ContentLength64 = buffer.Length;
                Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();
            }
            catch (Exception ex)
            {
                Logging.Error("Error while handling callback!", ex);
            }
        }

        public void Stop()
        {
            this.listener.Stop();

            this.running = false;
        }
    }
}
