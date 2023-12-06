using System;
using System.Collections.Generic;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;


namespace TDRS
{
    /// <summary>
    /// This is the main entry point and overall manager for all information related
    /// to the Trait-Driven Relationship System.
    /// 
    /// This MonoBehaviour is responsible for managing information about all the
    /// traits, preconditions, and effects.
    /// </summary>
    public class RelationshipSystemManager : MonoBehaviour
    {
        protected TraitLibrary _traitLibrary = new TraitLibrary();
        protected EffectLibrary _effectLibrary = new EffectLibrary();
        protected PreconditionLibrary _preconditionLibrary = new PreconditionLibrary();

        public List<TextAsset> traitFiles = new List<TextAsset>();

        public TraitLibrary TraitLibrary => _traitLibrary;
        public EffectLibrary EffectLibrary => _effectLibrary;
        public PreconditionLibrary PreconditionLibrary => _preconditionLibrary;

        private void Awake()
        {
            foreach (var t in traitFiles)
            {
                LoadTraits(t);
            }
        }

        private void LoadTraits(TextAsset traitFile)
        {
            var fileText = traitFile.text;

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)  // see height_in_inches in sample yml 
                .Build();

        }

        public void AddSocialEntity()
        {
            throw new NotImplementedException();
        }

        public SocialRelationship AddRelationship(SocialEntity a, SocialEntity b)
        {
            throw new NotImplementedException();
        }

        public bool RelationshipExists(SocialEntity a, SocialEntity b)
        {
            return false;
        }

        public SocialRelationship GetRelationship(SocialEntity a, SocialEntity b)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SocialRelationship> GetIncomingRelationships(SocialEntity a)
        {
            return new SocialRelationship[0];
        }

        public IEnumerable<SocialRelationship> GetOutgoingRelationships(SocialEntity a)
        {
            return new SocialRelationship[0];
        }
    }
}
