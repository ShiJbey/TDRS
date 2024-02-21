using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDRS.Demo
{
	[DefaultExecutionOrder(5)]
	public class RelationshipStatusDemo : MonoBehaviour
	{
		[SerializeField]
		private AgentController m_agent;

		// Start is called before the first frame update
		void Start()
		{
			SocialEngineController.Instance.Initialize();
			SocialEngineController.Instance.RegisterAgentsAndRelationships();
		}

		// Update is called once per frame
		void Update()
		{

		}

		public void IncreaseFriendship(int amount)
		{
			SocialEngineController.Instance.State
				.GetRelationship(m_agent.UID, "player")
				.Stats.GetStat("Friendship").BaseValue += amount;
		}
	}

}
