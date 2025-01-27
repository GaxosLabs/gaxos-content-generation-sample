namespace ContentGeneration.Models
{
    public record QueryParameters
    {
        public uint Limit = 100;
        public uint Offset = 0;

        public enum SortTarget
        {
            Id, CreatedAt
        }
        public enum SortDirection
        {
            Ascending, Descending
        }
        public record SortBy
        {
            public SortTarget Target;
            public SortDirection Direction;
        }
        public SortBy[] Sort;
        
        public string FilterByPlayerId = null;
        public string FilterByAssetType = null;
    }
}