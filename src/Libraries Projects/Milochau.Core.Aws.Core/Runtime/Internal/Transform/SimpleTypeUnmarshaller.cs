/*
 * Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License").
 * You may not use this file except in compliance with the License.
 * A copy of the License is located at
 * 
 *  http://aws.amazon.com/apache2.0
 * 
 * or in the "license" file accompanying this file. This file is distributed
 * on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied. See the License for the specific language governing
 * permissions and limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using Amazon.Runtime.Internal.Util;
using Amazon.Util;
using ThirdParty.Json.LitJson;

namespace Amazon.Runtime.Internal.Transform
{
    static class SimpleTypeUnmarshaller<T>
    {
        public static T Unmarshall(JsonUnmarshallerContext context)
        {
            context.Read();
            string text = context.ReadText();
            if (text == null)
                return default(T);

            return (T)Convert.ChangeType(text, typeof(T), CultureInfo.InvariantCulture);
        }
    }

    /// <summary>
    /// Unmarshaller for int fields
    /// </summary>
    public class IntUnmarshaller : IUnmarshaller<int, JsonUnmarshallerContext>
    {
        private IntUnmarshaller() { }

        private static IntUnmarshaller _instance = new IntUnmarshaller();

        public static IntUnmarshaller Instance
        {
            get
            {
                return _instance;
            }
        }

        public int Unmarshall(JsonUnmarshallerContext context)
        {
            return SimpleTypeUnmarshaller<int>.Unmarshall(context);
        }
    }

    /// <summary>
    /// Unmarshaller for nullable int fields. Implemented only for JSON context
    /// to handle cases where value can be null e.g. {'Priority': null}.
    /// </summary>
    public class NullableIntUnmarshaller : IUnmarshaller<int?, JsonUnmarshallerContext>
    {
        private NullableIntUnmarshaller() { }

        public int? Unmarshall(JsonUnmarshallerContext context)
        {
            context.Read();
            string text = context.ReadText();

            if (text == null)
            {
                return null;
            }
            return int.Parse(text, CultureInfo.InvariantCulture);
        }
    }

    /// <summary>
    /// Unmarshaller for long fields
    /// </summary>
    public class LongUnmarshaller : IUnmarshaller<long, JsonUnmarshallerContext>
    {
        private LongUnmarshaller() { }

        private static LongUnmarshaller _instance = new LongUnmarshaller();

        public static LongUnmarshaller Instance
        {
            get
            {
                return _instance;
            }
        }

        public long Unmarshall(JsonUnmarshallerContext context)
        {
            return SimpleTypeUnmarshaller<long>.Unmarshall(context);
        }
    }

    /// <summary>
    /// Unmarshaller for float fields
    /// </summary>
    public class FloatUnmarshaller : IUnmarshaller<float, JsonUnmarshallerContext>
    {
        private FloatUnmarshaller() { }

        public float Unmarshall(JsonUnmarshallerContext context)
        {
            return SimpleTypeUnmarshaller<float>.Unmarshall(context);
        }
    }

    /// <summary>
    /// Unmarshaller for double fields
    /// </summary>
    public class DoubleUnmarshaller : IUnmarshaller<double, JsonUnmarshallerContext>
    {
        private DoubleUnmarshaller() { }

        private static DoubleUnmarshaller _instance = new DoubleUnmarshaller();

        public static DoubleUnmarshaller Instance
        {
            get
            {
                return _instance;
            }
        }

        public double Unmarshall(JsonUnmarshallerContext context)
        {
            return SimpleTypeUnmarshaller<double>.Unmarshall(context);
        }
    }

    /// <summary>
    /// Unmarshaller for bool fields
    /// </summary>
    public class BoolUnmarshaller : IUnmarshaller<bool, JsonUnmarshallerContext>
    {
        private BoolUnmarshaller() { }

        private static BoolUnmarshaller _instance = new BoolUnmarshaller();

        public static BoolUnmarshaller Instance
        {
            get
            {
                return _instance;
            }
        }

        public bool Unmarshall(JsonUnmarshallerContext context)
        {
            return SimpleTypeUnmarshaller<bool>.Unmarshall(context);
        }
    }

    /// <summary>
    /// Unmarshaller for string fields
    /// </summary>
    public class StringUnmarshaller : IUnmarshaller<string, JsonUnmarshallerContext>
    {
        private StringUnmarshaller() { }

        private static StringUnmarshaller _instance = new StringUnmarshaller();

        public static StringUnmarshaller Instance
        {
            get
            {
                return _instance;
            }
        }

        public static StringUnmarshaller GetInstance()
        {
            return StringUnmarshaller.Instance;
        }

        public string Unmarshall(JsonUnmarshallerContext context)
        {
            return SimpleTypeUnmarshaller<string>.Unmarshall(context);
        }
    }

    /// <summary>
    /// Unmarshaller for DateTime fields
    /// </summary>
    public class DateTimeUnmarshaller : IUnmarshaller<DateTime, JsonUnmarshallerContext>
    {
        private DateTimeUnmarshaller() { }

        private static DateTimeUnmarshaller _instance = new DateTimeUnmarshaller();

        public DateTime Unmarshall(JsonUnmarshallerContext context)
        {
            context.Read();
            string text = context.ReadText();
            return UnmarshallInternal(text, treatAsNullable: false).Value;
        }

        /// <summary>
        ///  Unmarshalls given string as a DateTime. Handles cases where we want to unmarshall 
        ///  as just a DateTime or a nullable Datetime.
        /// </summary>
        /// <param name="text">Value to be parsed</param>
        /// <param name="treatAsNullable">If true, the method will return null if text is null. 
        /// If false, the method will return default(DateTime), if text is null.</param>
        /// <returns></returns>
        internal static DateTime? UnmarshallInternal(string text, bool treatAsNullable)
        {
            Double seconds;
            if (Double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out seconds))
            {
                return AWSSDKUtils.EPOCH_START.AddSeconds(seconds);
            }
            else
            {
                if (text == null)
                {
                    if (treatAsNullable) { return null; }
                    else { return default(DateTime); }
                }

                return DateTime.Parse(text, CultureInfo.InvariantCulture);
            }
        }
    }

    /// <summary>
    /// Unmarshaller for MemoryStream fields
    /// </summary>
    public class MemoryStreamUnmarshaller : IUnmarshaller<MemoryStream, JsonUnmarshallerContext>
    {
        private MemoryStreamUnmarshaller() { }

        private static MemoryStreamUnmarshaller _instance = new MemoryStreamUnmarshaller();

        public static MemoryStreamUnmarshaller Instance
        {
            get
            {
                return _instance;
            }
        }

        public MemoryStream Unmarshall(JsonUnmarshallerContext context)
        {
            context.Read();
            if (context.CurrentTokenType == JsonToken.Null)
                return null;

            byte[] bytes = Convert.FromBase64String(context.ReadText());
            MemoryStream stream = new MemoryStream(bytes);
            return stream;
        }
    }

    /// <summary>
    /// Unmarshaller for ResponseMetadata
    /// </summary>
    public class ResponseMetadataUnmarshaller : IUnmarshaller<ResponseMetadata, JsonUnmarshallerContext>
    {
        private ResponseMetadataUnmarshaller() { }

        private static ResponseMetadataUnmarshaller _instance = new ResponseMetadataUnmarshaller();

        public ResponseMetadata Unmarshall(JsonUnmarshallerContext context)
        {
            ResponseMetadata metadata = new ResponseMetadata();
            int depth = context.CurrentDepth;

            while (context.CurrentDepth >= depth)
            {
                context.Read();
                if (context.TestExpression("ResponseMetadata/RequestId"))
                {
                    metadata.RequestId = StringUnmarshaller.GetInstance().Unmarshall(context);
                }
            }

            return metadata;
        }
    }

    public class KeyValueUnmarshaller<K, V, KUnmarshaller, VUnmarshaller> :
    IUnmarshaller<KeyValuePair<K, V>, JsonUnmarshallerContext>
        where KUnmarshaller :IUnmarshaller<K, JsonUnmarshallerContext>
        where VUnmarshaller : IUnmarshaller<V, JsonUnmarshallerContext>
    {
        private KUnmarshaller keyUnmarshaller;
        private VUnmarshaller valueUnmarshaller;

        public KeyValueUnmarshaller(KUnmarshaller keyUnmarshaller, VUnmarshaller valueUnmarshaller)
        {
            this.keyUnmarshaller = keyUnmarshaller;
            this.valueUnmarshaller = valueUnmarshaller;
        }

        public KeyValuePair<K, V> Unmarshall(JsonUnmarshallerContext context)
        {
            K key = this.keyUnmarshaller.Unmarshall(context);
            V value = this.valueUnmarshaller.Unmarshall(context);

            return new KeyValuePair<K, V>(key, value);
        }
    }

    public class ListUnmarshaller<I, IUnmarshaller> : IUnmarshaller<List<I>, JsonUnmarshallerContext>
        where IUnmarshaller : IUnmarshaller<I, JsonUnmarshallerContext>
    {
        private IUnmarshaller iUnmarshaller;

        public ListUnmarshaller(IUnmarshaller iUnmarshaller)
        {
            this.iUnmarshaller = iUnmarshaller;
        }

        public List<I> Unmarshall(JsonUnmarshallerContext context)
        {
            context.Read(); // Read [ or null
            if (context.CurrentTokenType == JsonToken.Null)
                return new List<I>();

            // If a list is present in the response, use AlwaysSendList,
            // so if the response was empty, reusing the object in the request we will
            // end up sending the same empty collection back.
            List<I> list = new AlwaysSendList<I>();
            while (!context.Peek(JsonToken.ArrayEnd)) // Peek for ]
            {
                list.Add(iUnmarshaller.Unmarshall(context));
            }
            context.Read(); // Read ]
            return list;
        }
    }

    public class DictionaryUnmarshaller<TKey, TValue, TKeyUnmarshaller, TValueUnmarshaller> : IUnmarshaller<Dictionary<TKey, TValue>, JsonUnmarshallerContext>
        where TKeyUnmarshaller : IUnmarshaller<TKey, JsonUnmarshallerContext>
        where TValueUnmarshaller : IUnmarshaller<TValue, JsonUnmarshallerContext>
    {
        private KeyValueUnmarshaller<TKey, TValue, TKeyUnmarshaller, TValueUnmarshaller> KVUnmarshaller;

        public DictionaryUnmarshaller(TKeyUnmarshaller kUnmarshaller, TValueUnmarshaller vUnmarshaller)
        {
            KVUnmarshaller = new KeyValueUnmarshaller<TKey, TValue, TKeyUnmarshaller, TValueUnmarshaller>(kUnmarshaller, vUnmarshaller);
        }                

        public Dictionary<TKey, TValue> Unmarshall(JsonUnmarshallerContext context)
        {
            context.Read(); // Read { or null
            if (context.CurrentTokenType == JsonToken.Null)
                return new Dictionary<TKey,TValue>();

            // If a dictionary is present in the response, use AlwaysSendDictionary,
            // so if the response was empty, reusing the object in the request we will
            // end up sending the same empty collection back.
            Dictionary<TKey, TValue> dictionary = new AlwaysSendDictionary<TKey, TValue>();
            while (!context.Peek(JsonToken.ObjectEnd)) // Peek }
            {
                KeyValuePair<TKey, TValue> item = KVUnmarshaller.Unmarshall(context);
                dictionary.Add(item.Key, item.Value);
            }
            context.Read(); // Read }
            return dictionary;

        }
    }
}
