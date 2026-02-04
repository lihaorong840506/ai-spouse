using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AISpouse.Core;

namespace AISpouse.UI
{
    public class ChatUI : MonoBehaviour
    {
        [Header("UI 참조 (자동 검색됨)")]
        [SerializeField]
        private TMP_InputField _inputField;

        [SerializeField]
        private Button _sendButton;

        [SerializeField]
        private ScrollRect _scrollRect;

        [SerializeField]
        private Transform _messageContainer;

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
            
            if (_conversationManager == null)
            {
                Debug.LogError("[ChatUI] ConversationManager를 찾을 수 없습니다! GameObject에 ConversationManager 컴포넌트를 추가하세요.");
            }
        }

        private void SetupReferences()
        {
            _conversationManager = FindObjectOfType<ConversationManager>();

            if (_inputField == null)
            {
                _inputField = GameObject.Find("InputField")?.GetComponent<TMP_InputField>();
            }

            if (_sendButton == null)
            {
                _sendButton = GameObject.Find("SendButton")?.GetComponent<Button>();
            }

            if (_scrollRect == null)
            {
                _scrollRect = GameObject.Find("Scroll View")?.GetComponent<ScrollRect>();
            }

            if (_messageContainer == null)
            {
                var scrollView = GameObject.Find("Scroll View");
                if (scrollView != null)
                {
                    _messageContainer = scrollView.GetComponentInChildren<VerticalLayoutGroup>()?.transform;
                }
            }

            if (_inputField == null)
                Debug.LogError("[ChatUI] InputField를 찾을 수 없습니다. TMP_InputField가 있는 GameObject를 'InputField'라고 이름지으세요.");
            if (_sendButton == null)
                Debug.LogWarning("[ChatUI] SendButton을 찾을 수 없습니다.");

            if (_conversationManager != null)
            {
                _conversationManager.OnUserMessageSent += OnUserMessageSent;
                _conversationManager.OnAIResponseReceived += OnAIResponseReceived;
                _conversationManager.OnError += OnError;
            }
        }

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

            _conversationManager.ProcessUserMessage(message);

            if (_clearInputAfterSend)
            {
                _inputField.text = "";
                _inputField.ActivateInputField();
            }

            if (_disableInputWhileProcessing)
            {
                SetInputInteractable(false);
            }
        }

        private void OnUserMessageSent(string message)
        {
            AddMessageToUI(message, true);
            Debug.Log($"[ChatUI] 사용자 메시지: {message}");
        }

        private void OnAIResponseReceived(string response)
        {
            AddMessageToUI(response, false);
            
            if (_disableInputWhileProcessing)
            {
                SetInputInteractable(true);
            }

            ScrollToBottom();
        }

        private void OnError(string error)
        {
            AddSystemMessage($"오류: {error}");
            
            if (_disableInputWhileProcessing)
            {
                SetInputInteractable(true);
            }
        }

        private void AddMessageToUI(string message, bool isUser)
        {
            if (_messageContainer == null)
            {
                Debug.LogError("[ChatUI] MessageContainer가 설정되지 않았습니다!");
                return;
            }

            CreateSimpleMessage(message, isUser);
            ScrollToBottom();
        }

        private void CreateSimpleMessage(string message, bool isUser)
        {
            GameObject textObj = new GameObject(isUser ? "UserMessage" : "AIMessage");
            textObj.transform.SetParent(_messageContainer, false);

            TMP_Text textComponent = textObj.AddComponent<TextMeshProUGUI>();
            textComponent.text = message;
            textComponent.fontSize = 16;
            textComponent.color = isUser ? new Color(0.2f, 0.4f, 1f) : new Color(0.2f, 0.8f, 0.2f);
            textComponent.alignment = isUser ? TextAlignmentOptions.Right : TextAlignmentOptions.Left;

            ContentSizeFitter fitter = textObj.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

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

        private void ScrollToBottom()
        {
            if (_scrollRect == null) return;

            Canvas.ForceUpdateCanvases();
            _scrollRect.verticalNormalizedPosition = 0f;
        }

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
