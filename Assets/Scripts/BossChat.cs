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

        [SerializeField] private Text IAdolfResponse;

        public LLMClient llm;
        private bool warmUpDone = false;
        private string lastResponse;



        void Start()
        {
            _ = llm.Warmup(WarmUpCallback);
        }

        public async void SendMessage(string message)
        {
            if (warmUpDone)
            {
                var response = await llm.Chat(message);
                lastResponse = response;
                // Debug.Log("Player: " + message);
                Debug.Log("lastResponse: " + lastResponse);
                Debug.Log("Current: " + response);
                IAdolfResponse.text = response;
            }
            else
            {
                Debug.LogWarning("LLM is not warmed up yet.");
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