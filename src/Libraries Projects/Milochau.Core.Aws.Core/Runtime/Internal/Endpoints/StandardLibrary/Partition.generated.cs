/*******************************************************************************
 *  Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
 *  Licensed under the Apache License, Version 2.0 (the "License"). You may not use
 *  this file except in compliance with the License. A copy of the License is located at
 *
 *  http://aws.amazon.com/apache2.0
 *
 *  or in the "license" file accompanying this file.
 *  This file is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
 *  CONDITIONS OF ANY KIND, either express or implied. See the License for the
 *  specific language governing permissions and limitations under the License.
 * *****************************************************************************
 *    __  _    _  ___
 *   (  )( \/\/ )/ __)
 *   /__\ \    / \__ \
 *  (_)(_) \/\/  (___/
 *
 *  AWS SDK for .NET
 *
 */
namespace Amazon.Runtime.Internal.Endpoints.StandardLibrary
{
    /// <summary>
    /// Generated implementation of partition-specific data.
    /// Based on the data from partitions.json
    /// </summary>
    public partial class Partition
    {
        static Partition()
        {
            var aws = new PartitionAttributesShape
            {
                name = "aws",
                dnsSuffix = "amazonaws.com",
                dualStackDnsSuffix = "api.aws",
                supportsFIPS = true,
                supportsDualStack = true,
                implicitGlobalRegion = "us-east-1"
            };
            _partitionsByRegex.Add(@"^(us|eu)\-\w+\-\d+$", aws);
            _partitionsByRegionName.Add("aws-global", aws);
            _partitionsByRegionName.Add("eu-west-3", aws);
            _partitionsByRegionName.Add("us-east-1", aws);

            _defaultPartition = aws;
        }
    }
}