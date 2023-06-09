﻿using UnityEngine;

namespace LightGive
{
	[CreateAssetMenu(menuName = CreatorPath, fileName = CreatorName)]
	public class ManagerCreator : ScriptableObject
	{
		public const string CreatorName = "ManagerCreator";
		public const string CreatorPath = "LightGive/Create ManagerCreator";

		[SerializeField]
		private GameObject[] m_createManagers;
		public GameObject[] createManagers { get { return m_createManagers; } }

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void InitializeBeforeSceneLoad()
		{
			var managerCreator = Resources.Load<ManagerCreator>("ManagerCreator");
			if (managerCreator == null)
			{
				Debug.Log("Manager Creator does not exist.\nIn project view Create/" + ManagerCreator.CreatorPath + " from generate.");
				return;
			}

			string objectNames = "";
			for (int i = 0; i < managerCreator.createManagers.Length; i++)
			{
				if (managerCreator.createManagers[i] == null)
					continue;
				var obj = Instantiate(managerCreator.createManagers[i]);
				objectNames += "\n" + (i + 1).ToString("0") + "." + obj.name + ",";
			}
		}
	}
}