using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace AISpouse.AI
{
    public class AzureOpenAIClient : MonoBehaviour
    {
        [SerializeField]
        private Core.GameConfig _gameConfig;

        private List<ChatMessage> _conversationHistory = new List<ChatMessage>();

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

            Debug.Log("[AzureOpenAIClient] 시스템 프롬프트 설정 완료");
        }

        public async Task<AIResponse> SendMessageAsync(string userMessage)
        {
            if (_gameConfig == null)
            {
                Debug.LogError("[AzureOpenAIClient] GameConfig가 설정되지 않았습니다!");
                return AIResponse.Failure("설정 오류");
            }

            if (!_gameConfig.IsValidConfig())
            {
                Debug.LogError("[AzureOpenAIClient] 유효하지 않은 설정입니다!");
                return AIResponse.Failure("설정 오류: API 키와 엔드포인트를 확인하세요");
            }

            _conversationHistory.Add(new ChatMessage
            {
                role = "user",
                content = userMessage
            });

            TrimConversationHistory();

            try
            {
                var requestBody = new AzureOpenAIRequest
                {
                    messages = _conversationHistory.ToArray(),
                    max_tokens = _gameConfig.MaxTokens,
                    temperature = _gameConfig.Temperature
                };

                string jsonBody = JsonUtility.ToJson(requestBody);
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

                string url = $"{_gameConfig.AzureEndpoint}/openai/deployments/{_gameConfig.DeploymentName}/chat/completions?api-version={_gameConfig.ApiVersion}";

                using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
                {
                    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    request.downloadHandler = new DownloadHandlerBuffer();
                    request.SetRequestHeader("Content-Type", "application/json");
                    request.SetRequestHeader("api-key", _gameConfig.AzureApiKey);
                    request.timeout = _gameConfig.RequestTimeoutSeconds;

                    Debug.Log($"[AzureOpenAIClient] 요청 전송: {userMessage.Substring(0, Math.Min(50, userMessage.Length))}...");

                    var operation = request.SendWebRequest();
                    while (!operation.isDone)
                    {
                        await Task.Yield();
                    }

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        string responseText = request.downloadHandler.text;
                        var response = ParseResponse(responseText);
                        
                        _conversationHistory.Add(new ChatMessage
                        {
                            role = "assistant",
                            content = response.Content
                        });

                        Debug.Log($"[AzureOpenAIClient] 응답 수신: {response.Content.Substring(0, Math.Min(50, response.Content.Length))}...");
                        
                        return AIResponse.Success(response.Content);
                    }
                    else
                    {
                        string error = $"API 요청 실패: {request.error}";
                        Debug.LogError($"[AzureOpenAIClient] {error}");
                        Debug.LogError($"[AzureOpenAIClient] 응답: {request.downloadHandler.text}");
                        return AIResponse.Failure(error);
                    }
                }
            }
            catch (Exception ex)
            {
                string error = $"예외 발생: {ex.Message}";
                Debug.LogError($"[AzureOpenAIClient] {error}");
                return AIResponse.Failure(error);
            }
        }

        private void TrimConversationHistory()
        {
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

        private OpenAIResponseData ParseResponse(string jsonResponse)
        {
            try
            {
                var response = JsonUtility.FromJson<AzureOpenAIResponse>(jsonResponse);
                
                if (response.choices != null && response.choices.Length > 0)
                {
                    return new OpenAIResponseData
                    {
                        Content = response.choices[0].message.content
                    };
                }
                
                return new OpenAIResponseData { Content = "응답을 받지 못했습니다." };
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AzureOpenAIClient] 응답 파싱 오류: {ex.Message}");
                return new OpenAIResponseData { Content = "응답 파싱 오류" };
            }
        }

        public IReadOnlyList<ChatMessage> GetConversationHistory()
        {
            return _conversationHistory.AsReadOnly();
        }

        public void ClearHistory()
        {
            string systemPrompt = null;
            
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

            Debug.Log("[AzureOpenAIClient] 대화 히스토리 초기화 완료");
        }
    }

    [Serializable]
    public class AzureOpenAIRequest
    {
        public ChatMessage[] messages;
        public int max_tokens;
        public float temperature;
    }

    [Serializable]
    public class AzureOpenAIResponse
    {
        public Choice[] choices;
    }

    [Serializable]
    public class Choice
    {
        public Message message;
    }

    [Serializable]
    public class Message
    {
        public string content;
    }

    [Serializable]
    public class ChatMessage
    {
        public string role;
        public string content;
    }

    public class OpenAIResponseData
    {
        public string Content { get; set; }
    }

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
