using WinstaNext.Core.Theme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using InstagramApiSharp.API.Builder;
using Newtonsoft.Json;
using Microsoft.Toolkit.Uwp.Helpers;
using System.Security.Cryptography;
using System.IO;
using InstagramApiSharp.Classes;
using InstagramApiSharp.API;
using WinstaNext.Models.Core;
using Windows.Globalization;
using System.Runtime.InteropServices.WindowsRuntime;
using WinstaNext.Helpers;

namespace WinstaNext
{
    internal class ApplicationSettingsManager
    {
        string SessionEncryptionKey { get => "%NGame_Winstagram.App@GRANOW11/=$%! "; }

        string UserSessionsFolderName { get => "UserSessions"; }

        string AppThemeSetting { get => "AppTheme"; }
        string AppLanguageSettings { get => "AppLanguage"; }
        string AutoPlaySettings { get => "AutoPlay"; }
        string RemoveFeedAdsSetting { get => "RemoveFeedAds"; }
        string ShowLoginSetting { get => "ShowLogin"; }
        string UserNamesSetting { get => "UserNames"; }

        StorageFolder LocalFolder { get; }
        ApplicationDataContainer LocalSettings { get; }
        ApplicationDataContainer RoamingSettings { get; }

        public static ApplicationSettingsManager Instance { get; }

        static ApplicationSettingsManager()
        {
            Instance = new ApplicationSettingsManager();
        }

        private ApplicationSettingsManager()
        {
            LocalFolder = ApplicationData.Current.LocalFolder;
            LocalSettings = ApplicationData.Current.LocalSettings;
            RoamingSettings = ApplicationData.Current.RoamingSettings;
        }

        public LanguageDefinition[] GetSupportedLanguages()
        {
            return new LanguageDefinition[]
            {
                new LanguageDefinition("Default",""),
                new LanguageDefinition("English","en-Us"),
                new LanguageDefinition("Persian (پارسی)","fa-Ir"),
            };
        }

        public string GetLanguage()
        {
            if (LocalSettings.Values.TryGetValue(AppLanguageSettings, out var lang))
            {
                return lang.ToString();
            }
            else
            {
                return SetLanguage();
            }
        }

        public string SetLanguage(string lang = "")
        {
            LocalSettings.Values[AppLanguageSettings] = lang;
            ApplicationLanguages.PrimaryLanguageOverride = lang;
            return lang;
        }

        public bool GetAutoPlay()
        {
            if (LocalSettings.Values.TryGetValue(AutoPlaySettings, out var autoplay))
            {
                return Convert.ToBoolean(autoplay);
            }
            else
            {
                return SetAutoPlay();
            }
        }

        public bool SetAutoPlay(bool enabled = true)
        {
            LocalSettings.Values[AutoPlaySettings] = enabled;
            return enabled;
        }

        public bool GetRemoveFeedAds()
        {
            if (LocalSettings.Values.TryGetValue(RemoveFeedAdsSetting, out var removeAds))
            {
                return (bool)removeAds;
            }
            else
            {
                return SetRemoveFeedAds();
            }
        }

        public bool SetRemoveFeedAds(bool removeFeedAds = false)
        {
            LocalSettings.Values[RemoveFeedAdsSetting] = removeFeedAds;
            return removeFeedAds;
        }

        public AppTheme GetTheme()
        {
            if (LocalSettings.Values.TryGetValue(AppThemeSetting, out var theme))
            {
                return Enum.Parse<AppTheme>(theme.ToString());
            }
            else
            {
                SetTheme(AppTheme.Default);
                return AppTheme.Default;
            }
        }

        public void SetTheme(AppTheme appTheme)
        {
            LocalSettings.Values[AppThemeSetting] = appTheme.ToString();
        }

        public Dictionary<string, string> GetUsersList()
        {
            if (LocalSettings.Values.TryGetValue(UserNamesSetting, out var users))
            {
                if (users != null)
                {
                    return JsonConvert.DeserializeObject<Dictionary<string, string>>(users.ToString());
                }
                else
                {
                    return SetUsersList();
                }
            }
            else
            {
                return SetUsersList();
            }
        }

        public async Task AddOrUpdateUser(long pk, string session, string username)
        {
            await AddOrUpdateUser(pk.ToString(), session, username);
        }

        async Task AddOrUpdateUser(string pk, string session, string username)
        {
            await SetUserSession(pk, session);
            var users = GetUsersList();
            if (!users.ContainsKey(pk))
                users.Add(pk, username);
            else users[pk] = username;
            SetUsersList(users);
        }

        private Dictionary<string, string> SetUsersList(Dictionary<string, string> users = null)
        {
            if (users == null) users = new Dictionary<string, string>();
            LocalSettings.Values[UserNamesSetting] = JsonConvert.SerializeObject(users);
            return users;
        }

        public bool GetShowLoginScreen()
        {
            var users = GetUsersList();
            if (users.Count == 0) return true;
            return false;
        }

        public async Task<string> GetUserSession(string userPk)
        {
            var folder = await LocalFolder.CreateFolderAsync(UserSessionsFolderName, CreationCollisionOption.OpenIfExists);
            try
            {
                var file = await folder.GetFileAsync(userPk);
                var str = await FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf8);
                return DecryptString(SessionEncryptionKey, str);
                //using (var read = await file.OpenReadAsync())
                //{
                //    var str = await read.ReadTextAsync(Encoding.UTF8);
                //    return DecryptString(SessionEncryptionKey, str);
                //}
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SetUserSession(long userPk, string session)
        {
            await SetUserSession(userPk.ToString(), session);
        }

        async Task SetUserSession(string userPk, string session)
        {
            var folder = await LocalFolder.CreateFolderAsync(UserSessionsFolderName, CreationCollisionOption.OpenIfExists);
            try
            {
                ((App)App.Current).SetCurrentUserSession(session);
                var file = await folder.CreateFileAsync(userPk, CreationCollisionOption.OpenIfExists);
                session = EncryptString(SessionEncryptionKey, session);
                await FileIO.WriteTextAsync(file, session, Windows.Storage.Streams.UnicodeEncoding.Utf8);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string EncryptString(string key, string plainInput)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(plainInput);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    plainInput = Convert.ToBase64String(ms.ToArray());
                }
            }
            return plainInput;
        }

        public string DecryptString(string key, string cipherText)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
    }
}
