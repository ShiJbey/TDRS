namespace TraitBasedOpinionSystem
{
    /// <summary>
    /// TraitTypes manage metadata associated with Traits.
    ///
    /// TraitTypes create instances of traits and allow the
    /// OpinionSystem to save memory by only having a single
    /// copy of trait metadata.
    /// </summary>
    public class Trait
    {
        protected readonly string _name;
        protected readonly string _description;
        protected readonly ISocialRule[] _rules;

        public Trait(string name)
        {
            _name = name;
            _description = "";
            _rules = new ISocialRule[0];
        }

        public Trait(string name, ISocialRule[] rules)
        {
            _name = name;
            _description = "";
            _rules = rules;
        }

        public Trait(string name, string description)
        {
            _name = name;
            _description = description;
            _rules = new ISocialRule[0];
        }

        public Trait(string name, string description, ISocialRule[] rules)
        {
            _name = name;
            _description = description;
            _rules = rules;
        }

        public string Name
        {
            get { return _name; }
        }

        public string Description
        {
            get { return _description; }
        }

        public ISocialRule[] Rules
        {
            get { return _rules; }
        }
    }
}
