using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AISpouse.AI
{
    /// <summary>
    /// OpenAI API 통신 클라이언트
    /// </summary>
    public class OpenAIClient : MonoBehaviour
    {
        [SerializeField]
        private Core.GameConfig _gameConfig;

        private const string OPENAI_API_URL = "https://api.openai.com/v1/chat/completions";
        private List<ChatMessage> _conversationHistory = new List<ChatMessage>();

        /// <summary>
        /// 대화 초기화 (시스템 프롬프트 설정)
        /// </summary>
        public void Initialize(string systemPrompt)
        {
            _conversationHistory.Clear();
            
            if (!string.IsNullOrEmpty(systemPrompt))
            {
                _conversationHistory.Add(new ChatMessage
                {
                    role = "system",
                    content = systemPrompt
                });
            }

            Debug.Log("[OpenAIClient] 시스템 프롬프트 설정 완료");
        }

        /// <summary>
        /// 사용자 메시지를 전송하고 AI 응답을 받습니다
        /// </summary>
        public async Task<AIResponse> SendMessageAsync(string userMessage)
        {
            if (_gameConfig == null)
            {
                Debug.LogError("[OpenAIClient] GameConfig가 설정되지 않았습니다!");
                return AIResponse.Failure("설정 오류");
            }

            if (!_gameConfig.IsValidApiKey())
            {
                Debug.LogError("[OpenAIClient] 유효하지 않은 API 키입니다!");
                return AIResponse.Failure("API 키 오류");
            }

            // 사용자 메시지를 히스토리에 추가
            _conversationHistory.Add(new ChatMessage
            {
                role = "user",
                content = userMessage
            });

            // 히스토리 크기 제한
            TrimConversationHistory();

            try
            {
                var requestBody = new OpenAIRequest
                {
                    model = _gameConfig.ModelName,
                    messages = _conversationHistory.ToArray(),
                    max_tokens = _gameConfig.MaxTokens,
                    temperature = _gameConfig.Temperature
                };

                string jsonBody = JsonConvert.SerializeObject(requestBody, Formatting.None);
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

                using (UnityWebRequest request = new UnityWebRequest(OPENAI_API_URL, "POST"))
                {
                    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    request.downloadHandler = new DownloadHandlerBuffer();
                    request.SetRequestHeader("Content-Type", "application/json");
                    request.SetRequestHeader("Authorization", $"Bearer {_gameConfig.OpenAIApiKey}");
                    request.timeout = _gameConfig.RequestTimeoutSeconds;

                    Debug.Log($"[OpenAIClient] 요청 전송: {userMessage.Substring(0, Math.Min(50, userMessage.Length))}...");

                    // 비동기 요청
                    var operation = request.SendWebRequest();
                    while (!operation.isDone)
                    {
                        await Task.Yield();
                    }

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        string responseText = request.downloadHandler.text;
                        var response = ParseResponse(responseText);
                        
                        // AI 응답을 히스토리에 추가
                        _conversationHistory.Add(new ChatMessage
                        {
                            role = "assistant",
                            content = response.Content
                        });

                        Debug.Log($"[OpenAIClient] 응답 수신: {response.Content.Substring(0, Math.Min(50, response.Content.Length))}...");
                        
                        return AIResponse.Success(response.Content);
                    }
                    else
                    {
                        string error = $"API 요청 실패: {request.error}";
                        Debug.LogError($"[OpenAIClient] {error}");
                        Debug.LogError($"[OpenAIClient] 응답: {request.downloadHandler.text}");
                        return AIResponse.Failure(error);
                    }
                }
            }
            catch (Exception ex)
            {
                string error = $"예외 발생: {ex.Message}";
                Debug.LogError($"[OpenAIClient] {error}");
                return AIResponse.Failure(error);
            }
        }

        /// <summary>
        /// 대화 히스토리 크기 제한
        /// </summary>
        private void TrimConversationHistory()
        {
            // 시스템 메시지 제외하고 카운트
            int nonSystemCount = 0;
            int systemMessagesCount = 0;

            for (int i = 0; i < _conversationHistory.Count; i++)
            {
                if (_conversationHistory[i].role == "system")
                {
                    systemMessagesCount++;
                }
                else
                {
                    nonSystemCount++;
                }
            }

            // 최대 히스토리를 초과하면 오래된 메시지 제거 (시스템 메시지 제외)
            while (nonSystemCount > _gameConfig.MaxConversationHistory)
            {
                for (int i = 0; i < _conversationHistory.Count; i++)
                {
                    if (_conversationHistory[i].role != "system")
                    {
                        _conversationHistory.RemoveAt(i);
                        nonSystemCount--;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// OpenAI 응답 파싱
        /// </summary>
        private OpenAIResponseData ParseResponse(string jsonResponse)
        {
            try
            {
                var jObject = JObject.Parse(jsonResponse);
                var firstChoice = jObject["choices"]?[0];
                var message = firstChoice?["message"];

                return new OpenAIResponseData
                {
                    Content = message?["content"]?.ToString() ?? "응답을 받지 못했습니다."
                };
            }
            catch (Exception ex)
            {
                Debug.LogError($"[OpenAIClient] 응답 파싱 오류: {ex.Message}");
                return new OpenAIResponseData { Content = "응답 파싱 오류" };
            }
        }

        /// <summary>
        /// 현재 대화 히스토리 반환
        /// </summary>
        public IReadOnlyList<ChatMessage> GetConversationHistory()
        {
            return _conversationHistory.AsReadOnly();
        }

        /// <summary>
        /// 대화 히스토리 초기화
        /// </summary>
        public void ClearHistory()
        {
            string systemPrompt = null;
            
            // 시스템 프롬프트 보존
            foreach (var msg in _conversationHistory)
            {
                if (msg.role == "system")
                {
                    systemPrompt = msg.content;
                    break;
                }
            }

            _conversationHistory.Clear();
            
            if (!string.IsNullOrEmpty(systemPrompt))
            {
                _conversationHistory.Add(new ChatMessage
                {
                    role = "system",
                    content = systemPrompt
                });
            }

            Debug.Log("[OpenAIClient] 대화 히스토리 초기화 완료");
        }
    }

    /// <summary>
    /// API 요청 데이터 구조
    /// </summary>
    [Serializable]
    public class OpenAIRequest
    {
        public string model;
        public ChatMessage[] messages;
        public int max_tokens;
        public float temperature;
    }

    /// <summary>
    /// 채팅 메시지 구조
    /// </summary>
    [Serializable]
    public class ChatMessage
    {
        public string role;
        public string content;
    }

    /// <summary>
    /// OpenAI 응답 데이터
    /// </summary>
    public class OpenAIResponseData
    {
        public string Content { get; set; }
    }

    /// <summary>
    /// AI 응답 결과
    /// </summary>
    public class AIResponse
    {
        public bool IsSuccess { get; private set; }
        public string Content { get; private set; }
        public string ErrorMessage { get; private set; }

        public static AIResponse Success(string content)
        {
            return new AIResponse
            {
                IsSuccess = true,
                Content = content,
                ErrorMessage = null
            };
        }

        public static AIResponse Failure(string error)
        {
            return new AIResponse
            {
                IsSuccess = false,
                Content = null,
                ErrorMessage = error
            };
        }
    }
}
