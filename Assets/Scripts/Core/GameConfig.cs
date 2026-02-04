using UnityEngine;

namespace AISpouse.Core
{
    /// <summary>
    /// 게임 설정 및 API 키 관리
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "AI Spouse/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("OpenAI 설정")]
        [Tooltip("OpenAI API 키 (환경 변수나 .env 파일에서 로드 권장)")]
        [SerializeField]
        private string _openAIApiKey = "";

        [Tooltip("사용할 GPT 모델")]
        [SerializeField]
        private string _modelName = "gpt-3.5-turbo";

        [Tooltip("최대 토큰 수")]
        [SerializeField]
        private int _maxTokens = 500;

        [Tooltip("온도 (창의성 조절, 0.0~1.0)")]
        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float _temperature = 0.7f;

        [Header("대화 설정")]
        [Tooltip("최대 대화 히스토리 수")]
        [SerializeField]
        private int _maxConversationHistory = 10;

        [Tooltip("타임아웃 시간(초)")]
        [SerializeField]
        private int _requestTimeoutSeconds = 30;

        // 공개 속성
        public string OpenAIApiKey => _openAIApiKey;
        public string ModelName => _modelName;
        public int MaxTokens => _maxTokens;
        public float Temperature => _temperature;
        public int MaxConversationHistory => _maxConversationHistory;
        public int RequestTimeoutSeconds => _requestTimeoutSeconds;

        /// <summary>
        /// API 키가 유효한지 확인
        /// </summary>
        public bool IsValidApiKey()
        {
            return !string.IsNullOrEmpty(_openAIApiKey) && _openAIApiKey.StartsWith("sk-");
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
