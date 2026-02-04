using UnityEngine;

namespace AISpouse.Core
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "AI Spouse/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Azure OpenAI 설정")]
        [Tooltip("Azure OpenAI 엔드포인트 URL")]
        [SerializeField]
        private string _azureEndpoint = "https://luoji-ai-azure.cognitiveservices.azure.com";

        [Tooltip("Azure OpenAI API 키")]
        [SerializeField]
        private string _azureApiKey = "";

        [Tooltip("Azure OpenAI API 버전")]
        [SerializeField]
        private string _apiVersion = "2024-12-01-preview";

        [Tooltip("Azure 배포 이름 (Deployment Name)")]
        [SerializeField]
        private string _deploymentName = "gpt-4o";

        [Header("대화 설정")]
        [Tooltip("최대 토큰 수")]
        [SerializeField]
        private int _maxTokens = 500;

        [Tooltip("온도 (창의성 조절, 0.0~1.0)")]
        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float _temperature = 0.7f;

        [Tooltip("최대 대화 히스토리 수")]
        [SerializeField]
        private int _maxConversationHistory = 10;

        [Tooltip("타임아웃 시간(초)")]
        [SerializeField]
        private int _requestTimeoutSeconds = 30;

        public string AzureEndpoint => _azureEndpoint;
        public string AzureApiKey => _azureApiKey;
        public string ApiVersion => _apiVersion;
        public string DeploymentName => _deploymentName;
        public int MaxTokens => _maxTokens;
        public float Temperature => _temperature;
        public int MaxConversationHistory => _maxConversationHistory;
        public int RequestTimeoutSeconds => _requestTimeoutSeconds;

        public bool IsValidConfig()
        {
            return !string.IsNullOrEmpty(_azureApiKey) && 
                   !string.IsNullOrEmpty(_azureEndpoint) && 
                   !string.IsNullOrEmpty(_deploymentName);
        }

        private void OnValidate()
        {
            if (_maxTokens < 1)
                _maxTokens = 1;
            if (_maxTokens > 4000)
                _maxTokens = 4000;
        }
    }
}
