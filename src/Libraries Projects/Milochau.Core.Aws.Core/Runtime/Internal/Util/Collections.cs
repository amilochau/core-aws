using System.Collections.Generic;

namespace Milochau.Core.Aws.Core.Runtime.Internal.Util
{
    /*
     * The following types were added to the SDK to solve two issues:
     * 1. The SDK always initializes collection types, so it was impossible
     * to determine if a given collection came back from the service as empty
     * or did not come back from the service at all.
     * 2. The SDK does not send empty collections to the service, so it was
     * impossible to send an empty collection as a value to a service.
     * (Specifically, it was impossible to store an empty list or empty map
     * in a DynamoDB attribute, something that the service began to support
     * in late 2014.)
     * 
     * Resolution:
     * We have added these collection types and the new Is[Name]Set property
     * which can be read (get) to see if the [Name] collection was present,
     * and can be written (set) to specify that the [Name] collection should
     * or should not be sent to the server. The logic for the new property
     * is as follows:
     * 
     * Get
     *   [Name] is not null AND
     *     [Name] is AlwaysSendList/AlwaysSendDictionary OR
     *     [Name].Count > 0
     *       => True
     *   Otherwise
     *       => False
     * 
     * Set
     *   True =>
     *     Set [Name] to new AlwaysSendList/AlwaysSendDictionary
     *   False =>
     *     Set [Name] to new List/Dictionary
    */

    /// <summary>
    /// A list object that will always be sent to AWS services,
    /// even if it is empty.
    /// The AWS .NET SDK does not send empty collections to services, unless
    /// the collection is of this type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class AlwaysSendList<T> : List<T>
    {
        public AlwaysSendList()
            : base() { }
        public AlwaysSendList(IEnumerable<T> collection)
            : base(collection ?? new List<T>()) { }
    }

    /// <summary>
    /// A dictionary object that will always be sent to AWS services,
    /// even if it is empty.
    /// The AWS .NET SDK does not send empty collections to services, unless
    /// the collection is of this type.
    /// </summary>
    internal class AlwaysSendDictionary<TKey, TValue> : Dictionary<TKey, TValue> where TKey: notnull
    {
        public AlwaysSendDictionary()
            : base() { }

        public AlwaysSendDictionary(IEqualityComparer<TKey> comparer)
            : base(comparer) { }

        public AlwaysSendDictionary(IDictionary<TKey, TValue> dictionary)
            : base(dictionary ?? new Dictionary<TKey,TValue>()) { }
    }
}
