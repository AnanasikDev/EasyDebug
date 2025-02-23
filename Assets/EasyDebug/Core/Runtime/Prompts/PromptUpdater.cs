using System;
using UnityEngine;

namespace EasyDebug.Prompts
{
	public class PromptUpdater : MonoBehaviour
	{
        public static event Action onUpdateEvent;
        private void Update()
        {
            onUpdateEvent?.Invoke();
        }
    }
}