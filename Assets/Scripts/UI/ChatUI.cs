using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AISpouse.Core;

namespace AISpouse.UI
{
    /// <summary>
    /// 채팅 UI 관리자
    /// </summary>
    public class ChatUI : MonoBehaviour
    {
        [Header("UI 참조")]
        [SerializeField]
        private TMP_InputField _inputField;

        [SerializeField]
        private Button _sendButton;

        [SerializeField]
        private ScrollRect _scrollRect;

        [SerializeField]
        private Transform _messageContainer;

        [SerializeField]
        private GameObject _userMessagePrefab;

        [SerializeField]
        private GameObject _aiMessagePrefab;

        [Header("기능 설정")]
        [SerializeField]
        private bool _clearInputAfterSend = true;

        [SerializeField]
        private bool _disableInputWhileProcessing = true;

        private ConversationManager _conversationManager;

        private void Start()
        {
            SetupReferences();
            SetupEventListeners();
        }

        /// <summary>
        /// 컴포넌트 참조 설정
        /// </summary>
        private void SetupReferences()
        {
            _conversationManager = FindObjectOfType<ConversationManager>();

            if (_conversationManager == null)
            {
                Debug.LogError("[ChatUI] ConversationManager를 찾을 수 없습니다!");
                return;
            }

            if (_inputField == null)
            {
                _inputField = GameObject.Find("InputField")?.GetComponent<TMP_InputField>();
            }

            if (_sendButton == null)
            {
                _sendButton = GameObject.Find("SendButton")?.GetComponent<Button>();
            }

            // 컴포넌트 검증
            if (_inputField == null)
                Debug.LogError("[ChatUI] InputField가 설정되지 않았습니다!");
            if (_sendButton == null)
                Debug.LogWarning("[ChatUI] SendButton이 설정되지 않았습니다.");

            // 이벤트 연결
            _conversationManager.OnUserMessageSent += OnUserMessageSent;
            _conversationManager.OnAIResponseReceived += OnAIResponseReceived;
            _conversationManager.OnError += OnError;
        }

        /// <summary>
        /// UI 이벤트 리스너 설정
        /// </summary>
        private void SetupEventListeners()
        {
            if (_sendButton != null)
            {
                _sendButton.onClick.AddListener(SendMessage);
            }

            if (_inputField != null)
            {
                _inputField.onSubmit.AddListener(_ => SendMessage());
            }
        }

        /// <summary>
        /// 메시지 전송
        /// </summary>
        public void SendMessage()
        {
            if (_conversationManager == null || _inputField == null)
            {
                Debug.LogError("[ChatUI] 필수 컴포넌트가 없습니다!");
                return;
            }

            if (_conversationManager.IsProcessing())
            {
                Debug.LogWarning("[ChatUI] 현재 처리 중인 메시지가 있습니다.");
                return;
            }

            string message = _inputField.text.Trim();
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            // 메시지 처리
            _conversationManager.ProcessUserMessage(message);

            // 입력 필드 처리
            if (_clearInputAfterSend)
            {
                _inputField.text = "";
                _inputField.ActivateInputField();
            }

            // 입력 비활성화 (처리 중)
            if (_disableInputWhileProcessing)
            {
                SetInputInteractable(false);
            }
        }

        /// <summary>
        /// 사용자 메시지 전송 이벤트
        /// </summary>
        private void OnUserMessageSent(string message)
        {
            AddMessageToUI(message, true);
            Debug.Log($"[ChatUI] 사용자 메시지: {message}");
        }

        /// <summary>
        /// AI 응답 수신 이벤트
        /// </summary>
        private void OnAIResponseReceived(string response)
        {
            AddMessageToUI(response, false);
            
            // 입력 다시 활성화
            if (_disableInputWhileProcessing)
            {
                SetInputInteractable(true);
            }

            // 스크롤을 최하단으로
            ScrollToBottom();
        }

        /// <summary>
        /// 오류 발생 이벤트
        /// </summary>
        private void OnError(string error)
        {
            AddSystemMessage($"오류: {error}");
            
            // 입력 다시 활성화
            if (_disableInputWhileProcessing)
            {
                SetInputInteractable(true);
            }
        }

        /// <summary>
        /// 메시지를 UI에 추가
        /// </summary>
        private void AddMessageToUI(string message, bool isUser)
        {
            if (_messageContainer == null)
            {
                Debug.LogError("[ChatUI] MessageContainer가 설정되지 않았습니다!");
                return;
            }

            GameObject prefab = isUser ? _userMessagePrefab : _aiMessagePrefab;
            
            if (prefab == null)
            {
                // 프리팹이 없으면 기본 텍스트로 추가
                CreateSimpleMessage(message, isUser);
                return;
            }

            GameObject messageObj = Instantiate(prefab, _messageContainer);
            TMP_Text textComponent = messageObj.GetComponentInChildren<TMP_Text>();
            
            if (textComponent != null)
            {
                textComponent.text = message;
            }

            // 스크롤을 최하단으로
            ScrollToBottom();
        }

        /// <summary>
        /// 간단한 메시지 생성 (프리팹 없을 때)
        /// </summary>
        private void CreateSimpleMessage(string message, bool isUser)
        {
            GameObject textObj = new GameObject(isUser ? "UserMessage" : "AIMessage");
            textObj.transform.SetParent(_messageContainer, false);

            TMP_Text textComponent = textObj.AddComponent<TextMeshProUGUI>();
            textComponent.text = message;
            textComponent.fontSize = 14;
            textComponent.color = isUser ? Color.blue : Color.green;
            textComponent.alignment = isUser ? TextAlignmentOptions.Right : TextAlignmentOptions.Left;

            // 레이아웃 컴포넌트 추가
            ContentSizeFitter fitter = textObj.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        /// <summary>
        /// 시스템 메시지 추가
        /// </summary>
        private void AddSystemMessage(string message)
        {
            if (_messageContainer == null) return;

            GameObject textObj = new GameObject("SystemMessage");
            textObj.transform.SetParent(_messageContainer, false);

            TMP_Text textComponent = textObj.AddComponent<TextMeshProUGUI>();
            textComponent.text = message;
            textComponent.fontSize = 12;
            textComponent.color = Color.red;
            textComponent.alignment = TextAlignmentOptions.Center;

            ScrollToBottom();
        }

        /// <summary>
        /// 스크롤을 최하단으로 이동
        /// </summary>
        private void ScrollToBottom()
        {
            if (_scrollRect == null) return;

            // 다음 프레임에 스크롤 (레이아웃 업데이트 후)
            Canvas.ForceUpdateCanvases();
            _scrollRect.verticalNormalizedPosition = 0f;
        }

        /// <summary>
        /// 입력 상호작용 설정
        /// </summary>
        private void SetInputInteractable(bool interactable)
        {
            if (_inputField != null)
            {
                _inputField.interactable = interactable;
            }
            if (_sendButton != null)
            {
                _sendButton.interactable = interactable;
            }
        }

        private void OnDestroy()
        {
            if (_conversationManager != null)
            {
                _conversationManager.OnUserMessageSent -= OnUserMessageSent;
                _conversationManager.OnAIResponseReceived -= OnAIResponseReceived;
                _conversationManager.OnError -= OnError;
            }

            if (_sendButton != null)
            {
                _sendButton.onClick.RemoveListener(SendMessage);
            }

            if (_inputField != null)
            {
                _inputField.onSubmit.RemoveAllListeners();
            }
        }
    }
}
