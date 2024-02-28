using UnityEditor;

namespace TDRS
{
	public class ConfigFileAsset
	{
		[MenuItem("Assets/Create/TDRS/Traits YAML", false, 1)]
		private static void CreateNewTraitDefinitionsYaml()
		{
			ProjectWindowUtil.CreateAssetWithContent(
				"traits.yaml",
				@"# traits YAML (replace the data below)
- traitID: friendly
  traitType: agent
  displayName: Friendly
  description: ""[owner] is friendly""
  conflictingTraits:
    - jerk

- traitID: friend
  traitType: relationship
  displayName: Friend
  description: ""[owner] considers [target] to be a friend""
"
				);
		}

		[MenuItem("Assets/Create/TDRS/Social Events YAML", false, 1)]
		private static void CreateNewSocialEventsYaml()
		{
			ProjectWindowUtil.CreateAssetWithContent(
				"social_events.yaml",
				@"# social events YAML (replace the data below)
- name: Betrayal
  roles:
    - ""?betrayer""
    - ""?victim""
  description: ""[betrayer] betrayed [victim].""
  responses:
    - effects:
        - ""DecreaseRelationshipStat ?victim ?betrayer Friendship 10 5""
        - ""RemoveRelationshipTrait ?victim ?betrayer friend""
    - preconditions:
        - ""?victim.relationships.?victim_friend.traits.friend""
        - ""neq ?victim_friend ?betrayer""
      effects:
        - ""DecreaseRelationshipStat ?victim_friend ?betrayer Friendship 5 6""
    - preconditions:
        - ""?victim.relationships.?victim_family.traits.family""
        - ""neq ?victim_family ?betrayer""
      effects:
        - ""AddRelationshipTrait ?victim_family ?betrayer angry_at 10""

- name: BecomeFriends
  roles:
    - ""?character_a""
    - ""?character_b""
  description: ""[character_a] and [character_b] became friends.""
  responses:
    - effects:
        - ""AddRelationshipTrait ?character_a ?character_b friend""
        - ""AddRelationshipTrait ?character_b ?character_a friend""
"
				);
		}

		[MenuItem("Assets/Create/TDRS/Social Rules YAML", false, 1)]
		private static void CreateNewSocialRulesYaml()
		{
			ProjectWindowUtil.CreateAssetWithContent(
				"social_rules.yaml",
				@"# social rules YAML (replace the data below)
- description: ""Capulets dislike Montagues""
  precondition:
    - ""?owner.traits.capulet""
    - ""?target.traits.montague""
  modifiers:
    - statName: Friendship
      value: -25

- description: ""Montagues dislike Capulets""
  precondition:
    - ""?owner.traits.montague""
    - ""?target.traits.capulet""
  modifiers:
    - statName: Friendship
      value: -25
"
				);
		}
	}
}
