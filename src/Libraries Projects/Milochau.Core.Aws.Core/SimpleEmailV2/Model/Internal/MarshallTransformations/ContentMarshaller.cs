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

/*
 * Do not modify this file. This file is generated from the sesv2-2019-09-27.normal.json service model.
 */
using Amazon.Runtime.Internal.Transform;

namespace Amazon.SimpleEmailV2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// Content Marshaller
    /// </summary>
    public class ContentMarshaller : IRequestMarshaller<Content, JsonMarshallerContext> 
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <param name="requestObject"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public void Marshall(Content requestObject, JsonMarshallerContext context)
        {
            if(requestObject.IsSetCharset())
            {
                context.Writer.WritePropertyName("Charset");
                context.Writer.Write(requestObject.Charset);
            }

            if(requestObject.IsSetData())
            {
                context.Writer.WritePropertyName("Data");
                context.Writer.Write(requestObject.Data);
            }

        }

        /// <summary>
        /// Singleton Marshaller.
        /// </summary>
        public readonly static ContentMarshaller Instance = new ContentMarshaller();

    }
}