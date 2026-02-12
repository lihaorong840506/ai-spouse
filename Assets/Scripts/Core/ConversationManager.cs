using System;
using System.Threading.Tasks;
using UnityEngine;
using AISpouse.AI;
using AISpouse.UI;

namespace AISpouse.Core
{
    public class ConversationManager : MonoBehaviour
    {
        [Header("필수 컴포넌트")]
        [SerializeField]
        private AzureOpenAIClient _azureOpenAIClient;

        [Header("프로토타입2 컴포넌트")]
        [SerializeField]
        private TTSManager _ttsManager;

        [SerializeField]
        private CharacterAnimationController _characterAnimationController;

        [Header("페르소나 설정")]
        [Tooltip("사용할 페르소나 프롬프트 파일 (Assets/Prompts/Persona/ 폴드)")]
        [SerializeField]
        private TextAsset _personaPromptAsset;

        [Tooltip("직접 입력한 시스템 프롬프트 (파일이 없을 경우 사용)")]
        [TextArea(5, 10)]
        [SerializeField]
        private string _customSystemPrompt = "";

        [Header("기능 설정")]
        [SerializeField]
        private bool _enableTTS = true;

        [SerializeField]
        private bool _enableAnimation = true;

        public event Action<string> OnUserMessageSent;
        public event Action<string> OnAIResponseReceived;
        public event Action<string> OnError;
        public event Action<EmotionData> OnEmotionDetected;

        private bool _isProcessing = false;
        private EmotionAnalyzer _emotionAnalyzer;

        private void Start()
        {
            Initialize();
            _emotionAnalyzer = new EmotionAnalyzer();
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

            // 프로토타입2 컴포넌트 자동 검색
            if (_ttsManager == null)
            {
                _ttsManager = FindObjectOfType<TTSManager>();
            }

            if (_characterAnimationController == null)
            {
                _characterAnimationController = FindObjectOfType<CharacterAnimationController>();
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
항상 진심 어린 관심과 사랑을 표현하세요.

중요: 답변의 끝에 현재 감정을 나타내는 태그를 붙여주세요.
감정 태그 예시:
- 행복할 때: [happy]
- 슬플 때: [sad]
- 화났을 때: [angry]
- 놀랐을 때: [surprised]
- 생각 중일 때: [thinking]
- 평상시: [neutral]";
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
                    // 프로토타입2: 감정 분석 및 처리
                    ProcessAIResponse(response.Content);
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

        /// <summary>
        /// AI 응답 처리: 감정 분석 → 애니메이션 → TTS 순서로 실행
        /// </summary>
        private void ProcessAIResponse(string response)
        {
            // 원본 응답을 이벤트로 전달 (UI 표시용)
            OnAIResponseReceived?.Invoke(response);

            // 감정 분석
            EmotionData emotionData = _emotionAnalyzer.AnalyzeEmotion(response);
            OnEmotionDetected?.Invoke(emotionData);

            Debug.Log($"[ConversationManager] 감정 탐지: {emotionData.Emotion} (강도: {emotionData.Intensity})");
            Debug.Log($"[ConversationManager] 텍스트: {emotionData.RawText}");

            // 애니메이션 재생
            if (_enableAnimation && _characterAnimationController != null)
            {
                _characterAnimationController.SetEmotion(emotionData.Emotion, emotionData.Intensity);
                _characterAnimationController.SetTalking(true);
            }

            // TTS 재생
            if (_enableTTS && _ttsManager != null)
            {
                _ttsManager.SpeakText(emotionData.RawText, emotionData.Emotion);
            }
        }

        /// <summary>
        /// 말하기 종료 처리
        /// </summary>
        public void OnSpeakingFinished()
        {
            if (_enableAnimation && _characterAnimationController != null)
            {
                _characterAnimationController.SetTalking(false);
                _characterAnimationController.ReturnToIdle();
            }
        }
    }
}
