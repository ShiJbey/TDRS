using UnityEngine;

namespace TDRS.Demo
{
	public class RelationshipStatusSystem : MonoBehaviour
	{

		void Start()
		{
			SocialEngineController.OnRelationshipAdded += HandleRelationshipAdded;
			SocialEngineController.OnRelationshipRemoved += HandleRelationshipRemoved;
		}

		private void HandleRelationshipAdded(Relationship relationship)
		{
			relationship.OnStatChanged += HandleRelationshipStatChange;
		}

		private void HandleRelationshipRemoved(Relationship relationship)
		{
			relationship.OnStatChanged -= HandleRelationshipStatChange;
		}

		private void HandleRelationshipStatChange(
			object relationship,
			Relationship.OnStatChangedArgs args
		)
		{
			// This demo only cares about the friendship stat
			if (args.StatName != "Friendship") return;

			if (args.Value >= 0 && args.Value < 10)
			{
				(relationship as Relationship).SetRelationshipType("acquaintance");
			}
			else if (args.Value >= 10 && args.Value < 30)
			{
				(relationship as Relationship).SetRelationshipType("friend");
			}
			else if (args.Value >= 30 && args.Value < 40)
			{
				(relationship as Relationship).SetRelationshipType("good_friend");
			}
			else if (args.Value >= 40)
			{
				(relationship as Relationship).SetRelationshipType("best_friend");
			}
			else
			{
				(relationship as Relationship).SetRelationshipType("stranger");
			}

		}
	}

}
