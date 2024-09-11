﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using SpeedVechile.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedVechile.Domain.ViewModel
{
    public class PostVM
    {
        public Post Post { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> BrandList { get; set; }
        public IEnumerable<SelectListItem> VehicleTypeList { get; set; }
        public IEnumerable<SelectListItem> EngineAndFuelTypeList { get; set; }
        public IEnumerable<SelectListItem> TranmissionList { get; set; }

    }
}
