﻿using System;
using System.Collections.Generic;

namespace BitCoinManagerModels.BitCoin
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<Order> Orders { get; set; }
    }
}
