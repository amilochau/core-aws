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
    /// Destination Marshaller
    /// </summary>
    public class DestinationMarshaller : IRequestMarshaller<Destination> 
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <param name="requestObject"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public void Marshall(Destination requestObject, JsonWriter writer)
        {
            if(requestObject.IsSetBccAddresses())
            {
                writer.WritePropertyName("BccAddresses");
                writer.WriteArrayStart();
                foreach(var requestObjectBccAddressesListValue in requestObject.BccAddresses)
                {
                        writer.Write(requestObjectBccAddressesListValue);
                }
                writer.WriteArrayEnd();
            }

            if(requestObject.IsSetCcAddresses())
            {
                writer.WritePropertyName("CcAddresses");
                writer.WriteArrayStart();
                foreach(var requestObjectCcAddressesListValue in requestObject.CcAddresses)
                {
                        writer.Write(requestObjectCcAddressesListValue);
                }
                writer.WriteArrayEnd();
            }

            if(requestObject.IsSetToAddresses())
            {
                writer.WritePropertyName("ToAddresses");
                writer.WriteArrayStart();
                foreach(var requestObjectToAddressesListValue in requestObject.ToAddresses)
                {
                        writer.Write(requestObjectToAddressesListValue);
                }
                writer.WriteArrayEnd();
            }

        }

        /// <summary>
        /// Singleton Marshaller.
        /// </summary>
        public readonly static DestinationMarshaller Instance = new DestinationMarshaller();

    }
}