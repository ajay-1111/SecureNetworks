// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SecureNetworkDBContext.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Defines the SecureNetworkDBContext.cs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SecureNetworks.Models.DBModels;

namespace SecureNetworks.DataBaseContext
{
    /// <summary>The SecureNetworkDBContext.</summary>
    public class SecureNetworkDBContext : IdentityDbContext<ApplicationUserDBEntity>
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecureNetworkDBContext"/> class.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="configuration"></param>
        public SecureNetworkDBContext(DbContextOptions<SecureNetworkDBContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public virtual DbSet<ApplicationUserDBEntity> AspNetUsers { get; set; } = null!;
        
        public virtual DbSet<SNProductsEntity> tbl_SNProducts { get; set; } = null!;

        public virtual DbSet<SNProductCategoryEntity> tbl_SNProductsCategories { get; set; } = null!;

        public virtual DbSet<SNUserCartEntity> tbl_SNUserCartEntities { get; set; } = null!;
    }
}
