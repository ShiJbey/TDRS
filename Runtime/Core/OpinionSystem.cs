using System;
using System.Collections.Generic;

namespace TraitBasedOpinionSystem.Core
{
    /// <summary>
    /// OpinionSystem is the entry-point for the library and
    /// the core object that users interface with to manage
    /// chaacter opinions, traits, and relationship modifiers.
    /// </summary>
    public class OpinionSystem
    {

        private uint _nextActorId;
        private DirectedGraph<Actor, Opinion> _opinionGraph;

        public OpinionSystem()
        {
            _nextActorId = 0;
            _opinionGraph = new DirectedGraph<Actor, Opinion>();
        }

        /// <summary>
        /// Add a new character to the system
        /// </summary>
        /// <returns>Unique Identifier for the character</returns>
        public Actor SpawnActor()
        {
            var actorId = ++_nextActorId;
            var actor = new Actor(actorId);
            _opinionGraph.AddNode(actorId, actor);

            return actor;
        }

        /// <summary>
        /// Remove a character from the system
        /// </summary>
        /// <param name="actor"></param>
        public void RemoveActor(Actor actor)
        {
            _opinionGraph.RemoveNode(actor.ID);
        }


        /// <summary>
        /// Get Actor
        /// </summary>
        /// <param name="actor"></param>
        /// <returns>Actor with given ID</returns>
        public Actor GetActor(uint actor)
        {
            return _opinionGraph.GetNode(actor);
        }

    
        public Opinion GetOpinion(Actor subject, Actor target)
        {
            // Return existing opinion
            if (!_opinionGraph.HasEdge(subject.ID, target.ID))
            {
                _opinionGraph.AddEdge(subject.ID, target.ID, new Opinion(subject, target));
            }

            // Create new opinion
            return _opinionGraph.GetEdge(subject.ID, target.ID);
        }
    }
}
