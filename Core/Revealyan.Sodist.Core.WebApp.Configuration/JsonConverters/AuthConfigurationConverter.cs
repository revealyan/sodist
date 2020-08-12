using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Revealyan.Sodist.Core.WebApp.Configuration.JsonConverters
{
    public class AuthConfigurationConverter : JsonConverter
    {
        protected Type _base;
        public AuthConfigurationConverter()
        {
            _base = typeof(AuthConfiguration);
        }

        public override bool CanConvert(Type objectType) => objectType == _base;

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);

            if (!Enum.TryParse(jo["Type"]!.ToString(), out AuthType authType))
            {
                throw new Exception("Не удалось определить тип аутентификации и авторизации");
            }

            var result = new AuthConfiguration()
            {
                Type = authType,
                Parameters = authType switch
                {
                    AuthType.JWT => JsonConvert.DeserializeObject<JWTParameters>(jo["Parameters"]?.ToString() ?? string.Empty),
                    AuthType.Remote => JsonConvert.DeserializeObject<RemoteParameters>(jo["Parameters"]?.ToString() ?? string.Empty),
                    _ => null,
                }
            };

            return result;
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) => throw new NotImplementedException();
    }
}
