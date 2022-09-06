using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TraitBasedOpinionSystem.Core
{
    public abstract class TraitValue
    {
        public abstract float AsFloat();

        public abstract int AsInt();

    }

    public class NumberTrait : TraitValue
    {
        protected float _value;

        public NumberTrait(int value)
        {
            _value = value;
        }

        public override float AsFloat()
        {
            return _value;
        }

        public override int AsInt()
        {
            return (int)_value;
        }
    }
    
    public class DefaultTrait: TraitValue
    {
        public DefaultTrait()
        {
        }

        public override float AsFloat()
        {
            return 1;
        }

        public override int AsInt()
        {
            return 1;
        }
    }
}

