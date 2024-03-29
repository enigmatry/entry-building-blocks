﻿using System;

namespace Enigmatry.Entry.Randomness.Generators
{
    public sealed class RandomDoubleGenerator : BaseRandomGenerator
    {
        public RandomDoubleGenerator() : base(typeof(double)) { }

        public override dynamic Generate()
        {
            var bytes = GenerateByteArray();
            return BitConverter.ToDouble(bytes, 0);
        }
    }
}
