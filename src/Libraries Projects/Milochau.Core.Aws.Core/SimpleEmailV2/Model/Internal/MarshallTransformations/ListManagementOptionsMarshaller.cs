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
using ThirdParty.Json.LitJson;

namespace Amazon.SimpleEmailV2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// ListManagementOptions Marshaller
    /// </summary>
    public class ListManagementOptionsMarshaller : IRequestMarshaller<ListManagementOptions> 
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <param name="requestObject"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public void Marshall(ListManagementOptions requestObject, JsonWriter writer)
        {
            if(requestObject.IsSetContactListName())
            {
                writer.WritePropertyName("ContactListName");
                writer.Write(requestObject.ContactListName);
            }

            if(requestObject.IsSetTopicName())
            {
                writer.WritePropertyName("TopicName");
                writer.Write(requestObject.TopicName);
            }

        }

        /// <summary>
        /// Singleton Marshaller.
        /// </summary>
        public readonly static ListManagementOptionsMarshaller Instance = new ListManagementOptionsMarshaller();

    }
}