using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TraitBasedOpinionSystem.Core;

namespace TraitBasedOpinionSystem.Unity
{
    public class OpinionSystemManager : MonoBehaviour
    {
        private OpinionSystem _opinionSystem = new OpinionSystem();

        public OpinionSystem OpinionSystem { get { return _opinionSystem; } }
    }
}


