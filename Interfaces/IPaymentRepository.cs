﻿using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IPaymentRepository
    {
        public Payment SavePayment(Payment payment);


    }
        
        
}
