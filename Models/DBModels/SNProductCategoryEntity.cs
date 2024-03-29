﻿using System.ComponentModel.DataAnnotations;

namespace SecureNetworks.Models.DBModels
{
    public class SNProductCategoryEntity
    {
        [Key]
        public int SNCategoryID { get; set; }
        
        public string SNCategoryName { get; set; }

        public string SNCategoryDescription { get; set; }

        public DateTime SNCategoryCreatedDateTime { get; set; }

        public DateTime SNCategoryModifiedDateTime { get; set; }
    }
}
