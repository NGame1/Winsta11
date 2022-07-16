using CacheManager.Core;
using Windows.Storage;
using WinstaCore.Interfaces.Views;

namespace WinstaCore.Helpers.Cache
{
    public static class CacheManager
    {
        static StorageFolder TempFolder { get => ApplicationData.Current.TemporaryFolder; }

        private static ICacheManager<object> DataCache { get; set; }

        public static void InitializeCache()
        {
            DataCache = CacheFactory.Build<object>(p => p.WithDictionaryHandle());
        }

        public static object Add(string Key, object Value) => DataCache.AddOrUpdate(Key, Value, x => Value);

        public static object Get(string Key) => DataCache.Get(Key);

        public static void ClearCache() => DataCache.Clear();

        public static bool RemoveCache(string Key) => DataCache.Remove(Key);


        public static void AddPageCache<T>(T view, string accessKey) where T : IView
        {

        }
    }
}
