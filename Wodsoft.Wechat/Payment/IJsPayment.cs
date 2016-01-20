﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Wechat.Payment
{
    public interface IJsPayment : IPayment
    {
        string Nonce { get; }

        int TimeStamp { get; }

        string Signature { get; }
    }
}
