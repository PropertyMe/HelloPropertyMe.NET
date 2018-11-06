namespace PM.Api.Host.Models
{
    public class ChangedContactsRequest : BaseRequest
    {
        /// <summary>
        ///     Records returned will have a changed timestamp greater than this value
        /// </summary>
        public long Timestamp { get; set; }
    }
}
