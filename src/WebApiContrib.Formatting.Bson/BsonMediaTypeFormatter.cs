﻿using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System;
using System.Net;
using System.IO;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace WebApiContrib.Formatting.Bson
{
    public class BsonMediaTypeFormatter : MediaTypeFormatter
    {
        private JsonSerializerSettings _jsonSerializerSettings;
        private const string bsonMediaType = "application/bson";

        public BsonMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(bsonMediaType));
            _jsonSerializerSettings = CreateDefaultSerializerSettings();
        }

        public JsonSerializerSettings SerializerSettings
        {
            get { return _jsonSerializerSettings; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Serializer cannot be null");
                }

                _jsonSerializerSettings = value;
            }
        }

        public JsonSerializerSettings CreateDefaultSerializerSettings()
        {
            return new JsonSerializerSettings()
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                TypeNameHandling = TypeNameHandling.None
            };
        }

        public override bool CanReadType(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            return true;
        }

        public override bool CanWriteType(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            return true;
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream stream, HttpContent content, IFormatterLogger formatterLogger)
        {
            var tcs = new TaskCompletionSource<object>();
            if (content != null && content.Headers.ContentLength == 0) return null;

            try
            {
                var reader = new BsonReader(stream);

                if (typeof(IEnumerable).IsAssignableFrom(type)) reader.ReadRootValueAsArray = true;

                using (reader)
                {
                    var jsonSerializer = JsonSerializer.Create(_jsonSerializerSettings);
                    var output = jsonSerializer.Deserialize(reader, type);
                    tcs.SetResult(output);
                }
            }
            catch (Exception e)
            {
                tcs.SetResult(GetDefaultValueForType(type));
            }

            return tcs.Task;
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream stream, HttpContent content, TransportContext transportContext)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (stream == null) throw new ArgumentNullException("stream");

            var tcs = new TaskCompletionSource<object>();

            using (var bsonWriter = new BsonWriter(stream) { CloseOutput = false })
            {
                var jsonSerializer = JsonSerializer.Create(_jsonSerializerSettings);
                jsonSerializer.Serialize(bsonWriter, value);
                bsonWriter.Flush();
                tcs.SetResult(null);
            }

            return tcs.Task;
        }
    }
}