using AdvancedClipboard.Web.Models.Identity;
using System.ComponentModel.DataAnnotations;

namespace AdvancedClipboard.Web.Models
{
    /// <summary>
    /// A clipboard lane.
    /// </summary>
    public class LaneEntity
    {
        #region Properties

        /// <summary>
        /// The color.
        /// </summary>
        public string Color { get; set; } = string.Empty;

        /// <summary>
        /// The id.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// The name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The owner.
        /// </summary>
        public ApplicationUser? User { get; set; }

        /// <summary>
        /// The owners id.
        /// </summary>
        public Guid UserId { get; set; }

        #endregion Properties
    }
}
