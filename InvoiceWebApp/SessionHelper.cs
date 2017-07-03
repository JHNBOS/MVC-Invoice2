using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Text;

namespace InvoiceWebApp {

    public static class SessionHelper {

        public static void Set(this ISession session, string key, object value) {
            var settings = new JsonSerializerSettings();
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            settings.PreserveReferencesHandling = PreserveReferencesHandling.All;

            JsonConvert.DefaultSettings = () => settings;

            string json = JsonConvert.SerializeObject(value);
            byte[] serializedResult = System.Text.Encoding.UTF8.GetBytes(json);

            session.Set(key, serializedResult);
        }

        public static T Get<T>(this ISession session, string key) {
            var value = session.Get(key);
            string json = Encoding.UTF8.GetString(value);

            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(json);
        }

        public static bool IsExists(this ISession session, string key) {
            return session.Get(key) != null;
        }
    }
}