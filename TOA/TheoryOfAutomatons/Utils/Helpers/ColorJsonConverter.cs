using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheoryOfAutomatons.Utils.Helpers
{
    internal class ColorJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Color);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var color = (Color)value;
            var obj = new JObject();

            if (color.IsNamedColor)
            {
                obj["Name"] = color.Name;
            }
            else
            {
                obj["A"] = color.A;
                obj["R"] = color.R;
                obj["G"] = color.G;
                obj["B"] = color.B;
            }

            obj.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                                       JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);

            if (obj["Name"] != null)
            {
                return Color.FromName(obj["Name"].ToString());
            }
            else
            {
                byte a = obj["A"]?.Value<byte>() ?? 255;
                byte r = obj["R"].Value<byte>();
                byte g = obj["G"].Value<byte>();
                byte b = obj["B"].Value<byte>();

                return Color.FromArgb(a, r, g, b);
            }
        }
    }
}
