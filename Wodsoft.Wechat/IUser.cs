﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Wechat
{
    public interface IUser : IOpenId
    {
        string Scope { get; }

        string UnionId { get; }
    }
}
