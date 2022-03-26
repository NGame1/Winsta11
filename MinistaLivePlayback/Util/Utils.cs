using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
//using Windows.Web.Http;

namespace MinistaLivePlayback.Util
{
    public class Downloader
    {
        private const int fileNotFound404Hresult = -2145844844;
        public static int RETRIES = 3;
        public async static Task<IBuffer> DownloadBufferAsync(Uri url)
        {
            //Turn off cache for live playback
            var filter = new Windows.Web.Http.Filters.HttpBaseProtocolFilter();
            filter.CacheControl.WriteBehavior = Windows.Web.Http.Filters.HttpCacheWriteBehavior.NoCache;
            using (var httpClient = new HttpClient())
            {
                try
                {
                    httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
                    {
                        NoCache = true
                    };
                }
                catch { }

                var result = await httpClient.SendAsync(GetRequest(HttpMethod.Get,url));
                if (result.IsSuccessStatusCode)
                {
#if DEBUG
                    Logger.Log("GetBufferAsync suceeded for url: " + url);
#endif
                    var buf = await result.Content.ReadAsByteArrayAsync();
                    return buf.AsBuffer();
                }
                else
                {
#if DEBUG
                    Logger.Log("GetBufferAsync exception for url: " + url + " HRESULT 0x");
#endif
                    return null;
                }
            }
        }

        public async static Task<HttpResponseMessage> SendHeadRequestAsync(Uri url)
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
                    {
                        NoCache = true
                    };
                }
                catch { }
                var result = await httpClient.SendAsync(GetRequest(HttpMethod.Get,url));
                if (result.IsSuccessStatusCode)
                {
#if DEBUG
                    Logger.Log("Sending head request with: " + url);
#endif
                    return result;
                }
                else
                {
#if DEBUG
                    Logger.Log("SendHeadRequestAsync exception for url: " + url + " HRESULT 0x");
#endif
                    return null;
                }
            }
        }

        static HttpRequestMessage GetRequest(HttpMethod method, Uri uri)
        {
            var request = new HttpRequestMessage(method, uri);
            request.Headers.Add("User-Agent", "Instagram 164.0.0.46.123 Android (26/8.0.0; 480dpi; 1080x1794; HUAWEI/HONOR; PRA-LA1; HWPRA-H; hi6250; en_US; 252055945)");
            return request;
        }
    }
#if DEBUG
    public class Log
    {
        string CurrentLog = "";
        public StorageFile sampleFile;

        public Log()
        {
            CreateFile();
        }

        private /*async*/ void CreateFile()
        { }
        //private async Task CreateFileAsync()
        //{
        //    StorageFolder folder = ApplicationData.Current.LocalFolder;
        //    sampleFile = await folder.CreateFileAsync("LiveDashLog.txt", CreationCollisionOption.ReplaceExisting);
        //}

        public /*async*/ void WriteToFile(string s)
        {
            try
            {
                //if (sampleFile == null)
                //   await CreateFileAsync();

                //await FileIO.AppendTextAsync(sampleFile, s);
            }
            catch (Exception)
            {
                
            }
        }

        public string GetLog()
        {
            lock (this)
            {
                return CurrentLog;
            } 

        }

        public void Append(string s)
        {
            CurrentLog += s;
            if (CurrentLog.Length > 10000)
            {
                CurrentLog = CurrentLog.Substring(CurrentLog.Length - 10000);
            }
        }

        public void Clear()
        {
            lock (this)
            {
                CurrentLog = "";
            }
        }

    }
    

    public class Logger
    {
        public static Log logger = new Log();

        public static void Log(string s)
        {
            logger.Append(s + "\r\n");
            logger.WriteToFile(s + "\r\n");
            System.Diagnostics.Debug.WriteLine(s);
        }

        public static StorageFile GetFileLog()
        {
            return logger.sampleFile;
        }

        public static string GetLog()
        {
            return logger.GetLog();
        }

        public static string Display(Exception e)
        {
            return "HRESULT: " + e.HResult.ToString("X8");
        }
    }
#endif
}
