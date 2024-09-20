using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using TestApp.Models;

namespace TestApp.JsonConverters
{
    public class OrderJsonConverter : JsonConverter<Order>
    {
        public override Order? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Order value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString("id", value.Id.ToString());
            writer.WriteString("number", value.Number == null ? "null" : value.Number.ToString());
            writer.WriteString("date", value.Date == null ? "null" : value.Date.ToString()); 
            writer.WriteString("providerid", value.ProviderID.ToString());

            writer.WriteEndObject();
        }
    }
}
