namespace Milochau.Core.Aws.DynamoDB.Generator.Models
{
    internal enum AttributeType
    {
        String,
        Guid,
        Int,
        Long,
        Decimal,
        Double,
        Enum,
        Boolean,
        DateTimeOffset,
        DateTime, // @todo Not implemented

        Object,
    }
}
