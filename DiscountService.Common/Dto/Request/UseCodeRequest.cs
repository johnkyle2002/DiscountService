﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscountService.Common.Dto.Request
{
    public class UseCodeRequest
    {
        public required string Code { get; set; }
    }
}
