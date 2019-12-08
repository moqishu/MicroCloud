using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace LIN.MSA.Infrastructure
{
    public static class JsonUtil
    {
        public static readonly string JSON_DATETIME_FORMAT = "yyyy-MM-dd'T'HH:mm:ss";

        public static readonly string JSON_DATETIME_FORMAT2 = "yyyy-MM-dd HH:mm:ss";

        public static JsonSerializerSettings SerializerSettings
        {
            get
            {
                var settings = new JsonSerializerSettings();
                //settings.ContractResolver = new LowercaseContractResolver();
                settings.Converters.Add(new CustomerDateTimeConverter());
                settings.Converters.Add(new UpperCaseStringEnumConverter());
                settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //settings.Converters.Add(new RawJsonStringConverter(typeof(RawJsonString)));
                //settings.Converters.Add(new IsoDateTimeConverter());
                settings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                return settings;
            }
        }

        /// <summary>
        /// 对象转Json字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson(this object obj)
        {
            return obj.ToJson(false, false);
        }

        /// <summary>
        /// 对象转Json字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="indented"></param>
        /// <param name="ignoreNull"></param>
        /// <returns></returns>
        public static string ToJson(this object obj, bool indented, bool ignoreNull)
        {
            var settings = SerializerSettings;
            if (ignoreNull)
            {
                settings.NullValueHandling = NullValueHandling.Ignore;
                settings.MissingMemberHandling = MissingMemberHandling.Ignore;
                settings.DefaultValueHandling = DefaultValueHandling.Ignore;
            }

            return JsonConvert.SerializeObject(obj, indented ? Formatting.Indented : Formatting.None, settings);
        }

        /// <summary>
        /// Json字符串转对象
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object JsonToObject(this string value, Type type)
        {
            if (type == typeof(string))
                return value;
            return JsonConvert.DeserializeObject(value ?? "", type, SerializerSettings);
        }

        /// <summary>
        /// Json字符串转对象
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T JsonToObject<T>(this string value)
        {
            return JsonConvert.DeserializeObject<T>(value ?? "");
            //return JsonConvert.DeserializeObject<T>(value, SerializerSettings);
        }

    }

    public class CustomerDateTimeConverter : DateTimeConverterBase
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                if ((objectType == typeof(DateTime?)))
                    return null;
                else
                    throw new JsonSerializationException("Cannot convert null value to DateTime");
            }
            if (reader.TokenType == JsonToken.Date)
            {
                return reader.Value;
            }
            if (reader.TokenType != JsonToken.String)
            {
                throw new JsonSerializationException(string.Format("Unexpected token parsing date. Expected String, got {0}.", reader.TokenType));
            }
            string str = reader.Value.ToString();
            if (string.IsNullOrEmpty(str))
            {
                return DateTime.MinValue;
            }
            try
            {
                if (str.Contains("T"))
                    return DateTime.ParseExact(str, JsonUtil.JSON_DATETIME_FORMAT, CultureInfo.InvariantCulture);
                return DateTime.ParseExact(str, JsonUtil.JSON_DATETIME_FORMAT2, CultureInfo.InvariantCulture);
            }
            catch (FormatException formatException)
            {
                throw new JsonSerializationException("DateFormat must be: " + JsonUtil.JSON_DATETIME_FORMAT, formatException);
            }
        }

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (value == null)
                return;
            if (!(value is DateTime))
                throw new JsonSerializationException("Unexpected value when converting date. Expected DateTime");
            DateTime time = (DateTime)value;
            writer.WriteValue(time.ToString(JsonUtil.JSON_DATETIME_FORMAT, CultureInfo.InvariantCulture));
        }
    }

    public class UpperCaseStringEnumConverter : StringEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                Enum enum2 = (Enum)value;
                string first = enum2.ToString("G");
                if (char.IsNumber(first[0]) || (first[0] == '-'))
                {
                    writer.WriteValue(value);
                }
                else
                {
                    writer.WriteValue(first.ToUpper());
                }
            }
        }
    }
}