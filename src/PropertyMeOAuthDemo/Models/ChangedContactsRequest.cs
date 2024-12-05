using System.ComponentModel.DataAnnotations;

namespace PropertyMeOAuthDemo.Models;

public class ChangedContactsRequest : BaseRequest
{
    /// <summary>
    ///     Records returned will have a changed timestamp greater than this value
    /// </summary>
    [Required(ErrorMessage = "Timestamp is required.")]
    public long Timestamp { get; set; }
}