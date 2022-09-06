using NUnit.Framework;
using TraitBasedOpinionSystem.Core;

namespace TraitBasedOpinionSystem.Test
{
    [TestFixture]
    public class TestTraitType
    {
        [SetUp]
        public void Setup()
        {
            TraitTypeLibrary.AddTrait(new TraitType("Generous"));
            TraitTypeLibrary.AddTrait(new TraitType("Stingy"));
            TraitTypeLibrary.AddTrait(new TraitType("Masculine"));
            TraitTypeLibrary.AddTrait(new TraitType("Feminine"));
            TraitTypeLibrary.AddTrait(new TraitType("Mysoginist"));
            TraitTypeLibrary.AddTrait(new TraitType("Friendly"));
        }

        [Test]
        public void TestInstantiateTrait()
        {
            var traitType = TraitTypeLibrary.GetTrait("Generous");
            Assert.Fail();
        }
    }
}
