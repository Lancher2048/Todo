using System.Text.Json;
using Todo.Commons.Extensions;

namespace Todo.Commons.Converter
{
    public class DecimalJsonConverter : JsonConverter<decimal>
    {
        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
                return reader.GetDecimal();

            return reader.GetString().Equals("") ? "0.00".ToDecimal() : reader.GetString().ToDecimal();
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
