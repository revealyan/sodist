using Grpc.Net.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Revealyan.Sodist.Sandbox.GRPCService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Revealyan.Sodist.Sandbox.ConsoleApp
{
    class Program
    {
        static string _libName = $@"Revealyan.Sodist.Sandbox.LibraryCore";
        static string _libPath = $@"D:\Development\src\Sandbox\Revealyan.Sodist.Sandbox.LibraryCore\bin\Debug\netcoreapp3.1\{_libName}.dll";
        static void Main(string[] args)
        {
            Console.WriteLine(JsonConvert.DeserializeObject<AuthConfiguration>(""));
            //TestJson();
            //TestGRPC();
            //TestLibLoad();
            Console.ReadKey();
        }
        static void TestJson()
        {
            string aPath = "a.json";
            string bPath = "b.json";
            void AWrite() => File.WriteAllText(aPath, JsonConvert.SerializeObject(new AuthConfiguration() { Type = AuthType.JWT, Parameters = new JWTParameters() { Key = "big key", Assign = "Assign user" } }));
            void BWrite() => File.WriteAllText(bPath, JsonConvert.SerializeObject(new AuthConfiguration() { Type = AuthType.Remote, Parameters = new RemoteParameters() { Urls = "http://localhost:50000" } }));
            AWrite();
            BWrite();
            var ca = JsonConvert.DeserializeObject<AuthConfiguration>(File.ReadAllText(aPath));
            var cb = JsonConvert.DeserializeObject<AuthConfiguration>(File.ReadAllText(bPath));
        }
        static void TestLibLoad()
        {
            var asm = Assembly.LoadFrom(_libPath);
            var asm3 = Assembly.LoadFrom(_libPath);
            var asm2 = Assembly.Load(_libName);
        }
        static void TestGRPC()
        {
            Greeter.GreeterClient g1 = new Greeter.GreeterClient(GrpcChannel.ForAddress("https://localhost:5001/"));
            Greeter2.Greeter2Client g2 = new Greeter2.Greeter2Client(GrpcChannel.ForAddress("https://localhost:5001/"));
            var b = true;
            while (b)
            {
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.F1:
                        Console.WriteLine(g1.SayHello(new HelloRequest() { Name = "G1" }).Message);
                        break;
                    case ConsoleKey.F2:
                        Console.WriteLine(g2.SayHello(new HelloRequest2() { Name = "G2" }).Message);
                        break;
                    case ConsoleKey.Escape:
                        b = false;
                        break;
                    default:
                        Console.WriteLine("ты долбаёб");
                        break;
                }
            }
        }
    }

    public abstract class AuthParameters
    {

    }
    public class RemoteParameters : AuthParameters
    {
        public string Urls { get; set; }
    }

    public class JWTParameters : AuthParameters
    {
        public string Key { get; set; }
        public string Assign { get; set; }
    }

    [JsonConverter(typeof(AuthConfigurationConverter))]
    public class AuthConfiguration
    {
        public AuthType Type { get; set; } = AuthType.Remote;
        public AuthParameters? Parameters { get; set; }
    }

    public enum AuthType
    {
        Remote,
        JWT
    }
    public class AuthParametersContractResolver : DefaultContractResolver
    {
        protected Type _base;
        public AuthParametersContractResolver(Type @base)
        {
            _base = @base;
        }
        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (_base.IsAssignableFrom(objectType) && !objectType.IsAbstract)
                return null; 
            return base.ResolveContractConverter(objectType);
        }
    }
    public class AuthConfigurationConverter : JsonConverter
    {
        protected JsonSerializerSettings _settings;
        protected Type _base;
        public AuthConfigurationConverter()
        {
            _base = typeof(AuthConfiguration);
            _settings = new JsonSerializerSettings() { ContractResolver = new AuthParametersContractResolver(typeof(AuthParameters)) };
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == _base;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);

            if (!Enum.TryParse(jo["Type"].ToString(), out AuthType authType))
            {
                throw new Exception("Не удалось определить тип аутентификации и авторизации");
            }

            var result = new AuthConfiguration()
            {
                Type = authType,
                Parameters = authType switch
                {
                    AuthType.JWT => JsonConvert.DeserializeObject<JWTParameters>(jo["Parameters"]?.ToString()),
                    AuthType.Remote => JsonConvert.DeserializeObject<RemoteParameters>(jo["Parameters"]?.ToString()),
                    _ => null,
                }
            };

            return result;
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException(); // won't be called because CanWrite returns false
        }
    }
    public class BaseSpecifiedConcreteClassConverter : DefaultContractResolver
    {
        protected Type _base;
        public BaseSpecifiedConcreteClassConverter(Type @base)
        {
            _base = @base;
        }
        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (_base.IsAssignableFrom(objectType) && !objectType.IsAbstract)
                return null; // pretend TableSortRuleConvert is not specified (thus avoiding a stack overflow)
            return base.ResolveContractConverter(objectType);
        }
    }
    public class BaseConverter : JsonConverter
    {
        protected static Dictionary<Type, List<Type>> _loadedType = new Dictionary<Type, List<Type>>();
        protected static Dictionary<Type, JsonSerializerSettings> _loadedSettings = new Dictionary<Type, JsonSerializerSettings>();
        protected Type _base;
        protected List<Type> _types;
        public BaseConverter(Type @base)
        {
            _base = @base;
            if (_loadedType.ContainsKey(_base))
            {
                _types = _loadedType[_base];
            }
            else
            {
                var g = AppDomain.CurrentDomain.GetAssemblies().Aggregate(new List<Type>(), (l, asm) => { l.AddRange(asm.GetTypes()); return l; });
                _types = AppDomain.CurrentDomain.GetAssemblies().Aggregate(new List<Type>(), (types, asm) =>
                {
                    types.AddRange(asm.GetTypes().Where(t => _base.IsAssignableFrom(t) && !t.IsAbstract));
                    return types;
                });
                _loadedType[_base] = _types;
                _loadedSettings[_base] = new JsonSerializerSettings() { ContractResolver = new BaseSpecifiedConcreteClassConverter(_base) };
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == _base;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            var props = jo.Properties().ToList();
            if (props.Count == 0)
                return null;
            var result = new Dictionary<Type, object>();
            var everythingFlag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            foreach (var child in _types)
            {
                object desObj = null;
                try
                {
                    desObj = JsonConvert.DeserializeObject(jo.ToString(), child, _loadedSettings[_base]);
                }
                finally
                {
                    result.Add(child, desObj);
                }

            }
            if(result.Count == 0)
            {
                throw new Exception($"Для типа \"{_base.FullName}\" не удалось найти дочерний класс подходящий под описание");
            }
            else if(result.Count > 1)
            {
                throw new Exception($"Для типа \"{_base.FullName}\" удалось найти несколько дочерних классов подходящих под описание: [{result.Aggregate(string.Empty, (aggrString, kvp) => $"\"{aggrString}\",{kvp.Key.FullName}")}]");
            }
            return result.First().Value;
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException(); // won't be called because CanWrite returns false
        }
    }
}
