using System;
using ContentGeneration.Models;
using Newtonsoft.Json;
using UnityEngine;

namespace Sample.Common
{
    public static class ProfileSettings
    {
        public static string playerId
        {
            get
            {
                var ret = PlayerPrefs.GetString($"{nameof(ProfileSettings)}.{nameof(playerId)}", null);
                if (string.IsNullOrEmpty(ret))
                {
                    ret = Guid.NewGuid().ToString();
                    PlayerPrefs.SetString($"{nameof(ProfileSettings)}.{nameof(playerId)}", ret);
                }

                return ret;
            }
            set
            {
                if(value != playerId)
                {
                    PlayerPrefs.SetString($"{nameof(ProfileSettings)}.{nameof(playerId)}", value);
                }
            }
        }

        public static event Action<PublishedAsset> OnFlagChanged;
        public static PublishedAsset flag
        {
            get => DeserializeValue($"{nameof(ProfileSettings)}.{nameof(flag)}");
            set
            {
                PlayerPrefs.SetString($"{nameof(ProfileSettings)}.{nameof(flag)}", JsonConvert.SerializeObject(value));
                OnFlagChanged?.Invoke(value);
            }
        }
        public static event Action<PublishedAsset> OnShieldChanged;
        public static PublishedAsset shield
        {
            get => DeserializeValue($"{nameof(ProfileSettings)}.{nameof(shield)}");
            set
            {
                PlayerPrefs.SetString($"{nameof(ProfileSettings)}.{nameof(shield)}", JsonConvert.SerializeObject(value));
                OnShieldChanged?.Invoke(value);
            }
        }
        public static event Action<PublishedAsset> OnSwordChanged;
        public static PublishedAsset sword
        {
            get => DeserializeValue($"{nameof(ProfileSettings)}.{nameof(sword)}");
            set
            {
                PlayerPrefs.SetString($"{nameof(ProfileSettings)}.{nameof(sword)}", JsonConvert.SerializeObject(value));
                OnSwordChanged?.Invoke(value);
            }
        }
        public static event Action<PublishedAsset> OnBodyChanged;
        public static PublishedAsset body
        {
            get => DeserializeValue($"{nameof(ProfileSettings)}.{nameof(body)}");
            set
            {
                PlayerPrefs.SetString($"{nameof(ProfileSettings)}.{nameof(body)}", JsonConvert.SerializeObject(value));
                OnBodyChanged?.Invoke(value);
            }
        }

        static PublishedAsset DeserializeValue(string key)
        {
            var value = PlayerPrefs.GetString(key, null);
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            
            return JsonConvert.DeserializeObject<PublishedAsset>(value);
        }
    }
}