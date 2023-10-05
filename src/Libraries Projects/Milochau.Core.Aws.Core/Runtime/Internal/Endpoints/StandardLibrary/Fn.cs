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
using Amazon.Runtime.Endpoints;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ThirdParty.Json.LitJson;

namespace Amazon.Runtime.Internal.Endpoints.StandardLibrary
{
    /// <summary>
    /// Set of internal functions supported by ruleset conditions.
    /// </summary>
    public static class Fn
    {
        /// <summary>
        /// Evaluates whether a value (such as an endpoint parameter) is set
        /// </summary>
        public static bool IsSet(object value)
        {
            return value != null;
        }

        /// <summary>
        /// Extracts part of given object graph by path
        /// 
        /// Example: Given the input object {"Thing1": "foo", "Thing2": ["index0", "index1"], "Thing3": {"SubThing": 42}}
        /// GetAttr(object, "Thing1") returns "foo"
        /// path "Thing2[0]" returns "index0"
        /// path "Thing3.SubThing" returns 42
        /// Given the input IList list = {"foo", "bar"}
        /// GetAttr(list, "[0]") returns "foo"
        ///
        /// Every path segment must resolve to IPropertyBag
        /// Every path segment with indexer must resolve to IList
        /// Indexers must be at the very end of the path
        /// </summary>
        public static object GetAttr(object value, string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");

            var parts = path.Split('.');
            var propertyValue = value;
            
            for (int i = 0; i < parts.Length; i++)
            {
                var part = parts[i];

                // indexer is always at the end of path e.g. "Part1.Part2[3]"
                if (i == parts.Length - 1)
                {
                    var indexerStart = part.LastIndexOf('[');
                    var indexerEnd = part.Length - 1;

                    // indexer detected
                    if (indexerStart >= 0)
                    {
                        var propertyPath = part.Substring(0, indexerStart);
                        var index = int.Parse(part.Substring(indexerStart + 1, indexerEnd - indexerStart - 1));

                        // indexer can be passed directly as a path e.g. "[1]"
                        if (indexerStart > 0)
                        {
                            propertyValue = ((IPropertyBag)propertyValue)[propertyPath];
                        }

                        if (!(propertyValue is IList)) throw new ArgumentException("Object addressing by pathing segment '{part}' with indexer must be IList");

                        var list = (IList)propertyValue;
                        if (index < 0 || index > list.Count - 1) 
                        {
                            return null;
                        }
                        return list[index];
                    }
                }

                if (!(propertyValue is IPropertyBag)) throw new ArgumentException("Object addressing by pathing segment '{part}' must be IPropertyBag");
                propertyValue = ((IPropertyBag)propertyValue)[part];
            }

            return propertyValue;
        }

        /// <summary>
        /// Returns partition data for a region
        /// </summary>
        public static Partition Partition(string region)
        {
            return StandardLibrary.Partition.GetPartitionByRegion(region);
        }

        /// <summary>
        /// Interpolate template placeholders with values from "refs" dictionary.
        /// 
        /// e.g. Template "My url scheme is {url#scheme} for {region}",
        /// where "url" and "region" are keys in refs dictionary and "scheme" is property of object refs["url"].
        /// Uses GetAttr() to resolve {} placeholders, i.e. {object#prop1.prop2[3]} -> GetAttr(refs["object"], "prop1.prop2[3]").
        /// {{ and }} are considered as escape sequences to allow rule authors to output a literal { and } respectively.
        /// Throws ArgumentException if template is not well formed.
        /// </summary>
        public static string Interpolate(string template, Dictionary<string, object> refs)
        {
            const char OpenBracket = '{';
            const char CloseBracket = '}';
            var result = new StringBuilder();
            for (int i = 0; i < template.Length; i++)
            {
                var currentChar = template[i];
                char nextChar = (i < template.Length - 1) ? template[i + 1] : default(char);
                // translate {{ -> { and }} -> }
                if (currentChar == OpenBracket && nextChar == OpenBracket)
                {
                    result.Append(OpenBracket);
                    i++;
                    continue;
                }
                if (currentChar == CloseBracket && nextChar == CloseBracket)
                {
                    result.Append(CloseBracket);
                    i++;
                    continue;
                }
                // translate {object#path} -> value
                if (currentChar == OpenBracket)
                {
                    var placeholder = new StringBuilder();
                    while (i < template.Length - 1 && template[i + 1] != CloseBracket)
                    {
                        i++;
                        placeholder.Append(template[i]);
                    }
                    if (i == template.Length - 1)
                    {
                        throw new ArgumentException("template is missing closing }");
                    }
                    i++;
                    var refParts = placeholder.ToString().Split('#');
                    var refName = refParts[0];
                    if (refParts.Length > 1) // has path after #
                    {
                        result.Append(GetAttr(refs[refName], refParts[1]).ToString());
                    }
                    else
                    {
                        result.Append(refs[refName].ToString());
                    }
                }
                else if (currentChar == CloseBracket)
                {
                    throw new ArgumentException("template has non-matching closing bracket, use }} to output }");
                }
                else
                {
                    result.Append(currentChar);
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Interpolate all templates in all string nodes for given json
        /// </summary>
        public static string InterpolateJson(string json, Dictionary<string, object> refs)
        {
            var jsonObject = JsonMapper.ToObject(json);
            InterpolateJson(jsonObject, refs);
            return jsonObject.ToJson();
        }

        private static void InterpolateJson(JsonData json, Dictionary<string, object> refs)
        {
            if (json.IsString)
            {
                var jsonWrapper = (IJsonWrapper)json;
                jsonWrapper.SetString(Interpolate(jsonWrapper.GetString(), refs));
            }
            if (json.IsObject)
            {
                foreach (var key in json.PropertyNames)
                {
                    InterpolateJson(json[key], refs);
                }
            }
            if (json.IsArray)
            {
                foreach (JsonData item in json)
                {
                    InterpolateJson(item, refs);
                }
            }
        }
    }
}
