using System;
using System.Collections.Generic;
using System.Text;

namespace Milochau.Core.Aws.DynamoDB.Generator.Models
{
    internal enum AttributeCategory
    {
        Regular,
        Partition,
        Sort,
    }
}
