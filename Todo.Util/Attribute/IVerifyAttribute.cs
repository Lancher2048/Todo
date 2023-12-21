﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo.Commons
{
    public interface IVerifyAttribute
    {
        bool Verify(Type type, object obj, out string message);
    }
}
