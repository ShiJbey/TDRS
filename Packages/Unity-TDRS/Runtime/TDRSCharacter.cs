using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDRS
{
	public class TDRSCharacter : MonoBehaviour
	{
		public string entityID = "";
		public List<string> traitsAtStart = new List<string>();

		// Start is called before the first frame update
		void Start()
		{
			foreach (var traitID in traitsAtStart)
			{
				TDRSManager.Instance.AddTraitToNode(entityID, traitID);
			}
		}

		// Update is called once per frame
		void Update()
		{

		}
	}

}
