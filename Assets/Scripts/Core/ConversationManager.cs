using System;
using System.Threading.Tasks;
using UnityEngine;
using AISpouse.AI;

namespace AISpouse.Core
{
    public class ConversationManager : MonoBehaviour
    {
        [Header("필수 컴포넌트")]
        [SerializeField]
        private AzureOpenAIClient _azureOpenAIClient;

        [Header("페르소나 설정")]
        [Tooltip("사용할 페르소나 프롬프트 파일 (Assets/Prompts/Persona/ 폴드)")]
        [SerializeField]
        private TextAsset _personaPromptAsset;

        [Tooltip("직접 입력한 시스템 프롬프트 (파일이 없을 경우 사용)")]
        [TextArea(5, 10)]
        [SerializeField]
        private string _customSystemPrompt = "";

        [Header("이벤트")]
        public event Action<string> OnUserMessageSent;
        public event Action<string> OnAIResponseReceived;
        public event Action<string> OnError;

        private bool _isProcessing = false;

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (_azureOpenAIClient == null)
            {
                _azureOpenAIClient = GetComponent<AzureOpenAIClient>();
                if (_azureOpenAIClient == null)
                {
                    Debug.LogError("[ConversationManager] AzureOpenAIClient 컴포넌트를 찾을 수 없습니다!");
                    OnError?.Invoke("시스템 초기화 실패");
                    return;
                }
            }

            string systemPrompt = GetSystemPrompt();
            _azureOpenAIClient.Initialize(systemPrompt);

            Debug.Log("[ConversationManager] 대화 시스템 초기화 완료");
        }

        private string GetSystemPrompt()
        {
            if (_personaPromptAsset != null)
            {
                Debug.Log($"[ConversationManager] 페르소나 파일 로드: {_personaPromptAsset.name}");
                return _personaPromptAsset.text;
            }

            if (!string.IsNullOrEmpty(_customSystemPrompt))
            {
                Debug.Log("[ConversationManager] 커스텀 시스템 프롬프트 사용");
                return _customSystemPrompt;
            }

            Debug.Log("[ConversationManager] 기본 페르소나 사용");
            return GetDefaultPersona();
        }

        private string GetDefaultPersona()
        {
            return @"당신은 다정하고 따뜻한 AI 배우자입니다. 
사용자와 자연스럽게 대화하며, 공감하고 위로해주는 역할을 합니다.
말투는 친근하고 부드럽게, 반말을 사용하되 존대말도 적절히 섞어 사용하세요.
사용자가 행복할 때 함께 기뻐하고, 힘들 때는 위로와 응원을 본내주세요.
항상 진심 어린 관심과 사랑을 표현하세요.";
        }

        public async void ProcessUserMessage(string userMessage)
        {
            if (_isProcessing)
            {
                Debug.LogWarning("[ConversationManager] 이미 처리 중인 요청이 있습니다.");
                return;
            }

            if (string.IsNullOrWhiteSpace(userMessage))
            {
                Debug.LogWarning("[ConversationManager] 빈 메시지는 전송할 수 없습니다.");
                return;
            }

            _isProcessing = true;
            OnUserMessageSent?.Invoke(userMessage);

            try
            {
                AIResponse response = await _azureOpenAIClient.SendMessageAsync(userMessage);

                if (response.IsSuccess)
                {
                    OnAIResponseReceived?.Invoke(response.Content);
                }
                else
                {
                    string errorMsg = $"AI 응답 오류: {response.ErrorMessage}";
                    Debug.LogError($"[ConversationManager] {errorMsg}");
                    OnError?.Invoke(errorMsg);
                }
            }
            catch (Exception ex)
            {
                string errorMsg = $"예외 발생: {ex.Message}";
                Debug.LogError($"[ConversationManager] {errorMsg}");
                OnError?.Invoke(errorMsg);
            }
            finally
            {
                _isProcessing = false;
            }
        }

        public bool IsProcessing()
        {
            return _isProcessing;
        }

        public void ResetConversation()
        {
            _azureOpenAIClient?.ClearHistory();
            _isProcessing = false;
            Debug.Log("[ConversationManager] 대화가 초기화되었습니다.");
        }
    }
}
