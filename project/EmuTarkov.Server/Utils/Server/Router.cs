using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using ComponentAce.Compression.Libs.zlib;
using EmuTarkov.Common.Utils.App;

namespace EmuTarkov.Server.Utils.Server
{
    public class Router
    {
        private HttpListener router;
        private Dictionary<string, IResponse> staticRoutes;
        private Dictionary<string, IResponse> dynamicRoutes;

        public Router(string url)
        {
            new Uri(url);

            router = new HttpListener();
            staticRoutes = new Dictionary<string, IResponse>();
            dynamicRoutes = new Dictionary<string, IResponse>();

            if (!router.Prefixes.Contains(url))
            {
                router.Prefixes.Add(url);
            }
        }

        /// <summary>
        /// Start the server
        /// </summary>
        public void Start()
        {
            // execute server callbacks

            router.Start();
            Logger.LogInfo("EmuTarkov.Server: Router started");
        }

        /// <summary>
        /// Stops the server
        /// </summary>
        public void Stop()
        {
            // execute server callbacks

            router.Stop();
            Logger.LogInfo("EmuTarkov.Server: Router stopped");
        }

        /// <summary>
        /// Check if the server should be running
        /// </summary>
        public bool IsRunning()
        {
            return router.IsListening;
        }

        /// <summary>
        /// Add a static route to handle
        /// </summary>
        public void AddStaticRoute(string url, IResponse response)
        {
            staticRoutes.Add(url, response);
            Logger.LogInfo("EmuTarkov.Server: Router added static route " + url);
        }

        /// <summary>
        /// Add a dynamic route to handle
        /// </summary>
        public void AddDynamicRoute(string url, IResponse response)
        {
            dynamicRoutes.Add(url, response);
            Logger.LogInfo("EmuTarkov.Server: Router added dynamic route " + url);
        }

        /// <summary>
        /// Update the server
        /// </summary>
        public void Update()
        {
            HttpListenerContext ctx = router.GetContext();
            RequestInfo reqInfo = GetRequestInfo(ctx);

            Logger.LogInfo("EmuTarkov.Server: [" + reqInfo.Session + "] " + reqInfo.Uri.LocalPath);
            SendResponse(ctx, GetResponse(reqInfo));
        }

        /// <summary>
        /// Get the relevant request information
        /// </summary>
        private RequestInfo GetRequestInfo(HttpListenerContext ctx)
        {
            HttpListenerRequest req = ctx.Request;
            RequestInfo reqInfo = new RequestInfo(req.Url);

            // get body
            if (req.HasEntityBody)
            {
                using (Stream stream = req.InputStream)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        reqInfo.Body = SimpleZlib.Decompress(ms.ToArray(), null);
                    }
                }
            }
            
            // get session
            foreach (Cookie cookie in req.Cookies)
            {
                if (cookie.Name == "PHPSESSID")
                {
                    reqInfo.Session = cookie.Value;
                } 
            }

            return reqInfo;
        }

        /// <summary>
        /// Get the server response to a request
        /// </summary>
        private ResponseInfo GetResponse(RequestInfo reqInfo)
        {
            ResponseInfo respInfo = new ResponseInfo(null, false);

            if (staticRoutes.ContainsKey(reqInfo.Uri.LocalPath))
            {
                respInfo = staticRoutes[reqInfo.Uri.LocalPath].HandleResponse(reqInfo);
            }
            else
            {
                foreach (KeyValuePair<string, IResponse> pair in dynamicRoutes)
                {
                    if (reqInfo.Uri.LocalPath.Contains(pair.Key))
                    {
                        respInfo = pair.Value.HandleResponse(reqInfo);
                        break;
                    }
                }
            }

            return respInfo;
        }

        /// <summary>
        /// Send the server response
        /// </summary>
        private void SendResponse(HttpListenerContext ctx, ResponseInfo respInfo)
        {
            HttpListenerResponse resp = ctx.Response;

            if (respInfo.Body == null)
            {
                return;
            }

            // send response here
        }
    }
}
