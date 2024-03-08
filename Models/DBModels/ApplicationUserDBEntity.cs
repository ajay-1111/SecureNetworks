// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationUserDBEntity.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Defines the RTBFLogicGiftCardsTests.cs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SecureNetworks.Models.DBModels
{
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// The ApplicationUserDBEntity.
    /// </summary>
    public class ApplicationUserDBEntity : IdentityUser
    {
        /// <summary>Gets or sets the FirstName.</summary>
        [StringLength(100, ErrorMessage = "FirstName is Mandatory.", MinimumLength = 1)]
        [Required]
        public string FirstName { get; set; }

        /// <summary>Gets or sets the LastName.</summary>
        [StringLength(100, ErrorMessage = "LastName is Mandatory.", MinimumLength = 1)]
        [Required]
        public string LastName { get; set; } = string.Empty;

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [StringLength(10, ErrorMessage = "Telephone is Mandatory.", MinimumLength = 10)]
        [Required]
        public string Telephone { get; set; }
    }
}
