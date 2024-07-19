using System.Collections.Generic;

namespace Milochau.Core.Aws.ApiGateway
{
    /// <summary>List response</summary>
    public class ListResponse<TListModel, TKey>
    {
        /// <summary>Collection of items</summary>
        public ICollection<TListModel> Items { get; set; } = [];

        /// <summary>Whether the end of the list is reached</summary>
        public bool EndReached { get; set; }

        /// <summary>Key of the list item</summary>
        public TKey? LastKey { get; set; }
    }
}
