﻿using backend.servicios.DTOs;

namespace backend.api.Models.ResponseModels
{
    public class CampaignResponseModel
    {
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int OrganizacionId { get; set; }

        public ICollection<NeedDto> Needs { get; set; } = new List<NeedDto>();
    }
}
