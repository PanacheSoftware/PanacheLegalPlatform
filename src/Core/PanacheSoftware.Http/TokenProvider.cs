﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Http
{
    public class TokenProvider
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public class InitialApplicationState : TokenProvider
    {

    }
}
