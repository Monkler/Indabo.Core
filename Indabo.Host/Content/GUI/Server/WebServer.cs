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
            this.listener.Prefixes.Add($"http://*:{port}/");

            Logging.Info($"Webserver initalized with Port '{port}'!");
        }

        public void Start()
        {
            this.listener.Start();

            this.running = true;

            new Thread(() =>
            {
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
            }).Start();
        }

        private void HandleCallback(HttpListenerContext context)
        {
            try
            {
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                response.StatusCode = (int)HttpStatusCode.NotFound;
                byte[] buffer = Encoding.UTF8.GetBytes("Page not found...");

                if (request.Url.AbsolutePath == "/favicon.png" || request.Url.AbsolutePath == "/favicon.ico")
                {
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.ContentType = "image/png";

                    Assembly assembly = Assembly.GetExecutingAssembly();
                    string resourceName = "Indabo.Host.Content.GUI.Frame.Icon.Indabo.png";
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
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.ContentType = "application/json";

                    List<string> panels = DynamicHtmlLoader.GetFileNamesFromFolder(DynamicHtmlLoader.PANEL_FOLDER);

                    string json = JsonConvert.SerializeObject(panels, Formatting.None);
                    buffer = Encoding.UTF8.GetBytes(json);
                }
                else if (request.Url.AbsolutePath.StartsWith("/Panel/") || request.Url.AbsolutePath.StartsWith("/Widget/"))
                {   
                    if (request.Url.AbsolutePath.EndsWith("png"))
                    {
                        response.ContentType = "image/png";
                    }
                    else
                    {
                        response.ContentType = "text/html";
                    }                    

                    string absolutePanelPath = Path.Combine(ROOT_DIRECTORY, request.Url.AbsolutePath.TrimStart('/').Replace("/", "\\"));
                    absolutePanelPath = Uri.UnescapeDataString(absolutePanelPath);
                    if (File.Exists(absolutePanelPath))
                    {
                        response.StatusCode = (int)HttpStatusCode.OK;

                        buffer = File.ReadAllBytes(absolutePanelPath);

                        if (request.Url.AbsolutePath.EndsWith("html"))
                        {
                            Logging.Info($"Responsed Panel/Widget: '{request.Url.AbsolutePath}'");
                        }
                    }
                    else
                    {
                        buffer = Encoding.UTF8.GetBytes("Panel not found...");

                        if (request.Url.AbsolutePath.EndsWith("html"))
                        {
                            Logging.Warning($"Request of unkonwn Panel/Widget: '{request.Url.AbsolutePath}'");
                        }
                    }
                }
                else if (request.Url.AbsolutePath.StartsWith("/Icon/"))
                {
                    response.StatusCode = (int)HttpStatusCode.OK;

                    if (request.Url.AbsolutePath.EndsWith("svg")) {
                        response.ContentType = "image/svg+xml";
                    }
                    else
                    {
                        response.ContentType = "image/png";
                    }

                    Assembly assembly = Assembly.GetExecutingAssembly();
                    string resourceName = "Indabo.Host.Content.GUI.Frame" + request.Url.AbsolutePath.Replace('/', '.');
                    using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                    {
                        using (BinaryReader reader = new BinaryReader(stream))
                        {
                            buffer = reader.ReadBytes((int)stream.Length);
                        }
                    }
                }
                else if (request.Url.AbsolutePath == "/Frame.js")
                {
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.ContentType = "application/javascript";

                    Assembly assembly = Assembly.GetExecutingAssembly();
                    string resourceName = "Indabo.Host.Content.GUI.Frame.Frame.js";
                    using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                    {
                        using (BinaryReader reader = new BinaryReader(stream))
                        {
                            buffer = reader.ReadBytes((int)stream.Length);
                        }
                    }
                }
                else if (request.Url.AbsolutePath == "/Frame.css")
                {
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.ContentType = "text/css";

                    Assembly assembly = Assembly.GetExecutingAssembly();
                    string resourceName = "Indabo.Host.Content.GUI.Frame.Frame.css";
                    using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                    {
                        using (BinaryReader reader = new BinaryReader(stream))
                        {
                            buffer = reader.ReadBytes((int)stream.Length);
                        }
                    }
                }
                else
                {
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.ContentType = "text/html";

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

                try
                {
                    HttpListenerResponse response = context.Response;

                    response.StatusCode = (int)HttpStatusCode.NotFound;

                    response.ContentLength64 = 0;
                    Stream output = response.OutputStream;
                    output.Write(new byte[] { }, 0, 0);
                    output.Close();
                }
                catch (Exception)
                {
                    // Nothing to do...
                }
            }
        }

        public void Stop()
        {
            this.listener.Stop();

            this.running = false;
        }
    }
}
