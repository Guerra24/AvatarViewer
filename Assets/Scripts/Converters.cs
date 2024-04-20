using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace AvatarViewer
{
    public class Vector3JsonConverter : JsonConverter<Vector3>
    {
        public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (existingValue == null)
                existingValue = new();
            var obj = JObject.Load(reader);
            existingValue.x = obj.Value<float>("x");
            existingValue.y = obj.Value<float>("y");
            existingValue.z = obj.Value<float>("z");
            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("x");
            writer.WriteValue(value.x);
            writer.WritePropertyName("y");
            writer.WriteValue(value.y);
            writer.WritePropertyName("z");
            writer.WriteValue(value.z);
            writer.WriteEndObject();
        }
    }

    public class QuaternionJsonConverter : JsonConverter<Quaternion>
    {
        public override Quaternion ReadJson(JsonReader reader, Type objectType, Quaternion existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (existingValue == null)
                existingValue = new();
            var obj = JObject.Load(reader);
            existingValue.x = obj.Value<float>("x");
            existingValue.y = obj.Value<float>("y");
            existingValue.z = obj.Value<float>("z");
            existingValue.w = obj.Value<float>("w");
            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, Quaternion value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("x");
            writer.WriteValue(value.x);
            writer.WritePropertyName("y");
            writer.WriteValue(value.y);
            writer.WritePropertyName("z");
            writer.WriteValue(value.z);
            writer.WritePropertyName("w");
            writer.WriteValue(value.w);
            writer.WriteEndObject();
        }
    }
}
