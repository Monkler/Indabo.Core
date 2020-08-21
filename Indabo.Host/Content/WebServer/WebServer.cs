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
        private HttpListener listener;

        private bool running;

        public WebServer(int port, bool webServerPublicAccess = false)
        {
            this.listener = new HttpListener();

            if (webServerPublicAccess)
            {
                this.listener.Prefixes.Add($"http://*:{port}/");
            }
            else
            {
                this.listener.Prefixes.Add($"http://localhost:{port}/");
            }

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

                        Logging.Info($"New request: {context.Request.Url} ({ context.Request.UserHostName})");

                        new Thread(() =>
                        {
                            if (context.Request.IsWebSocketRequest)
                            {
                                WebSocketHandler.Instance.HandleWebSocketRequestAsync(context);
                            }
                            else
                            {
                                this.HandleCallback(context);
                            }
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
                    buffer = this.ResponseFile(response, "Indabo.Host.Content.GUI.Icon.Indabo.png", "image/png");
                }                
                else if (request.Url.AbsolutePath == "/Frame.js")
                {
                    buffer = this.ResponseFile(response, "Indabo.Host.Content.GUI.Frame.js", "application/javascript");
                }
                else if (request.Url.AbsolutePath == "/Frame.css")
                {
                    buffer = this.ResponseFile(response, "Indabo.Host.Content.GUI.Frame.css", "text/css");
                }
                else if (request.Url.AbsolutePath.StartsWith("/Icon/") ||
                    request.Url.AbsolutePath.StartsWith("/Font/"))
                {
                    string resourceName = "Indabo.Host.Content.GUI" + request.Url.AbsolutePath.Replace('/', '.');
                    buffer = this.ResponseFile(response, resourceName, ContentTypeParser.GetContentTypeFromFileName(request.Url.AbsolutePath));
                }
                else if (request.Url.AbsolutePath == "/Panels")
                {
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.ContentType = "application/json";

                    List<string> panels = DynamicHtmlLoader.GetFileNamesFromFolder(DynamicHtmlLoader.PANEL_FOLDER);

                    string json = JsonConvert.SerializeObject(panels, Formatting.None);
                    buffer = Encoding.UTF8.GetBytes(json);
                }
                else if (request.Url.AbsolutePath.StartsWith("/Panel/") || request.Url.AbsolutePath.StartsWith("/Widget/") || request.Url.AbsolutePath.StartsWith("/Library/"))
                {
                    if (request.Url.AbsolutePath.EndsWith("png"))
                    {
                        response.ContentType = "image/png";
                    }
                    else if (request.Url.AbsolutePath.EndsWith("js"))
                    {
                        response.ContentType = "application/javascript";
                    }
                    else
                    {
                        response.ContentType = "text/html";
                    }

                    string absolutePanelPath = Path.Combine(Config.ROOT_DIRECTORY, request.Url.AbsolutePath.Replace("/", "\\").TrimStart('\\'));
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
                else
                {
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.ContentType = "text/html";

                    Assembly assembly = Assembly.GetExecutingAssembly();
                    string resourceName = "Indabo.Host.Content.GUI.Frame.html";
                    using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string frame = reader.ReadToEnd();

                            string libraries = DynamicJavascriptLoader.LoadFromFolderAsHtmlTags(Config.ROOT_DIRECTORY, DynamicJavascriptLoader.LIBRARY_FOLDER);
                            string widgets = DynamicJavascriptLoader.LoadFromFolderAsHtmlTags(Config.ROOT_DIRECTORY, DynamicJavascriptLoader.WIDGET_FOLDER);

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

        private byte[] ResponseFile(HttpListenerResponse response, string resourceName, string contentType = "text/html")
        {
            response.StatusCode = (int)HttpStatusCode.OK;
            response.ContentType = contentType;

            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    return reader.ReadBytes((int)stream.Length);
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
