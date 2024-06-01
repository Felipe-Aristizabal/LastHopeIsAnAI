using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using LLMUnity;
using UnityEngine.UI;

namespace LLMUnitySamples
{
    public class BossChat : MonoBehaviour
    {
        public LLMClient llm;
        public bool warmUpDone = false;
        private string lastResponse;



        void Start()
        {
            _ = llm.Warmup(WarmUpCallback);
        }

        public async Task<string> SendMessage(string message)
        {
            if (warmUpDone)
            {
                var response = await llm.Chat(message);
                lastResponse = response;
                Debug.Log("Player: " + message);
                Debug.Log("Current: " + response);

                return lastResponse;
            }
            else
            {
                Debug.LogWarning("LLM is not warmed up yet.");
                return "A mouse ate my speaker";
            }
        }

        public void WarmUpCallback()
        {
            warmUpDone = true;
            Debug.Log("I'm Ready");
        }

        public void CancelRequests()
        {
            llm.CancelRequests();
        }

        public string GetLastResponse()
        {
            return lastResponse;
        }

    }
}