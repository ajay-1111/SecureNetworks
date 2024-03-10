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
    using Microsoft.AspNetCore.Identity;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The ApplicationUserDBEntity.
    /// </summary>
    public class ApplicationUserDBEntity : IdentityUser
    {
        [StringLength(100)]
        public string FirstName { get; set; }

       [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(100, MinimumLength = 10)]
        public string Telephone { get; set; }
        
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        public int? IsAdminUser { get; set; }
    }
}
