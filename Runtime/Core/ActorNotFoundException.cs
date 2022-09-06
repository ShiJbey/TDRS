using System;

namespace TraitBasedOpinionSystem.Core
{
    public class ActorNotFoundException : Exception
    {
        public ActorNotFoundException(uint actorId)
            : base($"No Actor found with ID: ({actorId})")
        {
        }
    }
}

